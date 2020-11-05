//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using System.Collections;
using UnityEngine;

namespace CoreFramework
{
    /// View for a scene transition UI
    /// 
    public abstract class SceneTransitionView : MonoBehaviour
    {
        #region Public functions
        /// Coroutine to start 'hiding' this UI
        /// 
        public IEnumerator Hide()
        {
            if(gameObject.activeInHierarchy == true)
            {
                yield return StartCoroutine(Hiding());
            }

            Destroy(gameObject);
        }

        /// Coroutine to handle the 'hiding' sequence of this transition UI
        /// 
        public abstract IEnumerator Hiding();

        /// Coroutine to start 'showing' this UI
        /// 
        public IEnumerator Show()
        {
            if(gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);

                yield return StartCoroutine(Showing());
            }
        }

        /// Coroutine to handle the 'showing' sequence of this transition UI
        /// 
        public abstract IEnumerator Showing();
        #endregion      
    }
}
