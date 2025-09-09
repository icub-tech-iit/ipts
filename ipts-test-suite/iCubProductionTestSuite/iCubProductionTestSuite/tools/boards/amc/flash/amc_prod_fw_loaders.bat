:start
@echo off

set PATH=C:\Program Files

echo "Program Option Bytes: BOR_LEV=0x3, disabling boot for CM4 and enabling only boot for CM7";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -ob BOR_LEV=0x3 BCM4=0x0 BCM7=0x1 -Rst -Run
echo;
echo "Flash prod eLoader"
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "..\..\icub-firmware-build\ETH\AMC\amc.eloader.hex" 0x08000000 --verify
echo;
echo "Flash prod eUpdater"
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "..\..\icub-firmware-build\ETH\AMC\amc.eupdater.hex" 0x08000000 --verify
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