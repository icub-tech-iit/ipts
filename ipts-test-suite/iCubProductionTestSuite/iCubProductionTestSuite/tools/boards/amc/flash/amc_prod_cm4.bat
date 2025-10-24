:start
@echo off

set PATH=C:\Program Files

echo "PROGRAM FIRST APP CORE1";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD freq=8000 ap=0 reset=SWrst
IF %errorlevel% NEQ 0 GOTO :error
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "..\..\icub-firmware-build\ETH\AMC\amc.appl.mot.hex" 0x08000000 --verify
IF %errorlevel% NEQ 0 GOTO :error
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -Rst -Run
IF %errorlevel% NEQ 0 GOTO :error
echo;
GOTO :end
:error
echo "There is an ERROR in the batch operation. Check prompted error message for more details. Ending...";
PAUSE
EXIT 1
:end
echo "All operations done successfully. Ending...";
PAUSE
EXIT 0