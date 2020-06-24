
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing

<ProgId("SVN_AddIn")>
Public Class UserControl1

    Dim WithEvents iSwApp As SldWorks
    'Dim userAddin As SwAddin = New SwAddin() 'couldn't get access to swapp in here!
    Public Const sTortPath As String = "C:\Users\benne\Documents\SVN\TortoiseProc.exe"
    Public Const sRepoLocalPath As String = "C:\Users\benne\Documents\SVN\cad1"
    Public Const sSVNPath As String = "C:\Program Files\TortoiseSVN\bin\svn.exe"

    Public statusAllOpenModels As SVNStatus
    Public allOpenDocs As ModelDoc2()
    Public allTreeViews As TreeView()

    Friend Sub getSwApp(ByRef swAppin As SldWorks)
        'Allows for swApp to be passed into this class.
        iSwApp = swAppin
    End Sub
    Private Sub butCheckinWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckinWithDependents.Click
        myCheckinWithDependents()
        updateStatusStrip()
    End Sub

    Private Sub butCheckinAll_Click(sender As Object, e As EventArgs) Handles butCheckinAll.Click
        myCheckinAll()
        updateStatusStrip()
    End Sub
    Private Sub butUnlockWithDependents_Click(sender As Object, e As EventArgs) Handles butUnlockWithDependents.Click
        myUnlockWithDependents()
        updateStatusStrip()
    End Sub
    Private Sub butUnlockAll_Click(sender As Object, e As EventArgs) Handles butUnlockAll.Click
        myUnlockAll()
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
        myGetLatestOrRevert(getAllOpenDocs(bMustBeVisible:=False))
        'myGetLatestOpenOnly()
        updateStatusStrip()
    End Sub

    Private Sub butGetLatestAllRepo_Click(sender As Object, e As EventArgs) Handles butGetLatestAllRepo.Click
        myGetLatestOrRevert()
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
    Public Sub updateStatusOfAllModelsVariable()
        statusAllOpenModels = getFileSVNStatus(bCheckServer:=True, getAllOpenDocs(bMustBeVisible:=False))
    End Sub



    Public Sub updateStatusStrip()
        Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc}
        If modDoc Is Nothing Then Exit Sub
        'Doesn't check the server because thats faster... But then it wont know if someone else has is checked out
        Dim status As SVNStatus = getFileSVNStatus(bCheckServer:=False, modDoc)
        Dim myCol As myColours = New myColours()
        myCol.initialize()
        If status.fp(0).lock6 = "K" Then
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
    '============= Start sub definitions

    Public Sub myUnlockActive()
        Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub
        unlockDocs(moddoc)
    End Sub
    Public Sub myUnlockWithDependents()
        'TODO Needs work. It currently tries to unlock all dependents whether they are locked or not
        ' creating a small error
        Dim modDoc() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        If modDoc(0).GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            unlockDocs(modDoc)
        Else
            unlockDocs(getComponentsOfAssemblyOptionalUpdateTree(modDoc(0)))
        End If
    End Sub
    Sub myUnlockAll()
        Dim bSuccess As Boolean = runTortoiseProcexeWithMonitor("/command:unlock /path:" & sRepoLocalPath & " /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("Releasing Locks Failed.")
        bSuccess = runTortoiseProcexeWithMonitor("/command:revert /path:" & sRepoLocalPath & " /closeonend:3")
        If Not bSuccess Then iSwApp.SendMsgToUser("Reverting to Vault copies failed.")
    End Sub
    Sub unlockDocs(ByRef modDocArr() As ModelDoc2)

        Dim Status As SVNStatus = getFileSVNStatus(bCheckServer:=True, modDocArr)

        Dim activeDoc As ModelDoc2 = iSwApp.ActiveDoc
        If activeDoc Is Nothing Then Exit Sub

        Dim bSuccess As Boolean = runTortoiseProcexeWithMonitor("/command:unlock /path:" &
                                         formatFilePathArrForTortoiseProc(
                                            getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")

        If Not bSuccess Then iSwApp.SendMsgToUserv("Releasing Locks Failed.")

        Status = getFileSVNStatus(bCheckServer:=False, modDocArr)
        Status.setReadWriteFromLockStatus()
        Status.releaseFileSystemAccessToReadOnlyModels()
        getComponentsOfAssemblyOptionalUpdateTree(activeDoc, Status)

        bSuccess = runTortoiseProcexeWithMonitor("/command:revert /path:" &
                                         formatFilePathArrForTortoiseProc(
                                            getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")

        If Not bSuccess Then iSwApp.SendMsgToUser("Reverting to Vault copies failed.")

    End Sub
    Public Function createBoolArray(ByRef iUbound As Integer, ByRef value As Boolean) As Boolean()
        Dim i As Integer
        Dim output(iUbound) As Boolean
        For i = 0 To iUbound
            output(i) = value
        Next
        Return output
    End Function
    Public Sub myCheckinWithDependents()
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
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

        Dim mySVNStatus As SVNStatus = getFileSVNStatus(bCheckServer:=True, modDocArr) 'Error when checking in Assembly. 
        mySVNStatus.setReadWriteFromLockStatus()
        getComponentsOfAssemblyOptionalUpdateTree(activeDoc, mySVNStatus)
    End Sub
    Public Sub myCheckinAll()
        Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String

        'Dim i As Integer
        'Dim index As Integer

        iSwApp.RunCommand(19, vbEmpty) 'Save All

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:""" & sRepoLocalPath & """ /closeonend:3")
        If Not bSuccess Then
            Exit Sub
        End If

        'Switch over files to read-only
        'OpenDocPathList = CType(getAllOpenDocs(True, True), String())
        Dim OpenDocModels() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=True)

        'Dim sOpenDocPath() As String = getFilePathsFromModDoiSwApp.SendMsgToUser("Active Document not found") cArr(OpenDocModels)

        Dim mySVNStatus As SVNStatus = getFileSVNStatus(bCheckServer:=True, OpenDocModels)
        mySVNStatus.setReadWriteFromLockStatus()
        getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc, mySVNStatus)

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
        If Not bSuccess Then iswApp.SendMsgToUser("Status Check Failed.")
    End Sub
    Sub myCleanupAndRelease()
        Dim bSuccess As Boolean

        Dim mySVNStatus As SVNStatus = getFileSVNStatus(bCheckServer:=True)
        mySVNStatus.setReadWriteFromLockStatus()
        getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc, mySVNStatus)
        'setReadWriteFromLockStatus(getAllOpenDocs(bMustBeVisible:=False))

        bSuccess = runTortoiseProcexeWithMonitor("/command:cleanup /cleanup /path:" & sRepoLocalPath)
        If Not bSuccess Then iswApp.SendMsgToUser("Cleanup Failed. This is often because the SVN server is attempting " &
                       "to open a file that SolidWorks is currently accessing. This occurs even when the file is read only. " &
                       "Try closing all open files and trying again. Or close SolidWorks and use ToroiseSVN to clean up. ")
    End Sub
    Sub checkoutDocs(ByRef modDocArr() As ModelDoc2)
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Active Document not found") : Exit Sub

        'Dim modDocArr() As ModelDoc2 = {modDoc}
        'Dim sActiveDocPath() As String = getFilePathsFromModDocArr(modDocArr)
        Dim sDocPathsToCheckout(modDocArr.Length - 1) As String
        Dim status As SVNStatus
        Dim bSuccess As Boolean = False
        Dim i As Integer
        Dim sCatMessage As String = ""
        Dim sCatMessageLocked As String = ""

        'Using objReader As System.IO.TextReader = System.IO.File.OpenText("C:\Users\benne\AppData\Local\Temp\tmp9BF6.tmp") 'System.IO.StreamReader(sTempFileName)
        '    sLine1 = objReader.ReadLine()
        '    sLine2 = objReader.ReadLine()
        'End Using
        ''objReader.Close()
        ''My.Computer.FileSystem.DeleteFile(sTempFileName)
        'Debug.Print(sTempFileName)
        status = getFileSVNStatus(bCheckServer:=True, modDocArr)



        'If status.fp(0).upToDate9 Is Nothing Then iSwApp.SendMsgToUser("Error: No Files found")

        'sCatMessage = ""
        '' File was found on vault!
        'For i = 0 To UBound(status.fp)
        '    If status.fp(i).upToDate9 Is Nothing Then Continue For
        '    If status.fp(i).upToDate9 = "*" Then
        '        'Vault has a newer version
        '        sCatMessage &= vbCrLf & status.fp(i).filename

        '    ElseIf status.fp(i).lock6 = "O" Then
        '        ' File locked for editing by another user!
        '        ' We'll just call the lock TortoiseProc process and let it+user deal with it
        '        'iSwApp.SendMsgToUser("File is locked by another user. Ask them to check it back in. " &
        '        '     "If you steal their lock (Not recommended) then " &
        '        '     vbCrLf & "1. Make sure you discuss it with them so that work isn't lost!" &
        '        '     vbCrLf & "2. In SolidWorks you'll have to do file > Get Write Access after you have the lock.")
        '        sCatMessageLocked &= vbCrLf & status.fp(i).filename
        '        sDocPathsToCheckout(i) = status.fp(i).filename
        '    Else
        '        sDocPathsToCheckout(i) = status.fp(i).filename
        '    End If
        'Next
        ''If sCatMessage <> "" And sCatMessageLocked <> "" Then
        ''    iSwApp.SendMsgToUser("The following files are out of date. Get Latest and try again. " & vbCrLf &
        ''        sCatMessage & vbCrLf &
        ''        "The following files are locked "
        ''End If

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

        status = getFileSVNStatus(bCheckServer:=False, modDocArr)
        status.setReadWriteFromLockStatus()
        getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc, status)
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


    'Public Sub myGetLatestAllRepo()
    '    myGetLatest(1)
    'End Sub
    'Public Sub myGetLatestOpenOnly()
    '    myGetLatest(0)
    'End Sub
    Sub myGetLatestOrRevert(Optional ByRef openDocModels As ModelDoc2() = Nothing,
                        Optional ByRef myGetType As getLatestType = getLatestType.update)
        'Dim modDocTemp As ModelDoc2
        Dim i As Integer
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim m As Integer = 0
        Dim n As Integer = 0

        'DEFAULTS
        'If OpenDocModels Is Nothing Then OpenDocModels = getAllOpenDocs(bMustBeVisible:=False)

        'Dim sFileListToUpdate(UBound(OpenDocModels)) As String
        'If Not openDocModels Is Nothing Then
        '    Dim sOpenDocPath() As String = getFilePathsFromModDocArr(openDocModels)
        'End If
        Dim mySVNStatus As SVNStatus = getFileSVNStatus(bCheckServer:=True, openDocModels)

        Dim sFileList(UBound(mySVNStatus.fp)) As String

        'Dim modDocArr(UBound(mySVNStatus)) As ModelDoc2
        'Dim mdFilesToReconnectWith(UBound(mySVNStatus.fp)) As ModelDoc2

        For i = 0 To UBound(mySVNStatus.fp)
            'modDocTemp = iSwApp.GetOpenDocumentByName(mySVNStatus.fp(i).filename)
            If mySVNStatus.fp(i).modDoc Is Nothing Then Continue For 'modDocTemp
            'modDocArr(m) = modDocTemp : m += 1
            If (mySVNStatus.fp(i).upToDate9 = "*") And ((myGetType = getLatestType.update) Or (myGetType = getLatestType.both)) Then
                ' File is out of date
                'sFileListToUpdate(j) = mySVNStatus.fp(i).filename : 
                mySVNStatus.fp(i).revertUpdate = getLatestType.update
                j += 1
            ElseIf (mySVNStatus.fp(i).addDelChg1 = "M") And (myGetType = getLatestType.revert) And (mySVNStatus.fp(i).lock6 <> "K") Then
                ' Local copy has been modified
                ' Note out of date files will go into FileListToUpdate and will be skipped over by revert.
                'sFileListToRevert(n) = mySVNStatus.fp(i).filename : n += 1
                mySVNStatus.fp(i).revertUpdate = getLatestType.revert
                j += 1
            End If
        Next

        mySVNStatus.setReadWriteFromLockStatus()
        getComponentsOfAssemblyOptionalUpdateTree(iSwApp.ActiveDoc, mySVNStatus)

        If j = 0 Then iSwApp.SendMsgToUser("All Files Checked Are Up to Date!") : Exit Sub

        mySVNStatus.releaseFileSystemAccessToReadOnlyModels()

        sFileList = mySVNStatus.sFilterGetLatestType(getLatestType.revert, bIgnoreUpdate:=True)
        If (Not sFileList Is Nothing) And ((myGetType = getLatestType.revert) Or (myGetType = getLatestType.both)) Then
            runTortoiseProcexeWithMonitor("/command:revert /path:" &
                                          formatFilePathArrForTortoiseProc(sFileList) & " /closeonend:3")
        End If
        sFileList = mySVNStatus.sFilterGetLatestType(getLatestType.update, bIgnoreUpdate:=False)
        If (Not sFileList Is Nothing) And ((myGetType = getLatestType.update) Or (myGetType = getLatestType.both)) Then
            runTortoiseProcexeWithMonitor("/command:update /path:" & formatFilePathArrForTortoiseProc(sFileList) & " /closeonend:3")
        End If

        mySVNStatus.reattachDocsToFileSystem()
    End Sub
    Enum getLatestType
        revert
        update
        both
    End Enum
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
            If modDocArr(i) Is Nothing Then Continue For
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

    Public Function getFileSVNStatus(ByVal bCheckServer As Boolean,
                              Optional ByRef modDocArr() As ModelDoc2 = Nothing,
                              Optional ByVal sFilePath() As String = Nothing,
                              Optional ByVal iRecursiveLevel As Integer = 0) As SVNStatus
        'Pass sFilePath = Create from the file path
        'Pass modDocArr = create from the modDocArr
        'Pass Neither = create for entire repo

        Dim oSVNProcess As New Process()
        Dim SVNstartInfo As New ProcessStartInfo
        Dim sOutputLines() As String
        Dim sOutputErrorLines() As String
        'Dim sLine2 As String
        Dim SVNOutput As String
        Dim SVNErrorOutput As String
        Dim bSuccess As Boolean = False
        Dim sFilePathCat As String = ""
        Dim sFilePathTemp As String
        Dim iLineStep As Integer = 1
        'Dim sModDocPathArr(0) As String
        'Dim iOutputUbound As Integer
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim n As Integer = 0
        Dim bExpectStatusAgainstRevision As Boolean = False
        Dim Index As Integer
        'If sFilePath.Length = 0 Then sFilePath = Nothing
        'If modDocArr.Length = 0 Then modDocArr = Nothing

        'If Not modDocArr Is Nothing Then
        '    ReDim sModDocPathArr(UBound(modDocArr))
        '    For i = 0 To UBound(modDocArr)
        '        If modDocArr(i) Is Nothing Then Continue For
        '        sModDocPathArr(i) = modDocArr(i).GetPathName
        '    Next
        'End If

        'TODO: Error check. One file from list is not in server folder.

        'For i = 0 To sFilePath.Length - 1
        '    If sFilePath(i) Is Nothing Then Exit For
        '    sFilePathCat = sFilePathCat & """" & sFilePath(i) & """ "
        'Next
        SVNstartInfo.Arguments = "status " & If(bCheckServer, "-u ", "") & "-v --non-interactive " & sRepoLocalPath 'sFilePathCat
        SVNstartInfo.FileName = sSVNPath
        SVNstartInfo.UseShellExecute = False
        SVNstartInfo.RedirectStandardOutput = True
        SVNstartInfo.RedirectStandardError = True
        SVNstartInfo.CreateNoWindow = True
        oSVNProcess.StartInfo = SVNstartInfo
        oSVNProcess.Start()

        'Monitor the process. Kill it if it stops responding
        Dim nResponding As Integer = 0
        Do While (Not oSVNProcess.HasExited)
            nResponding += 1
            If nResponding > 10000 Then 'Sort of milliseconds because of the sleep command. But not exactly.
                oSVNProcess.Kill()
                iSwApp.SendMsgToUser("SVNTortoise Window connecting to vault terminated")
                Dim badOutput As SVNStatus = New SVNStatus()
                'ReDim badOutput.statError(0) : ReDim badOutput.fp(0)
                badOutput.statError(0).sMessage = "Error: SVN Process timed out"
                Return badOutput
            End If
            System.Threading.Thread.Sleep(1)
        Loop

        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardOutput
            SVNOutput = oStreamReader.ReadToEnd()
        End Using
        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardError
            SVNErrorOutput = oStreamReader.ReadToEnd()
        End Using
        sOutputLines = SVNOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        sOutputErrorLines = SVNErrorOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        k = sOutputErrorLines.Length - 1

        Dim output As SVNStatus = New SVNStatus()
        ReDim output.fp(If(sFilePath Is Nothing, UBound(sOutputLines), UBound(sFilePath)))

        'Error Checking
        If (sOutputErrorLines Is Nothing) Or (sOutputLines Is Nothing) Then
            ReDim output.statError(0)
            output.statError(0).sMessage = "Error: SVN status output standard error is nothing. Must of not connected/read to SVN process"
            Return output
        End If

        If sOutputErrorLines.Length <> 0 Then
            'We got some errors if length > 0
            ReDim output.statError(UBound(sOutputErrorLines))
            For i = 0 To UBound(sOutputErrorLines)
                'All error messages are an array stored in the first element of the output structure
                output.statError(i).sMessage = sOutputErrorLines(i)
            Next

            If sOutputErrorLines.Length >= 2 Then
                If sOutputErrorLines(1).Contains("E215004") Then
                    'Log in Failed!
                    If iRecursiveLevel <> 0 Then
                        Return output
                    End If
                    'Open a log in, and then try again. 
                    runTortoiseProcexeWithMonitor("/command:repostatus /remote /path:" & sRepoLocalPath) 'log in
                    Return getFileSVNStatus(bCheckServer, modDocArr, sFilePath, iRecursiveLevel:=1)
                End If
            End If
        End If

        If sOutputLines.Length = 0 Then
            'If output(0).errorMessage Is Nothing Then ReDim Preserve output(0).errorMessage(0)
            ReDim output.statError(0)
            output.statError(0).sMessage = "Error: No Usable output lines returned from SVN process. "
            Return output
        End If

        If (bCheckServer) Then
            If sOutputLines(0).Substring(0, 23) = "Status against revision" Then
                ReDim output.statError(0)
                output.statError(0).sMessage = "Status Returned from SVN Server with No Items" 'If you change the string, change it other places in the code too!
                Return output

            ElseIf (sOutputLines.Length = 1) Then
                'If we are checking the server, we should expect a line 2. If its not there then theres an error.
                ReDim output.statError(0)
                output.statError(0).sMessage = "Error: Incomplete SVN Status. Could not Read Line 2. Line 1:" & sOutputLines(0)
                Return output
            End If
        End If

        If (sFilePath Is Nothing) And (modDocArr Is Nothing) Then
            'Looping through entire repo
            ReDim output.fp(UBound(sOutputLines))
            For i = 0 To UBound(sOutputLines) 'Step iLineStep
                If sOutputLines(i).Substring(0, 23) = "Status against revision" Then Continue For
                If sOutputLines(i).Contains("~$") Then Continue For 'Temporary file!
                sFilePathTemp = sOutputLines(i).Substring(21, sOutputLines(i).Length - 21)

                output.addOutputLineToSVNStatus(sOutputLines(i), j, sFilePathTemp, iSwApp.GetOpenDocumentByName(sFilePathTemp), bCheckServer)
                j = j + 1
            Next i
        ElseIf Not sFilePath Is Nothing Then
            'Function was given a list of file paths. Loop through these.
            ReDim output.fp(UBound(sFilePath))
            ReDim Preserve output.statError(UBound(sFilePath))
            For i = 0 To UBound(sFilePath) 'Step iLineStep
                'Index = Array.IndexOf(sOutputLines, sFilePath(i))
                If sFilePath(i) Is Nothing Then Continue For
                Index = findIndexContains(sOutputLines, sFilePath(i))
                If Index = -1 Then
                    output.statError(n).sFile = sFilePath(i)
                    output.statError(n).sMessage = "File Not Found"
                    n += 1
                End If
                output.addOutputLineToSVNStatus(sOutputLines(Index), j, sFilePath(i), iSwApp.GetOpenDocumentByName(sFilePath(i)), bCheckServer)
                j = j + 1
            Next i
        ElseIf Not modDocArr Is Nothing Then
            'Function was given a list of modDocs. Loop through these.
            ReDim output.fp(UBound(modDocArr))
            ReDim Preserve output.statError(UBound(modDocArr))
            For i = 0 To UBound(modDocArr) 'Step iLineStep
                If modDocArr(i) Is Nothing Then Continue For
                Index = findIndexContains(sOutputLines, modDocArr(i).GetPathName)
                If Index = -1 Then
                    output.statError(n).sFile = sFilePath(i)
                    output.statError(n).sMessage = "File Not Found"
                    n += 1
                End If
                output.addOutputLineToSVNStatus(sOutputLines(Index), j, modDocArr(i).GetPathName, modDocArr(i), bCheckServer)
                j = j + 1
            Next i
        End If

        If j > 0 Then ReDim Preserve output.fp(j - 1)

        'Error Checking
        Dim sCatMessage As String = ""
        If Not output.statError(0).sMessage Is Nothing Then
            ' Some sort of error from the svn command
            For i = 0 To UBound(output.statError)
                If output.statError(i).sMessage.Contains("W155007:") Then
                    'Common error. File not saved into repository
                    sCatMessage &= vbCrLf &
                        output.statError(i).sMessage & vbCrLf &
                            "Error W155007 File is not saved inside repository. " &
                            "Save the file inside the repository and try again. "
                ElseIf output.statError(i).sMessage = "Status Returned from SVN Server with No Items" Then
                    sCatMessage &= vbCrLf &
                        output.statError(i).sFile & vbCrLf &
                    "Error: Current save file For model Not found. Save it inside the repository."
                Else
                    'Other Errors
                    sCatMessage &= vbCrLf &
                        output.statError(i).sFile & vbCrLf &
                            "Error: " & output.statError(i).sMessage
                End If
            Next
            iSwApp.SendMsgToUser("Status Check failed with the following " & sCatMessage)
        End If

        Return output
    End Function
    Function findIndexContains(ByRef sLookInArr() As String, ByRef find As String) As Integer
        Dim i As Integer
        'Dim output As Integer
        For i = 0 To UBound(sLookInArr)
            'If sLookInArr(i).Contains(find) Then Return i
            If (Strings.InStr(sLookInArr(i), find, CompareMethod.Text) <> 0) Then Return i
        Next
        Return -1
    End Function
    Function findStatusForFile(ByRef sFileName As String, ByRef status As SVNStatus) As SVNStatus
        Dim i As Integer
        Dim output As SVNStatus = New SVNStatus()

        ReDim output.fp(0)
        ReDim output.statError(UBound(status.statError))
        output.statError = status.statError
        For i = 0 To UBound(status.fp)
            If (Strings.InStr(status.fp(i).filename, sFileName, CompareMethod.Text) <> 0) Then Exit For
        Next
        If i = (UBound(status.fp) + 1) Then Return Nothing
        output.fp(0) = status.fp(i)
        Return output
    End Function

    Public Function getComponentsOfAssemblyOptionalUpdateTree(
                                    ByRef modDoc As ModelDoc2,
                                    Optional mySVNStatus As SVNStatus = Nothing) As ModelDoc2()
        Dim bUC As Boolean = If(mySVNStatus Is Nothing, False, True)
        Dim sFileNameTemp As String = System.IO.Path.GetFileName(modDoc.GetPathName)
        Dim parentNode As TreeNode = Nothing

        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Couldn't find model") : Return Nothing

        If bUC Then
            TreeView1.Nodes.Clear()
            parentNode = New TreeNode(sFileNameTemp)
        End If

        If modDoc.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            If bUC Then
                setNodeColorFromStatus(parentNode, mySVNStatus)
                TreeView1.Nodes.Add(parentNode)
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

        TraverseComponent(swRootComp, mdComponentList, 1, parentNode, mySVNStatus)

        Dim mdComponentArr() As ModelDoc2 = mdComponentList.ToArray
        If bUC Then TreeView1.Nodes.Add(parentNode) : TreeView1.ExpandAll()

        Return mdComponentArr
    End Function

    Sub TraverseComponent(
                         ByRef swComp As Component2,
                         ByRef mdComponentList As List(Of ModelDoc2),
                         ByVal nLevel As Long,
                         Optional ByRef rootNode As TreeNode = Nothing,
                         Optional mySVNStatus As SVNStatus = Nothing)

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
            setNodeColorFromStatus(parentNode, mySVNStatus)
            'End If
        End If

        vChildComp = swComp.GetChildren
        For i = 0 To UBound(vChildComp)
            swChildComp = vChildComp(i)
            modDocChild = swChildComp.GetModelDoc2
            If modDocChild.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
                'Is part file
                If mdComponentList.Contains(modDocChild) Then Continue For 'avoid duplicates
                If bUC Then
                    sChildFileName = System.IO.Path.GetFileName(modDocChild.GetPathName)
                    childNode = New TreeNode(sChildFileName)
                    setNodeColorFromStatus(childNode, mySVNStatus)
                    parentNode.Nodes.Add(childNode)
                End If

                mdComponentList.Add(modDocChild)
            Else
                'Is assembly
                TraverseComponent(swChildComp, mdComponentList, nLevel + 1, ParentNode)
            End If
        Next i

        If bUC Then
            If nLevel = 1 Then
                rootNode = ParentNode
            Else
                rootNode.Nodes.Add(ParentNode)
            End If
        End If

    End Sub
    Sub setNodeColorFromStatus(
        ByRef rootNode As TreeNode,
        ByRef mySVNStatus As SVNStatus)
        Dim myCol As myColours = New myColours()
        myCol.initialize()

        Dim status1 As SVNStatus = findStatusForFile(rootNode.Text, mySVNStatus)
        If status1 Is Nothing Then
            rootNode.BackColor = myCol.unknown
        ElseIf status1.fp(0).lock6 = "K" Then
            rootNode.BackColor = myCol.lockedByYou
        ElseIf status1.fp(0).upToDate9 = "*" Then
            rootNode.BackColor = myCol.outOfDate
        ElseIf status1.fp(0).lock6 = "O" Then
            rootNode.BackColor = myCol.lockedBySomeoneElse
        ElseIf status1.fp(0).lock6 = " " Then
            rootNode.BackColor = myCol.available
        Else
            rootNode.BackColor = myCol.unknown
        End If
    End Sub
    Class myColours
        Public lockedByYou As Drawing.Color
        Public lockedBySomeoneElse As Drawing.Color
        Public available As Drawing.Color
        Public unknown As Drawing.Color
        Public outOfDate As Drawing.Color
        Public Sub initialize()
            lockedByYou = Drawing.Color.Aquamarine
            lockedBySomeoneElse = Drawing.Color.Khaki
            available = Drawing.Color.Bisque
            unknown = Drawing.Color.LightGray
            outOfDate = Drawing.Color.FromArgb(255, 77, 77) 'light red
        End Sub
    End Class
    Class SVNStatus
        Public fp(0) As filePpty
        Public statError(0) As statusError
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
        Structure statusError
            Public sMessage As String
            Public sFile As String
            Public modDocArr As ModelDoc2
        End Structure
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
        Public Function sFilterUpToDate9(ByRef filter As String, Optional ByVal bFilterNot As Boolean = False) As String()
            'bIgnoreUpdate will ignore the filter out elements where an update is required.
            Dim sPath(UBound(fp)) As String
            Dim j As Integer = 0
            For i As Integer = 0 To UBound(fp)
                If ((fp(i).upToDate9 = filter) And (Not bFilterNot)) Or ((Not fp(i).upToDate9 = filter) And (bFilterNot)) Then
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
