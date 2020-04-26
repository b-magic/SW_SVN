Imports System
Imports System.Collections
Imports System.Reflection
Imports System.Runtime.InteropServices

Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports SolidWorks.Interop.swpublished
Imports SolidWorksTools
Imports SolidWorksTools.File

Imports System.Collections.Generic
Imports System.Diagnostics

<Guid("ca5108e0-c3c5-47f0-8453-cd9b6a5e12af")>
<ComVisible(True)> _
<SwAddin( _
        Description:="SolidWorksSVN description", _
        Title:="SolidWorksSVN", _
        LoadAtStartup:=True _
        )> _
Public Class SwAddin
    Implements SolidWorks.Interop.swpublished.SwAddin

#Region "Local Variables"
    Dim WithEvents iSwApp As SldWorks
    Dim iCmdMgr As ICommandManager
    Dim addinID As Integer
    Dim openDocs As Hashtable
    Dim SwEventPtr As SldWorks
    Dim ppage As UserPMPage
    Dim iBmp As BitmapHandler

    Public Const mainCmdGroupID As Integer = 0

    'Update all 3 of these!
    Public iNumFlyoutButtons As Integer = 3
    Public mainItemID() As Integer = {0, 1, 2}
    Public flyoutGroupID() As Integer = {91, 92, 93}

    Public Const sTortPath As String = "C:\Users\benne\Documents\SVN\TortoiseProc.exe"
    Public Const sRepoLocalPath As String = "C:\Users\benne\Documents\SVN\fsae1"
    Public Const sSVNPath As String = "C:\Program Files\TortoiseSVN\bin\svn.exe"

    ' Public Properties
    ReadOnly Property SwApp() As SldWorks
        Get
            Return iSwApp
        End Get
    End Property

    ReadOnly Property CmdMgr() As ICommandManager
        Get
            Return iCmdMgr
        End Get
    End Property

    ReadOnly Property OpenDocumentsTable() As Hashtable
        Get
            Return openDocs
        End Get
    End Property
#End Region

#Region "SolidWorks Registration"

    <ComRegisterFunction()> Public Shared Sub RegisterFunction(ByVal t As Type)

        ' Get Custom Attribute: SwAddinAttribute
        Dim attributes() As Object
        Dim SWattr As SwAddinAttribute = Nothing

        attributes = System.Attribute.GetCustomAttributes(GetType(SwAddin), GetType(SwAddinAttribute))

        If attributes.Length > 0 Then
            SWattr = DirectCast(attributes(0), SwAddinAttribute)
        End If
        Try
            Dim hklm As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine
            Dim hkcu As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser

            Dim keyname As String = "SOFTWARE\SolidWorks\Addins\{" + t.GUID.ToString() + "}"
            Dim addinkey As Microsoft.Win32.RegistryKey = hklm.CreateSubKey(keyname)
            addinkey.SetValue(Nothing, 0)
            addinkey.SetValue("Description", SWattr.Description)
            addinkey.SetValue("Title", SWattr.Title)

            keyname = "Software\SolidWorks\AddInsStartup\{" + t.GUID.ToString() + "}"
            addinkey = hkcu.CreateSubKey(keyname)
            addinkey.SetValue(Nothing, SWattr.LoadAtStartup, Microsoft.Win32.RegistryValueKind.DWord)
        Catch nl As System.NullReferenceException
            Console.WriteLine("There was a problem registering this dll: SWattr is null.\n " & nl.Message)
            System.Windows.Forms.MessageBox.Show("There was a problem registering this dll: SWattr is null.\n" & nl.Message)
        Catch e As System.Exception
            Console.WriteLine("There was a problem registering this dll: " & e.Message)
            System.Windows.Forms.MessageBox.Show("There was a problem registering this dll: " & e.Message)
        End Try
    End Sub

    <ComUnregisterFunction()> Public Shared Sub UnregisterFunction(ByVal t As Type)
        Try
            Dim hklm As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine
            Dim hkcu As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser

            Dim keyname As String = "SOFTWARE\SolidWorks\Addins\{" + t.GUID.ToString() + "}"
            hklm.DeleteSubKey(keyname)

            keyname = "Software\SolidWorks\AddInsStartup\{" + t.GUID.ToString() + "}"
            hkcu.DeleteSubKey(keyname)
        Catch nl As System.NullReferenceException
            Console.WriteLine("There was a problem unregistering this dll: SWattr is null.\n " & nl.Message)
            System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: SWattr is null.\n" & nl.Message)
        Catch e As System.Exception
            Console.WriteLine("There was a problem unregistering this dll: " & e.Message)
            System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: " & e.Message)
        End Try

    End Sub

#End Region

