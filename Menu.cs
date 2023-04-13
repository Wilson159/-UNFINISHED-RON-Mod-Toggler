using System;
using static System.Console;

namespace RON_Mod_Toggler
{
    class Menu
    {
        private int _selectedIndex;
        private string[] _options;
        private string _prompt;
        private string _tip;

        public Menu(string prompt, string[] options, string tip = null)
        {
            _selectedIndex = 0;
            _tip = tip;
            _prompt = prompt;
            _options = options;
        }

        private void DisplayOptions()
        {
            WriteLine(_prompt);
            if (!string.IsNullOrEmpty(_tip))
            {
                ForegroundColor = ConsoleColor.DarkGray;
                WriteLine(_tip);
                ResetColor();
                WriteLine(" ");
            }
            else
            {
                WriteLine(" ");
            }

            for (int i = 0; i < _options.Length; i++)
            {
                string currentOption = _options[i];
                string prefix;
                string suffix;

                if (i == _selectedIndex)
                {
                    prefix = ">";
                    suffix = "<";
                }
                else
                {
                    prefix = " ";
                    suffix = " ";
                }

                WriteLine($"{prefix} [{currentOption}] {suffix}");
                CursorVisible = false;
            }
        }

        public int Run()
        {
            ConsoleKey keyPressed;
            do
            {
                Clear();
                DisplayOptions();

                ConsoleKeyInfo keyInfo = ReadKey(true);
                keyPressed = keyInfo.Key;

                if (keyPressed == ConsoleKey.UpArrow)
                {
                    _selectedIndex--;
                    if (_selectedIndex == -1) { _selectedIndex = _options.Length - 1; }
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    _selectedIndex++;
                    if (_selectedIndex == _options.Length) { _selectedIndex = 0; }
                }

            } while (keyPressed != ConsoleKey.Enter);

            return _selectedIndex;
        }
    }
}