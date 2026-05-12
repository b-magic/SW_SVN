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




# Demonstration picture:
![Example]("Examples/Lemonade Example.png" "Demonstration Picture")

Good Luck :) Have Fun :)
