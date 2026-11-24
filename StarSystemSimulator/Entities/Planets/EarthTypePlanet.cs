using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.Planets
{
    public sealed class EarthTypePlanet : Planet
    {
        public EarthTypePlanet() { }

        [SetsRequiredMembers]
        public EarthTypePlanet(string name, double radius, double weight, double toStarDistance, double year, List<Satellite> satellites)
            : base(StarObjectType.EarthPlanet,
                name, radius, weight, toStarDistance, year, satellites) { }
    }
}
