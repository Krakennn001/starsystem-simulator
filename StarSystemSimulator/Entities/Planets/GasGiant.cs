using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.Planets
{
    public sealed class GasGiant : Planet
    {
        public GasGiant() { }

        [SetsRequiredMembers]
        public GasGiant(string name, double radius, double weight, double toStarDistance, double year, List<Satellite> satellites)
            : base(StarObjectType.GiantPlanet,
                name, radius, weight, toStarDistance, year, satellites) { }
    }
}
