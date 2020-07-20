
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Drawing
'Imports System.Configuration

<ProgId("SVN_AddIn")>
Public Class UserControl1

    Public WithEvents iSwApp As SldWorks
    'Dim userAddin As SwAddin = New SwAddin() 'couldn't get access to swapp in here!

    'Public Const localRepoPath.text As String = "E:\SolidworksBackup\svn"
    'Public Const localRepoPath.text As String = "C:\Users\benne\Documents\SVN\cad1"

    Public statusOfAllOpenModels As SVNStatus
    Public allOpenDocs As ModelDoc2()
    Public allTreeViews As TreeView() = {New TreeView}

    Private Sub UserControl1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
           Handles MyBase.Load

        Dim docMenu As ContextMenuStrip
        Dim myToolItem As ToolStripMenuItem

        docMenu = New ContextMenuStrip()
        myToolItem = New ToolStripMenuItem("Refresh", My.Resources.VaultLogo128, AddressOf RefreshToolStripMenuItem_click)
        docMenu.Items.AddRange({myToolItem})

        Me.ContextMenuStrip = docMenu

        localRepoPath.Text = My.Settings.localRepoPath

    End Sub

    Friend Sub myInitialize(ByRef swAppin As SldWorks)
        'Allows for swApp to be passed into this class.
        iSwApp = swAppin


        initializeSwModelFunctions(iSwApp)
            svnModuleInitialize(iSwApp, Me, statusOfAllOpenModels)


        If verifyLocalRepoPath() Then
            iSwApp.SetUserPreferenceStringValue(
                swUserPreferenceStringValue_e.swFileLocationsDocuments,
                localRepoPath.Text)
        End If
    End Sub
    Friend Sub beforeClose()
        saveLocalRepoPathSettings()
    End Sub
    Private Sub butCheckinWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckinWithDependents.Click
        myCheckinWithDependents(iSwApp.ActiveDoc())
        updateStatusStrip()
    End Sub
    Private Sub butCheckinAll_Click(sender As Object, e As EventArgs) Handles butCheckinAll.Click
        myCheckinAll()
        updateStatusStrip()
    End Sub
    Private Sub RefreshToolStripMenuItem_click(sender As Object, e As EventArgs)

        refreshAddIn()
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
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub
        myCheckoutDoc(modDoc)
        updateStatusStrip()
    End Sub

    Private Sub butCheckoutWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckoutWithDependents.Click
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then iSwApp.SendMsgToUser("Error: Active Document not found") : Exit Sub
        myCheckoutWithDependents(modDoc)
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
    Private Sub boxCheck_Check(sender As Object, e As EventArgs) Handles onlineCheckBox.CheckedChanged
        If onlineCheckBox.Checked = False Then Exit Sub
        refreshAddIn()
    End Sub
    Private Sub butPickFolder_Click(sender As Object, e As EventArgs) Handles butPickFolder.Click
        pickFolder()
    End Sub
    Public Sub refreshAddIn()

        If Not verifyLocalRepoPath() Then Exit Sub

        'Set the referenced folder file to the local repository.
        'This will allow solidworks to find the files.
        'https://blogs.solidworks.com/tech/2014/06/search-path-order-for-opening-files-in-solidworks.html
        'http://help.solidworks.com/2012/English/api/swconst/SO_FileLocations.htm

        'iSwApp.SetUserPreferenceStringValue(
        '    swUserPreferenceStringValue_e.swFileLocationsDocuments,
        '    localRepoPath.Text)

        ''iSwApp.SetUserPreferenceStringValue(swUserPreferenceStringValue_e.swFileLocationsDocuments, "C:\Users\benne\Documents\SVN\fsae9\CAD\Subfolder")

        updateStatusOfAllModelsVariable(True)
        switchTreeViewToCurrentModel(bRetryWithRefresh:=False)

        saveLocalRepoPathSettings()
    End Sub

    Public Sub saveLocalRepoPathSettings()
        My.Settings.localRepoPath = localRepoPath.Text
        My.Settings.Save()
    End Sub

    Public Function pickFolder() As DialogResult
        Dim folderDlg As FolderBrowserDialog = New FolderBrowserDialog()
        Dim result As DialogResult = folderDlg.ShowDialog()

        If (result = DialogResult.OK) Then
            localRepoPath.Text = folderDlg.SelectedPath
            'Environment.SpecialFolder root = folderDlg.RootFolder
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
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc
        If modDoc Is Nothing Then Exit Sub

        Dim myCol As myColours = New myColours()
        Dim status As SVNStatus = findStatusForFile(modDoc.GetPathName)
        If IsNothing(status) Then Exit Sub

        myCol.initialize()
        If IsNothing(status) Then
            StatusStrip2.Text = ""
            StatusStrip2.BackColor = myCol.unknown
        ElseIf status.fp(0).addDelChg1 = "?" Then
            StatusStrip2.Text = "File is not saved on the Vault"
            StatusStrip2.BackColor = myCol.notOnVault
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
            parentNode = New TreeNode(sParentFileName)
            parentNode.Tag = swComp 'modDocParent
            setNodeColorFromStatus(parentNode)
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
                    childNode.Tag = swChildComp 'modDocChild
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
        'Dim comp As Component2
        Public openLabel As New ToolStripMenuItem("Open", My.Resources.VaultLogo128, AddressOf openEventHandler)
        Public unlockLabel As New ToolStripMenuItem("Release Lock", My.Resources.ReleaseActive, AddressOf unlockEventHandler)
        Public unlockWithDependentsLabel As New ToolStripMenuItem("Release Lock With Dependents", My.Resources.ReleaseAll, AddressOf unlockWithDependentsEventHandler)
        Public checkInLabel As New ToolStripMenuItem("Check In", My.Resources.CheckInActive, AddressOf checkInEventHandler)
        Public checkInWithDependentsLabel As New ToolStripMenuItem("Check In With Dependents", My.Resources.CheckInAll, AddressOf checkInWithDependentsEventHandler)
        Public checkOutStealLabel As New ToolStripMenuItem("Check Out (Steal Locks)", My.Resources.CheckOutActive, AddressOf checkOutStealLockEventHandler)
        Public checkOutActiveDoc As New ToolStripMenuItem("Check Out Doc", My.Resources.CheckOutActive, AddressOf checkOutActiveDocEventHandler)
        Public checkOutWithDependents As New ToolStripMenuItem("Check Out With Dependents", My.Resources.CheckOutWithDependents, AddressOf checkOutActiveWithDependentsEventHandler)
        Public Sub New(modDocInput As ModelDoc2, iSwAppInput As SldWorks)
            modDoc = modDocInput 'compInput.GetModelDoc2
            'comp = compInput
            iSwApp2 = iSwAppInput
        End Sub

        Sub openEventHandler(sender As Object, e As EventArgs)
            iSwApp2.ActivateDoc3(modDoc.GetPathName, True, swRebuildOnActivation_e.swUserDecision, 0)
        End Sub
        Sub unlockEventHandler(sender As Object, e As EventArgs)
            unlockDocs({modDoc})
        End Sub
        Sub unlockWithDependentsEventHandler(sender As Object, e As EventArgs)
            myUnlockWithDependents({modDoc})
        End Sub
        Sub checkInEventHandler(sender As Object, e As EventArgs)
            checkInDocs({modDoc}, svnAddInUtils.createBoolArray(1, True))
        End Sub
        Sub checkInWithDependentsEventHandler(sender As Object, e As EventArgs)
            myCheckinWithDependents(modDoc)
        End Sub
        Sub checkOutStealLockEventHandler(sender As Object, e As EventArgs)
            If swMessageBoxResult_e.swMbHitOk =
            iSwApp2.SendMsgToUser2("File is Currently checked out by another user. You can steal their " &
                                   "Locks by clicking the checkbox in the next window. If both you and that user " &
                                   "attempt to check in their copies, a conflict can occur. Always communicate " &
                                   "your intention to break someone's lock with that user.",
                                    swMessageBoxIcon_e.swMbWarning, swMessageBoxBtn_e.swMbOkCancel) Then
                unlockDocs({modDoc})
            End If
        End Sub
        Sub checkOutActiveDocEventHandler(sender As Object, e As EventArgs)
            myCheckoutDoc(modDoc)
        End Sub
        Sub checkOutActiveWithDependentsEventHandler(sender As Object, e As EventArgs)
            myCheckoutWithDependents(modDoc)
        End Sub

    End Class
    Sub setNodeColorFromStatus(
        ByRef rootNode As TreeNode)
        Dim myCol As myColours = New myColours()
        myCol.initialize()
        Dim status1 As SVNStatus = findStatusForFile(rootNode.Text)
        Dim modDoc As ModelDoc2
        Dim comp As Component2

        Dim bModelDocAttached As Boolean '= If(IsNothing(rootNode.Tag), False, True) ' True is modelDoc is attached to node
        Dim myContextMenu As myContextMenuClass

        Dim docMenu As ContextMenuStrip
        docMenu = New ContextMenuStrip()

        'If bCM Then
        '    rootNode.ContextMenuStrip.Items.Add(myContextMenu.openLabel)
        'End If

        If Not IsNothing(rootNode.Tag) Then
            If TypeOf rootNode.Tag Is Component2 Then
                bModelDocAttached = True
                comp = rootNode.Tag
                modDoc = comp.GetModelDoc2
            ElseIf TypeOf rootNode.Tag Is ModelDoc2 Then
                bModelDocAttached = True
                modDoc = rootNode.Tag
            Else
                bModelDocAttached = False
            End If
        Else
            bModelDocAttached = False
        End If

        If bModelDocAttached Then
            myContextMenu = New myContextMenuClass(modDoc, iSwApp)
            docMenu.Items.AddRange({myContextMenu.openLabel})
            'modDoc = rootNode.Tag
        End If

        If status1 Is Nothing Then
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"

        ElseIf status1.fp(0).lock6 = "K" Then
            rootNode.BackColor = myCol.lockedByYou
            rootNode.ToolTipText = "Checked Out By You"

            If bModelDocAttached Then
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.AddRange(
                        {myContextMenu.checkInLabel,
                        myContextMenu.checkInWithDependentsLabel,
                        myContextMenu.unlockLabel,
                        myContextMenu.unlockWithDependentsLabel})
                Else
                    docMenu.Items.AddRange(
                        {myContextMenu.checkInLabel,
                        myContextMenu.unlockLabel})
                End If
            End If

        ElseIf status1.fp(0).upToDate9 = "*" Then
            rootNode.BackColor = myCol.outOfDate
            rootNode.ToolTipText = "Your Copy is Out Of Date"
            If bModelDocAttached Then docMenu.Items.AddRange({myContextMenu.checkOutStealLabel})

        ElseIf status1.fp(0).lock6 = "O" Then
            rootNode.BackColor = myCol.lockedBySomeoneElse
            rootNode.ToolTipText = "Locked By Someone Else"
            If bModelDocAttached Then
                docMenu.Items.AddRange({myContextMenu.checkOutStealLabel})
                'If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.Add(myContextMenu.checkInWithDependentsLabel)
                End If
            End If
            'If bCM Then rootNode.ContextMenuStrip.Items.Add(myContextMenu.checkOutStealLabel)
        ElseIf status1.fp(0).addDelChg1 = "?" Then
            rootNode.BackColor = myCol.notOnVault
            rootNode.ToolTipText = "File is not saved the to the Vault"
        ElseIf status1.fp(0).lock6 = " " Then
            rootNode.BackColor = myCol.available
            rootNode.ToolTipText = "Available"
            If bModelDocAttached Then
                docMenu.Items.Add(myContextMenu.checkOutActiveDoc)
                If modDoc.GetType = swDocumentTypes_e.swDocASSEMBLY Then
                    docMenu.Items.AddRange({myContextMenu.checkInWithDependentsLabel,
                                           myContextMenu.checkOutWithDependents})
                End If
            End If
        Else
            rootNode.BackColor = myCol.unknown
            rootNode.ToolTipText = "Unknown"
            If bModelDocAttached Then docMenu.Items.AddRange({myContextMenu.openLabel})

        End If

        rootNode.ContextMenuStrip = docMenu
    End Sub
    Class myColours
        Public lockedByYou As Drawing.Color
        Public lockedBySomeoneElse As Drawing.Color
        Public available As Drawing.Color
        Public unknown As Drawing.Color
        Public outOfDate As Drawing.Color
        Public notOnVault As Drawing.Color
        Public Sub initialize()
            lockedByYou = Drawing.Color.FromArgb(159, 223, 159) 'Drawing.Color.Aquamarine
            lockedBySomeoneElse = Drawing.Color.FromArgb(174, 183, 207) 'Drawing.Color.Khaki
            available = Drawing.Color.White
            unknown = Drawing.Color.LightGray
            outOfDate = Drawing.Color.FromArgb(255, 129, 123)
            notOnVault = unknown
            'Drawing.Color.Bisque 'Drawing.Color.FromArgb(255, 77, 77) 'light red
        End Sub
    End Class

End Class
