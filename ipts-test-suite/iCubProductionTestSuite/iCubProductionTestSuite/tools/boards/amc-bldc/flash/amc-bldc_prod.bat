:start
@echo off

set PATH=C:\Program Files

"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD freq=8000 ap=0 reset=SWrst
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "..\..\icub-firmware-build\CAN\amcbldc\amcbldc.hex" 0x08000000 --verify
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -Rst -Run
echo "PRESS ENTER TO CONTINUE"
echo;
IF %errorlevel% NEQ 0 GOTO :error
GOTO :end
:error
echo There was an error.
PAUSE
EXIT 1
:end
echo End.
PAUSE
EXIT 0