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
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.onlineCheckBox = New System.Windows.Forms.CheckBox()
        Me.localRepoPath = New System.Windows.Forms.TextBox()
        Me.butPickFolder = New System.Windows.Forms.Button()
        Me.butRefresh = New System.Windows.Forms.Button()
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.butFindComponent = New System.Windows.Forms.Button()
        Me.BottomToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.TopToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.RightToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.LeftToolStripPanel = New System.Windows.Forms.ToolStripPanel()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
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
        Me.ContentPanel = New System.Windows.Forms.ToolStripContentPanel()
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
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(538, 577)
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
        Me.butPickFolder.Size = New System.Drawing.Size(126, 38)
        Me.butPickFolder.TabIndex = 16
        Me.butPickFolder.Text = "Pick Folder"
        Me.butPickFolder.UseVisualStyleBackColor = True
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
        Me.butCleanup.Size = New System.Drawing.Size(156, 38)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.Text = "SVN Clean Up"
        Me.butCleanup.UseVisualStyleBackColor = False
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(305, 86)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(97, 26)
        Me.TextBox1.TabIndex = 22
        Me.TextBox1.Text = "2024.04.12-A"
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
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButGetLocks, Me.ToolStripDropDownButCommit, Me.ToolStripDropDownButUnlock, Me.ToolStripDropDownButGetLatest})
        Me.ToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow
        Me.ToolStrip1.Location = New System.Drawing.Point(4, 86)
        Me.ToolStrip1.MaximumSize = New System.Drawing.Size(400, 800)
        Me.ToolStrip1.MinimumSize = New System.Drawing.Size(100, 200)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(181, 347)
        Me.ToolStrip1.Stretch = True
        Me.ToolStrip1.TabIndex = 0
        '
        'ToolStripDropDownButGetLocks
        '
        Me.ToolStripDropDownButGetLocks.DropDownButtonWidth = 40
        Me.ToolStripDropDownButGetLocks.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLocksWithDependents})
        Me.ToolStripDropDownButGetLocks.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButGetLocks.Image = Global.SolidWorksSVN.My.Resources.Resources.GetLocksIconOnly
        Me.ToolStripDropDownButGetLocks.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLocks.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButGetLocks.Name = "ToolStripDropDownButGetLocks"
        Me.ToolStripDropDownButGetLocks.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButGetLocks.Size = New System.Drawing.Size(172, 75)
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
        Me.ToolStripDropDownButCommit.DropDownButtonWidth = 40
        Me.ToolStripDropDownButCommit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownCommitWithDependents, Me.dropDownCommitAll})
        Me.ToolStripDropDownButCommit.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButCommit.Image = Global.SolidWorksSVN.My.Resources.Resources.Commit_Icon_Only
        Me.ToolStripDropDownButCommit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButCommit.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButCommit.Name = "ToolStripDropDownButCommit"
        Me.ToolStripDropDownButCommit.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButCommit.Size = New System.Drawing.Size(172, 75)
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
        Me.ToolStripDropDownButUnlock.DropDownButtonWidth = 40
        Me.ToolStripDropDownButUnlock.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownUnlockWithDependents, Me.dropDownUnlockAll})
        Me.ToolStripDropDownButUnlock.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButUnlock.Image = Global.SolidWorksSVN.My.Resources.Resources.unlockIconOnly1
        Me.ToolStripDropDownButUnlock.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButUnlock.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButUnlock.Name = "ToolStripDropDownButUnlock"
        Me.ToolStripDropDownButUnlock.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButUnlock.Size = New System.Drawing.Size(172, 75)
        Me.ToolStripDropDownButUnlock.Text = "Unlock && Revert"
        Me.ToolStripDropDownButUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownUnlockWithDependents
        '
        Me.dropDownUnlockWithDependents.Name = "dropDownUnlockWithDependents"
        Me.dropDownUnlockWithDependents.Size = New System.Drawing.Size(230, 34)
        Me.dropDownUnlockWithDependents.Text = "With Dependents"
        '
        'dropDownUnlockAll
        '
        Me.dropDownUnlockAll.Name = "dropDownUnlockAll"
        Me.dropDownUnlockAll.Size = New System.Drawing.Size(230, 34)
        Me.dropDownUnlockAll.Text = "All"
        '
        'ToolStripDropDownButGetLatest
        '
        Me.ToolStripDropDownButGetLatest.DropDownButtonWidth = 40
        Me.ToolStripDropDownButGetLatest.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLatestAllOpenFiles, Me.dropDownGetLatestAll})
        Me.ToolStripDropDownButGetLatest.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.ToolStripDropDownButGetLatest.Image = Global.SolidWorksSVN.My.Resources.Resources.GetLatestIconOnly
        Me.ToolStripDropDownButGetLatest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLatest.Margin = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.ToolStripDropDownButGetLatest.Name = "ToolStripDropDownButGetLatest"
        Me.ToolStripDropDownButGetLatest.Padding = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.ToolStripDropDownButGetLatest.Size = New System.Drawing.Size(172, 75)
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
        'ContentPanel
        '
        Me.ContentPanel.Size = New System.Drawing.Size(250, 525)
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(144.0!, 144.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.butFindComponent)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.butRefresh)
        Me.Controls.Add(Me.butPickFolder)
        Me.Controls.Add(Me.localRepoPath)
        Me.Controls.Add(Me.onlineCheckBox)
        Me.Controls.Add(Me.butCleanup)
        Me.Controls.Add(Me.TreeView1)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(545, 1200)
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
    Friend WithEvents TextBox1 As TextBox
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
End Class
