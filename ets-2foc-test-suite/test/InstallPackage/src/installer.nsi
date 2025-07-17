; installer.nsi
;
; Neuro stim SW install script
;
;--------------------------------


; The name of the installer
Name "2FOC Test program"

; The file to write
OutFile "2FOC-Setup.exe"

; The default installation directory
InstallDir $INSTDIR

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
;InstallDirRegKey HKLM "Software\Neuro" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


!include LogicLib.nsh

Function .onInit
 ReadEnvStr $R0 SYSTEMDRIVE
  StrCpy $INSTDIR `$R0\2FOCTEST`
FunctionEnd

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------


Section -Prerequisites

;SetOutPath "$INSTDIR\esd1"
;  File ".\esd1\usb331.sys"
;  File ".\esd1\c331.sys"
;  File ".\esd1\calcan32.dll"
;  File ".\esd1\canui32.dll"
;  File ".\esd1\ntcan.dll"
;  File ".\esd1\disk1"
;  File ".\esd1\canesd2k.inf"
;     
;SetOutPath $INSTDIR

; ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection ClassInstall32.NT 128 .\esd1\canesd2k.inf"
; ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection C331 128 .\esd1\canesd2k.inf"




SetOutPath $INSTDIR

  File ".\canu2292.inf"
  File ".\canu331.inf"
  SetOutPath "$INSTDIR\x86"
  File ".\x86\calcan32.dll"
  File ".\x86\canui32.dll"
  File ".\x86\ntcan.dll"
  File ".\x86\usb2292.sys"
  File ".\x86\usb331.sys"
  
  SetOutPath $INSTDIR
  File ".\x86\canui32.dll"
 ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection ClassInstall32.NT 128 .\canu331.inf"
 ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection U331 128 .\canu331.inf"
 ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection ClassInstall32.NT 128 .\canu2292.inf"
 ExecWait "$WINDIR\system32\rundll32.exe SETUPAPI.DLL,InstallHinfSection U2292 128 .\canu2292.inf"

MessageBox MB_OK "Connettere l'adattatore USB-CAN ad una porta USB$\nAttendere che windows presenti la finestra di installazione nuovo hardware,$\nscegliere quindi l'opzione 'No non ora' e premere due volte il tasto 'avanti'"

MessageBox MB_OK "Verra' ora lanciata l'installazione dell'ambiente di sviluppo Microchip, necessario alla programmazione del DSP.$\nE' importante che l'installazione venga portata a termine lasciando immutata la directory di installazione di default.$\n$\nDurante l'installazione verra' chiesto se si desidera installare il compilatore HI-TECH. Selezioneare 'no'.$\n$\nQualora durante la procedura di installazione dovesse comparire una finestra di avviso riguardante alcuni file in uso cliccare 'Ignore'.$\n$\nAl termine dell'installazione verra' mostrata la finestra 'MPLAB IDE Document Select'. Chiuderla cliccando sulla X rossa in alto a destra"

  SetOutPath "$INSTDIR\DSP"
  File ".\DSP\Data1.cab"
  File ".\DSP\ISSetup.dll"
  File ".\DSP\MPLAB Tools v8.46.msi"
  File ".\DSP\mplabcert.bmp"
  File ".\DSP\setup.exe" 
  SetOutPath $INSTDIR 
   ExecWait ".\DSP\setup.exe" 

MessageBox MB_OK "Verra' ora lanciata l'installazione dell'ambiente di MPLAB X, necessario per utilizzare il Pickit4.$\nE' importante che l'installazione venga portata a termine lasciando immutata la directory di installazione di default.\nInstallare solo 'MPLAB IPE' ed '16-bit MCUs'"
   
  SetOutPath "$INSTDIR\MPLABX"
  File ".\MPLABX\MPLABX-v6.15-windows-installer.exe" 
  SetOutPath $INSTDIR 
   ExecWait ".\MPLABX\MPLABX-v6.15-windows-installer.exe"

MessageBox MB_OK "Verranno ora copiati nella directory di installazione i driver CAN completi$\nQualora si riscontrino problemi nel riconoscimento dell'ESD-CAN da parte di Windows$\nsi possono utilizzare tali driver per aggionrare il dispositivo da Device Manager"
  SetOutPath "$INSTDIR\drivers_can"
  File /r ".\drivers_can\Win9x\*"
  File /r ".\drivers_can\Win32\*"
  File /r ".\drivers_can\Win64\*"
  File /r ".\drivers_can\WinNT\*"

