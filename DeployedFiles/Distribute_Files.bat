@echo on

xcopy MageExtractor\Mage.dll                      F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageDisplayLib.dll            F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageExtContentFilters.dll     F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageExtExtractionFilters.dll  F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y

xcopy MageExtractor\Mage.dll                      F:\Documents\Projects\JohnSandoval\APE_DLL\Lib\ /D /Y
xcopy MageExtractor\MageDisplayLib.dll            F:\Documents\Projects\JohnSandoval\APE_DLL\Lib\ /D /Y

xcopy MageExtractor\Mage.dll                      F:\Documents\projects\JohnSandoval\APE_GUI\lib\ /D /Y
xcopy MageExtractor\MageDisplayLib.dll            F:\Documents\projects\JohnSandoval\APE_GUI\lib\ /D /Y
xcopy MageExtractor\MageUIComponents.dll          F:\Documents\projects\JohnSandoval\APE_GUI\lib\ /D /Y

xcopy MageExtractor\Mage.dll                      F:\Documents\projects\JoshAldrich\InterferenceDetection\InterDetect\DLLLibrary\ /D /Y
xcopy MageExtractor\Mage.dll                      F:\Documents\projects\JoshAldrich\ProteinParsimony\SetCover\Lib\ /D /Y

@echo off
sleep 1
@echo on