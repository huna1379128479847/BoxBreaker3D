# BlockBreaker3D.Models API Reference

このドキュメントは `BlockBreaker3D.Models` プロジェクト内の主要な公開型のAPIリファレンスをまとめたものです。
各型について、ファイルパス、公開プロパティ、公開メソッド、簡易説明と使用上の注意点を記載しています。

---

## 目次
- BoxBehaviour
- BallBehaviour
- SurfaceBehaviour
- BlockBehaviour
- GameDataHolder
- ObjectBase
- IObject

---

## BoxBehaviour
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/Box/BoxBehaviour.cs`
- 継承: `ObjectBase`
- 概要: ゲームの箱（プレイフィールド）を表す主要コンポーネント。サーフェスの管理、ボールの遷移、ブロック残数チェックなどを行う。

公開プロパティ/フィールド:
- `float TransitionInset { get; }` - サーフェス遷移時にボールを内側に押し込むためのマージン。
- `SurfaceBehaviour DefaultSurface { get; internal set; }` - デフォルトでボールをスポーンするサーフェス。
- `IReadOnlyReactiveProperty<SurfaceBehaviour> CurrentSurface { get; }` - 現在ボールが存在するサーフェスのリアクティブプロパティ。
- `bool IsEnabled { get; set; }` - BoxBehaviour の有効フラグ（シグナル受信時に使用）。
- `Material SharedOnOutSurface { get; }` - ボールが出た面に適用する共有マテリアル。

公開メソッド:
- `void Constract(SignalBus signalBus, GameDataHolder dataHolder)` - Zenject で注入される初期化メソッド。データバインドとゲームシグナルの購読を行う。
- `void EnterBall(BallBehaviour ball)` - ボールが箱に入る際に呼び出す。ボールをバインドする。
- `bool Transition(string target)` - 指定した名前のサーフェスへボールを遷移させる。遷移が成功したら true を返す。遷移クールダウンを考慮する。
- `int GetTotalBlockCount()` - 全サーフェス上のブロック合計数を返す。
- `void CheckClear()` - ブロックが全て破壊されているか判定し、条件を満たせばシグナルを送出する。

保護/内部:
- `protected override void OnReset()` - リセット処理。全サーフェス、ボールを初期状態に戻し、デフォルトサーフェスにスポーンさせる。

注意:
- サーフェス遷移処理では `BallBehaviour.CanTransition` を参照し、`MarkTransition` を呼んで二重遷移を防ぐ。
- `Constract` 内で `DefaultSurface` が null の場合は `_surfaces[0]` がデフォルトに設定される。

---

## BallBehaviour
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/Balls/BallBehaviour.cs`
- 継承: `ObjectBase`
- 概要: プレイヤーが操作するボールのロジックを持つ。移動（Mover）・当たり判定（CollisionHandler）などのCompを持ち、状態の管理、ターン（方向変更）やダメージ処理、スポーン/デスポーンアニメ再生を行う。

公開プロパティ/フィールド:
- `int HP { get; internal set; }` - ボールの体力（残機に相当）。
- `Vector2 Direction { get; internal set; }` - ボールの2D方向（正規化される）。
- `Surface CurrentSurface { get; internal set; }` - 現在のサーフェス（列挙型 `Surface`）
- `float Speed { get; internal set; }` - ボールの速度。
- `float TurningAngle { get; internal set; }` - 回転角度（ターン時に使用）。
- `IReactiveProperty<int> TurnRemaining { get; }` - 残りターン数を示すリアクティブプロパティ。
- `int MaxTurn { get; }` - 最大ターン数。

