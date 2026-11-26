xcopy Output\MageInstaller.exe \\proto-2\Software\Mage /D /Y
xcopy Output\MageInstaller.exe \\pnl\projects\OmicsSW\Mage /D /Y
xcopy Output\MageInstaller.exe \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage /D /Y

xcopy ..\README.md \\proto-2\Software\Mage /D /Y
xcopy ..\README.md \\pnl\projects\OmicsSW\Mage /D /Y
xcopy ..\README.md \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage /D /Y

xcopy ..\RevisionHistory.txt \\proto-2\Software\Mage /D /Y
xcopy ..\RevisionHistory.txt \\pnl\projects\OmicsSW\Mage /D /Y
xcopy ..\RevisionHistory.txt \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage /D /Y

xcopy Output\MageInstaller_CBDMS.exe \\cbdms\dms_programs\_Installers /D /Y
xcopy ..\README.md \\cbdms\dms_programs\_Installers /D /Y
xcopy ..\RevisionHistory.txt \\cbdms\dms_programs\_Installers /D /Y

xcopy ..\DeployedFiles\MageConcatenator\*.* \\proto-2\Software\Mage\Exe_Only\MageConcatenator\ /D /Y
xcopy ..\DeployedFiles\MageConcatenator\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\MageConcatenator\ /D /Y
xcopy ..\DeployedFiles\MageConcatenator\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\MageConcatenator\ /D /Y

xcopy ..\DeployedFiles\MageExtractor\*.* \\proto-2\Software\Mage\Exe_Only\MageExtractor\ /D /Y
xcopy ..\DeployedFiles\MageExtractor\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\MageExtractor\ /D /Y
xcopy ..\DeployedFiles\MageExtractor\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\MageExtractor\ /D /Y

xcopy ..\DeployedFiles\MageFilePackager\*.* \\proto-2\Software\Mage\Exe_Only\MageFilePackager\ /D /Y
xcopy ..\DeployedFiles\MageFilePackager\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\MageFilePackager\ /D /Y
xcopy ..\DeployedFiles\MageFilePackager\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\MageFilePackager\ /D /Y

xcopy ..\DeployedFiles\MageFileProcessor\*.* \\proto-2\Software\Mage\Exe_Only\MageFileProcessor\ /D /Y
xcopy ..\DeployedFiles\MageFileProcessor\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\MageFileProcessor\ /D /Y
xcopy ..\DeployedFiles\MageFileProcessor\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\MageFileProcessor\ /D /Y

xcopy ..\DeployedFiles\MageMetaDataProcessor\*.* \\proto-2\Software\Mage\Exe_Only\MageMetaDataProcessor\ /D /Y
xcopy ..\DeployedFiles\MageMetaDataProcessor\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\MageMetaDataProcessor\ /D /Y
xcopy ..\DeployedFiles\MageMetaDataProcessor\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\MageMetaDataProcessor\ /D /Y

xcopy ..\DeployedFiles\Ranger\*.* \\proto-2\Software\Mage\Exe_Only\Ranger\ /D /Y
xcopy ..\DeployedFiles\Ranger\*.* \\pnl\projects\OmicsSW\Mage\Exe_Only\Ranger\ /D /Y
xcopy ..\DeployedFiles\Ranger\*.* \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only\Ranger\ /D /Y

xcopy ..\README.md \\proto-2\Software\Mage\Exe_Only /D /Y
xcopy ..\README.md \\pnl\projects\OmicsSW\Mage\Exe_Only /D /Y
xcopy ..\README.md \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only /D /Y

xcopy ..\RevisionHistory.txt \\proto-2\Software\Mage\Exe_Only /D /Y
xcopy ..\RevisionHistory.txt \\pnl\projects\OmicsSW\Mage\Exe_Only /D /Y
xcopy ..\RevisionHistory.txt \\Proto-3\DMS_Programs_Dist\AnalysisToolManagerDistribution\Mage\Exe_Only /D /Y

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
