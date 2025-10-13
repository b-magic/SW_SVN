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

        ' Deep copy of fp array (structure array)
        If Me.fp IsNot Nothing Then
            myClone.fp = CType(Me.fp.Clone(), filePpty())
        End If

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
        Public released As String
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
                                 Optional ByVal bCheckServer As Boolean = False,
                                 Optional ByVal sReleaseProperty As String = "")
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
        fp(j).released = sReleaseProperty
    End Sub
    Sub setReadWriteFromLockStatus()
        Dim i As Integer
        'Dim temp As String
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
                                 Optional byFiltITemp As Integer = Nothing,
                                 Optional sFiltFileName As String = Nothing,
                                 Optional bFiltBReconnect As Integer = Nothing,
                                 Optional gltFiltRevertUpdate As getLatestType = getLatestType.undefined,
                                 Optional sFiltReleasedRemoved As String = Nothing) As SVNStatus
        'Dim sFilePaths(UBound(fp)) As String
        Dim returnFunc As SVNStatus
        Dim newFP(UBound(fp)) As filePpty
        Dim j As Integer = 0
        Dim iPassFilter As Integer = 0

        'Dim iNumFilters As Integer = 0
        'If Not IsNothing(sFiltAddDelChg1) Then iNumFilters += 1
        'If Not IsNothing(sFiltPptyMods2) Then iNumFilters += 2
        'If Not IsNothing(sFiltWorkingDirLock3) Then iNumFilters += 4
        'If Not IsNothing(sFiltAddWithHist4) Then iNumFilters += 8
        'If Not IsNothing(sFiltSwitchWParent5) Then iNumFilters += 16
        'If Not IsNothing(sFiltLock6) Then iNumFilters += 32
        'If Not IsNothing(sFiltTree7) Then iNumFilters += 64
        'If Not IsNothing(sFiltUpToDate9) Then iNumFilters += 128
        'If Not byFiltITemp = 0 Then iNumFilters += 256
        'If Not IsNothing(sFiltFileName) Then iNumFilters += 512
        'If bFiltBReconnect Then iNumFilters += 1024 ' Because default is False
        'If gltFiltRevertUpdate <> getLatestType.undefined Then iNumFilters += 2048
        'If Not IsNothing(sFiltReleasedRemoved) Then iNumFilters += 4096

        'If iNumFilters = 0 Then
        '    Return Nothing
        'ElseIf (iNumFilters = 2) Or (((Math.Log(iNumFilters) / Math.Log(2.0#)) Mod 2) = 0) Then
        '    'Fancy way of determining that only one option was selected

        '    Select Case iNumFilters
        '        Case 1
        '            For i = 0 To UBound(fp)
        '                If sFiltAddDelChg1.Contains(fp(i).addDelChg1) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 2
        '            For i = 0 To UBound(fp)
        '                If sFiltPptyMods2.Contains(fp(i).pptyMods2) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 4
        '            For i = 0 To UBound(fp)
        '                If sFiltWorkingDirLock3.Contains(fp(i).workingDirLock3) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 8
        '            For i = 0 To UBound(fp)
        '                If sFiltAddWithHist4.Contains(fp(i).addWithHist4) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 16
        '            For i = 0 To UBound(fp)
        '                If sFiltSwitchWParent5.Contains(fp(i).switchWParent5) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 32
        '            For i = 0 To UBound(fp)
        '                If sFiltLock6.Contains(fp(i).lock6) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 64
        '            For i = 0 To UBound(fp)
        '                If sFiltTree7.Contains(fp(i).tree7) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 128
        '            For i = 0 To UBound(fp)
        '                If sFiltUpToDate9.Contains(fp(i).upToDate9) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 256
        '            For i = 0 To UBound(fp)
        '                If fp(i).iTemp = byFiltITemp Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 512
        '            For i = 0 To UBound(fp)
        '                If fp(i).filename = sFiltFileName Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 1024
        '            For i = 0 To UBound(fp)
        '                If fp(i).bReconnect = bFiltBReconnect Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 2048
        '            For i = 0 To UBound(fp)
        '                If fp(i).revertUpdate = gltFiltRevertUpdate Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '        Case 4096
        '            For i = 0 To UBound(fp)
        '                If Not (fp(i).revertUpdate = sFiltReleasedRemoved) Then
        '                    newFP(j) = fp(i)
        '                    j += 1
        '                End If
        '            Next
        '    End Select
        'Else
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

            'If (IsNothing(byFiltITemp)) Then
            'ElseIf fp(i).iTemp = byFiltITemp Then
            'Else Continue For
            'End If

            If (IsNothing(sFiltFileName)) Then
            ElseIf fp(i).filename = sFiltFileName Then
            Else Continue For
            End If

            'If (IsNothing(bFiltBReconnect)) Then
            'ElseIf fp(i).bReconnect = bFiltBReconnect Then
            'Else Continue For
            'End If

            'If gltFiltRevertUpdate <> getLatestType.undefined Then
            'ElseIf fp(i).revertUpdate = gltFiltRevertUpdate Then
            'Else Continue For
            'End If

            If (IsNothing(sFiltReleasedRemoved)) Then
                'Pass
            ElseIf Not (fp(i).released = sFiltReleasedRemoved) Then
                'Pass
            Else
                'Fail
                Continue For
            End If

            'made it this far, passed the filter
            newFP(j) = fp(i)
            j += 1
        Next
        'End If

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
    Public Function sFilterReleased(ByRef filter As String) As String()
        Dim sPath(UBound(fp)) As String
        Dim j As Integer = 0
        For i As Integer = 0 To UBound(fp)
            If (fp(i).released = filter) Then
                sPath(j) = fp(i).filename
                j += 1
            End If
        Next
        If j = 0 Then Return Nothing
        Return sPath
    End Function
    Sub releaseFileSystemAccessToRevertOrUpdateModels(iSwApp As SolidWorks.Interop.sldworks.SldWorks, Optional index As Integer() = Nothing)
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
                If fp(index(i)).modDoc.GetType = swDocumentTypes_e.swDocDRAWING Then
                    'forcereleaselocks isn't a thing for drawings. Need to close and reopen it. 
                    iSwApp.CloseDoc(fp(index(i)).modDoc.GetTitle)
                    fp(index(i)).modDoc = Nothing
                Else
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
    Sub reattachDocsToFileSystem(index As Integer(), iSwApp As SolidWorks.Interop.sldworks.SldWorks)
        'Pass -1 to set index as all
        Dim reloadOrReplaceResult As swComponentReloadError_e
        Dim swErrors, swWarnings As Integer

        If index Is Nothing Then Exit Sub

        If index(0) = -1 Then
            'index = Enumerable.Range(0, UBound(fp))
            ReDim index(UBound(fp))
            For i = 0 To UBound(fp)
                index(i) = i
            Next
        End If

        For i = 0 To UBound(index)
            If fp(index(i)).modDoc Is Nothing Then
                If Strings.InStr(System.IO.Path.GetExtension(fp(index(i)).filename), ".slddrw", CompareMethod.Text) <> 0 Then
                    'not able to detach a drawing from the file system. Had to close and reopen. 
                    fp(index(i)).modDoc = iSwApp.OpenDoc6(
                        fp(index(i)).filename,
                        swDocumentTypes_e.swDocDRAWING,
                        swOpenDocOptions_e.swOpenDocOptions_LoadModel,
                        "", swErrors, swWarnings)
                End If
                Continue For
            End If
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
    Function updateStatusLocally(iSwApp As SolidWorks.Interop.sldworks.SldWorks) As Boolean
        'Updates locks and release status without contacting server

        Dim newOutput As SVNStatus = getFileSVNStatus(bCheckServer:=False,, bUpdateStatusOfAllOpenModels:=False)
        Dim i, oldIndex, oldUboundFp As Integer
        Dim filePptyToAdd As New List(Of filePpty)
        'Dim nToAdd As Integer = 0
        Dim sPropArr(,) As String
        Dim sRelease(UBound(fp)) As String

        If newOutput Is Nothing Then Return False

        'Try
        '    newOutputFilteredLocked = newOutput.statusFilter(sFiltLock6:="K")
        '    newOutputFilteredUnlocked = newOutput.statusFilter(sFiltLock6:=" OTB")
        'Catch
        '    Return Nothing
        'End Try
        If IsNothing(newOutput) Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        ElseIf newOutput.fp.Length = 0 Then
            'iSwApp.EnableBackgroundProcessing = False 'bProcessingTemp
            Return False
        End If

        sPropArr = svnPropget()
        For j = 0 To UBound(fp)
            sRelease(j) = vLookup(fp(j).filename.Replace("\", "/"), sPropArr, 1)
        Next

        For i = 0 To UBound(newOutput.fp)
            oldIndex = -1
            If IsNothing(newOutput.fp(i)) Then Continue For
            If IsNothing(newOutput.fp(i).lock6) Then Continue For

            'Search for match in old fp
            For k = 0 To UBound(fp)
                If (Strings.InStr(fp(k).filename, newOutput.fp(i).filename, CompareMethod.Text) <> 0) Then
                    oldIndex = k
                    Exit For
                End If
            Next
            If oldIndex = -1 Then
                'didn't find a match
                filePptyToAdd.Add(newOutput.fp(i))
                Continue For
            End If
            If IsNothing(newOutput.fp(i).lock6) Or IsNothing(fp(oldIndex).lock6) Then Continue For
            If ((fp(oldIndex).lock6 = "O") Or (fp(oldIndex).lock6 = "T") Or (fp(oldIndex).lock6 = "B")) And (Not (newOutput.fp(i).lock6 = "K")) Then Continue For 'we're not calling the server, so don't want to overwrite info only available from server. 


            fp(oldIndex).lock6 = newOutput.fp(i).lock6
            If Not IsNothing(sRelease(oldIndex)) Then fp(oldIndex).released = sRelease(oldIndex)
        Next

        oldUboundFp = UBound(fp)
        If filePptyToAdd.Count > 0 Then
            ReDim Preserve fp(UBound(fp) + filePptyToAdd.Count)
            filePptyToAdd.CopyTo(fp, oldUboundFp + 1)
        Else
            ' list is empty
        End If

        Return True
    End Function
    Function updateFromSvnServer(Optional bRefreshAllTreeViews As Boolean = False) As Boolean
        'iSwApp.EnableBackgroundProcessing = True
        Dim allOpenDocs As ModelDoc2() = getAllOpenDocs(bMustBeVisible:=False)
        Dim output As SVNStatus

        If allOpenDocs Is Nothing Then Return False
        If verifyCommandArgumentLength("12345678901234567890123456789012345678901234567890" &
                                    formatFilePathArrForProc(getFilePathsFromModDocArr(allOpenDocs), sDelimiter:=""" """)) Then 'Added 50 extra characters as safety to make up for arguments/commands other than just the filenames

            output = getFileSVNStatus(bCheckServer:=True, allOpenDocs)

        Else
            'Too many open docs to request each individually from the server. We will run into issue with the argument being too long. Instead just request all of the files. 
            output = getFileSVNStatus(bCheckServer:=True,)
            'TODO: filter the output to just open files? Might speed things up?
        End If


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
