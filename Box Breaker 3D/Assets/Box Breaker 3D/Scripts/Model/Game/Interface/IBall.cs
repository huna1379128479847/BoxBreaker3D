using BoxBreaker3D.Data;

namespace BoxBreaker3D.Model.Interfaces
{
    public interface IBall : IModel
    {
        float Speed { get; set; }
        BallSurface CurrentSurface { get; set; }
        IBox CurrentBox { get; set; }
    }
}
