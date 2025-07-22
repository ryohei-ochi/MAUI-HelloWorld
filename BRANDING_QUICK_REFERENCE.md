# ブランディングカスタマイズ クイックリファレンス

## ファイル構造

```
HelloWorld/
├── HelloWorld.csproj          # プロジェクト設定とブランディング設定
├── BRANDING_CUSTOMIZATION.md  # 詳細実装ガイド
├── BRANDING_QUICK_REFERENCE.md # このファイル
└── Resources/
    ├── AppIcon/
    │   ├── appicon.svg        # メインアプリアイコン (512x512px推奨)
    │   └── appiconfg.svg      # アダプティブアイコン用フォアグラウンド
    └── Splash/
        └── splash.svg         # スプラッシュスクリーン (正方形推奨)
```

## 重要な設定値

### プロジェクト設定 (HelloWorld.csproj)
```xml
<!-- 対象プラットフォーム (MacCatalyst除外) -->
<TargetFrameworks>net8.0-android;net8.0-ios;net8.0-windows10.0.19041.0</TargetFrameworks>

<!-- アプリアイコン設定 -->
<MauiIcon Include="Resources\AppIcon\appicon.svg" 
          ForegroundFile="Resources\AppIcon\appiconfg.svg" 
          Color="#512BD4" />

<!-- スプラッシュスクリーン設定 -->
<MauiSplashScreen Include="Resources\Splash\splash.svg" 
                  Color="#512BD4" 
                  BaseSize="128,128" />
```

### ブランドカラー
- **プライマリ**: `#512BD4` (ブランド紫)
- **グラデーション**: `#667eea` → `#764ba2`
- **テキスト**: `#ffffff` / `#f8f9fa`

## ファイル命名規則

| ファイル | 用途 | 形式 | サイズ |
|---------|------|------|--------|
| `appicon.svg` | メインアプリアイコン | SVG | 512x512px |
| `appiconfg.svg` | アダプティブアイコン前景 | SVG | 512x512px |
| `splash.svg` | スプラッシュスクリーン | SVG | 正方形 |

## プラットフォーム別動作

### Android
- **アダプティブアイコン**: `appiconfg.svg` + 背景色
- **スプラッシュ**: Splash Screen API統合
- **生成ファイル**: `mipmap-*/ic_launcher*.png`

### iOS  
- **アプリアイコン**: `appicon.svg`から自動生成
- **起動画面**: Launch Screen storyboard
- **生成ファイル**: `Assets.xcassets/AppIcon.appiconset/`

### Windows
- **アプリタイル**: スタートメニュー用
- **スプラッシュ**: UWPスタイル
- **生成ファイル**: `Images/Square*.png`

## よくあるエラーと解決方法

### ビルドエラー
```
エラー: 指定されたファイルが存在しません
解決: Resources/AppIcon/ と Resources/Splash/ のファイル存在確認
```

### SVGエラー
```
エラー: SVGファイルの解析に失敗
解決: XML構文とSVG仕様への準拠確認
```

### 色指定エラー
```
エラー: 無効な色形式
解決: #RRGGBB形式での16進数指定使用
```

## 検証コマンド

### ビルド前検証
```bash
# プロジェクトビルド
dotnet build

# 特定プラットフォーム
dotnet build -f net8.0-android
dotnet build -f net8.0-ios
dotnet build -f net8.0-windows10.0.19041.0
```

### テスト実行
```bash
# 全テスト実行
dotnet test

# ブランディング関連テストのみ
dotnet test --filter "Category=Branding"
```

## デバッグ情報

### ログ出力場所
- **Visual Studio**: デバッグ出力ウィンドウ
- **VS Code**: デバッグコンソール
- **コマンドライン**: 標準出力

### 検証ログ例
```
Starting build validation for branding resources...
✓ Project file validation passed
✓ Resource files validation passed  
✓ Directory structure validation passed
Build validation completed. Errors: 0, Warnings: 0
```

## 変更時のチェックリスト

### アイコン変更時
- [ ] `appicon.svg` 更新
- [ ] `appiconfg.svg` 更新 (Android用)
- [ ] ビルドテスト実行
- [ ] 各プラットフォームでの表示確認

### スプラッシュスクリーン変更時  
- [ ] `splash.svg` 更新
- [ ] 色設定確認 (`Color="#512BD4"`)
- [ ] ビルドテスト実行
- [ ] 起動時の表示確認

### プロジェクト設定変更時
- [ ] `.csproj` ファイル更新
- [ ] ビルド検証実行
- [ ] 全プラットフォームテスト
- [ ] ドキュメント更新

## サポートリソース

- **詳細ガイド**: `BRANDING_CUSTOMIZATION.md`
- **テストコード**: `Tests/` フォルダ
- **検証ツール**: `BuildValidation.cs`
- **エラーハンドリング**: `Tests/ResourceErrorHandler.cs`

---
*このクイックリファレンスは開発効率向上のための要点をまとめています。詳細な実装情報は BRANDING_CUSTOMIZATION.md を参照してください。*