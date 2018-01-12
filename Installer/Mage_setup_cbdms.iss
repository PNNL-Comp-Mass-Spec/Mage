; This is an Inno Setup configuration file
; http://www.jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\DeployedFiles\MageFileProcessor\Mage.dll')

[CustomMessages]
AppName=Mage_CBDMS
[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence
[Files]

Source: ..\DeployedFiles\CBDMS\MageConcatenator.exe.config              ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\ICSharpCode.SharpZipLib.dll   ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Jayrock.Json.dll              ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Mage.dll                      ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageDisplayLib.dll            ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageUIComponents.dll          ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MyEMSLReader.dll              ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Pacifica.Core.dll             ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\PRISM.dll                     ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\ShFolderBrowser.dll           ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Data.SQLite.dll        ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageConcatenator.exe          ; DestDir: {app}\MageConcatenator

Source: ..\DeployedFiles\MageConcatenator\ReadMe.txt                    ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Logging.config                ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\QueryDefinitions.xml          ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\x64\SQLite.Interop.dll        ; DestDir: {app}\MageConcatenator\x64
Source: ..\DeployedFiles\MageConcatenator\x86\SQLite.Interop.dll        ; DestDir: {app}\MageConcatenator\x86

Source: ..\DeployedFiles\CBDMS\MageExtractor.exe.config                 ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\ICSharpCode.SharpZipLib.dll      ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Jayrock.Json.dll                 ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Mage.dll                         ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageDisplayLib.dll               ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtContentFilters.dll        ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtExtractionFilters.dll     ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageUIComponents.dll             ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MyEMSLReader.dll                 ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Pacifica.Core.dll                ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\PRISM.dll                        ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\ShFolderBrowser.dll              ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Data.SQLite.dll           ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtractor.exe                ; DestDir: {app}\MageExtractor

Source: ..\DeployedFiles\MageExtractor\ReadMe.txt                       ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Logging.config                   ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\QueryDefinitions.xml             ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\x64\SQLite.Interop.dll           ; DestDir: {app}\MageExtractor\x64
Source: ..\DeployedFiles\MageExtractor\x86\SQLite.Interop.dll           ; DestDir: {app}\MageExtractor\x86

Source: ..\DeployedFiles\CBDMS\MageFilePackager.exe.config              ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\ICSharpCode.SharpZipLib.dll   ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Jayrock.Json.dll              ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Mage.dll                      ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageDisplayLib.dll            ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageUIComponents.dll          ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MyEMSLReader.dll              ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Pacifica.Core.dll             ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\PRISM.dll                     ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\ShFolderBrowser.dll           ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Data.SQLite.dll        ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageFilePackager.exe          ; DestDir: {app}\MageFilePackager

Source: ..\DeployedFiles\MageFilePackager\ReadMe.txt                    ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Logging.config                ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\QueryDefinitions.xml          ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\x64\SQLite.Interop.dll        ; DestDir: {app}\MageFilePackager\x64
Source: ..\DeployedFiles\MageFilePackager\x86\SQLite.Interop.dll        ; DestDir: {app}\MageFilePackager\x86

Source: ..\DeployedFiles\CBDMS\MageFileProcessor.exe.config             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ICSharpCode.SharpZipLib.dll  ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Jayrock.Json.dll             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Mage.dll                     ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageDisplayLib.dll           ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageExtContentFilters.dll    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageExtExtractionFilters.dll ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageUIComponents.dll         ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MyEMSLReader.dll             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Pacifica.Core.dll            ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\PRISM.dll                    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ShFolderBrowser.dll          ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Data.SQLite.dll       ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageFileProcessor.exe        ; DestDir: {app}\MageFileProcessor

Source: ..\DeployedFiles\MageFileProcessor\ReadMe.txt                   ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Logging.config               ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\QueryDefinitions.xml         ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ColumnMapping.txt            ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\x64\SQLite.Interop.dll       ; DestDir: {app}\MageFileProcessor\x64
Source: ..\DeployedFiles\MageFileProcessor\x86\SQLite.Interop.dll       ; DestDir: {app}\MageFileProcessor\x86

Source: ..\DeployedFiles\CBDMS\MageMetadataProcessor.exe.config         ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\ICSharpCode.SharpZipLib.dll          ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Jayrock.Json.dll         ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Mage.dll                 ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageDisplayLib.dll       ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageUIComponents.dll     ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MyEMSLReader.dll         ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Pacifica.Core.dll        ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\PRISM.dll                ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\ShFolderBrowser.dll      ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Data.SQLite.dll   ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageMetadataProcessor.exe; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\ReadMe.txt               ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\QueryDefinitions.xml     ; DestDir: {app}\MageMetaDataProcessor

Source: ..\DeployedFiles\Ranger\ICSharpCode.SharpZipLib.dll             ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Jayrock.Json.dll                        ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Mage.dll                                ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MageDisplayLib.dll                      ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MageUIComponents.dll                    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MyEMSLReader.dll                        ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Pacifica.Core.dll                       ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\PRISM.dll                               ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\ShFolderBrowser.dll                     ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\RangerLib.dll                           ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Data.SQLite.dll                  ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Ranger.exe                              ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\ReadMe.txt                              ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\QueryDefinitions.xml                    ; DestDir: {app}\Ranger

Source: ..\MageExtractor\Extractor.ico                                  ; DestDir: {app}\MageExtractor
Source: ..\MageFileProcessor\wand.ico                                   ; DestDir: {app}\MageFileProcessor

Source: Images\textdoc.ico                   ; DestDir: {app}
Source: Images\delete_16x.ico                ; DestDir: {app}
Source: ..\README.md                         ; DestDir: {app}
Source: ..\RevisionHistory.txt               ; DestDir: {app}

[Dirs]
Name: {commonappdata}\Mage; Flags: uninsalwaysuninstall

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
; Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked

[Icons]
Name: {group}\Mage Extractor; Filename: {app}\MageExtractor\MageExtractor.exe; IconFilename: {app}\MageExtractor\Extractor.ico; IconIndex: 0; Comment: Mage Extractor
Name: {group}\Mage File Processor; Filename: {app}\MageFileProcessor\MageFileProcessor.exe; IconFilename: {app}\MageFileProcessor\wand.ico; IconIndex: 0; Comment: Mage File Processor
Name: {group}\Mage Concatenator; Filename: {app}\MageConcatenator\MageConcatenator.exe; Comment: Mage Concatenator
Name: {group}\Mage Metadata Processor; Filename: {app}\MageMetadataProcessor\MageMetadataProcessor.exe; Comment: Mage Metadata Processor
Name: {group}\Ranger; Filename: {app}\Ranger\Ranger.exe; Comment: Ranger
Name: {group}\ReadMe File; Filename: {app}\README.md; IconFilename: {app}\textdoc.ico; IconIndex: 0; Comment: Mage ReadMe
Name: {group}\Uninstall Mage; Filename: {uninstallexe}; IconFilename: {app}\delete_16x.ico; IconIndex: 0

Name: {commondesktop}\Mage Extractor (CBDMS); Filename: {app}\MageExtractor\MageExtractor.exe; Tasks: desktopicon; IconFilename: {app}\MageExtractor\Extractor.ico; IconIndex: 0; Comment: Mage Extractor
Name: {commondesktop}\Mage File Processor (CBDMS); Filename: {app}\MageFileProcessor\MageFileProcessor.exe; Tasks: desktopicon; IconFilename: {app}\MageFileProcessor\wand.ico; IconIndex: 0; Comment: Mage File Processor

[Setup]
AppName=Mage_CBDMS
AppVersion={#ApplicationVersion}
;AppVerName=Mage_CBDMS
AppID=MageIdCBDMS
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=http://omics.pnl.gov/software
AppSupportURL=http://omics.pnl.gov/software
AppUpdatesURL=http://omics.pnl.gov/software
DefaultDirName={pf}\Mage_CBDMS
DefaultGroupName=Mage (CBDMS)
AppCopyright=© PNNL
;LicenseFile=.\License.rtf
PrivilegesRequired=poweruser
OutputBaseFilename=MageInstaller_CBDMS
;VersionInfoVersion=1.57
VersionInfoVersion={#ApplicationVersion}
VersionInfoCompany=PNNL
VersionInfoDescription=Mage (CBDMS)
VersionInfoCopyright=PNNL
DisableFinishedPage=true
ShowLanguageDialog=no
SetupIconFile=..\MageFileProcessor\wand.ico
InfoBeforeFile=.\readme.rtf
ChangesAssociations=false
;WizardImageFile=..\Deploy\Images\MageSetupSideImage.bmp
;WizardSmallImageFile=..\Deploy\Images\MageSetupSmallImage.bmp
InfoAfterFile=.\postinstall.rtf
EnableDirDoesntExistWarning=false
AlwaysShowDirOnReadyPage=true
UninstallDisplayIcon={app}\delete_16x.ico
ShowTasksTreeLines=true
OutputDir=.\Output
[Registry]
;Root: HKCR; Subkey: MageFile; ValueType: string; ValueName: ; ValueData:Mage File; Flags: uninsdeletekey
;Root: HKCR; Subkey: MageSetting\DefaultIcon; ValueType: string; ValueData: {app}\wand.ico,0; Flags: uninsdeletevalue
[UninstallDelete]
Name: {app}; Type: filesandordirs
Name: {app}\MageConcatenator; Type: filesandordirs
Name: {app}\MageExtractor; Type: filesandordirs
Name: {app}\MageFileProcessor; Type: filesandordirs
Name: {app}\MageMetadataProcessor; Type: filesandordirs
Name: {app}\Ranger; Type: filesandordirs