#Region "ISwAddin Implementation"

    Function ConnectToSW(ByVal ThisSW As Object, ByVal Cookie As Integer) As Boolean Implements SolidWorks.Interop.swpublished.SwAddin.ConnectToSW
        iSwApp = ThisSW
        addinID = Cookie

        ' Setup callbacks
        iSwApp.SetAddinCallbackInfo(0, Me, addinID)

        ' Setup the Command Manager
        iCmdMgr = iSwApp.GetCommandManager(Cookie)
        AddCommandMgr()

        'Setup the Event Handlers
        SwEventPtr = iSwApp
        openDocs = New Hashtable
        AttachEventHandlers()

        'Setup Sample Property Manager
        AddPMP()

        ConnectToSW = True
    End Function

    Function DisconnectFromSW() As Boolean Implements SolidWorks.Interop.swpublished.SwAddin.DisconnectFromSW

        RemoveCommandMgr()
        RemovePMP()
        DetachEventHandlers()

        System.Runtime.InteropServices.Marshal.ReleaseComObject(iCmdMgr)
        iCmdMgr = Nothing
        System.Runtime.InteropServices.Marshal.ReleaseComObject(iSwApp)
        iSwApp = Nothing
        'The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers 
        GC.Collect()
        GC.WaitForPendingFinalizers()

        GC.Collect()
        GC.WaitForPendingFinalizers()

        DisconnectFromSW = True
    End Function
#End Region

