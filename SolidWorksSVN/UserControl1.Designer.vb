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
        Me.butStatus = New System.Windows.Forms.Button()
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.butGetLatestAllRepo = New System.Windows.Forms.Button()
        Me.butGetLatestOpenOnly = New System.Windows.Forms.Button()
        Me.butCheckoutWithDependents = New System.Windows.Forms.Button()
        Me.butCheckoutActiveDoc = New System.Windows.Forms.Button()
        Me.butUnlockAll = New System.Windows.Forms.Button()
        Me.butUnlockActive = New System.Windows.Forms.Button()
        Me.butCheckinAll = New System.Windows.Forms.Button()
        Me.butCheckinWithDependents = New System.Windows.Forms.Button()
        Me.onlineCheckBox = New System.Windows.Forms.CheckBox()
        Me.localRepoPath = New System.Windows.Forms.TextBox()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.butPickFolder = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'StatusStrip2
        '
        Me.StatusStrip2.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.StatusStrip2.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.StatusStrip2.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip2.Location = New System.Drawing.Point(0, 980)
        Me.StatusStrip2.Name = "StatusStrip2"
        Me.StatusStrip2.Size = New System.Drawing.Size(645, 22)
        Me.StatusStrip2.SizingGrip = False
        Me.StatusStrip2.TabIndex = 9
        Me.StatusStrip2.Text = "StatusStrip2"
        '
        'TreeView1
        '
        Me.TreeView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeView1.Location = New System.Drawing.Point(6, 609)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(639, 368)
        Me.TreeView1.TabIndex = 10
        '
        'butStatus
        '
        Me.butStatus.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.butStatus.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.Status
        Me.butStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butStatus.Location = New System.Drawing.Point(166, 515)
        Me.butStatus.Name = "butStatus"
        Me.butStatus.Size = New System.Drawing.Size(148, 79)
        Me.butStatus.TabIndex = 13
        Me.butStatus.UseVisualStyleBackColor = False
        '
        'butCleanup
        '
        Me.butCleanup.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.butCleanup.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.Cleanup
        Me.butCleanup.Location = New System.Drawing.Point(6, 515)
        Me.butCleanup.Name = "butCleanup"
        Me.butCleanup.Size = New System.Drawing.Size(148, 79)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.UseVisualStyleBackColor = False
        '
        'butGetLatestAllRepo
        '
        Me.butGetLatestAllRepo.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLatestAllRepo1
        Me.butGetLatestAllRepo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLatestAllRepo.Location = New System.Drawing.Point(166, 295)
        Me.butGetLatestAllRepo.Margin = New System.Windows.Forms.Padding(6)
        Me.butGetLatestAllRepo.Name = "butGetLatestAllRepo"
        Me.butGetLatestAllRepo.Size = New System.Drawing.Size(148, 79)
        Me.butGetLatestAllRepo.TabIndex = 7
        Me.butGetLatestAllRepo.UseVisualStyleBackColor = True
        '
        'butGetLatestOpenOnly
        '
        Me.butGetLatestOpenOnly.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.GetLatestAllOpen1
        Me.butGetLatestOpenOnly.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGetLatestOpenOnly.Location = New System.Drawing.Point(6, 295)
        Me.butGetLatestOpenOnly.Margin = New System.Windows.Forms.Padding(6)
        Me.butGetLatestOpenOnly.Name = "butGetLatestOpenOnly"
        Me.butGetLatestOpenOnly.Size = New System.Drawing.Size(148, 79)
        Me.butGetLatestOpenOnly.TabIndex = 6
        Me.butGetLatestOpenOnly.UseVisualStyleBackColor = True
        '
        'butCheckoutWithDependents
        '
        Me.butCheckoutWithDependents.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.CheckOutWithDependents
        Me.butCheckoutWithDependents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckoutWithDependents.Location = New System.Drawing.Point(166, 204)
        Me.butCheckoutWithDependents.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckoutWithDependents.Name = "butCheckoutWithDependents"
        Me.butCheckoutWithDependents.Size = New System.Drawing.Size(148, 79)
        Me.butCheckoutWithDependents.TabIndex = 5
        Me.butCheckoutWithDependents.UseVisualStyleBackColor = True
        '
        'butCheckoutActiveDoc
        '
        Me.butCheckoutActiveDoc.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.CheckOutActive
        Me.butCheckoutActiveDoc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckoutActiveDoc.Location = New System.Drawing.Point(6, 204)
        Me.butCheckoutActiveDoc.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckoutActiveDoc.Name = "butCheckoutActiveDoc"
        Me.butCheckoutActiveDoc.Size = New System.Drawing.Size(148, 79)
        Me.butCheckoutActiveDoc.TabIndex = 4
        Me.butCheckoutActiveDoc.UseVisualStyleBackColor = True
        '
        'butUnlockAll
        '
        Me.butUnlockAll.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.ReleaseAll
        Me.butUnlockAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butUnlockAll.Location = New System.Drawing.Point(166, 113)
        Me.butUnlockAll.Margin = New System.Windows.Forms.Padding(6)
        Me.butUnlockAll.Name = "butUnlockAll"
        Me.butUnlockAll.Size = New System.Drawing.Size(148, 79)
        Me.butUnlockAll.TabIndex = 3
        Me.butUnlockAll.UseVisualStyleBackColor = True
        '
        'butUnlockActive
        '
        Me.butUnlockActive.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.ReleaseActive
        Me.butUnlockActive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butUnlockActive.Location = New System.Drawing.Point(6, 113)
        Me.butUnlockActive.Margin = New System.Windows.Forms.Padding(6)
        Me.butUnlockActive.Name = "butUnlockActive"
        Me.butUnlockActive.Size = New System.Drawing.Size(148, 79)
        Me.butUnlockActive.TabIndex = 2
        Me.butUnlockActive.UseVisualStyleBackColor = True
        '
        'butCheckinAll
        '
        Me.butCheckinAll.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.CheckInAll
        Me.butCheckinAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckinAll.Location = New System.Drawing.Point(166, 22)
        Me.butCheckinAll.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckinAll.Name = "butCheckinAll"
        Me.butCheckinAll.Size = New System.Drawing.Size(148, 79)
        Me.butCheckinAll.TabIndex = 1
        Me.butCheckinAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.butCheckinAll.UseVisualStyleBackColor = True
        '
        'butCheckinWithDependents
        '
        Me.butCheckinWithDependents.BackgroundImage = Global.SolidWorksSVN.My.Resources.Resources.CheckInActive
        Me.butCheckinWithDependents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butCheckinWithDependents.Location = New System.Drawing.Point(6, 22)
        Me.butCheckinWithDependents.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckinWithDependents.Name = "butCheckinWithDependents"
        Me.butCheckinWithDependents.Size = New System.Drawing.Size(148, 79)
        Me.butCheckinWithDependents.TabIndex = 0
        Me.butCheckinWithDependents.UseVisualStyleBackColor = True
        '
        'onlineCheckBox
        '
        Me.onlineCheckBox.AutoSize = True
        Me.onlineCheckBox.Checked = True
        Me.onlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.onlineCheckBox.Location = New System.Drawing.Point(6, 469)
        Me.onlineCheckBox.Name = "onlineCheckBox"
        Me.onlineCheckBox.Size = New System.Drawing.Size(106, 29)
        Me.onlineCheckBox.TabIndex = 14
        Me.onlineCheckBox.Text = "Online"
        Me.onlineCheckBox.UseVisualStyleBackColor = True
        '
        'localRepoPath
        '
        Me.localRepoPath.Location = New System.Drawing.Point(6, 432)
        Me.localRepoPath.Name = "localRepoPath"
        Me.localRepoPath.Size = New System.Drawing.Size(636, 31)
        Me.localRepoPath.TabIndex = 15
        Me.localRepoPath.Text = "Enter Path to Local Repository"
        '
        'butPickFolder
        '
        Me.butPickFolder.Location = New System.Drawing.Point(118, 469)
        Me.butPickFolder.Name = "butPickFolder"
        Me.butPickFolder.Size = New System.Drawing.Size(173, 34)
        Me.butPickFolder.TabIndex = 16
        Me.butPickFolder.Text = "Pick Folder"
        Me.butPickFolder.UseVisualStyleBackColor = True
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
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
        Me.Controls.Add(Me.butCheckoutWithDependents)
        Me.Controls.Add(Me.butCheckoutActiveDoc)
        Me.Controls.Add(Me.butUnlockAll)
        Me.Controls.Add(Me.butUnlockActive)
        Me.Controls.Add(Me.butCheckinAll)
        Me.Controls.Add(Me.butCheckinWithDependents)
        Me.Margin = New System.Windows.Forms.Padding(6)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(645, 1002)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents butCheckinWithDependents As Windows.Forms.Button
    Friend WithEvents butCheckinAll As Windows.Forms.Button
    Friend WithEvents butUnlockActive As Windows.Forms.Button
    Friend WithEvents butUnlockAll As Windows.Forms.Button
    Friend WithEvents butCheckoutActiveDoc As Windows.Forms.Button
    Friend WithEvents butCheckoutWithDependents As Windows.Forms.Button
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
