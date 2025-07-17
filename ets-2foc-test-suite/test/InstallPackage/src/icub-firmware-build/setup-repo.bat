:start
@echo off
echo Setting up the fw repository for the first use...
echo;
echo;

cd icub-firmware-build

if exist CAN\ (
  echo "repo already initialized.."
  git pull origin devel

) else (
    echo "initializing repo.."
    git init .
    git remote add -f origin https://github.com/robotology/icub-firmware-build.git
    git config core.sparseCheckout true
    echo CAN/2foc > .git/info/sparse-checkout
    git pull origin devel  
    git checkout devel
) 


IF %errorlevel% NEQ 0 GOTO :error
GOTO :end
:error
echo There was an error.
EXIT 1
:end
echo End.
EXIT 0