using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Entities.Stars;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace StarSystemSimulator.Entities.Planets
{
    [JsonDerivedType(typeof(EarthTypePlanet), typeDiscriminator: "earth_type")]
    [JsonDerivedType(typeof(GasGiant), typeDiscriminator: "gas_giant")]
    public abstract class Planet : IDescriptable, IPlanet
    {
        public required StarObjectType Type { get; set; }
        public required string Name { get; set; }
        public double Radius { get; set; }
        public double Weight { get; set; }
        public double ToStarDistance { get; set; }
        public double Year { get; set; }
        public List<Satellite> Satellites { get; set; } = [];

        protected Planet() { }

        [SetsRequiredMembers]
        protected Planet(StarObjectType objectType, string name,
            double radius, double weight, double toStarDistance, double year, List<Satellite> satellites)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(objectType);

            Type = objectType;
            Name = name;
            Radius = radius.ValidateRange(limits.Radius, "Радиус");
            Weight = weight.ValidateRange(limits.Weight, "Масса");
            ToStarDistance = toStarDistance.ValidateRange(limits.Distance, "Расстояние до звезды");
            Year = year.ValidateRange(limits.Year, "Год");
            if (satellites != null && satellites.Count != 0) Satellites = satellites;
        }

        public virtual List<(string Text, Format Format, Color Color)> GetFullInfo() =>
        [
            ($"{Type.ToRusString(true)} {Name}\n", Format.Title, Color.Default),
            ($"Расстояние до звезды: {ToStarDistance:E2}\n", Format.PointedList, Color.Default),
            ($"Масса: {Weight:E2}кг\n", Format.PointedList, Color.Default),
            ($"Год: {Year:N0}дней\n", Format.PointedList, Color.Default),
            ($"Радиус: {Radius:N1}км\n", Format.PointedList, Color.Default),
            ($"Спутников: {Satellites.Count:N0}шт\n", Format.PointedList, Color.Default)
        ];

        public virtual List<(string Text, Format Format, Color Color)> GetBrief() =>
        [
            ($"{Type.ToRusString(true)} {Name} ({Satellites.Count})", Format.Default, Color.Default)
        ];
    }
}
