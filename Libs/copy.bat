@echo ON

:start
set /p GameFolder=Game Folder:
if "%GameFolder:~-1%" == "\" (
    set GameFolder="%GameFolder:~0,-1%"
) else (
    set GameFolder="%GameFolder%"
)
goto check

:check
set BepInExFolder=%GameFolder:~0,-1%\BepInEx\core"
set LibraryFolder=%GameFolder:~0,-1%\Idle ShowOff_Data\Managed"
if not exist %GameFolder% (goto folder_error)
    if not exist %BepInExFolder% (goto folder_error)
        if not exist %LibraryFolder% (goto folder_error)
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