#!/usr/bin/env powershell

$filePath = "N:\VR-Projects\VR-Walk-Through\Assets\XR\Settings\OpenXR Package Settings.asset"
$backupPath = "N:\VR-Projects\VR-Walk-Through\OpenXR_Package_Settings.asset.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

# Create backup
Copy-Item $filePath $backupPath -Force
Write-Host "Backup created: $backupPath"

# Read entire file as text
$content = Get-Content $filePath -Raw

# Find all features: arrays and their boundaries
# A features array starts with "features:" and contains indented "- {fileID: ...}" lines
# We'll use regex to find each features array block and clean it

$totalRemoved = 0
$featuresArraysProcessed = 0

# Pattern to match a features array block:
# - Starts with "features:" (with optional leading whitespace)
# - Followed by lines that are indented (2+ spaces) and start with "- {fileID:"
# - Ends when we hit a line that's not indented as a feature entry
# We'll use a regex with multiline mode

$pattern = '(?ms)(^\s*features:\s*\n(?:^\s{2,}-\s*\{fileID:[^}]+\}\s*\n)+)'

$cleanedContent = $content
$totalRemoved = 0
$arraysProcessed = 0

# Use regex replacement with a callback
$cleanedContent = [System.Text.RegularExpressions.Regex]::Replace($content, $pattern, {
    param($match)
    $block = $match.Value
    $arraysProcessed++
    
    # Split the block into lines
    $blockLines = $block -split "`r?`n"
    $cleanedBlockLines = @()
    $removedInThisBlock = 0
    
    foreach ($line in $blockLines) {
        $trimmed = $line.Trim()
        if ($trimmed -eq "- {fileID: 0}") {
            $removedInThisBlock++
            $totalRemoved++
            continue  # Skip null entries
        }
        $cleanedBlockLines += $line
    }
    
    Write-Host "Array $arraysProcessed: removed $removedInThisBlock null entries"
    return ($cleanedBlockLines -join "`n")
})

# Write the cleaned file
Set-Content -Path $filePath -Value $cleanedContent -Encoding UTF8

Write-Host "Total null entries removed: $totalRemoved"
Write-Host "Features arrays processed: $arraysProcessed"

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
            # Still a null in features
        }
        # Check if we left the features array
        if ($line -notmatch '^\s{2,}-\s*\{fileID:' -and $trimmed -ne "features:") {
            if ($line -match '^\s*(---|\w+:|m_|customLoaderName)') {
                $inFeat = $false
            }
        }
    }
    
    if ($trimmed -eq "features:") {
        $inFeat = $true
    }
}

Write-Host "Features arrays found: $featArrays"
Write-Host "Script completed"