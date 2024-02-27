namespace GTA_5_RP
{
    public partial class Program
    {
        public class Support
        {
            public static (int Index, ConsoleKey Key) Table(int Index, string Start, List<string> Selection, int Position, params ConsoleKey[] Skip)
            {
                Console.CursorVisible = false;

                const byte O = 1;
                const byte L = 8;

                ConsoleKey? Key = null;

                do
                {
                    for (int i = 0; i < Selection.Count; i++)
                    {
                        Console.SetCursorPosition(L, Position + (i / O));

                        if (i == Index)
                        {
                            if (string.IsNullOrEmpty(Selection[i]))
                            {
                                switch (Key)
                                {
                                    case ConsoleKey.UpArrow:
                                        Index -= O;
                                        i -= 2;

                                        break;

                                    case ConsoleKey.DownArrow:
                                        Index += O;

                                        break;
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;

                                Console.Write($"{Start} {Selection[i]}");
                            }
                        }
                        else
                        {
                            Console.Write(string.Empty.PadLeft(Start.Length + 1) + Selection[i]);
                        }

                        Console.ResetColor();
                    };

                    var Read = Console.ReadKey(true);

                    switch (Read.Key)
                    {
                        case ConsoleKey.UpArrow:
                            Key = ConsoleKey.UpArrow;

                            if (Index < O)
                            {
                                Index += Selection.Count - 1;
                            }
                            else
                            {
                                Index -= O;
                            }

                            break;

                        case ConsoleKey.DownArrow:
                            Key = ConsoleKey.DownArrow;

                            if (Index + O < Selection.Count)
                            {
                                Index += O;
                            }
                            else
                            {
                                Index = 0;
                            }

                            break;

                        case ConsoleKey.Tab:
                        case ConsoleKey.F5:
                        case ConsoleKey.Escape:
                            if (Skip.Contains(Read.Key))
                            {
                                return (0, Read.Key);
                            }

                            break;

                        case ConsoleKey.Oem3:
                        case ConsoleKey.OemMinus:
                        case ConsoleKey.OemPlus:
                            if (Skip.Contains(Read.Key))
                            {
                                return (Index, Read.Key);
                            }

                            break;

                        case ConsoleKey.Enter:
                            Console.CursorVisible = true;

                            return (Index, Read.Key);
                    }
                }
                while (true);
            }

            public static bool Read(out string Line)
            {
                Line = string.Empty;

                int Index = 0;

                do
                {
                    var Read = Console.ReadKey(true);

                    switch (Read.Key)
                    {
                        case ConsoleKey.Escape:
                            return false;

                        case ConsoleKey.Enter:
                            return true;

                        case ConsoleKey.Backspace:
                            if (Index > 0)
                            {
                                Line = Line.Remove(Line.Length - 1);

                                Console.Write(Read.KeyChar);
                                Console.Write(' ');
                                Console.Write(Read.KeyChar);

                                Index--;
                            }

                            break;

                        default:
                            Line += Read.KeyChar;

                            Console.Write(Read.KeyChar);

                            Index++;

                            break;
                    }
                }
                while (true);
            }
        }
    }
}
