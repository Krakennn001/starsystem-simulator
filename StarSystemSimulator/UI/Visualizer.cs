using StarSystemSimulator.Entities;
using StarSystemSimulator.Entities.Stars;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;
using System.Text;

namespace StarSystemSimulator.UI
{
    public sealed class Visualizer
    {
        public Visualization Settings { get; }
        public int Seed { get; }

        private readonly StarSystem _system;
        private readonly Random _random;
        private readonly ColoredChar[,] _display;
        private readonly (Range<int> x, Range<int> y) _bounds;

        private readonly List<OrbitState> _starOrbits = [];
        private readonly List<PlanetOrbitState> _planetOrbits = [];
        private readonly List<OrbitState> _freeObjectOrbits = [];
        private bool _isInitialized = false;

        private double AspectRatio => (double)Settings.WHConstant.y / Settings.WHConstant.x;

        public Visualizer(StarSystem system, Visualization settings, int? seed = null)
        {
            Seed = seed ?? Random.Shared.Next();
            _random = new Random(Seed);
            _system = system;
            Settings = settings;
            _display = new ColoredChar[Settings.Window.y, Settings.Window.x];
            _bounds = (new Range<int>(0, Settings.Window.x - 1), new Range<int>(0, Settings.Window.y - 1));
        }

        public void Visualize(double deltaTime = 1.0)
        {
            if (!_isInitialized)
            {
                InitializeOrbits();
                Console.CursorVisible = false;
                Console.Clear();
            }

            for (int y = _bounds.y.Min; y <= _bounds.y.Max; y++)
                for (int x = _bounds.x.Min; x <= _bounds.x.Max; x++)
                    _display[y, x] = new ColoredChar(Color.Default, ' ');

            int freeCount = Math.Min(_system.FreeObjects.Count, Settings.FreeObjectsAmount.Max);
            for (int i = 0; i < freeCount; i++)
            {
                var orbit = _freeObjectOrbits[i];
                orbit.Angle = (orbit.Angle + orbit.AngularSpeed * deltaTime) % (2 * Math.PI);
                var pos = GetOrbitPoint(Settings.StarCenter, orbit.Radius, orbit.Angle);

                if (pos.X.IsInRange(_bounds.x) && pos.Y.IsInRange(_bounds.y))
                    DrawCircle(pos, 0, 1, "freeobject");
            }

            int starsCount = Math.Min(_system.Stars.Count, Settings.StarsAmount.Max);
            for (int i = 0; i < starsCount; i++)
            {
                var orbit = _starOrbits[i];
                orbit.Angle = (orbit.Angle + orbit.AngularSpeed * deltaTime) % (2 * Math.PI);
                var starPos = GetOrbitPoint(Settings.StarCenter, orbit.Radius, orbit.Angle);

                DrawStar(starPos, _system.Stars[i]);
            }

            int planetsCount = Math.Min(_system.Planets.Count, Settings.PlanetsAmount.Max);
            for (int i = 0; i < planetsCount; i++)
            {
                var pState = _planetOrbits[i];
                pState.PlanetOrbit.Angle = (pState.PlanetOrbit.Angle + pState.PlanetOrbit.AngularSpeed * deltaTime) % (2 * Math.PI);
                var planetPos = GetOrbitPoint(Settings.StarCenter, pState.PlanetOrbit.Radius, pState.PlanetOrbit.Angle);

                DrawCircle(planetPos, pState.VisualRadius, 1, "planet");

                for (int s = 0; s < pState.Satellites.Count; s++)
                {
                    var sat = pState.Satellites[s];
                    sat.Orbit.Angle = (sat.Orbit.Angle + sat.Orbit.AngularSpeed * deltaTime) % (2 * Math.PI);
                    var satPos = GetOrbitPoint(planetPos, sat.Orbit.Radius, sat.Orbit.Angle);

                    if (satPos.X.IsInRange(_bounds.x) && satPos.Y.IsInRange(_bounds.y))
                        DrawCircle(satPos, sat.VisualRadius, 0, "satellite");
                }
            }

            RenderBuffer();
        }

