@echo off
title Last Island of Survival - Recreated Client (Reverse Engineered)
echo ======================================================================
echo   Last Island of Survival LITE - Recreated 3D Client
echo   Engine: Python + PyOpenGL (Direct APK Reverse Engineering)
echo ======================================================================
echo.
python -m pip install -r requirements.txt --quiet
python viewer3d.py
pause
