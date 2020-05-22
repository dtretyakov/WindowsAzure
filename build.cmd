@ECHO OFF

Powershell -File ".\build.ps1" -Script ".\build.cake" -Verbosity Verbose -Command %*

EXIT /b %errorlevel%

PAUSE