        private void InitializeOrbits()
        {
            _starOrbits.Clear();
            _planetOrbits.Clear();
            _freeObjectOrbits.Clear();

            int starsCount = Math.Min(_system.Stars.Count, Settings.StarsAmount.Max);
            if (starsCount == 1)
            {
                _starOrbits.Add(new OrbitState(0, 0, 0));
            }
            else if (starsCount > 1)
            {
                double barycenterRadius = 6.0;
                double baseSpeed = 0.01;
                for (int i = 0; i < starsCount; i++)
                {
                    double startAngle = (2 * Math.PI / starsCount) * i;
                    _starOrbits.Add(new OrbitState(barycenterRadius, startAngle, baseSpeed));
                }
            }

            int minOrbit = starsCount > 1 ? 22 : 16;
            int maxOrbit = Settings.Window.x - Settings.StarCenter.x - 8;

            int planetsCount = Math.Min(_system.Planets.Count, Settings.PlanetsAmount.Max);
            for (int i = 0; i < planetsCount; i++)
            {
                var planet = _system.Planets[i];
                int visualRadius = GetPlanetVisualRadius(planet.Radius);

                double orbitRadius = _random.Next(minOrbit, Math.Max(minOrbit + 1, maxOrbit));
                double startAngle = _random.NextDouble() * 2 * Math.PI;

                double speed = (0.15 / Math.Sqrt(orbitRadius)) * (_random.NextDouble() * 0.4 + 0.8);

                var satStates = new List<SatelliteState>();
                if (planet.Satellites.Count > 0)
                {
                    int satVisualRadius = GetSatelliteVisualRadius(planet.Satellites.Max(s => s.Radius));

                    double satOrbitRadius = visualRadius + 3.0 + _random.NextDouble() * 2.0;
                    double satAngle = _random.NextDouble() * 2 * Math.PI;
                    double satSpeed = 0.15 + _random.NextDouble() * 0.1;

                    satStates.Add(new SatelliteState(satVisualRadius, new OrbitState(satOrbitRadius, satAngle, satSpeed)));
                }

                _planetOrbits.Add(new PlanetOrbitState(visualRadius, new OrbitState(orbitRadius, startAngle, speed), satStates));
            }

            int freeCount = Math.Min(_system.FreeObjects.Count, Settings.FreeObjectsAmount.Max);
            for (int i = 0; i < freeCount; i++)
            {
                double orbitRadius = _random.Next(18, Math.Max(19, maxOrbit + 5));
                double startAngle = _random.NextDouble() * 2 * Math.PI;
                int direction = _random.Next(2) == 0 ? 1 : -1;
                double speed = direction * (0.2 / Math.Sqrt(orbitRadius));

                _freeObjectOrbits.Add(new OrbitState(orbitRadius, startAngle, speed));
            }

            _isInitialized = true;
        }

        private (int X, int Y) GetOrbitPoint((int X, int Y) center, double radius, double angle)
        {
            int x = center.X + (int)Math.Round(Math.Cos(angle) * radius);
            int y = center.Y + (int)Math.Round(Math.Sin(angle) * radius / AspectRatio);
            return (x, y);
        }

        private void DrawStar((int X, int Y) center, Star star)
        {
            Color color = star.GlowColor;
            switch (star.Type)
            {
                case StarObjectType.NeutronStar: DrawCircle(center, 1, 2, "star", color); break;
                case StarObjectType.WhiteDwarfStar: DrawCircle(center, 4, 2, "star", color); break;
                case StarObjectType.MainStar: DrawCircle(center, 7, 3, "star", color); break;
                case StarObjectType.RedGiantStar: DrawCircle(center, 10, 4, "star", color); break;
            }
        }

