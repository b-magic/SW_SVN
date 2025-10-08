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

Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
'Imports System.Configuration








'<Guid("0E1DEB16-5C3F-45F0-8213-C1B211F0426B")>'<Guid("8E7E418A-13BA-45BB-B784-D6202C1F1C47")>'"ca5108e0-c3c5-47f0-8453-cd9b6a5e12af")>
'<ComVisible(True)>
'<SwAddin(
'        Description:="Version Control from a Central SVN Server AddIn",
'        Title:="SVN_Vault-Debug",
'        LoadAtStartup:=True
'        )>
<Guid("8E7E418A-13BA-45BB-B784-D6202C1F1C47")>'"ca5108e0-c3c5-47f0-8453-cd9b6a5e12af")>
<ComVisible(True)>
<SwAddin(
        Description:="Simple Collaboration and Version Control Using SVN",
        Title:="PlumVault",
        LoadAtStartup:=True
        )>
Public Class SwAddin
    Implements SolidWorks.Interop.swpublished.SwAddin

#Region "Local Variables"
    Dim WithEvents iSwApp As SldWorks
    Dim iCmdMgr As ICommandManager
    Dim addinID As Integer
    Dim openDocs As Hashtable
    Dim SwEventPtr As SldWorks
    Dim iBmp As BitmapHandler
    Dim iAppInitTimes As Integer = 0

    Dim cAppConfig As System.Configuration.Configuration
    Dim asSettings As System.Configuration.AppSettingsSection
    Public Const mainCmdGroupID As Integer = 0

    Dim myTaskPaneView As TaskpaneView
    Public myTaskPaneHost As UserControl1

    'Update all 3 of these!
    'Public iNumFlyoutButtons As Integer = 3
    'Public mainItemID() As Integer = {0, 1, 2}
    'Public flyoutGroupID() As Integer = {91, 92, 93}

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
        'If iAppInitTimes > 0 Then Return True

        iSwApp = ThisSW
        addinID = Cookie

        ' Setup callbacks
        iSwApp.SetAddinCallbackInfo(0, Me, addinID)

        ' Setup the Command Manager
        'iCmdMgr = iSwApp.GetCommandManager(Cookie)
        'AddCommandMgr()

        'Setup the Event Handlers
        SwEventPtr = iSwApp
        openDocs = New Hashtable
        AttachEventHandlers()

        If myTaskPaneView Is Nothing Then
            AddTaskPane()
        End If

        ConnectToSW = True
    End Function
    Function DisconnectFromSW() As Boolean Implements SolidWorks.Interop.swpublished.SwAddin.DisconnectFromSW

        'RemoveCommandMgr()
        RemoveTaskPane()
        DetachEventHandlers()


        'todo disconnect task pane/manager slash release com object for side bar task manager?
        'System.Runtime.InteropServices.Marshal.ReleaseComObject(iCmdMgr)
        'iCmdMgr = Nothing
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
    Public Sub AddTaskPane()

        'sInstallDirectory
        'sInstallDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        'svnModule
        'Dim imageList1() As String = {"C:\Users\\source\repos\SolidWorksVB\PlumVault\icons\PlumVault_20.png",
        '    "C:\Users\\source\repos\SolidWorksVB\PlumVault\icons\PlumVault_40.png",
        '    "C:\Users\\source\repos\SolidWorksVB\PlumVault\icons\PlumVault_64.png",
        '    "C:\Users\\source\repos\SolidWorksVB\PlumVault\icons\PlumVault_96.png",
        '    "C:\Users\\source\repos\SolidWorksVB\PlumVault\icons\PlumVault_128.png"}

        Dim dllPath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim installFolder As String = System.IO.Path.GetDirectoryName(dllPath)

        Dim imageList() As String = {
        System.IO.Path.Combine(installFolder, "PlumVault_20.png"),
        System.IO.Path.Combine(installFolder, "PlumVault_32.png"),
        System.IO.Path.Combine(installFolder, "PlumVault_40.png"),
        System.IO.Path.Combine(installFolder, "PlumVault_64.png"),
        System.IO.Path.Combine(installFolder, "PlumVault_96.png"),
        System.IO.Path.Combine(installFolder, "PlumVault_128.png")
    }

        'If Not My.Computer.FileSystem.FileExists(imageList(3)) Then
        '    iSwApp.SendMsgToUser2("File does NOT exist " & vbCrLf & imageList(3), swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
        'Else
        '    iSwApp.SendMsgToUser2("File Found! " & vbCrLf & imageList(3), swMessageBoxIcon_e.swMbInformation, swMessageBoxBtn_e.swMbOk)
        'End If

        myTaskPaneView = iSwApp.CreateTaskpaneView3(imageList, "SVN Task Pane")
        myTaskPaneHost = myTaskPaneView.AddControl("SVN_AddIn", "")
        'myTaskPaneHost.ContextMenu = New ContextMenu
        myTaskPaneHost.myInitialize(iSwApp)

    End Sub

    Public Sub RemoveTaskPane()
        Try
            myTaskPaneHost.beforeClose()
            'asSettings.Settings.Item("localRepoPath").Value = myTaskPaneHost.localRepoPath.Text 'saves the local repoPath
            myTaskPaneHost = Nothing
            myTaskPaneView.DeleteView()
            Marshal.ReleaseComObject(myTaskPaneView)
            myTaskPaneView = Nothing
        Catch ex As Exception
        End Try
    End Sub

    'Public Sub AddCommandMgr()

    '    Dim cmdGroup As ICommandGroup

    '    If iBmp Is Nothing Then
    '        iBmp = New BitmapHandler()
    '    End If

    '    Dim thisAssembly As Assembly

    '    Dim cmdIndex(3) As Integer
    '    'Dim cmdIndex0 As Integer, cmdIndex1 As Integer
    '    Dim Title As String = "VB Addin"
    '    Dim ToolTip As String = "VB Addin"


    '    Dim docTypes() As Integer = {swDocumentTypes_e.swDocASSEMBLY, _
    '                                   swDocumentTypes_e.swDocDRAWING, _
    '                                   swDocumentTypes_e.swDocPART}

    '    thisAssembly = System.Reflection.Assembly.GetAssembly(Me.GetType())

    '    Dim cmdGroupErr As Integer = 0
    '    Dim ignorePrevious As Boolean = False

    '    Dim registryIDs As Object = Nothing
    '    Dim getDataResult As Boolean = iCmdMgr.GetGroupDataFromRegistry(mainCmdGroupID, registryIDs)

    '    'Dim knownIDs As Integer() = New Integer(1) {mainItemID(0), mainItemID(1)}

    '    If getDataResult Then
    '        If Not CompareIDs(registryIDs, mainItemID) Then 'knownIDs) Then 'if the IDs don't match, reset the commandGroup
    '            ignorePrevious = True
    '        End If
    '    End If

    '    cmdGroup = iCmdMgr.CreateCommandGroup2(mainCmdGroupID, Title, ToolTip, "", -1, ignorePrevious, cmdGroupErr)
    '    If cmdGroup Is Nothing Or thisAssembly Is Nothing Then
    '        Throw New NullReferenceException()
    '    End If



    '    cmdGroup.LargeIconList = iBmp.CreateFileFromResourceBitmap("PlumVault.ToolbarLarge.bmp", thisAssembly)
    '    cmdGroup.SmallIconList = iBmp.CreateFileFromResourceBitmap("PlumVault.ToolbarSmall.bmp", thisAssembly)
    '    cmdGroup.LargeMainIcon = iBmp.CreateFileFromResourceBitmap("PlumVault.MainIconLarge.bmp", thisAssembly)
    '    cmdGroup.SmallMainIcon = iBmp.CreateFileFromResourceBitmap("PlumVault.MainIconSmall.bmp", thisAssembly)

    '    'Dim menuToolbarOption As Integer = swCommandItemType_e.swMenuItem Or swCommandItemType_e.swToolbarItem

    '    'cmdIndex = {0, 1, 2} 'cmdIndex0, cmdIndex1
    '    ''                      AddCommandItem2(Name, Position, HintString, ToolTip, ImageListIndex, CallbackFunction, EnableMethod, UserID, MenuTBOption)
    '    'cmdIndex(0) = cmdGroup.AddCommandItem2("CreateCube", -1, "Mouseover", "Label", 0, "CreateCube", "", mainItemID(0), menuToolbarOption)
    '    'cmdIndex(1) = cmdGroup.AddCommandItem2("Show PMP", -1, "Mouseover", "Label", 2, "ShowPMP", "PMPEnable", mainItemID(1), menuToolbarOption)

    '    'cmdGroup.HasToolbar = True
    '    'cmdGroup.HasMenu = True
    '    'cmdGroup.Activate()

    '    'Dim flyGroup1 As FlyoutGroup
    '    ''flyGroup1 = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
    '    ''      cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "FlyoutCallback", "FlyoutEnable")
    '    'flyGroup1 = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
    '    '      cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
    '    ''        AddCommandItem(Name,     Mouseover,  ImageListIndex,  CallbackFunction, UpdateCallbackFunction
    '    ''flyGroup1.AddCommandItem("FlyoutCommand 1", "test", 0, "FlyoutCommandItem1", "FlyoutEnable") '"FlyoutEnable") '"FlyoutDisabled
    '    'flyGroup1.RemoveAllCommandItems()
    '    'flyGroup1.AddCommandItem(System.DateTime.Now.ToLongTimeString(), "test", 0, "FlyoutCommandItem1", "FlyoutEnable")
    '    ''flyGroup1.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Simple
    '    'flyGroup1.FlyoutType = 1 'swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

    '    Dim flyGroupLock As FlyoutGroup
    '    flyGroupLock = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(0), "Lock and Checkout", "Lock/Checkout", "Lock and checkout the current document",
    '          cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
    '    flyGroupLock.RemoveAllCommandItems()
    '    flyGroupLock.AddCommandItem("Lock/Checkout", "Lock and checkout the current document", 0, "myCheckoutActiveDoc", "FlyoutEnable")
    '    flyGroupLock.FlyoutType = 1 'swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

    '    Dim flyGroupCheckin As FlyoutGroup
    '    flyGroupCheckin = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(1), "Checkin", "Checkin", "Checkin the current document",
    '          cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
    '    flyGroupCheckin.RemoveAllCommandItems()
    '    flyGroupCheckin.AddCommandItem("Checkin ActiveDoc", "Checkin the current document", 0, "myCheckinActiveDoc", "FlyoutEnable")
    '    flyGroupCheckin.AddCommandItem("Checkin All", "Checkin Any/All documents", 0, "myCommitAll", "FlyoutEnable")
    '    flyGroupCheckin.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

    '    Dim flyGroupGetLatest As FlyoutGroup
    '    flyGroupGetLatest = iCmdMgr.CreateFlyoutGroup(flyoutGroupID(2), "Get Latest", "Get Latest", "Get The Latest Files from The Vault",
    '          cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, "NoCallbackSub", "FlyoutEnable")
    '    flyGroupGetLatest.RemoveAllCommandItems()
    '    flyGroupGetLatest.AddCommandItem("Get Latest All", "Get Latest All", 0, "myGetLatestAll", "FlyoutEnable")
    '    flyGroupGetLatest.AddCommandItem("Get Latest Open Files Only", "Get Latest Open Docs", 0, "myGetLatestOpenOnly", "FlyoutEnable")
    '    flyGroupGetLatest.FlyoutType = swCommandFlyoutStyle_e.swCommandFlyoutStyle_Favorite

    '    For Each docType As Integer In docTypes
    '        Dim cmdTab As ICommandTab = iCmdMgr.GetCommandTab(docType, Title)
    '        Dim bResult As Boolean

    '        If Not cmdTab Is Nothing And Not getDataResult Or ignorePrevious Then 'if tab exists, but we have ignored the registry info, re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
    '            Dim res As Boolean = iCmdMgr.RemoveCommandTab(cmdTab)
    '            cmdTab = Nothing
    '        End If

    '        If cmdTab Is Nothing Then
    '            cmdTab = iCmdMgr.AddCommandTab(docType, Title)

    '            'Dim cmdBox0 As CommandTabBox = cmdTab.AddCommandTabBox

    '            'Dim cmdIDs(cmdIndex.Length) As Integer
    '            'Dim TextType(cmdIndex.Length) As Integer

    '            'cmdIDs(0) = cmdGroup.CommandID(cmdIndex(0))
    '            'TextType(0) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal

    '            'cmdIDs(1) = cmdGroup.CommandID(cmdIndex(1))
    '            'TextType(1) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal

    '            'cmdIDs(2) = cmdGroup.ToolbarId
    '            'TextType(2) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal


    '            'bResult = cmdBox0.AddCommands(cmdIDs, TextType)

    '            Dim cmdBox1 As CommandTabBox = cmdTab.AddCommandTabBox()
    '            Dim cmdIDs(iNumFlyoutButtons) as Integer
    '            Dim TextType(iNumFlyoutButtons) As Integer

    '            cmdIDs(0) = flyGroupLock.CmdID
    '            TextType(0) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

    '            cmdIDs(1) = flyGroupCheckin.CmdID
    '            TextType(1) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

    '            cmdIDs(2) = flyGroupGetLatest.CmdID
    '            TextType(2) = swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow

    '            bResult = cmdBox1.AddCommands(cmdIDs, TextType)

    '            cmdTab.AddSeparator(cmdBox1, cmdIDs(0))
    '            cmdTab.AddSeparator(cmdBox1, cmdIDs(1))

    '        End If
    '    Next

    '    thisAssembly = Nothing

    'End Sub


    'Public Sub RemoveCommandMgr()
    '    Try
    '        iBmp.Dispose()
    '        iCmdMgr.RemoveCommandGroup(mainCmdGroupID)
    '        For Each flyoutGroupID_element In flyoutGroupID
    '            iCmdMgr.RemoveFlyoutGroup(flyoutGroupID_element)
    '        Next flyoutGroupID_element
    '    Catch e As Exception
    '    End Try
    'End Sub


    'Function AddPMP() As Boolean
    '    ppage = New UserPMPage
    '    ppage.Init(iSwApp, Me)
    'End Function

    'Function RemovePMP() As Boolean
    '    ppage = Nothing
    'End Function

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
        'AttachEventsToAllDocuments()
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
            'AddHandler iSwApp.DocumentLoadNotify2, AddressOf Me.SldWorks_DocumentLoadNotify2
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
            'RemoveHandler iSwApp.DocumentLoadNotify2, AddressOf Me.SldWorks_DocumentLoadNotify2
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

    'Function SldWorks_DocumentLoadNotify2(ByVal docTitle As String, ByVal docPath As String) As Integer
    'End Function

    Function SldWorks_FileNewNotify2(ByVal newDoc As Object, ByVal doctype As Integer, ByVal templateName As String) As Integer
        'AttachEventsToAllDocuments()
    End Function

    Function SldWorks_ActiveModelDocChangeNotify() As Integer
        myTaskPaneHost.switchTreeViewToCurrentModel()

        'Dim mycontextmenu As New UserControl1.myContextMenuClass(iSwApp.ActiveDoc, iSwApp)
        'myTaskPaneHost.ContextMenuStrip.Items.AddRange({mycontextmenu.openLabel})

    End Function

    Function SldWorks_FileOpenPostNotify(ByVal FileName As String) As Integer
        'AttachEventsToAllDocuments()
        myTaskPaneHost.switchTreeViewToCurrentModel()
        myTaskPaneHost.externalSetReadWriteFromLockStatus1()
        'Dim mycontextmenu As New UserControl1.myContextMenuClass(iSwApp.ActiveDoc, iSwApp)
        'myTaskPaneHost.ContextMenuStripmenu.openLabel})

    End Function
#End Region

#Region "UI Callbacks"

    Dim mdComponentList As New List(Of ModelDoc2)()

#End Region

End Class

