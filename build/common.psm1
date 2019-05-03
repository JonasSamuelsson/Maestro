function exec([string]$_cmd) {
    write-host -ForegroundColor DarkGray ">>> $_cmd $args"
    $ErrorActionPreference = 'Continue'
    & $_cmd @args
    $ErrorActionPreference = 'Stop'
    if ($LASTEXITCODE -And $LASTEXITCODE -ne 0) {
        write-error "Failed with exit code $LASTEXITCODE"
        exit 1
    }
}

function BuildTestPack {
    param(
        [Parameter(Mandatory)][string] $project, 
        [string] $artifacts = $null, 
        [string] $buildCounter = $null, 
        [switch] $ci
    )

    $root = [System.IO.Path]::GetFullPath("$PSScriptRoot/../src/$project")
    $srcProject = [System.IO.Path]::GetFullPath("$root/src/$project.csproj")
    $testProject = [System.IO.Path]::GetFullPath("$root/test/$project.Tests.csproj")

    if (![System.IO.Directory]::Exists("$root")) {
        throw "Directory '$root' not found."
    }
    
    if (!$artifacts) {
        $artifacts = "$root/.artifacts/"
        Remove-Item -Recurse $artifacts -ErrorAction Ignore
    }

    if ($buildCounter) {
        exec GetVsProjectVersion -path $srcProject | ForEach-Object { SetAdoBuildNumber -buildNumber "$_ #$buildCounter" }
    }

    if ($env:TF_BUILD) {
        $ci = $true
    }

    exec dotnet build -c "release" $srcProject
    
    [string[]] $testArgs = @()
    
    if ($ci) {
        $testArgs += "--logger", "trx"
    }
    
    exec dotnet test -c "release" $testProject $testArgs
    
    exec dotnet pack --no-restore --no-build -c "release" -o $artifacts --include-symbols "-p:SymbolPackageFormat=snupkg" $srcProject
}

function GetVsProjectVersion([string] $path) {
    $found = $false
    $pattern = '<Version>(.*)</Version>'

    foreach ($line in (Get-Content $path)) {
        if (($line -match $pattern)) {
            $found = $true
            Write-Output $matches[1]
            return
        }
    }

    if (!$found) {
        throw "Could not find version tag in " + $path
    }
}

function SetAdoBuildNumber([parameter(ValueFromPipeline)] [string] $buildNumber) { 
    Write-Host "##vso[build.updatebuildnumber]$buildNumber"
}