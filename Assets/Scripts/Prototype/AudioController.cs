using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Prototype
{
    public static class Audio
    {
        private static AudioController _instance; //WHATT??? A SINGLETON??? HUH?????... yea idc anymore
        internal static void SetInstance(AudioController controller) => _instance = controller;

        public static void PlaySfx(string audioId, float customPitch = float.MaxValue)
            => _instance.PlaySfx(audioId, customPitch);
        
        public static void PlayBgm(string audioId, bool overwritePlaying = false) => _instance.PlayBgm(audioId, overwritePlaying);

        public static void StopBgm() => _instance.StopBgm();

        public static void FadeBgm(float from, float to, float duration) => _instance.FadeBgm(from, to, duration);
        
        public static void SetSfxVolume(float volume) => _instance.SetSfxVolume(volume);
                                                                                    
        public static void SetBgmVolume(float volume) => _instance.SetBgmVolume(volume); 
    }
    
    public class AudioController : MonoBehaviour
    {

        [Serializable]
        private struct SfxData
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
            [Range(0f, 2f)] public float pitch;

            public bool randomizePitch;
            [Range(0f, 2f)] public float pitchMin, pitchMax;
        }
        
        [Serializable]
        private struct BgmData
        {
            public string id;
            public AudioClip clip;
            public bool oneTime;
            [Range(0f, 1f)] public float volume;
        }
        
        [SerializeField] private AudioSource sfxAudio;
        [SerializeField] private AudioSource bgmAudio;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private SfxData[] allSfx;
        [SerializeField] private BgmData[] allBgm;

        private Dictionary<string, SfxData> sfxDict = new();
        private Dictionary<string, BgmData> bgmDict = new();

        private string currentBgmId;

        private void Awake()
        {
            foreach (var sfx in allSfx)
                sfxDict[sfx.id] = sfx;
            
            foreach (var bgm in allBgm)
                bgmDict[bgm.id] = bgm;

            Audio.SetInstance(this);
        }

        public void PlaySfx(string audioId, float customPitch = float.MaxValue)
        {
            if (!sfxDict.TryGetValue(audioId, out var data))
            {
                Debug.LogWarning($"COULD NOT FIND AUDIO WITH ID {audioId}");
                return;
            }

            if (!customPitch.Equals(float.MaxValue)) sfxAudio.pitch = customPitch;
            else
            {
                sfxAudio.pitch = data.pitch;
                if (data.randomizePitch) sfxAudio.pitch = Random.Range(data.pitchMin, data.pitchMax);
            }

            sfxAudio.volume = data.volume;
            sfxAudio.PlayOneShot(data.clip);
        }

        public void PlayBgm(string audioId, bool overwritePlaying)
        {
            if (!bgmDict.TryGetValue(audioId, out var data))
            {
                Debug.LogWarning($"COULD NOT FIND AUDIO WITH ID {audioId}");
                return;
            }

            if (audioId.Equals(currentBgmId) && !overwritePlaying) return;

            currentBgmId = audioId;
            
            bgmAudio.Stop();

            bgmAudio.volume = data.volume;
            bgmAudio.clip = data.clip;
            bgmAudio.loop = !data.oneTime;
            bgmAudio.Play();
        }

        public void StopBgm() => bgmAudio.Stop();

        public void FadeBgm(float from, float to, float duration)
        {
            var volume = from;
            DOTween.To(() => volume, x =>
            {
                SetMixerValue("masterVolume", x);
            }, to, duration);
        }

        public void SetSfxVolume(float volume) => SetMixerValue("sfxVolume", volume);

        public void SetBgmVolume(float volume) => SetMixerValue("bgmVolume", volume);

        private void SetMixerValue(string param, float value)
        {
            var volume = Mathf.Clamp(value, 0.0001f, 1f);
            mixer.SetFloat(param, Mathf.Log10(volume) * 20);
        }
    }
}