SectionEnd



; The stuff to install
Section "2FOC TEST sw (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  ; Put file there

  SetOutPath $INSTDIR

; Process NSIS plugin in required  

 ; Processes::KillProcess "snmload"

File "test_corto.bmp"
File "test_loop.bmp"
File "switches.bmp"
File "led_ar_ch1.bmp"
File "led_ar_ch2.bmp"
File "led_ve_ch1.bmp"
File "led_ve_ch2.bmp"
File "load.bmp"
File "led_off_ch1.bmp"
File "led_off_ch2.bmp"
File "12v_dcdc_signal.bmp"
File "12v_dcdc.bmp"
File "5v_dcdc_signal.bmp"
File "5v_dcdc.bmp"
File "tpv12.bmp"
File "tpv3.bmp"
File "tpv5.bmp"
File "jtag-program-bootloader.bmp"
File "jtag1.bmp"
File "jtag2.bmp"
File "jig-phase1.bmp"
File "jig-phase2.bmp"
File "jigcan.bmp"
File "jig.bmp"
File "clk.bmp"
File "iit.bmp"
File "canterm.bmp"
File "ACE.dll"
File "autosave.gup"
File "Bootloader1.hex"
File "Bootloader2.hex"
File "bootloader_PK4_test1.bat"
File "bootloader_PK4_test2.bat"
File "bootloader_PK4.bat"
File "change-canid-1.bat"
File "change-canid-2.bat"
File "2FOC1.hex"
File "2FOC2.hex"
File "can.ini"
File "canload1.bat"
File "canload2.bat"
File "canLoader.exe"
File "firmware-1.bat"
File "firmware-2.bat"
File "config.txt"
File "fase1.xml"
File "Files.dll"
File "freetype6.dll"
File "graph.exe"
File "graph.ini"
File "gulp.bat"
File "gulp.conf1"
File "gulp.conf2"
File "gulp_icon.xpm"
File "icd3load.bat"
File "icd3rst.bat"
File "installer.nsi"
File "intl.dll"
File "libatk-1.0-0.dll"
File "libcairo-2.dll"
File "libexpat-1.dll"
File "libfontconfig-1.dll"
File "libfreetype-6.dll"
File "libgdk-win32-2.0-0.dll"
File "libgdk_pixbuf-2.0-0.dll"
File "libgio-2.0-0.dll"
File "libglib-2.0-0.dll"
File "libgmodule-2.0-0.dll"
File "libgobject-2.0-0.dll"
File "libgthread-2.0-0.dll"
File "libgtk-win32-2.0-0.dll"
File "libpango-1.0-0.dll"
File "libpangocairo-1.0-0.dll"
File "libpangoft2-1.0-0.dll"
File "libpangowin32-1.0-0.dll"
File "libpng14-14.dll"
File "SDL.dll"
File "SDL_ttf.dll"
File "serial.ini"
File "STBLLIB.dll"
File "test.ico"
File "TestUI.exe"
File "WinIo.dll"
File "zlib1.dll"
File "WinIo32.dll"
File "wpcap.dll"
File "Packet.dll"
File "canreal.exe"
File "canreal.cspini"
File "2foc_commands_DS402.cspsl"
File "can.bmp"
File "canfilata.bmp"
File "cantermjump.bmp"
File "canu2292.inf"
File "canu331.inf"
File "fase2.xml"
File "etichetta.bmp"
File "jp3_jp4.bmp"
File "jp6_jp7.bmp"
File "jpopen1.bmp"
File "jpopen2.bmp"
File "led5vcan.bmp"
File "led5vspi.bmp"
File "ledv.bmp"
File "mclr.bmp"
File "p1.bmp"
File "p2.bmp"
File "p5.bmp"
File "p6.bmp"
File "jig-board-configuration-phase2.bmp"
File "jig-configuration-phase1.bmp"
File "jig-with-needles.bmp"
FIle "can-connector-j1-phase2.bmp"


SetOutPath $INSTDIR\icub-firmware-build
File "icub-firmware-build\setup-repo.bat"

SetOutPath $INSTDIR\setCanId
File "setCanId\setCanId.exe"

SetOutPath $INSTDIR
   
