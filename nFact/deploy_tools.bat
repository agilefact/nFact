cd %1
IF not exist tools ( 
	mkdir tools 
	cd tools
	mkdir specFlow
	cd..
	xcopy packages\SpecFlow.1.9.0\tools\*.* tools\specflow\*.* /Y /e
	)

IF not exist tools\specFlow ( 
	cd tools
	mkdir specFlow
	cd..
	xcopy packages\SpecFlow.1.9.0\tools\*.* tools\specflow\*.* /Y /e
	)

IF not exist tools\NUnit ( 
	cd tools
	mkdir NUnit
	cd..
	xcopy packages\NUnit.Runners.2.6.3\tools\*.* tools\NUnit\*.* /Y /e
	)

xcopy %1tools %2*.*  /Y /e