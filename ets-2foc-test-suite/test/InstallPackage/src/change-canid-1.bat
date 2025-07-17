;@echo off
echo;
echo ****************************************************************************
echo ************* Changes CAN ID ***********************************************
echo ****************************************************************************
echo;
set errorlevel=
for /f %%t in ('type can.ini')do @set can=%%t
echo cannet %can%
setCanId\setCanId.exe 14 1
exit %errorlevel%
pause