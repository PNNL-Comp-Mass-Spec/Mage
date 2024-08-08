; This is an Inno Setup configuration file
; https://jrsoftware.org/isinfo.php

#define ApplicationVersion GetFileVersion('..\DeployedFiles\MageFileProcessor\Mage.dll')

[CustomMessages]
AppName=Mage

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.
; Example with multiple lines:
; WelcomeLabel2=Welcome message%n%nAdditional sentence

[Files]
Source: ..\DeployedFiles\MageConcatenator\MageConcatenator.exe                          ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageConcatenator.exe.config                   ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Mage.dll                                      ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageDisplayLib.dll                            ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MageUIComponents.dll                          ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\MyEMSLReader.dll                              ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Newtonsoft.Json.dll                           ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Npgsql.dll                                    ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\Pacifica.dll                                  ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\PRISM.dll                                     ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\PRISMDatabaseUtils.dll                        ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\ShFolderBrowser.dll                           ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Buffers.dll                            ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Collections.Immutable.dll              ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Data.SQLite.dll                        ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Memory.dll                             ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Numerics.Vectors.dll                   ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Text.Encodings.Web.dll                 ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Text.Json.dll                          ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Threading.Channels.dll                 ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\System.ValueTuple.dll                         ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\x64\SQLite.Interop.dll                        ; DestDir: {app}\MageConcatenator\x64
Source: ..\DeployedFiles\MageConcatenator\x86\SQLite.Interop.dll                        ; DestDir: {app}\MageConcatenator\x86

Source: ..\DeployedFiles\MageConcatenator\ReadMe.txt                                    ; DestDir: {app}\MageConcatenator
Source: ..\DeployedFiles\MageConcatenator\QueryDefinitions.xml                          ; DestDir: {app}\MageConcatenator


Source: ..\DeployedFiles\MageExtractor\MageExtractor.exe                             ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtractor.exe.config                      ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Mage.dll                                      ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageDisplayLib.dll                            ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtContentFilters.dll                     ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageExtExtractionFilters.dll                  ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MageUIComponents.dll                          ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\MyEMSLReader.dll                              ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Newtonsoft.Json.dll                           ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Npgsql.dll                                    ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\Pacifica.dll                                  ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\PRISM.dll                                     ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\PRISMDatabaseUtils.dll                        ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\ShFolderBrowser.dll                           ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Buffers.dll                            ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Collections.Immutable.dll              ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Data.SQLite.dll                        ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Memory.dll                             ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Numerics.Vectors.dll                   ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Text.Encodings.Web.dll                 ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Text.Json.dll                          ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Threading.Channels.dll                 ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\System.ValueTuple.dll                         ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\x64\SQLite.Interop.dll                        ; DestDir: {app}\MageExtractor\x64
Source: ..\DeployedFiles\MageExtractor\x86\SQLite.Interop.dll                        ; DestDir: {app}\MageExtractor\x86

Source: ..\DeployedFiles\MageExtractor\ReadMe.txt                                    ; DestDir: {app}\MageExtractor
Source: ..\DeployedFiles\MageExtractor\QueryDefinitions.xml                          ; DestDir: {app}\MageExtractor


Source: ..\DeployedFiles\MageFilePackager\MageFilePackager.exe                          ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageFilePackager.exe.config                   ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Mage.dll                                      ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageDisplayLib.dll                            ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MageUIComponents.dll                          ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\MyEMSLReader.dll                              ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Newtonsoft.Json.dll                           ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Npgsql.dll                                    ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\Pacifica.dll                                  ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\PRISM.dll                                     ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\PRISMDatabaseUtils.dll                        ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\ShFolderBrowser.dll                           ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Buffers.dll                            ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Collections.Immutable.dll              ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Data.SQLite.dll                        ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Memory.dll                             ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Numerics.Vectors.dll                   ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Text.Encodings.Web.dll                 ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Text.Json.dll                          ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Threading.Channels.dll                 ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\System.ValueTuple.dll                         ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\x64\SQLite.Interop.dll                        ; DestDir: {app}\MageFilePackager\x64
Source: ..\DeployedFiles\MageFilePackager\x86\SQLite.Interop.dll                        ; DestDir: {app}\MageFilePackager\x86

Source: ..\DeployedFiles\MageFilePackager\ReadMe.txt                                    ; DestDir: {app}\MageFilePackager
Source: ..\DeployedFiles\MageFilePackager\QueryDefinitions.xml                          ; DestDir: {app}\MageFilePackager


Source: ..\DeployedFiles\MageFileProcessor\MageFileProcessor.exe                         ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageFileProcessor.exe.config                  ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Mage.dll                                      ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageDisplayLib.dll                            ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageExtContentFilters.dll                     ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageExtExtractionFilters.dll                  ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MageUIComponents.dll                          ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Newtonsoft.Json.dll                           ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\MyEMSLReader.dll                              ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Npgsql.dll                                    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\Pacifica.dll                                  ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\PRISM.dll                                     ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\PRISMDatabaseUtils.dll                        ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ShFolderBrowser.dll                           ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Buffers.dll                            ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Collections.Immutable.dll              ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Data.SQLite.dll                        ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Memory.dll                             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Numerics.Vectors.dll                   ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Text.Encodings.Web.dll                 ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Text.Json.dll                          ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Threading.Channels.dll                 ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\System.ValueTuple.dll                         ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\x64\SQLite.Interop.dll                        ; DestDir: {app}\MageFileProcessor\x64
Source: ..\DeployedFiles\MageFileProcessor\x86\SQLite.Interop.dll                        ; DestDir: {app}\MageFileProcessor\x86

