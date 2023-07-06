Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Public Class SVNStatus
    Public fp(0) As filePpty
    'Public statError(0) As statusError

    Public Function Clone() As SVNStatus
        Dim myClone As SVNStatus = DirectCast(Me.MemberwiseClone(), SVNStatus)
        'myClone.fp = New filePpty
        'myClone.statError = New statusError
        Return myClone
    End Function
    Structure filePpty
        Public filename As String
        Public modDoc As ModelDoc2
        Public bReconnect As Boolean
        Public revertUpdate As getLatestType
        ' Each 1-9 is a one character string
        ' See http://svnbook.red-bean.com/en/1.8/svn.ref.svn.c.status.html
        Public addDelChg1 As String
        Public pptyMods2 As String
        Public workingDirLock3 As String
        Public addWithHist4 As String
        Public switchWParent5 As String
        Public lock6 As String
        Public tree7 As String
        'col 8 is blank
        Public upToDate9 As String
    End Structure
    'Structure statusError
    '    Public sMessage As String
    '    Public sFile As String
    '    Public modDocArr As ModelDoc2
    'End Structure
    Sub addOutputLineToSVNStatus(ByRef sOutputLine As String,
                                 ByRef j As Integer,
                                 ByRef sFilePathTemp As String,
                                 ByRef modDoc As ModelDoc2,
                                 Optional ByVal bCheckServer As Boolean = False)
        fp(j).filename = sFilePathTemp
        fp(j).modDoc = modDoc

        fp(j).addDelChg1 = sOutputLine.Substring(0, 1)
        fp(j).pptyMods2 = sOutputLine.Substring(1, 1)
        fp(j).workingDirLock3 = sOutputLine.Substring(2, 1)
        fp(j).addWithHist4 = sOutputLine.Substring(3, 1)
        fp(j).switchWParent5 = sOutputLine.Substring(4, 1)
        fp(j).lock6 = sOutputLine.Substring(5, 1)
        fp(j).tree7 = sOutputLine.Substring(6, 1)
        'col 8 is blank
        If bCheckServer Then
            fp(j).upToDate9 = sOutputLine.Substring(8, 1)
        Else
            fp(j).upToDate9 = "NoUpdate"
        End If
        fp(j).revertUpdate = getLatestType.none
    End Sub
    Sub setReadWriteFromLockStatus()
        Dim i As Integer
        'Dim sw = New Stopwatch()
        'sw.Start()

        For i = 0 To UBound(fp)
            If fp(i).modDoc Is Nothing Then Continue For
            If fp(i).lock6 = "K" Then
                ' The user got a lock! Let's change to write access
                fp(i).modDoc.SetReadOnlyState(False)
                fp(i).bReconnect = False
            ElseIf fp(i).addDelChg1 = "?" Then
                ' The file is not on the vault. Set to read/write
                fp(i).modDoc.SetReadOnlyState(False)
                fp(i).bReconnect = False
            Else
                'User didn't get a lock.
                fp(i).modDoc.SetReadOnlyState(True)
                fp(i).bReconnect = True
            End If
        Next
        'sw.Stop()
        'Debug.WriteLine("setReadWriteFromLockStatus Time Taken: " + sw.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"))
    End Sub
    Public Function sFilterGetLatestType(ByRef filter As getLatestType, Optional ByVal bIgnoreUpdate As Boolean = False) As String()
        'bIgnoreUpdate will ignore the filter out elements where an update is required.
        Dim sPath(UBound(fp)) As String
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            If (fp(i).revertUpdate = filter) And Not ((bIgnoreUpdate) And (fp(i).upToDate9 = "*")) Then
                sPath(j) = fp(i).filename
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Return sPath
    End Function
    Public Function sFilterUpToDate9(
                        ByRef filter As String,
                        Optional ByVal bFilterNot As Boolean = False) As String()
        'Optional ByVal pathFilterArray As String() = Nothing) As String()

        'bIgnoreUpdate will ignore the filter out elements where an update is required.
        Dim sPath(UBound(fp)) As String
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            'If findIndexContains(pathFilterArray, fp(i).filename) = -1 Then Continue For
            If ((fp(i).upToDate9 = filter) And (Not bFilterNot)) Or ((Not fp(i).upToDate9 = filter) And (bFilterNot)) Then
                sPath(j) = fp(i).filename
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Return sPath
    End Function
    Public Function sFilterChanges(ByRef filter As String) As String()
        Dim sPath(UBound(fp)) As String
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            If (fp(i).addDelChg1 = filter) Then
                sPath(j) = fp(i).filename
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Return sPath
    End Function
    Sub releaseFileSystemAccessToReadOnlyModels()
        'Even if files are read-only, they are still "in-use" by Solidworks, so the files cannot be
        ' overwritten by SVN. We have to dettach each file, overwrite with SVN, then reattach.
        'Dim i As Integer
        'Dim k As Integer
        'Dim mdDocsToReattach(UBound(modDocArr)) As ModelDoc2
        For i = 0 To UBound(fp)
            If fp(i).modDoc Is Nothing Then Continue For
            If fp(i).modDoc.IsOpenedReadOnly() Then
                ' ForceReleaseLocks is releasing SolidWorks's system lock on the file
                ' Which prevents other programs (like SVN) from overwriting the file
                ' This allows the file to be overwritten by the New version
                If fp(i).modDoc.GetType <> swDocumentTypes_e.swDocDRAWING Then
                    'The method doesn't work for Drawings
                    fp(i).modDoc.ForceReleaseLocks()
                    fp(i).bReconnect = True
                End If
            Else
                ' The user has an obsolete write copy of a file. They'll have to
                ' Decide if they want to destroy their changes or the ones on the vault.
                ' Do nothing. Let Tortoise.exe notify user of their issue.
            End If
        Next
    End Sub
    Sub reattachDocsToFileSystem()
        For i = 0 To UBound(fp)
            If fp(i).modDoc Is Nothing Then Continue For
            'Reattaches the file system to SolidWorks. Whatever that means :p
            If fp(i).bReconnect Then
                fp(i).modDoc.ReloadOrReplace(
                    ReadOnly:=True, ReplaceFileName:=False, DiscardChanges:=True)
            End If
        Next
    End Sub
End Class
