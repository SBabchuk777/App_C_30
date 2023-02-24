using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.AudioCore
{

    public class StreamGroupManagement : MonoBehaviour
    {
        internal static bool IsGroupEnabled(StreamGroup group)
        {
            bool res;
            if (AudioController.musicLikeStreams.Contains(group))
            {
                res = AudioSettings.IsMusicEnabled();
            }
            else
            {
                res = AudioSettings.IsSoundsEnabled();
            }
            return res;
        }
        
        internal static float GetStreamsVolume(StreamGroup group, float volume_override = -1)
        {
            float res;
            if (AudioController.musicLikeStreams.Contains(group))
            {
                res = AudioSettings.GetMusicVol();
            }
            else
            {
                res = AudioSettings.GetSoundsVol();
            }

            if (volume_override >= 0 || AudioController.groupVolume.TryGetValue(group, out volume_override))
            {
                res *= volume_override;
            }
            return res;
        }
        
        private static void GetPlayingSources(StreamGroup group, ref List<AudioSource> receiver)
        {
            AudioSourceManagement.GetPlayingStreams(GetStreamContainer(group), ref receiver);
        }

        internal static List<AudioSource> GetStreamContainer(StreamGroup group, bool paused = false)
        {
            return AudioController.GetContainer(group, paused ? AudioController.pausedStreams : AudioController.streams);
        }
        
        internal static float GetSoundsEndTime(StreamGroup group)
        {
            float res = 0;
            var container = GetStreamContainer(group);
            if (container != null && container.Count > 0)
            {
                res = container.FindAll(x => x.clip != null && x.IsPlaying() && !x.loop).Max(x => x.clip.length - x.time);
                Mathf.Clamp(res, 0, float.MaxValue);
            }
            return res;
        }
        
        internal static void ReleaseSources(StreamGroup group)
        {
            ReleaseOwnSources(group);
        }
        
        internal static void SetGroupVolume(float volume, StreamGroup group)
        {
            if (!AudioController.groupVolume.ContainsKey(group))
            {
                AudioController.groupVolume.Add(group, volume);
            }
            else
            {
                AudioController.groupVolume[group] = volume;
            }
            SetStreamsVolume(volume, group);
        }
        
        internal static void SetStreamsVolume(float volume, StreamGroup group)
        {
            volume = GetStreamsVolume(group, volume);
            AudioSourceManagement.SetVolume(GetStreamContainer(group), volume);
        }
        
        internal static void PauseStreams(bool pause, StreamGroup group)
        {
            var container = GetStreamContainer(group, true);

            if (pause)
            {
                GetPlayingSources(group, ref container);
            }
            AudioSourceManagement.PauseStreams(container, pause);

            if (!pause)
            {
                container.Clear();
            }
        }
        
        private static void ReleaseOwnSources(StreamGroup group)
        {
            var container = GetStreamContainer(group);
            foreach (var t in container)
            {
                AudioSourceManagement.StopStream(t);
            }
        }
    }
}