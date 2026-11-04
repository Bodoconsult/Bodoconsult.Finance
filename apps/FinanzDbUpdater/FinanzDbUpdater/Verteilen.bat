
REM Kopieren

robocopy %~dp0bin\Release\net10.0-windows10.0.17763 \\192.168.10.122\software$\FinanzDbUpdater\ /XF appSettings.json /s

pause