using StarSystemSimulator.Utils;
using StarSystemSimulator.UI;

namespace StarSystemSimulator.Extensions
{
    public static class ToStringExtensions
    {
        public static MenuStatus ToMenuStatus(this string text, Color normal = Color.DarkGray, Color selected = Color.Gray)
        {
            return new(text, normal, selected);
        }

        public static string ToRusString(this StarObjectType type, bool needCapitalLetter = false)
        {
            string name = type switch
            {
                StarObjectType.StarSystem => "звездная система",
                StarObjectType.MainStar => "звезда основной последовательности",
                StarObjectType.NeutronStar => "нейтронная звезда",
                StarObjectType.RedGiantStar => "красный гигант",
                StarObjectType.WhiteDwarfStar => "белый карлик",
                StarObjectType.EarthPlanet => "планета земного типа",
                StarObjectType.GiantPlanet => "газовый гигант",
                StarObjectType.Satellite => "спутник",
                StarObjectType.Asteroid => "астероид",
                StarObjectType.Comet => "комета",
                _ => "неизвестный"
            };

            return needCapitalLetter
                ? char.ToUpper(name[0]) + name[1..]
                : name;
        }

        public static string ToRusString(this AsteroidType type, bool needCapitalLetter = false)
        {
            string name = type switch
            {
                AsteroidType.Carbonaceous => "углеродный",
                AsteroidType.Silicate => "силикатный",
                AsteroidType.Metallic => "металлический",
                _ => "неизвестный"
            };

            return needCapitalLetter
                ? char.ToUpper(name[0]) + name[1..]
                : name;
        }

        public static string ToRusString(this Color color, bool needCapitalLetter = false)
        {
            string name = color switch
            {
                Color.Blue => "синий",
                Color.LightBlue => "голубой",
                Color.White => "белый",
                Color.LightYellow => "ярко-желтый",
                Color.Yellow => "желтый",
                Color.Orange => "оранжевый",
                Color.Red => "красный",

                Color.Cyan => "бирюзовый",
                Color.LightCyan => "светло-бирюзовый",
                Color.Teal => "сине-зеленый",
                Color.DeepBlue => "темно-синий",

                Color.Green => "зеленый",
                Color.LightGreen => "салатовый",
                Color.Lime => "лаймовый",
                Color.Olive => "оливковый",

                Color.Purple => "фиолетовый",
                Color.Violet => "сиреневый",
                Color.Magenta => "пурпурный",
                Color.Pink => "розовый",

                Color.Brown => "коричневый",
                Color.Sand => "песочный",
                Color.Gold => "золотистый",
                Color.Crimson => "малиновый",

                Color.LightGray => "светло-серый",
                Color.Gray => "серый",
                Color.DarkGray => "темно-серый",

                Color.Default => "системный",

                _ => "неизвестный"
            };

            return needCapitalLetter
                ? char.ToUpper(name[0]) + name[1..]
                : name;
        }

        public static string ToAnsiString(this Color color) => color switch
        {
            Color.Blue => "\u001b[38;2;155;176;255m",           // #9BB0FF
            Color.LightBlue => "\u001b[38;2;170;191;255m",      // #AABFFF
            Color.White => "\u001b[38;2;248;247;255m",          // #F8F7FF
            Color.LightYellow => "\u001b[38;2;255;244;234m",    // #FFF4EA
            Color.Yellow => "\u001b[38;2;255;210;161m",         // #FFD2A1
            Color.Orange => "\u001b[38;2;255;165;0m",           // #FFA500
            Color.Red => "\u001b[38;2;255;90;70m",              // #FF5A46
            Color.Cyan => "\u001b[38;2;100;220;235m",           // #64DCEB
            Color.LightCyan => "\u001b[38;2;175;238;238m",      // #AFEEEE
            Color.Teal => "\u001b[38;2;70;175;175m",            // #46AFAF
            Color.DeepBlue => "\u001b[38;2;85;120;220m",        // #5578DC

            Color.Green => "\u001b[38;2;110;210;130m",          // #6ED282
            Color.LightGreen => "\u001b[38;2;165;240;175m",     // #A5F0AF
            Color.Lime => "\u001b[38;2;185;235;95m",            // #B9EB5F
            Color.Olive => "\u001b[38;2;160;175;105m",          // #A0AF69

            Color.Purple => "\u001b[38;2;180;130;245m",         // #B482F5
            Color.Violet => "\u001b[38;2;205;165;255m",         // #CDA5FF
            Color.Magenta => "\u001b[38;2;235;110;195m",        // #EB6EC3
            Color.Pink => "\u001b[38;2;255;170;205m",           // #FFAACD

            Color.Brown => "\u001b[38;2;185;125;85m",           // #B97D55
            Color.Sand => "\u001b[38;2;235;205;155m",           // #EBCD9B
            Color.Gold => "\u001b[38;2;255;200;75m",            // #FFC84B
            Color.Crimson => "\u001b[38;2;215;55;75m",          // #D7374B

            Color.LightGray => "\u001b[38;2;200;205;215m",      // #C8CDD7
            Color.Gray => "\u001b[38;2;140;145;155m",           // #8C919B
            Color.DarkGray => "\u001b[38;2;90;95;105m",         // #5A5F69

            _ => "\u001b[0m"
        };
    }
}
