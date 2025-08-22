:start
@echo off

set PATH=C:\Program Files

echo "Program Option Bytes disabling boot for CM4 and enabling only boot for CM7";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -ob BCM4=0x0 BCM7=0x1 -Rst -Run
echo "Flash firmware for tests on CM7 core";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD freq=8000 ap=0 reset=SWrst
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "hex\amc_test_cm7_only.hex" 0x08000000 --verify
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