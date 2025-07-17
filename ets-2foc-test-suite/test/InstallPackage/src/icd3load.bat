@echo off
"%ProgramFiles%\Microchip\MPLAB IDE\Programmer Utilities\ICD3\ICD3CMD.exe" -P33FJ128MC802 -E -M -Fbootloader.hex
"%ProgramFiles%\Microchip\MPLAB IDE\Programmer Utilities\ICD3\ICD3CMD.exe" -P33FJ128MC802 -L
echo;
echo;
pause