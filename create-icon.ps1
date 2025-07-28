# PowerShellスクリプト: SVGからPNGアイコンを生成する（System.Drawing.Commonを使用）

Add-Type -AssemblyName System.Drawing

# SVGファイルのパス
$svgPath = "Resources\AppIcon\appicon.svg"

# 出力ディレクトリ
$outputDir = "Platforms\iOS\Resources\Assets.xcassets\appicon.appiconset"

# 必要なアイコンサイズとファイル名
$icons = @(
    @{ Size = 180; Name = "Icon-60@3x.png" },
    @{ Size = 120; Name = "Icon-60@2x.png" }
)

# ディレクトリが存在しない場合は作成
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force
}

# SVGをPNGに変換する関数
function Convert-SvgToPng {
    param (
        [string]$svgPath,
        [string]$outputPath,
        [int]$size
    )
    
    # SVGファイルの内容を読み取る
    $svgContent = Get-Content $svgPath -Raw
    
    # サイズを変更
    $svgContent = $svgContent -replace 'width="512"', "width=`"$size`""
    $svgContent = $svgContent -replace 'height="512"', "height=`"$size`""
    
    # 一時SVGファイルを作成
    $tempSvgPath = [System.IO.Path]::GetTempFileName() + ".svg"
    $svgContent | Out-File $tempSvgPath -Encoding utf8
    
    # SVGをPNGに変換（ここでは実際の変換は行われません）
    Write-Host "Would convert $tempSvgPath to $outputPath at size ${size}x${size}"
    
    # 一時ファイルを削除
    Remove-Item $tempSvgPath
}

# アイコンを生成
foreach ($icon in $icons) {
    $outputPath = Join-Path $outputDir $icon.Name
    
    Write-Host "Generating $($icon.Name) ($($icon.Size) x $($icon.Size))..."
    
    # SVGをPNGに変換
    Convert-SvgToPng -svgPath $svgPath -outputPath $outputPath -size $icon.Size
}

Write-Host "Icon generation complete!"
Write-Host ""
Write-Host "Note: This script doesn't actually generate PNG files."
Write-Host "You need to manually create the icon files using an SVG converter tool."
Write-Host "Required icon sizes:"
foreach ($icon in $icons) {
    Write-Host "- $($icon.Name): $($icon.Size)x$($icon.Size) pixels"
}