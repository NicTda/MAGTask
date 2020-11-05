//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using UnityEngine;

namespace MAGTask
{
    /// Utility class for spawning particles
    /// 
	public static class ParticleUtils
	{
        #region ISerializable functions
        /// @param text
        ///     The text to spawn
        /// @param parent
        ///     The parent to attach the particles to
        /// 
        /// @return The spawned particle text component
        ///
        public static ParticleTextView SpawnTextParticles(string text, Transform parent)
        {
            var particleObject = ResourceUtils.LoadAndInstantiateGameObject(ParticleIdentifiers.k_textReward, parent);
            var particleText = particleObject.GetComponent<ParticleTextView>();
            particleText.SafeText(text);
            return particleText;
        }

        /// @param text
        ///     The text to spawn
        /// @param position
        ///     The position to set
        /// 
        /// @return The spawned particle text component
        ///
        public static ParticleTextView SpawnTextParticles(string text, Vector3 position)
        {
            var particleObject = ResourceUtils.LoadAndInstantiateGameObject(ParticleIdentifiers.k_textReward);
            particleObject.transform.position = position;
            var particleText = particleObject.GetComponent<ParticleTextView>();
            particleText.SafeText(text);
            return particleText;
        }

        /// @param text
        ///     The text to spawn
        /// @param parent
        ///     The parent to attach the particles to
        /// @param position
        ///     The position to set
        /// 
        /// @return The spawned particle text component
        ///
        public static ParticleTextView SpawnTextParticles(string text, Transform parent, Vector3 position)
        {
            var particleText = SpawnTextParticles(text, parent);
            particleText.transform.position = position;
            return particleText;
        }

        /// @param particleID
        ///     The particle ID to spawn
        /// 
        /// @return The spawned particle object
        ///
        public static GameObject SpawnParticles(string particleID)
        {
            var particleObject = ResourceUtils.LoadAndInstantiateGameObject(particleID);
            return particleObject;
        }

        /// @param particleID
        ///     The particle ID to spawn
        /// @param parent
        ///     The parent to attach the particles to
        /// 
        /// @return The spawned particle object
        ///
        public static GameObject SpawnParticles(string particleID, Transform parent)
        {
            var particleObject = ResourceUtils.LoadAndInstantiateGameObject(particleID, parent);
            return particleObject;
        }

        /// @param particleID
        ///     The particle ID to spawn
        /// @param position
        ///     The position to set
        /// 
        /// @return The spawned particle object
        ///
        public static GameObject SpawnParticles(string particleID, Vector3 position)
        {
            var particleObject = SpawnParticles(particleID);
            particleObject.transform.position = position;
            return particleObject;
        }

        /// @param particleID
        ///     The particle ID to spawn
        /// @param parent
        ///     The parent to attach the particles to
        /// @param position
        ///     The position to set
        /// 
        /// @return The spawned particle object
        ///
        public static GameObject SpawnParticles(string particleID, Transform parent, Vector3 position)
        {
            var particleObject = SpawnParticles(particleID, parent);
            particleObject.transform.position = position;
            return particleObject;
        }

        /// @param currencyID
        ///     The currency to display
        /// @param parent
        ///     The parent to attach the particles to
        /// @param position
        ///     The position to set
        /// @param destination
        ///     The destination to set
        /// 
        /// @return The spawned particle reward
        ///
        public static ParticleReward SpawnRewardParticles(string currencyID, Transform parent, Vector3 position, Vector3 destination, float timeScale = 1.0f)
        {
            var particleObject = SpawnParticles(ParticleIdentifiers.k_currencyReward, parent, position);
            var particleReward = particleObject.GetComponent<ParticleReward>();
            particleReward.SetTimeScale(timeScale);
            particleReward.Initialise(currencyID, position, destination);
            return particleReward;
        }
        #endregion
    }
}
