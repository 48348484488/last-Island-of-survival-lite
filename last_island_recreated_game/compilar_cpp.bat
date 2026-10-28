@echo off
setlocal
cd /d "%~dp0"

echo =======================================================
echo   Compilando Last Island of Survival LITE (C++)
echo =======================================================

set RAYLIB_DIR=..\raylib-6.2_win64_mingw-w64

if not exist "%RAYLIB_DIR%" (
    echo [ERRO] Pasta do Raylib nao encontrada em: %RAYLIB_DIR%
    pause
    exit /b 1
)

echo Usando compilador G++/Clang e Raylib...
g++ main.cpp -O2 -o game.exe -I"%RAYLIB_DIR%\include" -L"%RAYLIB_DIR%\lib" -lraylib -lopengl32 -lgdi32 -lwinmm

if %ERRORLEVEL% equ 0 (
    echo.
    echo =======================================================
    echo   [SUCESSO] Jogo compilado com sucesso! (game.exe)
    echo =======================================================
) else (
    echo.
    echo [ERRO] Falha na compilacao.
)

pause
