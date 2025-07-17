;@echo off
echo;
echo ****************************************************************************
echo ************* Flashes Bootloader *******************************************
echo ****************************************************************************
echo;

REM Prompt the user to select the programmer version
echo Please select the programmer version:
echo [1] TPPK3
echo [2] TPPK4
choice /C 12 /M "Enter 1 for TPPK3 or 2 for TPPK4:"

REM Check if the user pressed '1' or '2'
echo You selected option %errorlevel%

REM Handle the user's choice
if errorlevel 2 goto TPPK4
if errorlevel 1 goto TPPK3

:TPPK3
REM Commands for TPK3
echo Using TPK3 programmer
cd icub-firmware-build\CAN\2foc
"C:\Program Files\Microchip\MPLABX\v6.15\mplab_platform\mplab_ipe\ipecmd.exe" -TPPK3 -P33FJ128MC802 -E -M -F2foc.bootloader.hex -OL
goto END

:TPPK4
REM Commands for TPK4
echo Using  TPK4 programmer
cd icub-firmware-build\CAN\2foc
"C:\Program Files\Microchip\MPLABX\v6.15\mplab_platform\mplab_ipe\ipecmd.exe" -TPPK4 -P33FJ128MC802 -E -M -F2foc.bootloader.hex -OL
goto END

:END
pause
exit %errorlevel%
