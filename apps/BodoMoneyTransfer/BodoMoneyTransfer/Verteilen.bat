REM Kopieren
robocopy %~dp0\bin\Release\net8.0 \\192.168.10.122\software$\BodoMoneyTransferCore\ /s  /XF appSettings.json *.log

pause