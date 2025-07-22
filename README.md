# HelloWorld - .NET MAUI ブランディングカスタマイズプロジェクト

## 概要

このプロジェクトは、.NET MAUIを使用したクロスプラットフォームアプリケーションで、アプリアイコンとスプラッシュスクリーンのカスタマイズ機能を実装したデモンストレーションアプリです。Android、iOS、Windowsプラットフォームでの一貫したブランディング体験を提供します。

## 🎯 主な機能

- **クロスプラットフォーム対応**: Android、iOS、Windows（MacCatalyst除外）
- **カスタムアプリアイコン**: SVG形式による高品質なアイコン表示
- **カスタムスプラッシュスクリーン**: ブランド統一されたアプリ起動画面
- **アダプティブアイコン**: Android向けの動的アイコン対応
- **包括的テストスイート**: ブランディング機能の検証テスト
- **ビルド時検証**: リソースファイルの自動検証システム

## 🛠️ 技術スタック

- **.NET 8.0**
- **.NET MAUI (Multi-platform App UI)**
- **C# 12**
- **XAML**
- **SVG** (ベクターグラフィックス)

## 📱 対象プラットフォーム

| プラットフォーム | バージョン | 状態 |
|----------------|------------|------|
| Android | API Level 21+ (Android 5.0+) | ✅ サポート |
| iOS | iOS 11.0+ | ✅ サポート |
| Windows | Windows 10 Version 2004+ | ✅ サポート |
| MacCatalyst | - | ❌ 意図的に除外 |

## 🚀 クイックスタート

### 前提条件

- Visual Studio 2022 17.8+ または Visual Studio Code
- .NET 8.0 SDK
- 各プラットフォーム向けの開発環境:
  - **Android**: Android SDK、エミュレーターまたは実機
  - **iOS**: Xcode (macOS)、iOS シミュレーターまたは実機
  - **Windows**: Windows 10/11 開発環境

### インストールと実行

1. **リポジトリのクローン**
   ```bash
   git clone <repository-url>
   cd HelloWorld
   ```

2. **依存関係の復元**
   ```bash
   dotnet restore
   ```

3. **ビルドと実行**
   ```bash
   # Android
   dotnet build -f net8.0-android
   
   # iOS
   dotnet build -f net8.0-ios
   
   # Windows
   dotnet build -f net8.0-windows10.0.19041.0
   ```

## 📁 プロジェクト構造

```
HelloWorld/
├── 📄 HelloWorld.csproj          # プロジェクト設定とブランディング設定
├── 📄 MauiProgram.cs             # アプリケーション初期化とブランディング検証
├── 📄 MainPage.xaml              # メインUI（テスト実行ボタン含む）
├── 📄 BuildValidation.cs         # ビルド時リソース検証システム
├── 📁 Resources/
│   ├── 📁 AppIcon/
│   │   ├── 🎨 appicon.svg        # メインアプリアイコン (512x512px)
│   │   └── 🎨 appiconfg.svg      # アダプティブアイコン用フォアグラウンド
│   └── 📁 Splash/
│       └── 🎨 splash.svg         # スプラッシュスクリーン画像
├── 📁 Tests/                     # 包括的テストスイート
│   ├── 📄 AppIconTests.cs        # アプリアイコンテスト
│   ├── 📄 SplashScreenTests.cs   # スプラッシュスクリーンテスト
│   ├── 📄 PlatformSpecificTests.cs # プラットフォーム固有テスト
│   ├── 📄 BrandingIntegrationTests.cs # 統合テスト
│   └── 📄 TestRunner.cs          # テスト実行エンジン
└── 📁 Platforms/                 # プラットフォーム固有設定
    ├── 📁 Android/
    ├── 📁 iOS/
    └── 📁 Windows/
```

## 🎨 ブランディング設定

### カラーパレット
- **プライマリカラー**: `#512BD4` (ブランド紫)
- **グラデーション**: `#667eea` → `#764ba2`
- **テキストカラー**: `#ffffff` / `#f8f9fa`

