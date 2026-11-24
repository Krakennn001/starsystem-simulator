using StarSystemSimulator.Descriptions;
using StarSystemSimulator.Utils;

namespace StarSystemSimulator.UI
{
    public sealed class MenuItem(string title, Action action, Func<MenuStatus>? statusProvider = null, bool isBackButton = false, Color? color = null)
    {
        public string Title { get; } = title;
        public Func<MenuStatus>? StatusProvider { get; } = statusProvider;
        public Action Action { get; } = action;
        public bool IsBackButton { get; } = isBackButton;
        public Color? Color { get; } = color;
    }

    public sealed class ConsoleMenu
    {
        public string Header { get; }
        private readonly Func<List<MenuItem>> _itemsProvider;
        private readonly Func<bool>? _exitCondition;
        private readonly Printer _printer;
        private readonly int _maxVisibleItems;

        private int _selectedIndex = 0;
        private int _scrollOffset = 0; // Индекс первого видимого элемента на экране
        private int _scrollStep = 1;

        public ConsoleMenu(string header, List<MenuItem> items, Printer? printer = null, int maxVisibleItems = 0)
            : this(header, () => items, printer, maxVisibleItems) { }

        public ConsoleMenu(string header, Func<List<MenuItem>> itemsProvider, Printer? printer = null, int maxVisibleItems = 0, Func<bool>? exitCondition = null)
        {
            Header = header;
            _itemsProvider = itemsProvider;
            _printer = printer ?? new Printer();
            _maxVisibleItems = maxVisibleItems;
            _exitCondition = exitCondition;
        }

        public void Show()
        {
            Console.CursorVisible = false;
            bool exitMenu = false;

            while (!exitMenu)
            {
                var items = _itemsProvider();
                if (items.Count == 0) break;
                if (_selectedIndex >= items.Count) _selectedIndex = items.Count - 1;

                Render(items);
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Tab || keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift))
                {
                    _scrollStep = _scrollStep == 1 ? 10 : 1;
                    if (keyInfo.Key == ConsoleKey.Tab) continue;
                }

                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        _selectedIndex = _selectedIndex > 0 ? (_selectedIndex - _scrollStep) : items.Count - 1;
                        _selectedIndex = _selectedIndex >= 0 ? _selectedIndex : ((_selectedIndex % items.Count) + items.Count) % items.Count;
                        break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        _selectedIndex = _selectedIndex < items.Count - 1 ? (_selectedIndex + _scrollStep) : 0;
                        _selectedIndex = _selectedIndex < items.Count ? _selectedIndex : (_selectedIndex % items.Count);
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        var selected = items[_selectedIndex];
                        selected.Action.Invoke();
                        if (selected.IsBackButton || (_exitCondition?.Invoke() ?? false))
                            exitMenu = true;
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        exitMenu = true;
                        break;
                }
            }
        }

        private void Render(List<MenuItem> items)
        {
            Console.Clear();

            // Вычисляем, сколько строк поместится на экране (оставляем ~7 строк под шапку и индикаторы прокрутки)
            int visibleCount = _maxVisibleItems > 0
                ? _maxVisibleItems
                : Math.Max(5, Console.WindowHeight - 7);

            // Сдвигаем окно прокрутки вслед за курсором
            if (_selectedIndex < _scrollOffset)
                _scrollOffset = _selectedIndex;
            else if (_selectedIndex >= _scrollOffset + visibleCount)
                _scrollOffset = _selectedIndex - visibleCount + 1;

            // Защита от пустого места снизу при удалении элементов
            if (_scrollOffset > Math.Max(0, items.Count - visibleCount))
                _scrollOffset = Math.Max(0, items.Count - visibleCount);

            int endIndex = Math.Min(items.Count, _scrollOffset + visibleCount);

            var buffer = new List<(string str, Format format, Color color)>
            {
                (Header, Format.Title, Color.Cyan),
                ($"Управление: W/S или стрелочки — выбор | Tab — шаг прокрутки (сейчас: {_scrollStep}) | Enter — ОК | Esc — назад\n\n", Format.Default, Color.Gray)
            };

            // Индикатор, что сверху есть скрытые элементы
            if (_scrollOffset > 0)
                buffer.Add(($"  ▲ ... ещё {_scrollOffset} сверху ...\n", Format.Default, Color.DarkGray));

            for (int i = _scrollOffset; i < endIndex; i++)
            {
                var item = items[i];
                bool isSelected = i == _selectedIndex;

                string prefix = isSelected ? "> " : "  ";
                Color itemColor = isSelected ? (item.Color ?? Color.Gold) : Color.LightGray;

                buffer.Add(($"{prefix}{item.Title}", Format.NumberedList, itemColor));

                if (item.StatusProvider != null)
                {
                    MenuStatus status = item.StatusProvider();
                    if (!string.IsNullOrWhiteSpace(status.Text))
                        buffer.Add(($" [{status.Text}]", Format.Default, status.GetColor(isSelected)));
                }

                buffer.Add(("\n", Format.Default, Color.Default));
            }

            // Индикатор, что снизу есть скрытые элементы
            int remainingBelow = items.Count - endIndex;
            if (remainingBelow > 0)
                buffer.Add(($"  V ... ещё {remainingBelow} снизу ...\n", Format.Default, Color.DarkGray));

            _printer.Print(buffer, startNumber: _scrollOffset + 1);
        }
    }
}
