# BoxBreaker3D

BoxBreaker3D は Unity + Zenject ベースのブロック崩し（3D）プロジェクトです。
ゲームロジック、データ、ビューが分離され、Signal / DI（Zenject）で疎結合に構成されています。

主な特徴
- Zenject による依存性注入と Signal ベースのイベント通信
- `HighElixir` 系のユーティリティ（StateMachine, Timers など）を利用
- UI とゲームロジックは `View` / `ViewModel` / `Models` に分割
- DOTween を使った簡易な演出

動作環境
- Unity 2019/2020 系（プロジェクトの Unity バージョンに合わせてください）
- .NET Framework 4.7.1（エディタ側のコンパイル設定）

セットアップ
1. Unity でプロジェクトを開く
2. `Window` -> `Package Manager` で必要なパッケージ（TextMeshPro 等）を確認
3. `Assets/BlockBreaker3D/Zenject/GameInstaller` 等のインストーラーでシーン依存のセットアップが行われます

実行方法（簡易）
1. Unity で該当シーン（プロジェクト内の起点シーン）を開いて Play を押す
2. ゲーム開始タイミングはプロジェクトのシーン構成に依存します。`GameStateManager.Initialize()` が呼ばれると `GameStarted` シグナルが発行され、メインの処理が開始されます

- 主要ディレクトリ（簡単）
- `Assets/BlockBreaker3D/Scripts/Datas` : 各種データ定義、Scriptable オブジェクト
- `Assets/BlockBreaker3D/Scripts/Models` : ゲーム内部のモデル・振る舞い（Ball, Block, Surface, GameState 管理等）
- `Assets/BlockBreaker3D/Scripts/UI_Input/View` : UI / 表示用スクリプト（スコア表示、ゲームオーバー表示、パドル演出など）
- `Assets/BlockBreaker3D/Zenject` : Zenject 用インストーラや依存性設定

参照すべき主要クラス
- `GameStateManager` : ゲーム状態遷移、Pause / GameOver / LevelCompleted / GameClear などのシグナル発行（`Assets/BlockBreaker3D/Scripts/Models/InGame/GameStateManager.cs`）
- `ScoreHolder` / `ScoreView` : スコア保持と UI 表示（`Assets/BlockBreaker3D/Scripts/Models/InGame/GameStatus/ScoreHolder.cs` / `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/ScoreView.cs`）
- `PaddleView` : 入力に応じたパドルの演出（`Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/PaddleView.cs`）
- `GameOverView` : ゲームオーバー時の UI（アニメーションは未実装のメソッドあり）`Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/GameOveriew.cs`
- `CompCreator` / `Comp` : ブロック等のコンポーネント生成、ダメージ管理（`Assets/BlockBreaker3D/Scripts/Models/InGame/Component/`）
- `BoxBehaviour` / `SurfaceBehaviour` / `WallBehaviour` : 物理・当たり判定 ロジック（`Assets/BlockBreaker3D/Scripts/Models/InGame/Box/` 等）

- トラブルシュート
- ビルドエラーが出る場合はまず `Assembly-CSharp` プロジェクトのコンパイルログを確認してください
- Zenject の依存注入でエラーが出る場合はインストーラ（`Zenject/Scripts`）の登録を確認してください

ライセンス
- 特に指定のない限りリポジトリ内のファイルは個別に管理されています（必要なら LICENSE ファイルを追加してください）

---
この README はプロジェクト全体を俯瞰するための簡易ガイドです。詳細は `docs/Manual.md` を参照してください。
