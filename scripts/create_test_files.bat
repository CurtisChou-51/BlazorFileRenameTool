@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"

set "BASE_DIR=baseFolder"

echo 正在建立測試資料夾與檔案結構...

if exist "%BASE_DIR%" (
    echo 發現舊的 %BASE_DIR% 目錄，正在清除...
    rmdir /S /Q "%BASE_DIR%"
)

mkdir "%BASE_DIR%"
type nul > "%BASE_DIR%\file1.txt"

mkdir "%BASE_DIR%\folder1"
type nul > "%BASE_DIR%\folder1\file2.txt"

mkdir "%BASE_DIR%\folder1\subfolder"
type nul > "%BASE_DIR%\folder1\subfolder\file3.txt"

mkdir "%BASE_DIR%\folder2"
type nul > "%BASE_DIR%\folder2\file4.txt"

echo 完成! 測試結構已建立於 %CD%\%BASE_DIR%
pause
