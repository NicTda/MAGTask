//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// Extension class for localising text
    ///
    public static class LocalisationExtension
    {
        private static LocalisationService m_cachedLocalisationService = null;

        #region Public functions
        /// @param text
        /// 	The text to localise
        /// @oaram args
        ///     The format arguments
        ///
        public static string Localise(this string text, params object[] args)
        {
            return text.Localise(string.Empty, args);
        }

        /// @param text
        /// 	The text to localise
        /// @oaram args
        ///     The format arguments
        ///
        public static string LocaliseGame(this string text, params object[] args)
        {
            return text.Localise(LocalisedTextIdentifiers.k_categoryGame, args);
        }

        /// @param text
        /// 	The text to localise
        /// @param category
        ///     The category to check
        /// @oaram args
        ///     The format arguments
        ///
        public static string Localise(this string text, string category, params object[] args)
        {
            string localisedText = text;
            if(string.IsNullOrEmpty(text) == false)
            {
                // Cache the localisation service if needed
                if (m_cachedLocalisationService == null)
                {
                    m_cachedLocalisationService = GlobalDirector.Service<LocalisationService>();
                }

                // Localise the text
                if (string.IsNullOrEmpty(category) == false)
                {
                    localisedText = m_cachedLocalisationService.GetText(category, text);
                }
                else
                {
                    localisedText = m_cachedLocalisationService.GetText(text);
                }

                // Apply arguments if available
                if (args.Length > 0)
                {
                    localisedText = string.Format(localisedText, args);
                }
            }
            return localisedText;
        }

        /// @param text
        /// 	The text to localise
        /// @oaram args
        ///     The format arguments
        ///
        public static string LocaliseGameRandom(this string text, params object[] args)
        {
            return text.LocaliseRandom(LocalisedTextIdentifiers.k_categoryGame, args);
        }

        /// @param text
        /// 	The text to localise
        /// @param category
        ///     The category to check
        /// @oaram args
        ///     The format arguments
        ///
        public static string LocaliseRandom(this string text, string category, params object[] args)
        {
            // Cache the localisation service if needed
            if (m_cachedLocalisationService == null)
            {
                m_cachedLocalisationService = GlobalDirector.Service<LocalisationService>();
            }

            // Localise the text
            string localisedText = m_cachedLocalisationService.GetRandomText(category, text);

            // Apply arguments if available
            if (args.Length > 0)
            {
                localisedText = string.Format(localisedText, args);
            }
            return localisedText;
        }
        #endregion
    }
}