        private void DrawCircle((int X, int Y) center, int radius, int brightConstant, string type, Color? color = null)
        {
            int minY = Math.Max(_bounds.y.Min, center.Y - radius - 1);
            int maxY = Math.Min(_bounds.y.Max, center.Y + radius + 1);
            int minX = Math.Max(_bounds.x.Min, center.X - radius * 2 - 2);
            int maxX = Math.Min(_bounds.x.Max, center.X + radius * 2 + 2);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    double dist = Distance(center, (x, y));
                    if (dist > radius) continue;
                    char symbol = GetSymbolByDistance(type, dist, brightConstant);
                    Color finalColor = type switch
                    {
                        "planet" => Settings.PlanetColor,
                        "satellite" => Settings.SatelliteColor,
                        "freeobject" => Settings.FreeObjectColor,
                        _ => color ?? Color.Default
                    };
                    _display[y, x] = new ColoredChar(finalColor, symbol);
                }
            }
        }

        private char GetSymbolByDistance(string type, double dist, int brightConstant)
        {
            int maxIdx = Settings.BrightSymbols.Length - 1;
            int idx = type switch
            {
                "planet" => (int)Math.Ceiling(dist * 2),
                "satellite" => (int)Math.Ceiling(dist * 6),
                "freeobject" => maxIdx,
                _ => (int)(dist / Math.Max(1, brightConstant))
            };
            return Settings.BrightSymbols[Math.Clamp(idx, 0, maxIdx)];
        }

        private double Distance((int X, int Y) p1, (int X, int Y) p2)
        {
            double dx = (p1.X - p2.X) * Settings.WHConstant.x;
            double dy = (p1.Y - p2.Y) * Settings.WHConstant.y;
            return Math.Sqrt(dx * dx + dy * dy) / Math.Max(Settings.WHConstant.x, Settings.WHConstant.y);
        }

        private void RenderBuffer()
        {
            Console.SetCursorPosition(0, 0);
            var sb = new StringBuilder(Settings.Window.x * Settings.Window.y * 12);
            string currentColor = string.Empty;
            for (int y = _bounds.y.Min; y <= _bounds.y.Max; y++)
            {
                for (int x = _bounds.x.Min; x <= _bounds.x.Max; x++)
                {
                    var cell = _display[y, x];
                    if (cell.Color != currentColor)
                    {
                        sb.Append(cell.Color);
                        currentColor = cell.Color;
                    }
                    sb.Append(cell.Char);
                }
                if (y < _bounds.y.Max) sb.AppendLine();
            }
            sb.Append(Color.Default.ToAnsiString());
            Console.Write(sb.ToString());
        }

        private static int GetPlanetVisualRadius(double planetRadius)
        {
            var limitsEarth = LimitsConfig.Get<PlanetLimits>(StarObjectType.EarthPlanet);
            var limitsGiant = LimitsConfig.Get<PlanetLimits>(StarObjectType.GiantPlanet);
            if (planetRadius < (limitsEarth.Radius.Min + limitsEarth.Radius.Max) / 2) return 1;
            if (planetRadius < limitsEarth.Radius.Max) return 2;
            if (planetRadius < (limitsGiant.Radius.Min + limitsGiant.Radius.Max) / 2) return 3;
            return 4;
        }

        private static int GetSatelliteVisualRadius(double satRadius)
        {
            var limits = LimitsConfig.Get<PlanetLimits>(StarObjectType.Satellite);
            return satRadius < (limits.Radius.Min + limits.Radius.Max) / 2 ? 0 : 1;
        }

        private sealed class OrbitState(double radius, double angle, double angularSpeed)
        {
            public double Radius { get; } = radius;
            public double Angle { get; set; } = angle;
            public double AngularSpeed { get; } = angularSpeed;
        }

        private sealed class PlanetOrbitState(int visualRadius, OrbitState planetOrbit, List<SatelliteState> satellites)
        {
            public int VisualRadius { get; } = visualRadius;
            public OrbitState PlanetOrbit { get; } = planetOrbit;
            public List<SatelliteState> Satellites { get; } = satellites;
        }

        private sealed class SatelliteState(int visualRadius, OrbitState orbit)
        {
            public int VisualRadius { get; } = visualRadius;
            public OrbitState Orbit { get; } = orbit;
        }

        struct ColoredChar(Color color, char _char)
        {
            public string Color { get; set; } = color.ToAnsiString();
            public char Char { get; set; } = _char;
        }
    }
}
