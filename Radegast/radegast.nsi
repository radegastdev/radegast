;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Includes
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
!include "LogicLib.nsh"
!include WordFunc.nsh
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

!define APPNAME "Radegast"
!define VERSION "1.27"
!define MAINEXEC "${APPNAME}.exe"
!define DOTNET_VERSION "3.5"

; The name of the installer
Name "${APPNAME}"

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

; The stuff to install
Section "${APPNAME} core (required)"
	
  
  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Put file there
  File /r /x *.nsi /x *.bak /x *.mdb /x *.pdb /x *.application /x *vshost*.* /x *installer*.* /x *.so /x *.dylib *.*
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\${APPNAME}" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
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
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
  DeleteRegKey HKLM "SOFTWARE\${APPNAME}"

  ; Remove files and uninstaller
  Delete $INSTDIR\*.*
  Delete $INSTDIR\aiml\*.*
  RMDir $INSTDIR\aiml
  Delete $INSTDIR\aiml_config\*.*
  RMDir $INSTDIR\aiml_config
  Delete $INSTDIR\openmetaverse_data\*.*
  RMDir $INSTDIR\openmetaverse_data
  
  ; Remove shortcuts, if any
  Delete "$DESKTOP\${APPNAME}.lnk"
  Delete "$SMPROGRAMS\${APPNAME}\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\${APPNAME}"
  RMDir "$INSTDIR"

SectionEnd