Source: ..\DeployedFiles\MageFileProcessor\ReadMe.txt                                    ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\ColumnMapping.txt                             ; DestDir: {app}\MageFileProcessor
Source: ..\DeployedFiles\MageFileProcessor\QueryDefinitions.xml                          ; DestDir: {app}\MageFileProcessor


Source: ..\DeployedFiles\MageMetaDataProcessor\MageMetadataProcessor.exe                     ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageMetadataProcessor.exe.config              ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Mage.dll                                      ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageDisplayLib.dll                            ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MageUIComponents.dll                          ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Newtonsoft.Json.dll                           ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\MyEMSLReader.dll                              ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Npgsql.dll                                    ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\Pacifica.dll                                  ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\PRISM.dll                                     ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\PRISMDatabaseUtils.dll                        ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\ShFolderBrowser.dll                           ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Buffers.dll                            ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Collections.Immutable.dll              ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Data.SQLite.dll                        ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Memory.dll                             ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Numerics.Vectors.dll                   ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Text.Encodings.Web.dll                 ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Text.Json.dll                          ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Threading.Channels.dll                 ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\System.ValueTuple.dll                         ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\x64\SQLite.Interop.dll                        ; DestDir: {app}\MageMetaDataProcessor\x64
Source: ..\DeployedFiles\MageMetaDataProcessor\x86\SQLite.Interop.dll                        ; DestDir: {app}\MageMetaDataProcessor\x86

Source: ..\DeployedFiles\MageMetaDataProcessor\ReadMe.txt                                    ; DestDir: {app}\MageMetaDataProcessor
Source: ..\DeployedFiles\MageMetaDataProcessor\QueryDefinitions.xml                          ; DestDir: {app}\MageMetaDataProcessor


Source: ..\DeployedFiles\Ranger\Ranger.exe                                    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\ICSharpCode.SharpZipLib.dll                   ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Mage.dll                                      ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MageDisplayLib.dll                            ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MageUIComponents.dll                          ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Microsoft.Bcl.AsyncInterfaces.dll             ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Microsoft.Bcl.HashCode.dll                    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Microsoft.Extensions.DependencyInjection.Abstractions.dll ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Microsoft.Extensions.Logging.Abstractions.dll ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\MyEMSLReader.dll                              ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Newtonsoft.Json.dll                           ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Npgsql.dll                                    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\Pacifica.dll                                  ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\PRISM.dll                                     ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\PRISMDatabaseUtils.dll                        ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\RangerLib.dll                                 ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\ShFolderBrowser.dll                           ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Buffers.dll                            ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Collections.Immutable.dll              ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Data.SQLite.dll                        ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Diagnostics.DiagnosticSource.dll       ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Memory.dll                             ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Numerics.Vectors.dll                   ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Runtime.CompilerServices.Unsafe.dll    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Text.Encodings.Web.dll                 ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Text.Json.dll                          ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Threading.Channels.dll                 ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.Threading.Tasks.Extensions.dll         ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\System.ValueTuple.dll                         ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\x64\SQLite.Interop.dll                        ; DestDir: {app}\Ranger\x64
Source: ..\DeployedFiles\Ranger\x86\SQLite.Interop.dll                        ; DestDir: {app}\Ranger\x86

Source: ..\DeployedFiles\Ranger\ReadMe.txt                                    ; DestDir: {app}\Ranger
Source: ..\DeployedFiles\Ranger\QueryDefinitions.xml                          ; DestDir: {app}\Ranger

Source: ..\MageExtractor\Extractor.ico                                        ; DestDir: {app}\MageExtractor
Source: ..\MageFileProcessor\wand.ico                                         ; DestDir: {app}\MageFileProcessor

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

Name: {commondesktop}\Mage Extractor; Filename: {app}\MageExtractor\MageExtractor.exe; Tasks: desktopicon; IconFilename: {app}\MageExtractor\Extractor.ico; IconIndex: 0; Comment: Mage Extractor
Name: {commondesktop}\Mage File Processor; Filename: {app}\MageFileProcessor\MageFileProcessor.exe; Tasks: desktopicon; IconFilename: {app}\MageFileProcessor\wand.ico; IconIndex: 0; Comment: Mage File Processor

[Setup]
AppName=Mage
AppVersion={#ApplicationVersion}
;AppVerName=Mage
AppID=MageId
AppPublisher=Pacific Northwest National Laboratory
AppPublisherURL=https://omics.pnl.gov/software
AppSupportURL=https://omics.pnl.gov/software
AppUpdatesURL=https://github.com/PNNL-Comp-Mass-Spec/Mage
ArchitecturesAllowed=x64 x86
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\Mage
DefaultGroupName=Mage
AppCopyright=© PNNL
;LicenseFile=.\License.rtf
PrivilegesRequired=admin
OutputBaseFilename=MageInstaller
;VersionInfoVersion=1.57
VersionInfoVersion={#ApplicationVersion}
VersionInfoCompany=PNNL
VersionInfoDescription=Mage
VersionInfoCopyright=PNNL
DisableFinishedPage=yes
DisableWelcomePage=no
ShowLanguageDialog=no
SetupIconFile=..\MageFileProcessor\wand.ico
InfoBeforeFile=.\readme.rtf
ChangesAssociations=no
;WizardImageFile=..\Deploy\Images\MageSetupSideImage.bmp
;WizardSmallImageFile=..\Deploy\Images\MageSetupSmallImage.bmp
WizardStyle=modern
InfoAfterFile=.\postinstall.rtf
EnableDirDoesntExistWarning=no
AlwaysShowDirOnReadyPage=yes
UninstallDisplayIcon={app}\delete_16x.ico
ShowTasksTreeLines=yes
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
