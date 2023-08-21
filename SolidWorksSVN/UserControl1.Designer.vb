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
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.inclParentAssemblyCheckBox = New System.Windows.Forms.CheckBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.StatusStrip2 = New System.Windows.Forms.StatusStrip()
        Me.butFindComponent = New System.Windows.Forms.Button()
        Me.ToolStrip1.SuspendLayout()
        Me.ToolStripContainer1.LeftToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.Location = New System.Drawing.Point(3, 741)
        Me.TreeView1.MinimumSize = New System.Drawing.Size(478, 194)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(670, 1245)
        Me.TreeView1.TabIndex = 10
        '
        'onlineCheckBox
        '
        Me.onlineCheckBox.AutoSize = True
        Me.onlineCheckBox.Checked = True
        Me.onlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.onlineCheckBox.Location = New System.Drawing.Point(4, 663)
        Me.onlineCheckBox.Name = "onlineCheckBox"
        Me.onlineCheckBox.Size = New System.Drawing.Size(80, 24)
        Me.onlineCheckBox.TabIndex = 14
        Me.onlineCheckBox.Text = "Online"
        Me.onlineCheckBox.UseVisualStyleBackColor = True
        '
        'localRepoPath
        '
        Me.localRepoPath.Location = New System.Drawing.Point(3, 615)
        Me.localRepoPath.Name = "localRepoPath"
        Me.localRepoPath.Size = New System.Drawing.Size(506, 26)
        Me.localRepoPath.TabIndex = 15
        Me.localRepoPath.Text = "Enter Path to Local Repository"
        '
        'butPickFolder
        '
        Me.butPickFolder.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butPickFolder.Location = New System.Drawing.Point(104, 646)
        Me.butPickFolder.Name = "butPickFolder"
        Me.butPickFolder.Size = New System.Drawing.Size(165, 38)
        Me.butPickFolder.TabIndex = 16
        Me.butPickFolder.Text = "Pick Folder"
        Me.butPickFolder.UseVisualStyleBackColor = True
        '
        'butRefresh
        '
        Me.butRefresh.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butRefresh.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butRefresh.Location = New System.Drawing.Point(10, 690)
        Me.butRefresh.Name = "butRefresh"
        Me.butRefresh.Size = New System.Drawing.Size(136, 44)
        Me.butRefresh.TabIndex = 17
        Me.butRefresh.Text = "Refresh"
        Me.butRefresh.UseVisualStyleBackColor = True
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(36, 50)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButGetLocks, Me.ToolStripDropDownButCommit, Me.ToolStripDropDownButUnlock, Me.ToolStripDropDownButGetLatest})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.MaximumSize = New System.Drawing.Size(400, 800)
        Me.ToolStrip1.MinimumSize = New System.Drawing.Size(200, 400)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(200, 525)
        Me.ToolStrip1.Stretch = True
        Me.ToolStrip1.TabIndex = 0
        '
        'ToolStripDropDownButGetLocks
        '
        Me.ToolStripDropDownButGetLocks.DropDownButtonWidth = 40
        Me.ToolStripDropDownButGetLocks.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLocksWithDependents})
        Me.ToolStripDropDownButGetLocks.Image = Global.SolidWorksSVN.My.Resources.Resources.GetLocksIconOnly
        Me.ToolStripDropDownButGetLocks.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLocks.Name = "ToolStripDropDownButGetLocks"
        Me.ToolStripDropDownButGetLocks.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripDropDownButGetLocks.Size = New System.Drawing.Size(191, 79)
        Me.ToolStripDropDownButGetLocks.Text = "Get Locks"
        Me.ToolStripDropDownButGetLocks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownGetLocksWithDependents
        '
        Me.dropDownGetLocksWithDependents.Name = "dropDownGetLocksWithDependents"
        Me.dropDownGetLocksWithDependents.Size = New System.Drawing.Size(252, 34)
        Me.dropDownGetLocksWithDependents.Text = "With Dependents"
        '
        'ToolStripDropDownButCommit
        '
        Me.ToolStripDropDownButCommit.DropDownButtonWidth = 40
        Me.ToolStripDropDownButCommit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownCommitWithDependents, Me.dropDownCommitAll})
        Me.ToolStripDropDownButCommit.Image = Global.SolidWorksSVN.My.Resources.Resources.Commit_Icon_Only
        Me.ToolStripDropDownButCommit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButCommit.Name = "ToolStripDropDownButCommit"
        Me.ToolStripDropDownButCommit.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripDropDownButCommit.Size = New System.Drawing.Size(191, 79)
        Me.ToolStripDropDownButCommit.Tag = "butTagCommit"
        Me.ToolStripDropDownButCommit.Text = "Commit"
        Me.ToolStripDropDownButCommit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.ToolStripDropDownButCommit.ToolTipText = "Commit"
        '
        'dropDownCommitWithDependents
        '
        Me.dropDownCommitWithDependents.Name = "dropDownCommitWithDependents"
        Me.dropDownCommitWithDependents.Size = New System.Drawing.Size(252, 34)
        Me.dropDownCommitWithDependents.Text = "With Dependents"
        '
        'dropDownCommitAll
        '
        Me.dropDownCommitAll.Name = "dropDownCommitAll"
        Me.dropDownCommitAll.Size = New System.Drawing.Size(252, 34)
        Me.dropDownCommitAll.Text = "All"
        '
        'ToolStripDropDownButUnlock
        '
        Me.ToolStripDropDownButUnlock.DropDownButtonWidth = 40
        Me.ToolStripDropDownButUnlock.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownUnlockWithDependents, Me.dropDownUnlockAll})
        Me.ToolStripDropDownButUnlock.Image = Global.SolidWorksSVN.My.Resources.Resources.unlockIconOnly1
        Me.ToolStripDropDownButUnlock.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButUnlock.Name = "ToolStripDropDownButUnlock"
        Me.ToolStripDropDownButUnlock.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripDropDownButUnlock.Size = New System.Drawing.Size(191, 79)
        Me.ToolStripDropDownButUnlock.Text = "Unlock && Revert"
        Me.ToolStripDropDownButUnlock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownUnlockWithDependents
        '
        Me.dropDownUnlockWithDependents.Name = "dropDownUnlockWithDependents"
        Me.dropDownUnlockWithDependents.Size = New System.Drawing.Size(252, 34)
        Me.dropDownUnlockWithDependents.Text = "With Dependents"
        '
        'dropDownUnlockAll
        '
        Me.dropDownUnlockAll.Name = "dropDownUnlockAll"
        Me.dropDownUnlockAll.Size = New System.Drawing.Size(252, 34)
        Me.dropDownUnlockAll.Text = "All"
        '
        'ToolStripDropDownButGetLatest
        '
        Me.ToolStripDropDownButGetLatest.DropDownButtonWidth = 40
        Me.ToolStripDropDownButGetLatest.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.dropDownGetLatestAllOpenFiles, Me.dropDownGetLatestAll})
        Me.ToolStripDropDownButGetLatest.Image = Global.SolidWorksSVN.My.Resources.Resources.GetLatestIconOnly
        Me.ToolStripDropDownButGetLatest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButGetLatest.Name = "ToolStripDropDownButGetLatest"
        Me.ToolStripDropDownButGetLatest.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripDropDownButGetLatest.Size = New System.Drawing.Size(191, 79)
        Me.ToolStripDropDownButGetLatest.Text = "Get Latest"
        Me.ToolStripDropDownButGetLatest.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'dropDownGetLatestAllOpenFiles
        '
        Me.dropDownGetLatestAllOpenFiles.Name = "dropDownGetLatestAllOpenFiles"
        Me.dropDownGetLatestAllOpenFiles.Size = New System.Drawing.Size(222, 34)
        Me.dropDownGetLatestAllOpenFiles.Text = "All Open Files"
        '
        'dropDownGetLatestAll
        '
        Me.dropDownGetLatestAll.Name = "dropDownGetLatestAll"
        Me.dropDownGetLatestAll.Size = New System.Drawing.Size(222, 34)
        Me.dropDownGetLatestAll.Text = "All"
        '
        'ToolStripContainer1
        '
        Me.ToolStripContainer1.BottomToolStripPanelVisible = False
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(250, 525)
        '
        'ToolStripContainer1.LeftToolStripPanel
        '
        Me.ToolStripContainer1.LeftToolStripPanel.Controls.Add(Me.ToolStrip1)
        Me.ToolStripContainer1.Location = New System.Drawing.Point(10, 3)
        Me.ToolStripContainer1.MaximumSize = New System.Drawing.Size(474, 600)
        Me.ToolStripContainer1.MinimumSize = New System.Drawing.Size(280, 460)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        Me.ToolStripContainer1.Size = New System.Drawing.Size(450, 525)
        Me.ToolStripContainer1.TabIndex = 18
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        Me.ToolStripContainer1.TopToolStripPanelVisible = False
        '
        'butCleanup
        '
        Me.butCleanup.BackColor = System.Drawing.SystemColors.ControlLight
        Me.butCleanup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCleanup.Cursor = System.Windows.Forms.Cursors.Hand
        Me.butCleanup.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butCleanup.Location = New System.Drawing.Point(152, 691)
        Me.butCleanup.Name = "butCleanup"
        Me.butCleanup.Size = New System.Drawing.Size(128, 44)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.Text = "Clean Up"
        Me.butCleanup.UseVisualStyleBackColor = False
        '
        'inclParentAssemblyCheckBox
        '
        Me.inclParentAssemblyCheckBox.AutoSize = True
        Me.inclParentAssemblyCheckBox.Location = New System.Drawing.Point(3, 568)
        Me.inclParentAssemblyCheckBox.Name = "inclParentAssemblyCheckBox"
        Me.inclParentAssemblyCheckBox.Size = New System.Drawing.Size(210, 24)
        Me.inclParentAssemblyCheckBox.TabIndex = 21
        Me.inclParentAssemblyCheckBox.Text = "Include Parent Assembly"
        Me.inclParentAssemblyCheckBox.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(501, 220)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(38, 26)
        Me.TextBox1.TabIndex = 22
        Me.TextBox1.Text = "2023.08.21-D"
        '
        'StatusStrip2
        '
        Me.StatusStrip2.Dock = System.Windows.Forms.DockStyle.None
        Me.StatusStrip2.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip2.Location = New System.Drawing.Point(4, 532)
        Me.StatusStrip2.Name = "StatusStrip2"
        Me.StatusStrip2.Padding = New System.Windows.Forms.Padding(2, 0, 21, 0)
        Me.StatusStrip2.Size = New System.Drawing.Size(202, 22)
        Me.StatusStrip2.TabIndex = 0
        Me.StatusStrip2.Text = "StatusStrip1"
        '
        'butFindComponent
        '
        Me.butFindComponent.Enabled = False
        Me.butFindComponent.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.butFindComponent.Location = New System.Drawing.Point(410, 691)
        Me.butFindComponent.Name = "butFindComponent"
        Me.butFindComponent.Size = New System.Drawing.Size(89, 44)
        Me.butFindComponent.TabIndex = 23
        Me.butFindComponent.Text = "Find"
        Me.butFindComponent.UseVisualStyleBackColor = True
        Me.butFindComponent.Visible = False
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(144.0!, 144.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.butFindComponent)
        Me.Controls.Add(Me.StatusStrip2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.inclParentAssemblyCheckBox)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Controls.Add(Me.butRefresh)
        Me.Controls.Add(Me.butPickFolder)
        Me.Controls.Add(Me.localRepoPath)
        Me.Controls.Add(Me.onlineCheckBox)
        Me.Controls.Add(Me.butCleanup)
        Me.Controls.Add(Me.TreeView1)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(542, 993)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ToolStripContainer1.LeftToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.LeftToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TreeView1 As Windows.Forms.TreeView
    Friend WithEvents butCleanup As Windows.Forms.Button
    Friend WithEvents onlineCheckBox As Windows.Forms.CheckBox
    Friend WithEvents localRepoPath As Windows.Forms.TextBox
    Friend WithEvents butPickFolder As Windows.Forms.Button
    Friend WithEvents butRefresh As Windows.Forms.Button
    Friend WithEvents ToolStrip1 As Windows.Forms.ToolStrip
    Friend WithEvents ToolStripContainer1 As Windows.Forms.ToolStripContainer
    Friend WithEvents ToolStripDropDownButGetLocks As Windows.Forms.ToolStripSplitButton
    Friend WithEvents dropDownGetLocksWithDependents As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButCommit As Windows.Forms.ToolStripSplitButton
    Friend WithEvents dropDownCommitWithDependents As Windows.Forms.ToolStripMenuItem
    Friend WithEvents dropDownCommitAll As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButUnlock As Windows.Forms.ToolStripSplitButton
    Friend WithEvents dropDownUnlockWithDependents As Windows.Forms.ToolStripMenuItem
    Friend WithEvents dropDownUnlockAll As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripDropDownButGetLatest As Windows.Forms.ToolStripSplitButton
    Friend WithEvents dropDownGetLatestAllOpenFiles As Windows.Forms.ToolStripMenuItem
    Friend WithEvents dropDownGetLatestAll As Windows.Forms.ToolStripMenuItem
    Friend WithEvents inclParentAssemblyCheckBox As CheckBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents StatusStrip2 As StatusStrip
    Friend WithEvents butFindComponent As Button
End Class
