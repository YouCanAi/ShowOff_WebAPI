@echo ON

:start
set /p GameFolder=Game Folder:
if "%GameFolder:~-1%" == "\" (
    set GameFolder="%GameFolder:~0,-1%"
)
goto check

:check
set BepInExFolder=%GameFolder:~0,-1%\BepInEx\core"
set LibraryFolder=%GameFolder:~0,-1%\Idle ShowOff_Data\Managed"
goto copy

:folder_error
echo Wrong folder detected, check input
    goto start

:copy
copy %BepInExFolder%\BepInEx.dll "%cd%"
copy %LibraryFolder%\Assembly-CSharp.dll "%cd%"
copy %LibraryFolder%\UnityEngine.dll "%cd%"
copy %LibraryFolder%\UnityEngine.CoreModule.dll "%cd%"
copy %LibraryFolder%\UnityEngine.JSONSerializeModule.dll "%cd%"
echo Done.
pause