:start
@echo off

set PATH=C:\Program Files

echo "First Program User Configuration Option Bytes DBANK and nSWBOOT0";
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -ob DBANK=0x0 nSWBOOT0=1 -Rst -Run
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD freq=8000 ap=0 reset=SWrst
echo;
"%PATH%\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe" -c port=SWD -d "hex\amcbldc.test.hex" 0x08000000 --verify
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