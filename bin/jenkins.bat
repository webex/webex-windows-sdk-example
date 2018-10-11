
@echo OFF



if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin" set MSBUILDDIR=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\IDE" set DEVENVDIR=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\IDE
echo %MSBUILDDIR%
echo %DEVENVDIR%

pushd .
bin\nuget.exe restore solutions\KitchenSink\KitchenSink.sln -NonInteractive

REM clear NuGet package cache
bin\nuget.exe locals all -clear

set SDKNuGetPackage=2.0.0-EFT04
echo SDKNuGetPackage is %SDKNuGetPackage%

echo remove old Cisco.Webex.WindowsSDK.%SDKNuGetPackage%
rd /S /Q solutions\KitchenSink\packages\Cisco.Webex.WindowsSDK.%SDKNuGetPackage%

echo install new Cisco.Webex.WindowsSDK.%SDKNuGetPackage% from last successful build package from jenkins sdk develop branch build.
bin\nuget.exe install Cisco.Webex.WindowsSDK -Source "%cd%"\bin\ -Version %SDKNuGetPackage% -PreRelease -SolutionDirectory solutions\KitchenSink\ -Verbosity detailed
if not %errorlevel% == 0 ( 
	echo install new Cisco.Webex.WindowsSDK.%SDKNuGetPackage% failed!
	goto EXIT 
)

REM echo copy scf libraries to solutions\KitchenSink\packages\%SDKNuGetPackage%\
REM copy /y spark-client-framework\scfLibrary\Release\*.dll solutions\KitchenSink\packages\%SDKNuGetPackage%\
REM if not %errorlevel% == 0 ( 
	REM echo update scf libraries failed.
	REM goto EXIT 
REM )
REM echo copy WebexSDK.dll to solutions\KitchenSink\packages\%SDKNuGetPackage%\
REM copy /y sdk\binary\[build]\bin\x86\Release\WebexSDK.dll solutions\KitchenSink\packages\%SDKNuGetPackage%\
REM if not %errorlevel% == 0 ( 
	REM echo update WebexSDK.dll failed.
	REM goto EXIT 
REM )

REM "%MSBUILDDIR%\msbuild.exe" solutions\KitchenSink\KitchenSink.sln /t:Rebuild /p:Configuration="Debug" /p:Platform="x86"
"%DEVENVDIR%\devenv" solutions\KitchenSink\KitchenSink.sln /Rebuild "Debug|x86"
if not %errorlevel% == 0 ( 
	echo build debug version failed!
	goto EXIT 
)

REM "%MSBUILDDIR%\msbuild.exe" solutions\KitchenSink\KitchenSink.sln /t:Rebuild /p:Configuration="Release" /p:Platform="x86"
"%DEVENVDIR%\devenv" solutions\KitchenSink\KitchenSink.sln /Rebuild "Release|x86"
if not %errorlevel% == 0 (
	echo build release version failed!
	goto EXIT
)

popd

:EXIT
echo error level: %errorlevel% 
EXIT /B %errorlevel%