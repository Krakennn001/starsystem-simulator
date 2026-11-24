namespace StarSystemSimulator.Entities.Planets
{
    public interface IPlanet
    {
        double Radius { get; }  // в км
        double Weight { get; }  // в кг
        List<Satellite> Satellites { get; }
        double ToStarDistance { get; }  // в км
        double Year { get; }    // в днях
    }
}
