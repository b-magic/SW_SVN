Imports System.CodeDom
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
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
        Public iTemp As Byte
    End Structure
    'Structure statusError
    '    Public sMessage As String
    '    Public sFile As String
    '    Public modDocArr As ModelDoc2
    'End Structure
    Public Function getModDocArr() As SolidWorks.Interop.sldworks.ModelDoc2()
        Dim modDocArr(UBound(fp)) As SolidWorks.Interop.sldworks.ModelDoc2
        Dim i As Integer

        For i = 0 To UBound(fp)
            modDocArr(i) = fp(i).modDoc
        Next

        Return modDocArr

    End Function
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
        Dim temp As String
        'Dim sw = New Stopwatch()
        'sw.Start()

        For i = 0 To UBound(fp)

            If fp(i).modDoc Is Nothing Then Continue For
            Try
                fp(i).modDoc.IsOpenedReadOnly() 'catching error where modDoc2 obj get unattached
            Catch ex As Exception
                fp(i).modDoc = Nothing
                Continue For
            End Try

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
    Public Sub mFilterLocked(Optional bFilterUnlocked As Boolean = False)
        Dim newFilePropertyArr(UBound(fp)) As filePpty
        Dim j As Integer = 0

        For i = 0 To UBound(fp)
            If (Not bFilterUnlocked) And (fp(i).lock6 = "K") Then
                newFilePropertyArr(j) = fp(i)
                j = j + 1
            ElseIf (bFilterUnlocked) And (fp(i).lock6 = " ") Then
                newFilePropertyArr(j) = fp(i)
                j = j + 1
            End If
        Next

        If j = 0 Then
            fp = Nothing
            Exit Sub
        End If

        ReDim Preserve newFilePropertyArr(j - 1)

        fp = newFilePropertyArr
    End Sub
    Public Function statusFilter(
                                 Optional sFiltAddDelChg1 As String = Nothing,
                                 Optional sFiltPptyMods2 As String = Nothing,
                                 Optional sFiltWorkingDirLock3 As String = Nothing,
                                 Optional sFiltAddWithHist4 As String = Nothing,
                                 Optional sFiltSwitchWParent5 As String = Nothing,
                                 Optional sFiltLock6 As String = Nothing,
                                 Optional sFiltTree7 As String = Nothing, 'note: svn leave col 8 blank
                                 Optional sFiltUpToDate9 As String = Nothing,
                                 Optional byFiltITemp As Byte = Nothing,
                                 Optional sFiltFileName As String = Nothing,
                                 Optional bFiltBReconnect As Boolean = Nothing,
                                 Optional gltFiltRevertUpdate As getLatestType = Nothing) As SVNStatus
        'Dim sFilePaths(UBound(fp)) As String
        Dim returnFunc As SVNStatus
        Dim newFP(UBound(fp)) As filePpty
        Dim j As Integer = 0
        Dim iPassFilter As Integer = 0

        Dim iNumFilters As Integer =
            (IsNothing(sFiltAddDelChg1) * 1 + IsNothing(sFiltPptyMods2) * 2 + IsNothing(sFiltWorkingDirLock3) * 4 + IsNothing(sFiltAddWithHist4) * 8 +
            IsNothing(sFiltSwitchWParent5) * 16 + IsNothing(sFiltLock6) * 32 + IsNothing(sFiltTree7) * 64 + IsNothing(sFiltUpToDate9) * 128 +
            IsNothing(byFiltITemp) * 256 + IsNothing(sFiltFileName) * 512 + IsNothing(bFiltBReconnect) * 1024 + IsNothing(gltFiltRevertUpdate) * 2048)

        If iNumFilters = 0 Then
            Return Nothing
        ElseIf (iNumFilters = 2) Or (((Math.Log(iNumFilters) / Math.Log(2.0#)) Mod 2) = 0) Then
            'Fancy way of determining that only one option was selected

            Select Case iNumFilters
                Case 1
                    For i = 0 To UBound(fp)
                        If sFiltAddDelChg1.Contains(fp(i).addDelChg1) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 2
                    For i = 0 To UBound(fp)
                        If sFiltPptyMods2.Contains(fp(i).pptyMods2) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 4
                    For i = 0 To UBound(fp)
                        If sFiltWorkingDirLock3.Contains(fp(i).workingDirLock3) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 8
                    For i = 0 To UBound(fp)
                        If sFiltAddWithHist4.Contains(fp(i).addWithHist4) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 16
                    For i = 0 To UBound(fp)
                        If sFiltSwitchWParent5.Contains(fp(i).switchWParent5) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 32
                    For i = 0 To UBound(fp)
                        If sFiltLock6.Contains(fp(i).lock6) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 64
                    For i = 0 To UBound(fp)
                        If sFiltTree7.Contains(fp(i).tree7) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 128
                    For i = 0 To UBound(fp)
                        If sFiltUpToDate9.Contains(fp(i).upToDate9) Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 256
                    For i = 0 To UBound(fp)
                        If fp(i).iTemp = byFiltITemp Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 512
                    For i = 0 To UBound(fp)
                        If fp(i).filename = sFiltFileName Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 1024
                    For i = 0 To UBound(fp)
                        If fp(i).bReconnect = bFiltBReconnect Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
                Case 2048
                    For i = 0 To UBound(fp)
                        If fp(i).revertUpdate = gltFiltRevertUpdate Then
                            newFP(j) = fp(i)
                            j += 1
                        End If
                    Next
            End Select
        Else
            'more than 1 option was selected

            For i = 0 To UBound(fp)
                If (IsNothing(sFiltAddDelChg1)) Then
                ElseIf sFiltAddDelChg1.Contains(fp(i).addDelChg1) Then
                Else Continue For
                End If

                If (IsNothing(sFiltPptyMods2)) Then
                ElseIf sFiltPptyMods2.Contains(fp(i).pptyMods2) Then
                Else Continue For
                End If

                If (IsNothing(sFiltWorkingDirLock3)) Then
                ElseIf sFiltWorkingDirLock3.Contains(fp(i).workingDirLock3) Then
                Else Continue For
                End If

                If (IsNothing(sFiltAddWithHist4)) Then
                ElseIf sFiltAddWithHist4.Contains(fp(i).addWithHist4) Then
                Else Continue For
                End If

                If (IsNothing(sFiltSwitchWParent5)) Then
                ElseIf sFiltSwitchWParent5.Contains(fp(i).switchWParent5) Then
                Else Continue For
                End If

                If (IsNothing(sFiltLock6)) Then
                ElseIf sFiltLock6.Contains(fp(i).lock6) Then
                Else Continue For
                End If

                If (IsNothing(sFiltTree7)) Then
                ElseIf sFiltTree7.Contains(fp(i).tree7) Then
                Else Continue For
                End If

                If (IsNothing(sFiltUpToDate9)) Then
                ElseIf sFiltUpToDate9.Contains(fp(i).upToDate9) Then
                Else Continue For
                End If

                If (IsNothing(byFiltITemp)) Then
                ElseIf fp(i).iTemp = byFiltITemp Then
                Else Continue For
                End If

                If (IsNothing(sFiltFileName)) Then
                ElseIf fp(i).filename = sFiltFileName Then
                Else Continue For
                End If

                If (IsNothing(bFiltBReconnect)) Then
                ElseIf fp(i).bReconnect = bFiltBReconnect Then
                Else Continue For
                End If

                If (IsNothing(gltFiltRevertUpdate)) Then
                ElseIf fp(i).revertUpdate = gltFiltRevertUpdate Then
                Else Continue For
                End If

                'made it this far, passed the filter
                newFP(j) = fp(i)
                j += 1
            Next
        End If

        If j <> 0 Then
            ReDim Preserve newFP(j - 1)
            returnFunc = Me.Clone
            returnFunc.fp = newFP
            Return returnFunc
        Else
            Return Nothing
        End If
    End Function
    Public Function mFilterGetLatestType(ByRef filter As getLatestType, Optional ByVal bIgnoreUpdate As Boolean = False) As ModelDoc2()
        'bIgnoreUpdate will ignore the filter out elements where an update is required.
        Dim modDocArr(UBound(fp)) As ModelDoc2
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            If (fp(i).revertUpdate = filter) And Not ((bIgnoreUpdate) And (fp(i).upToDate9 = "*")) Then
                modDocArr(j) = fp(i).modDoc
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Return modDocArr
    End Function
    Function mSubsetAndGetModDocArr(ByRef iIndex() As Integer) As ModelDoc2()
        Dim modelDocList As New List(Of ModelDoc2)()

        For i = 0 To UBound(iIndex)
            modelDocList.Add(fp(iIndex(i)).modDoc)
        Next

        Dim returnModDocArr As ModelDoc2() = modelDocList.ToArray
        Return returnModDocArr
    End Function
    Function sSubsetAndGetFileNameArr(ByRef iIndex() As Integer) As String()
        Dim fileNameList As New List(Of String)()

        For i = 0 To UBound(iIndex)
            fileNameList.Add(fp(iIndex(i)).filename)
        Next

        Dim returnFileNameArr As ModelDoc2() = fileNameList.ToArray
        Return returnFileNameArr
    End Function
    Public Function indexFilterGetLatestType(ByRef filter As getLatestType, Optional ByVal bIgnoreUpdate As Boolean = False) As Integer()
        'bIgnoreUpdate will ignore the filter out elements where an update is required.

        Dim indexList As New List(Of Integer)()

        Dim sPath(UBound(fp)) As String
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            If (fp(i).revertUpdate = filter) And Not ((bIgnoreUpdate) And (fp(i).upToDate9 = "*")) Then
                indexList.Add(i)
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Dim iReturnIndex As Integer() = indexList.ToArray
        Return iReturnIndex
    End Function

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
    Sub releaseFileSystemAccessToRevertOrUpdateModels(Optional index As Integer() = Nothing)
        'Even if files are read-only, they are still "in-use" by Solidworks, so the files cannot be
        ' overwritten by SVN. We have to dettach each file, overwrite with SVN, then reattach.
        Dim i As Integer
        'Dim k As Integer
        'Dim mdDocsToReattach(UBound(modDocArr)) As ModelDoc2

        If index Is Nothing Then Exit Sub

        If index(0) = -1 Then
            'index = Enumerable.Range(0, UBound(fp))
            ReDim index(UBound(fp))
            For i = 0 To UBound(fp)
                index(i) = i
            Next
        End If

        For i = 0 To UBound(index)
            If fp(index(i)).modDoc Is Nothing Then Continue For
            If fp(index(i)).modDoc.IsOpenedReadOnly() Then
                ' ForceReleaseLocks is releasing SolidWorks's system lock on the file
                ' Which prevents other programs (like SVN) from overwriting the file
                ' This allows the file to be overwritten by the New version
                If fp(index(i)).modDoc.GetType <> swDocumentTypes_e.swDocDRAWING Then
                    'The method doesn't work for Drawings
                    fp(index(i)).modDoc.ForceReleaseLocks() 'Forces solidworks to release it's lock on the file, not to be confused with SVN lock.
                    fp(index(i)).bReconnect = True
                End If
            Else
                ' The user has an obsolete write copy of a file. They'll have to
                ' Decide if they want to destroy their changes or the ones on the vault.
                ' Do nothing. Let Tortoise.exe notify user of their issue.
            End If
        Next
    End Sub
    Sub reattachDocsToFileSystem(index As Integer())
        'Pass -1 to set index as all
        Dim reloadOrReplaceResult As swComponentReloadError_e

        If index Is Nothing Then Exit Sub

        If index(0) = -1 Then
            'index = Enumerable.Range(0, UBound(fp))
            ReDim index(UBound(fp))
            For i = 0 To UBound(fp)
                index(i) = i
            Next
        End If

        For i = 0 To UBound(index)
            If fp(index(i)).modDoc Is Nothing Then Continue For
            Try
                fp(index(i)).modDoc.IsOpenedReadOnly()
            Catch ex As Exception
                fp(index(i)).modDoc = Nothing
                Continue For
            End Try

            'Reattaches the file system to SolidWorks. Whatever that means :p
            If fp(index(i)).bReconnect Then
                reloadOrReplaceResult = fp(index(i)).modDoc.ReloadOrReplace(
                    ReadOnly:=True, ReplaceFileName:=Nothing, DiscardChanges:=True)
                Debug.Print(fp(index(i)).filename & " - Reload/Replace Result: " & reloadOrReplaceResult)
                fp(i).bReconnect = False 'reset it
            End If
        Next
    End Sub
    Function updateLockStatusLocally() As Boolean

        Dim newOutput As SVNStatus = getFileSVNStatus(bCheckServer:=False, Me.getModDocArr(), bUpdateStatusOfAllOpenModels:=False)
        Dim newOutputFilteredLocked As SVNStatus
        Dim newOutputFilteredUnlocked As SVNStatus
        Dim i As Integer

        Try
            newOutputFilteredLocked = newOutput.statusFilter(sFiltLock6:="K")
            newOutputFilteredUnlocked = newOutput.statusFilter(sFiltLock6:=" OTB")
        Catch
            Return Nothing
        End Try
        If IsNothing(newOutput) Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        ElseIf newOutput.fp.Length = 0 Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        End If

        'First pass
        For i = 0 To UBound(fp)

            Try 'need to do try to prevent 'outside of bounds' error with newOutput.fp(i)
                If fp(i).filename = newOutput.fp(i).filename Then
                    'First Try worked
                    fp(i).lock6 = newOutput.fp(i).lock6
                End If
            Catch
            End Try

            If (fp(i).lock6 = "K") And (Not IsNothing(newOutputFilteredUnlocked)) Then
                'Search through the unlocked ones from the new svnstatus
                'there's no point searching through the locked ones from the new svn status
                For j = 0 To UBound(newOutputFilteredUnlocked.fp)
                    If fp(i).filename = newOutputFilteredUnlocked.fp(j).filename Then
                        fp(i).lock6 = newOutputFilteredUnlocked.fp(j).lock6
                        Exit For
                    End If
                Next
            ElseIf Not IsNothing(newOutputFilteredUnlocked) Then
                'Old was unlocked; search through the new locked... 
                For j = 0 To UBound(newOutputFilteredLocked.fp)
                    If fp(i).filename = newOutputFilteredLocked.fp(j).filename Then
                        fp(i).lock6 = newOutputFilteredLocked.fp(j).lock6
                        Exit For
                    End If
                Next
            End If
        Next
        Return True
    End Function
    Function updateFromSvnServer(Optional bRefreshAllTreeViews As Boolean = False) As Boolean
        'iSwApp.EnableBackgroundProcessing = True

        Dim output As SVNStatus = getFileSVNStatus(bCheckServer:=True, getAllOpenDocs(bMustBeVisible:=False))
        'Dim bProcessingTemp As Boolean = iSwApp.EnableBackgroundProcessing

        If IsNothing(output) Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        ElseIf output.fp.Length = 0 Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        End If

        fp = output.fp
        'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
        Return True

    End Function
End Class
