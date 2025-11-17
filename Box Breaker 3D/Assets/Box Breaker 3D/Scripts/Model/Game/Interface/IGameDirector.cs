namespace BoxBreaker3D.Model.Interfaces
{
    public interface IGameDirector : IModel
    {
        GameContext GameContext { get; }
        IBall Ball { get; set; }
    }
}