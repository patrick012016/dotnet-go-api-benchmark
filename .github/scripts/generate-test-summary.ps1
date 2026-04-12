<#
.SYNOPSIS
    Generates a markdown summary of test results and code coverage.
.DESCRIPTION
    Safely parses .trx and coverage.cobertura.xml files to extract metrics.
    Designed for GitHub Actions CI/CD pipelines.
.PARAMETER WorkspacePath
    The root directory of the repository or workspace where the action is executed. Defaults to the GITHUB_WORKSPACE environment variable.
.PARAMETER SummaryPath
    The file path where the generated markdown summary will be appended. Defaults to the GITHUB_STEP_SUMMARY environment variable for GitHub Actions.
.PARAMETER TestResultsDir
    The name of the directory inside the workspace that contains the .trx and coverage.cobertura.xml files. Defaults to "TestResults".
#>

[CmdletBinding()]
param (
    [Parameter(Mandatory = $false)]
    [string]$WorkspacePath = $env:GITHUB_WORKSPACE,

    [Parameter(Mandatory = $false)]
    [string]$SummaryPath = $env:GITHUB_STEP_SUMMARY,

    [Parameter(Mandatory = $false)]
    [string]$TestResultsDir = "TestResults"
)

# Strict Mode and global error handling
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

try {
    # Safe local path resolution
    if ([string]::IsNullOrWhiteSpace($SummaryPath)) {
        $SummaryPath = Join-Path -Path $PSScriptRoot -ChildPath "local-summary-test.md"
        Write-Warning "Running locally. Summary path not provided. Result will be written to: $SummaryPath"
    }

    # Workspace path validation
    if (-not (Test-Path -Path $WorkspacePath -PathType Container)) {
        Write-Warning "Workspace directory not found: $WorkspacePath. Default metrics will be used."
    }

    $testResultsPath = Join-Path -Path $WorkspacePath -ChildPath $TestResultsDir
    
    # Output variables initialization
    $total = 0; $passed = 0; $failed = 0
    $coverageText = "No data"

    # Secure function to load XML (XXE Mitigation)
    function Get-SecureXml {
        param([string]$FilePath)
        
        $settings = New-Object System.Xml.XmlReaderSettings
        $settings.DtdProcessing = [System.Xml.DtdProcessing]::Prohibit
        $settings.XmlResolver = $null
        
        # Read file safely
        $reader = [System.Xml.XmlReader]::Create($FilePath, $settings)
        $xmlDoc = New-Object System.Xml.XmlDocument
        
        try {
            $xmlDoc.Load($reader)
        }
        finally {
            if ($null -ne $reader) {
                $reader.Close()
                $reader.Dispose()
            }
        }
        
        return $xmlDoc
    }

    if (Test-Path -Path $testResultsPath -PathType Container) {
        
        # Fetching data from the TRX file
        $trxFile = Get-ChildItem -Path $testResultsPath -Filter "*.trx" -Recurse -File | Select-Object -First 1
        
        if ($null -ne $trxFile) {
            try {
                Write-Verbose "Parsing TRX file: $($trxFile.FullName)"
                $trx = Get-SecureXml -FilePath $trxFile.FullName
                $counters = $trx.GetElementsByTagName("Counters")
                
                if ($null -ne $counters -and $counters.Count -gt 0) {
                    $counterNode = $counters.Item(0)

                    [int]::TryParse($counterNode.GetAttribute("total"), [ref]$total) | Out-Null
                    [int]::TryParse($counterNode.GetAttribute("passed"), [ref]$passed) | Out-Null
                    [int]::TryParse($counterNode.GetAttribute("failed"), [ref]$failed) | Out-Null
                }
            } catch {
                Write-Warning "Failed to parse TRX file. Error: $($_.Exception.Message)"
            }
        } else {
            Write-Verbose "No TRX file found in $testResultsPath"
        }

        # Fetching data from the Cobertura file
        $covFile = Get-ChildItem -Path $testResultsPath -Filter "coverage.cobertura.xml" -Recurse -File | Select-Object -First 1
        
        if ($null -ne $covFile) {
            try {
                Write-Verbose "Parsing Cobertura file: $($covFile.FullName)"
                $cov = Get-SecureXml -FilePath $covFile.FullName
                $lineRateStr = $cov.DocumentElement.GetAttribute("line-rate")
                
                if (-not [string]::IsNullOrWhiteSpace($lineRateStr)) {
                    $lineRate = 0.0
                    if ([double]::TryParse($lineRateStr, [System.Globalization.NumberStyles]::Any, [System.Globalization.CultureInfo]::InvariantCulture, [ref]$lineRate)) {
                        $percent = [math]::Round($lineRate * 100, 2)
                        $coverageText = "$percent %"
                    } else {
                        Write-Warning "Failed to parse line-rate value: $lineRateStr"
                    }
                }
            } catch {
                Write-Warning "Failed to parse Cobertura file. Error: $($_.Exception.Message)"
            }
        } else {
            Write-Verbose "No coverage file found in $testResultsPath"
        }
    }
    
# Summary markdown

    $markdown = @"
    
| Metric | Result |
|---|---|
| **Code coverage** | **$coverageText** |
| **Total tests** | $total |
| Passed ✅ | $passed |
| Failed ❌ | $failed |
    
"@

    # Ensure the target directory exists
    $summaryDir = Split-Path -Path $SummaryPath -Parent
    if (-not [string]::IsNullOrWhiteSpace($summaryDir) -and -not (Test-Path -Path $summaryDir)) {
        New-Item -ItemType Directory -Path $summaryDir -Force | Out-Null
    }

    # Write to file
    $markdown | Out-File -FilePath $SummaryPath -Append -Encoding utf8
    Write-Verbose "Summary successfully written to $SummaryPath"

} catch {
    Write-Error "A critical error occurred while generating the summary: $($_.Exception.Message)"
    exit 1
}
