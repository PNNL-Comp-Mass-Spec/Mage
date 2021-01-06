@echo on

xcopy MageExtractor\Mage.dll                      F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\Mage.pdb                      F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageDisplayLib.dll            F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageExtContentFilters.dll     F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy MageExtractor\MageExtExtractionFilters.dll  F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y
xcopy Ranger\RangerLib.dll                        F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\ /D /Y

xcopy MageExtractor\Mage.dll                      F:\Documents\Projects\JohnSandoval\APE\lib\ /D /Y
xcopy MageExtractor\MageDisplayLib.dll            F:\Documents\Projects\JohnSandoval\APE\lib\ /D /Y
xcopy MageExtractor\MageUIComponents.dll          F:\Documents\Projects\JohnSandoval\APE\lib\ /D /Y

xcopy MageExtractor\Mage.dll                      F:\Documents\projects\JoshAldrich\InterferenceDetection\InterDetect\DLLLibrary\ /D /Y
xcopy MageExtractor\Mage.dll                      F:\Documents\projects\JoshAldrich\ProteinParsimony\SetCover\Lib\ /D /Y

@echo off
pause
