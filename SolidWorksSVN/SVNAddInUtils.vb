Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst

Public Module svnAddInUtils
    Public Function createBoolArray(ByRef iUbound As Integer, ByRef value As Boolean) As Boolean()
        Dim i As Integer
        Dim output(iUbound) As Boolean
        For i = 0 To iUbound
            output(i) = value
        Next
        Return output
    End Function
    Public Function catWithNewLine(stringArr() As String) As String
        Dim i As Integer
        Dim output As String = ""
        If stringArr Is Nothing Then Return ""
        For i = 0 To UBound(stringArr)
            If stringArr(i) Is Nothing Then Continue For
            output &= vbCrLf & stringArr(i)
        Next
        Return output
    End Function
    Public Function findIndexContains(ByVal sLookInArr() As String, ByVal find As String) As Integer
        Dim i As Integer
        'Dim output As Integer
        For i = 0 To UBound(sLookInArr)
            'If sLookInArr(i).Contains(find) Then Return i
            If (Strings.InStr(sLookInArr(i), find, CompareMethod.Text) <> 0) Then Return i
        Next
        Return -1
    End Function
    Public Sub DeleteTreeViewAt(ByVal index As Integer, ByRef prLst As TreeView())
        Dim i As Integer

        ' Move all element back one position
        For i = index + 1 To UBound(prLst)
            prLst(i - 1) = prLst(i)
        Next

        ' Shrink the array by one, removing the last one
        ReDim Preserve prLst(UBound(prLst) - 1)
    End Sub
    Public Function GetSolidworksCustomProperty(doc As ModelDoc2, propName As String) As String
        Dim valOut As String = ""
        Dim resolvedVal As String = ""
        'Dim wasResolved As Boolean
        Dim found As Boolean

        Dim custMgr As CustomPropertyManager = doc.Extension.CustomPropertyManager("")
        found = custMgr.Get4(propName, False, valOut, resolvedVal)

        If found Then
            Return resolvedVal
        Else
            Return ""
        End If
    End Function

    Public Sub SetSolidworksCustomProperty(doc As ModelDoc2, propName As String, propValue As String)
        Dim custMgr As CustomPropertyManager = doc.Extension.CustomPropertyManager("")
        custMgr.Add3(propName, swCustomInfoType_e.swCustomInfoText, propValue, swCustomPropertyAddOption_e.swCustomPropertyReplaceValue)
    End Sub

    Public Function ensureUserHasLocks(modDocArr() As ModelDoc2, Optional bRetry As Boolean = True) As Boolean()
        ' TODO: 1. Fix functions that expect single boolean output. 2. move the ensure not nothing of each element of the array to parent functions
        Dim j As Integer = 0

        For j = 0 To UBound(modDocArr)

        Next

        Dim mySVNStatus = getFileSVNStatus(bCheckServer:=False, modDocArr)
        'Dim modDocArr_noNothing() As ModelDoc2 = RemoveNullsFromArray(modDocArr)

        Dim userHasLock(modDocArr.Length - 1) As Boolean
        Dim modsNeedingLocks As New List(Of ModelDoc2)

        For i As Integer = 0 To modDocArr.Length - 1
            If modDocArr(i) Is Nothing Then
                userHasLock(i) = Nothing
                Continue For
            End If

            If mySVNStatus.fp(i).lock6 = "K" Then
                userHasLock(i) = True
            Else
                userHasLock(i) = False
                modsNeedingLocks.Add(modDocArr(i))
            End If
        Next

        If modsNeedingLocks.Count = 0 Then
            ' User has all the locks!
            Return userHasLock
        ElseIf bRetry Then
            'Didn't have all the locks, but First time, so try to get them. 
            ' #TODO Would be better to use runSvnByArgs instead of using tortoise
            getLocksOfDocs(modsNeedingLocks.ToArray())
            ' Check again!
            Return ensureUserHasLocks(modDocArr, bRetry:=False)
        Else
            ' don't have all the locks, and have already tried once to get them. 
            Return userHasLock
        End If
    End Function
    Public Function RemoveNullsFromArray(Of T)(inputArray() As T) As T()
        If inputArray Is Nothing Then Return New T() {}
        Return inputArray.Where(Function(x) x IsNot Nothing).ToArray()
    End Function

    Public Function checkNoLocks(modDocArr() As ModelDoc2) As Boolean
        Dim mySVNStatus = getFileSVNStatus(bCheckServer:=False, modDocArr)
        Dim userHasLock(modDocArr.Length - 1) As Boolean

        For i As Integer = 0 To modDocArr.Length - 1
            If mySVNStatus.fp(i).lock6 = "K" Then
                Return False
            End If
        Next

        Return True

    End Function
    Public Function getMatchingDrawingForArray(modDocArr As ModelDoc2(), iSwApp As SldWorks) As ModelDoc2()
        Dim outputList As New List(Of ModelDoc2)(modDocArr)

        For Each modDoc In modDocArr
            Dim result As ModelDoc2() = getMatchingComponentAndDrawing(modDoc, iSwApp)
            If result.Length >= 2 AndAlso result(1) IsNot Nothing Then
                outputList.Add(result(1))
            End If
        Next

        Return outputList.ToArray()
    End Function

    Public Function getMatchingComponentAndDrawing(modDoc As ModelDoc2, iSwApp As SldWorks) As ModelDoc2()
        Dim modDocPath As String = modDoc.GetPathName()
        If String.IsNullOrWhiteSpace(modDocPath) Then Return Nothing
        Dim folder As String = Path.GetDirectoryName(modDocPath)
        Dim baseName As String = Path.GetFileNameWithoutExtension(modDocPath)
        Dim extension As String = Path.GetExtension(modDocPath).ToUpperInvariant()
        Dim result(1) As ModelDoc2

        'Important: Part/Assemble is always in position 0. Drawing always in position 1. 

        ' Check if it's a Part or Assembly
        If extension = ".SLDPRT" OrElse extension = ".SLDASM" Then
            result(0) = modDoc

            ' Look for matching drawing
            Dim drwPath As String = Path.Combine(folder, baseName & ".SLDDRW")
            If File.Exists(drwPath) Then
                result(1) = CType(iSwApp.OpenDoc6(drwPath, swDocumentTypes_e.swDocDRAWING, swOpenDocOptions_e.swOpenDocOptions_Silent, "", 0, 0), ModelDoc2)
                Dim myPageSetup As PageSetup = CType(result(1).PageSetup, PageSetup)
                myPageSetup.PrinterPaperSize = 17
            End If

        ElseIf extension = ".SLDDRW" Then
            result(1) = modDoc

            ' Try .SLDPRT first
            Dim prtPath As String = Path.Combine(folder, baseName & ".SLDPRT")
            If File.Exists(prtPath) Then
                result(0) = CType(iSwApp.OpenDoc6(prtPath, swDocumentTypes_e.swDocPART, swOpenDocOptions_e.swOpenDocOptions_Silent, "", 0, 0), ModelDoc2)
            Else
                ' Try .SLDASM
                Dim asmPath As String = Path.Combine(folder, baseName & ".SLDASM")
                If File.Exists(asmPath) Then
                    result(0) = CType(iSwApp.OpenDoc6(asmPath, swDocumentTypes_e.swDocASSEMBLY, swOpenDocOptions_e.swOpenDocOptions_Silent, "", 0, 0), ModelDoc2)
                End If
            End If

            Dim myPageSetup As PageSetup = CType(result(1).PageSetup, PageSetup)
            myPageSetup.PrinterPaperSize = 17

        Else
            iSwApp.SendMsgToUser("Error. Not a part, assembly, or drawing. Exiting.")
            Return Nothing ' Not a part, assembly, or drawing
        End If
        Return result   'Important: Part/Assemble is always in position 0. Drawing always in position 1. 
    End Function
    Public Function FilterModelDocs(modDocArr As ModelDoc2()) As ModelDoc2()
        ' Create and show the form
        Dim filterForm As New ModelDocFilterForm(modDocArr)
        If filterForm.ShowDialog() = DialogResult.OK Then
            Return filterForm.FilteredDocs
        Else
            Return modDocArr ' Return original if user cancels
        End If
    End Function

    Public Class ModelDocFilterForm
        Inherits Form

        Private checkedListBox As New CheckedListBox()
        Private okButton As New Button()
        Private docList As New List(Of ModelDoc2)

        Private _filteredDocs As ModelDoc2()

        Public ReadOnly Property FilteredDocs As ModelDoc2()
            Get
                Return _filteredDocs
            End Get
        End Property

        Public Sub New(modDocArr As ModelDoc2())
            Me.Text = "Select Files"
            Me.Size = New Drawing.Size(500, 400)
            Me.StartPosition = FormStartPosition.CenterScreen

            checkedListBox.Dock = DockStyle.Fill
            checkedListBox.CheckOnClick = True

            For Each doc As ModelDoc2 In modDocArr
                Dim fileName As String = IO.Path.GetFileName(doc.GetPathName())
                checkedListBox.Items.Add(fileName, True)
                docList.Add(doc)
            Next

            okButton.Text = "OK"
            okButton.Dock = DockStyle.Bottom
            AddHandler okButton.Click, AddressOf OkButton_Click

            Me.Controls.Add(checkedListBox)
            Me.Controls.Add(okButton)
        End Sub

        Private Sub OkButton_Click(sender As Object, e As EventArgs)
            Dim selectedDocs As New List(Of ModelDoc2)

            For i As Integer = 0 To checkedListBox.Items.Count - 1
                If checkedListBox.GetItemChecked(i) Then
                    selectedDocs.Add(docList(i))
                End If
            Next

            _filteredDocs = selectedDocs.ToArray()
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub
    End Class


End Module
