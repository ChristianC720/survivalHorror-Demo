<#
.SYNOPSIS
    Runs the project's EditMode and PlayMode tests in Unity batchmode.

.DESCRIPTION
    Resolves the project root relative to this script, runs each test platform in a
    separate Unity process and writes NUnit XML results and logs to <project>/TestResults/.
    Exits non-zero when Unity fails, results are missing, or any test fails.

.PARAMETER UnityPath
    Full path to the Unity editor executable matching ProjectSettings/ProjectVersion.txt.

.PARAMETER Platform
    Which platforms to run. Defaults to both, EditMode first.

.EXAMPLE
    ./Tools/run-tests.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe"
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $UnityPath,

    [ValidateSet('EditMode', 'PlayMode')]
    [string[]] $Platform = @('EditMode', 'PlayMode')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) {
    Write-Error "Unity executable not found: '$UnityPath'."
    exit 1
}

$projectRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
if (-not (Test-Path -LiteralPath (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt'))) {
    Write-Error "Project root '$projectRoot' does not look like a Unity project."
    exit 1
}

function Test-ProjectOpenInEditor {
    param([string] $Root)

    $lockFile = Join-Path $Root 'Temp/UnityLockfile'
    if (-not (Test-Path -LiteralPath $lockFile)) {
        return $false
    }

    try {
        $stream = [System.IO.File]::Open($lockFile, 'Open', 'ReadWrite', 'None')
        $stream.Dispose()
        return $false
    }
    catch [System.IO.IOException] {
        return $true
    }
}

if (Test-ProjectOpenInEditor -Root $projectRoot) {
    Write-Error "The project at '$projectRoot' is open in a Unity Editor. Close it before running batchmode tests."
    exit 1
}

$resultsDir = Join-Path $projectRoot 'TestResults'
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

$overallExitCode = 0

foreach ($testPlatform in $Platform) {
    $resultsFile = Join-Path $resultsDir "$testPlatform.xml"
    $logFile = Join-Path $resultsDir "$testPlatform.log"
    Remove-Item -LiteralPath $resultsFile, $logFile -ErrorAction SilentlyContinue

    $arguments = @(
        '-batchmode',
        '-projectPath', "`"$projectRoot`"",
        '-runTests',
        '-testPlatform', $testPlatform,
        # Only project assemblies; package testables (e.g. com.unity.inputsystem) bring their own suites.
        '-assemblyNames', "Project.Tests.$testPlatform",
        '-testResults', "`"$resultsFile`"",
        '-logFile', "`"$logFile`""
    ) -join ' '

    Write-Host "Running $testPlatform tests..."
    $process = Start-Process -FilePath $UnityPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
    $unityExitCode = $process.ExitCode

    if (-not (Test-Path -LiteralPath $resultsFile)) {
        Write-Host "${testPlatform}: no results file produced (Unity exit code $unityExitCode). See $logFile" -ForegroundColor Red
        $overallExitCode = if ($unityExitCode -ne 0) { $unityExitCode } else { 1 }
        continue
    }

    [xml] $results = Get-Content -LiteralPath $resultsFile -Raw
    $run = $results.'test-run'
    $summary = "total=$($run.total) passed=$($run.passed) failed=$($run.failed) skipped=$($run.skipped) result=$($run.result)"
    $failed = ([int] $run.failed -gt 0) -or ($run.result -like 'Failed*')

    if ($unityExitCode -ne 0 -or $failed) {
        Write-Host "$testPlatform FAILED ($summary, Unity exit code $unityExitCode). See $resultsFile" -ForegroundColor Red
        $overallExitCode = if ($unityExitCode -ne 0) { $unityExitCode } else { 1 }
    }
    else {
        Write-Host "$testPlatform passed ($summary)." -ForegroundColor Green
    }
}

exit $overallExitCode
