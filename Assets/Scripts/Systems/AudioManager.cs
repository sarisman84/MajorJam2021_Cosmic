using System;
using System.Collections.Generic;
using UnityEngine;

namespace MajorJam.System
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton Impl

        public static AudioManager Manager { get; private set; }

        private void ImplementSingleton()
        {
            if (Manager)
            {
                Destroy(gameObject);
                return;
            }

            Manager = this;
        }

        #endregion


        public List<CustomAudioClip> audioClips;

        private void Awake()
        {
            ImplementSingleton();

            foreach (var clipInfo in audioClips)
            {
                clipInfo.Player = gameObject.AddComponent<AudioSource>();
                clipInfo.Player.clip = clipInfo.clip;
                clipInfo.Player.volume = clipInfo.volume;
                clipInfo.Player.pitch = clipInfo.pitch;
                clipInfo.Player.loop = clipInfo.loop;
                clipInfo.Player.playOnAwake = false;
            }
        }


        public void Play(string clipName)
        {
            if (audioClips.Find(c => c.clipId.Equals(clipName)) is { } foundClip)
            {
                foundClip.Player.Play();
                return;
            }

            Debug.LogWarning($"Could not find a clip with the id: {clipName}");
        }
    }


    [Serializable]
    public class CustomAudioClip
    {
        public string clipId;
        public AudioClip clip;
        [Range(0, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch;
        public bool loop;


        public AudioSource Player { get; set; }
    }
}