#Region "UI Methods"
    Public Sub AddCommandMgr()

        Dim cmdGroup As ICommandGroup

        If iBmp Is Nothing Then
            iBmp = New BitmapHandler()
        End If

        Dim thisAssembly As Assembly

        Dim cmdIndex(3) As Integer
        'Dim cmdIndex0 As Integer, cmdIndex1 As Integer
        Dim Title As String = "VB Addin"
        Dim ToolTip As String = "VB Addin"


        Dim docTypes() As Integer = {swDocumentTypes_e.swDocASSEMBLY, _
                                       swDocumentTypes_e.swDocDRAWING, _
                                       swDocumentTypes_e.swDocPART}

        thisAssembly = System.Reflection.Assembly.GetAssembly(Me.GetType())

        Dim cmdGroupErr As Integer = 0
        Dim ignorePrevious As Boolean = False

        Dim registryIDs As Object = Nothing
        Dim getDataResult As Boolean = iCmdMgr.GetGroupDataFromRegistry(mainCmdGroupID, registryIDs)

        'Dim knownIDs As Integer() = New Integer(1) {mainItemID(0), mainItemID(1)}

        If getDataResult Then
            If Not CompareIDs(registryIDs, mainItemID) Then 'knownIDs) Then 'if the IDs don't match, reset the commandGroup
                ignorePrevious = True
            End If
        End If

        cmdGroup = iCmdMgr.CreateCommandGroup2(mainCmdGroupID, Title, ToolTip, "", -1, ignorePrevious, cmdGroupErr)
        If cmdGroup Is Nothing Or thisAssembly Is Nothing Then
            Throw New NullReferenceException()
        End If



        cmdGroup.LargeIconList = iBmp.CreateFileFromResourceBitmap("SolidWorksSVN.ToolbarLarge.bmp", thisAssembly)
        cmdGroup.SmallIconList = iBmp.CreateFileFromResourceBitmap("SolidWorksSVN.ToolbarSmall.bmp", thisAssembly)
        cmdGroup.LargeMainIcon = iBmp.CreateFileFromResourceBitmap("SolidWorksSVN.MainIconLarge.bmp", thisAssembly)
        cmdGroup.SmallMainIcon = iBmp.CreateFileFromResourceBitmap("SolidWorksSVN.MainIconSmall.bmp", thisAssembly)

        'Dim menuToolbarOption As Integer = swCommandItemType_e.swMenuItem Or swCommandItemType_e.swToolbarItem

        'cmdIndex = {0, 1, 2} 'cmdIndex0, cmdIndex1
        ''                      AddCommandItem2(Name, Position, HintString, ToolTip, ImageListIndex, CallbackFunction, EnableMethod, UserID, MenuTBOption)
        'cmdIndex(0) = cmdGroup.AddCommandItem2("CreateCube", -1, "Mouseover", "Label", 0, "CreateCube", "", mainItemID(0), menuToolbarOption)
        'cmdIndex(1) = cmdGroup.AddCommandItem2("Show PMP", -1, "Mouseover", "Label", 2, "ShowPMP", "PMPEnable", mainItemID(1), menuToolbarOption)

        'cmdGroup.HasToolbar = True
        'cmdGroup.HasMenu = True
        'cmdGroup.Activate()

        'Dim flyGroup1 As FlyoutGroup
        ''flyGroup1 = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
        ''      cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "FlyoutCallback", "FlyoutEnable")
        'flyGroup1 = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
        '      cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
        ''        AddCommandItem(Name,     Mouseover,  ImageListIndex,  CallbackFunction, UpdateCallbackFunction
        ''flyGroup1.AddCommandItem("FlyoutCommand 1", "test", 0, "FlyoutCommandItem1", "FlyoutEnable") '"FlyoutEnable") '"FlyoutDisabled
        'flyGroup1.RemoveAllCommandItems()
        'flyGroup1.AddCommandItem(System.DateTime.Now.ToLongTimeString(), "test", 0, "FlyoutCommandItem1", "FlyoutEnable")
        ''flyGroup1.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Simple
        'flyGroup1.FlyoutType = 1 'swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

        Dim flyGroupLock As FlyoutGroup
        flyGroupLock = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Lock and Checkout", "Lock/Checkout", "Lock and checkout the current document",
              cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
        flyGroupLock.RemoveAllCommandItems()
        flyGroupLock.AddCommandItem("Lock/Checkout", "Lock and checkout the current document", 0, "myCheckoutActiveDoc", "FlyoutEnable")
        flyGroupLock.FlyoutType = 1 'swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

        Dim flyGroupCheckin As FlyoutGroup
        flyGroupCheckin = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(1), "Checkin", "Checkin", "Checkin the current document",
              cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
        flyGroupCheckin.RemoveAllCommandItems()
        flyGroupCheckin.AddCommandItem("Checkin ActiveDoc", "Checkin the current document", 0, "myCheckinActiveDoc", "FlyoutEnable")
        flyGroupCheckin.AddCommandItem("Checkin All", "Checkin Any/All documents", 0, "myCheckinAll", "FlyoutEnable")
        flyGroupCheckin.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

        Dim flyGroupGetLatest As FlyoutGroup
        flyGroupGetLatest = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(2), "Get Latest", "Get Latest", "Get The Latest Files from The Vault",
              cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
        flyGroupGetLatest.RemoveAllCommandItems()
        flyGroupGetLatest.AddCommandItem("Get Latest All", "Get Latest All", 0, "myGetLatestAll", "FlyoutEnable")
        flyGroupGetLatest.AddCommandItem("Get Latest Open Files Only", "Get Latest Open Docs", 0, "myGetLatestOpenOnly", "FlyoutEnable")
        flyGroupGetLatest.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

        For Each docType As Integer In docTypes
            Dim cmdTab As ICommandTab = iCmdMgr.GetCommandTab(docType, Title)
            Dim bResult As Boolean

            If Not cmdTab Is Nothing And Not getDataResult Or ignorePrevious Then 'if tab exists, but we have ignored the registry info, re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
                Dim res As Boolean = iCmdMgr.RemoveCommandTab(cmdTab)
                cmdTab = Nothing
            End If

            If cmdTab Is Nothing Then
                cmdTab = iCmdMgr.AddCommandTab(docType, Title)

                'Dim cmdBox0 As CommandTabBox = cmdTab.AddCommandTabBox

                'Dim cmdIDs(cmdIndex.Length) As Integer
                'Dim TextType(cmdIndex.Length) As Integer

                'cmdIDs(0) = cmdGroup.CommandID(cmdIndex(0))
                'TextType(0) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal

                'cmdIDs(1) = cmdGroup.CommandID(cmdIndex(1))
                'TextType(1) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal

                'cmdIDs(2) = cmdGroup.ToolbarId
                'TextType(2) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal


                'bResult = cmdBox0.AddCommands(cmdIDs, TextType)

                Dim cmdBox1 As CommandTabBox = cmdTab.AddCommandTabBox()
                Dim cmdIDs(iNumFlyoutButtons) as Integer
                Dim TextType(iNumFlyoutButtons) As Integer

                cmdIDs(0) = flyGroupLock.CmdID
                TextType(0) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

                cmdIDs(1) = flyGroupCheckin.CmdID
                TextType(1) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

                cmdIDs(2) = flyGroupGetLatest.CmdID
                TextType(2) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

                bResult = cmdBox1.AddCommands(cmdIDs, TextType)

                cmdTab.AddSeparator(cmdBox1, cmdIDs(0))
                cmdTab.AddSeparator(cmdBox1, cmdIDs(1))

            End If
        Next

        thisAssembly = Nothing

    End Sub


    Public Sub RemoveCommandMgr()
        Try
            iBmp.Dispose()
            iCmdMgr.RemoveCommandGroup(mainCmdGroupID)
            For Each flyoutGroupID_element In flyoutGroupID
                iCmdMgr.RemoveFlyoutGroup(flyoutGroupID_element)
            Next flyoutGroupID_element
        Catch e As Exception
        End Try
    End Sub


    Function AddPMP() As Boolean
        ppage = New UserPMPage
        ppage.Init(iSwApp, Me)
    End Function

    Function RemovePMP() As Boolean
        ppage = Nothing
    End Function

    Function CompareIDs(ByVal storedIDs() As Integer, ByVal addinIDs() As Integer) As Boolean

        Dim storeList As New List(Of Integer)(storedIDs)
        Dim addinList As New List(Of Integer)(addinIDs)

        addinList.Sort()
        storeList.Sort()

        If Not addinList.Count = storeList.Count Then

            Return False
        Else

            For i As Integer = 0 To addinList.Count - 1
                If Not addinList(i) = storeList(i) Then

                    Return False
                End If
            Next
        End If

        Return True
    End Function
#End Region

#Region "Event Methods"
    Sub AttachEventHandlers()
        AttachSWEvents()

        'Listen for events on all currently open docs
        AttachEventsToAllDocuments()
    End Sub

    Sub DetachEventHandlers()
        DetachSWEvents()

        'Close events on all currently open docs
        Dim docHandler As DocumentEventHandler
        Dim key As ModelDoc2
        Dim numKeys As Integer
        numKeys = openDocs.Count
        If numKeys > 0 Then
            Dim keys() As Object = New Object(numKeys - 1) {}

            'Remove all document event handlers
            openDocs.Keys.CopyTo(keys, 0)
            For Each key In keys
                docHandler = openDocs.Item(key)
                docHandler.DetachEventHandlers() 'This also removes the pair from the hash
                docHandler = Nothing
                key = Nothing
            Next
        End If
    End Sub

    Sub AttachSWEvents()
        Try
            AddHandler iSwApp.ActiveDocChangeNotify, AddressOf Me.SldWorks_ActiveDocChangeNotify
            AddHandler iSwApp.DocumentLoadNotify2, AddressOf Me.SldWorks_DocumentLoadNotify2
            AddHandler iSwApp.FileNewNotify2, AddressOf Me.SldWorks_FileNewNotify2
            AddHandler iSwApp.ActiveModelDocChangeNotify, AddressOf Me.SldWorks_ActiveModelDocChangeNotify
            AddHandler iSwApp.FileOpenPostNotify, AddressOf Me.SldWorks_FileOpenPostNotify
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Sub

    Sub DetachSWEvents()
        Try
            RemoveHandler iSwApp.ActiveDocChangeNotify, AddressOf Me.SldWorks_ActiveDocChangeNotify
            RemoveHandler iSwApp.DocumentLoadNotify2, AddressOf Me.SldWorks_DocumentLoadNotify2
            RemoveHandler iSwApp.FileNewNotify2, AddressOf Me.SldWorks_FileNewNotify2
            RemoveHandler iSwApp.ActiveModelDocChangeNotify, AddressOf Me.SldWorks_ActiveModelDocChangeNotify
            RemoveHandler iSwApp.FileOpenPostNotify, AddressOf Me.SldWorks_FileOpenPostNotify
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Sub

    Sub AttachEventsToAllDocuments()
        Dim modDoc As ModelDoc2
        modDoc = iSwApp.GetFirstDocument()
        While Not modDoc Is Nothing
            If Not openDocs.Contains(modDoc) Then
                AttachModelDocEventHandler(modDoc)
            End If
            modDoc = modDoc.GetNext()
        End While
    End Sub

    Function AttachModelDocEventHandler(ByVal modDoc As ModelDoc2) As Boolean
        If modDoc Is Nothing Then
            Return False
        End If
        Dim docHandler As DocumentEventHandler = Nothing

        If Not openDocs.Contains(modDoc) Then
            Select Case modDoc.GetType
                Case swDocumentTypes_e.swDocPART
                    docHandler = New PartEventHandler()
                Case swDocumentTypes_e.swDocASSEMBLY
                    docHandler = New AssemblyEventHandler()
                Case swDocumentTypes_e.swDocDRAWING
                    docHandler = New DrawingEventHandler()
            End Select

            docHandler.Init(iSwApp, Me, modDoc)
            docHandler.AttachEventHandlers()
            openDocs.Add(modDoc, docHandler)
        End If
    End Function

    Sub DetachModelEventHandler(ByVal modDoc As ModelDoc2)
        Dim docHandler As DocumentEventHandler
        docHandler = openDocs.Item(modDoc)
        openDocs.Remove(modDoc)
        modDoc = Nothing
        docHandler = Nothing
    End Sub
#End Region

#Region "Event Handlers"
    Function SldWorks_ActiveDocChangeNotify() As Integer
        'TODO: Add your implementation here
    End Function

    Function SldWorks_DocumentLoadNotify2(ByVal docTitle As String, ByVal docPath As String) As Integer

    End Function

    Function SldWorks_FileNewNotify2(ByVal newDoc As Object, ByVal doctype As Integer, ByVal templateName As String) As Integer
        AttachEventsToAllDocuments()
    End Function

    Function SldWorks_ActiveModelDocChangeNotify() As Integer
        'TODO: Add your implementation here
    End Function

    Function SldWorks_FileOpenPostNotify(ByVal FileName As String) As Integer
        AttachEventsToAllDocuments()
    End Function
#End Region

#Region "UI Callbacks"
    Sub CreateCube()

        'make sure we have a part open
        Dim partTemplate As String
        Dim model As ModelDoc2
        Dim featMan As FeatureManager

        partTemplate = iSwApp.GetUserPreferenceStringValue(swUserPreferenceStringValue_e.swDefaultTemplatePart)
        If Not partTemplate = "" Then
            model = iSwApp.NewDocument(partTemplate, swDwgPaperSizes_e.swDwgPaperA2size, 0.0, 0.0)

            model.InsertSketch2(True)
            model.SketchRectangle(0, 0, 0, 0.1, 0.1, 0.1, False)

            'Extrude the sketch
            featMan = model.FeatureManager
            featMan.FeatureExtrusion(True, _
                                      False, False, _
                                      swEndConditions_e.swEndCondBlind, swEndConditions_e.swEndCondBlind, _
                                      0.1, 0.0, _
                                      False, False, _
                                      False, False, _
                                      0.0, 0.0, _
                                      False, False, _
                                      False, False, _
                                      True, _
                                      False, False)
        Else
            System.Windows.Forms.MessageBox.Show("There is no part template available. Please check your options and make sure there is a part template selected, or select a new part template.")
        End If
    End Sub
    Sub ShowPMP()
        If Not ppage Is Nothing Then
            ppage.Show()
        End If
    End Sub

    Function PMPEnable() As Integer
        If iSwApp.ActiveDoc Is Nothing Then
            PMPEnable = 0
        Else
            PMPEnable = 1
        End If
    End Function

    Sub myCheckinActiveDoc()
        Dim modDocArr1() As ModelDoc2 = {iSwApp.ActiveDoc()}
        If modDocArr1(0).GetType = swDocumentTypes_e.swDocASSEMBLY Then
            checkInDocs(getComponentsOfAssembly(modDocArr1(0)))
        Else
            checkInDocs(modDocArr1)
        End If
    End Sub

    Sub checkInDocs(ByRef modDocsArr() As ModelDoc2)

        'Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        'Dim sActiveDocPath As String = modDoc.GetPathName()
        Dim modDocArr() As ModelDoc2 = {iSwApp.ActiveDoc()}
        'Dim swErrors As Long
        'Dim swWarnings As Long
        Dim bSuccess As Boolean = False
        Dim sErrorFiles As String = ""
        Dim i As Integer
        Dim j As Integer = 0

        For i = 0 To modDocsArr.Length - 1
            If modDocArr(i).IsOpenedReadOnly() Or modDocArr(i).IsOpenedViewOnly() Then
                sErrorFiles &= modDocArr(i).GetPathName & vbCrLf
                modDocArr(i) = Nothing
                'Else
                '    'Cleans up the loop to remove empty elements
                '    modDocArr(j) = modDocArr(i)
                '    j += 1
                '    modDocArr(i) = Nothing
            End If
        Next
        If sErrorFiles <> "" Then
            iSwApp.SendMsgToUser("The following file(s) are Read-Only. You need write access to check in. " &
                                 "If you believe you have the file locked, you can try File > Reload" &
                                 sErrorFiles)
            Exit Sub
        End If

        save3AndShowErrorMessages(modDocArr)

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:" &
                                                 formatFilePathArrForTortoiseProc(
                                                    getFilePathsFromModDocArr(modDocArr)) & " /closeonend:3")

        setReadWriteFromLockStatus(modDocArr)

    End Sub

    Sub myCheckinAll()
        Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String

        Dim i As Integer
        'Dim index As Integer

        iSwApp.RunCommand(19, vbEmpty) 'Save All

        bSuccess = runTortoiseProcexeWithMonitor("/command:commit /path:""" & sRepoLocalPath & """ /closeonend:3")
        If Not bSuccess Then
            Exit Sub
        End If

        'Switch over files to read-only
        'OpenDocPathList = CType(getAllOpenDocs(True, True), String())
        Dim OpenDocModels() As ModelDoc2 = getAllOpenDocs(bMustBeVisible:=True)

        Dim sOpenDocPath() As String = getFilePathsFromModDocArr(OpenDocModels)

        'Run SVN to see status of all
        Dim mySVNStatus As SVNStatus() = getFileSVNStatus(sOpenDocPath, bCheckServer:=False)

        'If modDoc.modDoc.GetPathName is in list of all local items in vault
        ' AND does not have a correct key then set read-only

        setReadWriteFromLockStatus(OpenDocModels)

    End Sub

    Sub myCheckoutActiveDoc()
        Dim modDoc As ModelDoc2 = iSwApp.ActiveDoc()
        Dim modDocArr() As ModelDoc2 = {modDoc}
        Dim sActiveDocPath() As String = {modDoc.GetPathName()}
        Dim status As SVNStatus
        Dim bSuccess As Boolean = False

        'Using objReader As System.IO.TextReader = System.IO.File.OpenText("C:\Users\benne\AppData\Local\Temp\tmp9BF6.tmp") 'System.IO.StreamReader(sTempFileName)
        '    sLine1 = objReader.ReadLine()
        '    sLine2 = objReader.ReadLine()
        'End Using
        ''objReader.Close()
        ''My.Computer.FileSystem.DeleteFile(sTempFileName)
        'Debug.Print(sTempFileName)
        status = getFileSVNStatus(sActiveDocPath, True)(0)

        If status.errorMessage <> "" Then
            ' Some sort of error from the svn command
            If status.errorMessage.Contains("W155007:") Then
                'Common error. File not saved into repository
                iSwApp.SendMsgToUser("Status Check failed." & vbCrLf &
                        "Error: File is not saved inside repository. " &
                        "Save the file inside the repository and try again" &
                        vbCrLf & "Error message:" & vbCrLf & status.errorMessage)
            ElseIf status.errorMessage = "Status Returned from SVN Server with No Items" Then
                iSwApp.SendMsgToUser("Current save file For model Not found. Save it inside the repository.")
                Exit Sub
            Else
                'Other Errors
                iSwApp.SendMsgToUser("Status Check failed." &
                        vbCrLf & "Error message:" & vbCrLf & status.errorMessage)
            End If
            Exit Sub
        End If
        ' File was found on vault!
        If status.upToDate9 = "*" Then
            'Vault has a newer version
            iSwApp.SendMsgToUser("Local copy is out of date. Update from Vault and try again.")
            Exit Sub
        End If
        If status.lock6 = "O" Then
            ' File locked for editing by another user!
            ' We'll just call the lock TortoiseProc process and let it+user deal with it
            iSwApp.SendMsgToUser("File is locked by another user. Ask them to check it back in. " &
                     "If you steal their lock (Not recommended) then " &
                     vbCrLf & "1. Make sure you discuss it with them so that work isn't lost!" &
                     vbCrLf & "2. In SolidWorks you'll have to do file > Get Write Access after you have the lock.")
        End If
        bSuccess = runTortoiseProcexeWithMonitor("/command:lock /path:""" & sActiveDocPath(0) & """ /closeonend:3")
        If Not bSuccess Then
            Exit Sub
        End If

        setReadWriteFromLockStatus(modDocArr)

    End Sub

    Sub myGetLatestAll()
        myGetLatest(1)
    End Sub

    Sub myGetLatestOpenOnly()
        myGetLatest(0)
    End Sub

    Sub myGetLatest(ByVal updateClosedDocs As Integer)
        'Dim bSuccess As Boolean
        'Dim OpenDocPathList() As String
        Dim OpenDocModels() As ModelDoc2
        Dim modDocTemp As ModelDoc2

        Dim i As Integer
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim m As Integer = 0
        'Dim index As Integer
        Dim sFilePathCat As String

        OpenDocModels = getAllOpenDocs(bMustBeVisible:=False)

        Dim sFileListToUpdate(OpenDocModels.Length - 1) As String
        Dim docListToReload(OpenDocModels.Length - 1) As ModelDoc2

        Dim sOpenDocPath() As String = getFilePathsFromModDocArr(OpenDocModels)

        'Run SVN to see status of all
        Dim mySVNStatus As SVNStatus() = getFileSVNStatus(sOpenDocPath, bCheckServer:=True)

        Dim modDocArr(mySVNStatus.Length - 1) As ModelDoc2
        For i = 0 To mySVNStatus.Length - 1

            If mySVNStatus(i).upToDate9 = "*" Then
                'If file is out of date

                modDocTemp = SwApp.GetOpenDocumentByName(mySVNStatus(i).filename)
                'index = Array.IndexOf(sOpenDocPath, mySVNStatus(i)) ' Searches for an SVNfilepath in the array of open file paths
                'If index >= 0 Then

                If Not modDocTemp Is Nothing Then
                    'If File Is open Then
                    modDocArr(m) = modDocTemp
                    m = m + 1
                    If modDocTemp.IsOpenedReadOnly() Then
                        ' ForceReleaseLocks is releasing SolidWorks's system lock on the file
                        ' Which prevents other programs (like SVN) from overwriting the file
                        ' This allows the file to be overwritten by the New version
                        If modDocTemp.GetType <> 3 Then
                            'The method doesn't work for Drawings
                            modDocTemp.ForceReleaseLocks()
                            docListToReload(k) = modDocArr(i)
                            k = k + 1
                        End If
                    Else
                        ' The user has an obsolete write copy of a file. They'll have to
                        ' Decide if they want to destroy their changes or the ones on the vault.
                        ' Do nothing. Let Tortoise.exe notify user of their issue.
                    End If
                    sFileListToUpdate(j) = mySVNStatus(i).filename
                    j = j + 1
                Else
                    If updateClosedDocs = 1 Then
                        ' file out of date, not open
                        ' if bUpdateClosed then
                        ' Add to file list 
                        sFileListToUpdate(j) = mySVNStatus(i).filename
                        j = j + 1
                    End If
                End If
            End If
        Next

        If j = 0 Then
            iSwApp.SendMsgToUser("All Files Checked Are Up to Date!")
            Exit Sub
        End If

        runTortoiseProcexeWithMonitor("/command:update /path:" & formatFilePathArrForTortoiseProc(sFileListToUpdate))

        For i = 0 To k - 1
            If docListToReload(i) Is Nothing Then Exit For
            'Reattaches the file system to SolidWorks. Whatever that means :p
            'This only works for parts/Assemblies!
            'docModTemp.ForceReleaseLocks()
            docListToReload(i).ReloadOrReplace(ReadOnly:=True, ReplaceFileName:=False, DiscardChanges:=True)
        Next

        setReadWriteFromLockStatus(modDocArr)
    End Sub
    Sub NoCallbackSub()

    End Sub

    Sub FlyoutCommandItem1()
        iSwApp.SendMsgToUser("Flyout command 1")
    End Sub

    Function FlyoutEnable() As Integer
        Return 1
    End Function

    Function FlyoutDisable() As Integer
        Return 0
    End Function
    Sub FlyoutCallback()

    End Sub
    Function getFilePathsFromModDocArr(modDocArr() As ModelDoc2) As String()
        Dim getFilePathsArr(modDocArr.Length - 1) As String
        For i = 0 To modDocArr.Length - 1
            getFilePathsArr(i) = modDocArr(i).GetPathName()
        Next
        Return (getFilePathsArr)
    End Function
    Function getAllOpenDocs(ByRef bMustBeVisible As Boolean) As ModelDoc2()
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
        ReDim Preserve modDocOutput(i - 1)
        Return modDocOutput
    End Function
    Function formatFilePathArrForTortoiseProc(ByRef sFilePathArr() As String) As String
        Dim sFilePathCat As String = """" & sFilePathArr(0)
        For i = 1 To sFilePathArr(0).Length - 1
            If sFilePathArr(i) Is Nothing Then Continue For
            sFilePathCat &= "*" & sFilePathArr(i)
        Next
        sFilePathCat &= """"
        Return sFilePathCat
    End Function
    Function runTortoiseProcexeWithMonitor(ByRef sArguments As String) As Boolean
        ' See https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html
        Dim oTortProcess As New Process()
        Dim tortStartInfo As New ProcessStartInfo

        tortStartInfo.FileName = sTortPath
        tortStartInfo.Arguments = sArguments
        tortStartInfo.WorkingDirectory = sRepoLocalPath
        oTortProcess.StartInfo = tortStartInfo
        oTortProcess.Start()

        'Monitor the process. Kill it if it stops responding
        Dim nResponding As Integer = 0
        Do While (Not oTortProcess.HasExited)
            nResponding += oTortProcess.Responding
            If nResponding > 5000 Then 'Sort of milliseconds because of the sleep command. But not exactly.
                oTortProcess.Kill()
                iSwApp.SendMsgToUser("SVNTortoise Window connecting to vault terminated")
                Return False
            End If
            System.Threading.Thread.Sleep(1)
        Loop

        Return True
    End Function
    Sub save3AndShowErrorMessages(ByRef modDocArr() As ModelDoc2)
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
    Sub setReadWriteFromLockStatus(ByRef modDoc() As ModelDoc2)
        Dim status() As SVNStatus
        Dim sDocPaths(modDoc.Length - 1) As String
        Dim sStolenLockPaths As String = ""
        Dim i As Integer
        Dim j As Integer

        ' Now the file couldve been locked or not by the user in the dialog box
        ' So we'll query the vault and see what happened
        For i = 0 To sDocPaths.Length - 1
            If modDoc(i) Is Nothing Then Continue For
            sDocPaths(j) = modDoc(i).GetPathName
            j += 1
        Next

        status = getFileSVNStatus(sDocPaths, False)
        If status(0).errorMessage <> "" Then
            iSwApp.SendMsgToUser("Status Check failed." &
                        vbCrLf & "Error message:" & vbCrLf & status(0).errorMessage)
        End If

        For i = 0 To sDocPaths.Length - 1
            If status(i).lock6 = "K" Then
                ' The user got a lock! Let's change to write access
                modDoc(i).SetReadOnlyState(False)
                'save3AndShowErrorMessages(modDoc)
            ElseIf (status(i).lock6 = "S") Or (status(i).lock6 = "B") Then
                'Lock stolen / broken
                sStolenLockPaths = sStolenLockPaths & vbCrLf & status(i).filename
                modDoc(i).SetReadOnlyState(True)
            ElseIf status(i).lock6 = " " Then
                'User didn't get a lock.
                modDoc(i).SetReadOnlyState(True)
            End If
        Next

        If sStolenLockPaths <> "" Then
            SwApp.SendMsgToUser("Another user has stolen/broken your lock(s) for the following part(s). " &
                    "They are being switched to read-only. You'll have to Lock/Checkout them out again." &
                    "Any changes since last checked-in to the server may be lost." &
                    sStolenLockPaths)
        End If
    End Sub
    Function getFileSVNStatus(ByVal sFilePath() As String, ByVal bCheckServer As Boolean) As SVNStatus()
        Dim output(sFilePath.Length - 1) As SVNStatus 'starts with one element then gets redim'd later
        Dim oSVNProcess As New Process()
        Dim SVNstartInfo As New ProcessStartInfo
        Dim sOutputLines() As String
        Dim sOutputErrorLines() As String
        'Dim sLine2 As String
        Dim SVNOutput As String
        Dim SVNErrorOutput As String
        Dim bSuccess As Boolean = False
        Dim sFilePathCat As String = ""
        Dim iLineStep As Integer = 1
        Dim i As Integer = 0
        Dim j As Integer = 0

        'TODO: Error check. One file from list is not in server folder.

        For i = 0 To sFilePath.Length - 1
            If sFilePath(i) Is Nothing Then Exit For
            sFilePathCat = sFilePathCat & """" & sFilePath(i) & """ "
        Next
        SVNstartInfo.Arguments = "status" & If(bCheckServer, " -u", "") & " --verbose " & sFilePathCat

        SVNstartInfo.FileName = sSVNPath
        SVNstartInfo.UseShellExecute = False
        SVNstartInfo.RedirectStandardOutput = True
        SVNstartInfo.RedirectStandardError = True
        SVNstartInfo.CreateNoWindow = True
        oSVNProcess.StartInfo = SVNstartInfo
        oSVNProcess.Start()
        oSVNProcess.WaitForExit()

        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardOutput
            SVNOutput = oStreamReader.ReadToEnd()
        End Using
        Using oStreamReader As System.IO.StreamReader = oSVNProcess.StandardError
            SVNErrorOutput = oStreamReader.ReadToEnd()
        End Using
        sOutputLines = SVNOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        sOutputErrorLines = SVNErrorOutput.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

        For i = 0 To sOutputErrorLines.Length - 1
            df

        Next


        'Error Checking
        If output(0).errorMessage Is Nothing Then
            output(0).errorMessage = "Error: SVN status output standard error is nothing. Must of not connected/read to SVN process"
            Return output
        ElseIf output(0).errorMessage <> "" Then
            Return output
        ElseIf sOutputLines(0) Is Nothing Then
            output(0).errorMessage = "Error: Could not read SVN Status. Returned Empty Line"
            Return output
        End If

        If (bCheckServer) Then
            If sOutputLines(0).Substring(0, 23) = "Status against revision" Then
                output(0).errorMessage = "Status Returned from SVN Server with No Items" 'Don't edit this without also changing if statements that check if errormessage is this message 
                Return output

            ElseIf (sOutputLines.Length = 1) Then
                'If we are checking the server, we should expect a line 2. If its not there then theres an error.
                output(0).errorMessage = "Error: Incomplete SVN Status. Could not Read Line 2. Line 1:" & sOutputLines(0)
                Return output
                'ElseIf (sOutputLines(0).Contains(sFilePath)) And (sOutputLines(1).Substring(0, 23) = "Status against revision") Then
            Else
                bSuccess = True
                iLineStep = 2 ' skips every second line
            End If
        Else 'ElseIf (sOutputLines(0).Contains(sFilePath)) Then
            bSuccess = True
            iLineStep = 1
        End If
        If bSuccess Then
            ' Success
            'ReDim Preserve output((sOutputLines.Length - 1) / (iLineStep + 1)) 'server tags an extra line on. True = -1, so this stops before the extra line

            For i = 0 To sOutputLines.Length - 1 Step iLineStep
                output(j).filename = sOutputLines(i).Substring(41, sOutputLines(i).Length - 41)
                output(j).addDelChg1 = sOutputLines(i).Substring(0, 1)
                output(j).modifications2 = sOutputLines(i).Substring(1, 1)
                output(j).workingDirLock3 = sOutputLines(i).Substring(2, 1)
                output(j).addWithHist4 = sOutputLines(i).Substring(3, 1)
                output(j).switchWParent5 = sOutputLines(i).Substring(4, 1)
                output(j).lock6 = sOutputLines(i).Substring(5, 1)
                output(j).tree7 = sOutputLines(i).Substring(6, 1)
                'col 8 is blank
                If bCheckServer Then
                    output(j).upToDate9 = sOutputLines(i).Substring(8, 1)
                Else
                    output(j).upToDate9 = "NoUpdate"
                End If
                j = j + 1
            Next i
        Else
            output(0).errorMessage = "Error. First read line from SVN Status: " & sOutputLines(0)
        End If
        Return output
    End Function
    Function getComponentsOfAssembly(ByRef modDoc As ModelDoc2) As ModelDoc2()

        If modDoc.GetType <> swDocumentTypes_e.swDocASSEMBLY Then
            Throw New System.Exception("modDoc is not an Assembly")
        End If

        Dim mdComponentList As New List(Of ModelDoc2)()
        Dim i As Integer = 0
        Dim swFeat As IFeature
        Dim entity As Entity
        Dim component As IComponent2

        swFeat = modDoc.FirstFeature

        While Not swFeat Is Nothing
            entity = swFeat
            If entity.GetType = swSelectType_e.swSelCOMPONENTS Then
                component = swFeat.GetSpecificFeature2
                mdComponentList.Add(component.GetModelDoc2)
            End If
            swFeat = modDoc.GetNextFeature
        End While

        Dim mdComponentArr() As ModelDoc2 = mdComponentList.ToArray
        Return mdComponentArr
    End Function

    Structure SVNStatus
        ' Each 1-9 is a one character string
        ' See http://svnbook.red-bean.com/en/1.8/svn.ref.svn.c.status.html
        Public filename As String
        Public addDelChg1 As String
        Public modifications2 As String
        Public workingDirLock3 As String
        Public addWithHist4 As String
        Public switchWParent5 As String
        Public lock6 As String
        Public tree7 As String
        'col 8 is blank
        Public upToDate9 As String
        Public errorMessage As String
    End Structure

#End Region

End Class

