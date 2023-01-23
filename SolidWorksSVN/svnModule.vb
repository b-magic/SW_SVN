Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports System.Configuration

Public Module svnModule

    Dim myUserControl As UserControl1
    Dim iSwApp As SldWorks
    Dim statusOfAllOpenModels As SVNStatus

    Public sSVNPath As String '= "C:\Program Files\TortoiseSVN\bin\svn.exe"
    Public sTortPath As String '= "C:\Users\benne\Documents\SVN\TortoiseProc.exe"
    Public sInstallDirectory As String

    Friend Sub svnModuleInitialize(
                                  mySwAppPass As SldWorks,
                                  myUserControlPass As UserControl1,
                                  statusOfAllOpenModelsPass As SVNStatus)
        sInstallDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        sSVNPath = sInstallDirectory & "\bin\svn.exe"
        If Not My.Computer.FileSystem.FileExists(sSVNPath) Then
            sSVNPath = sInstallDirectory & "\svn.exe" 'Try a slightly different path
            If Not My.Computer.FileSystem.FileExists(sSVNPath) Then
                iSwApp.SendMsgToUser2("Error: " & sInstallDirectory & "\bin\svn.exe" & "does not exist.",
                                    swMessageBoxIcon_e.swMbStop, swMessageBoxBtn_e.swMbOk)
                myUserControlPass.onlineCheckBox.Checked = False
            End If
        End If

        sTortPath = sInstallDirectory & "\bin\TortoiseProc.exe"  'System.Environment.CurrentDirectory & "\TortoiseProc.exe"
        If Not My.Computer.FileSystem.FileExists(sTortPath) Then
            sTortPath = sInstallDirectory & "\TortoiseProc.exe" 'Try a slightly different path
            If Not My.Computer.FileSystem.FileExists(sTortPath) Then
                iSwApp.SendMsgToUser2("Error: " & sInstallDirectory & "\bin\TortoiseProc.exe" & "does not exist.",
                                       swMessageBoxIcon_e.swMbStop, swMessageBoxBtn_e.swMbOk)
                myUserControlPass.onlineCheckBox.Checked = False
            End If
        End If

        myUserControl = myUserControlPass
        iSwApp = mySwAppPass
        statusOfAllOpenModels = statusOfAllOpenModelsPass

    End Sub

    Public Function updateStatusOfAllModelsVariable(Optional bRefreshAllTreeViews As Boolean = False) As Boolean

        Dim output As SVNStatus = getFileSVNStatus(bCheckServer:=True, getAllOpenDocs(bMustBeVisible:=False))

        If IsNothing(output) Then
            Return False
        ElseIf output.fp.Length = 0 Then
            Return False
        End If

        If bRefreshAllTreeViews Then myUserControl.refreshAllTreeViewsVariable()
        Return True
    End Function

    Public Function getFileSVNStatus(ByVal bCheckServer As Boolean,
                              Optional ByRef modDocArr() As ModelDoc2 = Nothing,
                              Optional ByVal iRecursiveLevel As Integer = 0) As SVNStatus
        'Pass sFilePath = Create from the file path
        'Pass modDocArr = create from the modDocArr
        'Pass Neither = create for entire repo

        Dim modDocTemp As ModelDoc2
        Dim sOutputLines() As String
        Dim sOutputErrorLines() As String
        'Dim sLine2 As String
        Dim bSuccess As Boolean = False
        Dim sFilePathCat As String = ""
        Dim sFilePathTemp As String
        Dim iLineStep As Integer = 1
        Dim sModDocPathArr() As String = getFilePathsFromModDocArr(modDocArr)
        Dim sFileStartIndex As String
        Dim sCatMessage As String = ""
        Dim arguments As String

        Dim processOutput As rawProcessReturn

        'Dim iOutputUbound As Integer
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim bExpectStatusAgainstRevision As Boolean = False
        Dim Index As Integer

        'SVNstartInfo.Arguments = "status " & If(bCheckServer, "-u ", "") & "-v --non-interactive E:\SolidworksBackup\svn " 'sFilePathCat 

        If Not verifyLocalRepoPath() Then Return Nothing

        arguments = "status " & If(bCheckServer, "-u ", "") & "-v --non-interactive '" & myUserControl.localRepoPath.Text & "'" 'sFilePathCat 

        'iSwApp.SendMsgToUser(sSVNPath)
        processOutput = runSvnProcess(sSVNPath, arguments)

        sOutputLines = processOutput.output.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        sOutputErrorLines = processOutput.outputError.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

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
                    iSwApp.SendMsgToUser(svnAddInUtils.catWithNewLine(sOutputErrorLines))

                    'https://tortoisesvn.net/docs/nightly/TortoiseSVN_en/tsvn-automation.html
                    runTortoiseProcexeWithMonitor("/command:repostatus /remote /path: '" & myUserControl.localRepoPath.Text & "'") 'log in
                    Return getFileSVNStatus(bCheckServer, modDocArr, iRecursiveLevel:=1)
                ElseIf sOutputErrorLines(i).Contains("E170013") Then
                    'Couldn't connect. Server is off or no internet connection
                    If iSwApp.SendMsgToUser2("SVN timed out while attempting to connect to the vault. " &
                      "Would you like to switch to offline? " & vbCrLf & vbCrLf & "Error Message Below" &
                      catWithNewLine(sOutputErrorLines),
                      swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
                        myUserControl.onlineCheckBox.Checked = False
                    End If
                    Return Nothing
                ElseIf sOutputErrorLines(i).Contains("W155007:") Then
                    'Common error. File not saved into repository. Or folder is not connected to a repository.
                    sCatMessage &= vbCrLf &
                        sOutputErrorLines(i) & vbCrLf &
                        "Error W155007 the path is not associated with a repository. " &
                        "You may need to either checkout the repository to the folder with tortoiseSVN, " &
                        "or save the file inside an existing local repository And try again. "
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
            sFileStartIndex = Strings.InStr(sOutputLines(i), myUserControl.localRepoPath.Text, CompareMethod.Text) - 1
            If sFileStartIndex = -2 Then Continue For
            sFilePathTemp = sOutputLines(i).Substring(sFileStartIndex, sOutputLines(i).Length - sFileStartIndex)

            modDocTemp = iSwApp.GetOpenDocumentByName(sFilePathTemp)
            If modDocTemp Is Nothing Then Continue For

            statusOfAllOpenModels.addOutputLineToSVNStatus(sOutputLines(i), m, sFilePathTemp, modDocTemp, bCheckServer)
            m = m + 1

            If Not IsNothing(modDocArr) Then
                Index = svnAddInUtils.findIndexContains(sModDocPathArr, sFilePathTemp)
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
    Function runSvnProcess(filename As String, arguments As String) As rawProcessReturn

        Dim iWaitTime As Integer = 10000 'milliseconds to wait for the SVN process to finish

        Dim output As rawProcessReturn
        Dim oSVNProcess As New Process()
        Dim SVNstartInfo As New ProcessStartInfo
        SVNstartInfo.Arguments = arguments
        SVNstartInfo.FileName = filename
        SVNstartInfo.UseShellExecute = False
        SVNstartInfo.RedirectStandardOutput = True
        SVNstartInfo.RedirectStandardError = True
        SVNstartInfo.CreateNoWindow = True
        oSVNProcess.StartInfo = SVNstartInfo

        '============
        'sbOutputLines = New System.Text.StringBuilder()

        ' Set our event handler to asynchronously read the sort output.
        'AddHandler oSVNProcess.OutputDataReceived, AddressOf SortOutputHandler

        'iSwApp.SendMsgToUser(filename & vbCrLf & arguments)

        oSVNProcess.Start()

        'Using Sync
        Using ostreamreader As System.IO.StreamReader = oSVNProcess.StandardOutput
            output.output = ostreamreader.ReadToEnd()
        End Using
        Using ostreamreader As System.IO.StreamReader = oSVNProcess.StandardError
            output.outputError = ostreamreader.ReadToEnd()
        End Using

        Do While Not oSVNProcess.WaitForExit(iWaitTime)
            'If the process doesn't finish after 10s then kill it and send error message to user
            oSVNProcess.Kill()
            If iSwApp.SendMsgToUser2("SVN timed out While attempting To connect To the vault. " &
                                  "Would you like to give it more time?",
                                  swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
                iSwApp.SendMsgToUser("Switching to offline mode")
                myUserControl.onlineCheckBox.Checked = False
                Return Nothing
            Else
                iWaitTime += 5000
            End If
        Loop


        Return output
    End Function

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
            unlockDocs(myUserControl.getComponentsOfAssemblyOptionalUpdateTree(modDoc(0)))
        End If
    End Sub
    Sub unlockDocs(Optional ByRef modDocArr() As ModelDoc2 = Nothing)
        Dim bSuccess As Boolean
        Dim Status As SVNStatus

        If IsNothing(modDocArr) Then
            If Not verifyLocalRepoPath() Then Exit Sub
            bSuccess = runTortoiseProcexeWithMonitor("/command:unlock /path:'" & myUserControl.localRepoPath.Text & "' /closeonend:3")

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

    Public Sub myCheckinWithDependents(modDoc As ModelDoc2)

        Dim modDocArr1() As ModelDoc2
        Dim bRequired() As Boolean

        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
            modDocArr1 = myUserControl.getComponentsOfAssemblyOptionalUpdateTree(modDoc)
            bRequired = svnAddInUtils.createBoolArray(UBound(modDocArr1), False)
            bRequired(0) = True
            checkInDocs(modDocArr1, bRequired)
        Else
            modDocArr1 = {modDoc}
            checkInDocs(modDocArr1, svnAddInUtils.createBoolArray(1, True))
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

        If bRequiredDoc Is Nothing Then bRequiredDoc = svnAddInUtils.createBoolArray(UBound(modDocArr), True)

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

        myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        statusOfAllOpenModels.setReadWriteFromLockStatus()
    End Sub
    Public Sub myCheckinAll()
        Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String

        'Dim i As Integer
        'Dim index As Integer

        iSwApp.RunCommand(19, vbEmpty) 'Save All


        If Not verifyLocalRepoPath() Then Exit Sub
        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:""" & myUserControl.localRepoPath.Text & """ /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("TortoiseSVN Process Failed.") : Exit Sub

        'Switch over files to read-only
        'OpenDocPathList = CType(getAllOpenDocs(True, True), String())
        Dim OpenDocModels() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=True)

        'Dim sOpenDocPath() As String = getFilePathsFromModDoiSwApp.SendMsgToUser("Active Document not found") cArr(OpenDocModels)

        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then Exit Sub

        myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        statusOfAllOpenModels.setReadWriteFromLockStatus()

    End Sub
    Public Sub myCheckoutDoc(modDoc As ModelDoc2)
        'Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        checkoutDocs({modDoc})
    End Sub
    Public Sub myCheckoutWithDependents(modDoc As ModelDoc2)
        'Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        checkoutDocs(myUserControl.getComponentsOfAssemblyOptionalUpdateTree(modDoc))
    End Sub
    Sub myRepoStatus()
        Dim bSuccess As Boolean
        Dim modDoc As ModelDoc2
        Dim modDocArr() As ModelDoc2

        If iSwApp.ActiveDoc Is Nothing Then
            iSwApp.SendMsgToUser("A File must be open")
            Exit Sub
            'bSuccess = runTortoiseProcexeWithMonitor("/command:repostatus /remote")
        Else
            modDoc = iSwApp.ActiveDoc
            modDocArr = myUserControl.getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc)
            bSuccess = runTortoiseProcexeWithMonitor("/command:repostatus /path:" &
                                                 formatModDocArrForTortoiseProc(modDocArr) &
                                                 " /remote")
        End If
        If Not bSuccess Then iSwApp.SendMsgToUser("Status Check Failed.")
    End Sub
    Sub myCleanupAndRelease()
        Dim bSuccessStatus As Boolean
        Dim bSuccessCleanup As Boolean
        Dim allOpenDocs As ModelDoc2() = getAllOpenDocs(bMustBeVisible:=False)

        bSuccessStatus = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)

        If Not verifyLocalRepoPath() Then Exit Sub
        If bSuccessStatus Then
            bSuccessCleanup = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:'" & myUserControl.localRepoPath.Text & "'")
        Else
            'Manually release file system locks
            For Each modDoc In allOpenDocs
                modDoc.ForceReleaseLocks()
            Next

            bSuccessCleanup = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:'" & myUserControl.localRepoPath.Text & "'")
            For Each modDoc In allOpenDocs
                'Manually reattach to file system
                modDoc.ReloadOrReplace(ReadOnly:=True, ReplaceFileName:=False, DiscardChanges:=False)
            Next
        End If

        If bSuccessCleanup Then
            bSuccessStatus = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
            If bSuccessStatus Then
                statusOfAllOpenModels.setReadWriteFromLockStatus()
                myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
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

        myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)

        statusOfAllOpenModels.setReadWriteFromLockStatus()

    End Sub
    Function verifyLocalRepoPath(Optional bInteractive As Boolean = True) As Boolean

        Dim response As swMessageBoxResult_e
        Dim processOutput As rawProcessReturn
        Dim arguments As String
        Dim sLocalPath As String

        If IsNothing(myUserControl) Then Return False

        sLocalPath = myUserControl.localRepoPath.Text

        If Not myUserControl.onlineCheckBox.Checked Then Return False

        'Check the file exists on the computer
        If Not My.Computer.FileSystem.DirectoryExists(sLocalPath) Then
            If Not bInteractive Then Return False
            response = iSwApp.SendMsgToUser2(
                "Local Folder Location " & vbCrLf & sLocalPath & vbCrLf &
                "was not found. Would you like to select a new folder? ",
                swMessageBoxIcon_e.swMbWarning,
                swMessageBoxBtn_e.swMbYesNo)
            If response = swMessageBoxResult_e.swMbHitYes Then

                If (myUserControl.pickFolder() = System.Windows.Forms.DialogResult.OK) Then
                    Return verifyLocalRepoPath(bInteractive)
                Else
                    Return False
                End If
            ElseIf response = swMessageBoxResult_e.swMbHitNo Then
                iSwApp.SendMsgToUser2("Switching to offline.", swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
                myUserControl.onlineCheckBox.Checked = False
                Return False
            End If

        End If

        'Check the path is actually connected to a repo
        arguments = "info " & "--non-interactive '" & sLocalPath & "'" 'sFilePathCat 

        processOutput = runSvnProcess(sSVNPath, arguments)
        If processOutput.outputError.Contains("W155007:") Then
            If Not bInteractive Then Return False
            response = iSwApp.SendMsgToUser2("The following directory is not connected to an SVN Repository. " &
                                  "Would you like to download the entire vault to this folder? " & vbCrLf & sLocalPath,
                                    swMessageBoxIcon_e.swMbWarning,
                                    swMessageBoxBtn_e.swMbYesNo)
            If response = swMessageBoxResult_e.swMbHitYes Then
                '1. Checkout entire folder
                runTortoiseProcexeWithMonitor(" /command:checkout /path " & sLocalPath)

                Return verifyLocalRepoPath(bInteractive)
            End If

            response = iSwApp.SendMsgToUser2("The following directory is not connected to an SVN Repository. " &
                                  "Would you like to select a new folder? " & vbCrLf & sLocalPath,
                                    swMessageBoxIcon_e.swMbWarning,
                                    swMessageBoxBtn_e.swMbYesNo)
            If response = swMessageBoxResult_e.swMbHitYes Then

                If (myUserControl.pickFolder() = System.Windows.Forms.DialogResult.OK) Then
                    Return verifyLocalRepoPath(bInteractive)
                Else
                    Return False
                End If
            ElseIf response = swMessageBoxResult_e.swMbHitNo Then
                iSwApp.SendMsgToUser2("Switching to offline.", swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
                Return False
            Else
                Return False
            End If
        Else
            Return True
        End If

        Return False ' code shouldn't get here...

    End Function


    Sub myGetLatestOrRevert(Optional ByRef modDocArr As ModelDoc2() = Nothing,
                        Optional ByRef myGetType As getLatestType = getLatestType.update,
                            Optional ByRef bVerbose As Boolean = False)
        'Dim modDocTemp As ModelDoc2
        Dim i As Integer
        Dim j As Integer = 0
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
                myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
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
            myUserControl.switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        End If
    End Sub
    Public Enum getLatestType
        none
        revert
        update
        both
    End Enum
    Function formatFilePathArrForTortoiseProc(ByRef sFilePathArr() As String) As String
        Dim sFilePathCat As String = """" & sFilePathArr(0)
        For i = 0 To sFilePathArr.Length - 1
            If sFilePathArr(i) Is Nothing Then Continue For
            sFilePathCat &= "*" & sFilePathArr(i)
        Next
        sFilePathCat &= """"
        Return sFilePathCat
    End Function
    Function formatModDocArrForTortoiseProc(ByRef modDocArr() As ModelDoc2) As String
        Dim sFilePathCat As String = """" & modDocArr(0).GetPathName
        Dim sTempPathName As String
        For i = 1 To UBound(modDocArr)
            If modDocArr(i) Is Nothing Then Continue For
            sTempPathName = modDocArr(i).GetPathName
            If sTempPathName.Contains("~~") Then Continue For    'skip in-context parts/assemblies.
            sFilePathCat &= "*" & sTempPathName
        Next
        sFilePathCat &= """"
        Return sFilePathCat
    End Function

    Function runTortoiseProcexeWithMonitor(ByRef sArguments As String) As Boolean
        ' See https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html
        Dim oTortProcess As New Process()
        Dim tortStartInfo As New ProcessStartInfo

        tortStartInfo.FileName = sTortPath  'System.Environment.CurrentDirectory & "\\TortoiseProc.exe" 'AppDomain.CurrentDomain.BaseDirectory & 'sTortPath
        'iSwApp.SendMsgToUser(sTortPath)

        tortStartInfo.Arguments = sArguments
        If Not verifyLocalRepoPath() Then Return Nothing
        tortStartInfo.WorkingDirectory = myUserControl.localRepoPath.Text
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
    Public Function findStatusForFile(ByRef sFileName As String) As SVNStatus
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
    Public Structure rawProcessReturn
        Public output As String
        Public outputError As String
    End Structure
    Public Structure lockStatus
        Public eDisposition As lockDisposition
        Public sFilePaths() As String
    End Structure
    Public Enum lockDisposition
        noSteal
        stealAndOverwrite
        stealAndDoNotOverwrite
        unknown
    End Enum

End Module
