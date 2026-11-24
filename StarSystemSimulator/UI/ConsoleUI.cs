using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Entities;
using StarSystemSimulator.Entities.FreeObjects;
using StarSystemSimulator.Entities.Planets;
using StarSystemSimulator.Entities.Stars;
using StarSystemSimulator.Extensions;
using StarSystemSimulator.Services;
using StarSystemSimulator.Settings;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.UI
{
    public enum Form
    {
        M,  // Мужской род
        F,  // Женский род
        N,  // Средний род
        P   // Множественное число
    }

    public readonly record struct MenuStatus(
        string Text,
        Color NormalColor,
        Color SelectedColor
    )
    {
        public Color GetColor(bool isSelected) => isSelected ? SelectedColor : NormalColor;
    }

    public static class Status
    {
        public static MenuStatus None(Form form = Form.M) => new(
            Text: form == Form.P ? "отсутствуют" : "отсутствует",
            NormalColor: Color.DarkGray,
            SelectedColor: Color.Gray
        );

        public static MenuStatus Change(Form form = Form.M)
        {
            string text = form switch
            {
                Form.M => "изменен",
                Form.F => "изменена",
                Form.N => "изменено",
                Form.P => "изменены",
                _ => string.Empty
            };

            return new MenuStatus(text, Color.Red, Color.Pink);
        }

        public static MenuStatus Save(Form form = Form.M)
        {
            string text = form switch
            {
                (Form.M) => "сохранен",
                (Form.F) => "сохранена",
                (Form.N) => "сохранено",
                (Form.P) => "сохранены",
                _ => string.Empty
            };

            return new MenuStatus(text, Color.Green, Color.LightGreen);
        }

        public static MenuStatus System(Form form = Form.M)
        {
            string text = form switch
            {
                (Form.M) => "системный",
                (Form.F) => "системная",
                (Form.N) => "системное",
                (Form.P) => "системные",
                _ => string.Empty
            };

            return new MenuStatus(text, Color.Sand, Color.Lime);
        }
    }

    public sealed class ConsoleUI
    {
        private readonly Printer _printer = new();

        private StarSystem? _currentSystem;
        private MenuStatus _systemSourceStatus = Status.None(Form.F);

        private AppSettings _settings = new();
        private MenuStatus _settingsSourceStatus = Status.System(Form.P);

        // ====================
        //  РАЗДЕЛЫ МЕНЮ
        // ====================

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var mainMenu = new ConsoleMenu("ГЛАВНОЕ МЕНЮ",
            [
                new("Управление солнечной системой", OpenSystemMenu, () => _systemSourceStatus),
                new("Настройки", OpenSettingsMenu, () => _settingsSourceStatus),
                new("Визуализация", RunVisualization),
                new("Выход", () => Environment.Exit(0))
            ], _printer);

            mainMenu.Show();
        }

        private void OpenSystemMenu()
        {
            var systemMenu = new ConsoleMenu("УПРАВЛЕНИЕ СОЛНЕЧНОЙ СИСТЕМОЙ",
            [
                new("Сгенерировать", GenerateSystemFlow),
                new("Изменить", EditSystemFlow),
                new("Сохранить в файл", SaveSystemFlow, () => _systemSourceStatus),
                new("Загрузить из файла", LoadSystemFlow),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            systemMenu.Show();
        }

        private void OpenSettingsMenu()
        {
            var settingsMenu = new ConsoleMenu("НАСТРОЙКИ",
            [
                new("Изменить генерацию", OpenGenerationSelectMenu),
                new("Изменить визуализацию", OpenVisualizationSelectMenu),
                new("Сохранить в файл", SaveSettingsFlow, () => _settingsSourceStatus),
                new("Загрузить из файла", LoadSettingsFlow),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            settingsMenu.Show();
        }

        // ====================
        //  РАЗДЕЛ ГЕНЕРАЦИИ
        // ====================
        private void GenerateSystemFlow()
        {
            Console.Clear();
            _printer.Print([
                ("ГЕНЕРАЦИЯ СОЛНЕЧНОЙ СИСТЕМЫ", Format.Title, Color.Cyan),
                ("Введите сид (оставьте пустым для случайного): ", Format.Default, Color.White)
            ]);

            Console.CursorVisible = true;
            string? input = Console.ReadLine();
            Console.CursorVisible = false;

            int seed = int.TryParse(input, out int parsedSeed) ? parsedSeed : Random.Shared.Next();

            var systemGenerator = new StarSystemGenerator(seed);
            var generatedSystem = systemGenerator.Generate(_settings.GenerationProbabilities);

            _printer.PrintLines([
                ($"\nСистема успешно сгенерирована (Seed: {seed}).\n", Format.Default, Color.LightGreen)
            ]);
            _printer.Print(generatedSystem.GetBrief());
            _printer.PrintLines([
                ("\nПрименить эту систему?\n", Format.Default, Color.White)
            ]);

            if (AskConfirmation())
            {
                _currentSystem = generatedSystem;
                _systemSourceStatus = Status.Change(Form.F);
                ShowMessage("Система применена!", Color.Green);
            }
            else
            {
                ShowMessage("Генерация отклонена. Текущая система не изменена.", Color.Yellow);
            }
        }

        private void EditSystemFlow()
        {
            if (_currentSystem == null)
            {
                ShowMessage("Ошибка: сначала создайте или загрузите солнечную систему!", Color.Red);
                return;
            }

            new ConsoleMenu("РЕДАКТИРОВАНИЕ СОЛНЕЧНОЙ СИСТЕМЫ", () =>
            [
                new(
                "Звезды",
                () => ShowObjectListMenu(
                        "ЗВЕЗДЫ",
                    () =>
                        _currentSystem.Stars,
                    s =>
                        $"{s.Name} ({s.Type.ToRusString(true)})",
                        EditStarMenu,
                    s =>
                        new($"{s.Temperature} K", s.GlowColor, s.GlowColor)),
                () =>
                    ValStatus($"{_currentSystem.Stars.Count}шт")),


                new(
                "Планеты",
                () => ShowObjectListMenu(
                        "ПЛАНЕТЫ",
                    () =>
                        _currentSystem.Planets,
                    p =>
                        $"{p.Name} ({p.Type.ToRusString(true)})",
                        EditPlanetMenu,
                    p =>
                        ValStatus($"Спутников: {p.Satellites.Count}")),
                () =>
                    ValStatus($"{_currentSystem.Planets.Count}шт")),
                    

                new(
                "Свободные объекты",
                () => ShowObjectListMenu(
                        "СВОБОДНЫЕ ОБЪЕКТЫ",
                    () =>
                        _currentSystem.FreeObjects,
                    o =>
                        $"{o.Name} ({o.Type.ToRusString(true)})",
                        EditFreeObjectMenu,
                    o =>
                        ValStatus($"V: {o.Speed:N1} км/с")),
                () =>
                    ValStatus($"{_currentSystem.FreeObjects.Count}шт")),


                new("Назад", () => { }, isBackButton: true)
            ], _printer).Show();
        }

        private void ShowObjectListMenu<T>(string title, Func<List<T>> list, Func<T, string> titleSelector, Action<T, List<T>> editAction, Func<T, MenuStatus> statusSelector)
        {
            new ConsoleMenu($"ВЫБОР: {title}", () =>
                [.. list().Select(obj =>
                    new MenuItem(titleSelector(obj),
                        () =>
                            editAction(obj, list()),
                        () =>
                            statusSelector(obj))),
                new("Назад", () => { },isBackButton: true)],
                _printer).Show();
        }

        private void EditStarMenu(Star star, List<Star> list) => new ConsoleMenu($"ЗВЕЗДА: {star.Name.ToUpper()}", () =>
        [
            EditNameItem(() => star.Name, v => star.Name = v),
            EditNumItem("Масса (кг)", () => star.Weight, v => star.Weight = v, "E2"),
            EditNumItem("Радиус (км)", () => star.Radius, v => star.Radius = v, "N1"),
            EditNumItem("Температура (K)", () => star.Temperature, v =>
            {
                star.Temperature = v;
                (star.GlowColor, var cls) = StarUtils.GetSpectralData(v);
                if (star is MainStar ms) ms.Class = cls;
                else if (star is RedGiant rg) rg.Class = cls;
                else if (star is WhiteDwarf wd) wd.Class = cls;
            }, "N0", () => new($"{star.Temperature} K", star.GlowColor, star.GlowColor)),
            DeleteItem("Удалить звезду", list, star),
            new("Назад", () => { }, isBackButton: true)
        ], _printer, exitCondition: () => !list.Contains(star)).Show();

        private void EditPlanetMenu(Planet planet, List<Planet> list) => new ConsoleMenu($"ПЛАНЕТА: {planet.Name.ToUpper()}", () =>
        [
            EditNameItem(() => planet.Name, v => planet.Name = v),
            EditNumItem("Масса (кг)", () => planet.Weight, v => planet.Weight = v, "E2"),
            EditNumItem("Радиус (км)", () => planet.Radius, v => planet.Radius = v, "N1"),
            EditNumItem("Расстояние до звезды (км)", () => planet.ToStarDistance, v => planet.ToStarDistance = v, "E2"),
            EditNumItem("Длительность года (дней)", () => planet.Year, v => planet.Year = v, "N1"),
            new("Спутники", () => ShowObjectListMenu($"СПУТНИКИ {planet.Name.ToUpper()}", () => planet.Satellites, s => s.Name, EditSatelliteMenu, s => ValStatus($"R: {s.Radius:N0} км")),
                () => ValStatus($"{planet.Satellites.Count}шт")),
            DeleteItem("Удалить планету", list, planet),
            new("Назад", () => { }, isBackButton: true)
        ], _printer, exitCondition: () => !list.Contains(planet)).Show();

        private void EditSatelliteMenu(Satellite sat, List<Satellite> list) =>
            new ConsoleMenu($"СПУТНИК: {sat.Name.ToUpper()}", () =>
        [
            EditNameItem(() => sat.Name, v => sat.Name = v),
            EditNumItem("Масса (кг)", () => sat.Weight, v => sat.Weight = v, "E2"),
            EditNumItem("Радиус (км)", () => sat.Radius, v => sat.Radius = v, "N1"),
            EditNumItem("Расстояние до планеты (км)", () => sat.ToPlanetDistance, v => sat.ToPlanetDistance = v, "E2"),
            EditNumItem("Длительность года (дней)", () => sat.Year, v => sat.Year = v, "N1"),
            DeleteItem("Удалить спутник", list, sat),
            new("Назад", () => { }, isBackButton: true)
        ], _printer, exitCondition: () => !list.Contains(sat)).Show();

        private void EditFreeObjectMenu(FreeObject obj, List<FreeObject> list)
        {
            List<MenuItem> items =
            [
                EditNameItem(() => obj.Name, v => obj.Name = v),
                EditNumItem("Масса (кг)", () => obj.Weight, v => obj.Weight = v, "E2"),
                EditNumItem("Радиус (км)", () => obj.Radius, v => obj.Radius = v, "N1"),
                EditNumItem("Скорость (км/с)", () => obj.Speed, v => obj.Speed = v, "N1"),
                EditNumItem("Расстояние до звезды (км)", () => obj.ToStarDistance, v =>
                {
                    obj.ToStarDistance = v;
                    if (obj is Comet c) c.IsActive = v.IsInRange(LimitsConfig.Get<FreeObjectLimits>(StarObjectType.Comet).Activation);
                }, "E2")
            ];

            if (obj is Comet comet)
                items.Add(EditNumItem("Длина хвоста (км)", () => comet.TailLength, v => comet.TailLength = v.ValidateRange(LimitsConfig.Get<FreeObjectLimits>(StarObjectType.Comet).Tail, "Длина хвоста"), "N1"));

            items.Add(DeleteItem("Удалить объект", list, obj));
            items.Add(new("Назад", () => { }, isBackButton: true));

            new ConsoleMenu($"ОБЪЕКТ: {obj.Name.ToUpper()}", () => items, _printer, exitCondition: () => !list.Contains(obj)).Show();
        }

        private static MenuStatus ValStatus(string text) => new(text, Color.Cyan, Color.LightCyan);
        private MenuItem EditNameItem(Func<string> getter, Action<string> setter) => new("Название", () =>
        {
            Console.Clear();
            Console.CursorVisible = true;
            _printer.Print([($"Введите новое название (текущее: {getter()}): ", Format.Default, Color.White)]);
            string? input = Console.ReadLine()?.Trim();
            Console.CursorVisible = false;

            if (!string.IsNullOrWhiteSpace(input))
            {
                setter(input);
                _systemSourceStatus = Status.Change(Form.F);
                ShowMessage("Название обновлено.", Color.Green);
            }
        }, () => ValStatus(getter()));
        private MenuItem EditNumItem<T>(string title, Func<T> getter, Action<T> setter, string format, Func<MenuStatus>? customStatus = null)
            where T : struct, IParsable<T>, IFormattable => new(title, () =>
            {
                if (AskValue<T>($"Введите {title.ToLower()} (текущее: {getter().ToString(format, null)}): ") is { } val)
                {
                    try
                    {
                        setter(val);
                        _systemSourceStatus = Status.Change(Form.F);
                        ShowMessage("Значение обновлено.", Color.Green);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Ошибка: {ex.Message}", Color.Red);
                    }
                }
            }, customStatus ?? (() => ValStatus(getter().ToString(format, null))));
        private MenuItem DeleteItem<T>(string title, List<T> list, T item) => new(title, () =>
        {
            if (AskConfirmation())
            {
                list.Remove(item);
                _systemSourceStatus = Status.Change(Form.F);
                ShowMessage("Объект удален.", Color.Green);
            }
        }, color: Color.Red);

        private void SaveSystemFlow()
        {
            if (_currentSystem == null)
            {
                ShowMessage("Ошибка: нечего сохранять. Система отсутствует.", Color.Red);
                return;
            }

            string? path = AskFilePath("Введите путь или имя файла для сохранения (например, system.json): ");
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                Serializer.SaveToFile(_currentSystem, path);
                _systemSourceStatus = Status.Save(Form.F);
                ShowMessage($"Система сохранена в файл '{path}'.", Color.Green);
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при сохранении: {ex.Message}", Color.Red);
            }
        }

        private void LoadSystemFlow()
        {
            string? path = AskFilePath("Введите путь к файлу системы для загрузки: ");
            if (string.IsNullOrWhiteSpace(path)) return;

            if (!File.Exists(path))
            {
                ShowMessage("Ошибка: файл не найден!", Color.Red);
                return;
            }

            try
            {
                _currentSystem = Serializer.LoadFromFile<StarSystem>(path);
                _systemSourceStatus = Status.Save(Form.F);
                ShowMessage($"Система загружена из файла '{path}'.", Color.Green);
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при загрузке: {ex.Message}", Color.Red);
            }
        }

        // ====================
        //  РАЗДЕЛ НАСТРОЕК
        // ====================
        private void OpenGenerationSelectMenu()
        {
            var objectSelectMenu = new ConsoleMenu("ВЫБОР ОБЪЕКТА ДЛЯ НАСТРОЙКИ",
            [
                new("Настройки звезд", () => OpenObjectGenerationSelectMenu("Звезды")),
                new("Настройки планет", () => OpenObjectGenerationSelectMenu("Планеты")),
                new("Настройки спутников", () => OpenObjectGenerationSelectMenu("Спутники")),
                new("Настройки свободных объектов", () => OpenObjectGenerationSelectMenu("Свободные объекты")),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            objectSelectMenu.Show();
        }

        private void OpenObjectGenerationSelectMenu(string objectCategory)
        {
            var items = GetGenerationItems(objectCategory);
            items.Add(new("Назад", () => { }, isBackButton: true));

            var specificMenu = new ConsoleMenu($"НАСТРОЙКИ {objectCategory.ToUpper()}:", items, _printer);
            specificMenu.Show();

            if (objectCategory == "Звезды")
            {
                var stars = _settings.GenerationProbabilities.Stars;
                if (stars.IsValidSum()) return;

                stars.ValidateSum();
                _settingsSourceStatus = Status.Change(Form.P);

                ShowMessage(
                    "Сумма шансов звезд не была равна 1. Значения скорректированы:\n" +
                    $"> Основная последовательность: {stars.MainStar:N3}\n" +
                    $"> Белый карлик: {stars.WhiteDwarf:N3}\n" +
                    $"> Красный гигант: {stars.RedGiant:N3}\n" +
                    $"> Нейтронная звезда: {stars.NeutronStar:N3}",
                    Color.Yellow
                );
            }
        }

        private List<MenuItem> GetGenerationItems(string objectCategory)
        {
            return objectCategory switch
            {
                "Звезды" =>
                [
                    new("Диапазон генерации",
                        () => EditRange(_settings.GenerationProbabilities.Stars.Amount, v => UpdateStars(s => s with { Amount = v })),
                        () => _settings.GenerationProbabilities.Stars.Amount.ToString().ToMenuStatus()),
                    new("Шанс: Звезда основной последовательности",
                        () => EditChance(_settings.GenerationProbabilities.Stars.MainStar, v => UpdateStars(s => s with { MainStar = v })),
                        () => _settings.GenerationProbabilities.Stars.MainStar.ToString("N3").ToMenuStatus()),
                    new("Шанс: Белый карлик",
                        () => EditChance(_settings.GenerationProbabilities.Stars.WhiteDwarf, v => UpdateStars(s => s with { WhiteDwarf = v })),
                        () => _settings.GenerationProbabilities.Stars.WhiteDwarf.ToString("N3").ToMenuStatus()),
                    new("Шанс: Красный гигант",
                        () => EditChance(_settings.GenerationProbabilities.Stars.RedGiant, v => UpdateStars(s => s with { RedGiant = v })),
                        () => _settings.GenerationProbabilities.Stars.RedGiant.ToString("N3").ToMenuStatus()),
                    new("Шанс: Нейтронная звезда",
                        () => EditChance(_settings.GenerationProbabilities.Stars.NeutronStar, v => UpdateStars(s => s with { NeutronStar = v })),
                        () => _settings.GenerationProbabilities.Stars.NeutronStar.ToString("N3").ToMenuStatus())
                ],

                "Планеты" =>
                [
                    new("Диапазон генерации",
                        () => EditRange(_settings.GenerationProbabilities.Planets.Amount, v => UpdatePlanets(p => p with { Amount = v })),
                        () => _settings.GenerationProbabilities.Planets.Amount.ToString().ToMenuStatus()),
                    new("Шанс: Газовый гигант",
                        () => EditChance(_settings.GenerationProbabilities.Planets.GasGiant, v => UpdatePlanets(p => p with { GasGiant = v })),
                        () => $"Гигант: {_settings.GenerationProbabilities.Planets.GasGiant:N3} | Земной тип: {_settings.GenerationProbabilities.Planets.EarthType:N3}".ToMenuStatus())
                ],

                "Спутники" =>
                [
                    new("Диапазон генерации",
                        () => EditRange(_settings.GenerationProbabilities.Satellites.Amount, v => UpdateSatellites(s => s with { Amount = v })),
                        () => _settings.GenerationProbabilities.Satellites.Amount.ToString().ToMenuStatus())
                ],

                "Свободные объекты" =>
                [
                    new("Диапазон генерации",
                        () => EditRange(_settings.GenerationProbabilities.FreeObjects.Amount, v => UpdateFreeObjects(f => f with { Amount = v })),
                        () => _settings.GenerationProbabilities.FreeObjects.Amount.ToString().ToMenuStatus()),
                    new("Шанс: Комета",
                        () => EditChance(_settings.GenerationProbabilities.FreeObjects.Comet, v => UpdateFreeObjects(f => f with { Comet = v })),
                        () => $"Комета: {_settings.GenerationProbabilities.FreeObjects.Comet:N3} | Астероид: {_settings.GenerationProbabilities.FreeObjects.Asteroid:N3}".ToMenuStatus())
                ],

                _ => []
            };
        }

        private void UpdateStars(Func<StarProbabilities, StarProbabilities> update) =>
            _settings = _settings with { GenerationProbabilities = _settings.GenerationProbabilities with { Stars = update(_settings.GenerationProbabilities.Stars) } };

        private void UpdatePlanets(Func<PlanetProbabilities, PlanetProbabilities> update) =>
            _settings = _settings with { GenerationProbabilities = _settings.GenerationProbabilities with { Planets = update(_settings.GenerationProbabilities.Planets) } };

        private void UpdateSatellites(Func<SatelliteProbabilities, SatelliteProbabilities> update) =>
            _settings = _settings with { GenerationProbabilities = _settings.GenerationProbabilities with { Satellites = update(_settings.GenerationProbabilities.Satellites) } };

        private void UpdateFreeObjects(Func<FreeObjectProbabilities, FreeObjectProbabilities> update) =>
            _settings = _settings with { GenerationProbabilities = _settings.GenerationProbabilities with { FreeObjects = update(_settings.GenerationProbabilities.FreeObjects) } };

        private void EditRange(Range<int> current, Action<Range<int>> setter)
        {
            var val = AskValue<Range<int>>($"Введите новый диапазон (текущий: {current}): ");
            if (!val.HasValue) return;
            setter(val.Value);
            _settingsSourceStatus = Status.Change(Form.P);
            ShowMessage($"Диапазон обновлен: {val.Value}", Color.Green);
        }

        private void EditChance(double current, Action<double> setter)
        {
            var val = AskValue<double>($"Введите новое значение (текущее: {current:N3}): ");
            if (!val.HasValue) return;
            if (val.Value is < 0.0 or > 1.0)
            {
                ShowMessage("Ошибка: шанс должен быть в пределах от 0 до 1.", Color.Red);
                return;
            }
            setter(val.Value);
            _settingsSourceStatus = Status.Change(Form.P);
            ShowMessage($"Значение обновлено: {val.Value}", Color.Green);
        }

        private void OpenVisualizationSelectMenu()
        {
            var vizSelectMenu = new ConsoleMenu("ВЫБОР ПАРАМЕТРА ДЛЯ НАСТРОЙКИ",
            [
                new("Цвет", () => OpenColorMenu()),
                new("Количество", () => OpenAmountMenu()),
                new("Позиционирование", () => OpenPositionMenu()),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            vizSelectMenu.Show();
        }

        private void OpenColorMenu()
        {
            var colorMenu = new ConsoleMenu("НАСТРОЙКА ЦВЕТА ОБЪЕКТОВ",
            [
                new("Планеты", () =>
                    EditColor((val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { PlanetColor = val } }), () =>
                    new MenuStatus(_settings.Visualization.PlanetColor.ToRusString(), _settings.Visualization.PlanetColor, Color.Gray)),
                new("Спутники", () =>
                    EditColor((val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { SatelliteColor = val } }), () =>
                    new MenuStatus(_settings.Visualization.SatelliteColor.ToRusString(), _settings.Visualization.SatelliteColor, Color.Gray)),
                new("Свободные объекты", () =>
                    EditColor((val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { FreeObjectColor = val } }), () =>
                    new MenuStatus(_settings.Visualization.FreeObjectColor.ToRusString(), _settings.Visualization.FreeObjectColor, Color.Gray)),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            colorMenu.Show();
        }

        private void EditColor(Action<Color> setter)
        {
            var colorChooseMenu = new ConsoleMenu($"ВЫБОР ЦВЕТА",
            [
                .. Enum.GetValues<Color>().Select(color => new MenuItem(
                    color.ToRusString(),
                    () =>
                    {
                        setter(color);
                        _settingsSourceStatus = Status.Change(Form.P);
                        ShowMessage($"Установлен цвет: {color.ToRusString()}", Color.Green);
                    },
                    color: color
                )),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            colorChooseMenu.Show();
        }

        private void OpenAmountMenu()
        {
            var amountMenu = new ConsoleMenu("НАСТРОЙКА КОЛИЧЕСТВА ОБЪЕКТОВ",
            [
                new("Звезды", () =>
                    EditAmount(_settings.Visualization.StarsAmount, (val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { StarsAmount = val } }), () =>
                    new MenuStatus(_settings.Visualization.StarsAmount.ToString(), Color.DarkGray, Color.Gray)),
                new("Планеты", () =>
                    EditAmount(_settings.Visualization.PlanetsAmount, (val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { PlanetsAmount = val } }), () =>
                    new MenuStatus(_settings.Visualization.PlanetsAmount.ToString(), Color.DarkGray, Color.Gray)),
                new("Свободные объекты", () =>
                    EditAmount(_settings.Visualization.FreeObjectsAmount, (val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { FreeObjectsAmount = val } }), () =>
                    new MenuStatus(_settings.Visualization.FreeObjectsAmount.ToString(), Color.DarkGray, Color.Gray)),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            amountMenu.Show();
        }

        private void EditAmount(Range<int> current, Action<Range<int>> setter)
        {
            int? val = AskValue<int>($"Введите новое максимальное количество (текущее: {current.Max}): ");
            if (!val.HasValue) return;
            setter(new Range<int>(0, Math.Max(0, val.Value)));
            _settingsSourceStatus = Status.Change(Form.P);
            ShowMessage($"Максимум обновлен: {val.Value}", Color.Green);
        }

        private void OpenPositionMenu()
        {
            var positionMenu = new ConsoleMenu("НАСТРОЙКА ПОЗИЦИОНИРОВАНИЯ",
            [
                new("Размер окна", () =>
                    EditPosition(_settings.Visualization.Window, (val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { Window = val } }), () =>
                    new MenuStatus(_settings.Visualization.Window.ToString(), Color.DarkGray, Color.Gray)),
                new("Центр системы", () =>
                    EditPosition(_settings.Visualization.StarCenter, (val) =>
                    _settings = _settings with { Visualization = _settings.Visualization with { StarCenter = val } }), () =>
                    new MenuStatus(_settings.Visualization.StarCenter.ToString(), Color.DarkGray, Color.Gray)),
                new("Назад", () => { }, isBackButton: true)
            ], _printer);

            positionMenu.Show();
        }

        private void EditPosition((int x, int y) current, Action<(int x, int y)> setter)
        {
            int? valX = AskValue<int>($"Введите новое значение по горизонтали (текущее: {current.x}): ");
            if (!valX.HasValue) return;
            int? valY = AskValue<int>($"Введите новое значение по вертикали (текущее: {current.y}): ");
            if (!valY.HasValue) return;
            setter((valX.Value, valY.Value));
            _settingsSourceStatus = Status.Change(Form.P);
            ShowMessage($"Позиционирвоание обновлено: {(valX.Value, valY.Value)}", Color.Green);
        }

        private void SaveSettingsFlow()
        {
            string? path = AskFilePath("Введите имя файла для сохранения настроек (например, settings.json): ");
            if (string.IsNullOrWhiteSpace(path)) return;
            try
            {
                Serializer.SaveToFile(_settings, path);
                _settingsSourceStatus = Status.Save(Form.P);
                ShowMessage($"Настройки сохранены в файл '{path}'.", Color.Green);
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при сохранении: {ex.Message}", Color.Red);
            }
        }

        private void LoadSettingsFlow()
        {
            string? path = AskFilePath("Введите путь к файлу настроек: ");
            if (string.IsNullOrWhiteSpace(path)) return;
            if (!File.Exists(path))
            {
                ShowMessage("Ошибка: файл настроек не найден!", Color.Red);
                return;
            }
            try
            {
                var settings = Serializer.LoadFromFile<AppSettings>(path);
                if (settings != null)
                {
                    _settings = settings;
                    _settingsSourceStatus = Status.Save(Form.P);
                    ShowMessage($"Настройки загружены из файла '{path}'.", Color.Green);
                } else
                {
                    ShowMessage($"Ошибка при загрузке из файла '{path}'.", Color.Green);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при загрузке: {ex.Message}", Color.Red);
            }
        }

        // ====================
        //  РАЗДЕЛ ВИЗУАЛИЗАЦИИ
        // ====================
        private void RunVisualization()
        {
            if (_currentSystem == null)
            {
                ShowMessage("Ошибка: невозможно запустить визуализацию — солнечная система не создана!", Color.Red);
                return;
            }
            var visualizer = new Visualizer(_currentSystem, _settings.Visualization);
            while (Console.KeyAvailable) Console.ReadKey(true);
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape || key == ConsoleKey.Backspace)
                        break;
                }
                visualizer.Visualize(deltaTime: 1.0);
                Thread.Sleep(50);
            }
            Console.Clear();
        }

        // ====================
        //  ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ====================
        private bool AskConfirmation()
        {
            int choice = 0;
            string[] options = ["Применить", "Отказаться"];
            int startTop = Console.CursorTop;
            while (true)
            {
                Console.SetCursorPosition(0, startTop);
                var lines = new List<(string, Format, Color)>();
                for (int i = 0; i < options.Length; i++)
                {
                    bool isSelected = i == choice;
                    string prefix = isSelected ? "> " : "  ";
                    Color color = isSelected ? Color.Gold : Color.LightGray;
                    lines.Add(($"{prefix}{options[i]}   ", Format.NumberedList, color));
                }
                _printer.PrintLines(lines);
                var key = Console.ReadKey(true).Key;
                if (key is ConsoleKey.W or ConsoleKey.UpArrow or ConsoleKey.S or ConsoleKey.DownArrow)
                    choice = 1 - choice;
                else if (key is ConsoleKey.Enter or ConsoleKey.Spacebar)
                    return choice == 0;
                else if (key == ConsoleKey.Escape)
                    return false;
            }
        }

        private string? AskFilePath(string prompt)
        {
            Console.Clear();
            _printer.Print([(prompt, Format.Default, Color.White)]);
            Console.CursorVisible = true;
            string? input = Console.ReadLine();
            Console.CursorVisible = false;
            return input?.Trim();
        }

        private T? AskValue<T>(string prompt) where T: struct, IParsable<T>
        {
            Console.Clear();
            Console.CursorVisible = true;

            try
            {
                _printer.Print([(prompt, Format.Default, Color.White)]);

                if (T.TryParse(Console.ReadLine(), null, out T result))
                {
                    return result;
                }

                ShowMessage("Некорректный ввод. Значение не обновлено.", Color.Yellow);
                return null;
            }
            finally
            {
                Console.CursorVisible = false;
            }
        }

        private void ShowMessage(string message, Color color)
        {
            _printer.PrintLines([
            ($"\n{message}", Format.Default, color),
                ("Нажмите любую клавишу для продолжения...", Format.Default, Color.Gray)
            ]);
            Console.ReadKey(true);
        }
    }
}