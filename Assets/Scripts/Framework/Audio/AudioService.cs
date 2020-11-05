//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework.Json;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreFramework
{
    /// Service that manages the audio
    /// 
	public sealed class AudioService : Service, ISavable
    {
        private const string k_audioSourceName = "AudioHolder";
        private const string k_audioPathFormat = "Audio/{0}/{1}";
        private const int k_sfxListSize = 10;
        private const int k_loopListSize = 3;

        private const string k_keyMusicMuted = "MusicMuted";
        private const string k_keySFXMuted = "SFXMuted";
        private const int k_unmuted = 0;
        private const int k_muted = 1;

        public bool m_loaded { get; set; } = false;

        public bool MusicMuted { get { return m_musicMuted; } }
        public bool SFXMuted { get { return m_sfxMuted; } }
        public AudioSource Music { get { return m_music; } }

        private GameObject m_audioSourceHolder = null;
        private AudioSource m_music = null;
        private AudioSource m_crossMusic = null;
        private TimeService m_timeService = null;

        private List<AudioSource> m_sfxList = new List<AudioSource>(k_sfxListSize);
        private int m_nextSFXIndex = 0;

        private List<AudioSource> m_loopingList = new List<AudioSource>(k_loopListSize);
        private int m_nextLoopingIndex = 0;

        private Dictionary<string, List<string>> m_sfxRegistry = new Dictionary<string, List<string>>(k_sfxListSize);

        private Tweener m_fadeTween = null;
        private bool m_musicMuted = false;
        private bool m_sfxMuted = false;
        private float m_pitch = 1.0f;
        private float m_musicVolume = 1.0f;

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
            m_timeService = GlobalDirector.Service<TimeService>();
            m_pitch = m_timeService.GetCurrentTimeScale();
            m_timeService.OnTimeScaleChanged += OnTimeScaleChanged;

            // Create a game object to hold audio sources
            m_audioSourceHolder = new GameObject(k_audioSourceName);
            GameObject.DontDestroyOnLoad(m_audioSourceHolder);

            // Load settings if there
            m_musicMuted = PlayerPrefs.GetInt(k_keyMusicMuted, k_unmuted) != k_unmuted;
            m_sfxMuted = PlayerPrefs.GetInt(k_keySFXMuted, k_unmuted) != k_unmuted;

            // Create a music source for the music
            CreateMusicSource();

            // Create audio sources for the sfx
            CreateSFXSources(k_sfxListSize);
            CreateLoopingSources(k_loopListSize);

            this.RegisterCaching();
        }

        /// Clears the local data
        ///     
        public void Clear()
        {
        }

        /// Called when the application is paused
        /// 
        public override void OnApplicationPause()
        {
            PauseMusic();
            StopAllSFX();
        }

        /// Called when the application is resumed
        /// 
        public override void OnApplicationResume()
        {
            if (m_musicMuted == false)
            {
                ResumeMusic();
            }
        }
        #endregion

        #region ISavable functions
        /// @return The serialized data
        /// 
        public object Serialize()
        {
            var jsonData = new JsonDictionary();
            jsonData[k_keyMusicMuted] = m_musicMuted;
            jsonData[k_keySFXMuted] = m_sfxMuted;
            return jsonData;
        }

        /// @param data
        /// 	The json data
        /// 	
        public void Deserialize(object data)
        {
            var jsonData = data.AsDictionary();
            ChangeMusicMute(jsonData.GetValueOrDefault(k_keyMusicMuted, false).AsBool());
            ChangeSFXMute(jsonData.GetValueOrDefault(k_keySFXMuted, false).AsBool());
            m_loaded = true;
        }
        #endregion

        #region Public functions
        /// @param musicID
        ///     The ID of the music to play
        /// @param [optional] looping
        ///     Whether the music is looping or not. True by default.
        /// 
        public void PlayMusic(string musicID, bool looping = true)
        {
            if (musicID != string.Empty)
            {
                if (IsMusicPlaying(musicID) == false)
                {
                    var clip = LoadClip(AudioIdentifiers.k_typeMusic, musicID);
                    if (clip != null)
                    {
                        m_music.clip = clip;
                        m_music.loop = looping;
                        m_music.Play();
                        if (m_musicMuted == true)
                        {
                            PauseMusic();
                        }
                    }
                }
                else if (m_musicMuted == false)
                {
                    ResumeMusic();
                }
            }
        }

        /// @param musicID
        ///     The ID of the music to play
        /// @param [optional] callback
        ///     The function to call once the fade is done
        /// 
        public void PlayMusicFade(string musicID, Action callback = null)
        {
            PlayMusic(musicID);
            m_music.volume = 0.0f;
            m_fadeTween = m_music.DOFade(m_musicVolume, 1.0f).OnComplete(() =>
            {
                callback.SafeInvoke();
            });
        }

        /// @param musicID
        ///     The ID of the music to play
        /// @param [optional] callback
        ///     The function to call once the fade is done
        /// 
        public void PlayMusicFadeCross(string musicID, Action callback = null)
        {
            if (m_musicMuted == false)
            {
                // Play the background track
                m_crossMusic.clip = m_music.clip;
                m_crossMusic.time = m_music.time;
                m_crossMusic.volume = m_music.volume;
                m_crossMusic.Play();
            }

            // Play the new track
            PlayMusic(musicID);
            m_music.time = (m_crossMusic.time).Mod(m_music.clip.length);
            m_music.volume = 0.0f;
            m_fadeTween = DOTween.To(() => m_music.volume, (value) =>
            {
                m_music.volume = value;
                m_crossMusic.volume = m_crossMusic.volume - value;

            }, m_musicVolume, 1.0f).OnComplete(() =>
            {
                m_crossMusic.Stop();
                callback.SafeInvoke();
            });
        }

        /// @param musicID
        ///     The ID of the music to check
        ///     
        /// @return Whether this music is currently playing
        /// 
        public bool IsMusicPlaying(string musicID)
        {
            return (m_music.clip != null && m_music.clip.name == musicID && m_music.isPlaying);
        }

        /// Pauses the music, if any is playing
        /// 
        public void PauseMusic()
        {
            m_music.Pause();
        }

        /// Resumes the music, if any is paused
        /// 
        public void ResumeMusic()
        {
            m_music.UnPause();
        }

        /// Stops the current playing music
        /// 
        public void StopMusic()
        {
            m_music.Stop();
            m_fadeTween.Stop();
        }

        /// @param [optional] callback
        ///     The function to call once the fade is done
        /// 
        public void StopMusicFade(Action callback = null)
        {
            m_fadeTween = m_music.DOFade(0.0f, 1.0f).OnComplete(() =>
            {
                StopMusic();
                callback.SafeInvoke();
            });
        }

        /// @param volume
        ///     The volume to apply to music
        ///     
        public void SetMusicVolume(float volume)
        {
            m_musicVolume = volume;
            m_music.volume = volume;
        }

        /// @param effectID
        ///     The ID of the sfx to play
        /// 
        public void PlaySFX(string effectID)
        {
            if (m_sfxMuted == false && effectID != string.Empty)
            {
                var clip = LoadClip(AudioIdentifiers.k_typeSFX, effectID);
                if (clip != null)
                {
                    var sfxSource = GetNextSFXSource();
                    sfxSource.Stop();
                    sfxSource.clip = clip;
                    sfxSource.Play();
                }
            }
        }

        /// @param effectID
        ///     The base ID of the sfx to play
        /// 
        public void PlaySFXRandom(string effectID)
        {
            if (m_sfxMuted == false && effectID != string.Empty)
            {
                var possibleSFX = GetPossibleSFX(effectID);
                if (possibleSFX.Count > 0)
                {
                    var sfxID = possibleSFX.GetRandom();
                    PlaySFX(sfxID);
                }
            }
        }

        /// @param effectID
        ///     The ID of the sfx to play
        /// 
        /// @return The audio clip so we can stop it
        /// 
        public AudioSource PlaySFXLooped(string effectID)
        {
            AudioSource loopingSource = null;
            if (m_sfxMuted == false && effectID != string.Empty)
            {
                var clip = LoadClip(AudioIdentifiers.k_typeSFX, effectID);
                if (clip != null)
                {
                    loopingSource = GetNextLoopingSource();
                    loopingSource.Stop();
                    loopingSource.loop = true;
                    loopingSource.clip = clip;
                    loopingSource.Play();
                }
            }
            return loopingSource;
        }

        /// Stops all playing SFX
        /// 
        public void StopAllSFX()
        {
            for (int i = 0; i < m_sfxList.Count; ++i)
            {
                m_sfxList[i].Stop();
            }

            for (int i = 0; i < m_loopingList.Count; ++i)
            {
                m_loopingList[i].Stop();
            }
        }

        /// @param muted
        ///     Whether to mute the music channel or not
        /// 
        public void SetMusicMuted(bool muted)
        {
            ChangeMusicMute(muted);
            this.Save();
        }

        /// @param muted
        ///     Whether to mute the sfx channel or not
        /// 
        public void SetSFXMuted(bool muted)
        {
            ChangeSFXMute(muted);
            this.Save();
        }

        /// @param pitch
        ///     The new pitch to apply
        ///     
        public void SetPitch(float pitch)
        {
            m_pitch = pitch;
            if (m_music != null)
            {
                m_music.pitch = m_pitch;
                m_crossMusic.pitch = m_pitch;
            }
            foreach (var sfx in m_sfxList)
            {
                sfx.pitch = m_pitch;
            }
            foreach (var loopingSfx in m_loopingList)
            {
                loopingSfx.pitch = m_pitch;
            }
        }
        #endregion

        #region Private functions
        /// Create the audio source for the music channel
        /// 
        private void CreateMusicSource()
        {
            if (m_music == null)
            {
                // Create an audio source for the music
                m_music = m_audioSourceHolder.AddComponent<AudioSource>();
                m_music.pitch = m_pitch;
                SetMusicVolume(m_musicVolume);

                // Create the background track for cross fade
                m_crossMusic = m_audioSourceHolder.AddComponent<AudioSource>();
                m_crossMusic.pitch = m_pitch;
                m_crossMusic.volume = 0.0f;
            }
        }

        /// @param listSize
        ///     The amount of audio source to have for the SFX channel
        ///     
        private void CreateSFXSources(int listSize)
        {
            for (int i = 0; i < listSize; ++i)
            {
                var newSFX = m_audioSourceHolder.AddComponent<AudioSource>();
                newSFX.pitch = m_pitch;
                m_sfxList.Add(newSFX);
            }
        }

        /// @param listSize
        ///     The amount of audio source to have for the SFX channel
        ///     
        private void CreateLoopingSources(int listSize)
        {
            for (int i = 0; i < listSize; ++i)
            {
                var newSFX = m_audioSourceHolder.AddComponent<AudioSource>();
                newSFX.pitch = m_pitch;
                m_loopingList.Add(newSFX);
            }
        }

        /// @return The next available audio source to play an SFX
        /// 
        private AudioSource GetNextSFXSource()
        {
            var source = m_sfxList[m_nextSFXIndex];

            ++m_nextSFXIndex;
            if (m_nextSFXIndex >= m_sfxList.Count)
            {
                m_nextSFXIndex = 0;
            }

            return source;
        }

        /// @return The next available audio source to play a looping SFX
        /// 
        private AudioSource GetNextLoopingSource()
        {
            var source = m_loopingList[m_nextLoopingIndex];

            ++m_nextLoopingIndex;
            if (m_nextLoopingIndex >= m_loopingList.Count)
            {
                m_nextLoopingIndex = 0;
            }

            return source;
        }

        /// @param type
        ///     The type of clip to load
        /// @param clipID
        ///     The ID of the clip to load
        ///     
        /// @return The loaded AudioClip
        /// 
        private AudioClip LoadClip(string type, string clipID)
        {
            return Resources.Load<AudioClip>(string.Format(k_audioPathFormat, type, clipID));
        }

        /// @param muted
        ///     Whether to mute the music channel or not
        /// 
        private void ChangeMusicMute(bool muted)
        {
            m_musicMuted = muted;
            if (m_musicMuted == true)
            {
                PlayerPrefs.SetInt(k_keyMusicMuted, k_muted);
                PauseMusic();
            }
            else
            {
                PlayerPrefs.SetInt(k_keyMusicMuted, k_unmuted);
                ResumeMusic();
            }
        }

        /// @param muted
        ///     Whether to mute the music channel or not
        /// 
        private void ChangeSFXMute(bool muted)
        {
            m_sfxMuted = muted;
            if (m_sfxMuted == true)
            {
                PlayerPrefs.SetInt(k_keySFXMuted, k_muted);
                StopAllSFX();
            }
            else
            {
                PlayerPrefs.SetInt(k_keySFXMuted, k_unmuted);
            }
        }

        /// @param timeScale
        ///     The new time scale
        ///     
        private void OnTimeScaleChanged(float timeScale)
        {
            SetPitch(timeScale);
        }

        /// @param baseSFXID
        ///     The effect to check
        ///     
        /// @return The list of possible sfx
        /// 
        private List<string> GetPossibleSFX(string baseSFXID)
        {
            List<string> possibleSFX = null;
            if (m_sfxRegistry.ContainsKey(baseSFXID) == true)
            {
                possibleSFX = m_sfxRegistry[baseSFXID];
            }
            else
            {
                possibleSFX = new List<string>();
                int index = 0;
                while (true)
                {
                    string clipID = baseSFXID + index++;
                    var clip = LoadClip(AudioIdentifiers.k_typeSFX, clipID);
                    if (clip != null)
                    {
                        possibleSFX.Add(clipID);
                    }
                    else
                    {
                        break;
                    }
                }

                // Cache the sfx list
                m_sfxRegistry.Add(baseSFXID, possibleSFX);
            }
            return possibleSFX;
        }
        #endregion
    }
}