### リソースファイル仕様

| ファイル | 用途 | 形式 | 推奨サイズ |
|---------|------|------|-----------|
| `appicon.svg` | メインアプリアイコン | SVG | 512x512px |
| `appiconfg.svg` | アダプティブアイコン前景 | SVG | 512x512px |
| `splash.svg` | スプラッシュスクリーン | SVG | 正方形 |

## 🧪 テスト機能

アプリケーション内で以下のテストを実行できます：

### テストカテゴリ
- **アプリアイコンテスト**: アイコン表示と設定の検証
- **スプラッシュスクリーンテスト**: 起動画面の表示と設定検証
- **プラットフォーム固有テスト**: 各プラットフォームでの動作検証
- **統合テスト**: 全体的なブランディング機能の検証
- **リソース検証テスト**: ファイル存在とフォーマット検証

### テスト実行方法

#### アプリ内実行
1. アプリを起動
2. 対応するテストボタンをタップ
3. 結果をアラートとデバッグ出力で確認

#### コマンドライン実行
```bash
# 全テスト実行
dotnet test

# ブランディング関連テストのみ
dotnet test --filter "Category=Branding"
```

## 🔧 開発ガイド

### ブランディングリソースの変更

1. **アイコン変更時**:
   - `Resources/AppIcon/appicon.svg` を更新
   - `Resources/AppIcon/appiconfg.svg` を更新（Android用）
   - ビルドテストを実行
   - 各プラットフォームで表示確認

2. **スプラッシュスクリーン変更時**:
   - `Resources/Splash/splash.svg` を更新
   - 色設定確認（`Color="#512BD4"`）
   - ビルドテストを実行
   - 起動時の表示確認

### ビルド検証

プロジェクトには自動ビルド検証システムが組み込まれています：

```csharp
// MauiProgram.cs内で自動実行
ValidateBrandingResources();
```

検証項目：
- ✅ プロジェクト設定の妥当性
- ✅ リソースファイルの存在
- ✅ SVGファイルの形式検証
- ✅ ディレクトリ構造の確認
- ✅ 色指定の形式チェック

## 📚 ドキュメント

詳細な実装情報については以下のドキュメントを参照してください：

- **[BRANDING_CUSTOMIZATION.md](BRANDING_CUSTOMIZATION.md)**: 詳細実装ガイド
- **[BRANDING_QUICK_REFERENCE.md](BRANDING_QUICK_REFERENCE.md)**: クイックリファレンス
- **[Tests/README.md](Tests/README.md)**: テストスイート詳細

## 🐛 トラブルシューティング

### よくある問題

#### ビルドエラー
```
エラー: 指定されたファイルが存在しません
解決: Resources/AppIcon/ と Resources/Splash/ のファイル存在確認
```

#### SVGファイルエラー
```
エラー: SVGファイルの解析に失敗
解決: XML構文とSVG仕様への準拠確認
```

#### 色指定エラー
```
エラー: 無効な色形式
解決: #RRGGBB形式での16進数指定使用
```

### デバッグ情報

ログ出力場所：
- **Visual Studio**: デバッグ出力ウィンドウ
- **VS Code**: デバッグコンソール
- **コマンドライン**: 標準出力

## 🤝 コントリビューション

1. フォークを作成
2. フィーチャーブランチを作成 (`git checkout -b feature/amazing-feature`)
3. 変更をコミット (`git commit -m 'Add amazing feature'`)
4. ブランチにプッシュ (`git push origin feature/amazing-feature`)
5. プルリクエストを作成

## 📄 ライセンス

このプロジェクトはMITライセンスの下で公開されています。詳細は[LICENSE](LICENSE)ファイルを参照してください。

## 🙏 謝辞

- Microsoft .NET MAUI チーム
- .NET コミュニティ
- SVGアイコンデザインリソース

---

**注意**: このプロジェクトは教育・デモンストレーション目的で作成されており、.NET MAUIでのブランディングカスタマイズのベストプラクティスを示しています。