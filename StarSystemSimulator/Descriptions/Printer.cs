using StarSystemSimulator.Extensions;
using StarSystemSimulator.Utils;
using System.Text;

namespace StarSystemSimulator.Descriptions
{
    public class Printer
    {
        private const string AnsiReset = "\u001b[0m";
        private const string AnsiBold = "\u001b[1m";

        /// <summary>
        /// Выводит список форматированных фрагментов подряд (без автоматического переноса в конце)
        /// </summary>
        public void Print(IEnumerable<(string str, Format format, Color color)> fragments, int startNumber = 1)
        {
            var sb = new StringBuilder();
            int currentNumber = startNumber;

            foreach (var (str, fmt, clr) in fragments)
            {
                AppendFormatted(sb, str, fmt, clr.ToAnsiString(), ref currentNumber, startNumber);
            }

            sb.Append(AnsiReset);
            Console.Write(sb.ToString());
        }

        /// <summary>
        /// Выводит один форматированный фрагмент с переносом строки в конце
        /// </summary>
        public void PrintLine(string str = "", Format format = Format.Default, Color color = Color.Default)
        {
            Print([(str, format, color), ("\n", Format.Default, Color.Default)]);
        }

        /// <summary>
        /// Выводит список фрагментов в одну строку и добавляет перенос строки в самом конце
        /// </summary>
        public void PrintLine(IEnumerable<(string str, Format format, Color color)> fragments)
        {
            var list = new List<(string, Format, Color)>(fragments)
            {
                ("\n", Format.Default, Color.Default)
            };
            Print(list);
        }

        /// <summary>
        /// Выводит каждый элемент списка с новой строки
        /// </summary>
        public void PrintLines(IEnumerable<(string str, Format format, Color color)> lines)
        {
            var sb = new StringBuilder();
            int currentNumber = 1;

            foreach (var (str, fmt, clr) in lines)
            {
                AppendFormatted(sb, str, fmt, clr.ToAnsiString(), ref currentNumber);
                if (!str.EndsWith('\n'))
                {
                    sb.AppendLine();
                }
            }

            sb.Append(AnsiReset);
            Console.Write(sb.ToString());
        }

        private static void AppendFormatted(StringBuilder sb, string str, Format fmt, string color, ref int currentNumber, int startNumber = 1)
        {
            switch (fmt)
            {
                case Format.Title:
                    currentNumber = startNumber;
                    sb.Append(color);
                    sb.AppendLine("========================================");
                    sb.Append($"{AnsiBold}\t{str.TrimEnd('\n')}{AnsiReset}{color}");
                    sb.AppendLine();
                    sb.AppendLine("========================================");
                    break;

                case Format.NumberedList:
                    sb.Append($"{AnsiBold}{currentNumber++}.\t{AnsiReset}{color}{str}");
                    break;

                case Format.PointedList:
                    sb.Append($"{AnsiBold}*\t{AnsiReset}{color}{str}");
                    break;

                case Format.Default:
                default:
                    sb.Append($"{color}{str}");
                    break;
            }

            sb.Append(AnsiReset);
        }
    }
}
