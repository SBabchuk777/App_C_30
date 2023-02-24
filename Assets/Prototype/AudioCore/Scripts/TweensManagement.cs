using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace Prototype.AudioCore
{
    public class TweensManagement : MonoBehaviour
    {
        internal static List<Sequence> GetTweenContainer(StreamGroup group, bool paused = false)
        {
            return AudioController.GetContainer(group, paused ? AudioController.pausedTweens : AudioController.tweens);
        }
        
        internal static void ReleaseTweens()
        {
            foreach (var t in AudioController.AvailableGroups)
            {
                ReleaseTweens(t);
            }
        }

        private static void ReleaseTweens(StreamGroup group)
        {
            var container = GetTweenContainer(group);
            foreach (var t in container)
            {
                t.Kill(true);
            }
            container.Clear();
        }
        
        internal static void PauseTweens(bool pause)
        {
            foreach (var t in AudioController.AvailableGroups)
            {
                PauseTweens(pause, t);
            }
        }

        private static void PauseTweens(bool pause, StreamGroup group)
        {
            var container = GetTweenContainer(group, true);

            if (pause)
            {
                container.AddRange(GetPlayingTweens(group).ToArray());
            }
            foreach (var t in container)
            {
                if (pause)
                {
                    t.Pause();
                }
                else
                {
                    t.Play();
                }
            }
            if (!pause)
            {
                container.Clear();
            }
        }
        
        private static List<Sequence> GetPlayingTweens(StreamGroup group)
        {
            var container = GetTweenContainer(group);
            return container.Where(t => t.IsPlaying()).ToList();
        }
    }
}