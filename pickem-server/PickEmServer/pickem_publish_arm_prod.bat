REM base version - udpate this one
REM  1.0.X - first deploy to prod
SET ASSM_VERSION=2.1.51-linarm

REM append date-time
SET ASSM_VERSION=%ASSM_VERSION%-%date:~4,2%-%date:~7,2%-%date:~10,4%-%time:~0,2%%time:~3,2%%time:~6,2%
REM remove space when time is 1 to 9 AM
SET ASSM_VERSION=%ASSM_VERSION: =%

dotnet publish -c Production -r linux-arm /p:Version=%ASSM_VERSION%

pause