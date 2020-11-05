//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;

namespace CoreFramework
{
    /// Component that triggers an Audio SFX from an animation
    ///
    public sealed class AudioTrigger : AnimationListener
    {
        private static AudioService m_audioService = null;

        private string m_music = string.Empty;
        private AudioSource m_loopingSFX = null;

        #region GameBehaviour functions
        /// Destroy function
        /// 
        private void OnDestroy()
        {
            StopLoopingSFX();
            ResumeMusic();
        }

        /// The Initialise function
        ///
        protected override void Initialise()
        {
            m_audioService = GlobalDirector.Service<AudioService>();
        }
        #endregion

        #region Public functions
        /// @param sfxID
        ///     The sfx to play
        ///
        public void PlayMusic(string musicID)
        {
            if (m_audioService != null && string.IsNullOrEmpty(musicID) == false)
            {
                if (m_music == string.Empty)
                {
                    // Save the music that we interrupt
                    m_music = m_audioService.Music.clip.name;
                }
                m_audioService.PlayMusicFadeCross(musicID);
            }
        }

        /// Resumes the interrupted music
        ///
        public void ResumeMusic()
        {
            if (m_music != string.Empty)
            {
                m_audioService.PlayMusicFadeCross(m_music);
            }
        }

        /// @param sfxID
        ///     The sfx to play
        ///
        public void PlaySFX(string sfxID)
        {
            if (m_audioService != null && string.IsNullOrEmpty(sfxID) == false)
            {
                m_audioService.PlaySFX(sfxID);
            }
        }

        /// @param sfxID
        ///     The sfx to play
        /// @param variant
        ///     The variant to use for that SFX
        ///
        public void PlaySFXVariant(string sfxID, int variant)
        {
            PlaySFX(sfxID + variant);
        }

        /// @param animationEvent
        ///     The animation event
        ///
        public void PlaySFXWithVariant(AnimationEvent animationEvent)
        {
            PlaySFXVariant(animationEvent.stringParameter, animationEvent.intParameter);
        }

        /// @param sfxID
        ///     The sfx to play
        ///
        public void PlayRandomSFX(string sfxID)
        {
            if (m_audioService != null && string.IsNullOrEmpty(sfxID) == false)
            {
                m_audioService.PlaySFXRandom(sfxID);
            }
        }

        /// @param sfxID
        ///     The sfx to play
        ///
        public void PlayLoopingSFX(string sfxID)
        {
            if (m_audioService != null)
            {
                StopLoopingSFX();
                m_loopingSFX = m_audioService.PlaySFXLooped(sfxID);
            }
        }

        /// Stops the current looping SFX
        /// 
        public void StopLoopingSFX()
        {
            if (m_loopingSFX != null)
            {
                m_loopingSFX.Stop();
                m_loopingSFX = null;
            }
        }
        #endregion
    }
}
