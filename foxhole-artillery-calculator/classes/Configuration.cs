using System;

namespace foxhole_artillery_calculator.classes
{
    /// <summary>
    /// Application configuration
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Hotkey mappings for various actions
        /// </summary>
        public HotkeyConfiguration Hotkeys { get; set; } = new HotkeyConfiguration();

        /// <summary>
        /// UI settings
        /// </summary>
        public UIConfiguration UI { get; set; } = new UIConfiguration();

        /// <summary>
        /// Artillery weapon ranges
        /// </summary>
        public ArtilleryConfiguration Artillery { get; set; } = new ArtilleryConfiguration();

        /// <summary>
        /// General application settings
        /// </summary>
        public GeneralConfiguration General { get; set; } = new GeneralConfiguration();
    }

    /// <summary>
    /// Hotkey configuration
    /// </summary>
    public class HotkeyConfiguration
    {
        public string EnemyCoordinates { get; set; } = "SUBTRACT";
        public string FriendlyCoordinates { get; set; } = "ADD";
        public string Screenshot { get; set; } = "SNAPSHOT";
        public string ChangeResolution { get; set; } = "MULTIPLY";
        public string ToggleWindow { get; set; } = "DECIMAL";
    }

    /// <summary>
    /// UI configuration
    /// </summary>
    public class UIConfiguration
    {
        public string ActiveFieldColor { get; set; } = "Lime";
        public string InRangeColor { get; set; } = "#7F00FF00";
        public string OutOfRangeColor { get; set; } = "#7FFF0000";
        public bool EnableSound { get; set; } = true;
        public string VoiceCulture { get; set; } = "en-US";
    }

    /// <summary>
    /// Artillery weapon ranges configuration
    /// </summary>
    public class ArtilleryConfiguration
    {
        public RangeConfiguration Mortar { get; set; } = RangeConfiguration.CreateMortarDefault();
        public RangeConfiguration FieldArtillery { get; set; } = RangeConfiguration.CreateFieldArtilleryDefault();
        public RangeConfiguration Howitzer { get; set; } = RangeConfiguration.CreateHowitzerDefault();
        public RangeConfiguration Gunship { get; set; } = RangeConfiguration.CreateGunshipDefault();
    }

    /// <summary>
    /// Range configuration for an artillery weapon
    /// </summary>
    public class RangeConfiguration
    {
        public double MinRange { get; set; }
        public double MaxRange { get; set; }

        private const double MORTAR_MIN = 44;
        private const double MORTAR_MAX = 66;
        private const double FIELD_ARTILLERY_MIN = 74;
        private const double FIELD_ARTILLERY_MAX = 151;
        private const double HOWITZER_MIN = 59;
        private const double HOWITZER_MAX = 166;
        private const double GUNSHIP_MIN = 49;
        private const double GUNSHIP_MAX = 101;

        public static RangeConfiguration CreateMortarDefault() => 
            new RangeConfiguration { MinRange = MORTAR_MIN, MaxRange = MORTAR_MAX };

        public static RangeConfiguration CreateFieldArtilleryDefault() => 
            new RangeConfiguration { MinRange = FIELD_ARTILLERY_MIN, MaxRange = FIELD_ARTILLERY_MAX };

        public static RangeConfiguration CreateHowitzerDefault() => 
            new RangeConfiguration { MinRange = HOWITZER_MIN, MaxRange = HOWITZER_MAX };

        public static RangeConfiguration CreateGunshipDefault() => 
            new RangeConfiguration { MinRange = GUNSHIP_MIN, MaxRange = GUNSHIP_MAX };
    }

    /// <summary>
    /// General application settings
    /// </summary>
    public class GeneralConfiguration
    {
        public bool BeepOnFieldChange { get; set; } = false;
        public int ScreenshotWidth { get; set; } = 100;
        public int ScreenshotHeight { get; set; } = 50;
    }
}
