using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

namespace Prototype.AudioCore
{
    public enum StreamGroup
    {
        FX,
        Voice,
        Music,
    }
    
    public static class AudioController
    {
        public static Action<bool> OnPause;
        
        internal static List<StreamGroup> singleStream = new List<StreamGroup> {
        StreamGroup.Voice,
        StreamGroup.Music,
    };
        
        internal static List<StreamGroup> musicLikeStreams = new List<StreamGroup>()
    {
        StreamGroup.Music,
    };
        
        internal static Dictionary<StreamGroup, float> groupVolume = new Dictionary<StreamGroup, float>()
    {
        { StreamGroup.FX, 0.8f },
        { StreamGroup.Voice, 1f },
        { StreamGroup.Music, 0.8f },
    };
        
        internal static Dictionary<StreamGroup, List<AudioSource>>
            streams = new Dictionary<StreamGroup, List<AudioSource>>(),
            userStreams = new Dictionary<StreamGroup, List<AudioSource>>(), 
            pausedStreams = new Dictionary<StreamGroup, List<AudioSource>>(); 
        
        private const string
            PAUSE = "||";
        
        internal static GameObject
            srcParrent;
        
        internal static Dictionary<StreamGroup, List<Sequence>>
            tweens = new Dictionary<StreamGroup, List<Sequence>>(),
            pausedTweens = new Dictionary<StreamGroup, List<Sequence>>();
        
        internal static List<AudioSource>
            lockedStreams = new List<AudioSource>();

       
        private static StreamGroup[]
            availableGroups = null;

        internal static StreamGroup[] AvailableGroups
        {
            get
            {
                if (availableGroups == null)
                {
                    var A = System.Enum.GetValues(typeof(StreamGroup));
                    availableGroups = new StreamGroup[A.Length];

                    for (var i = 0; i < A.Length; i++)
                    {
                        availableGroups[i] = (StreamGroup)A.GetValue(i);
                    }
                }
                return availableGroups;
            }
        }

        #region Methods to play sound by AudioClip.name (LanguageAudio is used to load sounds from Resources)
        public static void StopSound(string snd_name)
        {
            var container = GetStreams(snd_name);
            AudioSourceManagement.StopStreams(container);
        }
        
        public static void StopSound(string snd_name, StreamGroup group)
        {
            var container = GetStreams(snd_name, group);
            AudioSourceManagement.StopStreams(container);
        }
        
        public static bool IsSoundPlaying(string snd_name)
        {
            var lst = GetStreams(snd_name);
            return IsSoundPlaying(snd_name, lst);
        }
        
        public static bool IsSoundPlaying(string snd_name, StreamGroup group)
        {
            var lst = GetStreams(snd_name, group);
            return IsSoundPlaying(snd_name, lst);
        }
        
