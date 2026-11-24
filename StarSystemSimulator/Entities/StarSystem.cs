using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Entities.FreeObjects;
using StarSystemSimulator.Entities.Planets;
using StarSystemSimulator.Entities.Stars;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities
{
    public class StarSystem : IDescriptable
    {
        public required StarObjectType Type { get; set; }
        public required string Name { get; set; }
        public List<Star> Stars { get; set; }
        public List<Planet> Planets { get; set; }
        public List<FreeObject> FreeObjects { get; set; }

        [SetsRequiredMembers]
        public StarSystem()
        {
            Stars = [];
            Planets = [];
            FreeObjects = [];
            Type = StarObjectType.StarSystem;
            Name = "";
        }

        [SetsRequiredMembers]
        public StarSystem(string name, List<Star> stars, List<Planet> planets, List<FreeObject> freeObjects)
        {
            Type = StarObjectType.StarSystem;
            Name = name;
            Stars = stars;
            Planets = planets;
            FreeObjects = freeObjects;
        }

        public List<(string Text, Format Format, Color Color)> GetFullInfo() {
            var res = GetBrief();

            foreach(var star in Stars)
            {
                res = [.. res, .. star.GetFullInfo()];
            }

            foreach (var planet in Planets)
            {
                res = [.. res, .. planet.GetFullInfo()];
                foreach (var sata in planet.Satellites)
                {
                    res = [.. res, .. sata.GetBrief()];
                    res = [.. res, "\n".ToDefault()];
                }
            }

            foreach (var freeObject in FreeObjects)
            {
                res = [.. res, .. freeObject.GetBrief()];
                res = [.. res, "\n".ToDefault()];
            }

            return res;
        }
        public List<(string Text, Format Format, Color Color)> GetBrief() =>
        [
            ($"{Type.ToRusString(true)} {Name}\n", Format.Title, Color.Default),
            ($"Звезд: {Stars.Count}\n", Format.PointedList, Color.Default),
            ($"Планет: {Planets.Count}\n", Format.PointedList, Color.Default),
            ($"Свободных объектов: {FreeObjects.Count}\n", Format.PointedList, Color.Default),
        ];
    }
}
