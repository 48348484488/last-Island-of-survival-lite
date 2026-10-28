cmake_minimum_required(VERSION 3.16)
project(LastIslandOfSurvivalLite C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# -----------------------------------------------------------------------------------------
# Platform Detection & Raylib Configuration
# -----------------------------------------------------------------------------------------
if(ANDROID)
    # Android NDK Build Settings
    message(STATUS "Building for Android (Mobile)")
    add_definitions(-DPLATFORM_ANDROID)
    
    # raylib for Android
    find_package(raylib REQUIRED)
    
    add_library(${PROJECT_NAME} SHARED
        main.cpp
    )
    
    target_link_libraries(${PROJECT_NAME} PRIVATE
        raylib
        android
        log
        GLESv2
        EGL
    )
else()
    # Desktop Build Settings (Windows / Linux / macOS)
    message(STATUS "Building for Desktop")
    add_definitions(-DPLATFORM_DESKTOP)
    
    # Check local raylib directory
    if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/../raylib-6.2_win64_mingw-w64")
        set(RAYLIB_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../raylib-6.2_win64_mingw-w64")
        include_directories("${RAYLIB_DIR}/include")
        link_directories("${RAYLIB_DIR}/lib")
    endif()
    
    add_executable(${PROJECT_NAME}
        main.cpp
    )
    
    if(WIN32)
        target_link_libraries(${PROJECT_NAME} PRIVATE
            raylib
            opengl32
            gdi32
            winmm
        )
    else()
        target_link_libraries(${PROJECT_NAME} PRIVATE
            raylib
            GL
            m
            pthread
            dl
            rt
            X11
        )
    endif()
endif()
