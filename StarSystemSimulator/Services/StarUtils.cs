using StarSystemSimulator.Utils;

namespace StarSystemSimulator.Services
{
    public static class StarUtils
    {
        public static (Color, SpectralClass) GetSpectralData(int temp) =>
            temp < 3700  ? (Color.Red, SpectralClass.M) :
            temp < 5200  ? (Color.Orange, SpectralClass.K) :
            temp < 6000  ? (Color.Yellow, SpectralClass.G) :
            temp < 7500  ? (Color.LightYellow, SpectralClass.F) :
            temp < 10000 ? (Color.White, SpectralClass.A) :
            temp < 30000 ? (Color.LightBlue, SpectralClass.B) :
                           (Color.Blue, SpectralClass.O);

        public static AsteroidType GetAsteroidType(double radius, double weight) =>
            (radius < 10 && weight < 1e17) ? AsteroidType.Carbonaceous :
            (radius < 30 && weight < 1e18) ? AsteroidType.Silicate :
                                             AsteroidType.Metallic;
    }
}
