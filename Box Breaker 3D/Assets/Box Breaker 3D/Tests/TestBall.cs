//using BoxBreaker3D.Data;
//using BoxBreaker3D.Model.Interfaces;
//using HighElixir.Timers;
//using UnityEngine;

//public class TestBall : MonoBehaviour, IBall
//{
//    [SerializeField] private Surface _direction;
//    [SerializeField] private float _speed;
//    [SerializeField] private Vector2 _dir;
//    private BallSurface _surface = new(Surface.Right, Vector2.right, Vector3.zero);
//    private TimerTicket _counter;

//    public float Speed { get => _speed; set => _speed = value; }
//    public BallSurface CurrentSurface { get => _surface; set => _surface = value; }
//    public IBox CurrentBox { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
//    public Vector3 Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

//    public ObjectInfo Info => throw new System.NotImplementedException();

//    public void Pause()
//    {
//    }

//    public void Resume()
//    {
//    }

//    public void UpdateModel(float deltaTime)
//    {
//        var current = transform.position;
//        transform.position = current + _surface.GetWorldDirection() * deltaTime * _speed;
//    }

//    private void Awake()
//    {
//        _counter = GlobalTimer.FixedUpdate.PulseRegister(1.5f, "方向転換タイマー", andStart: true,
//            onPulse: () =>
//            {
//                var prev = _surface.Face;

//                _surface.Face = prev switch
//                    {
//                        Surface.Front => Surface.Right,
//                        Surface.Right => Surface.Backward,
//                        Surface.Backward => Surface.Left,
//                        Surface.Left => Surface.Front,
//                        _ => prev
//                    };

//                // 面が変わったらローカル方向を回転！
//                //_surface.RotateLocalDir90(clockwise: true);

//            Debug.Log($"Changed {prev} → {_surface.Face}, LocalDir = {_surface.LocalDir}");
//    });

//    }

//    // Update is called once per frame
//    private void FixedUpdate()
//    {
//        UpdateModel(Time.fixedDeltaTime);
//    }

//    private void OnValidate()
//    {
//        _surface.Face = _direction;
//        _surface.LocalDir = _dir;
//    }
//}
