//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// Lists the available audio IDs
    /// 
    public partial class AudioIdentifiers
    {
        /// Musics
        ///
        public const string k_musicMain = "POL-puzzle-kid-short";
        public const string k_musicLevel = "POL-castle-rooms-short";

        /// SFX
        ///
        public const string k_sfxButtonBack = "buttonNeutral";
        public const string k_sfxButtonPositive = "buttonPositive";
        public const string k_sfxButtonNeutral = "buttonNeutral";
        public const string k_sfxButtonSelect = "button_select";
        public const string k_sfxStarburst = "starburst";

        /// Map SFX
        /// 
        public const string k_sfxMapUnlock = "map_unlock";

        /// Level SFX
        /// 
        public const string k_sfxPopPositive = "pop_positive";
        public const string k_sfxPopNegative = "pop_negative";
        public const string k_sfxTileSelect = "tile_select";
        public const string k_sfxTileDeselect = "tile_deselect";
        public const string k_sfxLevelSelect = "level_select";
        public const string k_sfxLevelFail = "level_failed";
        public const string k_sfxLevelComplete = "level_complete";
        public const string k_sfxObjectiveComplete = "objective_complete";

        /// Looped SFX
        /// 
        public const string k_loopScoreCounting = "score_counting";

        /// Popups
        /// 
        public const string k_sfxPopupPresent = "popup_present";
        public const string k_sfxPopupDismiss = "";
        public const string k_sfxPopupError = "popup_error";
    }
}
