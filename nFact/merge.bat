"ILRepack\ILRepack.exe" /union /out:"SpecReplay.exe" SpecReplayApp.exe Microsoft.Owin.Hosting.dll Nancy.SassAndCoffee.dll Newtonsoft.Json.dll Newtonsoft.Json.dll System.Net.Http.dll System.Net.Http.Formatting.dll System.Web.Http.dll
REM xcopy $SpecReplay.exe Output\*.* /E /Y
