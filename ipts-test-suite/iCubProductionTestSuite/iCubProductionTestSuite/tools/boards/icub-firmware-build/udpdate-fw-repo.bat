:start
@echo off
echo Updating firmware binaries...
echo;

git pull origin master

IF %errorlevel% NEQ 0 GOTO :error
GOTO :end
:error
echo There was an error.
EXIT 1
:end
git log > fw-log.txt
echo End.
EXIT 0