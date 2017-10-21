;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Includes
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
!include "LogicLib.nsh"
!include WordFunc.nsh
!include "FileFunc.nsh"
!insertmacro VersionCompare

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Compiler flags
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
SetOverwrite on				; overwrite files
SetCompress auto			; compress iff saves space
SetCompressor /solid lzma	; compress whole installer as one block
SetDatablockOptimize off	; only saves us 0.1%, not worth it
XPStyle on                  ; add an XP manifest to the installer
RequestExecutionLevel admin	; on Vista we must be admin because we write to Program Files

LangString LanguageCode ${LANG_ENGLISH}  "en"
!define DOTNET_URL "https://download.microsoft.com/download/F/9/4/F942F07D-F26F-4F30-B4E3-EBD54FABA377/NDP462-KB3151800-x86-x64-AllOS-ENU.exe"
!define MSI31_URL "http://download.microsoft.com/download/1/4/7/147ded26-931c-4daf-9095-ec7baf996f46/WindowsInstaller-KB893803-v2-x86.exe"

!define APPNAME "Radegast"
!define VERSION "2.25"
!define MAINEXEC "${APPNAME}.exe"
!define DOTNET_VERSION "4.5"
!define VOICEPACK "RadegastVoicepack-2.0.exe"
!define UNINST_REG "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

; The name of the installer
Name "${APPNAME}"
BrandingText "Made by a Cinder"

; The file to write
OutFile "..\${APPNAME}-${VERSION}-installer.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\${APPNAME}"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\${APPNAME}" "Install_Dir"

;--------------------------------

LicenseText "Please review the license terms before installing Radegast"
LicenseData "license.txt"

; Pages
Page license
Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; Check MSI and .NET
Section ".NET check"
  ; Comment out below if not mandatory
  ; SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $TEMP
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.5" "SP"
  ${If} $0 < "1"
	goto CheckMSI
  ${EndIf}
  goto NewDotNET

  CheckMSI:
  ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
  ;                               MSI                                          ;
  ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
  GetDLLVersion "$SYSDIR\msi.dll" $R0 $R1
  IntOp $R2 $R0 / 0x00010000 ; $R2 now contains major version
  IntOp $R3 $R0 & 0x0000FFFF ; $R3 now contains minor version
  IntOp $R4 $R1 / 0x00010000 ; $R4 now contains release
  IntOp $R5 $R1 & 0x0000FFFF ; $R5 now contains build
  StrCpy $0 "$R2.$R3.$R4.$R5" ; $0 now contains string like "1.2.0.192"
  DetailPrint "MSI version $0"
 
  ${If} $R2 < '3'
    goto AskMSI
  ${ElseIf} $R2 == '3'
    DetailPrint "MSI3.x"
	${If} $R3 < '1'
	  goto AskMSI
    ${EndIf}
  ${Else}
    DetailPrint "MSI3.1 already installed"
    goto DownloadDotNET
  ${EndIf}

  AskMSI:
    SetOutPath "$TEMP"
    SetOverwrite on
 
    MessageBox MB_YESNOCANCEL|MB_ICONEXCLAMATION \
    "Your MSI version: $0.$\nRequired Version: 3.1 or greater.$\nDownload MSI version from www.microsoft.com?" \
    /SD IDYES IDYES DownloadMSI IDNO DownloadDotNET
    goto GiveUpDotNET ;IDCANCEL
 
  DownloadMSI:
    DetailPrint "Beginning download of MSI3.1."
    NSISDL::download ${MSI31_URL} "$TEMP\WindowsInstaller-KB893803-v2-x86.exe"
    DetailPrint "Completed download."
    Pop $0
    ${If} $0 == "cancel"
      MessageBox MB_YESNO|MB_ICONEXCLAMATION \
      "Download cancelled.  Continue Installation?" \
      IDYES DownloadDotNET IDNO GiveUpDotNET
    ${ElseIf} $0 != "success"
      MessageBox MB_YESNO|MB_ICONEXCLAMATION \
      "Download failed:$\n$0$\n$\nContinue Installation?" \
      IDYES DownloadDotNET IDNO GiveUpDotNET
    ${EndIf}
    DetailPrint "Pausing installation while downloaded MSI3.1 installer runs."
    ExecWait '$TEMP\WindowsInstaller-KB893803-v2-x86.exe /quiet /norestart' $0
    DetailPrint "Completed MSI3.1 install/update. Exit code = '$0'. Removing MSI3.1 installer."
    Delete "$TEMP\WindowsInstaller-KB893803-v2-x86.exe"
    DetailPrint "MSI3.1 installer removed."
    goto DownloadDotNET

  DownloadDotNET:
    DetailPrint "Beginning download of .NET 4.6.2."
    NSISdl::download /TIMEOUT=30000 ${DOTNET_URL} "$TEMP\dotNetFx462_Full_setup.exe" /END
    Pop $0
    DetailPrint "Result: $0"
    StrCmp $0 "success" InstallDotNet
    StrCmp $0 "cancel" GiveUpDotNET
    NSISdl::download /TIMEOUT=30000 /NOPROXY ${DOTNET_URL} "$TEMP\dotNetFx462_Full_setup.exe" /END
    Pop $0
    DetailPrint "Result: $0"
    StrCmp $0 "success" InstallDotNet
 
    MessageBox MB_ICONSTOP "Download failed: $0"
    goto NewDotNET

   GiveUpDotNET:
     DetailPrint "Installation cancelled by user."
	 goto NewDotNET

  InstallDotNet:
    DetailPrint "Completed download."
    Pop $0
    ${If} $0 == "cancel"
      MessageBox MB_YESNO|MB_ICONEXCLAMATION \
      "Download cancelled.  Continue Installation?" \
      IDYES NewDotNET IDNO GiveUpDotNET
    ${EndIf}
    DetailPrint "Pausing installation while downloaded .NET Framework installer runs."
	MessageBox MB_OKCANCEL "Setup will now install .NET Framework$\nThis will take a while." \
	  IDOK +1 IDCANCEL GiveUpDotNET
    ExecWait '$TEMP\dotNetFx462_Full_setup.exe /q /norestart /c:"install /q"'
    DetailPrint "Completed .NET Framework install/update. Removing .NET Framework installer."
    Delete "$TEMP\dotNetFx462_Full_setup.exe"
    DetailPrint ".NET Framework installer removed."
	goto NewDotNET

  NewDotNET:
    DetailPrint ".NET Framework check complete."

