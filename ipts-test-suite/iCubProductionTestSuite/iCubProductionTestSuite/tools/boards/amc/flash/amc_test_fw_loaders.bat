:start
@echo off

set PATH=C:\Program Files

echo "Program Option Bytes: BOR_LEV=0x3, disabling boot for CM4 and enabling only boot for CM7";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -ob BOR_LEV=0x3 BCM4=0x0 BCM7=0x1 -Rst -Run
IF %errorlevel% NEQ 0 GOTO :error
echo;
echo "Flash test eLoader"
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "hex\amc.eloader_test.hex" 0x08000000 --verify
IF %errorlevel% NEQ 0 GOTO :error
echo;
echo "Flash test eUpdater"
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "..\..\icub-firmware-build\ETH\AMC\amc.eupdater.hex" 0x08000000 --verify
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