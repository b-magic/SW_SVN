<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UserControl1
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.StatusStrip2 = New System.Windows.Forms.StatusStrip()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.onlineCheckBox = New System.Windows.Forms.CheckBox()
        Me.localRepoPath = New System.Windows.Forms.TextBox()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.butPickFolder = New System.Windows.Forms.Button()
        Me.butStatus = New System.Windows.Forms.Button()
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.butGetLatestAllRepo = New System.Windows.Forms.Button()
        Me.butGetLatestOpenOnly = New System.Windows.Forms.Button()
        Me.butGetLockWithDependents = New System.Windows.Forms.Button()
        Me.butGetLockActiveDoc = New System.Windows.Forms.Button()
        Me.butUnlockAll = New System.Windows.Forms.Button()
        Me.butUnlockActive = New System.Windows.Forms.Button()
        Me.butCheckinAll = New System.Windows.Forms.Button()
        Me.butCheckinWithDependents = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'StatusStrip2
        '
        Me.StatusStrip2.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.StatusStrip2.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.StatusStrip2.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip2.Location = New System.Drawing.Point(0, 780)
        Me.StatusStrip2.Name = "StatusStrip2"
        Me.StatusStrip2.Padding = New System.Windows.Forms.Padding(0, 0, 10, 0)
        Me.StatusStrip2.Size = New System.Drawing.Size(483, 22)
        Me.StatusStrip2.SizingGrip = False
        Me.StatusStrip2.TabIndex = 9
        Me.StatusStrip2.Text = "StatusStrip2"
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.Location = New System.Drawing.Point(4, 488)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(481, 295)
        Me.TreeView1.TabIndex = 10
        '
        'onlineCheckBox
        '
        Me.onlineCheckBox.AutoSize = True
        Me.onlineCheckBox.Checked = True
        Me.onlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.onlineCheckBox.Location = New System.Drawing.Point(4, 375)
        Me.onlineCheckBox.Name = "onlineCheckBox"
        Me.onlineCheckBox.Size = New System.Drawing.Size(80, 24)
        Me.onlineCheckBox.TabIndex = 14
        Me.onlineCheckBox.Text = "Online"
        Me.onlineCheckBox.UseVisualStyleBackColor = True
        '
        'localRepoPath
        '
        Me.localRepoPath.Location = New System.Drawing.Point(4, 331)
        Me.localRepoPath.Name = "localRepoPath"
        Me.localRepoPath.Size = New System.Drawing.Size(478, 26)
        Me.localRepoPath.TabIndex = 15
        Me.localRepoPath.Text = "Enter Path to Local Repository"
        '
        'butPickFolder
        '
        Me.butPickFolder.Location = New System.Drawing.Point(88, 372)
        Me.butPickFolder.Name = "butPickFolder"
        Me.butPickFolder.Size = New System.Drawing.Size(129, 34)
        Me.butPickFolder.TabIndex = 16
        Me.butPickFolder.Text = "Pick Folder"
        Me.butPickFolder.UseVisualStyleBackColor = True
        '
        'butStatus
        '
        Me.butStatus.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.butStatus.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.Status
        Me.butStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butStatus.Enabled = False
        Me.butStatus.Location = New System.Drawing.Point(135, 412)
        Me.butStatus.Name = "butStatus"
        Me.butStatus.Size = New System.Drawing.Size(120, 62)
        Me.butStatus.TabIndex = 13
        Me.butStatus.UseVisualStyleBackColor = False
        '
        'butCleanup
        '
        Me.butCleanup.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.butCleanup.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.Cleanup
        Me.butCleanup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCleanup.Location = New System.Drawing.Point(4, 412)
        Me.butCleanup.Name = "butCleanup"
        Me.butCleanup.Size = New System.Drawing.Size(120, 62)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.UseVisualStyleBackColor = False
        '
        'butGetLatestAllRepo
        '
        Me.butGetLatestAllRepo.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLatestAllRepo1
        Me.butGetLatestAllRepo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLatestAllRepo.Location = New System.Drawing.Point(135, 235)
        Me.butGetLatestAllRepo.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butGetLatestAllRepo.Name = "butGetLatestAllRepo"
        Me.butGetLatestAllRepo.Size = New System.Drawing.Size(120, 62)
        Me.butGetLatestAllRepo.TabIndex = 7
        Me.butGetLatestAllRepo.UseVisualStyleBackColor = True
        '
        'butGetLatestOpenOnly
        '
        Me.butGetLatestOpenOnly.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLatestAllOpen1
        Me.butGetLatestOpenOnly.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLatestOpenOnly.Location = New System.Drawing.Point(4, 235)
        Me.butGetLatestOpenOnly.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butGetLatestOpenOnly.Name = "butGetLatestOpenOnly"
        Me.butGetLatestOpenOnly.Size = New System.Drawing.Size(120, 62)
        Me.butGetLatestOpenOnly.TabIndex = 6
        Me.butGetLatestOpenOnly.UseVisualStyleBackColor = True
        '
        'butGetLockWithDependents
        '
        Me.butGetLockWithDependents.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLocksPlusDependents
        Me.butGetLockWithDependents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLockWithDependents.Location = New System.Drawing.Point(135, 163)
        Me.butGetLockWithDependents.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butGetLockWithDependents.Name = "butGetLockWithDependents"
        Me.butGetLockWithDependents.Size = New System.Drawing.Size(120, 62)
        Me.butGetLockWithDependents.TabIndex = 5
        Me.butGetLockWithDependents.UseVisualStyleBackColor = True
        '
        'butGetLockActiveDoc
        '
        Me.butGetLockActiveDoc.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLocks
        Me.butGetLockActiveDoc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLockActiveDoc.Location = New System.Drawing.Point(4, 163)
        Me.butGetLockActiveDoc.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butGetLockActiveDoc.Name = "butGetLockActiveDoc"
        Me.butGetLockActiveDoc.Size = New System.Drawing.Size(120, 62)
        Me.butGetLockActiveDoc.TabIndex = 4
        Me.butGetLockActiveDoc.UseVisualStyleBackColor = True
        '
        'butUnlockAll
        '
        Me.butUnlockAll.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.ReleaseAll
        Me.butUnlockAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butUnlockAll.Location = New System.Drawing.Point(135, 91)
        Me.butUnlockAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butUnlockAll.Name = "butUnlockAll"
        Me.butUnlockAll.Size = New System.Drawing.Size(120, 62)
        Me.butUnlockAll.TabIndex = 3
        Me.butUnlockAll.UseVisualStyleBackColor = True
        '
        'butUnlockActive
        '
        Me.butUnlockActive.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.ReleaseActive
        Me.butUnlockActive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butUnlockActive.Location = New System.Drawing.Point(4, 91)
        Me.butUnlockActive.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butUnlockActive.Name = "butUnlockActive"
        Me.butUnlockActive.Size = New System.Drawing.Size(120, 62)
        Me.butUnlockActive.TabIndex = 2
        Me.butUnlockActive.UseVisualStyleBackColor = True
        '
        'butCheckinAll
        '
        Me.butCheckinAll.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.CommitAll
        Me.butCheckinAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckinAll.Location = New System.Drawing.Point(135, 17)
        Me.butCheckinAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butCheckinAll.Name = "butCheckinAll"
        Me.butCheckinAll.Size = New System.Drawing.Size(120, 62)
        Me.butCheckinAll.TabIndex = 1
        Me.butCheckinAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.butCheckinAll.UseVisualStyleBackColor = True
        '
        'butCheckinWithDependents
        '
        Me.butCheckinWithDependents.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.Commit
        Me.butCheckinWithDependents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckinWithDependents.Location = New System.Drawing.Point(4, 17)
        Me.butCheckinWithDependents.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.butCheckinWithDependents.Name = "butCheckinWithDependents"
        Me.butCheckinWithDependents.Size = New System.Drawing.Size(120, 62)
        Me.butCheckinWithDependents.TabIndex = 0
        Me.butCheckinWithDependents.UseVisualStyleBackColor = True
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.butPickFolder)
        Me.Controls.Add(Me.localRepoPath)
        Me.Controls.Add(Me.onlineCheckBox)
        Me.Controls.Add(Me.butStatus)
        Me.Controls.Add(Me.butCleanup)
        Me.Controls.Add(Me.TreeView1)
        Me.Controls.Add(Me.StatusStrip2)
        Me.Controls.Add(Me.butGetLatestAllRepo)
        Me.Controls.Add(Me.butGetLatestOpenOnly)
        Me.Controls.Add(Me.butGetLockWithDependents)
        Me.Controls.Add(Me.butGetLockActiveDoc)
        Me.Controls.Add(Me.butUnlockAll)
        Me.Controls.Add(Me.butUnlockActive)
        Me.Controls.Add(Me.butCheckinAll)
        Me.Controls.Add(Me.butCheckinWithDependents)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(483, 802)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents butCheckinWithDependents As Windows.Forms.Button
    Friend WithEvents butCheckinAll As Windows.Forms.Button
    Friend WithEvents butUnlockActive As Windows.Forms.Button
    Friend WithEvents butUnlockAll As Windows.Forms.Button
    Friend WithEvents butGetLockActiveDoc As Windows.Forms.Button
    Friend WithEvents butGetLockWithDependents As Windows.Forms.Button
    Friend WithEvents butGetLatestOpenOnly As Windows.Forms.Button
    Friend WithEvents butGetLatestAllRepo As Windows.Forms.Button
    Friend WithEvents StatusStrip2 As Windows.Forms.StatusStrip
    Friend WithEvents TreeView1 As Windows.Forms.TreeView
    Friend WithEvents butCleanup As Windows.Forms.Button
    Friend WithEvents butStatus As Windows.Forms.Button
    Friend WithEvents onlineCheckBox As Windows.Forms.CheckBox
    Friend WithEvents localRepoPath As Windows.Forms.TextBox
    Friend WithEvents FolderBrowserDialog1 As Windows.Forms.FolderBrowserDialog
    Friend WithEvents butPickFolder As Windows.Forms.Button
End Class
