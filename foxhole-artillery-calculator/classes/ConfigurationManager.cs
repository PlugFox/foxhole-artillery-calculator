using System;
using System.IO;
using System.Globalization;

namespace foxhole_artillery_calculator.classes
{
    /// <summary>
    /// Manages application configuration using simple INI file format
    /// </summary>
    public class ConfigurationManager
    {
        private static readonly string ConfigFileName = "config.ini";
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FoxholeArtilleryCalculator",
            ConfigFileName
        );

        private static AppConfiguration _instance;

        /// <summary>
        /// Gets the current configuration instance
        /// </summary>
        public static AppConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadConfiguration();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loads configuration from INI file or creates default if not exists
        /// </summary>
        private static AppConfiguration LoadConfiguration()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(ConfigFilePath))
                {
                    var defaultConfig = new AppConfiguration();
                    SaveConfiguration(defaultConfig);
                    return defaultConfig;
                }

                var config = new AppConfiguration();
                string[] lines = File.ReadAllLines(ConfigFilePath);
                string currentSection = "";

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith(";"))
                        continue;

                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        currentSection = trimmed.Substring(1, trimmed.Length - 2).ToLower();
                        continue;
                    }

                    int equalsIndex = trimmed.IndexOf('=');
                    if (equalsIndex <= 0)
                        continue;

                    string key = trimmed.Substring(0, equalsIndex).Trim();
                    string value = trimmed.Substring(equalsIndex + 1).Trim();

                    ApplyConfigValue(config, currentSection, key, value);
                }

                return config;
            }
            catch (Exception ex)
            {
                LogError("Error loading configuration", ex);
                return new AppConfiguration();
            }
        }

        /// <summary>
        /// Applies a configuration value to the appropriate property
        /// </summary>
        private static void ApplyConfigValue(AppConfiguration config, string section, string key, string value)
        {
            try
            {
                switch (section)
                {
                    case "hotkeys":
                        ApplyHotkeyValue(config.Hotkeys, key, value);
                        break;
                    case "ui":
                        ApplyUIValue(config.UI, key, value);
                        break;
                    case "general":
                        ApplyGeneralValue(config.General, key, value);
                        break;
                    case "mortar":
                        ApplyRangeValue(config.Artillery.Mortar, key, value);
                        break;
                    case "field_artillery":
                        ApplyRangeValue(config.Artillery.FieldArtillery, key, value);
                        break;
                    case "howitzer":
                        ApplyRangeValue(config.Artillery.Howitzer, key, value);
                        break;
                    case "gunship":
                        ApplyRangeValue(config.Artillery.Gunship, key, value);
                        break;
                }
            }
            catch
            {
                // Ignore invalid values
            }
        }

        private static void ApplyHotkeyValue(HotkeyConfiguration hotkeys, string key, string value)
        {
            switch (key.ToLower())
            {
                case "enemy_coordinates": hotkeys.EnemyCoordinates = value; break;
                case "friendly_coordinates": hotkeys.FriendlyCoordinates = value; break;
                case "screenshot": hotkeys.Screenshot = value; break;
                case "change_resolution": hotkeys.ChangeResolution = value; break;
                case "toggle_window": hotkeys.ToggleWindow = value; break;
            }
        }

        private static void ApplyUIValue(UIConfiguration ui, string key, string value)
        {
            switch (key.ToLower())
            {
                case "active_field_color": ui.ActiveFieldColor = value; break;
                case "in_range_color": ui.InRangeColor = value; break;
                case "out_of_range_color": ui.OutOfRangeColor = value; break;
                case "enable_sound": ui.EnableSound = ParseBool(value); break;
                case "voice_culture": ui.VoiceCulture = value; break;
            }
        }

        private static void ApplyGeneralValue(GeneralConfiguration general, string key, string value)
        {
            switch (key.ToLower())
            {
                case "beep_on_field_change": general.BeepOnFieldChange = ParseBool(value); break;
                case "screenshot_width": general.ScreenshotWidth = int.Parse(value); break;
                case "screenshot_height": general.ScreenshotHeight = int.Parse(value); break;
            }
        }

        private static void ApplyRangeValue(RangeConfiguration range, string key, string value)
        {
            switch (key.ToLower())
            {
                case "min_range": range.MinRange = double.Parse(value, CultureInfo.InvariantCulture); break;
                case "max_range": range.MaxRange = double.Parse(value, CultureInfo.InvariantCulture); break;
            }
        }

        private static bool ParseBool(string value)
        {
            value = value.ToLower();
            return value == "true" || value == "1" || value == "yes" || value == "on";
        }

        /// <summary>
        /// Saves configuration to INI file
        /// </summary>
        public static void SaveConfiguration(AppConfiguration config)
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(ConfigFilePath))
                {
                    writer.WriteLine("# Foxhole Artillery Calculator Configuration");
                    writer.WriteLine("# This file is automatically generated with default values");
                    writer.WriteLine("# You can customize hotkeys and other settings here");
                    writer.WriteLine();

                    writer.WriteLine("[Hotkeys]");
                    writer.WriteLine("# Available hotkeys: NUMPAD0-NUMPAD9, ADD, SUBTRACT, MULTIPLY, DIVIDE, DECIMAL");
                    writer.WriteLine("# Function keys: F1-F24, SNAPSHOT (PrtScr), etc.");
                    writer.WriteLine("enemy_coordinates = " + config.Hotkeys.EnemyCoordinates);
                    writer.WriteLine("friendly_coordinates = " + config.Hotkeys.FriendlyCoordinates);
                    writer.WriteLine("screenshot = " + config.Hotkeys.Screenshot);
                    writer.WriteLine("change_resolution = " + config.Hotkeys.ChangeResolution);
                    writer.WriteLine("toggle_window = " + config.Hotkeys.ToggleWindow);
                    writer.WriteLine();

                    writer.WriteLine("[UI]");
                    writer.WriteLine("active_field_color = " + config.UI.ActiveFieldColor);
                    writer.WriteLine("in_range_color = " + config.UI.InRangeColor);
                    writer.WriteLine("out_of_range_color = " + config.UI.OutOfRangeColor);
                    writer.WriteLine("enable_sound = " + config.UI.EnableSound.ToString().ToLower());
                    writer.WriteLine("voice_culture = " + config.UI.VoiceCulture);
                    writer.WriteLine();

                    writer.WriteLine("[General]");
                    writer.WriteLine("beep_on_field_change = " + config.General.BeepOnFieldChange.ToString().ToLower());
                    writer.WriteLine("screenshot_width = " + config.General.ScreenshotWidth);
                    writer.WriteLine("screenshot_height = " + config.General.ScreenshotHeight);
                    writer.WriteLine();

                    writer.WriteLine("[Mortar]");
                    writer.WriteLine("min_range = " + config.Artillery.Mortar.MinRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("max_range = " + config.Artillery.Mortar.MaxRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();

                    writer.WriteLine("[Field_Artillery]");
                    writer.WriteLine("min_range = " + config.Artillery.FieldArtillery.MinRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("max_range = " + config.Artillery.FieldArtillery.MaxRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();

                    writer.WriteLine("[Howitzer]");
                    writer.WriteLine("min_range = " + config.Artillery.Howitzer.MinRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("max_range = " + config.Artillery.Howitzer.MaxRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();

                    writer.WriteLine("[Gunship]");
                    writer.WriteLine("min_range = " + config.Artillery.Gunship.MinRange.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine("max_range = " + config.Artillery.Gunship.MaxRange.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                LogError("Error saving configuration", ex);
            }
        }

        /// <summary>
        /// Logs error to a file
        /// </summary>
        private static void LogError(string message, Exception ex)
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath);
                string logFilePath = Path.Combine(directory, "error.log");
                string logMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}\n{3}\n\n",
                    DateTime.Now, message, ex.Message, ex.StackTrace);
                File.AppendAllText(logFilePath, logMessage);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}", message, ex.Message));
            }
        }

        /// <summary>
        /// Reloads configuration from file
        /// </summary>
        public static void ReloadConfiguration()
        {
            _instance = LoadConfiguration();
        }

        /// <summary>
        /// Gets the configuration file path
        /// </summary>
        public static string GetConfigFilePath()
        {
            return ConfigFilePath;
        }

        /// <summary>
        /// Parses a hotkey string to VKeys enum
        /// </summary>
        public static KeyboardHook.VKeys ParseHotkey(string hotkeyString)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString))
            {
                throw new ArgumentException("Hotkey string cannot be empty");
            }

            if (Enum.TryParse<KeyboardHook.VKeys>(hotkeyString, true, out var vkey))
            {
                return vkey;
            }

            throw new ArgumentException(string.Format("Invalid hotkey: {0}", hotkeyString));
        }
    }
}
