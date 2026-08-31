cmake_minimum_required(VERSION 3.20)
project(wa2_cpp VERSION 0.1.0 LANGUAGES CXX)

option(WA2_BUILD_APP "Build the SDL2 application" ON)
option(WA2_BUILD_TESTS "Build core tests" ON)
option(WA2_WITH_FFMPEG "Enable FFmpeg movie playback" OFF)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

add_library(wa2_core STATIC
    src/archive.cpp
    src/text.cpp
    src/vm.cpp
    src/system_store.cpp
    src/runtime.cpp
)
target_include_directories(wa2_core PUBLIC include)
if(MSVC)
    target_compile_options(wa2_core PRIVATE /W4 /permissive-)
else()
    target_compile_options(wa2_core PRIVATE -Wall -Wextra -Wpedantic)
endif()

if(WA2_BUILD_TESTS)
    enable_testing()
    add_executable(wa2_core_tests tests/core_tests.cpp)
    target_link_libraries(wa2_core_tests PRIVATE wa2_core)
    add_test(NAME wa2_core_tests COMMAND wa2_core_tests)
endif()

if(WA2_BUILD_APP)
    find_package(PkgConfig REQUIRED)
    pkg_check_modules(SDL2 REQUIRED IMPORTED_TARGET sdl2)
    pkg_check_modules(SDL2_IMAGE REQUIRED IMPORTED_TARGET SDL2_image)
    pkg_check_modules(SDL2_MIXER REQUIRED IMPORTED_TARGET SDL2_mixer)
    pkg_check_modules(SDL2_TTF REQUIRED IMPORTED_TARGET SDL2_ttf)

    add_executable(wa2_cpp
        src/main.cpp
        src/sdl_frontend.cpp
        src/video.cpp
    )
    target_link_libraries(wa2_cpp PRIVATE
        wa2_core
        PkgConfig::SDL2
        PkgConfig::SDL2_IMAGE
        PkgConfig::SDL2_MIXER
        PkgConfig::SDL2_TTF
    )

    if(NOT WIN32)
        target_compile_definitions(wa2_core PRIVATE WA2_WITH_SDL=1)
        target_link_libraries(wa2_core PUBLIC PkgConfig::SDL2)
    endif()

    if(WA2_WITH_FFMPEG)
        pkg_check_modules(FFMPEG REQUIRED IMPORTED_TARGET
            libavcodec libavformat libavutil libswresample libswscale)
        target_compile_definitions(wa2_cpp PRIVATE WA2_WITH_FFMPEG=1)
        target_link_libraries(wa2_cpp PRIVATE PkgConfig::FFMPEG)
    endif()

    if(NINTENDO_SWITCH)
        set_target_properties(wa2_cpp PROPERTIES OUTPUT_NAME "wa2-cpp")
        nx_generate_nacp(wa2_cpp.nacp
            NAME "WA2 C++ Runtime"
            AUTHOR "WA2 C++ port contributors"
            VERSION "${PROJECT_VERSION}")
        nx_create_nro(wa2_cpp NACP wa2_cpp.nacp)
    endif()
endif()
