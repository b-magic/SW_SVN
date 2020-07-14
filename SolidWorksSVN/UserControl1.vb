
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing

<ProgId("SVN_AddIn")>
Public Class UserControl1

    Public WithEvents iSwApp As SldWorks
    'Dim userAddin As SwAddin = New SwAddin() 'couldn't get access to swapp in here!
    Public Const sTortPath As String = "C:\Users\benne\Documents\SVN\TortoiseProc.exe"
    'Public Const localRepoPath.text As String = "E:\SolidworksBackup\svn"
    'Public Const localRepoPath.text As String = "C:\Users\benne\Documents\SVN\cad1"
    Public Const sSVNPath As String = "C:\Program Files\TortoiseSVN\bin\svn.exe"

    Public statusOfAllOpenModels As SVNStatus
    Public allOpenDocs As ModelDoc2()
    Public allTreeViews As TreeView() = {New TreeView}

    Friend Sub getSwApp(ByRef swAppin As SldWorks)
        'Allows for swApp to be passed into this class.
        iSwApp = swAppin
    End Sub
    Private Sub butCheckinWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckinWithDependents.Click
        myCheckinWithDependents(iSwApp.ActiveDoc())
        updateStatusStrip()
    End Sub

    Private Sub butCheckinAll_Click(sender As Object, e As EventArgs) Handles butCheckinAll.Click
        myCheckinAll()
        updateStatusStrip()
    End Sub
    Private Sub butUnlockActive_Click(sender As Object, e As EventArgs) Handles butUnlockActive.Click
        myUnlockActive()
        updateStatusStrip()
    End Sub
    Private Sub butUnlockAll_Click(sender As Object, e As EventArgs) Handles butUnlockAll.Click
        unlockDocs()
        updateStatusStrip()
    End Sub
    Private Sub butCheckoutActiveDoc_Click(sender As Object, e As EventArgs) Handles butCheckoutActiveDoc.Click
        myCheckoutActiveDoc()
        updateStatusStrip()
    End Sub

    Private Sub butCheckoutWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckoutWithDependents.Click
        myCheckoutWithDependents()
        updateStatusStrip()
    End Sub

    Private Sub butGetLatestOpenOnly_Click(sender As Object, e As EventArgs) Handles butGetLatestOpenOnly.Click
        myGetLatestOrRevert(getAllOpenDocs(bMustBeVisible:=False),, bVerbose:=True)
        'myGetLatestOpenOnly()
        updateStatusStrip()
    End Sub

    Private Sub butGetLatestAllRepo_Click(sender As Object, e As EventArgs) Handles butGetLatestAllRepo.Click
        myGetLatestOrRevert(,, bVerbose:=True)
        updateStatusStrip()
        'myGetLatestAllRepo()
    End Sub
    Private Sub StatusStrip2_ItemClicked(sender As Object, e As Windows.Forms.ToolStripItemClickedEventArgs) Handles StatusStrip2.ItemClicked
        updateStatusStrip()
    End Sub
    Private Sub butCleanup_Click(sender As Object, e As EventArgs) Handles butCleanup.Click
        myCleanupAndRelease()
    End Sub
    Private Sub butStatus_Click(sender As Object, e As EventArgs) Handles butStatus.Click
        myRepoStatus()
    End Sub
    Public Function updateStatusOfAllModelsVariable(Optional bRefreshAllTreeViews As Boolean = False) As Boolean

        Dim output As SVNStatus = getFileSVNStatus(bCheckServer:=True, getAllOpenDocs(bMustBeVisible:=False))

        If IsNothing(output) Then
            Return False
        ElseIf output.fp.Length = 0 Then
            Return False
        End If

        If bRefreshAllTreeViews Then refreshAllTreeViewsVariable()
        Return True
    End Function

    Public Sub updateStatusStrip()
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then Exit Sub

        Dim myCol As myColours = New myColours()
        Dim status As SVNStatus = findStatusForFile(modDoc.GetPathName)
        If IsNothing(status) Then Exit Sub

        myCol.initialize()
        If IsNothing(status) Then
            StatusStrip2.Text = ""
            StatusStrip2.BackColor = myCol.unknown
        ElseIf status.fp(0).lock6 = "K" Then
            StatusStrip2.Text = "Locked by you"
            StatusStrip2.BackColor = myCol.lockedByYou
        ElseIf status.fp(0).lock6 = "O" Then
            StatusStrip2.Text = "Locked By someone Else"
            StatusStrip2.BackColor = myCol.lockedBySomeoneElse
        ElseIf status.fp(0).lock6 = " " Then
            StatusStrip2.Text = "Available"
            StatusStrip2.BackColor = myCol.available
        End If
    End Sub

    Public Sub myUnlockActive()
        Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub
        unlockDocs(modDoc)
    End Sub
    Sub myUnlockWithDependents(modDoc As ModelDoc2())

        'Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        If modDoc(0).GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            unlockDocs(modDoc)
        Else
            unlockDocs(getComponentsOfAssemblyOptionalUpdateTree(modDoc(0)))
        End If
    End Sub
    Sub unlockDocs(Optional ByRef modDocArr() As ModelDoc2 = Nothing)
        Dim bSuccess As Boolean
        Dim Status As SVNStatus

        If IsNothing(modDocArr) Then
            bSuccess = runTortoiseProcexeWithMonitor("/command:unlock /path:" & localRepoPath.text & " /closeonend:3")

        Else
            Status = getFileSVNStatus(bCheckServer:=True, modDocArr)
            If IsNothing(Status) Then Exit Sub

            bSuccess = runTortoiseProcexeWithMonitor("/command:unlock /path:" &
                                             formatFilePathArrForTortoiseProc(
                                                getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")
        End If

        If Not bSuccess Then iSwApp.SendMsgToUserv("Releasing Locks Failed.")

        myGetLatestOrRevert(modDocArr, getLatestType.revert)

    End Sub
    Public Shared Function createBoolArray(ByRef iUbound As Integer, ByRef value As Boolean) As Boolean()
        Dim i As Integer
        Dim output(iUbound) As Boolean
        For i = 0 To iUbound
            output(i) = value
        Next
        Return output
    End Function
    Public Sub myCheckinWithDependents(modDoc As ModelDoc2)

        Dim modDocArr1() As ModelDoc2
        Dim bRequired() As Boolean

        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
            modDocArr1 = getComponentsOfAssemblyOptionalUpdateTree(modDoc)
            bRequired = createBoolArray(UBound(modDocArr1), False)
            bRequired(0) = True
            checkInDocs(modDocArr1, bRequired)
        Else
            modDocArr1 = {modDoc}
            checkInDocs(modDocArr1, createBoolArray(1, True))
        End If
    End Sub
    Sub checkInDocs(ByRef modDocArr() As ModelDoc2,
                    Optional ByVal bRequiredDoc() As Boolean = Nothing)
        Dim bSuccess As Boolean = False
        Dim sErrorFiles As String = ""
        Dim i As Integer
        Dim j As Integer = 0

        Dim activeDoc As ModelDoc2 = iSwApp.ActiveDoc
        If activeDoc Is Nothing Then Exit Sub

        If bRequiredDoc Is Nothing Then bRequiredDoc = createBoolArray(UBound(modDocArr), True)

        If modDocArr Is Nothing Then
            iSwApp.SendMsgToUser("Active Document not found")
            Exit Sub
        ElseIf modDocArr.Length = 0 Then
            iSwApp.SendMsgToUser("Active Document not found")
            Exit Sub
        End If

        For i = 0 To UBound(modDocArr)
            If modDocArr(i).IsOpenedReadOnly() Or modDocArr(i).IsOpenedViewOnly() Then

                If bRequiredDoc(i) Then
                    sErrorFiles &= modDocArr(i).GetPathName & vbCrLf
                End If
                modDocArr(i) = Nothing
                j += 1
            End If
        Next
        If sErrorFiles <> "" Then
            iSwApp.SendMsgToUser("The following file(s) are Read-Only. You need write access to check in. " &
                                 "If you believe you have the file locked, you can try File > Reload" & vbCrLf &
                                 sErrorFiles)
            If j = i Then Exit Sub 'All Files were removed
        End If

        save3AndShowErrorMessages(modDocArr)

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:" &
                                                 formatFilePathArrForTortoiseProc(
                                                    getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("Tortoise App Failed.") : Exit Sub

        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then Exit Sub

        switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        statusOfAllOpenModels.setReadWriteFromLockStatus()
    End Sub
    Public Sub myCheckinAll()
        Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String

        'Dim i As Integer
        'Dim index As Integer

        iSwApp.RunCommand(19, vbEmpty) 'Save All

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:""" & localRepoPath.text & """ /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("TortoiseSVN Process Failed.") : Exit Sub

        'Switch over files to read-only
        'OpenDocPathList = CType(getAllOpenDocs(True, True), String())
        Dim OpenDocModels() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=True)

        'Dim sOpenDocPath() As String = getFilePathsFromModDoiSwApp.SendMsgToUser("Active Document not found") cArr(OpenDocModels)

        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then Exit Sub

        switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        statusOfAllOpenModels.setReadWriteFromLockStatus()

    End Sub
    Public Sub myCheckoutActiveDoc()
        Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        checkoutDocs(modDoc)
    End Sub
    Public Sub myCheckoutWithDependents()
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        checkoutDocs(getComponentsOfAssemblyOptionalUpdateTree(modDoc))
    End Sub
    Sub myRepoStatus()
        Dim bSuccess As Boolean
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        Dim modDocArr() As ModelDoc2

        If modDoc Is Nothing Then
            iSwApp.SendMsgToUser("A File must be open")
            Exit Sub
            'bSuccess = runTortoiseProcexeWithMonitor("/command:repostatus /remote")
        Else
            modDocArr = getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc)
            bSuccess = runTortoiseProcexeWithMonitor("/command:repostatus /path:" &
                                                 formatFilePathArrForTortoiseProc(modDocArr) &
                                                 " /remote")
        End If
        If Not bSuccess Then iSwApp.SendMsgToUser("Status Check Failed.")
    End Sub
    Sub myCleanupAndRelease()
        Dim bSuccessStatus As Boolean
        Dim bSuccessCleanup As Boolean
        Dim allOpenDocs As ModelDoc2() = getAllOpenDocs(bMustBeVisible:=False)

        bSuccessStatus = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)

        If bSuccessStatus Then
            bSuccessCleanup = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:" & localRepoPath.text)
        Else
            'Manually release file system locks
            For Each modDoc In allOpenDocs
                modDoc.ForceReleaseLocks()
            Next

            bSuccessCleanup = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:" & localRepoPath.text)
            For Each modDoc In allOpenDocs
                'Manually reattach to file system
                modDoc.ReloadOrReplace(ReadOnly:=True, ReplaceFileName:=False, DiscardChanges:=False)
            Next
        End If

        If bSuccessCleanup Then
            bSuccessStatus = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
            If bSuccessStatus Then
                statusOfAllOpenModels.setReadWriteFromLockStatus()
                switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
            End If
        Else
            iSwApp.SendMsgToUser("Cleanup Failed. This is often because the SVN server is attempting " &
                    "to open a file that SolidWorks is currently accessing. This occurs even when the file is read only. " &
                    "Try closing all open files and trying again. Or close SolidWorks and use ToroiseSVN to clean up. ")
        End If
    End Sub
    Sub checkoutDocs(ByRef modDocArr() As ModelDoc2)
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        'Dim modDocArr() As ModelDoc2 = {modDoc}
        'Dim sActiveDocPath() As String = getFilePathsFromModDocArr(modDocArr)
        Dim sDocPathsToCheckout(modDocArr.Length - 1) As String
        Dim status As SVNStatus
        Dim bSuccess As Boolean = False
        Dim sCatMessage As String = ""
        Dim sCatMessageLocked As String = ""

        status = getFileSVNStatus(bCheckServer:=True, modDocArr)
        If IsNothing(status) Then Exit Sub
        'If Not IsNothing(status.statError(0).sMessage) Then
        '    'If status.statError(0).sMessage <> "" Then
        '    '    iSwApp.SendMsgToUser(status.statError(0).sMessage)
        '    'Else
        '    '    iSwApp.SendMsgToUser("Error Contacting SVN Server")
        '    'End If
        '    'Exit Sub

        'End If

        sDocPathsToCheckout = status.sFilterUpToDate9("*", bFilterNot:=True)

        sCatMessage = catWithNewLine(status.sFilterUpToDate9("*"))

        If sCatMessage <> "" Then
            iSwApp.SendMsgToUser("Local copy is out of date. Update from Vault and try again." & vbCrLf & sCatMessage)
            'TODO add window. Ask user if they want to update
            If sDocPathsToCheckout(0) Is Nothing Then Exit Sub
        End If
        If sDocPathsToCheckout(0) Is Nothing Then
            iSwApp.SendMsgToUser("No Files available to be locked.")
            Exit Sub
        End If
        bSuccess = runTortoiseProcexeWithMonitor("/command:lock /path:" & formatFilePathArrForTortoiseProc(sDocPathsToCheckout) & " /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("Tortoise Process Failed.") : Exit Sub
        'status = getFileSVNStatus(bCheckServer:=False, modDocArr)

        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then Exit Sub

        switchTreeViewToCurrentModel(bRetryWithRefresh:=False)

        statusOfAllOpenModels.setReadWriteFromLockStatus()

    End Sub
    Function catWithNewLine(stringArr() As String) As String
        Dim i As Integer
        Dim output As String = ""
        If stringArr Is Nothing Then Return ""
        For i = 0 To UBound(stringArr)
            If stringArr(i) Is Nothing Then Continue For
            output &= vbCrLf & stringArr(i)
        Next
        Return output
    End Function
    Sub myGetLatestOrRevert(Optional ByRef modDocArr As ModelDoc2() = Nothing,
                        Optional ByRef myGetType As getLatestType = getLatestType.update,
                            Optional ByRef bVerbose As Boolean = False)
        'Dim modDocTemp As ModelDoc2
        Dim i As Integer
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim status As SVNStatus
        Dim bSuccess As Boolean

        'Update to use getFileSVNStatus so its only the
        If IsNothing(modDocArr) Then
            updateStatusOfAllModelsVariable()
            status = statusOfAllOpenModels
        Else
            status = getFileSVNStatus(bCheckServer:=True, modDocArr)
        End If
        If IsNothing(status) Then Exit Sub

        Dim sFileList(UBound(status.fp)) As String

        For i = 0 To UBound(status.fp)
            'modDocTemp = iSwApp.GetOpenDocumentByName(mySVNStatus.fp(i).filename)
            If status.fp(i).modDoc Is Nothing Then Continue For 'modDocTemp
            'modDocArr(m) = modDocTemp : m += 1
            If (status.fp(i).upToDate9 = "*") And ((myGetType = getLatestType.update) Or (myGetType = getLatestType.both)) Then
                ' File is out of date
                'sFileListToUpdate(j) = mySVNStatus.fp(i).filename : 
                status.fp(i).revertUpdate = getLatestType.update
                sFileList(j) = status.fp(i).filename
                j += 1
            ElseIf (status.fp(i).addDelChg1 = "M") And (myGetType = getLatestType.revert) And (statusOfAllOpenModels.fp(i).lock6 <> "K") Then
                ' Local copy has been modified
                ' Note out of date files will go into FileListToUpdate and will be skipped over by revert.
                'sFileListToRevert(n) = mySVNStatus.fp(i).filename : n += 1
                status.fp(i).revertUpdate = getLatestType.revert
                sFileList(j) = status.fp(i).filename
                j += 1
            End If
        Next

        status.setReadWriteFromLockStatus()

        If j = 0 Then
            If bVerbose Then iSwApp.SendMsgToUser("All Files Checked Are Up to Date!")
            If updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True) Then
                switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
            End If
            Exit Sub
        End If

        status.releaseFileSystemAccessToReadOnlyModels()

        'HELP
        sFileList = status.sFilterGetLatestType(getLatestType.revert, bIgnoreUpdate:=False)
        If (Not sFileList Is Nothing) And ((myGetType = getLatestType.revert) Or (myGetType = getLatestType.both)) Then
            bSuccess = runTortoiseProcexeWithMonitor("/command:revert /path:" &
                                          formatFilePathArrForTortoiseProc(sFileList) & " /closeonend:3")
            If Not bSuccess Then iSwApp.SendMsgToUserv("Revert Files Failed.")
        End If
        sFileList = status.sFilterGetLatestType(getLatestType.update, bIgnoreUpdate:=False)
        If (Not sFileList Is Nothing) And ((myGetType = getLatestType.update) Or (myGetType = getLatestType.both)) Then
            bSuccess = runTortoiseProcexeWithMonitor("/command:update /path:" & formatFilePathArrForTortoiseProc(sFileList) & " /closeonend:3")
            If Not bSuccess Then iSwApp.SendMsgToUserv("Updating Files Failed.")
        End If

        status.reattachDocsToFileSystem()

        If updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True) Then
            switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        End If
    End Sub
    Enum getLatestType
        none
        revert
        update
        both
    End Enum
    Sub NoCallbackSub()
    End Sub
    Sub FlyoutCommandItem1()
        iSwApp.SendMsgToUser("Flyout command 1")
    End Sub
    Function FlyoutEnable() As Integer
        Return 1
    End Function
    Function FlyoutDisable() As Integer
        Return 0
    End Function
    Sub FlyoutCallback()

    End Sub
    Function getFilePathsFromModDocArr(modDocArr() As ModelDoc2) As String()
        If IsNothing(modDocArr) Then Return Nothing

        Dim getFilePathsArr(modDocArr.Length - 1) As String
        For i = 0 To modDocArr.Length - 1
            If modDocArr(i) Is Nothing Then Continue For
            getFilePathsArr(i) = modDocArr(i).GetPathName()
        Next
        Return (getFilePathsArr)
    End Function
    Function getAllOpenDocs(ByRef bMustBeVisible As Boolean) As ModelDoc2()
        'bMustBeVisible ignores invisible files such as parts within an assembly

        Dim modDoc As ModelDoc2 = iSwApp.GetFirstDocument
        Dim iNumDocsOpen As Integer = iSwApp.GetDocumentCount()
        Dim modDocOutput(iNumDocsOpen - 1) As ModelDoc2
        Dim sPath(iNumDocsOpen - 1) As String
        Dim i As Integer
        Dim j As Integer = 0

        For i = 0 To iNumDocsOpen
            If modDoc Is Nothing Then
                'reached the end of the open docs somehow before iNumDocs was reached
                Exit For
            End If
            ' Assembly components are opened, but are not visible
            ' until opened by the user
            If bMustBeVisible Then
                If modDoc.Visible() Then
                    modDocOutput(j) = modDoc
                    'sPath(i) = modDoc.GetPathName()
                    j += 1
                End If
            Else
                modDocOutput(j) = modDoc
                'sPath(i) = modDoc.GetPathName()
                j += 1
            End If
            'sTitle = modDoc.GetTitle
            'sPath = modDoc.GetPathName
            modDoc = modDoc.GetNext
        Next
        ReDim Preserve modDocOutput(j - 1)
        Return modDocOutput
    End Function
    Overloads Function formatFilePathArrForTortoiseProc(ByRef sFilePathArr() As String) As String
        Dim sFilePathCat As String = """" & sFilePathArr(0)
        For i = 0 To sFilePathArr.Length - 1
            If sFilePathArr(i) Is Nothing Then Continue For
            sFilePathCat &= "*" & sFilePathArr(i)
        Next
        sFilePathCat &= """"
        Return sFilePathCat
    End Function
    Overloads Function formatFilePathArrForTortoiseProc(ByRef modDocArr() As ModelDoc2) As String
        Dim sFilePathCat As String = """" & modDocArr(0).GetPathName
        For i = 1 To UBound(modDocArr)
            If modDocArr(i) Is Nothing Then Continue For
            sFilePathCat &= "*" & modDocArr(i).GetPathName
        Next
        sFilePathCat &= """"
        Return sFilePathCat
    End Function

    Function runTortoiseProcexeWithMonitor(ByRef sArguments As String) As Boolean
        ' See https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html
        Dim oTortProcess As New Process()
        Dim tortStartInfo As New ProcessStartInfo

        tortStartInfo.FileName = sTortPath
        tortStartInfo.Arguments = sArguments
        tortStartInfo.WorkingDirectory = localRepoPath.text
        oTortProcess.StartInfo = tortStartInfo
        oTortProcess.Start()

        'Monitor the process. Kill it if it stops responding
        Dim nResponding As Integer = 0
        Do While (Not oTortProcess.HasExited)
            nResponding += Not (oTortProcess.Responding)
            If nResponding > 3000 Then 'Sort of milliseconds because of the sleep command. But not exactly.
                oTortProcess.Kill()
                iSwApp.SendMsgToUser("SVNTortoise Window Timed Out")
                Return False
            End If
            System.Threading.Thread.Sleep(1)
        Loop

        Return True
    End Function
    Sub save3AndShowErrorMessages(ByRef modDocArr() As ModelDoc2)
        Dim bSuccess As Boolean
        Dim swErrors As Long
        Dim swWarnings As Long
        Dim i As Integer
        Dim sErrorFiles As String = ""

        For i = 0 To modDocArr.Length - 1
            If modDocArr(i) Is Nothing Then Continue For
            bSuccess = modDocArr(i).Save3(1, swErrors, swWarnings)
            If Not bSuccess Then
                sErrorFiles &= modDocArr(i).GetPathName & vbCrLf & " Errors: " & swErrors & vbCrLf &
                    "Warnings: " & swWarnings & vbCrLf
            End If
        Next

        If sErrorFiles <> "" Then
            iSwApp.SendMsgToUser("Error could not save the following file. For Error Codes, google swFileSaveError_e " &
                     "and for Warnings google swFileSaveWarning_e " & vbCrLf &
                     sErrorFiles)
            Exit Sub
        End If
    End Sub
    Structure lockStatus
        Public eDisposition As lockDisposition
        Public sFilePaths() As String
    End Structure
    Enum lockDisposition
        noSteal
        stealAndOverwrite
        stealAndDoNotOverwrite
        unknown
    End Enum

    'Private Shared sbOutputLines As System.Text.StringBuilder = Nothing

    Public Function getFileSVNStatus(ByVal bCheckServer As Boolean,
                              Optional ByRef modDocArr() As ModelDoc2 = Nothing,
                              Optional ByVal iRecursiveLevel As Integer = 0) As SVNStatus
        'Pass sFilePath = Create from the file path
        'Pass modDocArr = create from the modDocArr
        'Pass Neither = create for entire repo

        Dim oSVNProcess As New Process()
        Dim SVNstartInfo As New ProcessStartInfo
        Dim modDocTemp As ModelDoc2
        Dim sOutputLines() As String
        Dim sOutputErrorLines() As String
        'Dim sLine2 As String
        Dim SVNOutput As String
        Dim SVNErrorOutput As String
        Dim bSuccess As Boolean = False
        Dim sFilePathCat As String = ""
        Dim sFilePathTemp As String
        Dim iLineStep As Integer = 1
        Dim sModDocPathArr() As String = getFilePathsFromModDocArr(modDocArr)
        Dim sFileStartIndex As String
        Dim sCatMessage As String = ""

        'Dim iOutputUbound As Integer
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim bExpectStatusAgainstRevision As Boolean = False
        Dim Index As Integer

        'SVNstartInfo.Arguments = "status " & If(bCheckServer, "-u ", "") & "-v --non-interactive E:\SolidworksBackup\svn " 'sFilePathCat 

        SVNstartInfo.Arguments = "status " & If(bCheckServer, "-u ", "") & "-v --non-interactive " & localRepoPath.text  'sFilePathCat 
        SVNstartInfo.FileName = sSVNPath
        SVNstartInfo.UseShellExecute = False
        SVNstartInfo.RedirectStandardOutput = True
        SVNstartInfo.RedirectStandardError = True
        SVNstartInfo.CreateNoWindow = True
        oSVNProcess.StartInfo = SVNstartInfo

        '============
        'sbOutputLines = New System.Text.StringBuilder()

        ' Set our event handler to asynchronously read the sort output.
        'AddHandler oSVNProcess.OutputDataReceived, AddressOf SortOutputHandler

        oSVNProcess.Start()

        ''Monitor the process. Kill it if it stops responding
        'Dim nResponding As Integer = 0
        'Dim totalTime As Integer = 0
        'Do While (Not oSVNProcess.HasExited)
        '    nResponding += (Not oSVNProcess.Responding)
        '    totalTime += 1
        '    If (nResponding > 1000) Or (totalTime > 10000) Then 'Sort of milliseconds because of the sleep command. But not exactly.
        '        oSVNProcess.Kill()

        '        If iSwApp.SendMsgToUser2("SVN timed out While attempting To connect To the vault. " &
        '                              "Would you Like To switch To offline?",
        '                              swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
        '            onlineCheckBox.Checked = False
        '        End If
        '        Return Nothing
        '    End If
        '    System.Threading.Thread.Sleep(1)
        'Loop

        'Using  async
        'SVNOutput = sbOutputLines.ToString

        'Using Sync
        Using ostreamreader As System.IO.StreamReader = oSVNProcess.StandardOutput
            SVNOutput = ostreamreader.ReadToEnd()
        End Using
        Using ostreamreader As System.IO.StreamReader = oSVNProcess.StandardError
            SVNErrorOutput = ostreamreader.ReadToEnd()
        End Using

        If Not oSVNProcess.WaitForExit(10000) Then
            'If the process doesn't finish after 10s then kill it and send error message to user
            oSVNProcess.Kill()
            If iSwApp.SendMsgToUser2("SVN timed out While attempting To connect To the vault. " &
                                  "Would you Like To switch To offline?",
                                  swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
                onlineCheckBox.Checked = False
            End If
            Return Nothing
        End If



        sOutputLines = SVNOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        sOutputErrorLines = SVNErrorOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        k = sOutputErrorLines.Length - 1
        'sOutputErrorLines = {""}

        Dim output As SVNStatus = New SVNStatus()
        statusOfAllOpenModels = output ' Be careful! This does not copy. This makes both point to the same memory! We will split/copy them later if theres no errors.
        'ReDim output.fp(UBound(sOutputLines))
        ReDim output.fp(sOutputLines.Length - 1)

        'Error Checking
        If (sOutputErrorLines Is Nothing) Or (sOutputLines Is Nothing) Then
            iSwApp.SendMsgToUser("Error: SVN status output standard error is nothing. Must of not connected/read to SVN process")
            Return Nothing
        End If

        If sOutputErrorLines.Length <> 0 Then
            'We got some errors if length > 0
            For i = 0 To UBound(sOutputErrorLines)
                If sOutputErrorLines(i).Contains("E215004") Then
                    'Log in Failed!
                    If iRecursiveLevel <> 0 Then
                        Return Nothing
                    End If
                    'Open a log in, and then try again. 
                    iSwApp.SendMsgToUser(catWithNewLine(sOutputErrorLines))
                    runTortoiseProcexeWithMonitor("/command:repostatus /remote /path:" & localRepoPath.text) 'log in
                    Return getFileSVNStatus(bCheckServer, modDocArr, iRecursiveLevel:=1)
                ElseIf sOutputErrorLines(i).Contains("E170013") Then
                    'Couldn't connect. Server is off or no internet connection
                    If iSwApp.SendMsgToUser2("SVN timed out while attempting to connect to the vault. " &
                      "Would you like to switch to offline? " & vbCrLf & vbCrLf & vbCrLf & "Error Message Below" &
                      catWithNewLine(sOutputErrorLines),
                      swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
                        onlineCheckBox.Checked = False
                    End If
                    Return Nothing
                ElseIf sOutputErrorLines(i).Contains("W155007:") Then
                    'Common error. File not saved into repository
                    sCatMessage &= vbCrLf &
                        sOutputErrorLines(i) & vbCrLf &
                        "Error W155007 File is not saved inside repository. " &
                        "Save the file inside the repository and try again. "
                Else
                    'Other Errors
                    sCatMessage &= vbCrLf &
                        sOutputErrorLines(i) & vbCrLf &
                        "Error: " & sOutputErrorLines(i)
                End If
            Next i
        End If

        If sOutputLines.Length = 0 Then
            If sCatMessage <> "" Then
                iSwApp.SendMsgToUser(sCatMessage)
                'Unknown other error. Continue running svnstatus function.
            Else
                iSwApp.SendMsgToUser(sCatMessage & vbCrLf & "Error: No Usable output lines returned from SVN. " &
                    "Possible Reasons: No connection to server.")
            End If
            Return Nothing
        End If

        If (bCheckServer) Then
            If sOutputLines(0).Substring(0, 23) = "Status against revision" Then
                iSwApp.SendMsgToUser("Status Returned from SVN Server with No Items") 'If you change the string, change it other places in the code too!
                Return output
            ElseIf (sOutputLines.Length = 1) Then
                'If we are checking the server, we should expect a line 2. If its not there then theres an error.
                iSwApp.SendMsgToUser("Error: Incomplete SVN Status. Could not Read Line 2. Line 1:" & sOutputLines(0))
                Return output
            End If
        End If

        ReDim output.fp(UBound(sOutputLines))
        statusOfAllOpenModels = output.Clone

        For i = 0 To UBound(sOutputLines)
            If sOutputLines(i).Substring(0, 23) = "Status against revision" Then Continue For
            If sOutputLines(i).Contains("~$") Then Continue For 'Temporary file!
            sFileStartIndex = Strings.InStr(sOutputLines(i), localRepoPath.text, CompareMethod.Text) - 1
            If sFileStartIndex = -2 Then Continue For
            sFilePathTemp = sOutputLines(i).Substring(sFileStartIndex, sOutputLines(i).Length - sFileStartIndex)

            modDocTemp = iSwApp.GetOpenDocumentByName(sFilePathTemp)
            If modDocTemp Is Nothing Then Continue For

            statusOfAllOpenModels.addOutputLineToSVNStatus(sOutputLines(i), m, sFilePathTemp, modDocTemp, bCheckServer)
            m = m + 1

            If Not IsNothing(modDocArr) Then
                Index = findIndexContains(sModDocPathArr, sFilePathTemp)
                If Index = -1 Then Continue For
                output.addOutputLineToSVNStatus(sOutputLines(i), j, sFilePathTemp, modDocTemp, bCheckServer)
                j += 1
            End If
        Next i

        If j > 0 Then ReDim Preserve output.fp(j - 1)
        If m > 0 Then ReDim Preserve statusOfAllOpenModels.fp(m - 1)

        If IsNothing(modDocArr) Then
            Return statusOfAllOpenModels
        Else
            Return output
        End If



    End Function

    'Private Shared Sub SortOutputHandler(sendingProcess As Object,
    '       outLine As DataReceivedEventArgs)

    '    ' Collect the sort command output.
    '    If Not String.IsNullOrEmpty(outLine.Data) Then
    '        ' Add the text to the collected output.
    '        sbOutputLines.Append(vbCrLf & outLine.Data)
    '    End If
    'End Sub

    Public Shared Function findIndexContains(ByVal sLookInArr() As String, ByVal find As String) As Integer
        Dim i As Integer
        'Dim output As Integer
        For i = 0 To UBound(sLookInArr)
            'If sLookInArr(i).Contains(find) Then Return i
            If (Strings.InStr(sLookInArr(i), find, CompareMethod.Text) <> 0) Then Return i
        Next
        Return -1
    End Function
    Function findStatusForFile(ByRef sFileName As String) As SVNStatus
        Dim i As Integer
        Dim bSuccess As Boolean
        Dim output As SVNStatus = New SVNStatus()

        If IsNothing(statusOfAllOpenModels) Then
            bSuccess = updateStatusOfAllModelsVariable()
            If Not bSuccess Then Return Nothing
        End If

        ReDim output.fp(0)

        For i = 0 To UBound(statusOfAllOpenModels.fp)
            If (Strings.InStr(statusOfAllOpenModels.fp(i).filename, sFileName, CompareMethod.Text) <> 0) Then
                Exit For
            End If
        Next
        If i = (UBound(statusOfAllOpenModels.fp) + 1) Then Return Nothing
        output.fp(0) = statusOfAllOpenModels.fp(i)
        Return output
    End Function
    Public Sub switchTreeViewToCurrentModel(Optional bRetryWithRefresh As Boolean = True)

        If Not onlineCheckBox.Checked Then Exit Sub

        TreeView1.BeginUpdate()
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()

        Dim treeNodeTemp As TreeNode = findStoredTreeView(modDoc.GetPathName, bRetryWithRefresh)
        If IsNothing(treeNodeTemp) Then Exit Sub

        Dim clonedNode As TreeNode = CType(treeNodeTemp.Clone(), TreeNode)

        TreeView1.Nodes.Clear()
        TreeView1.Nodes.Insert(0, clonedNode)
        TreeView1.Nodes(0).Expand()
        TreeView1.Show()
        TreeView1.EndUpdate()
    End Sub
    Function findStoredTreeView(pathName As String, Optional bRetryWithRefresh As Boolean = True) As TreeNode
        Dim i As Integer
        Dim bSuccess As Boolean
        'Dim bFound As Boolean = False

        If IsNothing(allTreeViews(0)) Then
            bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
            If Not bSuccess Then iSwApp.SendMsgToUser("Status Update Failed.") : Return Nothing

            bRetryWithRefresh = False
        End If

        'Try to find it using the existing allTreeViews object. This is the fastest
        For i = 0 To UBound(allTreeViews)
            If allTreeViews(i).Nodes.Count = 0 Then Continue For
            If (Strings.InStr(allTreeViews(i).Nodes(0).Text, System.IO.Path.GetFileName(pathName), CompareMethod.Text) <> 0) Then
                Return allTreeViews(i).Nodes(0)
            End If
        Next

        If Not bRetryWithRefresh Then Return Nothing
        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then iSwApp.SendMsgToUser("Status Update Failed.") : Return Nothing

        For i = 0 To UBound(allTreeViews)
            If (Strings.InStr(allTreeViews(i).Nodes(0).Text, System.IO.Path.GetFileName(pathName), CompareMethod.Text) <> 0) Then
                Return allTreeViews(i).Nodes(0)
            End If
        Next

        Return Nothing 'Couldn't find it!

    End Function
    Sub refreshAllTreeViewsVariable()
        Dim modDocArray As ModelDoc2() = getAllOpenDocs(bMustBeVisible:=True)
        Dim i As Integer
        ReDim allTreeViews(UBound(modDocArray))
        'allTreeViews(UBound(modDocArray)) = New TreeView()

        For i = 0 To UBound(modDocArray)
            If modDocArray(i) Is Nothing Then Continue For
            allTreeViews(i) = New TreeView
            getComponentsOfAssemblyOptionalUpdateTree(modDocArray(i), i)
        Next
    End Sub

    Public Function getComponentsOfAssemblyOptionalUpdateTree(
                                    ByRef modDoc As ModelDoc2,
                                    Optional ByVal allTreeViewsIndexToUpdate As Integer = Nothing) As ModelDoc2()

        Dim bUC As Boolean = If(IsNothing(allTreeViewsIndexToUpdate), False, True)
        Dim sFileNameTemp As String = System.IO.Path.GetFileName(modDoc.GetPathName)
        Dim parentNode As TreeNode = Nothing

        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Couldn't find model") : Return Nothing

        If bUC Then
            allTreeViews(allTreeViewsIndexToUpdate).Nodes.Clear()
            parentNode = New TreeNode(sFileNameTemp)
            parentNode.Tag = modDoc
        End If

        If modDoc.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            If bUC Then
                setNodeColorFromStatus(parentNode)
                allTreeViews(allTreeViewsIndexToUpdate).Nodes.Add(parentNode)
            End If
            Return {modDoc}
            'iswApp.SendMsgToUser("Error: Model is not an assembly.")
            'Throw New System.Exception("modDoc is not an Assembly")
        End If

        Dim mdComponentList As New List(Of ModelDoc2)()
        Dim swConfMgr As ConfigurationManager
        Dim swConf As Configuration
        Dim swRootComp As Component2

        swConfMgr = modDoc.ConfigurationManager
        swConf = swConfMgr.ActiveConfiguration
        swRootComp = swConf.GetRootComponent3(True)

        TraverseComponent(swRootComp, mdComponentList, 1, parentNode)

        Dim mdComponentArr() As ModelDoc2 = mdComponentList.ToArray
        If bUC Then allTreeViews(allTreeViewsIndexToUpdate).Nodes.Add(parentNode)

        Return mdComponentArr
    End Function

    Sub TraverseComponent(
                         ByRef swComp As Component2,
                         ByRef mdComponentList As List(Of ModelDoc2),
                         ByVal nLevel As Long,
                         Optional ByRef rootNode As TreeNode = Nothing)

        'https://help.solidworks.com/2016/English/api/sldworksapi/Traverse_Assembly_at_Component_and_Feature_Levels_Using_Recursion_Example_VBNET.htm
        Dim bUC As Boolean = If(rootNode Is Nothing, False, True)
        Dim vChildComp As Object
        Dim swChildComp As Component2
        Dim sPadStr As String = " "
        Dim i As Long
        Dim modDocChild As ModelDoc2
        Dim sChildFileName As String
        Dim parentNode As TreeNode = Nothing
        Dim childNode As TreeNode = Nothing
        Dim modDocParent As ModelDoc2 = swComp.GetModelDoc2
        Dim sParentFileName As String = System.IO.Path.GetFileName(modDocParent.GetPathName)
        Dim tempStatus As SVNStatus = New SVNStatus()

        mdComponentList.Add(modDocParent)
        If bUC Then
            'If nLevel = 1 Then
            'ParentNode = rootNode
            'Else
            parentNode = New TreeNode(sParentFileName)
            parentNode.Tag = modDocParent
            setNodeColorFromStatus(parentNode)
            'End If
        End If

        vChildComp = swComp.GetChildren
        For i = 0 To UBound(vChildComp)
            swChildComp = vChildComp(i)
            modDocChild = swChildComp.GetModelDoc2
            If IsNothing(modDocChild) Then
                Continue For
            End If
            If modDocChild.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
                'Is part file
                If mdComponentList.Contains(modDocChild) Then Continue For 'avoid duplicates
                If bUC Then
                    sChildFileName = System.IO.Path.GetFileName(modDocChild.GetPathName)
                    childNode = New TreeNode(sChildFileName)
                    childNode.Tag = modDocChild
                    setNodeColorFromStatus(childNode)
                    parentNode.Nodes.Add(childNode)
                End If

                mdComponentList.Add(modDocChild)
            Else
                'Is assembly
                TraverseComponent(swChildComp, mdComponentList, nLevel + 1, parentNode)
            End If
        Next i

        If bUC Then
            If nLevel = 1 Then
                rootNode = parentNode
            Else
                rootNode.Nodes.Add(parentNode)
            End If
        End If

    End Sub
    Class myContextMenuClass

        Dim iSwApp2 As SldWorks
        Dim modDoc As ModelDoc2
        Public openLabel As New ToolStripMenuItem("Open", My.Resources.VaultLogo128, AddressOf openEventHandler)
        Public unlockLabel As New ToolStripMenuItem("Release Lock", My.Resources.VaultLogo128, AddressOf unlockEventHandler)
        Public unlockWithDependentsLabel As New ToolStripMenuItem("Release Lock With Dependents", My.Resources.VaultLogo128, AddressOf unlockWithDependentsEventHandler)
        Public checkInLabel As New ToolStripMenuItem("Check In", My.Resources.VaultLogo128, AddressOf checkInEventHandler)
        Public checkInWithDependentsLabel As New ToolStripMenuItem("Check In With Dependents", My.Resources.VaultLogo128, AddressOf checkInWithDependentsEventHandler)
        Public checkOutStealLabel As New ToolStripMenuItem("Check Out (Steal Locks)", My.Resources.VaultLogo128, AddressOf checkOutStealLockEventHandler)
        Public checkOutActiveDoc As New ToolStripMenuItem("Check Out Active Doc", My.Resources.VaultLogo128, AddressOf checkOutActiveDocEventHandler)
        Public checkOutWithDependents As New ToolStripMenuItem("Check Out With Dependents", My.Resources.VaultLogo128, AddressOf checkOutActiveWithDependentsEventHandler)
        Public Sub New(modDocInput As ModelDoc2, iSwAppInput As SldWorks)
            modDoc = modDocInput
            iSwApp2 = iSwAppInput
        End Sub
        Sub openEventHandler(sender As Object, e As EventArgs)
            iSwApp2.ActivateDoc3(modDoc.GetPathName, True, swRebuildOnActivation_e.swUserDecision, 0)
        End Sub
        Sub unlockEventHandler(sender As Object, e As EventArgs)
            iSwApp2.unlockDocs({modDoc})
        End Sub
        Sub unlockWithDependentsEventHandler(sender As Object, e As EventArgs)
            iSwApp2.myUnlockWithDependents({modDoc})
        End Sub
        Sub checkInEventHandler(sender As Object, e As EventArgs)
            iSwApp2.checkInDocs({modDoc}, createBoolArray(1, True))
        End Sub
        Sub checkInWithDependentsEventHandler(sender As Object, e As EventArgs)
            iSwApp2.myCheckinWithDependents({modDoc})
        End Sub
        Sub checkOutStealLockEventHandler(sender As Object, e As EventArgs)
            If swMessageBoxResult_e.swMbHitOk =
            iSwApp2.SendMsgToUser2("File is Currently checked out by another user. You can steal their " &
                                   "Locks by clicking the checkbox in the next window. If both you and that user " &
                                   "attempt to check in their copies, a conflict can occur. Always communicate " &
                                   "your intention to break someone's lock with that user.",
                                    swMessageBoxIcon_e.swMbWarning, swMessageBoxBtn_e.swMbOkCancel) Then
                iSwApp2.unlockDocs({modDoc})
            End If
        End Sub
        Sub checkOutActiveDocEventHandler(sender As Object, e As EventArgs)
            iSwApp2.unlockDocs({modDoc})
        End Sub
        Sub checkOutActiveWithDependentsEventHandler(sender As Object, e As EventArgs)
            iSwApp2.myunlockwithDependents(modDoc)
        End Sub

    End Class
    Sub setNodeColorFromStatus(
        ByRef rootNode As TreeNode)
        Dim myCol As myColours = New myColours()
        myCol.initialize()
        Dim status1 As SVNStatus = findStatusForFile(rootNode.Text)

        Dim bCM As Boolean = If(IsNothing(rootNode.Tag), True, False)
        Dim myContextMenu As New myContextMenuClass(rootNode.Tag, iSwApp)

        Dim docMenu As ContextMenuStrip
        docMenu = New ContextMenuStrip()

        'If bCM Then
        '    rootNode.ContextMenuStrip.Items.Add(myContextMenu.openLabel)
        'End If

        If status1 Is Nothing Then
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"
            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.openLabel})

        ElseIf status1.fp(0).lock6 = "K" Then
            rootNode.BackColor = myCol.lockedByYou
            rootNode.ToolTipText = "Checked Out By You"

            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.checkInLabel, myContextMenu.checkInWithDependentsLabel, myContextMenu.unlockLabel, myContextMenu.unlockWithDependentsLabel})

            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkInLabel)
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkInWithDependentsLabel)
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.unlockLabel)
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.unlockWithDependentsLabel)
        ElseIf status1.fp(0).upToDate9 = "*" Then
            rootNode.BackColor = myCol.outOfDate
            rootNode.ToolTipText = "Your Copy is Out Of Date"
            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.checkOutStealLabel})

        ElseIf status1.fp(0).lock6 = "O" Then
            rootNode.BackColor = myCol.lockedBySomeoneElse
            rootNode.ToolTipText = "Locked By Someone Else"
            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.checkOutStealLabel})
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkOutStealLabel)

        ElseIf status1.fp(0).lock6 = " " Then
            rootNode.BackColor = myCol.available
            rootNode.ToolTipText = "Available"
            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.checkOutActiveDoc, myContextMenu.checkOutWithDependents})
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkOutActiveDoc)
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkOutWithDependents)

        Else
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"
            If bCM Then docMenu.Items.AddRange(New ToolStripMenuItem() _
                {myContextMenu.openLabel})

        End If

        rootNode.ContextMenuStrip = docMenu
    End Sub
    Class myColours
        Public lockedByYou As Drawing.Color
        Public lockedBySomeoneElse As Drawing.Color
        Public available As Drawing.Color
        Public unknown As Drawing.Color
        Public outOfDate As Drawing.Color
        Public Sub initialize()
            lockedByYou = Drawing.Color.FromArgb(159, 223, 159) 'Drawing.Color.Aquamarine
            lockedBySomeoneElse = Drawing.Color.FromArgb(174, 183, 207) 'Drawing.Color.Khaki
            available = Drawing.Color.White
            unknown = Drawing.Color.LightGray
            outOfDate = Drawing.Color.FromArgb(255, 129, 123)
            'Drawing.Color.Bisque 'Drawing.Color.FromArgb(255, 77, 77) 'light red
        End Sub
    End Class
    Class SVNStatus
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

            For i = 0 To UBound(fp)
                If fp(i).modDoc Is Nothing Then Continue For
                If fp(i).lock6 = "K" Then
                    ' The user got a lock! Let's change to write access
                    fp(i).modDoc.SetReadOnlyState(False)
                    fp(i).bReconnect = False
                    'save3AndShowErrorMessages(modDoc)
                Else
                    'User didn't get a lock.
                    fp(i).modDoc.SetReadOnlyState(True)
                    fp(i).bReconnect = True
                End If
            Next
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
End Class
