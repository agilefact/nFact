param([string]$Path, [string]$Server, [string]$Source, [string]$SpecName)

$dotNetVersion = "4.0" # valid versions are [2.0, 3.5, 4.0]
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"

$tfsExe = join-path -path $Path -childpath "TFS\tf.exe"
$msbuildExe = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"

$Dest = join-path -path $Path -childpath "projects"
$Dest = join-path -path $Dest -childpath $SpecName

Remove-Item -Recurse -Force $Dest
New-Item -ItemType directory -Path $Dest

Set-Content -Value $Source -Path $Dest\source

&$tfsExe workspace /new /noprompt /collection:$Server /permission:Private "SpecRunner"
&$tfsExe workfold /map $Source $Dest /workspace:"SpecRunner" 
cd $Dest
&$tfsExe get . /Recursive
&$tfsExe workspace /delete /noprompt "SpecRunner"

$SpecBin = join-path -path $Dest -childpath "bin\debug\" 
$SpecProj = $SpecName + ".csproj"

&$msbuildExe $SpecProj
