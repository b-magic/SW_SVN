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

End Module
