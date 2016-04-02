# bus\_rode Release Setup
In some project. When you release a app, but you find it isn't work. Why? Bucause you must set some file in release folder. Then, your app can run.  
This document can tech you how to set file which app need in every project.

*TIPS:  
All releasing application have been deleted. It is necessary that rebuild it.*

---
#### bus\_rode
* EMPTY FOLDER: libary
* file.bat \(This file has set in the release folder.\)
* bus\_rode\_icon.dat \(This file has set in the release folder.\)
* desktop.txt \(This file has set in the release folder. This file's format is written in **desktop format describe.txt** which also in release folder\)
* bus\_rode\_compression.dll \(Build it in this project called **bus\_rode\_compression**. This libary project has included in this solution\)
* bus\_rode\_check.dll \(Build it in this project called **bus\_rode\_check**. This libary project has included in this solution\)
* bus\_rode\_add.exe \(Build it in this project called **bus\_rode\_add**. This libary project has included in this solution\)

*TIPS:*  
*bus\_rode unnormal exit code describe is written in**Exit code describe.txt** which also in release folder*


---
#### bus\_rode\_add
This app need set some file in release folder, but this application depend on app called **bus\_rode**. So needn't set some file in release folder.

*TIPS:*  
*Never run this app alone\(Only bus\_rode can launch it\)*

---
#### bus\_rode\_check
Needn't set.

---
#### bus\_rode\_compression
Needn't set.

---
#### bus\_rode\_compression\_ui
* ICSharpCode.SharpZipLib.dll \(This file has set in the release folder. **This file comes form the third part** [Cilck here to know more on github](http://icsharpcode.github.io/SharpZipLib/)\)
* bus\_rode\_compression.dll \(Build it in this project called **bus\_rode\_compression**. This libary project has included in this solution\)

---
#### bus\_rode\_develop
* bus\_rode\_compression.dll \(Build it in this project called **bus\_rode\_compression**. This libary project has included in this solution\)
* bus\_rode\_develop\_tools.dll \(Build it in this project called **bus\_rode\_develop\_tools**. This libary project has included in this solution\)
* desktop.txt \(This file has set in the release folder. This file's format is written in **desktop format describe.txt** which also in release folder\)

---
#### bus\_rode\_develop\_tools
Needn't set.

---
#### bus\_rode\_smaple
Needn't set.

---
#### bus\_rode\_smaple\_csharp
Needn't set.
