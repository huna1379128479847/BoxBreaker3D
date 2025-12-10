# クラスリファレンス（プロジェクト全体一覧）

このドキュメントは `Assets/BlockBreaker3D/Scripts` 以下で定義されている主なクラス・構造体・インターフェイスの一覧と簡潔な説明を示します。ファイルパスは代表的な場所を併記しています。

---

## Datas

- `AnimationData` — `Assets/BlockBreaker3D/Scripts/Datas/Scriptable/AnimationData.cs`
  - ScriptableObject。アニメーション設定（遷移時間・イージング等）を保持。

- `ColorData` — `Assets/BlockBreaker3D/Scripts/Datas/Scriptable/Colordata.cs`
  - 色データを保持する Scriptable オブジェクト。

- `CompData` — `Assets/BlockBreaker3D/Scripts/Datas/Component/CompData.cs`
  - コンポーネント（ブロック等）に紐づく設定データ。

- `DamageData` — `Assets/BlockBreaker3D/Scripts/Datas/Component/DamageData.cs`
  - ダメージ設定（量、タイプ等）を表すデータ定義。

- `GameData` — `Assets/BlockBreaker3D/Scripts/Datas/GameData.cs`
  - ゲーム全体の設定や定数をまとめたデータクラス / Scriptable。

- `Surface` — `Assets/BlockBreaker3D/Scripts/Datas/Surface.cs`
  - マップ・面（Surface）の設定を表すデータ（レイアウトやサイズ等）。

- `SurfaceLayout` — `Assets/BlockBreaker3D/Scripts/Datas/Mapping/SurfaceLayout.cs`
  - Surface のマッピング・レイアウト定義。

- `IObjectData` — `Assets/BlockBreaker3D/Scripts/Datas/Mapping/IObjectData.cs`
  - マップ上オブジェクトのメタデータ用インターフェイス。

- `ObjectType` — `Assets/BlockBreaker3D/Scripts/Datas/ObjectType.cs`
  - ゲーム内オブジェクトの種類を示す列挙型（例: Ball / Block / Wall 等）。

### Signals (Zenject Signal 用データ)

- `GameSignal` — `Assets/BlockBreaker3D/Scripts/Datas/Signals/GameSignal.cs`
  - ゲーム状態やイベント（GameStarted / GameOver / Pause / Resume / LevelCompleted 等）を表すシグナルデータ。

- `InputSignal` — `Assets/BlockBreaker3D/Scripts/Datas/Signals/InputSignal.cs`
  - 入力イベント（タッチやクリック、スワイプ等）を表現するシグナル。

- `ObjectCollisionSignal` — `Assets/BlockBreaker3D/Scripts/Datas/Signals/ObjectCollisionSignal.cs`
  - オブジェクトの衝突発生時に送られるシグナル（衝突対象や位置などを含む）。

- `ScoreSignal` — `Assets/BlockBreaker3D/Scripts/Datas/Signals/ScoreSignal.cs`
  - スコア変化通知用のシグナル（増加量や合計スコア等）。

---

## Models / InGame

- `IObject` — `Assets/BlockBreaker3D/Scripts/Models/InGame/IObject.cs`
  - ゲーム内オブジェクトのインターフェイス。ライフサイクルや基本操作を規定。

- `ObjectBase` — `Assets/BlockBreaker3D/Scripts/Models/InGame/ObjectBase.cs`
  - `IObject` の基本実装。共通処理（位置、状態管理など）を提供。

- `GameStateManager` — `Assets/BlockBreaker3D/Scripts/Models/InGame/GameStateManager.cs`
  - ゲームの状態遷移を管理（Playing / Paused / GameOver / LevelCompleted / GameClear 等）。
  - Zenject の `SignalBus` を用いて状態変化を通知。

- `GameDataHolder` — `Assets/BlockBreaker3D/Scripts/Models/InGame/GameDataHolder.cs`
  - 実行時のゲームデータ（現在のレベル情報や設定の参照）を保持するユーティリティ。

### Ball 関連
- `BallBehaviour` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/BallBehaviour.cs`
  - ボールの物理的振る舞い、衝突処理のエントリ。

- `IBallCollisionHandler` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Interfaces/IBallCollisionHandler.cs`
  - ボールの衝突ハンドラのインターフェイス。

- `IBallComp` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Interfaces/IBallComp.cs`
  - ボールに取り付くコンポーネントのインターフェイス。

- `IBallMover` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Interfaces/IBallMover.cs`
  - ボール移動ロジックを抽象化するインターフェイス。

- `IBallTurnService` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Interfaces/IBallTurnService.cs`
  - ボールの軌道や回転に関するサービスインターフェイス。

- `BallCollisionHandler` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Comps/BallCollisionHandler.cs`
  - 衝突時の細かい処理（ダメージ計算、反射等）を扱う実装。

