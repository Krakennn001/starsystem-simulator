namespace StarSystemSimulator.Utils
{
    public enum AsteroidType
    {
        Carbonaceous,  // Углеродистые  | Радиус < 10км И Масса < 1e17кг
        Silicate,      // Силикатные    | Радиус < 30км И Масса < 1e18кг
        Metallic       // Металлические | Радиус > 30км ИЛИ Масса > 1е18кг
    }
}
