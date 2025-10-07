<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserControl1
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Open a File to See its Status")
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.onlineCheckBox = New System.Windows.Forms.CheckBox()
        Me.localRepoPath = New System.Windows.Forms.TextBox()
        Me.butPickFolder = New System.Windows.Forms.Button()
        Me.butRefresh = New System.Windows.Forms.Button()
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.butFindComponent = New System.Windows.Forms.Button()
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripSplitButFolder = New System.Windows.Forms.ToolStripSplitButton()
        Me.PickSVNFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFolderPickerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SVNCleanupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyFileNameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyFileNameWithDependentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyActiveFilesParentFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyFullPathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyFilesPathsWithDependentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopySvnUrlToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopySvnUrlWithDependentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShareWithColleagueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchFileNameOnlineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoogleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.McToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DigikeyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileFromURLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateSvnFilelistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateSvnFilelistWithDependentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButGetLocks = New System.Windows.Forms.ToolStripSplitButton()
        Me.dropDownGetLocksWithDependents = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButCommit = New System.Windows.Forms.ToolStripSplitButton()
        Me.dropDownCommitWithDependents = New System.Windows.Forms.ToolStripMenuItem()
        Me.dropDownCommitAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButUnlock = New System.Windows.Forms.ToolStripSplitButton()
        Me.dropDownUnlockWithDependents = New System.Windows.Forms.ToolStripMenuItem()
        Me.dropDownUnlockAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButGetLatest = New System.Windows.Forms.ToolStripSplitButton()
        Me.dropDownGetLatestAllOpenFiles = New System.Windows.Forms.ToolStripMenuItem()
        Me.dropDownGetLatestAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButReleases = New System.Windows.Forms.ToolStripSplitButton()
        Me.ApproveReleaseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditNewRevisionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel()
        Me.ContextMenu1 = New System.Windows.Forms.ContextMenu()
        Me.ContextMenu2 = New System.Windows.Forms.ContextMenu()
        Me.versionLabel = New System.Windows.Forms.Label()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.Location = New System.Drawing.Point(4, 621)
        Me.TreeView1.MinimumSize = New System.Drawing.Size(250, 194)
        Me.TreeView1.Name = "TreeView1"
        TreeNode2.Name = "Node0"
        TreeNode2.Text = "Open a File to See its Status"
        Me.TreeView1.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode2})
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(562, 329)
        Me.TreeView1.TabIndex = 10
        '
        'onlineCheckBox
        '
        Me.onlineCheckBox.AutoSize = True
        Me.onlineCheckBox.Checked = True
        Me.onlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.onlineCheckBox.Location = New System.Drawing.Point(4, 43)
        Me.onlineCheckBox.Name = "onlineCheckBox"
        Me.onlineCheckBox.Size = New System.Drawing.Size(80, 24)
        Me.onlineCheckBox.TabIndex = 14
        Me.onlineCheckBox.Text = "Online"
        Me.onlineCheckBox.UseVisualStyleBackColor = True
        '
        'localRepoPath
        '
        Me.localRepoPath.Location = New System.Drawing.Point(4, 3)
        Me.localRepoPath.Name = "localRepoPath"
        Me.localRepoPath.Size = New System.Drawing.Size(538, 26)
        Me.localRepoPath.TabIndex = 15
        Me.localRepoPath.Text = "Enter Path to Local Repository"
        '
        'butPickFolder
        '
        Me.butPickFolder.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butPickFolder.Location = New System.Drawing.Point(103, 42)
        Me.butPickFolder.Name = "butPickFolder"
        Me.butPickFolder.Size = New System.Drawing.Size(126, 25)
        Me.butPickFolder.TabIndex = 16
        Me.butPickFolder.Text = "Pick Folder"
        Me.butPickFolder.UseVisualStyleBackColor = True
        Me.butPickFolder.Visible = False
        '
        'butRefresh
        '
        Me.butRefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butRefresh.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butRefresh.Location = New System.Drawing.Point(4, 571)
        Me.butRefresh.Name = "butRefresh"
        Me.butRefresh.Size = New System.Drawing.Size(136, 44)
        Me.butRefresh.TabIndex = 17
        Me.butRefresh.Text = "Refresh"
        Me.butRefresh.UseVisualStyleBackColor = True
        '
        'butCleanup
        '
        Me.butCleanup.BackColor = System.Drawing.SystemColors.ControlLight
        Me.butCleanup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCleanup.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butCleanup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!)
        Me.butCleanup.Location = New System.Drawing.Point(246, 42)
        Me.butCleanup.Name = "butCleanup"
        Me.butCleanup.Size = New System.Drawing.Size(156, 25)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.Text = "SVN Clean Up"
        Me.butCleanup.UseVisualStyleBackColor = False
        Me.butCleanup.Visible = False
        '
        'butFindComponent
        '
        Me.butFindComponent.Enabled = False
        Me.butFindComponent.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butFindComponent.Location = New System.Drawing.Point(146, 571)
        Me.butFindComponent.Name = "butFindComponent"
        Me.butFindComponent.Size = New System.Drawing.Size(89, 44)
        Me.butFindComponent.TabIndex = 23
        Me.butFindComponent.Text = "Find"
        Me.butFindComponent.UseVisualStyleBackColor = True
        Me.butFindComponent.Visible = False
        '
        'BottomToolStripPanel
        '
        Me.BottomToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.BottomToolStripPanel.Name = "BottomToolStripPanel"
        Me.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.BottomToolStripPanel.RowMargin = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.BottomToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'TopToolStripPanel
        '
        Me.TopToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.TopToolStripPanel.Name = "TopToolStripPanel"
        Me.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.TopToolStripPanel.RowMargin = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.TopToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'RightToolStripPanel
        '
        Me.RightToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.RightToolStripPanel.Name = "RightToolStripPanel"
        Me.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.RightToolStripPanel.RowMargin = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.RightToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'LeftToolStripPanel
        '
        Me.LeftToolStripPanel.Location = New System.Drawing.Point(0, 0)
        Me.LeftToolStripPanel.Name = "LeftToolStripPanel"
        Me.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.LeftToolStripPanel.RowMargin = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.LeftToolStripPanel.Size = New System.Drawing.Size(0, 0)
        '
        'ToolStrip1
        '
        Me.ToolStrip1.CanOverflow = False
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(36, 50)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripSplitButFolder, Me.ToolStripDropDownButGetLocks, Me.ToolStripDropDownButCommit, Me.ToolStripDropDownButUnlock, Me.ToolStripDropDownButGetLatest, Me.ToolStripDropDownButReleases})
        Me.ToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow
        Me.ToolStrip1.Location = New System.Drawing.Point(4, 73)
        Me.ToolStrip1.MaximumSize = New System.Drawing.Size(400, 800)
        Me.ToolStrip1.MinimumSize = New System.Drawing.Size(100, 200)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(183, 481)
        Me.ToolStrip1.Stretch = True
        Me.ToolStrip1.TabIndex = 0
        '
        'ToolStripSplitButFolder
        '
        Me.ToolStripSplitButFolder.AutoSize = False
        Me.ToolStripSplitButFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripSplitButFolder.DropDownButtonWidth = 50
        Me.ToolStripSplitButFolder.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PickSVNFolderToolStripMenuItem, Me.SVNCleanupToolStripMenuItem, Me.CopyToClipboardToolStripMenuItem, Me.SearchFileNameOnlineToolStripMenuItem, Me.OpenFileFromURLToolStripMenuItem, Me.CreateSvnFilelistToolStripMenuItem})
        Me.ToolStripSplitButFolder.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripSplitButFolder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripSplitButFolder.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSplitButFolder.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripSplitButFolder.Name = "ToolStripSplitButFolder"
        Me.ToolStripSplitButFolder.Padding = New System.Windows.Forms.Padding(2, 50, 2, 50)
        Me.ToolStripSplitButFolder.Size = New System.Drawing.Size(174, 40)
        Me.ToolStripSplitButFolder.Text = "File..."
        Me.ToolStripSplitButFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay
        '
        'PickSVNFolderToolStripMenuItem
        '
        Me.PickSVNFolderToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenFolderPickerToolStripMenuItem})
        Me.PickSVNFolderToolStripMenuItem.Name = "PickSVNFolderToolStripMenuItem"
        Me.PickSVNFolderToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.PickSVNFolderToolStripMenuItem.Text = "Pick SVN Folder"
        '
        'OpenFolderPickerToolStripMenuItem
        '
        Me.OpenFolderPickerToolStripMenuItem.Name = "OpenFolderPickerToolStripMenuItem"
        Me.OpenFolderPickerToolStripMenuItem.Size = New System.Drawing.Size(264, 34)
        Me.OpenFolderPickerToolStripMenuItem.Text = "Open Folder Picker"
        '
        'SVNCleanupToolStripMenuItem
        '
        Me.SVNCleanupToolStripMenuItem.Name = "SVNCleanupToolStripMenuItem"
        Me.SVNCleanupToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.SVNCleanupToolStripMenuItem.Text = "SVN Cleanup"
        '
        'CopyToClipboardToolStripMenuItem
        '
        Me.CopyToClipboardToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyFileNameToolStripMenuItem, Me.CopyActiveFilesParentFolderToolStripMenuItem, Me.CopyFullPathToolStripMenuItem, Me.CopySvnUrlToolStripMenuItem, Me.ShareWithColleagueToolStripMenuItem})
        Me.CopyToClipboardToolStripMenuItem.Name = "CopyToClipboardToolStripMenuItem"
        Me.CopyToClipboardToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.CopyToClipboardToolStripMenuItem.Text = "Copy ... to Clipboard"
        '
        'CopyFileNameToolStripMenuItem
        '
        Me.CopyFileNameToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyFileNameWithDependentsToolStripMenuItem})
        Me.CopyFileNameToolStripMenuItem.Name = "CopyFileNameToolStripMenuItem"
        Me.CopyFileNameToolStripMenuItem.Size = New System.Drawing.Size(280, 34)
        Me.CopyFileNameToolStripMenuItem.Text = "File Name"
        '
        'CopyFileNameWithDependentsToolStripMenuItem
        '
        Me.CopyFileNameWithDependentsToolStripMenuItem.Name = "CopyFileNameWithDependentsToolStripMenuItem"
        Me.CopyFileNameWithDependentsToolStripMenuItem.Size = New System.Drawing.Size(252, 34)
        Me.CopyFileNameWithDependentsToolStripMenuItem.Text = "With Dependents"
        '
        'CopyActiveFilesParentFolderToolStripMenuItem
        '
        Me.CopyActiveFilesParentFolderToolStripMenuItem.Name = "CopyActiveFilesParentFolderToolStripMenuItem"
        Me.CopyActiveFilesParentFolderToolStripMenuItem.Size = New System.Drawing.Size(280, 34)
        Me.CopyActiveFilesParentFolderToolStripMenuItem.Text = "Parent Folder"
        '
        'CopyFullPathToolStripMenuItem
        '
        Me.CopyFullPathToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyFilesPathsWithDependentsToolStripMenuItem})
        Me.CopyFullPathToolStripMenuItem.Name = "CopyFullPathToolStripMenuItem"
        Me.CopyFullPathToolStripMenuItem.Size = New System.Drawing.Size(280, 34)
        Me.CopyFullPathToolStripMenuItem.Text = "Full Path"
        '
        'CopyFilesPathsWithDependentsToolStripMenuItem
        '
        Me.CopyFilesPathsWithDependentsToolStripMenuItem.Name = "CopyFilesPathsWithDependentsToolStripMenuItem"
        Me.CopyFilesPathsWithDependentsToolStripMenuItem.Size = New System.Drawing.Size(252, 34)
        Me.CopyFilesPathsWithDependentsToolStripMenuItem.Text = "With Dependents"
        '
        'CopySvnUrlToolStripMenuItem
        '
        Me.CopySvnUrlToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopySvnUrlWithDependentsToolStripMenuItem})
        Me.CopySvnUrlToolStripMenuItem.Name = "CopySvnUrlToolStripMenuItem"
        Me.CopySvnUrlToolStripMenuItem.Size = New System.Drawing.Size(280, 34)
        Me.CopySvnUrlToolStripMenuItem.Text = "SVN URL"
        '
        'CopySvnUrlWithDependentsToolStripMenuItem
        '
        Me.CopySvnUrlWithDependentsToolStripMenuItem.Name = "CopySvnUrlWithDependentsToolStripMenuItem"
        Me.CopySvnUrlWithDependentsToolStripMenuItem.Size = New System.Drawing.Size(252, 34)
        Me.CopySvnUrlWithDependentsToolStripMenuItem.Text = "With Dependents"
        '
        'ShareWithColleagueToolStripMenuItem
        '
        Me.ShareWithColleagueToolStripMenuItem.Name = "ShareWithColleagueToolStripMenuItem"
        Me.ShareWithColleagueToolStripMenuItem.Size = New System.Drawing.Size(280, 34)
        Me.ShareWithColleagueToolStripMenuItem.Text = "Sharing File Message"
        '
        'SearchFileNameOnlineToolStripMenuItem
        '
        Me.SearchFileNameOnlineToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GoogleToolStripMenuItem, Me.McToolStripMenuItem, Me.DigikeyToolStripMenuItem})
        Me.SearchFileNameOnlineToolStripMenuItem.Name = "SearchFileNameOnlineToolStripMenuItem"
        Me.SearchFileNameOnlineToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.SearchFileNameOnlineToolStripMenuItem.Text = "Search File Name Online"
        '
        'GoogleToolStripMenuItem
        '
        Me.GoogleToolStripMenuItem.Name = "GoogleToolStripMenuItem"
        Me.GoogleToolStripMenuItem.Size = New System.Drawing.Size(231, 34)
        Me.GoogleToolStripMenuItem.Text = "Google"
        '
        'McToolStripMenuItem
        '
        Me.McToolStripMenuItem.Name = "McToolStripMenuItem"
        Me.McToolStripMenuItem.Size = New System.Drawing.Size(231, 34)
        Me.McToolStripMenuItem.Text = "McMaster-Carr"
        '
        'DigikeyToolStripMenuItem
        '
        Me.DigikeyToolStripMenuItem.Name = "DigikeyToolStripMenuItem"
        Me.DigikeyToolStripMenuItem.Size = New System.Drawing.Size(231, 34)
        Me.DigikeyToolStripMenuItem.Text = "Digikey"
        '
        'OpenFileFromURLToolStripMenuItem
        '
        Me.OpenFileFromURLToolStripMenuItem.Name = "OpenFileFromURLToolStripMenuItem"
        Me.OpenFileFromURLToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.OpenFileFromURLToolStripMenuItem.Text = "Open File From URL"
        Me.OpenFileFromURLToolStripMenuItem.Visible = False
        '
        'CreateSvnFilelistToolStripMenuItem
        '
        Me.CreateSvnFilelistToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CreateSvnFilelistWithDependentsToolStripMenuItem})
        Me.CreateSvnFilelistToolStripMenuItem.Name = "CreateSvnFilelistToolStripMenuItem"
        Me.CreateSvnFilelistToolStripMenuItem.Size = New System.Drawing.Size(305, 34)
        Me.CreateSvnFilelistToolStripMenuItem.Text = "Create SVN FileList"
        Me.CreateSvnFilelistToolStripMenuItem.Visible = False
        '
        'CreateSvnFilelistWithDependentsToolStripMenuItem
        '
        Me.CreateSvnFilelistWithDependentsToolStripMenuItem.Name = "CreateSvnFilelistWithDependentsToolStripMenuItem"
        Me.CreateSvnFilelistWithDependentsToolStripMenuItem.Size = New System.Drawing.Size(252, 34)
        Me.CreateSvnFilelistWithDependentsToolStripMenuItem.Text = "With Dependents"
        '
        'ToolStripDropDownButGetLocks
        '
        Me.ToolStripDropDownButGetLocks.DropDownButtonWidth = 50
        Me.ToolStripDropDownButGetLocks.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLocksWithDependents})
        Me.ToolStripDropDownButGetLocks.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButGetLocks.Image = Global.PlumVault.My.Resources.Resources.GetLocksIconOnly
        Me.ToolStripDropDownButGetLocks.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLocks.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButGetLocks.Name = "ToolStripDropDownButGetLocks"
        Me.ToolStripDropDownButGetLocks.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButGetLocks.Size = New System.Drawing.Size(174, 75)
        Me.ToolStripDropDownButGetLocks.Text = "Get Locks"
        Me.ToolStripDropDownButGetLocks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownGetLocksWithDependents
        '
        Me.dropDownGetLocksWithDependents.Name = "dropDownGetLocksWithDependents"
        Me.dropDownGetLocksWithDependents.Size = New System.Drawing.Size(230, 34)
        Me.dropDownGetLocksWithDependents.Text = "With Dependents"
        '
        'ToolStripDropDownButCommit
        '
        Me.ToolStripDropDownButCommit.DropDownButtonWidth = 50
        Me.ToolStripDropDownButCommit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownCommitWithDependents, Me.dropDownCommitAll})
        Me.ToolStripDropDownButCommit.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButCommit.Image = Global.PlumVault.My.Resources.Resources.Commit_Icon_Only
        Me.ToolStripDropDownButCommit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButCommit.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButCommit.Name = "ToolStripDropDownButCommit"
        Me.ToolStripDropDownButCommit.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButCommit.Size = New System.Drawing.Size(174, 75)
        Me.ToolStripDropDownButCommit.Tag = "butTagCommit"
        Me.ToolStripDropDownButCommit.Text = "Commit"
        Me.ToolStripDropDownButCommit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.ToolStripDropDownButCommit.ToolTipText = "Commit"
        '
        'dropDownCommitWithDependents
        '
        Me.dropDownCommitWithDependents.Name = "dropDownCommitWithDependents"
        Me.dropDownCommitWithDependents.Size = New System.Drawing.Size(230, 34)
        Me.dropDownCommitWithDependents.Text = "With Dependents"
        '
        'dropDownCommitAll
        '
        Me.dropDownCommitAll.Name = "dropDownCommitAll"
        Me.dropDownCommitAll.Size = New System.Drawing.Size(230, 34)
        Me.dropDownCommitAll.Text = "All"
        '
        'ToolStripDropDownButUnlock
        '
        Me.ToolStripDropDownButUnlock.DropDownButtonWidth = 50
        Me.ToolStripDropDownButUnlock.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownUnlockWithDependents, Me.dropDownUnlockAll})
        Me.ToolStripDropDownButUnlock.Font = New System.Drawing.Font("Segoe UI", 7.5!)
        Me.ToolStripDropDownButUnlock.Image = Global.PlumVault.My.Resources.Resources.unlockIconOnly1
        Me.ToolStripDropDownButUnlock.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButUnlock.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButUnlock.Name = "ToolStripDropDownButUnlock"
        Me.ToolStripDropDownButUnlock.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButUnlock.Size = New System.Drawing.Size(174, 74)
        Me.ToolStripDropDownButUnlock.Text = "Unlock && Revert"
        Me.ToolStripDropDownButUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownUnlockWithDependents
        '
        Me.dropDownUnlockWithDependents.Name = "dropDownUnlockWithDependents"
        Me.dropDownUnlockWithDependents.Size = New System.Drawing.Size(223, 34)
        Me.dropDownUnlockWithDependents.Text = "With Dependents"
        '
        'dropDownUnlockAll
        '
        Me.dropDownUnlockAll.Name = "dropDownUnlockAll"
        Me.dropDownUnlockAll.Size = New System.Drawing.Size(223, 34)
        Me.dropDownUnlockAll.Text = "All"
        '
        'ToolStripDropDownButGetLatest
        '
        Me.ToolStripDropDownButGetLatest.DropDownButtonWidth = 50
        Me.ToolStripDropDownButGetLatest.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLatestAllOpenFiles, Me.dropDownGetLatestAll})
        Me.ToolStripDropDownButGetLatest.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButGetLatest.Image = Global.PlumVault.My.Resources.Resources.GetLatestIconOnly
        Me.ToolStripDropDownButGetLatest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLatest.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButGetLatest.Name = "ToolStripDropDownButGetLatest"
        Me.ToolStripDropDownButGetLatest.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButGetLatest.Size = New System.Drawing.Size(174, 75)
        Me.ToolStripDropDownButGetLatest.Text = "Get Latest"
        Me.ToolStripDropDownButGetLatest.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownGetLatestAllOpenFiles
        '
        Me.dropDownGetLatestAllOpenFiles.Name = "dropDownGetLatestAllOpenFiles"
        Me.dropDownGetLatestAllOpenFiles.Size = New System.Drawing.Size(205, 34)
        Me.dropDownGetLatestAllOpenFiles.Text = "All Open Files"
        '
        'dropDownGetLatestAll
        '
        Me.dropDownGetLatestAll.Name = "dropDownGetLatestAll"
        Me.dropDownGetLatestAll.Size = New System.Drawing.Size(205, 34)
        Me.dropDownGetLatestAll.Text = "All"
        '
        'ToolStripDropDownButReleases
        '
        Me.ToolStripDropDownButReleases.DropDownButtonWidth = 50
        Me.ToolStripDropDownButReleases.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ApproveReleaseToolStripMenuItem, Me.EditNewRevisionToolStripMenuItem})
        Me.ToolStripDropDownButReleases.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButReleases.Image = Global.PlumVault.My.Resources.Resources.Released2IconOnly
        Me.ToolStripDropDownButReleases.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButReleases.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButReleases.Name = "ToolStripDropDownButReleases"
        Me.ToolStripDropDownButReleases.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButReleases.Size = New System.Drawing.Size(174, 75)
        Me.ToolStripDropDownButReleases.Text = "Releases"
        Me.ToolStripDropDownButReleases.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'ApproveReleaseToolStripMenuItem
        '
        Me.ApproveReleaseToolStripMenuItem.Name = "ApproveReleaseToolStripMenuItem"
        Me.ApproveReleaseToolStripMenuItem.Size = New System.Drawing.Size(264, 34)
        Me.ApproveReleaseToolStripMenuItem.Text = "RELEASE and Approve"
        '
        'EditNewRevisionToolStripMenuItem
        '
        Me.EditNewRevisionToolStripMenuItem.Name = "EditNewRevisionToolStripMenuItem"
        Me.EditNewRevisionToolStripMenuItem.Size = New System.Drawing.Size(264, 34)
        Me.EditNewRevisionToolStripMenuItem.Text = "EDIT New Revision"
        '
        'ContentPanel
        '
        Me.ContentPanel.Size = New System.Drawing.Size(250, 525)
        '
        'versionLabel
        '
        Me.versionLabel.AutoSize = True
        Me.versionLabel.Location = New System.Drawing.Point(251, 112)
        Me.versionLabel.Name = "versionLabel"
        Me.versionLabel.Size = New System.Drawing.Size(121, 20)
        Me.versionLabel.TabIndex = 24
        Me.versionLabel.Text = "Version number"
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(144.0!, 144.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.versionLabel)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.butFindComponent)
        Me.Controls.Add(Me.butRefresh)
        Me.Controls.Add(Me.butPickFolder)
        Me.Controls.Add(Me.localRepoPath)
        Me.Controls.Add(Me.onlineCheckBox)
        Me.Controls.Add(Me.butCleanup)
        Me.Controls.Add(Me.TreeView1)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(569, 952)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TreeView1 As Windows.Forms.TreeView
    Friend WithEvents butCleanup As Windows.Forms.Button
    Friend WithEvents onlineCheckBox As Windows.Forms.CheckBox
    Friend WithEvents localRepoPath As Windows.Forms.TextBox
    Friend WithEvents butPickFolder As Windows.Forms.Button
    Friend WithEvents butRefresh As Windows.Forms.Button
    Friend WithEvents butFindComponent As Button
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripDropDownButGetLocks As ToolStripSplitButton
    Friend WithEvents dropDownGetLocksWithDependents As ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButCommit As ToolStripSplitButton
    Friend WithEvents dropDownCommitWithDependents As ToolStripMenuItem
    Friend WithEvents dropDownCommitAll As ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButUnlock As ToolStripSplitButton
    Friend WithEvents dropDownUnlockWithDependents As ToolStripMenuItem
    Friend WithEvents dropDownUnlockAll As ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButGetLatest As ToolStripSplitButton
    Friend WithEvents dropDownGetLatestAllOpenFiles As ToolStripMenuItem
    Friend WithEvents dropDownGetLatestAll As ToolStripMenuItem
    Friend WithEvents BottomToolStripPanel As ToolStripPanel
    Friend WithEvents TopToolStripPanel As ToolStripPanel
    Friend WithEvents RightToolStripPanel As ToolStripPanel
    Friend WithEvents LeftToolStripPanel As ToolStripPanel
    Friend WithEvents ContentPanel As ToolStripContentPanel
    Friend WithEvents ContextMenu1 As ContextMenu
    Friend WithEvents ContextMenu2 As ContextMenu
    Friend WithEvents versionLabel As Label
    Friend WithEvents ToolStripDropDownButReleases As ToolStripSplitButton
    Friend WithEvents ApproveReleaseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditNewRevisionToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSplitButFolder As ToolStripSplitButton
    Friend WithEvents CreateSvnFilelistToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CreateSvnFilelistWithDependentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PickSVNFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SVNCleanupToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyToClipboardToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyFileNameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyActiveFilesParentFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyFullPathToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopySvnUrlToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShareWithColleagueToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SearchFileNameOnlineToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileFromURLToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyFileNameWithDependentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyFilesPathsWithDependentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GoogleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents McToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DigikeyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFolderPickerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopySvnUrlWithDependentsToolStripMenuItem As ToolStripMenuItem
End Class
