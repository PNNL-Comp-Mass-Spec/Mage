@echo off

echo.
echo.
echo The copy commands in this batch file are deprecated
echo.
echo Instead, use Mage\DeployedFiles\Distribute_Files.bat
echo.
echo.

Goto Done:

echo Distributing AnyCPU Mage

xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestMagePlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Program\bin\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_AScore_PlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Mage_PlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestAScorePlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Ape_PlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_MultiAlign_Aggregator_PlugIn\bin\Debug\" /Y /D
xcopy Release\Mage.* "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestApePlugIn\bin\Debug\" /Y /D

xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestMagePlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Program\bin\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_AScore_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Mage_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestAScorePlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Ape_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_MultiAlign_Aggregator_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISM.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestApePlugIn\bin\Debug\" /Y /D

xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestMagePlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Common\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\AM_Program\bin\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_AScore_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Mage_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestAScorePlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_Ape_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Plugins\AM_MultiAlign_Aggregator_PlugIn\bin\Debug\" /Y /D
xcopy Release\PRISMDatabaseUtils.dll "F:\Documents\Projects\DataMining\DMS_Managers\Analysis_Manager\Test_Plugins\TestApePlugIn\bin\Debug\" /Y /D

:Done
pause
