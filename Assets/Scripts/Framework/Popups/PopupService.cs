//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreFramework
{
    /// This service handles popup requests
    /// 
    public sealed class PopupService : Service
    {
        private const string k_popupPrefabsPath = "Prefabs/Popups/";
        private const string k_popupCanvasName = "PopupCanvas";
        private const int k_popupCanvasSortOrder = 20;

        public static string InfoID = PopupIdentifiers.k_popupInfo;
        public static string QuestionID = PopupIdentifiers.k_popupQuestion;
        public static string PrefabPath = string.Empty;

        public Transform CanvasTransform { get { return m_popupCanvasObject?.transform; } }

        private GameObject m_popupCanvasObject = null;

        private Dictionary<string, GameObject> m_popupPrefabs = new Dictionary<string, GameObject>();

        private List<PopupView> m_popupQueue = new List<PopupView>();
        private List<PopupView> m_priorityPopupQueue = new List<PopupView>();
        private List<PopupView> m_toastsPopupQueue = new List<PopupView>();

        private bool m_enabled = true;

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            CreateCanvas();
        }
        #endregion

        #region Public functions
        /// @param enabled
        ///     Whether to enable or disable the popup service
        /// 
		public void SetEnabled(bool enabled)
        {
            m_enabled = enabled;
            UpdatePopupVisiblities();
        }

        /// @return Whether there are popups currently displayed
        /// 
        public bool HasPopupDisplayed()
        {
            return (m_popupQueue.Count > 0) || (m_priorityPopupQueue.Count > 0);
        }

        /// @return Whether there are toasts currently displayed
        /// 
        public bool HasToastDisplayed()
        {
            return (m_toastsPopupQueue.Count > 0);
        }

        /// @param popupID
        ///     The ID of the popup to register
        /// @param popupPrefab
        ///     The prefab to associate to the ID
        /// 
        public void RegisterPopupPrefab(string popupID, GameObject popupPrefab)
        {
            m_popupPrefabs.AddOrUpdate(popupID, popupPrefab);
        }

        /// @param popupID
        ///     popup ID to present
        /// @param [optional] layerable
        ///     If this popup allows other popups to be displayed on top of it. Defaults to true.
        /// 
        /// @return The created PopupView. Null if the prefab could not be loaded
        /// 
        public PopupView QueuePopup(string popupID, bool layerable = true)
        {
            PopupView popupView = CreatePopup(popupID);
            if (popupView != null)
            {
                popupView.m_layerable = layerable;
                popupView.Hide();

                m_popupQueue.Add(popupView);
                UpdatePopupVisiblities();
            }

            return popupView;
        }

        /// @param popupID
        ///     popup ID to present
        /// 
        /// @return The created PopupView. Null if the prefab could not be loaded
        /// 
        public PopupView QueuePriorityPopup(string popupID)
        {
            PopupView popupView = CreatePopup(popupID);
            if (popupView != null)
            {
                popupView.m_layerable = true;
                popupView.m_priority = true;
                popupView.Hide();

                m_priorityPopupQueue.Add(popupView);
                UpdatePopupVisiblities();
            }

            return popupView;
        }

        /// @param popupID
        ///     popup ID to present
        /// 
        /// @return The created PopupView. Null if the prefab could not be loaded
        /// 
        public PopupView QueueToastPopup(string popupID)
        {
            PopupView popupView = CreatePopup(popupID);
            if (popupView != null)
            {
                popupView.m_layerable = true;
                popupView.m_toast = true;
                popupView.Hide();

                m_toastsPopupQueue.Add(popupView);
                UpdatePopupVisiblities();
            }

            return popupView;
        }

        /// @param popupView
        ///     The PopupView to attempt to dismiss
        /// 
        public void DismissPopup(PopupView popupView)
        {
            if (popupView != null)
            {
                popupView.RequestDismiss();
            }

            if (m_popupQueue.Remove(popupView) == false)
            {
                m_priorityPopupQueue.Remove(popupView);
            }

            UpdatePopupVisiblities();
        }

        /// Dismisses all displayed popups
        /// 
        public void DismissDisplayedPopups()
        {
            for (int i = (m_popupQueue.Count - 1); i >= 0; --i)
            {
                if (m_popupQueue[i].gameObject.activeInHierarchy)
                {
                    DismissPopup(m_popupQueue[i]);
                }
            }

            // All priority popups should be visible
            for (int i = (m_priorityPopupQueue.Count - 1); i >= 0; --i)
            {
                DismissPopup(m_priorityPopupQueue[i]);
            }

            // Only the first toast should be visible
            if (m_toastsPopupQueue.Count > 0)
            {
                DismissPopup(m_toastsPopupQueue.GetFirst());
            }

            UpdatePopupVisiblities();
        }

        /// Used by PopupView::Dismiss to ensure the service has removed
        /// any references to this popup as it has been dismissed.
        /// Also updates popup visibilities
        /// 
        /// @param popupView
        ///     The PopupView to remove references to
        /// 
        public void RemovePopup(PopupView popupView)
        {
            var popupList = m_popupQueue;
            if(popupView.m_toast == true)
            {
                popupList = m_toastsPopupQueue;
            }
            else if(popupView.m_priority == true)
            {
                popupList = m_priorityPopupQueue;
            }

            if (popupList.Remove(popupView) == true)
            {
                UpdatePopupVisiblities();
            }
        }
        #endregion

        #region Private functions
        /// Creates the popup canvas
        /// 
        private void CreateCanvas()
        {
            if(PrefabPath != string.Empty)
            {
                m_popupCanvasObject = ResourceUtils.LoadAndInstantiateGameObject(PrefabPath);
            }

            if(m_popupCanvasObject == null)
            {
                m_popupCanvasObject = new GameObject(k_popupCanvasName);

                Canvas canvas = m_popupCanvasObject.AddComponent<Canvas>();
                canvas.sortingOrder = k_popupCanvasSortOrder;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.additionalShaderChannels =
                    AdditionalCanvasShaderChannels.Normal |
                    AdditionalCanvasShaderChannels.Tangent |
                    AdditionalCanvasShaderChannels.TexCoord1;

                var canvasScaler = m_popupCanvasObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1080, 1920);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0.5f;

                m_popupCanvasObject.AddComponent<GraphicRaycaster>();
            }

            m_popupCanvasObject.name = k_popupCanvasName;
            GameObject.DontDestroyOnLoad(m_popupCanvasObject);
        }

        /// @param popupID
        ///     popup ID to present
        /// 
        /// @return The PopupView of the created popup, or null
        /// 
        private PopupView CreatePopup(string popupID)
        {
            PopupView popupView = null;

            // Check if the popup is registered
            var popupPrefab = m_popupPrefabs.GetValueOrDefault(popupID, null);

            if(popupPrefab == null)
            {
                // Popup not registered, try to load from default location
                string popupPath = k_popupPrefabsPath + popupID;
                popupPrefab = Resources.Load<GameObject>(popupPath);
            }

            if (popupPrefab != null)
            {
                GameObject popupObject = GameObject.Instantiate(popupPrefab) as GameObject;
                popupView = popupObject.GetComponent<PopupView>();
                popupView.Initialise();
                popupView.transform.SetParent(m_popupCanvasObject.transform, false);

                if (popupView == null)
                {
                    Debug.LogError(string.Format("Failed to get PopupView component from object '{0}'", popupObject.name));
                }

                popupObject.name = popupID;
            }
            else
            {
                Debug.LogError(string.Format("Failed to load popup prefab '{0}'", popupID));
            }

            return popupView;
        }

        /// Updates the visibility of all popups
        /// 
        private void UpdatePopupVisiblities()
        {
            if (m_enabled == true)
            {
                if (m_popupQueue.Count > 0)
                {
                    bool displayed = false;
                    bool hidePopup = false;
                    foreach (var popup in m_popupQueue)
                    {
                        if (hidePopup == false)
                        {
                            if ((displayed == false) || (popup.m_layerable == true))
                            {
                                popup.RequestPresent();
                                displayed = true;
                            }
                            else
                            {
                                hidePopup = true;
                            }
                        }

                        if (hidePopup == true)
                        {
                            popup.Hide();
                        }
                        else
                        {
                            hidePopup = !popup.m_coverable;
                        }
                    }
                }

                if (m_priorityPopupQueue.Count > 0)
                {
                    foreach (var popup in m_priorityPopupQueue)
                    {
                        popup.RequestPresent();

                        // Only one priority popup can be seen at a time
                        break;
                    }
                }

                if (m_toastsPopupQueue.Count > 0)
                {
                    // Only one toast popup can be seen at a time
                    m_toastsPopupQueue.GetFirst().RequestPresent();
                }

                UpdatePopupOrders();
            }
            else
            {
                foreach (var popupView in m_popupQueue)
                {
                    popupView.Hide();
                }

                foreach (var popupView in m_priorityPopupQueue)
                {
                    popupView.Hide();
                }

                foreach (var popupView in m_toastsPopupQueue)
                {
                    popupView.Hide();
                }
            }
        }

        /// Ensures popups are in the correct view order on the canvas
        /// 
        private void UpdatePopupOrders()
        {
            for (int i = (m_toastsPopupQueue.Count - 1); i >= 0; --i)
            {
                m_toastsPopupQueue[i].transform.SetAsFirstSibling();
            }

            for (int i = (m_priorityPopupQueue.Count - 1); i >= 0; --i)
            {
                m_priorityPopupQueue[i].transform.SetAsFirstSibling();
            }

            for (int i = (m_popupQueue.Count - 1); i >= 0; --i)
            {
                m_popupQueue[i].transform.SetAsFirstSibling();
            }
        }
        #endregion
    }
}
