param
(
    [string]$azCopyPath = $null,
    [string]$destinationEndpoint = $null,
    [string]$downloadPrefix = $null,
    [string]$destinationKey = $null,
    [string]$msiPath = $null,
    [string]$msiFilename = $null,
    [string]$authApiEndpoint = $null,
    [string]$productApiEndpoint = $null,
    [string]$productId = $null,
    [string]$authApiUsername = $null,
    [string]$authApiPassword = $null,
    [string]$productNewVersion = $null,
    [string]$setIsLatestAsString = $null,
    [string]$setIsPreReleaseAsString = $null
)

# Setup the script
$ErrorActionPreference = "Stop"
if ($setIsLatestAsString -eq $null -or $setIsLatestAsString -eq "")
{
	Write-Error "setIsLatestAsString not defined"
}
$setIsLatest = $false
if ($setIsLatestAsString.ToLower() -eq "true" -or $setIsLatestAsString.ToLower() -eq "`$true" -or $setIsLatestAsString -eq "1")
{
	$setIsLatest = $true
}
if ($setIsPreReleaseAsString -eq $null -or $setIsPreReleaseAsString -eq "") 
{
	Write-Error "setIsPreReleaseAsString not defined"
}
$setIsPreRelease = $false
if ($setIsPreReleaseAsString.ToLower() -eq "true" -or $setIsPreReleaseAsString.ToLower() -eq "`$true" -or $setIsPreReleaseAsString -eq "1")
{
	$setIsPreRelease = $true
}

# First, lets upload the file remotely
Write-Output "Uploading asset to Azure blob..."
& ($azCopyPath) ("`"/source:" + $msiPath + "`"") ("`"/dest:" + $destinationEndpoint + "`"") ("`"/DestKey:" + $destinationKey + "`"") ("`"/Pattern:" + $msiFilename + "`"") ("/Y")
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Upload to Azure failed for " + $msiFilename + " in " + $msiPath
}

# Call the auth API to get a JWT for use in subsequent API calls
Write-Output "Authenticating against LTS SSO"
$authBody = 
@{
    Username = $authApiUsername
    Password = $authApiPassword
};
$authResponse = Invoke-RestMethod -Uri $authApiEndpoint -Body (ConvertTo-Json $authBody) -ContentType "application/json" -Method Put
$jwt = $authResponse.JsonWebToken

# Now lets call the API to add this product version
Write-Output "Getting product metadata from LTS backend"
$allProductsResponse = Invoke-RestMethod -uri $productApiEndpoint -Method Get -Headers @{"Authorization" = "Bearer " + $jwt }
$foundProduct = $null
foreach ($product in $allProductsResponse)
{
    if ($product.ProductId -eq $productId)
    {
        $foundProduct = $product
        break
    }
}
if ($foundProduct -eq $null)
{
    Write-Error "Failed to find required product"
}
$versionExists = $false
if ($foundProduct.Versions -ne $null)
{
    foreach ($version in $foundProduct.Versions)
    {
        if ($version.Version -eq $productNewVersion)
        {
            $versionExists = $true
            break
        }
    }
}
if ($versionExists)
{
    Write-Error "Version already exists"
}
$downloadUrl = $downloadPrefix
if (-not $downloadPrefix.EndsWith("/"))
{
    $downloadUrl += "/"
}
$downloadUrl += $msiFilename
$newVersion =
@{
    Version = $productNewVersion
    IsLatest = $setIsLatest
    IsPreRelease = $setIsPreRelease
    DownloadUrl = $downloadUrl
};
if ($foundProduct.Versions -eq $null)
{
    $foundProduct.Versions = @($newVersion)
}
else
{
    $foundProduct.Versions += $newVersion
}
Write-Output "Updating product metadata in LTS backend"
$authResponse = Invoke-RestMethod -Uri $productApiEndpoint -Body (ConvertTo-Json $foundProduct) -ContentType "application/json" -Method Put -Headers @{"Authorization" = "Bearer " + $jwt }

# We're all done
Write-Output "Uploaded package to Azure and added to LTS backend"