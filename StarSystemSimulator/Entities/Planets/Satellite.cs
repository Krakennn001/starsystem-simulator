using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Diagnostics.CodeAnalysis;

namespace StarSystemSimulator.Entities.Planets
{
    public sealed class Satellite : IDescriptable
    {
        public required StarObjectType Type { get; set; }
        public required string Name { get; set; }
        public double Weight { get; set; }  // в кг
        public double Radius { get; set; }  // в км
        public double ToPlanetDistance { get; set; }  // в км
        public double Year { get; set; }  // в днях
        public int ID { get; set; }

        public Satellite() { }

        [SetsRequiredMembers]
        public Satellite(string name, double radius, double weight, double distance, double year, int id = 0)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(StarObjectType.Satellite);

            Type    = StarObjectType.Satellite;
            ID      = id;
            Name    = name;
            Radius  = radius.ValidateRange(limits.Radius, "Радиус");
            Weight  = weight.ValidateRange(limits.Weight, "Масса");
            Year    = year.ValidateRange(limits.Year, "Год");
            ToPlanetDistance = distance.ValidateRange(limits.Distance, "Расстояние до планеты");
        }

        public List<(string, Format, Color)> GetFullInfo() =>
        [
            ($"{Type.ToRusString(true)} {Name}\n", Format.Title, Color.Default),
            ($"Расстояние до планеты: {ToPlanetDistance:E2}км\n", Format.PointedList, Color.Default),
            ($"Масса: {Weight:E2}кг\n", Format.PointedList, Color.Default),
            ($"Год: {Year:N0}дней\n", Format.PointedList, Color.Default),
            ($"Радиус: {Radius:N1}км\n", Format.PointedList, Color.Default)
        ];

        public List<(string, Format, Color)> GetBrief() =>
        [
            ($"{Type.ToRusString(true)} {Name}", Format.Default, Color.Default)
        ];
    }
}
