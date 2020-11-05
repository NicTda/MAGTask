//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAGTask
{
    /// View component of the top panel area
    ///
    public sealed class TopPanelView : GameBehaviour
    {
        private const string k_topHolderPath = "Prefabs/UI/TopPanelHolder";
        private const int k_maxParticles = 10;
        private const int k_megaThreshold = 10000;
        private const int k_megaParticles = 50;

        [Serializable]
        public class TopHolder
        {
            [SerializeField]
            public RectTransform m_holder = null;
            [SerializeField]
            public Image m_flash = null;
        }
        [SerializeField]
        private List<TopPanelHolder> m_holders = new List<TopPanelHolder>(3);
        [SerializeField]
        private GameObject m_creditsMaxedHolder = null;
        [SerializeField]
        private RectTransform m_hiddenPanelsHolder = null;
        [SerializeField]
        private GameObject m_settingsHolder = null;

        private BankService m_bankService = null;
        private AudioService m_audioService = null;
        private PopupService m_popupService = null;
        private SceneService m_sceneService = null;
        private GameProgressService m_progressService = null;

        private static List<TopPanelView> s_instance = new List<TopPanelView>();
        private static bool s_showMaxed = false;

        private List<TopPanelHolder> m_extraHolders = new List<TopPanelHolder>(10);
        private Sequence m_currencySequence = null;
        private bool m_shopEnabled = true;
        private bool m_loading = false;
        private bool m_inShop = false;

        #region GameBehaviour functions
        /// OnDestroy function
        ///
        private void OnDestroy()
        {
            m_sceneService.OnSceneActive -= OnSceneActive;
            m_currencySequence.Stop();
            if (s_instance.Contains(this))
            {
                s_instance.Remove(this);
            }
        }

        /// Initialise function
        ///
        protected override void Initialise()
        {
            m_bankService = GlobalDirector.Service<BankService>();
            m_audioService = GlobalDirector.Service<AudioService>();
            m_popupService = GlobalDirector.Service<PopupService>();
            m_sceneService = GlobalDirector.Service<SceneService>();
            m_progressService = GlobalDirector.Service<GameProgressService>();

            m_sceneService.OnSceneActive += OnSceneActive;
            if (s_instance.Contains(this) == false)
            {
                s_instance.Add(this);
            }
        }
        #endregion

        #region Public functions
        /// @param enable
        ///     Whether to enable the shop or not
        /// 
        public void SetShopEnabled(bool enable)
        {
            m_shopEnabled = enable;
        }

        /// @param currencyID
        ///     The tapped currency
        ///
        public void OnCurrencyTapped(string currencyID)
        {
            //m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonPositive);
            if (m_loading == false && m_shopEnabled == true)
            {
                /*
                if(m_inShop == true)
                {
                    // We're already in the shop
                    var shopView = GameObject.FindObjectOfType<ShopView>();
                    shopView.FocusOnCurrency(currencyID);
                }
                else
                {
                    // Load the shop
                    m_loading = true;
                    ShopLocalDirector.s_currencyFocus = currencyID;
                    m_sceneService.LoadSceneAdditively(SceneIdentifiers.k_shop);
                }
                */
            }
        }

        /// Called when the player wants to see the settings
        ///
        public void OnSettingsPressed()
        {
            //m_audioService.PlaySFX(AudioIdentifiers.k_sfxButtonNeutral);
            //m_sceneService.LoadSceneAdditively(SceneIdentifiers.k_settings);
        }

        /// Stops the reward sequence
        /// 
        public static void StopRewards()
        {
            if (s_instance.Count > 0)
            {
                s_instance.GetLast().m_currencySequence.Stop();
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        public static void BankRewardScreen(CurrencyItem reward, Vector3 position, Action callback = null)
        {
            if (s_instance.Count > 0 && reward.m_value > 0)
            {
                s_instance.GetLast().SpawnRewardScreen(reward, position, true, callback);
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        public static void SpawnRewardScreen(CurrencyItem reward, Vector3 position, Action callback = null)
        {
            if (s_instance.Count > 0 && reward.m_value > 0)
            {
                s_instance.GetLast().SpawnRewardScreen(reward, position, false, callback);
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        public static void BankRewardWorld(CurrencyItem reward, Vector3 position, Action callback = null)
        {
            if (s_instance.Count > 0 && reward.m_value > 0)
            {
                s_instance.GetLast().SpawnRewardWorld(reward, position, true, callback);
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        public static void SpawnRewardWorld(CurrencyItem reward, Vector3 position, Action callback = null)
        {
            if (s_instance.Count > 0 && reward.m_value > 0)
            {
                s_instance.GetLast().SpawnRewardWorld(reward, position, false, callback);
            }
        }

        /// @param currencyID
        ///     The currency to focus on
        /// 
        public static void ShowShop(string currencyID = "")
        {
            if (s_instance.Count > 0)
            {
                s_instance.GetLast().OnCurrencyTapped(currencyID);
            }
        }
        #endregion

        #region Private functions
        /// @param sceneName
        ///     The name of the active scene
        ///
        private void OnSceneActive(string sceneName)
        {
            if(sceneName == SceneIdentifiers.k_shop)
            {
                m_inShop = true;
                m_loading = false;
            }
            else
            {
                m_inShop = false;
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param bankReward
        ///     Whether to add the reward to the bank
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        private void SpawnRewardWorld(CurrencyItem reward, Vector3 position, bool bankReward, Action callback = null)
        {
            var screenPosition = Camera.main.WorldToScreenPoint(position);
            SpawnRewardScreen(reward, screenPosition, bankReward, callback);
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param bankReward
        ///     Whether to add the reward to the bank
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        private void SpawnRewardScreen(CurrencyItem reward, Vector3 position, bool bankReward, Action callback = null)
        {
            if (reward.m_value > 0)
            {
                SpawnRewardInternal(reward, position, () =>
                {
                    if (bankReward == true)
                    {
                        m_bankService.Deposit(reward);
                    }
                    callback.SafeInvoke();
                });
            }
        }

        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        private void SpawnRewardInternal(CurrencyItem reward, Vector3 position, Action callback)
        {
            // Spawn a bunch of particles
            int particleAmount = Math.Min(reward.m_value, k_maxParticles);
            if(reward.m_value > k_megaThreshold)
            {
                particleAmount = k_megaParticles;
            }

            TopPanelHolder holder = GetHolder(reward.m_currencyID);
            SpawnRewardInternal(particleAmount, reward, position, holder, callback);
        }

        /// @param amount
        ///     The amount of particles to spawn
        /// @param reward
        ///     The reward to spawn
        /// @param position
        ///     The position to spawn at
        /// @param holder
        ///     The holder to interact with
        /// @param callback
        ///     The function to call when the ceremony is done
        ///
        private void SpawnRewardInternal(int amount, CurrencyItem reward, Vector3 position, TopPanelHolder holder, Action callback)
        {
            // Show the holder if needed
            holder.Show();

            // Spawn a bunch of particles
            int particleFinished = 0;
            for (int i = 0; i < amount; ++i)
            {
                ++holder.m_activeParticles;
                var particleReward = ParticleUtils.SpawnRewardParticles(reward.m_currencyID, transform.parent, position, holder.transform.position);
                particleReward.OnReachedDestination += () =>
                {
                    // This is called when the particle hits the holder
                    m_currencySequence.Stop(true);
                    m_currencySequence = DOTween.Sequence();

                    // Show the flash
                    holder.m_flash.color = Color.white;

                    // Trigger the sequence
                    m_currencySequence.Append(holder.transform.DOPunchPosition(Vector3.up * Screen.height * 0.025f, 0.5f));
                    m_currencySequence.Insert(0.0f, holder.m_flash.DOColor(GameUtils.k_transparent, 0.5f));
                    m_currencySequence.OnComplete(() =>
                    {
                        --holder.m_activeParticles;

                        // Hide the holder if not needed anymore
                        holder.Hide();
                    });

                    PlayCollectSFX(reward.m_currencyID);
                    m_currencySequence.Play();
                };

                particleReward.OnRewarded += () =>
                {
                    // This is called even if the particle gets destroyed
                    ++particleFinished;
                    if (particleFinished == amount)
                    {
                        callback.SafeInvoke();
                    }
                };
            }

            // Text shown above
            ParticleUtils.SpawnTextParticles(string.Format(GameTextIdentifiers.k_rewardFormat, reward.m_value), transform.parent, position);
        }

        /// @param currencyID
        ///     The currency collected
        ///     
        private void PlayCollectSFX(string currencyID)
        {
            switch (currencyID)
            {
                case OverlayBankIdentifiers.k_currencyCredits:
                {
                    //m_audioService.PlaySFXRandom(AudioIdentifiers.k_randomXP);
                    break;
                }
                default:
                {
                    //m_audioService.PlaySFX(AudioIdentifiers.k_sfxCollectCurrency);
                    break;
                }
            }
        }

        /// @param currencyID
        ///     The currency to look for
        ///     
        /// @return The top panel for this currency
        /// 
        private TopPanelHolder GetHolder(string currencyID)
        {
            // Get from the static holders
            TopPanelHolder holder = GetHolder(m_holders, currencyID);
            if (holder == null)
            {
                // Get from the temporary holders
                holder = GetHolder(m_extraHolders, currencyID);
            }

            if (holder == null)
            {
                // Create extra holder
                var holderObject = ResourceUtils.LoadAndInstantiateGameObject(k_topHolderPath, m_hiddenPanelsHolder, currencyID);
                holder = holderObject.GetComponent<TopPanelHolder>();
                holder.SetCurrency(currencyID);
                holder.m_hidden = true;
                m_extraHolders.Add(holder);
            }
            return holder;
        }

        /// @param holders
        ///     The list of holders to look through
        /// @param currencyID
        ///     The currency to look for
        ///     
        /// @return The top panel for this currency
        /// 
        private TopPanelHolder GetHolder(List<TopPanelHolder> holders, string currencyID)
        {
            TopPanelHolder holder = null;
            foreach (var topHolder in holders)
            {
                if (topHolder.CurrencyID == currencyID)
                {
                    holder = topHolder;
                    break;
                }
            }
            return holder;
        }
        #endregion
    }
}
