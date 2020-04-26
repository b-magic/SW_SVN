
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst


Imports System.Collections.Generic

<ProgId("SVN_AddIn")>
Public Class UserControl1

    Dim WithEvents iSwApp As SldWorks
    'Dim userAddin As SwAddin = New SwAddin() 'couldn't get access to swapp in here!
    Public Const sTortPath As String = "C:\Users\benne\Documents\SVN\TortoiseProc.exe"
    Public Const sRepoLocalPath As String = "C:\Users\benne\Documents\SVN\fsae1"
    Public Const sSVNPath As String = "C:\Program Files\TortoiseSVN\bin\svn.exe"

    Friend Sub getSwApp(ByRef swAppin As SldWorks)
        Debug.Print(vbCrLf & "=========================" & vbCrLf & "=========================" & vbCrLf & "=========================")
        iSwApp = swAppin
    End Sub
    Private Sub butCheckinWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckinWithDependents.Click
        myCheckinWithDependents()
        'Debug.Print("Hello")
        'Dim modDoc As ModelDoc2
        'modDoc = iSwApp.ActiveDoc()
        'Debug.Print(modDoc.GetPathName)
    End Sub

    Private Sub butCheckinAll_Click(sender As Object, e As EventArgs) Handles butCheckinAll.Click
        myCheckinAll()
    End Sub

    Private Sub butUnlockActive_Click(sender As Object, e As EventArgs) Handles butUnlockActive.Click
        myUnlockActive()
    End Sub

    Private Sub butUnlockWithDependents_Click(sender As Object, e As EventArgs) Handles butUnlockWithDependents.Click
        myUnlockWithDependents()
    End Sub

    Private Sub butCheckoutActiveDoc_Click(sender As Object, e As EventArgs) Handles butCheckoutActiveDoc.Click

        myCheckoutActiveDoc()
    End Sub

    Private Sub butCheckoutWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckoutWithDependents.Click
        myCheckoutWithDependents()
    End Sub

    Private Sub butGetLatestOpenOnly_Click(sender As Object, e As EventArgs) Handles butGetLatestOpenOnly.Click
        myGetLatestOpenOnly()
    End Sub

    Private Sub butGetLatestAllRepo_Click(sender As Object, e As EventArgs) Handles butGetLatestAllRepo.Click
        myGetLatestAllRepo()
    End Sub

    '============= Start sub definitions

    Public Sub myUnlockActive()
        unlockDocs(iswApp.ActiveDoc())
    End Sub
    Public Sub myUnlockWithDependents()
        Dim modDoc As ModelDoc2 = iswApp.ActiveDoc()
        If modDoc.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            unlockDocs(iswApp.ActiveDoc())
        Else
            unlockDocs(getComponentsOfAssembly(modDoc))
        End If
    End Sub
    Sub myUnlockAll()
        Dim bSuccess As Boolean = runTortoiseProcexeWithMonitor("/command:unlock /path:" & sRepoLocalPath & " /closeonend:3")
        If Not bSuccess Then iswApp.SendMsgToUser("Releasing Locks Failed.")
    End Sub
    Sub unlockDocs(ByRef modDocArr() As ModelDoc2)
        Dim bSuccess As Boolean = runTortoiseProcexeWithMonitor("/command:unlock /path:" &
                                         formatFilePathArrForTortoiseProc(
                                            getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")

        If Not bSuccess Then iswApp.SendMsgToUser("Releasing Locks Failed.")

    End Sub
    Public Sub myCheckinWithDependents()
        Dim modDocArr1() As ModelDoc2 = {iswApp.ActiveDoc()}
        If modDocArr1(0).GetType = swDocumentTypes_e.swDocASSEMBLY Then
            checkInDocs(getComponentsOfAssembly(modDocArr1(0)))
        Else
            checkInDocs(modDocArr1)
        End If
    End Sub
    Sub checkInDocs(ByRef modDocsArr() As ModelDoc2)

        'Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        'Dim sActiveDocPath As String = modDoc.GetPathName()
        Dim modDocArr() As ModelDoc2 = {iswApp.ActiveDoc()}
        'Dim swErrors As Long
        'Dim swWarnings As Long
        Dim bSuccess As Boolean = False
        Dim sErrorFiles As String = ""
        Dim i As Integer
        Dim j As Integer = 0

        For i = 0 To modDocsArr.Length - 1
            If modDocArr(i).IsOpenedReadOnly() Or modDocArr(i).IsOpenedViewOnly() Then
                sErrorFiles &= modDocArr(i).GetPathName & vbCrLf
                modDocArr(i) = Nothing
                'Else
                '    'Cleans up the loop to remove empty elements
                '    modDocArr(j) = modDocArr(i)
                '    j += 1
                '    modDocArr(i) = Nothing
            End If
        Next
        If sErrorFiles <> "" Then
            iswApp.SendMsgToUser("The following file(s) are Read-Only. You need write access to check in. " &
                                 "If you believe you have the file locked, you can try File > Reload" &
                                 sErrorFiles)
            Exit Sub
        End If

        save3AndShowErrorMessages(modDocArr)

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:" &
                                                 formatFilePathArrForTortoiseProc(
                                                    getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")

        setReadWriteFromLockStatus(modDocArr)

    End Sub
    Public Sub myCheckinAll()
        Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String

        'Dim i As Integer
        'Dim index As Integer

        iswApp.RunCommand(19, vbEmpty) 'Save All

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:""" & sRepoLocalPath & """ /closeonend:3")
        If Not bSuccess Then
            Exit Sub
        End If

        'Switch over files to read-only
        'OpenDocPathList = CType(getAllOpenDocs(True, True), String())
        Dim OpenDocModels() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=True)

        Dim sOpenDocPath() As String = getFilePathsFromModDocArr(OpenDocModels)

        setReadWriteFromLockStatus(OpenDocModels)

    End Sub
    Public Sub myCheckoutActiveDoc()
        checkoutDocs({iswApp.ActiveDoc()})
    End Sub
    Public Sub myCheckoutWithDependents()
        checkoutDocs(getComponentsOfAssembly(iswApp.ActiveDoc()))
    End Sub
    Sub myRepoStatus()
        Dim bSuccess As Boolean
        bSuccess = runTortoiseProcexeWithMonitor("/command:repostatus /path:" & sRepoLocalPath & " /remote")
        If Not bSuccess Then iswApp.SendMsgToUser("Status Check Failed.")
    End Sub
    Sub myCleanupAndRelease()
        Dim bSuccess As Boolean
        setReadWriteFromLockStatus(getAllOpenDocs(bMustBeVisible:=False))

        bSuccess = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:" & sRepoLocalPath)
        If Not bSuccess Then iswApp.SendMsgToUser("Cleanup Failed. This is often because the SVN server is attempting " &
                       "to open a file that SolidWorks is currently accessing. This occurs even when the file is read only. " &
                       "Try closing all open files and trying again. Or close SolidWorks and use ToroiseSVN to clean up. ")
    End Sub
    Sub checkoutDocs(ByRef modDocArr() As ModelDoc2)
        Dim modDoc As ModelDoc2 = iswApp.ActiveDoc()
        'Dim modDocArr() As ModelDoc2 = {modDoc}
        Dim sActiveDocPath() As String = getFilePathsFromModDocArr(modDocArr)
        Dim sDocPathsToCheckout(modDocArr.Length - 1) As String
        Dim status() As SVNStatus
        Dim bSuccess As Boolean = False
        Dim i As Integer
        Dim sCatMessage As String = ""

        'Using objReader As System.IO.TextReader = System.IO.File.OpenText("C:\Users\benne\AppData\Local\Temp\tmp9BF6.tmp") 'System.IO.StreamReader(sTempFileName)
        '    sLine1 = objReader.ReadLine()
        '    sLine2 = objReader.ReadLine()
        'End Using
        ''objReader.Close()
        ''My.Computer.FileSystem.DeleteFile(sTempFileName)
        'Debug.Print(sTempFileName)
        status = getFileSVNStatus(sActiveDocPath, True)

        If Not status(0).errorMessage Is Nothing Then
            ' Some sort of error from the svn command
            For i = 0 To status(0).errorMessage.Length - 1
                If status(0).errorMessage(i).Contains("W155007:") Then
                    'Common error. File not saved into repository
                    sCatMessage &= vbCrLf &
                        status(0).errorFile(i) & vbCrLf &
                            "Error W155007 File is not saved inside repository. " &
                            "Save the file inside the repository and try again. "

                ElseIf status(0).errorMessage(i) = "Status Returned from SVN Server with No Items" Then
                    sCatMessage &= vbCrLf &
                        status(0).errorFile(i) & vbCrLf &
                    "Error: Current save file For model Not found. Save it inside the repository."
                Else
                    'Other Errors
                    sCatMessage &= vbCrLf &
                        status(0).errorFile(i) & vbCrLf &
                            "Error: " & status(0).errorMessage(i)
                End If
            Next
            iSwApp.SendMsgToUser("Status Check failed with the following " & sCatMessage)
            If status(0).upToDate9 Is Nothing Then Exit Sub
        End If

        If status(0).upToDate9 Is Nothing Then iswApp.SendMsgToUser("Error: No Files found")

        sCatMessage = ""
        ' File was found on vault!
        For i = 0 To status.Length - 1
            If status(i).upToDate9 Is Nothing Then Continue For
            If status(i).upToDate9 = "*" Then
                'Vault has a newer version
                sCatMessage &= vbCrLf & status(i).filename

            ElseIf status(i).lock6 = "O" Then
                ' File locked for editing by another user!
                ' We'll just call the lock TortoiseProc process and let it+user deal with it
                'iSwApp.SendMsgToUser("File is locked by another user. Ask them to check it back in. " &
                '     "If you steal their lock (Not recommended) then " &
                '     vbCrLf & "1. Make sure you discuss it with them so that work isn't lost!" &
                '     vbCrLf & "2. In SolidWorks you'll have to do file > Get Write Access after you have the lock.")
            Else
                sDocPathsToCheckout(i) = status(i).filename
            End If
        Next

        If sCatMessage <> "" Then
            iswApp.SendMsgToUser("Local copy is out of date. Update from Vault and try again." & vbCrLf & sCatMessage)
        End If

        bSuccess = runTortoiseProcexeWithMonitor("/command:lock /path:" & formatFilePathArrForTortoiseProc(sDocPathsToCheckout) & " /closeonend:3")

        setReadWriteFromLockStatus(modDocArr)

    End Sub
    Public Sub myGetLatestAllRepo()
        myGetLatest(1)
    End Sub
    Public Sub myGetLatestOpenOnly()
        myGetLatest(0)
    End Sub
    Sub myGetLatest(ByVal updateClosedDocs As Integer)
        'Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String
        Dim OpenDocModels() As ModelDoc2
        Dim modDocTemp As ModelDoc2

        Dim i As Integer
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim m As Integer = 0
        'Dim index As Integer
        'Dim sFilePathCat As String

        OpenDocModels = getAllOpenDocs(bMustBeVisible:=False)

        Dim sFileListToUpdate(OpenDocModels.Length - 1) As String
        Dim docListToReload(OpenDocModels.Length - 1) As ModelDoc2

        Dim sOpenDocPath() As String = getFilePathsFromModDocArr(OpenDocModels)

        'Run SVN to see status of all
        Dim mySVNStatus As SVNStatus() = getFileSVNStatus(sOpenDocPath, bCheckServer:=True)

        'TODO There's no error checking from the getSVNfile status! remember to use not mySVNStatus.sErrorMessage is nothing

        Dim modDocArr(mySVNStatus.Length - 1) As ModelDoc2
        For i = 0 To mySVNStatus.Length - 1

            If mySVNStatus(i).upToDate9 = "*" Then
                'If file is out of date

                modDocTemp = iswApp.GetOpenDocumentByName(mySVNStatus(i).filename)
                'index = Array.IndexOf(sOpenDocPath, mySVNStatus(i)) ' Searches for an SVNfilepath in the array of open file paths
                'If index >= 0 Then

                If Not modDocTemp Is Nothing Then
                    'If File Is open Then
                    modDocArr(m) = modDocTemp
                    m = m + 1
                    If modDocTemp.IsOpenedReadOnly() Then
                        ' ForceReleaseLocks is releasing SolidWorks's system lock on the file
                        ' Which prevents other programs (like SVN) from overwriting the file
                        ' This allows the file to be overwritten by the New version
                        If modDocTemp.GetType <> 3 Then
                            'The method doesn't work for Drawings
                            modDocTemp.ForceReleaseLocks()
                            docListToReload(k) = modDocArr(i)
                            k = k + 1
                        End If
                    Else
                        ' The user has an obsolete write copy of a file. They'll have to
                        ' Decide if they want to destroy their changes or the ones on the vault.
                        ' Do nothing. Let Tortoise.exe notify user of their issue.
                    End If
                    sFileListToUpdate(j) = mySVNStatus(i).filename
                    j = j + 1
                Else
                    If updateClosedDocs = 1 Then
                        ' file out of date, not open
                        ' if bUpdateClosed then
                        ' Add to file list 
                        sFileListToUpdate(j) = mySVNStatus(i).filename
                        j = j + 1
                    End If
                End If
            End If
        Next

        If j = 0 Then
            iswApp.SendMsgToUser("All Files Checked Are Up to Date!")
            Exit Sub
        End If

        runTortoiseProcexeWithMonitor("/command:update /path:" & formatFilePathArrForTortoiseProc(sFileListToUpdate))

        For i = 0 To k - 1
            If docListToReload(i) Is Nothing Then Exit For
            'Reattaches the file system to SolidWorks. Whatever that means :p
            'This only works for parts/Assemblies!
            'docModTemp.ForceReleaseLocks()
            docListToReload(i).ReloadOrReplace(ReadOnly:=True, ReplaceFileName:=False, DiscardChanges:=True)
        Next

        setReadWriteFromLockStatus(modDocArr)
    End Sub
    Sub NoCallbackSub()

    End Sub
    Sub FlyoutCommandItem1()
        iswApp.SendMsgToUser("Flyout command 1")
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
        Dim getFilePathsArr(modDocArr.Length - 1) As String
        For i = 0 To modDocArr.Length - 1
            getFilePathsArr(i) = modDocArr(i).GetPathName()
        Next
        Return (getFilePathsArr)
    End Function
    Function getAllOpenDocs(ByRef bMustBeVisible As Boolean) As ModelDoc2()
        'bMustBeVisible ignores invisible files such as parts within an assembly

        Dim modDoc As ModelDoc2 = iswApp.GetFirstDocument
        Dim iNumDocsOpen As Integer = iswApp.GetDocumentCount()
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
        ReDim Preserve modDocOutput(i - 1)
        Return modDocOutput
    End Function
    Function formatFilePathArrForTortoiseProc(ByRef sFilePathArr() As String) As String
        Dim sFilePathCat As String = """" & sFilePathArr(0)
        For i = 0 To sFilePathArr.Length - 1
            If sFilePathArr(i) Is Nothing Then Continue For
            sFilePathCat &= "*" & sFilePathArr(i)
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
        tortStartInfo.WorkingDirectory = sRepoLocalPath
        oTortProcess.StartInfo = tortStartInfo
        oTortProcess.Start()

        'Monitor the process. Kill it if it stops responding
        Dim nResponding As Integer = 0
        Do While (Not oTortProcess.HasExited)
            nResponding += oTortProcess.Responding
            If nResponding > 5000 Then 'Sort of milliseconds because of the sleep command. But not exactly.
                oTortProcess.Kill()
                iswApp.SendMsgToUser("SVNTortoise Window connecting to vault terminated")
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
            iswApp.SendMsgToUser("Error could not save the following file. For Error Codes, google swFileSaveError_e " &
                     "and for Warnings google swFileSaveWarning_e " & vbCrLf &
                     sErrorFiles)
            Exit Sub
        End If
    End Sub
    Sub setReadWriteFromLockStatus(ByRef modDoc() As ModelDoc2)
        Dim status() As SVNStatus
        Dim sDocPaths(modDoc.Length - 1) As String
        Dim sStolenLockPaths As String = ""
        Dim i As Integer
        Dim j As Integer = 0
        Dim sCatErrorMessage As String = ""

        ' Now the file couldve been locked or not by the user in the dialog box
        ' So we'll query the vault and see what happened
        For i = 0 To sDocPaths.Length - 1
            If modDoc(i) Is Nothing Then Continue For
            sDocPaths(j) = modDoc(i).GetPathName
            j += 1
        Next

        status = getFileSVNStatus(sDocPaths, False)
        ' TODO Also fix getFileSVNStatus error checking? commonize? Bring into the function? 
        If Not status(0).errorMessage(0) Is Nothing Then
            For i = 0 To status(0).errorMessage.Length - 1
                sCatErrorMessage &= vbCrLf & status(0).errorFile(i) &
                    vbCrLf & status(0).errorMessage(i)
            Next
            iSwApp.SendMsgToUser("Status Check Issues: " &
                        vbCrLf & sCatErrorMessage)
        End If

        For i = 0 To sDocPaths.Length - 1
            If status(i).lock6 = "K" Then
                ' The user got a lock! Let's change to write access
                modDoc(i).SetReadOnlyState(False)
                'save3AndShowErrorMessages(modDoc)
            ElseIf (status(i).lock6 = "S") Or (status(i).lock6 = "B") Then
                'Lock stolen / broken
                sStolenLockPaths = sStolenLockPaths & vbCrLf & status(i).filename
                modDoc(i).SetReadOnlyState(True)
            ElseIf status(i).lock6 = " " Then
                'User didn't get a lock.
                modDoc(i).SetReadOnlyState(True)
            End If
        Next

        If sStolenLockPaths <> "" Then
            iswApp.SendMsgToUser("Another user has stolen/broken your lock(s) for the following part(s). " &
                    "They are being switched to read-only. You'll have to Lock/Checkout them out again." &
                    "Any changes since last checked-in to the server may be lost." &
                    sStolenLockPaths)
        End If
    End Sub
    Function getFileSVNStatus(ByVal sFilePath() As String, ByVal bCheckServer As Boolean) As SVNStatus()
        Dim output(sFilePath.Length - 1) As SVNStatus 'starts with one element then gets redim'd later
        Dim oSVNProcess As New Process()
        Dim SVNstartInfo As New ProcessStartInfo
        Dim sOutputLines() As String
        Dim sOutputErrorLines() As String
        'Dim sLine2 As String
        Dim SVNOutput As String
        Dim SVNErrorOutput As String
        Dim bSuccess As Boolean = False
        Dim sFilePathCat As String = ""
        Dim iLineStep As Integer = 1
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim bExpectStatusAgainstRevision As Boolean = False

        'TODO: Error check. One file from list is not in server folder.

        For i = 0 To sFilePath.Length - 1
            If sFilePath(i) Is Nothing Then Exit For
            sFilePathCat = sFilePathCat & """" & sFilePath(i) & """ "
        Next
        SVNstartInfo.Arguments = "status" & If(bCheckServer, " -u", "") & " --verbose " & sFilePathCat

        SVNstartInfo.FileName = sSVNPath
        SVNstartInfo.UseShellExecute = False
        SVNstartInfo.RedirectStandardOutput = True
        SVNstartInfo.RedirectStandardError = True
        SVNstartInfo.CreateNoWindow = True
        oSVNProcess.StartInfo = SVNstartInfo
        oSVNProcess.Start()
        oSVNProcess.WaitForExit()

        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardOutput
            SVNOutput = oStreamReader.ReadToEnd()
        End Using
        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardError
            SVNErrorOutput = oStreamReader.ReadToEnd()
        End Using
        sOutputLines = SVNOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        sOutputErrorLines = SVNErrorOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        k = sOutputErrorLines.Length - 1

        'Error Checking
        If (sOutputErrorLines Is Nothing) Or (sOutputLines Is Nothing) Then
            output(0).errorMessage(i) = "Error: SVN status output standard error is nothing. Must of not connected/read to SVN process"
            Return output
        End If

        For i = 0 To sOutputErrorLines.Length - 1
            'All error messages are an array stored in the first element of the output structure
            output(0).errorMessage(i) = sOutputErrorLines(i)
        Next

        If (bCheckServer) Then
            If sOutputLines(0).Substring(0, 23) = "Status against revision" Then
                output(0).errorMessage(0) = "Status Returned from SVN Server with No Items" 'If you change the string, change it other places in the code too!
                Return output

            ElseIf (sOutputLines.Length = 1) Then
                'If we are checking the server, we should expect a line 2. If its not there then theres an error.
                output(0).errorMessage(0) = "Error: Incomplete SVN Status. Could not Read Line 2. Line 1:" & sOutputLines(0)
                Return output
                'ElseIf (sOutputLines(0).Contains(sFilePath)) And (sOutputLines(1).Substring(0, 23) = "Status against revision") Then
            Else
                'bSuccess = True
                '               iLineStep = 2 ' skips every second line
            End If
        Else 'ElseIf (sOutputLines(0).Contains(sFilePath)) Then
            'bSuccess = True
            'iLineStep = 1
        End If
        'If bSuccess Then
        ' Success
        'ReDim Preserve output((sOutputLines.Length - 1) / (iLineStep + 1)) 'server tags an extra line on. True = -1, so this stops before the extra line

        For i = 0 To sOutputLines.Length - 1 'Step iLineStep
            If sOutputLines(i).Substring(0, 23) = "Status against revision" Then
                If bExpectStatusAgainstRevision Then
                    bExpectStatusAgainstRevision = False
                    Continue For
                Else
                    output(0).errorMessage(k) = "Status Returned from SVN Server with No Items"
                    output(0).errorFile(k) = sOutputLines(i).Substring(41, sOutputLines(i).Length - 41)
                    bExpectStatusAgainstRevision = False
                    Continue For
                End If
            End If

            output(j).filename = sOutputLines(i).Substring(41, sOutputLines(i).Length - 41)
            output(j).addDelChg1 = sOutputLines(i).Substring(0, 1)
            output(j).modifications2 = sOutputLines(i).Substring(1, 1)
            output(j).workingDirLock3 = sOutputLines(i).Substring(2, 1)
            output(j).addWithHist4 = sOutputLines(i).Substring(3, 1)
            output(j).switchWParent5 = sOutputLines(i).Substring(4, 1)
            output(j).lock6 = sOutputLines(i).Substring(5, 1)
            output(j).tree7 = sOutputLines(i).Substring(6, 1)
            'col 8 is blank
            If bCheckServer Then
                output(j).upToDate9 = sOutputLines(i).Substring(8, 1)
            Else
                output(j).upToDate9 = "NoUpdate"
            End If
            j = j + 1
            If bCheckServer Then bExpectStatusAgainstRevision = True
        Next i

        'Else
        '   output(0).errorMessage = "Error. First read line from SVN Status: " & sOutputLines(0)
        'End If
        Return output
    End Function
    Function getComponentsOfAssembly(ByRef modDoc As ModelDoc2) As ModelDoc2()

        If modDoc.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            iswApp.SendMsgToUser("Error: Model is not an assembly.")
            'Throw New System.Exception("modDoc is not an Assembly")
        End If

        Dim mdComponentList As New List(Of ModelDoc2)()
        Dim i As Integer = 0
        Dim swFeat As IFeature
        Dim entity As Entity
        Dim component As IComponent2

        swFeat = modDoc.FirstFeature

        While Not swFeat Is Nothing
            entity = swFeat
            If entity.GetType = swSelectType_e.swSelCOMPONENTS Then
                component = swFeat.GetSpecificFeature2
                mdComponentList.Add(component.GetModelDoc2)
            End If
            swFeat = modDoc.GetNextFeature
        End While

        Dim mdComponentArr() As ModelDoc2 = mdComponentList.ToArray
        Return mdComponentArr
    End Function
    Structure SVNStatus
        ' Each 1-9 is a one character string
        ' See http://svnbook.red-bean.com/en/1.8/svn.ref.svn.c.status.html
        Public filename As String
        Public addDelChg1 As String
        Public modifications2 As String
        Public workingDirLock3 As String
        Public addWithHist4 As String
        Public switchWParent5 As String
        Public lock6 As String
        Public tree7 As String
        'col 8 is blank
        Public upToDate9 As String
        Public errorMessage() As String
        Public errorFile() As String
    End Structure

End Class
