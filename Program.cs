using System;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Console;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RON_Mod_Toggler
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            Title = "RON Mod Toggler";

            if (File.Exists(@"C:\ProgramData\RON Mod Toggler\config"))
            {
                SkipConfig();
            }
            else
            {
                string logo = @"
█▀█ █▀█ █▄░█   █▀▄▀█ █▀█ █▀▄   ▀█▀ █▀█ █▀▀ █▀▀ █░░ █▀▀ █▀█   █▀▀ █▀█ █▄░█ █▀▀ █ █▀▀
█▀▄ █▄█ █░▀█   █░▀░█ █▄█ █▄▀   ░█░ █▄█ █▄█ █▄█ █▄▄ ██▄ █▀▄   █▄▄ █▄█ █░▀█ █▀░ █ █▄█
            ";

                string prompt = $"{logo}\nPlease select an option below by pressing [ENTER].";
                string[] options = { "Configure", "About", "Exit" };
                string tip = "Use your Up Arrow and Down Arrow to navigate!";
                Menu mainMenu = new Menu(prompt, options, tip);
                int selectedIndex = mainMenu.Run();

                switch (selectedIndex)
                {
                    case 0:
                        Clear();
                        Configure();
                        break;
                    case 1:
                        Clear();
                        About();
                        break;
                    case 2:
                        Clear();
                        Exit();
                        break;
                }
            }
        }

        private static void SkipConfig()
        {
            string logo = @"
█▀█ █▀█ █▄░█   █▀▄▀█ █▀█ █▀▄   ▀█▀ █▀█ █▀▀ █▀▀ █░░ █▀▀ █▀█
█▀▄ █▄█ █░▀█   █░▀░█ █▄█ █▄▀   ░█░ █▄█ █▄█ █▄█ █▄▄ ██▄ █▀▄
            ";

            string prompt = $"{logo}\nPlease select an option below by pressing [ENTER].";
            string[] options = { "Mod List", "About", "Exit" };
            string tip = "Use your Up Arrow and Down Arrow to navigate!";
            Menu mainMenu = new Menu(prompt, options, tip);
            int selectedIndex = mainMenu.Run();

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    ModList();
                    break;
                case 1:
                    Clear();
                    About();
                    break;
                case 2:
                    Clear();
                    Exit();
                    break;
            }
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }

        private static void About()
        {
            ForegroundColor = ConsoleColor.White;
            WriteLine("About:");

            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine("RON Mod Toggler is a tool made for easily toggling on and off your Ready Or Not mods.\nUseful for playing with people that have no/certain mods.");
            ForegroundColor = ConsoleColor.White;
            WriteLine("\nCredits:");

            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine("- Discord: Wilson#0159\n- GitHub: https://github.com/Wilson159");

            ForegroundColor = ConsoleColor.DarkGreen;
            WriteLine("\nPress any key to return to the main menu.");
            ResetColor();
            ReadKey(true);
            Main();
        }

        private static void Configure(string promptParam = null)
        {
            string prompt = promptParam;
            if (String.IsNullOrEmpty(prompt)) { prompt = "Configuring requires selecting your Ready Or Not Pak folder."; }

            string tip = @"E.g. C:\Program Files (x86)\Steam\steamapps\common\Ready Or Not\ReadyOrNot\Content\Paks";
            string[] options = { "Continue", "Exit" };
            Menu menu = new Menu(prompt, options, tip);
            int selectedIndex = menu.Run();

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    SelectFolder();
                    break;
                case 1:
                    Clear();
                    Exit();
                    break;
            }
        }

        private static void SelectFolder()
        {


            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Select your Ready Or Not Pak folder.";
            dialog.InitialDirectory = "C:";
            dialog.IsFolderPicker = true;

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string[] splitPath = dialog.FileName.Split('\\');
                int splitLength = splitPath.Length;

                if ((splitLength < 3) || (splitPath[splitLength - 1] != "Paks" && splitPath[splitLength - 2] != "Content" && splitPath[splitLength - 3] != "ReadyOrNot"))
                {
                    string prompt = "Incorrect folder selection, try again or contact Wilson#0159 if you believe this is an error.";
                    Configure(prompt);
                }
                else
                {
                    Directory.CreateDirectory(@"C:\ProgramData\RON Mod Toggler\");

                    Dictionary<string, Dictionary<string, string>> data =
                    new Dictionary<string, Dictionary<string, string>>
                    {
                        {
                            "General Config",
                            new Dictionary<string, string>
                            {
                                {"path", $"{dialog.FileName}"},
                            }
                        },
                        {
                            "Enabled",
                            new Dictionary<string, string>()
                        },
                        {
                            "Disabled",
                            new Dictionary<string, string>()
                        }
                    };

                    foreach (string file in Directory.GetFiles(dialog.FileName))
                    {
                        string name = file.Remove(0, 83);
                        name = Regex.Replace(name, @"(pakchunk\d{1,})(-)(Mods)?(-)?(_)?", "");

                        string extension = name.Substring(name.Length - 4);

                        if (name == "WindowsNoEditor.pak") { continue; }

                        if (extension == ".pak") { 
                            name = name.Remove(name.Length - 4, 4);

                            data["Enabled"].Add(name, file);
                        } else
                        {
                            data["Disabled"].Add(name, file);
                        }
                    }

                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                    File.WriteAllText(@"C:\ProgramData\RON Mod Toggler\config", json);
                }

                SkipConfig();
            }
        }

        private static void ModList()
        {
            Dictionary<string, Dictionary<string, string>> data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(@"C:\ProgramData\RON Mod Toggler\config"));

            ForegroundColor = ConsoleColor.DarkGreen;
            WriteLine("Enabled:");
            ResetColor();
            foreach (KeyValuePair<string,string> kvp in data["Enabled"])
            {
                WriteLine($" - {kvp.Key}");
            }

            ForegroundColor = ConsoleColor.DarkRed;
            WriteLine("\nDisabled:");
            ResetColor();
            foreach (KeyValuePair<string, string> kvp in data["Disabled"])
            {
                WriteLine($" - {kvp.Key}");
            }

            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine("\n\nPress [ENTER] to return to the menu.");
            ResetColor();
            ReadKey(true);
            SkipConfig();
        }
    }
}