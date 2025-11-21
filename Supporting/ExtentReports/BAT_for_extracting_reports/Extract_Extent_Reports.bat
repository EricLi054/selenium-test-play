@echo off
echo.
echo.
echo Creating directories and extracting archives when count-down is complete.
echo.
echo Press Ctrl+C if you need to terminate this process.
echo.
@echo on
@echo off 
TIMEOUT /T 3

mkdir ExtentReports
mkdir ExtentReports\AgentJob1
mkdir ExtentReports\AgentJob2
mkdir ExtentReports\AgentJob3

@echo off
echo.
echo.
echo Extracting Extent Report Archives, please refresh view of folder when complete.
echo.
@echo on
@echo off 
setlocal
cd /d %~dp0
Call :UnZipFile "%cd%\ExtentReports\AgentJob1\" "%cd%\Agent job 1\9_TestReport.zip"
Call :UnZipFile "%cd%\ExtentReports\AgentJob2\" "%cd%\Agent job 2\9_TestReport.zip"
Call :UnZipFile "%cd%\ExtentReports\AgentJob3\" "%cd%\Agent job 3\9_TestReport.zip"
exit /b

:UnZipFile <ExtractTo> <newzipfile>
set vbs="%temp%\_.vbs"
if exist %vbs% del /f /q %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo objShell.NameSpace(%1).CopyHere(FilesInZip)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
cscript //nologo %vbs%
if exist %vbs% del /f /q %vbs%
