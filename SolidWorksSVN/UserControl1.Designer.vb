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
        Me.butCheckinWithDependents = New System.Windows.Forms.Button()
        Me.butCheckinAll = New System.Windows.Forms.Button()
        Me.butUnlockActive = New System.Windows.Forms.Button()
        Me.butUnlockWithDependents = New System.Windows.Forms.Button()
        Me.butCheckoutActiveDoc = New System.Windows.Forms.Button()
        Me.butCheckoutWithDependents = New System.Windows.Forms.Button()
        Me.butGetLatestOpenOnly = New System.Windows.Forms.Button()
        Me.butGetLatestAllRepo = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'butCheckinWithDependents
        '
        Me.butCheckinWithDependents.Location = New System.Drawing.Point(3, 106)
        Me.butCheckinWithDependents.Name = "butCheckinWithDependents"
        Me.butCheckinWithDependents.Size = New System.Drawing.Size(74, 41)
        Me.butCheckinWithDependents.TabIndex = 0
        Me.butCheckinWithDependents.Text = "Checkin" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butCheckinWithDependents.UseVisualStyleBackColor = True
        '
        'butCheckinAll
        '
        Me.butCheckinAll.Location = New System.Drawing.Point(3, 153)
        Me.butCheckinAll.Name = "butCheckinAll"
        Me.butCheckinAll.Size = New System.Drawing.Size(74, 41)
        Me.butCheckinAll.TabIndex = 1
        Me.butCheckinAll.Text = "Checkin" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All"
        Me.butCheckinAll.UseVisualStyleBackColor = True
        '
        'butUnlockActive
        '
        Me.butUnlockActive.Location = New System.Drawing.Point(83, 106)
        Me.butUnlockActive.Name = "butUnlockActive"
        Me.butUnlockActive.Size = New System.Drawing.Size(74, 41)
        Me.butUnlockActive.TabIndex = 2
        Me.butUnlockActive.Text = "Release" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butUnlockActive.UseVisualStyleBackColor = True
        '
        'butUnlockWithDependents
        '
        Me.butUnlockWithDependents.Location = New System.Drawing.Point(83, 153)
        Me.butUnlockWithDependents.Name = "butUnlockWithDependents"
        Me.butUnlockWithDependents.Size = New System.Drawing.Size(74, 41)
        Me.butUnlockWithDependents.TabIndex = 3
        Me.butUnlockWithDependents.Text = "Release" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All"
        Me.butUnlockWithDependents.UseVisualStyleBackColor = True
        '
        'butCheckoutActiveDoc
        '
        Me.butCheckoutActiveDoc.Location = New System.Drawing.Point(163, 106)
        Me.butCheckoutActiveDoc.Name = "butCheckoutActiveDoc"
        Me.butCheckoutActiveDoc.Size = New System.Drawing.Size(74, 41)
        Me.butCheckoutActiveDoc.TabIndex = 4
        Me.butCheckoutActiveDoc.Text = "Checkout" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Active"
        Me.butCheckoutActiveDoc.UseVisualStyleBackColor = True
        '
        'butCheckoutWithDependents
        '
        Me.butCheckoutWithDependents.Location = New System.Drawing.Point(163, 153)
        Me.butCheckoutWithDependents.Name = "butCheckoutWithDependents"
        Me.butCheckoutWithDependents.Size = New System.Drawing.Size(74, 41)
        Me.butCheckoutWithDependents.TabIndex = 5
        Me.butCheckoutWithDependents.Text = "Checkout" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "+Dependents"
        Me.butCheckoutWithDependents.UseVisualStyleBackColor = True
        '
        'butGetLatestOpenOnly
        '
        Me.butGetLatestOpenOnly.Location = New System.Drawing.Point(243, 106)
        Me.butGetLatestOpenOnly.Name = "butGetLatestOpenOnly"
        Me.butGetLatestOpenOnly.Size = New System.Drawing.Size(74, 41)
        Me.butGetLatestOpenOnly.TabIndex = 6
        Me.butGetLatestOpenOnly.Text = "Get Latest" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All Open"
        Me.butGetLatestOpenOnly.UseVisualStyleBackColor = True
        '
        'butGetLatestAllRepo
        '
        Me.butGetLatestAllRepo.Location = New System.Drawing.Point(243, 153)
        Me.butGetLatestAllRepo.Name = "butGetLatestAllRepo"
        Me.butGetLatestAllRepo.Size = New System.Drawing.Size(74, 41)
        Me.butGetLatestAllRepo.TabIndex = 7
        Me.butGetLatestAllRepo.Text = "Get Latest" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "All Repo"
        Me.butGetLatestAllRepo.UseVisualStyleBackColor = True
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.butGetLatestAllRepo)
        Me.Controls.Add(Me.butGetLatestOpenOnly)
        Me.Controls.Add(Me.butCheckoutWithDependents)
        Me.Controls.Add(Me.butCheckoutActiveDoc)
        Me.Controls.Add(Me.butUnlockWithDependents)
        Me.Controls.Add(Me.butUnlockActive)
        Me.Controls.Add(Me.butCheckinAll)
        Me.Controls.Add(Me.butCheckinWithDependents)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(345, 521)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents butCheckinWithDependents As Windows.Forms.Button
    Friend WithEvents butCheckinAll As Windows.Forms.Button
    Friend WithEvents butUnlockActive As Windows.Forms.Button
    Friend WithEvents butUnlockWithDependents As Windows.Forms.Button
    Friend WithEvents butCheckoutActiveDoc As Windows.Forms.Button
    Friend WithEvents butCheckoutWithDependents As Windows.Forms.Button
    Friend WithEvents butGetLatestOpenOnly As Windows.Forms.Button
    Friend WithEvents butGetLatestAllRepo As Windows.Forms.Button
End Class
