@echo off

for /f %%t in ('type can.ini')do @set can=%%t
echo cannet %can%
copy gulp.conf1  autosave.gup  /y
echo %can%, Baudrate 1000, ID len 1, 0x0 -^> 0x7ff >> autosave.gup
type gulp.conf2 >> autosave.gup

  
