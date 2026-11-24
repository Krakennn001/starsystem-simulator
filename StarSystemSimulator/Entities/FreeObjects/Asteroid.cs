using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Services;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.FreeObjects
{
    public sealed class Asteroid : FreeObject
    {
        public AsteroidType TypeOfAsteroid { get; set; }

        public Asteroid() { }

        [SetsRequiredMembers]
        public Asteroid(string name, double radius, double weight, double speed, double distance)
            : base(StarObjectType.Asteroid,
                  name, radius, weight, speed, distance)
        { TypeOfAsteroid = StarUtils.GetAsteroidType(radius, weight); }

        public override List<(string, Format, Color)> GetFullInfo()
        {
            var res = base.GetFullInfo();
            res.Add(($"Тип астероида: {TypeOfAsteroid.ToRusString()}\n", Format.PointedList, Color.Default));
            return res;
        }

        public override List<(string, Format, Color)> GetBrief()
        {
            var res = base.GetBrief();
            var first = res[0];
            res[0] = ($"{first.Text} ({TypeOfAsteroid.ToRusString()})", first.Format, first.Color);
            return res;
        }
    }
}