        private static bool IsSoundPlaying(string snd_name, List<AudioSource> lst)
        {
            var res = false;
            for (var i = 0; i < lst.Count; i++)
            {
                if (lst[i].IsPlaying())
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
        
        public static float PlaySound(string snd_name, StreamGroup group = StreamGroup.FX, float volume = 1, bool loop = false, float pitch = 1)
        {
            AudioSource src = null;
            return PlaySound(snd_name, ref src, group, volume, loop, pitch);
        }

        private static float PlaySound(string snd_name, ref AudioSource _src, StreamGroup group = StreamGroup.FX, float volume = -1, bool loop = false, float pitch = 1)
        {
            float res = 0;
            if (snd_name != "")
            {
                var clip = LanguageAudio.GetSoundByName(snd_name);
                res = PlaySound(clip, ref _src, group, volume, loop, pitch);
            }
            else
            {
                Debug.Log("Play sound called with null argument");
            }

            return res;
        }
        
        public static float PlaySound(string[] snd_name, float interval = 0, StreamGroup group = StreamGroup.FX, float volume = 1)
        {
            Sequence stack = null;
            AudioSource _src = null;
            return PlaySound(snd_name, ref stack, ref _src, interval, group, volume);
        }

        private static float PlaySound(string[] snd_name, ref Sequence _stack, ref AudioSource _src, float interval = 0, StreamGroup group = StreamGroup.FX, float volume = 1)
        {
            var sounds = new AudioClip[snd_name.Length];
            var stack = DOTween.Sequence();
            _stack = stack;
            var container = TweensManagement.GetTweenContainer(group);
            container.Add(stack);
            float t = 0;
            var src = AudioSourceManagement.GetStream(group);
            _src = src;
            AudioSourceManagement.LockStream(src, true);
            stack.OnComplete(() =>
          {
              container.Remove(stack);
              AudioSourceManagement.LockStream(src, false);
          });
            for (var i = 0; i < snd_name.Length; i++)
            {
                if (snd_name[i].Substring(0, 2).Equals(PAUSE))
                {
                    float delay;
                    if (float.TryParse(snd_name[i].Substring(2), out delay))
                    {
                        t += delay;
                    }
                }
                else
                {
                    sounds[i] = LanguageAudio.GetSoundByName(snd_name[i]);
                    var clip = sounds[i];
                    stack.InsertCallback(t, () =>
                  {
                      AudioSourceManagement.SetAndPlayStream(src, clip, group);
                  });
                    if (sounds[i] == null)
                    {
                        Debug.Log("Sound " + snd_name[i] + " is null");
                    }
                    else
                    {
                        t += sounds[i].length + interval;
                    }
                }
            }
            return Mathf.Clamp(t - interval, 0, float.MaxValue);
        }
        
        internal static void CrossFadeMusic(string snd_name, float duration = 0.5f, AudioSource src = null)
        {
            var group = StreamGroup.Music;
            var fade_src = src == null ? AudioSourceManagement.GetStream(group) : src;
            var clip = LanguageAudio.GetSoundByName(snd_name);
            var fader = DOTween.Sequence();
            var container = TweensManagement.GetTweenContainer(group);
            container.Add(fader);
            var vol = fade_src.volume;
            fader.Append(fade_src.DOFade(0, duration));
            fader.AppendCallback(() =>
          {
              fade_src.clip = clip;
              fade_src.Play();
          });
            fader.Append(fade_src.DOFade(vol, duration));
            fader.AppendCallback(() => container.Remove(fader));
        }
        
        public static float PlayMusic(string snd_name, float volume = -1)
        {
            return PlaySound(snd_name, StreamGroup.Music, volume, true);
        }
        
        public static float PlayVoice(string snd_name, float volume = -1, bool loop = false, float pitch = 1)
        {
            return PlaySound(snd_name, StreamGroup.Voice, volume, loop, pitch);
        }
        #endregion

        #region Methods to play sound as AudioClip
        public static float PlaySound(AudioClip clip, StreamGroup group = StreamGroup.FX, float volume = -1, bool loop = false, float pitch = 1)
        {
            AudioSource src = null;
            return PlaySound(clip, ref src, group, volume, loop, pitch);
        }

        private static float PlaySound(AudioClip clip, ref AudioSource _src, StreamGroup group = StreamGroup.FX, float volume = -1, bool loop = false, float pitch = 1)
        {
            float res = 0;

            if (clip != null)
            {
                _src = AudioSourceManagement.GetStream(group);
                res = AudioSourceManagement.SetAndPlayStream(_src, clip, group, volume, loop, pitch);
            }
            else
            {
                Debug.LogError("Не знайдено звука");
            }

            return res;
        }
        #endregion

        #region General methods
        internal static void InitStreams(GameObject src_parrent = null)
        {
            srcParrent = src_parrent;
            if (srcParrent == null)
            {
                srcParrent = new GameObject("AudioSources");
                GameObject.DontDestroyOnLoad(srcParrent);
            }

            foreach (var t in AvailableGroups)
            {
                AudioSourceManagement.GetStream(t);
            }
        }
        
        internal static List<T> GetContainer<T>(StreamGroup group, Dictionary<StreamGroup, List<T>> dic)
        {
            List<T> res = null;
            if (!dic.TryGetValue(group, out res))
            {
                res = new List<T>();
                dic.Add(group, res);
            }
            return res;
        }

        private static List<AudioSource> GetStreams(string snd_name)
        {
            var res = new List<AudioSource>();
            foreach (var t in AvailableGroups)
            {
                res.AddRange(GetStreams(snd_name, t));
            }
            return res;
        }

        private static List<AudioSource> GetStreams(string clip_name, StreamGroup group)
        {
            var res = new List<AudioSource>();
            res.AddRange(AudioSourceManagement.GetStreams(clip_name, StreamGroupManagement.GetStreamContainer((StreamGroup)group)));
            return res;
        }
        
        public static void Release(bool leave_music = false)
        {
            TweensManagement.ReleaseTweens();
            
            for (var i = 0; i < AvailableGroups.Length; i++)
            {
                var group = AvailableGroups[i];
                if (!leave_music || group != StreamGroup.Music)
                {
                    StreamGroupManagement.ReleaseSources(group);
                }
            }
            
            lockedStreams.Clear();
        }
        
        private static void EnableStreams(bool param, bool is_music)
        {
            for (var i = 0; i < AvailableGroups.Length; i++)
            {
                var group = AvailableGroups[i];
                if (musicLikeStreams.Contains(group) == is_music)
                {
                    AudioSourceManagement.EnableStreams(StreamGroupManagement.GetStreamContainer(group), param);
                }
            }
        }
        
        internal static void EnableSounds(bool param)
        {
            EnableStreams(param, false);
        }
        
        internal static void EnableMusic(bool param)
        {
            EnableStreams(param, true);
        }
        
        private static void SetStreamsVolume(float volume, bool is_music)
        {
            for (var i = 0; i < AvailableGroups.Length; i++)
            {
                var group = AvailableGroups[i];
                if (musicLikeStreams.Contains(group) == is_music)
                {
                    StreamGroupManagement.SetStreamsVolume(volume, group);
                }
            }
        }
        
        internal static void SetSoundsVolume(float volume)
        {
            SetStreamsVolume(volume, false);
        }
        
        internal static void SetMusicVolume(float volume)
        {
            SetStreamsVolume(volume, true);
        }
        
        public static void Pause(bool pause)
        {
            TweensManagement.PauseTweens(pause);

            foreach (var t in AvailableGroups)
            {
                StreamGroupManagement.PauseStreams(pause, t);
            }

            OnPause?.Invoke(pause);
        }
        
        public static bool IsPlaying(this AudioSource src)
        {
            return src.isPlaying || src.time != 0;
        }
        #endregion

        internal static bool IsLocked(this AudioSource src)
        {
            return lockedStreams.Contains(src);
        }

        internal static bool IsPaused(this AudioSource src, StreamGroup group)
        {
            return pausedStreams.ContainsKey(group) && AudioController.pausedStreams[group].Contains(src);
        }
    }
}