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

!define APPNAME "Radegast"
!define VERSION "2.7"
!define MAINEXEC "${APPNAME}.exe"
!define DOTNET_VERSION "3.5"
!define VOICEPACK "RadegastVoicepack-1.0.exe"
!define UNINST_REG "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

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
  File /r /x *.nsi /x *.bak /x *.mdb /x *.application /x *vshost*.* /x *installer*.* /x *.so /x *.dylib *.*
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\${APPNAME}" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM ${UNINST_REG} "DisplayName" "${APPNAME} ${VERSION}"
  WriteRegStr HKLM ${UNINST_REG} "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM ${UNINST_REG} "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegStr HKLM ${UNINST_REG} "Publisher" "Radegast Development Team"
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
  NSISdl::download /TIMEOUT=30000  "http://radegast.googlecode.com/files/${VOICEPACK}" "$INSTDIR\${VOICEPACK}"
  Pop $R0
  StrCmp $R0 "success" voice_download_success voice_download_failed
  
  voice_download_success:
	ExecWait '"$INSTDIR\${VOICEPACK}" /D=$INSTDIR' $0
    Delete "$INSTDIR\${VOICEPACK}"
    goto voice_download_end

  voice_download_failed:
    MessageBox MB_OK "Download failed: $R0. Skipping installation of voice."
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
