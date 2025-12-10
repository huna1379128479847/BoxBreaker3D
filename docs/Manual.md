# BoxBreaker3D — 簡易マニュアル

## 概要
BoxBreaker3D は 3D ブロック崩しゲームの実装プロジェクトで、以下の主要コンポーネントで構成されています。

- Models: ゲーム内部のロジックと状態（Ball, Block, Surface, GameStateManager）
- View: UI 表示、演出（ScoreView, GameOverView, PaddleView 等）
- ViewModel: ビューとモデルの仲介（スコアやカメラ制御など）
- Zenject: 依存注入と Signal を使った疎結合設計

## 操作方法
- マウス / タッチ / キーボード入力でパドルの方向を操作（実装に依存）
- ブロックを全て破壊すると LevelCompleted / GameClear が発生
- ボールを全て失うと GameOver を発行

## 主要スクリプトの説明
- `GameStateManager` (Assets/BlockBreaker3D/Scripts/Models/InGame/GameStateManager.cs)
  - ゲーム状態（Playing, Paused, GameOver, LevelCompleted, GameClear 等）を管理
  - Zenject の SignalBus を通して状態変化を発行

- `PaddleView` (Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/PaddleView.cs)
  - UI 上の演出用パドルを左右にスライドさせる
  - DOTween の Sequence を使って移動と戻りのアニメーションを行う

- `ScoreView` / `ScoreAbstractView` (Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/ScoreView.cs / ScoreAbstractView.cs)
  - スコアの表示および更新ロジック。`AbstractScoreView` は `UpdateScore(int)` を提供し、派生クラスで実装されます

- `GameOverView` (Assets/BlockBreaker3D/Scripts/UI_Input/View/InGame/GameOveriew.cs)
  - ゲームオーバー時の UI 表示とアニメーションを担当
  - `PlayGameOverAnim` は未実装のため、DOTween を使った演出を追加できます

### クラスリファレンス（抜粋）
- `GameStateManager`
  - 役割: ゲーム全体の状態管理と Zenject Signal の発行
  - 主なメソッド: `Initialize()`, `Pause(bool)`, `GameOver()`, `LevelCompleted()`, `RequestRespawn()`

- `PaddleView`
  - 役割: UI 上のパドルの見た目演出
  - 主なメソッド: `MoveToSide(bool isRight)`, `Enable(bool enable)`

- `ScoreHolder` / `ScoreView` / `AbstractScoreView`
  - 役割: スコア管理と UI 更新
  - 主なメソッド/プロパティ: `UpdateScore(int)`（ビュー側）、ホルダー側はスコア更新通知を発行

- `GameOverView`
  - 役割: ゲームオーバー UI 表示、アニメーション
  - 未実装: `PlayGameOverAnim()` を実装して、スコアのフェードやボタン表示などを行う

- `CompCreator` / `Comp`
  - 役割: ブロックやコンポーネント生成、ダメージ処理を扱う（`CompCreator.cs`, `Comp.cs`）

- `BoxBehaviour` / `SurfaceBehaviour` / `WallBehaviour`
  - 役割: ゲーム内オブジェクトの当たり判定、衝突処理、配置ロジック


## デバッグのヒント
- ステート遷移の確認は `GameStateManager` が発行する Signal を購読してログを出すと良いです。
- Zenject のバインディング確認は `Assets/BlockBreaker3D/Zenject/Scripts/GameInstaller.cs` を開いてください。
- DOTween のアニメーションが動かない場合は対象の GameObject がアクティブか、Tween の初期座標が正しいか確認してください。

## 追加タスク（提案）
- `GameOverView.PlayGameOverAnim` 実装: `_score` 表示をスケール/フェードで演出
- `ScoreAbstractView` の基底実装補完: プロジェクト内の `ScoreView` を参照し、抽象メソッドを整備

## 開発の進め方（簡易）
1. Unity でシーンを実行し、Play モードでゲーム挙動を確認
2. 修正箇所は小さくまとめてコミットする（例: UI アニメーション追加）
3. Zenject の設定変更や新しい Signal 追加時はテスト用の購読ロジックを追加して動作確認

---
このドキュメントはプロジェクトの全体像と開発者が最初に読むべきポイントをまとめたものです。必要に応じて追記してください。
