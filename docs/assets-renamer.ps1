# Sets the assets folder.
$assetsDir = ".\tutorial\.gitbook\assets"
$tutorialMarkdownDir = ".\tutorial\"

# Find for all possible pictures and sort them.
$pngFiles = Get-ChildItem -Path $assetsDir -Filter *.png | Sort-Object Name

# Step 1: Rename them to temporary names, to avoid naming conflict.
$tempMapping = @{}
$index = 0
foreach ($file in $pngFiles) {
    $tempName = "temp_{0:d4}.png" -f $index
    $tempMapping[$file.Name] = $tempName
    Rename-Item -Path $file.FullName -NewName $tempName
    $index++
}

# Step 2: Convert all temporary names to target file names.
$finalMapping = @{}
$index = 0
Get-ChildItem -Path $assetsDir -Filter temp_*.png | Sort-Object Name | ForEach-Object {
    $finalName = "images_{0:d4}.png" -f $index
    $finalMapping[$_.Name] = $finalName
    Rename-Item -Path $_.FullName -NewName $finalName
    $index++
}

# Step 3: Construct relation mapping between old and new file names.
$oldToNewMap = @{}
foreach ($key in $tempMapping.Keys) {
    $oldToNewMap[$key] = $finalMapping[$tempMapping[$key]]
}

# Step 4: Replace file paths referenced to pictures into new file names in all Markdown files.
Get-ChildItem -Path $tutorialMarkdownDir -Filter *.md -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding UTF8

    # Traverse all original file names, and update them up.
    foreach ($oldName in $oldToNewMap.Keys) {
        [string]$oldPath = "../.gitbook/assets/$oldName"
        [string]$newPath = "../.gitbook/assets/$($oldToNewMap[$oldName])"

        # Use 'string.Replace' method to replace values.
        # Do not use '-replace' operator because here the path should be escaped in order to keep regular expression behaving well,
        # which increases complexity of implementation.
        $content = $content.Replace($oldPath, $newPath)
    }

    # Write them back to Markdown files.
    # Please note that the option "-Encoding UTF8" is required because the default file generated will be UTF-8 with BOM, which is not expected.
    $content | Set-Content $_.FullName -NoNewline -Encoding UTF8
}
