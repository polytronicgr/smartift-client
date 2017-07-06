﻿param
(
    [Parameter(Mandatory=$true)]
    [string]$version = $null,
    [Parameter(Mandatory=$true)]
    [string]$codePath = $null
)

# Setup the script
$ErrorActionPreference = "Stop"

# Parse the version out
$versionParts = $version.Split(".", 4)
if ($versionParts.Length -ne 4)
{
    Write-Error "Invalid version string"
}
foreach ($versionPart in $versionParts)
{
    if ($versionPart -as [int] -eq $null)
    {
        Write-Error ("Invalid version - non numeric component: " + $versionPart)
    }
}

# Next let us update the assembly info files
$assemblyInfoFiles = @(($codePath + "\sift-win\Properties\AssemblyInfo.cs"))
foreach ($assemblyInfoFile in $assemblyInfoFiles)
{
	$existingContents = Get-Content $assemblyInfoFile
	for ($i = 0; $i -lt $existingContents.Length; $i++)
	{
		$line = $existingContents[$i]
		if ($line.StartsWith("[assembly: AssemblyVersion"))
		{
			$existingContents[$i] = "[assembly: AssemblyVersion(`"" +$version + "`")]"
			continue
		}
		if ($line.StartsWith("[assembly: AssemblyFileVersion"))
		{
			$existingContents[$i] = "[assembly: AssemblyFileVersion(`"" +$version + "`")]"
			continue
		}
	}
	$existingContents | Set-Content $assemblyInfoFile
	Write-Output ("Updated version in AssemblyInfo: " + $assemblyInfoFile)
}

# Now we need to edit the Wix file
$existingContents = Get-Content ($codePath + "\Setup\Product.wxs")
for ($i = 0; $i -lt $existingContents.Length; $i++)
{
    $line = $existingContents[$i]
    if ($line.StartsWith("  <Product "))
    {
		$shortVersion = $versionParts[0] + "." + $versionParts[1] + "." + $versionParts[2]
        $existingContents[$i] = "  <Product Id=`"*`" Name=`"LTS Admin Console`" Language=`"1033`" Version=`"" + $shortVersion + "`" Manufacturer=`"Logical Trading Systems LLC`" UpgradeCode=`"59d0dc2a-8f53-400a-9d08-9df7d4e093f6`">"
        continue
    }
}
$existingContents | Set-Content ($codePath + "\Setup\Product.wxs")
Write-Output ("Updated version in Setup.wxs: " + ($codePath + "\Setup\Product.wxs"))