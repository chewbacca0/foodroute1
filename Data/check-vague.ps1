$path = "c:\Users\emirh\Desktop\foodroute1-maingithub\Data\restaurant-dataset-antalya-extra.json"
$j = Get-Content $path -Raw -Encoding UTF8 | ConvertFrom-Json

Write-Host "=== Antalya restaurants with vague 'Antalya' address ==="
$vague = $j | Where-Object { $_.Address -eq "Antalya" }
Write-Host "Count: $($vague.Count)"
foreach ($r in $vague) {
    Write-Host "  - $($r.RestaurantName)"
}

Write-Host ""
Write-Host "=== Istanbul check ==="
$ist = Get-Content "c:\Users\emirh\Desktop\foodroute1-maingithub\Data\restaurant-dataset-istanbul.json" -Raw -Encoding UTF8 | ConvertFrom-Json
$vague2 = $ist | Where-Object { $_.Address -eq "İstanbul" -or $_.Address -eq "Istanbul" }
Write-Host "Vague Istanbul addresses: $($vague2.Count)"
foreach ($r in $vague2) {
    Write-Host "  - $($r.RestaurantName) | Lat:$($r.Latitude) Lng:$($r.Longitude)"
}
