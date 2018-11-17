REM base version - udpate this one
REM  0.8.X - web and server "running" first running release to test
SET ASSM_VERSION=2.0.47-linarm

REM append date-time
SET ASSM_VERSION=%ASSM_VERSION%-%date:~4,2%-%date:~7,2%-%date:~10,4%-%time:~0,2%%time:~3,2%%time:~6,2%
REM remove space when time is 1 to 9 AM
SET ASSM_VERSION=%ASSM_VERSION: =%

dotnet publish -c Test -r linux-arm /p:Version=%ASSM_VERSION%

pause