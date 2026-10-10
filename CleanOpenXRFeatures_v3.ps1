#!/usr/bin/env powershell

$filePath = "N:\VR-Projects\VR-Walk-Through\Assets\XR\Settings\OpenXR Package Settings.asset"
$backupPath = "N:\VR-Projects\VR-Walk-Through\OpenXR_Package_Settings.asset.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

# Create backup
Copy-Item $filePath $backupPath -Force
Write-Host "Backup created: $backupPath"

# Read the file content
$lines = Get-Content $filePath
$cleanedLines = @()
$inFeaturesArray = $false
$nullEntriesRemoved = 0
$featuresArrayCount = 0

foreach ($line in $lines) {
    $trimmed = $line.Trim()
    
    # Detect start of features: array
    if ($trimmed -eq "features:") {
        $inFeaturesArray = $true
    }
    
    # Check if this line is a null feature entry inside a features array
    if ($inFeaturesArray -and $line.Trim() -eq "- {fileID: 0}") {
        $nullEntriesRemoved++
        continue  # Skip this line (remove it)
    }
    
    # Check if we've left the features array
    # Features array entries are indented and start with "- {fileID:"
    # When we hit a line that's not a feature entry and not "features:", we may have left the array
    if ($inFeaturesArray) {
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            # Check if this is a new top-level property or document
            # Account for leading whitespace in the regex
            if ($line -match '^\s*(---|\w+:|m_|customLoaderName)') {
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
            # Still a null in features array - this would be an error
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
Write-Host "Script completed"