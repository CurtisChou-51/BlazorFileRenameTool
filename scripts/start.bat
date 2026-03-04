@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"

echo 正在啟動 FileRenameTool 伺服器...

set URL=http://localhost:5000

start "FileRenameTool Server" "FileRenameTool.exe" --urls="%URL%"

timeout /t 2 /nobreak >nul

echo 正在開啟瀏覽器...

start %URL%