;  SetOutPath $INSTDIR\wipfw
  
 ; File "wipfw\uninstall.cmd"
 ; File "wipfw\README.TXT"
  ;File "wipfw\configsnm.cmd"
 ; File "wipfw\install.cmd"
  
 ; SetOutPath $INSTDIR\wipfw\bin
  
  ;File "wipfw\bin\ip_fw-allow.sys"
  ;File "wipfw\bin\ip_fw-deny.sys"
  ;File "wipfw\bin\ipfw.exe"
  ;File "wipfw\bin\loadrules.cmd"
  
   
 ; DeleteRegValue HKLM "SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" "ArpRetryCount"
 ; WriteRegDWORD HKLM "SYSTEM\CurrentControlSet\Services\Tcpip\Parameters" "ArpRetryCount" 0x0
 ; WriteRegStr HKLM  "Software\Microsoft\Windows\CurrentVersion\run\" "NeuroNetwork" "$INSTDIR\snmload.exe"

  
  ; Write the installation path into the registry
;  WriteRegStr HKLM SOFTWARE\2FOCTEST "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\2FOCTEST" "DisplayName" "2FOCTEST"
;  WriteRegStr HKLM "Software\Neuro" "Path" "$INSTDIR"
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\2FOCTEST" "UninstallString" '"$INSTDIR\uninstall.exe"'
  
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\2FOCTEST" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\2FOCTEST" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
   
 ;  ExecWait "$INSTDIR\snmload.exe"
;ExecWait "$INSTDIR\vcredist_x86.exe"
       
   MessageBox MB_YESNO "Install Visaul C++ 2008 runtime?" /SD IDYES IDNO endVCR
 
  File ".\vcredist_x86.exe"
  ExecWait "$INSTDIR\vcredist_x86.exe"      
endVCR:

   MessageBox MB_YESNO "Install Visual C++ 2010 runtime?" /SD IDYES IDNO endVCR10
 
  File ".\vcredist_x86_2010.exe"
  ExecWait "$INSTDIR\vcredist_x86_2010.exe"        
endVCR10:

   MessageBox MB_YESNO "Install Git for Windows?" /SD IDYES IDNO endGIT
 
  File "GIT\Git-2.49.0-64-bit.exe"
  ExecWait "$INSTDIR\Git-2.49.0-64-bit.exe"      
endGIT:

SectionEnd

;Section "Stimulation example files"
 ;CreateDirectory "$INSTDIR\Stimoli"
; SetOutPath $INSTDIR\Stimoli
  
  ; Put file there

;  File "Stimoli\100ms.stim"
;  File "Stimoli\bifasic.stlib"
;  File "Stimoli\library.stim"
;  File "Stimoli\Longbifasic.stim"
;  File "Stimoli\spike.stlib"
;  File "Stimoli\Spikes.stim"
;  File "Stimoli\square.stlib"
;  File "Stimoli\testbifasic.stim"
 ; StrCpy $R0  1
  

;SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  SetOutPath $INSTDIR\ ; set CWD for exe
  CreateDirectory "$SMPROGRAMS\2FOCTEST"

;'  ${If} $R0 == '1'
  
;	  CreateShortCut "$SMPROGRAMS\Neuro\Stimoli.lnk" "$INSTDIR\Stimoli" "" "$INSTDIR\Stimoli" 0
	
  ;${Endif}
  CreateShortCut "$SMPROGRAMS\2FOCTEST\2FOCTEST.lnk" "$INSTDIR\TESTUI.exe" "" "$INSTDIR\TESTUI.exe" 0
  CreateShortCut "$SMPROGRAMS\2FOCTEST\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0



;MessageBox MB_OK "It's NECESSARY to reboot PC$\nPress OK to reboot NOW!" 
 ; Reboot

  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"


; Process NSIS plugin in required  

;  Processes::KillProcess "snmload"

  ; Remove registry keys
 ;   ExecWait "$INSTDIR\wipfw\uninstall.cmd"
 ; DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Neuro"
 ; DeleteRegKey HKLM SOFTWARE\Neuro
 ; DeleteRegValue HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" "NeuroNetwork" 

  Delete $INSTDIR\*.*
 ; Delete $INSTDIR\Stimoli\*.*
 ; Delete $INSTDIR\wipfw\bin\*.*
 ; Delete $INSTDIR\wipfw\*.*

  Delete "$INSTDIR\2FOCTEST\*.*"
  
  RMDir /r "$INSTDIR\icub-firmware-build"
  RMDir "$SMPROGRAMS\2FOCTEST"
 ; RMDir "$INSTDIR\Stimoli"
 ; RMDir "$INSTDIR\wipfw\bin"
 ; RMDir "$INSTDIR\wipfw"
  RMDir /r "$INSTDIR"
  

SectionEnd
