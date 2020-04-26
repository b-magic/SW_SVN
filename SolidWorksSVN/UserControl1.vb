Imports System.Runtime.InteropServices
Imports SolidWorks.Interop.sldworks
Imports swAddin

<ProgId("SVN_AddIn")>
Public Class UserControl1
    Dim swApp As SldWorks
    Friend Sub getSwApp(ByRef swAppin As Object)
        swApp = swAppin
    End Sub
    Private Sub butCheckinWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckinWithDependents.Click
        SwAddin.myCheckinWithDependents()
    End Sub

    Private Sub butCheckinAll_Click(sender As Object, e As EventArgs) Handles butCheckinAll.Click

    End Sub

    Private Sub butUnlockActive_Click(sender As Object, e As EventArgs) Handles butUnlockActive.Click

    End Sub

    Private Sub butUnlockWithDependents_Click(sender As Object, e As EventArgs) Handles butUnlockWithDependents.Click

    End Sub

    Private Sub butCheckoutActiveDoc_Click(sender As Object, e As EventArgs) Handles butCheckoutActiveDoc.Click

    End Sub

    Private Sub butCheckoutWithDependents_Click(sender As Object, e As EventArgs) Handles butCheckoutWithDependents.Click

    End Sub

    Private Sub butGetLatestOpenOnly_Click(sender As Object, e As EventArgs) Handles butGetLatestOpenOnly.Click

    End Sub

    Private Sub butGetLatestAllRepo_Click(sender As Object, e As EventArgs) Handles butGetLatestAllRepo.Click

    End Sub
End Class
