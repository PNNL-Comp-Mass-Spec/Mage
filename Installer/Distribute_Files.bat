xcopy Output\MageInstaller.exe \\floyd\software\Mage /D /Y
xcopy ..\README.md \\floyd\software\Mage /D /Y
xcopy ..\RevisionHistory.txt \\floyd\software\Mage /D /Y

xcopy Output\MageInstaller_CBDMS.exe \\cbdms\dms_programs\_Installers /D /Y
xcopy ..\README.md \\cbdms\dms_programs\_Installers /D /Y
xcopy ..\RevisionHistory.txt \\cbdms\dms_programs\_Installers /D /Y

xcopy ..\DeployedFiles\MageConcatenator\*.* \\floyd\software\Mage\Exe_Only\MageConcatenator\ /D /Y
xcopy ..\DeployedFiles\MageExtractor\*.* \\floyd\software\Mage\Exe_Only\MageExtractor\ /D /Y
xcopy ..\DeployedFiles\MageFilePackager\*.* \\floyd\software\Mage\Exe_Only\MageFilePackager\ /D /Y
xcopy ..\DeployedFiles\MageFileProcessor\*.* \\floyd\software\Mage\Exe_Only\MageFileProcessor\ /D /Y
xcopy ..\DeployedFiles\MageMetaDataProcessor\*.* \\floyd\software\Mage\Exe_Only\MageMetaDataProcessor\ /D /Y
xcopy ..\DeployedFiles\Ranger\*.* \\floyd\software\Mage\Exe_Only\Ranger\ /D /Y

xcopy ..\README.md \\FLOYD\software\Mage\Exe_Only /D /Y
xcopy ..\RevisionHistory.txt \\FLOYD\software\Mage\Exe_Only /D /Y

@echo off
echo.
echo.
echo Distributing DLLs
sleep 1

pushd ..\DeployedFiles
call Distribute_Files.bat

@echo off
popd

echo.
pause