公開メソッド:
- `void MarkTransition()` - 遷移時刻を記録し、すぐの再遷移を抑止する。
- `bool CanTransition(float cooldown)` - 指定クールダウンが経過しているか判定する。
- `void Construct(SignalBus signalBus, AnimationData animationData, IBallTurnService turnService, GameDataHolder holder, GameStateManager gameStateManager, BallMoverData mover = null, BallCollisionHandlerData collisionHandler = null)` - Zenject注入用初期化。Compの初期化やイベント購読を行う。
- `void TakeDamage(int damage)` - ダメージ処理。HPを減らし、デスポーンアニメを再生。残機0未満ならゲームオーバー処理。
- `UniTask PlaySpawnAnimation()` / `void StopSpawnAnimation()` - スポーンエフェクトの再生/停止。
- `UniTask PlayDespawnAnimation()` / `void StopDespawnAnimation()` - デスポーンエフェクトの再生/停止。
- `void ReverseX()` / `void ReverseY()` - 方向の反転。
- `void Reflect()` - Mover による反射処理を呼ぶ。

保護/内部:
- `protected override void NotifyCollisionEnter(Collision collision, ObjectType otherType)` - 衝突通知。ターン残数の増加を行う。
- `protected override void OnReset()` (エディタ限定) - 初期状態へ戻す。

注意:
- `Construct` で `BallMover` と `BallCollisionHandler` を Comp として登録する。外から `BallMoverData` や `BallCollisionHandlerData` を渡すことで差し替え可能。
- アニメは `AnimationData` の `OnSpawn` / `OnDespawn` に依存する。

---

## SurfaceBehaviour
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/Surface/SurfaceBehaviour.cs`
- 継承: `ObjectBase`
- 概要: 1つの面（Surface）を表す。サーフェス上のブロック管理、ボールのスポーンやクランプ、ワールド/ローカル座標変換、サーフェス間遷移の入り口判定などを担う。

公開プロパティ/フィールド:
- `string SurfaceType { get; }` - サーフェスタイプ識別子。
- `Surface SurfaceData { get; }` - Surface データ構造（座標変換や出口判定を提供）。
- `Vector3 SurfaceOrigin { get; }` - サーフェス基準位置（ワールド）。
- `Vector3 CamRotate { get; }` - UIの回転保持（ブロック残数表示用）。
- `Vector2 Size { get; }` - サーフェスのローカルサイズ（幅, 高さ）。
- `IReadOnlyReactiveProperty<Vector2> BallLocalPosition { get; }` - 表面上のボール座標（ローカル2D）。
- `int BlockRemainCount { get; }` - 登録されているブロック数（Alive含む）。
- `bool IsHoldingBall { get; }` - 現在ボールが面上に存在するかどうか。

公開メソッド:
- `void Constract(SignalBus sig)` - Zenject注入。SurfaceData初期化やUIテキスト設定。
- `Vector3 SurfaceLocalToWorld(Vector2 localPos)` - Surfaceローカル2D座標をワールド座標に変換。
- `Vector2 SurfaceWorldToLocal(Vector3 worldPos)` - ワールド座標をSurfaceローカル2D座標へ変換。
- `Vector3 SurfaceUVToWorld(Vector2 uv)` - UV(0..1)をSurfaceローカル領域にマップしてワールド座標を返す。
- `void SpawnThis(BallBehaviour ballBehaviour)` - この面へボールをスポーンする（位置探索・当たり判定を行う）。
- `void ClampTo()` - 現在のボール位置をこの面の領域内にクランプして再配置する（遷移直後の即時再退出防止に使用）。
- `void EnterThis(BallBehaviour ballBehaviour)` - 面にボールが入った際の処理（退出条件の監視登録など）。
- `void ExitThis()` - 面からボールが離れた際の処理（ブロックの覆いマテリアルの適用など）。
- `void SetCoveredMaterial(Material mat)` - 面上の全ブロックに覆いマテリアルを適用。

ブロック管理:
- `void RegisterBlock(BlockBehaviour block)` - 面にブロックを登録。
- `void UnregisterBlock(BlockBehaviour block)` - ブロックを登録解除（破壊時に BoxBehaviour のクリア判定を呼ぶ）。
- `int GetBlockCount()` - 生存中のブロック数を返す。
- `void NotyfyBreaked()` - ブロック破壊通知。全破壊ならシグナルを発する。

保護/内部:
- `protected override void NotifyFixedUpdate(float deltaTime)` - FixedUpdate相当でボールの位置を監視しローカル座標を更新する。
- `protected override void OnReset()` - リセット処理（登録ブロックの状態戻し、内部状態初期化）。

注意:
- `EnterThis` は _ballLocalPosition の購読を内部で作成し AddTo(this) しているため、this（SurfaceBehaviour）が破棄/無効化されると購読が解放される。
- `ClampTo` ではボールのコライダ半径と `BoxBehaviour.TransitionInset` を用いて適切なinsetを計算する。

---

## BlockBehaviour
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/Blocks/BlockBehaviour.cs`
- 継承: `ObjectBase`
- 概要: ブロックの振る舞い。ダメージ判定、スコア加算、破壊処理、リセット機能を持つ。

