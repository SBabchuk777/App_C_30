using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.AudioCore
{

    public class AudioSourceManagement : MonoBehaviour
    {
        internal static AudioSource GetStream(StreamGroup group)
        {
            AudioSource res = null;
            var container = StreamGroupManagement.GetStreamContainer(group);
            var single = AudioController.singleStream.Contains(group);
            for (var i = 0; container != null && i < container.Count; i++)
            {
                var src = container[i];

                if (!src.IsLocked() && (single || (!src.IsPlaying() && !src.IsPaused(group))))
                {
                    res = src;
                    break;
                }
            }
            if (container != null && res == null && (!single || container.Count == 0))
            {
                res = CreateStream();
                container.Add(res);
            }
            if (res == null)
            {
                Debug.LogError("Couldn't play sound on group " + group + " (is_single = " + single + "), but streams were locked or paused");
            }
            return res;
        }
        
        internal static List<AudioSource> GetStreams(string snd_name, List<AudioSource> container)
        {
            return container.FindAll(X =>
            {
                AudioClip clip;
                return (clip = X.clip) != null && clip.name == snd_name;
            });
        }

        internal static float SetAndPlayStream(AudioSource src, AudioClip clip, StreamGroup group, float volume = -1, bool loop = false, float pitch = 1)
        {
            float res = 0;
            if (src != null)
            {
                if (clip != null)
                {
                    src.clip = clip;
                    res = clip.length;
                    src.loop = loop;
                    src.mute = !StreamGroupManagement.IsGroupEnabled(group);
                    src.volume = StreamGroupManagement.GetStreamsVolume(group, volume);
                    src.pitch = pitch;
                    src.Play();
                }
            }
            return res;
        }
        
        internal static void GetPlayingStreams(List<AudioSource> container, ref List<AudioSource> receiver)
        {
            for (var i = 0; container != null && i < container.Count; i++)
            {
                if (container[i] != null)
                {
                    if (container[i].IsPlaying())
                    {
                        receiver.Add(container[i]);
                    }
                }
            }
        }
        
        internal static void LockStream(AudioSource src, bool _lock)
        {
            if (_lock)
            {
                if (!AudioController.lockedStreams.Contains(src))
                {
                    AudioController.lockedStreams.Add(src);
                }
            }
            else
            {
                AudioController.lockedStreams.Remove(src);
            }
        }
        
        internal static void PauseStreams(List<AudioSource> container, bool pause)
        {
            foreach (var source in container)
            {
                PauseStream(source, pause);
            }
            if (!pause)
            {
                container.Clear();
            }
        }
        
        internal static void SetVolume(List<AudioSource> container, float volume)
        {
            for (var i = 0; container != null && i < container.Count; i++)
            {
                container[i].volume = volume;
            }
        }
        
        internal static void EnableStreams(List<AudioSource> container, bool param)
        {
            foreach (var src in container.Where(src => src != null))
            {
                src.mute = !param;
            }
        }
        
        private static AudioSource CreateStream()
        {
            if (AudioController.srcParrent == null)
            {
                AudioController.InitStreams();
            }
            var src = AudioController.srcParrent.AddComponent<AudioSource>();
            src.playOnAwake = false;
            return src;
        }

        private static void PauseStream(AudioSource src, bool pause)
        {
            if (src == null) 
                return;
            
            if (pause)
            {
                src.Pause();
            }
            else
            {
                src.Play();
            }
        }
        
        internal static void StopStreams(List<AudioSource> container)
        {
            foreach (var t in container)
            {
                StopStream(t);
            }
        }
        
        internal static void StopStream(AudioSource src)
        {
            if (src == null) 
                return;
            
            src.Stop();
            src.clip = null;
        }
    }
}