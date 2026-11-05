REM Kopieren
robocopy %~dp0\bin\Release\net10.0-windows10.0.17763 \\192.168.10.122\software$\BodoMoneyTransferCore\ /s  /XF appSettings.json *.log

pause