公開プロパティ/フィールド:
- `bool IsAlive { get; set; }` - 生存状態（内部 HaveLifeComp を通じて管理）。
- `int HP { get; internal set; }` - ブロックHP。

公開メソッド:
- `void Construct(ScoreHolder score)` - Zenject 注入。Surface/Boxとの紐付け、HaveLifeComp の追加、初期登録などを行う。
- `void DestroyBlock(bool addScore = true, bool playEffect = true)` - ブロック破壊処理。スコア追加、エフェクト再生、非表示化を行う。
- `void Refresh()` - HaveLifeComp をリセットして復活可能にする。
- `void SetCoveredMaterial(Material mat)` - 覆い用のコピーオブジェクトを作成してマテリアルを変更する（被覆表示）。
- `void ResetMaterial()` - 被覆オブジェクトを非表示に戻す。

保護/内部:
- `protected override void NotifyCollisionEnter(Collision collision, ObjectType otherType)` - Ball との衝突を検知してライフを減らす。
- `protected override void OnReset()` - リセット処理（Surface 登録、GameObject 状態復帰、Comp のリフレッシュなど）。

注意:
- `SetCoveredMaterial` はオブジェクトのコピーを作りマテリアルを差し替える方式で、元のブロック衝突は無効化されるようにしている。

---

## GameDataHolder
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/GameStatus/GameDataHolder.cs`
- 概要: ゲーム全体で共有するデータの Holder。Zenject で注入されるシングルトン的役割。

公開プロパティ/フィールド:
- `ScoreHolder ScoreHolder { get; private set; }` - スコア管理オブジェクト。
- `BallBehaviour BallBehaviour { get; }` - 登録されたボールインスタンス。
- `IReadOnlyReactiveProperty<BoxBehaviour> BoxBehaviour { get; }` - 登録された BoxBehaviour をリアクティブで参照。
- `SignalBus SignalBus { get; set; }` - 注入されたSignalBus（公開セッタあり）。

公開メソッド:
- `void SetScoreHolder(ScoreHolder scoreHolder)` - ScoreHolder を差し替え。
- `void BindBall(BallBehaviour ballBehaviour)` - BallBehaviour を登録。
- `void BindBox(BoxBehaviour boxBehaviour)` - BoxBehaviour を登録（ReactiveProperty に格納）。

注意:
- コンストラクタで `SignalBus` と `ScoreHolder` を受け取り初期化する。

---

## ObjectBase
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/ObjectBase.cs`
- 概要: ゲーム内のオブジェクトが共通で持つ基本機能 (IObject 実装、Comp 管理等) を提供する抽象基底クラス。

主なメンバー (抜粋):
- `IObject BoxObject { get; set; }` - このオブジェクトが所属する箱（Box）を示す。
- protected メンバー `_compDatas`, `_comps` などでコンポーネント群を管理。

注意:
- 多くの振る舞いは継承先クラスでオーバーライドされる（`NotifyCollisionEnter` や `OnReset` など）。

---

## IObject
- ファイル: `Assets/BlockBreaker3D/Scripts/Models/InGame/Object/IObject.cs`
- 概要: `BoxObject` プロパティを持つ簡易インターフェース。ゲームオブジェクト間の親子/所属関係を示すために用いられる。

メンバー:
- `IObject BoxObject { get; set; }`

---

