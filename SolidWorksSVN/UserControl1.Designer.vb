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
        Me.components = New System.ComponentModel.Container()
        Me.butCheckinWithDependents = New System.Windows.Forms.Button()
        Me.butCheckinAll = New System.Windows.Forms.Button()
        Me.butUnlockWithDependents = New System.Windows.Forms.Button()
        Me.butUnlockAll = New System.Windows.Forms.Button()
        Me.butCheckoutActiveDoc = New System.Windows.Forms.Button()
        Me.butCheckoutWithDependents = New System.Windows.Forms.Button()
        Me.butGetLatestOpenOnly = New System.Windows.Forms.Button()
        Me.butGetLatestAllRepo = New System.Windows.Forms.Button()
        Me.StatusStrip2 = New System.Windows.Forms.StatusStrip()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.butCleanup = New System.Windows.Forms.Button()
        Me.butStatus = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'butCheckinWithDependents
        '
        Me.butCheckinWithDependents.Location = New System.Drawing.Point(6, 38)
        Me.butCheckinWithDependents.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckinWithDependents.Name = "butCheckinWithDependents"
        Me.butCheckinWithDependents.Size = New System.Drawing.Size(148, 79)
        Me.butCheckinWithDependents.TabIndex = 0
        Me.butCheckinWithDependents.Text = "Checkin" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butCheckinWithDependents.UseVisualStyleBackColor = True
        '
        'butCheckinAll
        '
        Me.butCheckinAll.Location = New System.Drawing.Point(166, 38)
        Me.butCheckinAll.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckinAll.Name = "butCheckinAll"
        Me.butCheckinAll.Size = New System.Drawing.Size(148, 79)
        Me.butCheckinAll.TabIndex = 1
        Me.butCheckinAll.Text = "Checkin" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All"
        Me.butCheckinAll.UseVisualStyleBackColor = True
        '
        'butUnlockWithDependents
        '
        Me.butUnlockWithDependents.Location = New System.Drawing.Point(6, 129)
        Me.butUnlockWithDependents.Margin = New System.Windows.Forms.Padding(6)
        Me.butUnlockWithDependents.Name = "butUnlockWithDependents"
        Me.butUnlockWithDependents.Size = New System.Drawing.Size(148, 79)
        Me.butUnlockWithDependents.TabIndex = 2
        Me.butUnlockWithDependents.Text = "Release" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butUnlockWithDependents.UseVisualStyleBackColor = True
        '
        'butUnlockAll
        '
        Me.butUnlockAll.Location = New System.Drawing.Point(166, 129)
        Me.butUnlockAll.Margin = New System.Windows.Forms.Padding(6)
        Me.butUnlockAll.Name = "butUnlockAll"
        Me.butUnlockAll.Size = New System.Drawing.Size(148, 79)
        Me.butUnlockAll.TabIndex = 3
        Me.butUnlockAll.Text = "Release" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All"
        Me.butUnlockAll.UseVisualStyleBackColor = True
        '
        'butCheckoutActiveDoc
        '
        Me.butCheckoutActiveDoc.Location = New System.Drawing.Point(6, 220)
        Me.butCheckoutActiveDoc.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckoutActiveDoc.Name = "butCheckoutActiveDoc"
        Me.butCheckoutActiveDoc.Size = New System.Drawing.Size(148, 79)
        Me.butCheckoutActiveDoc.TabIndex = 4
        Me.butCheckoutActiveDoc.Text = "Checkout" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butCheckoutActiveDoc.UseVisualStyleBackColor = True
        '
        'butCheckoutWithDependents
        '
        Me.butCheckoutWithDependents.Location = New System.Drawing.Point(166, 220)
        Me.butCheckoutWithDependents.Margin = New System.Windows.Forms.Padding(6)
        Me.butCheckoutWithDependents.Name = "butCheckoutWithDependents"
        Me.butCheckoutWithDependents.Size = New System.Drawing.Size(148, 79)
        Me.butCheckoutWithDependents.TabIndex = 5
        Me.butCheckoutWithDependents.Text = "Checkout" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "+Dependents"
        Me.butCheckoutWithDependents.UseVisualStyleBackColor = True
        '
        'butGetLatestOpenOnly
        '
        Me.butGetLatestOpenOnly.Location = New System.Drawing.Point(6, 311)
        Me.butGetLatestOpenOnly.Margin = New System.Windows.Forms.Padding(6)
        Me.butGetLatestOpenOnly.Name = "butGetLatestOpenOnly"
        Me.butGetLatestOpenOnly.Size = New System.Drawing.Size(148, 79)
        Me.butGetLatestOpenOnly.TabIndex = 6
        Me.butGetLatestOpenOnly.Text = "Get Latest" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All Open"
        Me.butGetLatestOpenOnly.UseVisualStyleBackColor = True
        '
        'butGetLatestAllRepo
        '
        Me.butGetLatestAllRepo.Location = New System.Drawing.Point(166, 311)
        Me.butGetLatestAllRepo.Margin = New System.Windows.Forms.Padding(6)
        Me.butGetLatestAllRepo.Name = "butGetLatestAllRepo"
        Me.butGetLatestAllRepo.Size = New System.Drawing.Size(148, 79)
        Me.butGetLatestAllRepo.TabIndex = 7
        Me.butGetLatestAllRepo.Text = "Get Latest" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All Repo"
        Me.butGetLatestAllRepo.UseVisualStyleBackColor = True
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
        Me.TreeView1.Location = New System.Drawing.Point(6, 589)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(639, 388)
        Me.TreeView1.TabIndex = 10
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(61, 4)
        '
        'butCleanup
        '
        Me.butCleanup.Location = New System.Drawing.Point(6, 504)
        Me.butCleanup.Name = "butCleanup"
        Me.butCleanup.Size = New System.Drawing.Size(148, 79)
        Me.butCleanup.TabIndex = 12
        Me.butCleanup.Text = "Cleanup"
        Me.butCleanup.UseVisualStyleBackColor = True
        '
        'butStatus
        '
        Me.butStatus.Location = New System.Drawing.Point(166, 504)
        Me.butStatus.Name = "butStatus"
        Me.butStatus.Size = New System.Drawing.Size(148, 79)
        Me.butStatus.TabIndex = 13
        Me.butStatus.Text = "Status"
        Me.butStatus.UseVisualStyleBackColor = True
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.butStatus)
        Me.Controls.Add(Me.butCleanup)
        Me.Controls.Add(Me.TreeView1)
        Me.Controls.Add(Me.StatusStrip2)
        Me.Controls.Add(Me.butGetLatestAllRepo)
        Me.Controls.Add(Me.butGetLatestOpenOnly)
        Me.Controls.Add(Me.butCheckoutWithDependents)
        Me.Controls.Add(Me.butCheckoutActiveDoc)
        Me.Controls.Add(Me.butUnlockAll)
        Me.Controls.Add(Me.butUnlockWithDependents)
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
    Friend WithEvents butUnlockWithDependents As Windows.Forms.Button
    Friend WithEvents butUnlockAll As Windows.Forms.Button
    Friend WithEvents butCheckoutActiveDoc As Windows.Forms.Button
    Friend WithEvents butCheckoutWithDependents As Windows.Forms.Button
    Friend WithEvents butGetLatestOpenOnly As Windows.Forms.Button
    Friend WithEvents butGetLatestAllRepo As Windows.Forms.Button
    Friend WithEvents StatusStrip2 As Windows.Forms.StatusStrip
    Friend WithEvents TreeView1 As Windows.Forms.TreeView
    Friend WithEvents ContextMenuStrip1 As Windows.Forms.ContextMenuStrip
    Friend WithEvents butCleanup As Windows.Forms.Button
    Friend WithEvents butStatus As Windows.Forms.Button
End Class
