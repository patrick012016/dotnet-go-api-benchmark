# A simple script that prints a summary of the tests 

param (
    [string]$WorkspacePath = $env:GITHUB_WORKSPACE,
    [string]$SummaryPath = $env:GITHUB_STEP_SUMMARY
)

# If the script is running locally (outside of GitHub Actions), print to the console
if ([string]::IsNullOrEmpty($SummaryPath)) {
    $SummaryPath = ".\local-summary-test.md"
    Write-Host "Uruchomienie lokalne. Wynik zostanie zapisany do $SummaryPath"
}

# Retrieving data from a TRX file (test results)
$trxFile = Get-ChildItem -Path "$WorkspacePath/TestResults" -Filter *.trx -Recurse | Select-Object -First 1
$total = 0; $passed = 0; $failed = 0

if ($trxFile) {
    [xml]$trx = Get-Content $trxFile.FullName
    $counters = $trx.GetElementsByTagName("Counters")[0]
    if ($counters) {
        $total = $counters.GetAttribute("total")
        $passed = $counters.GetAttribute("passed")
        $failed = $counters.GetAttribute("failed")
    }
}

# Retrieving data from a Cobertura file (code coverage)
$covFile = Get-ChildItem -Path "$WorkspacePath/TestResults" -Filter coverage.cobertura.xml -Recurse | Select-Object -First 1
$coverageText = "Brak danych"

if ($covFile) {
    [xml]$cov = Get-Content $covFile.FullName
    $lineRateStr = $cov.DocumentElement.GetAttribute("line-rate")
    if (![string]::IsNullOrWhiteSpace($lineRateStr)) {
        $lineRate = [double]::Parse($lineRateStr, [System.Globalization.CultureInfo]::InvariantCulture)
        $percent = [math]::Round($lineRate * 100, 2)
        $coverageText = "$percent %"
    }
}


$markdown = @"
## 📊 Podsumowanie testów

| Metryka | Wynik |
|---|---|
| **Pokrycie kodu** | **$coverageText** |
| **Wszystkie testy** | $total |
| Zaliczone ✅ | $passed |
| Nieudane ❌ | $failed |
"@

# Write to the file specified by the GitHub environment variable
$markdown | Out-File -FilePath $SummaryPath -Append -Encoding utf8