# 使用上の注記
- 本ドキュメントはソースコードの公開メンバーとコメントから要約して作成しています。実際の挙動や内部実装の詳細は該当ソースコードを参照してください。
- 他のアセンブリ（UniRx, Zenject, HighElixir など）は外部ライブラリとして多くの型を提供しており、それらはここではまとめていません。

---

このファイルは自動生成的に作成されました。追加で各アセンブリ全体（例えば `BlockBreaker3D.View` や外部パッケージ）を含めた完全なAPIドキュメント生成を希望する場合は、対象アセンブリを指定してください。

---

## 追加ドキュメント: View / ViewModel / Datas
以下ではワークスペース内の `BlockBreaker3D.View`, `BlockBreaker3D.ViewModel`, `BlockBreaker3D.Datas` に含まれる代表的な型と用途を簡潔にまとめます。必要であれば各プロジェクトごとに別ファイルで詳細化できます。

---

## BlockBreaker3D.View
- 代表ファイルパス例: `Assets/BlockBreaker3D/Scripts/View/InGame/...`
- 概要: ゲームのプレゼンテーション層。Unity の `MonoBehaviour` を使った UI と入力ハンドリング、ビューのライフサイクルを担う。

代表的な型:
- `PaddleView` (`Assets/.../UI/PaddleView.cs`) - パドル（プレイヤー入力で動くオブジェクト）の表示・操作を行うビュー。入力を受け取り ViewModel や Signal に変換する役割を持つ。
- `TurnHandler` (`Assets/.../Input/TurnHandler.cs`) - 入力イベント（回転 / ターン操作）を受け取り、`InputSignal` 等へ変換して再配布するハンドラ。
- `IGameView` (`Assets/.../IGameView.cs`) - ゲームビューの抽象インターフェース。複数のビュー実装間で共通の操作契約を提供する。
- `ScoreView`, `TurnView` (`Assets/.../UI/ScoreView.cs`, `Assets/.../UI/TurnView.cs`) - スコア表示や残ターン表示など、ViewModel の状態を描画するコンポーネント。

注意:
- View 層は主に Unity のコンポーネント、シリアライズフィールド、アニメーション、エフェクトの参照を持ち、ロジックは出来るだけ ViewModel / SignalBus に委譲する設計になっています。

---

## BlockBreaker3D.ViewModel
- 代表ファイルパス例: `Assets/BlockBreaker3D/Scripts/ViewModel/...`
- 概要: プレゼンテーションロジックと UI バインディングを担う。ReactiveProperty やコマンドを用いて View と Model 間のデータ同期を行う。

代表的な型:
- `ScoreLeaper` (`Assets/.../ViewModel/ScoreLeaper.cs`) - スコア変化時のアニメーション・イージングロジックを提供するユーティリティ/コンポーネント。
- `TurnViewModel` (`Assets/.../ViewModel/TurnViewModel.cs`) - 残ターンやターン入力の状態を保持する ViewModel。View からの操作を受け付けて Model 側へ通知する。

注意:
- ViewModel は UniRx の `ReactiveProperty` 等を用いるため、View 側は購読して状態変化に応答することで疎結合を保っています。

---

## BlockBreaker3D.Datas
- 代表ファイルパス例: `Assets/BlockBreaker3D/Scripts/Datas/...`
- 概要: ゲームデータの定義（ScriptableObject、シリアライズ可能データ、Signal 型、Comp 用データなど）。Model 層や View に注入される設定を保持する。

代表的な型:
- `AnimationData` - スポーン/デスポーン等のエフェクト参照を保持する ScriptableObject 型（`BallBehaviour` のアニメ参照などで利用）。
- `UnlockWithData`, `ValidateViewData` (`Assets/.../Datas/Component/*`) - コンポーネントやビューの検証、条件付きロックデータを保持するデータ型。
- `Signals` (`Assets/.../Datas/Signals/*`) - `GameSignal`, `InputSignal` 等のシグナル定義が含まれる。SignalBus 経由でシステム間通信に用いられる。

注意:
- Datas 層は設定値やデータ構造のみを持ち、ロジックは極力持たない設計です。`ScriptableObject` を使った差し替えやデータ駆動の挙動をサポートします。