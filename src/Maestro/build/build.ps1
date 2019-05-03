[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [string]$buildCounter = $null,
    [switch]$ci
)

$ErrorActionPreference = 'Stop'

Import-Module -Force -Scope Local "$PSScriptRoot/../../../build/common.psm1"

$expression = "exec BuildTestPack -project Maestro"

if ($artifacts) {
    $expression += " -artifacts $artifacts"
}

if ($buildCounter) {
    $expression += " -buildCounter $buildCounter"
}

if ($ci) {
    $expression += " -ci"
}

invoke-expression $expression
    
write-host -f green 'script completed'