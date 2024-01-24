:start
@echo off
echo Setting up the fw repository for the first use...
echo;
echo;
 
git init .
git remote add -f origin https://github.com/robotology/icub-firmware-build.git
git config core.sparseCheckout true
echo CAN/mtb4 > .git/info/sparse-checkout
echo CAN/mtb4c >> .git/info/sparse-checkout
echo CAN/strain2 >> .git/info/sparse-checkout
echo CAN/strain2c >> .git/info/sparse-checkout
echo CAN/rfe >> .git/info/sparse-checkout
echo CAN/amcbldc >> .git/info/sparse-checkout
echo ETH/MC4PLUS/bin >> .git/info/sparse-checkout
git pull origin master

IF %errorlevel% NEQ 0 GOTO :error
GOTO :end
:error
echo There was an error.
EXIT 1
:end
echo End.
EXIT 0