- `BallMover` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Comps/BallMover.cs`
  - ボールの移動制御（速度・方向の更新等）。

- `BallTurnService` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Balls/Comps/BallTurnService.cs`
  - 回転・曲がりに関連するサービスロジック。

### Blocks / Components
- `BlockBehaviour` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Blocks/BlockBehaviour.cs`
  - ブロックの当たり判定、破壊・ダメージ反映の管理。

- `Comp` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Component/Comp.cs`
  - ブロック等につくコンポーネントの基底クラス（装備・効果等）。

- `CompCreator` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Component/CompCreator.cs`
  - `Comp` の生成/配置や初期化を行うユーティリティ。

- `DamageCompInfo` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Component/DamageCompInfo.cs`
  - ダメージ用コンポーネントの補助データ（ダメージ種別や係数など）。

### その他オブジェクト
- `BoxBehaviour` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Box/BoxBehaviour.cs`
  - ゲームエリア（箱）に関する振る舞い。配置・範囲・当たり判定支援など。

- `SurfaceBehaviour` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Surface/SurfaceBehaviour.cs`
  - マップ面（Surface）の生成、レイアウト処理、当たり判定に関わるロジック。

- `WallBehaviour` — `Assets/BlockBreaker3D/Scripts/Models/InGame/Wall/WallBehaviour.cs`
  - 壁の当たり判定や反射条件の管理。

- `ScoreHolder` — `Assets/BlockBreaker3D/Scripts/Models/InGame/GameStatus/ScoreHolder.cs`
  - ゲーム内スコアを保持し、更新時に `ScoreSignal` 等で通知する中枢。

---

## UI / View / ViewModel

### ViewModel
- `BlockRemainingViewModel` — `Assets/BlockBreaker3D/Scripts/UI_Input/ViewModel/BlockRemainingViewModel.cs`
  - 残りブロック数の監視とビュー更新用の ViewModel。

- `CamViewModel` — `Assets/BlockBreaker3D/Scripts/UI_Input/ViewModel/CamViewModel.cs`
  - カメラ関連のパラメータや制御ロジックを保持する ViewModel。

- `FollowBallCam` — `Assets/BlockBreaker3D/Scripts/UI_Input/ViewModel/FollowBallCam.cs`
  - ボールを追尾するカメラの挙動実装。

- `ScoreLeaper` — `Assets/BlockBreaker3D/Scripts/UI_Input/ViewModel/ScoreLeaper.cs`
  - スコア増加時のアニメーション/演出を制御するユーティリティ。

- `TurnViewModel` — `Assets/BlockBreaker3D/Scripts/UI_Input/ViewModel/TurnViewModel.cs`
  - 手番・ターン表示に関するデータとロジック。

### View / UI
- `IGameView` / `GameViewBase` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/IGameView.cs`
  - 各種ゲーム画面（スコア表示、GameOver 画面など）に共通するインターフェイスと基底実装。

- `AbstractScoreView` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/ScoreAbstractView.cs`
  - スコア表示用の抽象基底クラス（`UpdateScore(int)` を実装すること）。

- `ScoreView` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/ScoreView.cs`
  - 実際のスコア UI 表示の実装（`ScoreHolder` からの通知を受け取る）。

- `GameOverView` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/GameOveriew.cs`
  - ゲームオーバー時の UI と演出（Score の表示アニメーション等）。

- `PaddleView` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/PaddleView.cs`
  - UI 上のパドル演出（入力に応じた左右移動と復帰アニメーション）。

- `TurnHandler` / `TurnView` — `Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/TurnHandler.cs`, `TurnView.cs`
  - ターンや残り手数表示の UI ロジックを担当。

---

## Zenject / Integration

- `BallInject` — `Assets/BlockBreaker3D/Zenject/Scripts/GO/BallInject.cs`
  - Ball オブジェクトの Zenject バインディング / 注入をサポートする補助スクリプト。

- Zenject インストーラ類（`Assets/BlockBreaker3D/Zenject/Scripts/*.cs`）
  - シーン/ゲーム内で必要な依存性バインディングや Signal の登録を行うクラス群。

---

## Editor

- `SurfaceEditor` — `Assets/BlockBreaker3D/Editor/InGame/SurfaceEditor.cs`
  - Surface（面）をエディタ上で編集するためのカスタムエディタ実装。

---

## 備考
- 上記は `Assets/BlockBreaker3D/Scripts` 配下で主に使用されるクラスの一覧です。asmdef や補助的な小規模クラスは省略している場合があります。
- 各クラスのより詳細な仕様や公開メソッド一覧が必要であれば、該当ファイルを解析してメソッド一覧（引数・戻り値・説明）を自動生成できます。

---

必要ならこのファイルを英語版に変換、あるいは自動的に API 参照（メソッド一覧）を追記して更新します。