Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst


Module swModelFunctions

    Dim iSwApp As SldWorks

    Friend Sub initializeSwModelFunctions(mySwAppPass As SldWorks)
        iSwApp = mySwAppPass
    End Sub

    Public Function getFilePathsFromModDocArr(modDocArr() As ModelDoc2) As String()
        If IsNothing(modDocArr) Then Return Nothing
        Dim i, j As Integer
        j = 0
        Dim getFilePathsArr(modDocArr.Length - 1) As String
        For i = 0 To modDocArr.Length - 1
            If modDocArr(i) Is Nothing Then Continue For
            Try
                getFilePathsArr(i - j) = modDocArr(i).GetPathName()
            Catch
                j += 1
            End Try
        Next
        If j > 0 Then
            If i = j Then Return Nothing
            ReDim Preserve getFilePathsArr(UBound(getFilePathsArr) - j)
        End If

        Return (getFilePathsArr)
    End Function

    Public Function getAllOpenDocs(ByRef bMustBeVisible As Boolean) As ModelDoc2()
        'bMustBeVisible ignores invisible files such as parts within an assembly

        Dim modDoc As ModelDoc2 = iSwApp.GetFirstDocument
        Dim iNumDocsOpen As Integer = iSwApp.GetDocumentCount()
        Dim modDocOutput(iNumDocsOpen - 1) As ModelDoc2
        Dim sPath(iNumDocsOpen - 1) As String
        Dim i As Integer
        Dim j As Integer = 0

        For i = 0 To iNumDocsOpen
            If modDoc Is Nothing Then
                'reached the end of the open docs somehow before iNumDocs was reached
                Exit For
            End If
            ' Assembly components are opened, but are not visible
            ' until opened by the user
            If bMustBeVisible Then
                If modDoc.Visible() Then
                    modDocOutput(j) = modDoc
                    'sPath(i) = modDoc.GetPathName()
                    j += 1
                End If
            Else
                modDocOutput(j) = modDoc
                'sPath(i) = modDoc.GetPathName()
                j += 1
            End If
            'sTitle = modDoc.GetTitle
            'sPath = modDoc.GetPathName
            modDoc = modDoc.GetNext
        Next
        ReDim Preserve modDocOutput(j - 1)
        Return modDocOutput
    End Function

    Public Sub saveAllOpenFiles(Optional bShowError As Boolean = False)

        Dim swFrame As SolidWorks.Interop.sldworks.Frame = iSwApp.Frame()
        Dim oModelWindows As Object = swFrame.ModelWindows
        'Dim modDoc As SolidWorks.Interop.sldworks.ModelDoc2

        Dim i As Integer
        Dim numSaved As Integer = 0
        Dim numFailed As Integer = 0

        Dim errors As Integer
        Dim warnings As Integer

        Dim modDocArr As ModelDoc2() = getAllOpenDocs(bMustBeVisible:=False)

        For i = 0 To UBound(modDocArr)

            If modDocArr(i) Is Nothing Then Continue For

            If modDocArr(i).GetSaveFlag() And (Not modDocArr(i).IsOpenedReadOnly) Then
                If False = modDocArr(i).Save3(0, errors, warnings) Then
                    Debug.Print("Save error: " & modDocArr(i).GetTitle & " Error:" & errors & " Warning:" & warnings)
                    numFailed += numFailed
                Else
                    numSaved += numSaved
                End If
            End If
        Next

        swFrame.SetStatusBarText(numSaved & " Files Saved. & " & numFailed & " Files Failed to Save.")

    End Sub
    Public Sub save3AndShowErrorMessages(ByRef modDocArr() As ModelDoc2)
        Dim bSuccess As Boolean
        Dim swErrors As Long
        Dim swWarnings As Long
        Dim i As Integer
        Dim sErrorFiles As String = ""

        For i = 0 To modDocArr.Length - 1
            If modDocArr(i) Is Nothing Then Continue For
            bSuccess = modDocArr(i).Save3(1, swErrors, swWarnings)
            If Not bSuccess Then
                sErrorFiles &= modDocArr(i).GetPathName & vbCrLf & " Errors: " & swErrors & vbCrLf &
                    "Warnings: " & swWarnings & vbCrLf
            End If
        Next

        If sErrorFiles <> "" Then
            iSwApp.SendMsgToUser("Error could not save the following file. For Error Codes, google swFileSaveError_e " &
                     "and for Warnings google swFileSaveWarning_e " & vbCrLf &
                     sErrorFiles)
            Exit Sub
        End If
    End Sub

End Module
