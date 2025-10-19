@echo off
REM ================================
REM Auto build and push Docker image
REM ================================

setlocal enabledelayedexpansion

REM --- CONFIGURATION ---
set IMAGE_NAME=tungb12ok/hiep
set DOCKERFILE_PATH=SWP391_BackEnd/Dockerfile
set CONTEXT_PATH=.
set TAG=latest

echo ==========================================
echo Building Docker image: %IMAGE_NAME%:%TAG%
echo ==========================================

docker build -t %IMAGE_NAME%:%TAG% -f %DOCKERFILE_PATH% %CONTEXT_PATH%

IF %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed!
    exit /b %ERRORLEVEL%
)

echo ==========================================
echo Pushing Docker image: %IMAGE_NAME%:%TAG%
echo ==========================================

docker push %IMAGE_NAME%:%TAG%

IF %ERRORLEVEL% NEQ 0 (
    echo ❌ Push failed!
    exit /b %ERRORLEVEL%
)

echo ✅ Docker image built and pushed successfully!
pause
