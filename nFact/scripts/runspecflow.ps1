param( [string]$RootPath, 
		[string]$SpecName, 
		[string]$ArtifactsPath, 
		[string]$TestRun, 
		[string]$Environment,
		[string]$Version)

# Terminate Script if any error occurs
$ErrorActionPreference = "Stop"

function nunitout{
    param
    (
        [Parameter(Mandatory=$false,ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true)][string]$Id
    )

    begin{
         # Write-Debug "Starting"
    }
    process {
         #Builds an insert query with csv members
         $ID
		 $ID >> "TestResult.txt"
    }
    end {
         # Write-Debug "Ending"
    }
}

# Define nunit and specflow execs

$nUnitExe = join-path -path $RootPath -childpath "NUnit\nunit-console-x86.exe"
$specFlowExe = join-path -path $RootPath -childpath "SpecFlow\specflow.exe"
$reportParserExe = join-path -path $RootPath -childpath "nFact.SpecFlow.exe"

# Define dir paths

$SpecPath = join-path -path $RootPath -childpath "projects"
$SpecPath = join-path -path $SpecPath -childpath $SpecName
$SpecProj = $SpecName + ".csproj"
$SpecBin = join-path -path $SpecPath -childpath "bin\debug\" 
$SpecDll = $SpecName + ".dll"

$SpecArtifacts = join-path -path $SpecBin -childpath "artifacts"

# Delete previous artifacts in project bin
If (Test-Path $SpecArtifacts){
	Remove-Item -Recurse -Force $SpecArtifacts
}

$TestResultXml = $SpecBin + "TestResult.xml"
$TestResultTxt = $SpecBin + "TestResult.txt"
$customArtifacts = join-path -path $ArtifactsPath -childpath "artifacts"

# Delete previous test output files
If (Test-Path $TestResultXml){
	Remove-Item $TestResultXml
}
If (Test-Path $TestResultTxt){
	Remove-Item $TestResultTxt
}
If (Test-Path $customArtifacts){
	Remove-Item $customArtifacts –Recurse
}

# Change to target path 

CD $SpecBin

# Run NUnit
&$nUnitExe $SpecDll /labels /xml=TestResult.xml | nunitout

$SpecProj = join-path -path $SpecPath -childpath $SpecProj

# Run Specflow
&$specFlowExe nunitexecutionreport $SpecProj /xmlTestResult:$TestResultXml

# Copy artifacts to artifacts store
If (Test-Path $SpecArtifacts){
	Copy-Item -Path $SpecArtifacts -Destination $customArtifacts –Recurse
}
If (Test-Path $TestResultXml){
	Copy-Item $TestResultXml $ArtifactsPath
}
If (Test-Path $TestResultTxt){
	Copy-Item $TestResultTxt $ArtifactsPath
}
$TestResultHtml = $SpecBin + "TestResult.html"
If (Test-Path $TestResultHtml){
	Copy-Item $TestResultHtml $ArtifactsPath

	$TestResultHtml = $ArtifactsPath + "\TestResult.html"
	&$reportParserExe $SpecName $TestResultHtml $TestRun $ArtifactsPath $Environment $Version
}

