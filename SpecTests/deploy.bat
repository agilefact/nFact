rem @echo off
set "projectsDir=projects"
set "testDir=SpecTests"
CD "%2"
IF not exist %projectsDir% ( mkdir %projectsDir% && echo %projectsDir% created)
CD %projectsDir%
IF not exist %testDir% ( mkdir %testDir% && echo %testDir% created)
CD %testDir%
xcopy %1*.* %2\%projectsDir%\%testDir% /Y /e
CD "%1"