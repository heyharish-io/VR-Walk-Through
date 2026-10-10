#!/usr/bin/env powershell

$filePath = "N:\VR-Projects\VR-Walk-Through\Assets\XR\Settings\OpenXR Package Settings.asset"
$backupPath = "N:\VR-Projects\VR-Walk-Through\OpenXR_Package_Settings.asset.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

# Create backup
Copy-Item $filePath $backupPath -Force
Write-Host "Backup created: $backupPath"

# Read the file content
$content = Get-Content $filePath -Raw

$lines = $content -split "`r?`n"
$cleanedLines = @()
$inFeaturesArray = $false
$nullEntriesRemoved = 0
$featuresArrayCount = 0

foreach ($line in $lines) {
    $trimmed = $line.Trim()
    
    # Detect start of features: array
    if ($trimmed -eq "features:") {
        $inFeaturesArray = $true
        $featuresArrayCount++
    }
    
    if ($inFeaturesArray) {
        # Check if this line is a null feature entry
        if ($line.Trim() -eq "- {fileID: 0}") {
            $nullEntriesRemoved++
            continue  # Skip this line (remove it)
        }
        
        # Check if we've left the features array
        # Features array entries are indented (2+ spaces) and start with "- {fileID:"
        # When we hit a line that's not a feature entry and not "features:", we may have left the array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            # Check if this is a new top-level property or document
            if ($line -match '^(---|\w+:|m_|customLoaderName)') {
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
$newLines = $newContent -split "`r?`n"

# Count nulls in features arrays
$inFeatures = $false
$nullsInFeatures = 0
$featuresArrayCount = 0

foreach ($line in $newContent -split "`r?`n") {
    $trimmed = $line.Trim()
    if ($trimmed -eq "features:") {
        $inFeatures = $true
        $featuresArrayCount++
    }
    
    if ($line.Trim() -eq "- {fileID: 0}") {
        if ($inFeatures) {
            # This shouldn't happen if cleanup worked
        }
    }
    
    if ($inFeatures) {
        if ($line.Trim() -eq "- {fileID: 0}") {
            # Still a null in features - count it
        }
    }
    
    if ($inFeatures) {
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^(---|\w+:|m_|customLoaderName|m_)') {
                # Might have left features array
            }
        }
    }
}

# Better verification: count nulls in features arrays by tracking state
$inFeatures = $false
$nullsInFeatures = 0
$featuresArrayCount = 0
$inFeaturesArray = $false

foreach ($line in ($newContent -split "`r?`n")) {
    $trimmed = $line.Trim()
    
    if ($trimmed -eq "features:") {
        $inFeaturesArray = $true
    }
    
    if ($inFeaturesArray) {
        if ($line.Trim() -eq "- {fileID: 0}") {
            # This is a null entry in features array
        }
        
        # Check if we left the features array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^(---|\w+:|m_|customLoaderName|m_)') {
                $inFeaturesArray = $false
            }
        }
    }
    
    if ($trimmed -eq "features:") {
        $inFeaturesArray = $true
    }
}

# Better: scan through and track state properly
$inFeat = $false
$nullsInFeat = 0
$featCount = 0

foreach ($line in ($newContent -split "`r?`n")) {
    $trimmed = $line.Trim()
    
    if ($trimmed -eq "features:") {
        $inFeat = $true
    }
    
    if ($inFeat) {
        if ($line.Trim() -eq "- {fileID: 0}") {
            # Still a null in features
        }
        
        # Check if we left the features array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^(---|\w+:|m_|customLoaderName|m_)') {
                $inFeat = $false
            }
        }
    }
    
    if ($trimmed -eq "features:") {
        $inFeat = $true
    }
}

# Better verification: read the file and check features arrays specifically
$content = Get-Content $filePath -Raw
$totalNullsInFile = ($content -split "- \{fileID: 0\}").Count - 1
Write-Host "Total fileID: 0 entries in file: $totalNullsInFile"

# Count features arrays
$featArrays = 0
$content -split "`r?`n" | ForEach-Object {
    if ($_.Trim() -eq "features:") { $featArrays++ }
}
Write-Host "Features arrays found: $featArrays"

# Count nulls in features arrays specifically
$inFeat = $false
$nullsInFeat = 0
$featArrays = 0
foreach ($line in ($newContent -split "`r?`n")) {
    $trimmed = $line.Trim()
    if ($trimmed -eq "features:") {
        $inFeat = $true
        $featArrays++
    }
    if ($inFeat) {
        if ($line.Trim() -eq "- {fileID: 0}") {
            # This is a null in features array
        }
        # Check if we left features array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^(---|\w+:|m_|customLoaderName|m_)') {
                $inFeat = $false
            }
        }
    }
    if ($line.Trim() -eq "features:") {
        $inFeat = $true
    }
}

Write-Host "Features arrays found: $featArrays"
Write-Host "Script completed"