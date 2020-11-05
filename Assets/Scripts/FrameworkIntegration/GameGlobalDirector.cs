//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Game global director
    /// 
    public sealed class GameGlobalDirector : GlobalDirector
    {
        [SerializeField]
        private bool m_debugStart = false;

        private LocalisationService m_localisationService = null;
        private MetadataService m_metadataService = null;

        #region GlobalDirector functions
        /// Register global services
        /// 
        protected override void OnStartAddServicesState()
        {
            // Add services
            m_serviceSupplier.RegisterService<SceneService>();
            m_serviceSupplier.RegisterService<SaveService>();
            m_serviceSupplier.RegisterService<TaskSchedulerService>();
            m_serviceSupplier.RegisterService<PopupService>();
            m_serviceSupplier.RegisterService<MetadataService>();
            m_serviceSupplier.RegisterService<InputService>();
            m_serviceSupplier.RegisterService<AudioService>();
            m_serviceSupplier.RegisterService<TimeService>();
            m_serviceSupplier.RegisterService<BankService>();
            m_serviceSupplier.RegisterService<LocalSaveService>();
            //m_serviceSupplier.RegisterService<GameTransactionService>();
            //m_serviceSupplier.RegisterService<GameSettingsService>();
            //m_serviceSupplier.RegisterService<RewardService>();
            //m_serviceSupplier.RegisterService<TutorialService>();
            //m_serviceSupplier.RegisterService<CeremonyService>();

            // Custom popups
            PopupService.PrefabPath = "Prefabs/PopupCanvas";
            PopupService.InfoID = PopupIdentifiers.k_gameInfo;
            PopupService.QuestionID = PopupIdentifiers.k_gameQuestion;

            // Custom sounds for popups
            //PopupView.s_sfxConfirmPressed = AudioIdentifiers.k_sfxButtonPositive;
            PopupView.s_sfxBackPressed = AudioIdentifiers.k_sfxButtonBack;
            //PopupView.s_sfxPresent = AudioIdentifiers.k_sfxPopupPresent;
            //PopupView.s_sfxDismiss = AudioIdentifiers.k_sfxPopupDismiss;

            // Starting currencies
            var gameProgressService = m_serviceSupplier.RegisterService<GameProgressService>();
            //gameProgressService.AddStartCurrency(OverlayBankIdentifiers.k_currencyPremium, 500);

            // Metadata
            m_metadataService = m_serviceSupplier.GetService<MetadataService>();
            m_metadataService.SetDefaultLoader(typeof(LocalisedTextData), typeof(LocalisedTextLoader));

            // Localisation
            m_localisationService = m_serviceSupplier.RegisterService<LocalisationService>();
            m_localisationService.AddCategoryToLoad(LocalisedTextIdentifiers.k_categoryGame);
        }
        #endregion
    }
}

