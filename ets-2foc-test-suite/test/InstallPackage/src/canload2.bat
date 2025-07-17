;@echo off
echo;
echo ****************************************************************************
echo ************* CAN flash will take several minutes, please wait **************
echo ****************************************************************************
echo;
set errorlevel=
for /f %%t in ('type can.ini')do @set can=%%t
echo cannet %can%
canLoader.exe --canDeviceType ecan --canDeviceNum %can% --boardId 13 --firmware 2FOC2.hex
exit %errorlevel%
pause