SectionEnd


;--------------------------------
; The stuff to install
; Main Section
;--------------------------------
Section "${APPNAME} core (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Put file there
  File /r /x *.nsi /x *.bak /x *.mdb /x *.application /x *vshost*.* /x *installer*.* /x *.cs /x *.csproj /x *.so /x *.dylib *.*
  ;File Radegast.exe

  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\${APPNAME}" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM ${UNINST_REG} "DisplayName" "${APPNAME} ${VERSION}"
  WriteRegStr HKLM ${UNINST_REG} "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM ${UNINST_REG} "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegStr HKLM ${UNINST_REG} "Publisher" "Cinder Roxley"
  WriteRegStr HKLM ${UNINST_REG} "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM ${UNINST_REG} "DisplayIcon" "$INSTDIR\${MAINEXEC}"
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "${UNINST_REG}" "EstimatedSize" "$0"
  WriteRegDWORD HKLM ${UNINST_REG} "NoModify" 1
  WriteRegDWORD HKLM ${UNINST_REG} "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

;--------------------------------
; Voice pack download and install
Section /o "${APPNAME} Voice Pack (extra download)"
  AddSize 6662
  IfFileExists "$INSTDIR\SLVoice.exe" voice_download_exists

  DetailPrint "Beginning download of Vivox Voicepack."
  NSISdl::download /TIMEOUT=30000 "http://downloads.radegast.life/${VOICEPACK}" "$INSTDIR\${VOICEPACK}" /END
  Pop $0
  DetailPrint "Result: $0"
  StrCmp $0 "success" voice_download_success
  StrCmp $0 "cancel" voice_download_cancelled
  NSISdl::download /TIMEOUT=30000 /NOPROXY "http://downloads.radegast.life/${VOICEPACK}" "$INSTDIR\${VOICEPACK}" /END
  Pop $0
  DetailPrint "Result: $0"
  StrCmp $0 "success" voice_download_success
 
  MessageBox MB_ICONSTOP "Download failed: $0"
  goto voice_download_end

  voice_download_success:
	ExecWait '"$INSTDIR\${VOICEPACK}" /D=$INSTDIR' $0
    Delete "$INSTDIR\${VOICEPACK}"
    goto voice_download_end

  voice_download_cancelled:
    DetailPrint "Installation cancelled by user."
	goto voice_download_end
  
  voice_download_exists:
    DetailPrint  "Voice pack already present, skipping the install"
	goto voice_download_end

  voice_download_end:
SectionEnd

;--------------------------------
; Start menu
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\${APPNAME}"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "$INSTDIR\${MAINEXEC}" "" "$INSTDIR\${MAINEXEC}" 0
  
SectionEnd

;--------------------------------
; Desktop shortcut
Section "Desktop shortcut"

  CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\${MAINEXEC}" "" "$INSTDIR\${MAINEXEC}" 0

SectionEnd

;--------------------------------
; Uninstaller

Section "Uninstall"
  IfFileExists "$INSTDIR\uninstall_voice.exe" +1 +2
  ExecWait '"$INSTDIR\uninstall_voice.exe" /S' $0
  
  ; Remove registry keys
  DeleteRegKey HKLM ${UNINST_REG}
  DeleteRegKey HKLM "SOFTWARE\${APPNAME}"

  ; Remove files and uninstaller
  Delete $INSTDIR\*.*
  Delete $INSTDIR\aiml\*.*
  RMDir $INSTDIR\aiml
  Delete $INSTDIR\aiml_config\*.*
  RMDir $INSTDIR\aiml_config
  Delete $INSTDIR\character\*.*
  RMDir $INSTDIR\character
  Delete $INSTDIR\openmetaverse_data\static_assets\*.*
  RMDir $INSTDIR\openmetaverse_data\static_assets
  RMDir $INSTDIR\openmetaverse_data
  Delete $INSTDIR\openmetaverse_data\*.*
  RMDir $INSTDIR\openmetaverse_data
  Delete $INSTDIR\shader_data\*.*
  RMDir $INSTDIR\shader_data
  
  ; Remove shortcuts, if any
  Delete "$DESKTOP\${APPNAME}.lnk"
  Delete "$SMPROGRAMS\${APPNAME}\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\${APPNAME}"
  RMDir "$INSTDIR"

SectionEnd
