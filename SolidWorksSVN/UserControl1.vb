
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.CodeDom.Compiler
'Imports System.Configuration

<ProgId("SVN_AddIn")>
Public Class UserControl1

    Public WithEvents iSwApp As SolidWorks.Interop.sldworks.SldWorks

    'Dim userAddin As SwAddin = New SwAddin() 'couldn't get access to swapp in here!

    'Public Const localRepoPath.text As String = "E:\SolidworksBackup\svn"
    'Public Const localRepoPath.text As String = "C:\Users\benne\Documents\SVN\cad1"

    Public statusOfAllOpenModels As SVNStatus = New SVNStatus
    Public allOpenDocs As ModelDoc2()

    'Dim modelDocList As New List(Of ModelDoc2)()
    Public allTreeViews As TreeView() = {New TreeView}
    'Public allTreeViews As New List(Of TreeView())

    Private Sub UserControl1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim docMenu As ContextMenuStrip
        Dim myrefreshItem, myCollapseItem As ToolStripMenuItem

        docMenu = New ContextMenuStrip()
        myrefreshItem = New ToolStripMenuItem("Refresh", My.Resources.VaultLogo128, AddressOf RefreshToolStripMenuItemEventHandler)
        myCollapseItem = New ToolStripMenuItem("Collapse", My.Resources.VaultLogo128, AddressOf collapseTreeViewHandler2)

        docMenu.Items.AddRange({myrefreshItem, myCollapseItem})

        Me.ContextMenuStrip = docMenu

    End Sub

    Friend Sub myInitialize(ByRef swAppin As SldWorks)
        'Allows for swApp to be passed into this class.
        iSwApp = swAppin

        initializeSwModelFunctions(iSwApp)
        svnModuleInitialize(iSwApp, Me, statusOfAllOpenModels)

        localRepoPath.Text = My.Settings.localRepoPath

        If iSwApp.GetDocumentCount = 0 Then

            If verifyLocalRepoPath(bInteractive:=True, bCheckLocalFolder:=True, bCheckServer:=False) Then
                If iSwApp.SendMsgToUser2("Would you like to get latest CAD files from the SVN Server? (SVN Update)", swMessageBoxIcon_e.swMbQuestion, swMessageBoxBtn_e.swMbYesNo) = swMessageBoxResult_e.swMbHitYes Then
                    runTortoiseProcexeWithMonitor("/command:update /path:" & My.Settings.localRepoPath & " /closeonend:3")
                End If
            End If
        Else
            refreshAddIn(bsaveLocalRepoPathSettings:=False)
        End If

    End Sub
    Friend Sub beforeClose()
        saveLocalRepoPathSettings()
    End Sub

    ' ### Get Locks
    Private Sub ToolStripDropDownGetLocks_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripDropDownButGetLocks.ButtonClick
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub

        getLocksOfDocs(GetSelectedModDocList(iSwApp))
        updateStatusStrip()
    End Sub
    Private Sub dropDownGetLocksWithDependents_Click(sender As Object, e As EventArgs) Handles dropDownGetLocksWithDependents.Click
        Dim modDocArr() As ModelDoc2

        modDocArr = getComponentsOfAssemblyOptionalUpdateTree(GetSelectedModDocList(iSwApp))

        If modDocArr Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub

        getLocksOfDocs(modDocArr)
        updateStatusStrip()
    End Sub

    ' ### Commit
    Private Sub ToolStripDropDownButCommit_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripDropDownButCommit.ButtonClick
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub
        tortCommitDocs(GetSelectedModDocList(iSwApp))
        updateStatusStrip()
    End Sub
    Private Sub dropDownCommitWithDependents_Click(sender As Object, e As EventArgs) Handles dropDownCommitWithDependents.Click
        tortCommitDocs(getComponentsOfAssemblyOptionalUpdateTree(GetSelectedModDocList(iSwApp)))
        updateStatusStrip()
    End Sub
    Private Sub dropDownCommitAll_Click(sender As Object, e As EventArgs) Handles dropDownCommitAll.Click
        myCommitAll()
        updateStatusStrip()
    End Sub

    ' ### Unlock
    Private Sub ToolStripDropDownButUnlock_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripDropDownButUnlock.ButtonClick
        unlockDocs(GetSelectedModDocList(iSwApp))
        updateStatusStrip()
    End Sub
    Private Sub dropDownUnlockWithDependents_Click(sender As Object, e As EventArgs) Handles dropDownUnlockWithDependents.Click
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub
        unlockDocs(getComponentsOfAssemblyOptionalUpdateTree(GetSelectedModDocList(iSwApp)))
        updateStatusStrip()
    End Sub
    Private Sub dropDownUnlockAll_Click(sender As Object, e As EventArgs) Handles dropDownUnlockAll.Click
        unlockDocs()
        updateStatusStrip()
    End Sub

    ' ### Get Latest
    Private Sub ToolStripDropDownButGetLatest_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripDropDownButGetLatest.ButtonClick
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub

        myGetLatestOrRevert(GetSelectedModDocList(iSwApp),, bVerbose:=True)
        'myGetLatestOpenOnly()
        updateStatusStrip()
    End Sub
    Private Sub dropDownGetLatestAllOpenFiles_Click(sender As Object, e As EventArgs) Handles dropDownGetLatestAllOpenFiles.Click
        Dim modDocArr() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=False)

        saveAllOpenFiles(bShowError:=True)

        myGetLatestOrRevert(modDocArr,, bVerbose:=True)
        'myGetLatestOpenOnly()
        updateStatusStrip()
    End Sub
    Private Sub dropDownGetLatestAll_Click(sender As Object, e As EventArgs) Handles dropDownGetLatestAll.Click
        saveAllOpenFiles(bShowError:=True)
        myGetLatestOrRevert(,, bVerbose:=True)
        updateStatusStrip()
        'myGetLatestAllRepo()
    End Sub
    Private Sub butFindComponent_Click(sender As Object, e As EventArgs) Handles butFindComponent.Click
        Dim modDocArr As ModelDoc() = GetSelectedModDocList(iSwApp)

    End Sub

    ' ### Refresh
    Private Sub RefreshToolStripMenuItemEventHandler(sender As Object, e As EventArgs)
        refreshAddIn()
    End Sub
    Private Sub collapseTreeViewHandler2(sender As Object, e As EventArgs)
        TreeView1.CollapseAll()
        TreeView1.Nodes(0).Expand()
    End Sub
    Private Sub butRefresh_Click(sender As Object, e As EventArgs) Handles butRefresh.Click

        'CLEANUP
        If iSwApp.GetDocumentCount() = 0 Then
            'No files are open
            If Me.onlineCheckBox.Checked Then
                If verifyLocalRepoPath(, bCheckLocalFolder:=True, bCheckServer:=True) Then
                    iSwApp.SendMsgToUser2("Couldn't find any open files to refresh the status for, but you are successfully communicating with SVN server. This button doesn't do anything if you don't have files open.",
                        swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
                Else
                    iSwApp.SendMsgToUser2("Unable to contact a server and verify that your local path is a synced SVN folder.",
                        swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
                End If
            Else
                verifyLocalRepoPath(, bCheckLocalFolder:=True, bCheckServer:=False)
                iSwApp.SendMsgToUser2("Couldn't find any open files to refresh the status for. Your 'online' checkbox is unchecked, so contact to the server was not attempted.",
                        swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
            End If
            Exit Sub
        End If

        statusOfAllOpenModels = New SVNStatus
        statusOfAllOpenModels.updateFromSvnServer(bRefreshAllTreeViews:=True)
        switchTreeViewToCurrentModel(bRetryWithRefresh:=False)

        statusOfAllOpenModels.setReadWriteFromLockStatus()

        refreshAddIn()
    End Sub
    ' ### Clean Up
    Private Sub butCleanup_Click(sender As Object, e As EventArgs) Handles butCleanup.Click
        iSwApp.SendMsgToUser("This unfortunately can't be run with SolidWorks Files open. Close all open files, then in Windows Explorer, right click > TortoiseSVN > Cleanup")
        'myCleanupAndRelease()
    End Sub

    ' ### Folder
    Private Sub butPickFolder_Click(sender As Object, e As EventArgs) Handles butPickFolder.Click
        pickFolder()
    End Sub

    Private Sub boxCheck_Check(sender As Object, e As EventArgs) Handles onlineCheckBox.CheckedChanged
        If onlineCheckBox.Checked = False Then Exit Sub
        refreshAddIn()
    End Sub

    ' ### Parts Tree


    ' ### Status

    Private Sub StatusStrip2_ItemClicked(sender As Object, e As Windows.Forms.ToolStripItemClickedEventArgs)
        updateStatusStrip()
    End Sub
    Public Sub externalSetReadWriteFromLockStatus1()
        externalSetReadWriteFromLockStatus()
    End Sub
    Public Function refreshAddIn(Optional bsaveLocalRepoPathSettings As Boolean = True) As Boolean

        If Not verifyLocalRepoPath(, bCheckLocalFolder:=True, bCheckServer:=False) Then Return False     'Only need to check the local since updateStatusOfAllModelsVariable will check server. 

        Dim pathArr() As String = IO.Directory.GetDirectories(localRepoPath.Text, "*.*", IO.SearchOption.AllDirectories)
        'Dim sUserPreference As String

        'Set the referenced folder file to the local repository.
        'This will allow solidworks to find the files.
        'https://blogs.solidworks.com/tech/2014/06/search-path-order-for-opening-files-in-solidworks.html
        'http://help.solidworks.com/2012/English/api/swconst/SO_FileLocations.htm

        '=== Had to comment out, since it was adding 5000 file locations references to SolidWorks====
        ''Add all the subdirectories of the repo to the "reference files location" 
        '' This will let solidworks find the files!
        'For Each myPath In pathArr
        '    If myPath.Contains("\.svn") Then Continue For 'Skips the hidden folder that contains all the backup files

        '    sUserPreference = iSwApp.GetUserPreferenceStringValue(
        '        swUserPreferenceStringValue_e.swFileLocationsDocuments) 'Get existing preferences

        '    iSwApp.SetUserPreferenceStringValue(
        '        swUserPreferenceStringValue_e.swFileLocationsDocuments,
        '        sUserPreference & ";" & myPath)
        'Next

        'TODO: try to change setuserPreference to only be once in this file

        'Also: Prevent multiple files with the same name to be added to the vault!

        ''iSwApp.SetUserPreferenceStringValue(swUserPreferenceStringValue_e.swFileLocationsDocuments, "C:\Users\benne\Documents\SVN\fsae9\CAD\Subfolder")

        If updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True) Then
            switchTreeViewToCurrentModel(bRetryWithRefresh:=False)
        End If
        saveLocalRepoPathSettings()
    End Function

    Public Sub saveLocalRepoPathSettings()
        My.Settings.localRepoPath = localRepoPath.Text
        My.Settings.Save()
    End Sub

    Public Function pickFolder() As DialogResult
        Dim folderDlg As FolderBrowserDialog = New FolderBrowserDialog()
        Dim result As DialogResult = folderDlg.ShowDialog()
        Dim sTempPath As String

        If (result = DialogResult.OK) Then
            sTempPath = folderDlg.SelectedPath
            'Environment.SpecialFolder root = folderDlg.RootFolder
            sTempPath = sTempPath.TrimEnd("\\")
            localRepoPath.Text = sTempPath
        End If

        Return result

        verifyLocalRepoPath()
    End Function

    Sub treeView1_NodeMouseClick(ByVal sender As Object,
    ByVal e As TreeNodeMouseClickEventArgs) _
    Handles TreeView1.NodeMouseClick

        'Dim sText As String = e.Node.Text
        'Dim modDoc As ModelDoc2
        Dim comp As Component2
        Dim activeModel As ModelDoc2 = iSwApp.ActiveDoc
        'Dim sText As String = localRepoPath.Text & "\" & e.Node.Text
        If activeModel Is Nothing Then Exit Sub

        If activeModel.GetType <> swDocumentTypes_e.swDocASSEMBLY Then Exit Sub

        'Debug.Assert(False, sText)
        'modDoc = activeModel.GetComponentByName(e.Node.Text)


        'Debug.Print(TypeOf e.Node.Tag)
        If TypeOf e.Node.Tag Is Component2 Then
            'If e.Node.Tag.GetType.ToString = "Component2" Then
            comp = e.Node.Tag
            comp.Select(False)
        End If

    End Sub

    Public Sub updateStatusStrip()

        'Exit Sub 'disabling for speed

        'Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        'If modDoc Is Nothing Then Exit Sub

        'Dim myCol As myColours = New myColours()
        'Dim status As SVNStatus = findStatusForFile(modDoc.GetPathName)
        'If IsNothing(status) Then Exit Sub

        'myCol.initialize()
        'If IsNothing(status) Then
        '    StatusStrip2.Text = ""
        '    StatusStrip2.BackColor = myCol.unknown
        'ElseIf status.fp(0).addDelChg1 = "?" Then
        '    StatusStrip2.Text = "File is not saved on the Vault"
        '    StatusStrip2.BackColor = myCol.notOnVault
        'ElseIf status.fp(0).lock6 = "K" Then
        '    StatusStrip2.Text = "Locked by you"
        '    StatusStrip2.BackColor = myCol.lockedByYou
        'ElseIf status.fp(0).lock6 = "O" Then
        '    StatusStrip2.Text = "Locked By someone Else"
        '    StatusStrip2.BackColor = myCol.lockedBySomeoneElse
        'ElseIf status.fp(0).lock6 = " " Then
        '    StatusStrip2.Text = "Available"
        '    StatusStrip2.BackColor = myCol.available
        'End If
    End Sub

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

    Public Sub switchTreeViewToCurrentModel(Optional bRetryWithRefresh As Boolean = True)

        If Not onlineCheckBox.Checked Then Exit Sub

        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        If modDoc Is Nothing Then Exit Sub

        Dim treeNodeIndex As Integer = findStoredTreeView(modDoc.GetPathName, bRetryWithRefresh)
        If IsNothing(treeNodeIndex) Or IsNothing(allTreeViews) Then Exit Sub
        If Not onlineCheckBox.Checked Then Exit Sub

        Dim treeNodeTemp As TreeNode = allTreeViews(treeNodeIndex).Nodes(0)

        Dim clonedNode As TreeNode = CType(treeNodeTemp.Clone(), TreeNode)

        TreeView1.Nodes.Clear()
        TreeView1.Nodes.Insert(0, clonedNode)
        TreeView1.Nodes(0).Expand()
        'TreeView1.ExpandAll()
        TreeView1.Show()

    End Sub
    Function findStoredTreeView(pathName As String, Optional bRetryWithRefresh As Boolean = True) As Integer
        Dim i As Integer
        Dim bSuccess As Boolean
        'Dim bFound As Boolean = False

        If IsNothing(allTreeViews) Then
            bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
            If Not bSuccess Then iSwApp.SendMsgToUser("Status Update Failed.") : Return Nothing
            bRetryWithRefresh = False
        End If

        If allTreeViews.Length = 0 Then
            bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
            If Not bSuccess Then iSwApp.SendMsgToUser("Status Update Failed.") : Return Nothing
            bRetryWithRefresh = False
        End If


        'Try to find it using the existing allTreeViews object. This is the fastest
        For i = 0 To UBound(allTreeViews)
            If allTreeViews(i).Nodes.Count = 0 Then Continue For
            If (Strings.InStr(allTreeViews(i).Nodes(0).Text, System.IO.Path.GetFileName(pathName), CompareMethod.Text) <> 0) Then
                Return i
            End If
        Next

        If Not bRetryWithRefresh Then Return Nothing
        bSuccess = updateStatusOfAllModelsVariable(bRefreshAllTreeViews:=True)
        If Not bSuccess Then iSwApp.SendMsgToUser("Status Update Failed.") : Return Nothing

        For i = 0 To UBound(allTreeViews)
            If allTreeViews(i).Nodes.Count > 0 Then
                If (Strings.InStr(allTreeViews(i).Nodes(0).Text, System.IO.Path.GetFileName(pathName), CompareMethod.Text) <> 0) Then
                    Return i
                End If
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
            allTreeViews(i).Visible = False
            getComponentsOfAssemblyOptionalUpdateTree({modDocArray(i)}, i)
        Next
    End Sub

    Public Function getComponentsOfAssemblyOptionalUpdateTree(
                                    ByRef modDocArr() As ModelDoc2,
                                    Optional ByVal allTreeViewsIndexToUpdate As Integer = Nothing,
                                    Optional ByVal bUniqueOnly As Boolean = True) As ModelDoc2()

        ' Checkin and checkout needs the modDocArray. The others just want filepaths. 

        Dim bUpdateTreeView As Boolean = If(IsNothing(allTreeViewsIndexToUpdate), False, True)
        Dim sFileNameTemp As String
        Dim parentNode As TreeNode = Nothing
        Dim modelDocList As New List(Of ModelDoc2)()
        Dim swConfMgr As ConfigurationManager
        Dim swConf As Configuration
        Dim swRootComp As Component2
        Dim modDocTemp As ModelDoc2

        Dim i, j As Integer
        j = 0

        If (UBound(modDocArr) > 0) And (Not IsNothing(allTreeViewsIndexToUpdate)) Then
            iSwApp.SendMsgToUser("Error: getComponentsOfAssemblyOptionalUpdateTree wasn't written to update tree views on multiple assemblies")
            Return Nothing
        End If

        For i = 0 To UBound(modDocArr)

            If IsNothing(modDocArr(i)) Then Continue For
            sFileNameTemp = System.IO.Path.GetFileName(modDocArr(i).GetPathName)

            'If modDocArr(i).GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            '    'check if it's actually an assembly...
            '    modelDocList.Add(modDocArr(i))
            '    Continue For
            'End If

            If bUpdateTreeView Then
                allTreeViews(allTreeViewsIndexToUpdate).Visible = False


                'For k = 0 To (allTreeViews(allTreeViewsIndexToUpdate).Nodes.Count - 1)
                '    allTreeViews(allTreeViewsIndexToUpdate).Nodes(0).Remove()
                'Next
                'allTreeViews(allTreeViewsIndexToUpdate).Nodes.Clear() ' <- HAS A BUG THAT FREEZES IN INTERNAL LOOP
                allTreeViews(allTreeViewsIndexToUpdate) = Nothing
                allTreeViews(allTreeViewsIndexToUpdate) = New TreeView

                parentNode = New TreeNode(sFileNameTemp)
                parentNode.Tag = modDocArr(0)
            End If

            If modDocArr(i).GetType = swDocumentTypes_e.swDocASSEMBLY Then
                swConfMgr = modDocArr(i).ConfigurationManager
                swConf = swConfMgr.ActiveConfiguration
                swRootComp = swConf.GetRootComponent3(True)

                TraverseComponent(swRootComp, modelDocList, 1, parentNode, bUniqueOnly)
                j += 1
            ElseIf modDocArr(i).GetType = swDocumentTypes_e.swDocDRAWING Then

                If bUpdateTreeView Then
                    setNodeColorFromStatus(parentNode)
                    'allTreeViews(allTreeViewsIndexToUpdate).Nodes.Add(parentNode)
                End If

                modelDocList.Add(modDocArr(i)) 'Add drawing
                j += 1
                'Try to find part/asy for drawing

                modDocTemp = iSwApp.GetOpenDocumentByName(System.IO.Path.ChangeExtension(modDocArr(i).GetPathName(), ".sldprt"))
                If Not (modDocTemp Is Nothing) Then
                    modelDocList.Add(modDocTemp)
                    j += 1
                Else
                    'couldn't find part. try again as assembly
                    modDocTemp = iSwApp.GetOpenDocumentByName(System.IO.Path.ChangeExtension(modDocArr(i).GetPathName(), ".sldasm"))
                    If Not (modDocTemp Is Nothing) Then
                        modelDocList.Add(modDocTemp)
                        j += 1
                    End If
                End If
            Else 'Part file
                'Should be a part file... not sure what else there is
                If bUpdateTreeView Then
                    setNodeColorFromStatus(parentNode)
                    allTreeViews(allTreeViewsIndexToUpdate).Nodes.Add(parentNode)
                End If
                modelDocList.Add(modDocArr(i))
                j += 1
                'iswApp.SendMsgToUser("Error: Model is not an assembly.")
                'Throw New System.Exception("modDoc is not an Assembly")


            End If
        Next

        If j = 0 Then
            iSwApp.SendMsgToUser("Couldn't find model")
            Return Nothing
        End If

        Dim mdComponentArr() As ModelDoc2 = modelDocList.ToArray
        If bUpdateTreeView Then
            allTreeViews(allTreeViewsIndexToUpdate).Sort()
            allTreeViews(allTreeViewsIndexToUpdate).Nodes.Add(parentNode)   'Do we really need this??
        End If

        Return mdComponentArr
    End Function

    Sub TraverseComponent(
                         ByRef swComp As Component2,
                         ByRef mdComponentList As List(Of ModelDoc2),
                         ByVal nLevel As Long,
                         Optional ByRef rootNode As TreeNode = Nothing,
                         Optional ByVal bUniqueOnly As Boolean = True)

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
        Dim slComponentList As List(Of String) = New List(Of String)

        mdComponentList.Add(modDocParent)
        slComponentList.Add(modDocParent.GetTitle)

        If bUC Then
            parentNode = New TreeNode(sParentFileName) '& " " & sGetDescription(modDocParent)
            parentNode.Tag = swComp 'modDocParent
            setNodeColorFromStatus(parentNode)
        End If

        Debug.Print(swComp.GetPathName())

        vChildComp = swComp.GetChildren
        If vChildComp Is Nothing Then Exit Sub 'I dunno why this would happen, but gotta prevent errors. Maybe all the children are lightweight??
        For i = 0 To UBound(vChildComp)
            swChildComp = vChildComp(i)
            Debug.Print(swChildComp.GetPathName())
            If swChildComp.IsEnvelope Then Continue For 'Skip envelope components
            modDocChild = swChildComp.GetModelDoc2
            If IsNothing(modDocChild) Then Continue For
            If modDocChild.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
                'Is part file
                If mdComponentList.Contains(modDocChild) Then Continue For 'avoid duplicates <- I don't know if this actually works...
                If slComponentList.Contains(modDocChild.GetTitle) And (bUniqueOnly) Then Continue For 'avoid duplicates

                If bUC Then
                    sChildFileName = System.IO.Path.GetFileName(modDocChild.GetPathName)
                    childNode = New TreeNode(sChildFileName) '& " " & sGetDescription(modDocChild)
                    childNode.Tag = swChildComp 'modDocChild
                    setNodeColorFromStatus(childNode)
                    parentNode.Nodes.Add(childNode)
                End If

                mdComponentList.Add(modDocChild)
                slComponentList.Add(modDocChild.GetTitle)
            ElseIf modDocChild.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                'Is assembly
                If mdComponentList.Contains(modDocChild) Then Continue For 'avoid duplicates <- I don't know if this actually works...
                If slComponentList.Contains(modDocChild.GetTitle) And (bUniqueOnly) Then Continue For 'avoid duplicates

                TraverseComponent(swChildComp, mdComponentList, nLevel + 1, parentNode, bUniqueOnly)

                mdComponentList.Add(modDocChild)
                slComponentList.Add(modDocChild.GetTitle)
            Else
                Exit Sub
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
    Public Class myContextMenuClass

        Public Shared iSwApp2 As SldWorks
        Dim modDoc As ModelDoc2
        Dim modDocArr As ModelDoc2()
        Dim parentUserControl2 As UserControl1
        'Dim comp As Component2
        Public collapse As New ToolStripMenuItem("Collapse", My.Resources.VaultLogo128, AddressOf collapseTreeViewHandler)
        Public openLabel As New ToolStripMenuItem("Open", My.Resources.VaultLogo128, AddressOf openEventHandler)
        Public unlockLabel As New ToolStripMenuItem("Unlock", My.Resources.unlockIconOnly1, AddressOf unlockEventHandler)
        Public unlockWithDependentsLabel As New ToolStripMenuItem("Unlock With Dependents", My.Resources.unlockIconOnly1, AddressOf unlockWithDependentsEventHandler)
        Public commitLabel As New ToolStripMenuItem("Commit", My.Resources.Commit_Icon_Only, AddressOf commitEventHandler)
        Public commitWithDependentsLabel As New ToolStripMenuItem("Commit With Dependents", My.Resources.Commit_Icon_Only, AddressOf commitWithDependentsEventHandler)
        Public getLocksStealLabel As New ToolStripMenuItem("Get Lock (Steal Locks)", My.Resources.GetLocksIconOnly, AddressOf getLockStealLockEventHandler)
        Public getLockActiveDoc As New ToolStripMenuItem("Get Lock", My.Resources.GetLocksIconOnly, AddressOf getLockActiveDocEventHandler)
        Public getLockWithDependents As New ToolStripMenuItem("Get Lock With Dependents", My.Resources.GetLocksIconOnly, AddressOf getLocksActiveWithDependentsEventHandler)
        Public addToRepo As New ToolStripMenuItem("Add & Initial Commit", My.Resources.VaultLogo128, AddressOf addToRepoEventHandler)
        Public showLog As New ToolStripMenuItem("View SVN Log", My.Resources.VaultLogo128, AddressOf showLogEventHandler)
        Public upRevEdit As New ToolStripMenuItem("Up Rev to Edit", My.Resources.VaultLogo128, AddressOf upRevEditEventHandler)
        Public release As New ToolStripMenuItem("Approve & Release", My.Resources.VaultLogo128, AddressOf releaseEventHandler)
        Public Sub New(modDocInput As ModelDoc2, iSwAppInput As SldWorks, parentUserControl As UserControl1)
            modDoc = modDocInput 'compInput.GetModelDoc2
            'comp = compInput
            iSwApp2 = iSwAppInput
            parentUserControl2 = parentUserControl
        End Sub
        Sub upRevEditEventHandler(sender As Object, e As EventArgs)
            editNewRev({modDoc})
        End Sub
        Sub releaseEventHandler(sender As Object, e As EventArgs)
            myReleaseDoc(modDoc)
        End Sub
        Sub collapseTreeViewHandler(sender As Object, e As EventArgs)
            parentUserControl2.TreeView1.CollapseAll()
        End Sub
        Sub openEventHandler(sender As Object, e As EventArgs)
            iSwApp2.ActivateDoc3(modDoc.GetPathName, True, swRebuildOnActivation_e.swUserDecision, 0)
        End Sub
        Sub unlockEventHandler(sender As Object, e As EventArgs)
            unlockDocs({modDoc})
        End Sub
        Sub unlockWithDependentsEventHandler(sender As Object, e As EventArgs)
            myUnlockWithDependents(modDoc)
        End Sub
        Sub commitEventHandler(sender As Object, e As EventArgs)
            tortCommitDocs({modDoc})
        End Sub
        Public Sub commitWithDependentsEventHandler(sender As Object, e As EventArgs)
            modDocArr = parentUserControl2.GetSelectedModDocList(iSwApp2)
            tortCommitDocs(parentUserControl2.getComponentsOfAssemblyOptionalUpdateTree(modDocArr))
        End Sub
        Sub getLockStealLockEventHandler(sender As Object, e As EventArgs)
            If swMessageBoxResult_e.swMbHitOk =
            iSwApp2.SendMsgToUser2("File is Currently checked out by another user. You can steal their " &
                                   "Locks by clicking the checkbox in the next window. If both you and that user " &
                                   "attempt to check in their copies, a conflict can occur. Always communicate " &
                                   "your intention to break someone's lock with that user.",
                                    swMessageBoxIcon_e.swMbWarning, swMessageBoxBtn_e.swMbOkCancel) Then
                getLocksOfDocs({modDoc}, bBreakLocks:=True)
            End If
        End Sub
        Sub getLockActiveDocEventHandler(sender As Object, e As EventArgs)
            getLocksOfDocs(parentUserControl2.GetSelectedModDocList(iSwApp2))
        End Sub
        Sub getLocksActiveWithDependentsEventHandler(sender As Object, e As EventArgs)
            getLocksOfDocs(parentUserControl2.getComponentsOfAssemblyOptionalUpdateTree(parentUserControl2.GetSelectedModDocList(iSwApp2)))
        End Sub
        Sub addToRepoEventHandler(sender As Object, e As EventArgs)

            addtoRepoFunc(parentUserControl2.GetSelectedModDocList(iSwApp2))
        End Sub
        Sub showLogEventHandler(sender As Object, e As EventArgs)
            subShowLog(modDoc.GetPathName)
        End Sub
    End Class
    ' TODO

    ' make the treenode tag attach a custom class that contains component, modDoc, filepath, description, maybe all the svnstatus stuff too? 
    Function getModDocAttachedToNode(rootNode As TreeNode) As ModelDoc2
        Dim comp As Component2

        If Not IsNothing(rootNode.Tag) Then
            If TypeOf rootNode.Tag Is Component2 Then
                comp = rootNode.Tag
                If Not ensureResolvedComponent(comp) Then Return Nothing
                getModDocAttachedToNode = comp.GetModelDoc2
            ElseIf TypeOf rootNode.Tag Is ModelDoc2 Then

                getModDocAttachedToNode = rootNode.Tag
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
        Return getModDocAttachedToNode
    End Function

    Sub setNodeColorFromStatus(
        ByRef rootNode As TreeNode)

        Dim myCol As myColours = New myColours()
        myCol.initialize()
        Dim status1 As SVNStatus
        Dim modDoc As ModelDoc2
        'Dim comp As Component2

        Dim bModelDocAttached As Boolean '= If(IsNothing(rootNode.Tag), False, True) ' True is modelDoc is attached to node
        Dim myContextMenu As myContextMenuClass

        Dim docMenu As ContextMenuStrip
        docMenu = New ContextMenuStrip()

        'If bCM Then
        '    rootNode.ContextMenuStrip.Items.Add(myContextMenu.openLabel)
        'End If

        modDoc = getModDocAttachedToNode(rootNode)
        status1 = findStatusForFile(rootNode.Text) 'IMPROVE ME FIXME. We want to include description, but still need to find the filename. Try separating by "*" since that is not allowed in filenames

        If modDoc Is Nothing Then
            bModelDocAttached = False
        Else
            bModelDocAttached = True
        End If

        myContextMenu = New myContextMenuClass(modDoc, iSwApp, Me) ' This gets overwritten immediately. It's just here to prevent pre-compile warnings
        If bModelDocAttached Then
            myContextMenu = New myContextMenuClass(modDoc, iSwApp, Me)
            docMenu.Items.AddRange({myContextMenu.openLabel, myContextMenu.collapse, myContextMenu.showLog})
            'modDoc = rootNode.Tag
        End If

        If status1 Is Nothing Then
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"

        ElseIf status1.fp(0).lock6 = "K" Then
            rootNode.BackColor = myCol.lockedByYou
            rootNode.ToolTipText = "You have the Lock"

            If bModelDocAttached Then
                docMenu.Items.AddRange({myContextMenu.release})
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.AddRange(
                        {myContextMenu.commitLabel,
                        myContextMenu.commitWithDependentsLabel,
                        myContextMenu.unlockLabel,
                        myContextMenu.unlockWithDependentsLabel})
                Else
                    docMenu.Items.AddRange(
                        {myContextMenu.commitLabel,
                        myContextMenu.unlockLabel})
                End If
            End If

        ElseIf status1.fp(0).upToDate9 = "*" Then
            rootNode.BackColor = myCol.outOfDate
            rootNode.ToolTipText = "Your Copy is Out Of Date"
            'If bModelDocAttached Then docMenu.Items.AddRange({myContextMenu.getLocksStealLabel})

        ElseIf status1.fp(0).lock6 = "O" Then
            rootNode.BackColor = myCol.lockedBySomeoneElse
            rootNode.ToolTipText = "Locked By Someone Else"
            If bModelDocAttached Then
                docMenu.Items.AddRange({myContextMenu.getLocksStealLabel})
                'If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.Add(myContextMenu.commitWithDependentsLabel)
                End If
            End If
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.getLocksStealLabel)
        ElseIf status1.fp(0).released = "||RELEASED||" Then
            rootNode.BackColor = myCol.released
            rootNode.ToolTipText = "Released"
            If bModelDocAttached Then
                docMenu.Items.AddRange({myContextMenu.upRevEdit})
            End If
        ElseIf status1.fp(0).addDelChg1 = "?" Then
            rootNode.BackColor = myCol.notOnVault
            rootNode.ToolTipText = "File is not saved the to the Vault"
            If bModelDocAttached Then
                docMenu.Items.Add(myContextMenu.addToRepo)
            End If

        ElseIf status1.fp(0).lock6 = " " Then
            rootNode.BackColor = myCol.available
            rootNode.ToolTipText = "Available"
            If bModelDocAttached Then
                docMenu.Items.Add(myContextMenu.getLockActiveDoc)
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.AddRange({myContextMenu.commitWithDependentsLabel,
                                           myContextMenu.getLockWithDependents})
                End If
            End If
        Else
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"
            'If bModelDocAttached Then docMenu.Items.AddRange({myContextMenu.openLabel})

        End If


        rootNode.ContextMenuStrip = docMenu
    End Sub
    Public Sub TestMethod()
        'MsgBox("The strings in the flavorEnum are:")
        Dim i As String
        Dim j As Integer = 0
        For Each i In [Enum].GetNames(GetType(swSelectType_e))

            Debug.Print(j & " - " & i)
            j += 1
        Next
    End Sub

    Public Function GetSelectedModDocList(iSwApp As SolidWorks.Interop.sldworks.SldWorks) As SolidWorks.Interop.sldworks.ModelDoc2() 'SolidWorks.Interop.sldworks.Component2()

        'Returns the active doc if nothing is selected

        Dim swSelCompArr() As SolidWorks.Interop.sldworks.Component2
        Dim modDocArr() As SolidWorks.Interop.sldworks.ModelDoc2
        Dim swComp As SolidWorks.Interop.sldworks.Component2
        Dim obSelected As Object
        Dim i As Long
        'Dim tempObj As Object
        'swSelectType_e.swSelSHEETS
        Dim activeModDoc As ModelDoc2 = iSwApp.ActiveDoc
        Dim swSelMgr As SolidWorks.Interop.sldworks.SelectionMgr = activeModDoc.SelectionManager
        Dim nSelCount As Long = swSelMgr.GetSelectedObjectCount2(-1)

        Dim myNames As String() = [Enum].GetNames(GetType(swSelectType_e))

        ReDim swSelCompArr(nSelCount - 1)
        ReDim modDocArr(0)

        If Not ((activeModDoc.GetType = swDocumentTypes_e.swDocPART) Or (activeModDoc.GetType = swDocumentTypes_e.swDocASSEMBLY)) Then
            'prevent selection manager (used later) from fatal errors on other files types
            Return {activeModDoc}
        End If

        For i = 1 To nSelCount
            ' need to grab all the components first before doing lightweight->resolve, otherwise the selection manager return 'nothing' for lightweight
            swSelCompArr(i - 1) = swSelMgr.GetSelectedObjectsComponent4(i, -1)
        Next

        For i = 1 To nSelCount

            swComp = swSelCompArr(i - 1)
            If ensureResolvedComponent(swComp) Then
                modDocArr(UBound(modDocArr)) = swComp.GetModelDoc2
            Else

                'unable to resolve component... maybe they had the top level selected? 
                obSelected = swSelMgr.GetSelectedObject6(i, -1)
                If obSelected Is Nothing Then Continue For

                Try
                    If obSelected.getPathName = activeModDoc.GetPathName Then 'check if they selected the top level
                        'They selected the top level... this was the only way I could pull it off
                        modDocArr(UBound(modDocArr)) = activeModDoc
                    Else
                        'couldn't get the component... not sure what they selected
                        Continue For
                    End If
                Catch ex As Exception
                    Continue For
                End Try


            End If

            ReDim Preserve modDocArr(UBound(modDocArr) + 1)
            'swSelCompArr(UBound(swSelCompArr)) = swComp
            'ReDim Preserve swSelCompArr(UBound(swSelCompArr) + 1)
        Next i

        If IsNothing(modDocArr(0)) Then
            'Return active doc if nothing is selected
            Return {activeModDoc}
        End If

        'Debug.Assert UBound(swSelCompArr) > 0
        'ReDim Preserve swSelCompArr(UBound(swSelCompArr) - 1)

        ReDim Preserve modDocArr(UBound(modDocArr) - 1)

        Return modDocArr

    End Function
    Class myColours
        Public lighterPurple As Drawing.Color
        Public darkerPurple As Drawing.Color
        Public lockedByYou As Drawing.Color
        Public lockedBySomeoneElse As Drawing.Color
        Public available As Drawing.Color
        Public unknown As Drawing.Color
        Public outOfDate As Drawing.Color
        Public notOnVault As Drawing.Color
        Public released As Drawing.Color
        Public Sub initialize()
            lighterPurple = Drawing.Color.FromArgb(208, 207, 229) 'used in icons
            darkerPurple = Drawing.Color.FromArgb(152, 150, 182) 'used in icons
            lockedByYou = Drawing.Color.FromArgb(159, 223, 159) 'Drawing.Color.Aquamarine
            lockedBySomeoneElse = Drawing.Color.FromArgb(255, 255, 153)
            available = Drawing.Color.White
            unknown = Drawing.Color.LightGray
            outOfDate = Drawing.Color.FromArgb(255, 129, 123)
            released = darkerPurple
            notOnVault = unknown
            'Drawing.Color.Bisque 'Drawing.Color.FromArgb(255, 77, 77) 'light red
        End Sub
    End Class

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub ApproveReleaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ApproveReleaseToolStripMenuItem.Click
        Dim modDocArr() As ModelDoc2 = GetSelectedModDocList(iSwApp)

        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc

        If UBound(modDocArr) > 0 Then
            If iSwApp.SendMsgToUser2("Only one component can be released at a time. Would you like to release the assembly " & vbCrLf & modDoc.GetTitle & " ?",
                        swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbYesNoCancel) <> swMessageBoxResult_e.swMbHitOk Then
                Exit Sub
            End If
        Else
            modDoc = modDocArr(0)
        End If

        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Document not found") : Exit Sub
        myReleaseDoc(modDoc)
    End Sub

    Private Sub EditNewRevisionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditNewRevisionToolStripMenuItem.Click
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub
        editNewRev(GetSelectedModDocList(iSwApp))
    End Sub

    Private Sub ToolStripDropDownButReleases_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripDropDownButReleases.ButtonClick
        ToolStripDropDownButReleases.ShowDropDown()
    End Sub
End Class
