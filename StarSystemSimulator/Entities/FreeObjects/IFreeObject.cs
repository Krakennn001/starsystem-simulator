namespace StarSystemSimulator.Entities.FreeObjects
{
    public interface IFreeObject
    {
        double Radius { get; }  // в км
        double ToStarDistance { get; }  // в км
        double Weight { get; }  // в кг
        double Speed { get; }  // в км/c
    }
}
