# PowerShellスクリプト: SVGからiOS用アイコンを生成する
# このスクリプトを実行するには、ImageMagickがインストールされている必要があります
# インストール方法: https://imagemagick.org/script/download.php

# SVGファイルのパス
$svgPath = "Resources\AppIcon\appicon.svg"

# 出力ディレクトリ
$outputDir = "Platforms\iOS\Resources\Assets.xcassets\appicon.appiconset"

# 必要なアイコンサイズとファイル名
$icons = @(
    @{ Size = 20; Scale = 1; Name = "Icon-20.png" },
    @{ Size = 20; Scale = 2; Name = "Icon-40.png" },
    @{ Size = 20; Scale = 3; Name = "Icon-60.png" },
    @{ Size = 29; Scale = 1; Name = "Icon-29.png" },
    @{ Size = 29; Scale = 2; Name = "Icon-58.png" },
    @{ Size = 29; Scale = 3; Name = "Icon-87.png" },
    @{ Size = 40; Scale = 1; Name = "Icon-40.png" },
    @{ Size = 40; Scale = 2; Name = "Icon-80.png" },
    @{ Size = 40; Scale = 3; Name = "Icon-120.png" },
    @{ Size = 60; Scale = 2; Name = "Icon-120.png" },
    @{ Size = 60; Scale = 3; Name = "Icon-180.png" },
    @{ Size = 76; Scale = 1; Name = "Icon-76.png" },
    @{ Size = 76; Scale = 2; Name = "Icon-152.png" },
    @{ Size = 83.5; Scale = 2; Name = "Icon-167.png" },
    @{ Size = 1024; Scale = 1; Name = "Icon-1024.png" }
)

# ディレクトリが存在しない場合は作成
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force
}

# ImageMagickを使用してアイコンを生成
foreach ($icon in $icons) {
    $size = $icon.Size * $icon.Scale
    $outputPath = Join-Path $outputDir $icon.Name
    
    Write-Host "Generating $($icon.Name) ($size x $size)..."
    
    # ImageMagickのコマンドを実行
    magick convert -background none -density 1200 -resize ${size}x${size} $svgPath $outputPath
}

Write-Host "Icon generation complete!"