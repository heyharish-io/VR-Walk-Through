#!/usr/bin/env powershell

$filePath = "N:\VR-Projects\VR-Walk-Through\Assets\XR\Settings\OpenXR Package Settings.asset"
$backupPath = "N:\VR-Projects\VR-Walk-Through\OpenXR_Package_Settings.asset.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

# Create backup
Copy-Item $filePath $backupPath -Force
Write-Host "Backup created: $backupPath"

# Read all lines
$lines = Get-Content $filePath
$cleanedLines = @()
$inFeaturesArray = $false
$nullEntriesRemoved = 0
$featuresArrayCount = 0

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    $trimmed = $line.Trim()
    
    # Detect start of features: array
    if ($trimmed -eq "features:") {
        $inFeaturesArray = $true
        $featuresArrayCount++
        Write-Host "Entering features array #$featuresArrayCount at line $($i+1)"
    }
    
    # Check if this line is a null feature entry inside a features array
    if ($inFeaturesArray -and $line.Trim() -eq "- {fileID: 0}") {
        $nullEntriesRemoved++
        Write-Host "  Removing null entry at line $($i+1)"
        continue  # Skip this line (remove it)
    }
    
    # Check if we've left the features array
    # Features array entries are indented (2+ spaces) and start with "- {fileID:"
    # When we hit a line that's not a feature entry and not "features:", we may have left the array
    if ($inFeaturesArray) {
        $isFeatureEntry = $line -match '^\s{2,}-\s*\{fileID:'
        $isFeaturesHeader = ($trimmed -eq "features:")
        
        if (-not $isFeatureEntry -and -not $isFeaturesHeader) {
            # This line is not a feature entry and not a features header
            # Check if it's a new top-level property or document start
            $trimmedLine = $line.TrimStart()
            if ($trimmedLine -match '^(---|[a-zA-Z_][\w]*:|m_|customLoaderName)') {
                Write-Host "  Leaving features array at line $($i+1): '$($line.Trim())'"
                $inFeaturesArray = $false
            }
        }
    }
    
    $cleanedLines += $line
}

$cleanedContent = $cleanedLines -join "`n"

# Write the cleaned file
Set-Content -Path $filePath -Value $cleanedContent -Encoding UTF8

Write-Host "Removed $nullEntriesRemoved null entries from features arrays"
Write-Host "Processed $featuresArrayCount features arrays"

# Verify
$newContent = Get-Content $filePath -Raw

# Count total fileID: 0 entries in file
$totalNulls = ($newContent -split "- \{fileID: 0\}").Count - 1
Write-Host "Total fileID: 0 entries in file: $totalNulls"

# Count features arrays
$featArrays = 0
$newContent -split "`r?`n" | ForEach-Object {
    if ($_.Trim() -eq "features:") { $featArrays++ }
}
Write-Host "Features arrays found: $featArrays"

# Verify no nulls in features arrays
$inFeat = $false
$nullsInFeat = 0
$featArrays = 0
$inFeatArray = $false

foreach ($line in ($newContent -split "`r?`n")) {
    $trimmed = $line.Trim()
    if ($trimmed -eq "features:") {
        $inFeat = $true
        $featArrays++
    }
    
    if ($inFeat) {
        if ($line.Trim() -eq "- {fileID: 0}") {
            $nullsInFeat++
        }
        # Check if we left the features array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^\s*(---|\w+:|m_|customLoaderName)') {
                $inFeat = $false
            }
        }
    }
}

Write-Host "Features arrays found: $featArrays"
Write-Host "Null entries remaining in features arrays: $nullsInFeat"
Write-Host "Total fileID: 0 entries in file: $((($newContent -split "- \{fileID: 0\}").Count - 1))"
Write-Host "Script completed"