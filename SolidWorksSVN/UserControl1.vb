
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

    'Public Const localRepoPath.text As String = "E:\SolidworksBackup\svn"
    'Public Const localRepoPath.text As String = "C:\Users\benne\Documents\SVN\cad1"

    Public statusOfAllOpenModels As SVNStatus
    Public allOpenDocs As ModelDoc2()
    Public allTreeViews As TreeView() = {New TreeView}

    Friend Sub myInitialize(ByRef swAppin As SldWorks)
        'Allows for swApp to be passed into this class.
        iSwApp = swAppin
        initializeSwModelFunctions(iSwApp)
        svnModuleInitialize(iSwApp, Me, statusOfAllOpenModels)
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
            iSwApp2.checkInDocs({modDoc}, SVNAddInUtils.createBoolArray(1, True))
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

End Class
