# Plum Vault (SW SVN) - SVN Integration for SolidWorks
## Disclaimer
This project is not endorsed by, affiliated with, maintained, authorized, or sponsored by SolidWorks or Dassault Systems. All product and company names are the registered trademarks of their original owners. The use of any trade name or trademark is for identification and reference purposes only and does not imply any association with the trademark holder of their product brand.

## What is PlumVault (SW SVN)?
Open Source Collaboration and Version Control for SolidWorks
* Version Control: revert to old versions, view file history, etc.
* Collaboration Features: Seamlessly allow multiple people to edit components in a large shared assembly without overwriting each others work. 
* File Storage

### How does it Work?
#### At a High Level
* Adds a SolidWorks add-in for seamless integration with long-standing open source software for version control- SubVersion (SVN) and TortoiseSVN.
* SVN = Subversion. The OG version control system. Written by software developers for collaborating on software development. It is very similar to git, but git by default stores the entire history of the repository (yes even files you think you deleted) locally on every users computer, which is fine for text files like source code. SVN meanwhile has a central server that stores history and provides some useful collaboration features. The most useful feature for CAD users is the ability to 'lock' out files, which prevents others from editing them at the same time as you and overwriting each other. 

#### How do I use it?
* There's a central server that stores all the CAD files and their history. 
* Everyone "checks out" a local copy of the repository from the server. All local files are 'read-only' by default.
* Want to edit a file? Take the 'lock', which 1) checks with the server that you have the latest version, then 2) tells the server no one else is allowed to get write-access. 
* Finished your changes? "Commit" your changes to the server. You can keep your lock to continue working, or release your lock so the next person can lock & edit it.
* Don't like your changes? You can release your lock and revert your copy to the latest on the server. 

## Getting Setup up
You will need to set up an SVN Server. Complete Setup assistance and training are available for Eligible Student Teams via Sponsorship.  




## Joining the Developer Team
#### Want to add new features to the project? 
### Getting Set up to code and improve the project as a developer

### Integrated Development Enviornment (IDE)
1. Pre-requisite: Windows computer. Not Mac. sorry. 
2. Get visual studio. https://visualstudio.microsoft.com/downloads/
  A. Students from certain universities can get it for free through microsoft azure. https://signup.azure.com/studentverification?offerType=3
  B. Otherwise, you can get the visual studio community package.  https://visualstudio.microsoft.com/downloads/
2. In the Windows app visual studio installer (not the extension), get the ".NET Desktop Developer" workload. 
3. Get the Visual Studio Installer (VSI) add-on package/extension so you can output install files. Note: This is separate/different than the windows app that has the same name. Instructions: https://www.codestack.net/solidworks-api/deployment/installer/vsi/. The instruction videos shows Within Visual studio to go Tools>Extensions>Manage, or just Extensions>Manage on newer versions. Within the popup window, search for "Windows Studio Installer" And select the first one, with an icon of an open cardboard box.  
4. Optional: You may wih to get the github extension at the same time. 
5. Note: In order to build the install file, you will also need tortoiseSVN installed on your local machine. https://tortoisesvn.net/downloads.html That should save the TortoiseSVN executable at "C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe". If not, copy it to that folder. 


### Solidworks
#### Get the API SDK add-in.
1. Add/remove programs > Solidworks > Modify. Go through until you can select products, expand the API checkbox.
2. Navigate to C:\Users\<username>\Documents\SOLIDWORKS Downloads\SOLIDWORKS 2019 SP02\apisdk
3. Run the SolidWorks API SDK.msi file. 

#### Issues & Fixes
##### Visual studio can't find sldworks references
1. In the VS solution explorer, SVN_Vault >(expand the tree)> Reference > click on SolidWorks.Interop.sldworks. The exclamation triangle icon on that item should disappear. 
2. If that didn't work, right click SVN_Vault project, and select "Manage NuGet packages", pick the Browse tab, and search for and install "Microsoft.NETFramework.ReferenceAssemblies"
If it doesn’t ask about templates, then follow this link

##### Missing sdk templates
I'm not sure if this is actaully needed, but 
https://www.codestack.net/solidworks-api/troubleshooting/addins/sdk-installation/
Which says to Save swvbaddin.zip into 
C:\Users\<username>\Documents\Visual Studio 2019\Templates\ProjectTemplates\Visual Basic

http://help.solidworks.com/2019/english/api/sldworksapiprogguide/Overview/SolidWorks_CSharp_and_VB.NET__Project_Templates.htm

##### The referenced component 'Microsoft.VisualBasic' could not be found.
Ignore this error. Things work fine while its there.

##### Unable to delete file "C:\Users\username\source\repos\SW_SVN\SolidWorksSVN\bin\SolidWorksSVN.dll". Access to the path 'C:\...\bin\SolidWorksSVN.dll' is denied.
1. Check if Solidworks is Running, and close it
2. Check if any Solidworks related processes are running in task manager, and force close them
3. Close and re-open visual studio
4. Restart computer


### Debugging
#### Visual Studio
1. Always start visual studio as administrator (start > visual studio > right click > run as administrator) otherwise you can't change the registry to actually open up 
the add-in inside solidworks. 

#### Creating an installer
1. Follow https://www.codestack.net/solidworks-api/deployment/installer/vsi/
2. After you right click on the SVN_Vault_installer project and click rebuild, the .msi installer file should show up in C:\Users\<username>\source\repos\SolidWorksVB\SW_SVN\Debug

# Demonstration picture:
![Example](Examples/SW_SVN_Add-In_Example.png "Demonstration Picture")

Good Luck :) Have Fun :)
