powershell -NoProfile -Command "& {Import-Module BitsTransfer; Import-Module .\libs\psake.psm1; Invoke-psake .\build.ps1 -framework 4.0x64}"
