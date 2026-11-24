using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Entities.Stars
{
    public interface IStar
    {
        double Weight { get; }      // в кг
        int Temperature { get; }    // в Кельвинах
        double Radius { get; }      // в км
        public Color GlowColor { get; }
    }
}
