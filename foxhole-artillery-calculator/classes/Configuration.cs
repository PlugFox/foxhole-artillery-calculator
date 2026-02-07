using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace foxhole_artillery_calculator.classes
{
    /// <summary>
    /// Application configuration loaded from config.yaml
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Hotkey mappings for various actions
        /// </summary>
        [YamlMember(Alias = "hotkeys")]
        public HotkeyConfiguration Hotkeys { get; set; } = new HotkeyConfiguration();

        /// <summary>
        /// UI settings
        /// </summary>
        [YamlMember(Alias = "ui")]
        public UIConfiguration UI { get; set; } = new UIConfiguration();

        /// <summary>
        /// Artillery weapon ranges
        /// </summary>
        [YamlMember(Alias = "artillery")]
        public ArtilleryConfiguration Artillery { get; set; } = new ArtilleryConfiguration();

        /// <summary>
        /// General application settings
        /// </summary>
        [YamlMember(Alias = "general")]
        public GeneralConfiguration General { get; set; } = new GeneralConfiguration();
    }

    /// <summary>
    /// Hotkey configuration
    /// </summary>
    public class HotkeyConfiguration
    {
        /// <summary>
        /// Hotkey for enemy coordinates input (default: SUBTRACT / NumPad -)
        /// </summary>
        [YamlMember(Alias = "enemy_coordinates")]
        public string EnemyCoordinates { get; set; } = "SUBTRACT";

        /// <summary>
        /// Hotkey for friendly coordinates input (default: ADD / NumPad +)
        /// </summary>
        [YamlMember(Alias = "friendly_coordinates")]
        public string FriendlyCoordinates { get; set; } = "ADD";

        /// <summary>
        /// Hotkey for taking a screenshot (default: SNAPSHOT / PrtScr)
        /// </summary>
        [YamlMember(Alias = "screenshot")]
        public string Screenshot { get; set; } = "SNAPSHOT";

        /// <summary>
        /// Hotkey for changing window resolution (default: MULTIPLY / NumPad *)
        /// </summary>
        [YamlMember(Alias = "change_resolution")]
        public string ChangeResolution { get; set; } = "MULTIPLY";

        /// <summary>
        /// Hotkey for minimize/maximize window (default: DECIMAL / NumPad .)
        /// </summary>
        [YamlMember(Alias = "toggle_window")]
        public string ToggleWindow { get; set; } = "DECIMAL";
    }

    /// <summary>
    /// UI configuration
    /// </summary>
    public class UIConfiguration
    {
        /// <summary>
        /// Color for active input field (default: Lime)
        /// </summary>
        [YamlMember(Alias = "active_field_color")]
        public string ActiveFieldColor { get; set; } = "Lime";

        /// <summary>
        /// Color for artillery in range (default: #7F00FF00)
        /// </summary>
        [YamlMember(Alias = "in_range_color")]
        public string InRangeColor { get; set; } = "#7F00FF00";

        /// <summary>
        /// Color for artillery out of range (default: #7FFF0000)
        /// </summary>
        [YamlMember(Alias = "out_of_range_color")]
        public string OutOfRangeColor { get; set; } = "#7FFF0000";

        /// <summary>
        /// Enable sound when announcing coordinates
        /// </summary>
        [YamlMember(Alias = "enable_sound")]
        public bool EnableSound { get; set; } = true;

        /// <summary>
        /// Voice culture for text-to-speech (default: en-US)
        /// </summary>
        [YamlMember(Alias = "voice_culture")]
        public string VoiceCulture { get; set; } = "en-US";
    }

    /// <summary>
    /// Artillery weapon ranges configuration
    /// </summary>
    public class ArtilleryConfiguration
    {
        /// <summary>
        /// Mortar range
        /// </summary>
        [YamlMember(Alias = "mortar")]
        public RangeConfiguration Mortar { get; set; } = RangeConfiguration.CreateMortarDefault();

        /// <summary>
        /// Field Artillery range
        /// </summary>
        [YamlMember(Alias = "field_artillery")]
        public RangeConfiguration FieldArtillery { get; set; } = RangeConfiguration.CreateFieldArtilleryDefault();

        /// <summary>
        /// Howitzer range
        /// </summary>
        [YamlMember(Alias = "howitzer")]
        public RangeConfiguration Howitzer { get; set; } = RangeConfiguration.CreateHowitzerDefault();

        /// <summary>
        /// Gunship range
        /// </summary>
        [YamlMember(Alias = "gunship")]
        public RangeConfiguration Gunship { get; set; } = RangeConfiguration.CreateGunshipDefault();
    }

    /// <summary>
    /// Range configuration for an artillery weapon
    /// </summary>
    public class RangeConfiguration
    {
        /// <summary>
        /// Minimum effective range
        /// </summary>
        [YamlMember(Alias = "min_range")]
        public double MinRange { get; set; }

        /// <summary>
        /// Maximum effective range
        /// </summary>
        [YamlMember(Alias = "max_range")]
        public double MaxRange { get; set; }

        // Default range constants
        private const double MORTAR_MIN = 44;
        private const double MORTAR_MAX = 66;
        private const double FIELD_ARTILLERY_MIN = 74;
        private const double FIELD_ARTILLERY_MAX = 151;
        private const double HOWITZER_MIN = 59;
        private const double HOWITZER_MAX = 166;
        private const double GUNSHIP_MIN = 49;
        private const double GUNSHIP_MAX = 101;

        /// <summary>
        /// Creates default Mortar range configuration
        /// </summary>
        public static RangeConfiguration CreateMortarDefault() => 
            new RangeConfiguration { MinRange = MORTAR_MIN, MaxRange = MORTAR_MAX };

        /// <summary>
        /// Creates default Field Artillery range configuration
        /// </summary>
        public static RangeConfiguration CreateFieldArtilleryDefault() => 
            new RangeConfiguration { MinRange = FIELD_ARTILLERY_MIN, MaxRange = FIELD_ARTILLERY_MAX };

        /// <summary>
        /// Creates default Howitzer range configuration
        /// </summary>
        public static RangeConfiguration CreateHowitzerDefault() => 
            new RangeConfiguration { MinRange = HOWITZER_MIN, MaxRange = HOWITZER_MAX };

        /// <summary>
        /// Creates default Gunship range configuration
        /// </summary>
        public static RangeConfiguration CreateGunshipDefault() => 
            new RangeConfiguration { MinRange = GUNSHIP_MIN, MaxRange = GUNSHIP_MAX };
    }

    /// <summary>
    /// General application settings
    /// </summary>
    public class GeneralConfiguration
    {
        /// <summary>
        /// Enable beep sound on field change
        /// </summary>
        [YamlMember(Alias = "beep_on_field_change")]
        public bool BeepOnFieldChange { get; set; } = false;

        /// <summary>
        /// Screenshot width in pixels
        /// </summary>
        [YamlMember(Alias = "screenshot_width")]
        public int ScreenshotWidth { get; set; } = 100;

        /// <summary>
        /// Screenshot height in pixels
        /// </summary>
        [YamlMember(Alias = "screenshot_height")]
        public int ScreenshotHeight { get; set; } = 50;
    }
}
