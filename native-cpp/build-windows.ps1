param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [string]$MingwRoot = "",
    [switch]$RunTests
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if ([string]::IsNullOrWhiteSpace($MingwRoot)) {
    $candidates = @($env:MINGW_PREFIX, "D:\mingw64", "C:\msys64\mingw64") |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    foreach ($candidate in $candidates) {
        if (Test-Path (Join-Path $candidate "bin\g++.exe")) {
            $MingwRoot = $candidate
            break
        }
    }
}

if ([string]::IsNullOrWhiteSpace($MingwRoot)) {
    throw "MinGW-w64 with SDL2 was not found. Pass -MingwRoot or set MINGW_PREFIX."
}

$Compiler = Join-Path $MingwRoot "bin\g++.exe"
$Strip = Join-Path $MingwRoot "bin\strip.exe"
$SdlInclude = Join-Path $MingwRoot "include\SDL2"
$SdlLib = Join-Path $MingwRoot "lib"
if (-not (Test-Path $Compiler) -or -not (Test-Path $SdlInclude)) {
    throw "MinGW-w64 with SDL2 was not found. Pass -MingwRoot or set MINGW_PREFIX."
}

$OutputDirectory = Join-Path $ProjectRoot "build-windows\$Configuration"
$VendorDirectory = Join-Path $ProjectRoot "build-windows\vendor-lib"
New-Item -ItemType Directory -Force -Path $OutputDirectory, $VendorDirectory | Out-Null

$StaticLibraries = @("libSDL2.a", "libSDL2_image.a", "libSDL2_mixer.a", "libSDL2_ttf.a")
foreach ($library in $StaticLibraries) {
    $source = Join-Path $SdlLib $library
    $target = Join-Path $VendorDirectory $library
    if (-not (Test-Path $source)) {
        throw "Missing SDL2 static library: $source"
    }
    if (-not (Test-Path $target) -or
        (Get-Item $source).LastWriteTimeUtc -gt (Get-Item $target).LastWriteTimeUtc) {
        Copy-Item -LiteralPath $source -Destination $target -Force
        & $Strip --strip-debug $target
        if ($LASTEXITCODE -ne 0) { throw "Failed to sanitize $library" }
    }
}

$CoreSources = @(
    "src\archive.cpp",
    "src\text.cpp",
    "src\vm.cpp",
    "src\system_store.cpp",
    "src\runtime.cpp"
) | ForEach-Object { Join-Path $ProjectRoot $_ }
$AppSources = @(
    "src\video.cpp",
    "src\sdl_frontend.cpp",
    "src\main.cpp"
) | ForEach-Object { Join-Path $ProjectRoot $_ }

$Arguments = @(
    "-std=c++17", "-Wall", "-Wextra", "-Wpedantic", "-DSDL_MAIN_HANDLED",
    "-I$(Join-Path $ProjectRoot 'include')", "-I$SdlInclude"
)
if ($Configuration -eq "Debug") {
    $Arguments += @("-O0", "-g3", "-DDEBUG")
} else {
    $Arguments += @("-O2", "-DNDEBUG", "-s")
}
$Arguments += $CoreSources + $AppSources
$Arguments += @(
    (Join-Path $VendorDirectory "libSDL2_image.a"),
    (Join-Path $VendorDirectory "libSDL2_mixer.a"),
    (Join-Path $VendorDirectory "libSDL2_ttf.a"),
    (Join-Path $VendorDirectory "libSDL2.a"),
    "-static-libgcc", "-static-libstdc++",
    "-ldinput8", "-ldxguid", "-ldxerr8", "-luser32", "-lgdi32", "-lwinmm",
    "-limm32", "-lole32", "-loleaut32", "-lshell32", "-lsetupapi", "-lversion",
    "-luuid", "-lusp10", "-lrpcrt4",
    "-o", (Join-Path $OutputDirectory "wa2_cpp.exe")
)

& $Compiler @Arguments
if ($LASTEXITCODE -ne 0) { throw "Windows application build failed." }
Write-Host "Built: $(Join-Path $OutputDirectory 'wa2_cpp.exe')"

if ($RunTests) {
    $TestOutput = Join-Path $OutputDirectory "wa2_core_tests.exe"
    $TestArguments = @(
        "-std=c++17", "-O0", "-g3", "-Wall", "-Wextra", "-Wpedantic",
        "-I$(Join-Path $ProjectRoot 'include')"
    ) + $CoreSources + @((Join-Path $ProjectRoot "tests\core_tests.cpp"), "-o", $TestOutput)
    & $Compiler @TestArguments
    if ($LASTEXITCODE -ne 0) { throw "Core test build failed." }
    Push-Location $OutputDirectory
    try {
        & $TestOutput
        if ($LASTEXITCODE -ne 0) { throw "Core tests failed." }
    } finally {
        Pop-Location
    }
}
