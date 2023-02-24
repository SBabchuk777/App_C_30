using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Prototype.AudioCore
{
    public class LanguageAudio : MonoBehaviour
    {
        private static Dictionary<string, AudioClip> nonlanguageSounds;
        private static Dictionary<string, AudioClip> languageSounds;

        private static readonly bool debug = false;

        private static string multilanguagePath = "Sounds/MultiLanguage/";
        private static string nonlanguagePath = "Sounds/NonLanguage/";
        
        internal static void Init()
        {
            nonlanguageSounds = new Dictionary<string, AudioClip>();
            languageSounds = new Dictionary<string, AudioClip>();
            LoadSounds(false, false);
            LoadSounds(true, false);
        }

        private static void ReleaseLanguageSounds()
        {
            languageSounds.Clear();
        }
        
        internal static AudioClip GetSoundByName(string name, bool multilanguage = true)
        {
            AudioClip sound = null;

            name = name.ToLower();

            if (languageSounds != null && languageSounds.ContainsKey(name))
            {
                sound = languageSounds[name];
            }
            else if (nonlanguageSounds != null && nonlanguageSounds.ContainsKey(name))
            {
                sound = nonlanguageSounds[name];
            }
            return sound;
        }
        
        private static string GetPath(string root, string name, string folder)
        {
            var path = root;
            if (folder != "")
            {
                path += folder + "/";
            }
            path += name;
            return path;
        }
        
        internal static void LoadSounds(bool multilanguage = true, bool virt = true)
        {
            if (virt)
            {
                DOVirtual.DelayedCall(0, () => LoadSounds(multilanguage, false));
                return;
            }
            if (multilanguage)
            {
                ReleaseLanguageSounds();
                var lang = GetLanguageName();
                var path = multilanguagePath + lang + "/";
                var res = Resources.LoadAll(path);
                for (var i = 0; i < res.Length; i++)
                {
                    var sound = res[i] as AudioClip;
                    StoreSound(languageSounds, res[i].name, sound);
                    if (debug && sound == null)
                    {
                        print(path + "\t\t" + lang + "\t\t" + (sound));
                    }
                }
            }
            else
            {
                var res = Resources.LoadAll(nonlanguagePath);
                for (var i = 0; i < res.Length; i++)
                {
                    var sound = res[i] as AudioClip;
                    StoreSound(nonlanguageSounds, res[i].name, sound);
                    if (debug && sound == null)
                    {
                        print(nonlanguagePath + "\t\t" + (sound));
                    }
                }
            }
        }
        
        internal static void LoadSoundByName(string name, bool multilanguage = true, string folder = "")
        {
            if (multilanguage)
            {
                var lang = GetLanguageName();
                var path = GetPath(multilanguagePath + lang + "/", name, folder);
                var sound = Resources.Load(path) as AudioClip;
                StoreSound(languageSounds, name, sound);

                if (debug && sound == null)
                {
                    print(path + "\t\t" + lang + "\t\t" + (sound));
                }
            }
            else
            {
                var path = GetPath(nonlanguagePath, name, folder);
                var sound = Resources.Load(path) as AudioClip;
                StoreSound(nonlanguageSounds, name, sound);

                if (debug && sound == null)
                {
                    print(name + "\t\t" + (sound != null));
                }
            }
        }
      
        private static void StoreSound(Dictionary<string, AudioClip> dic, string name, AudioClip sound)
        {
            dic.Add(name.ToLower(), sound);
        }

        private static string GetLanguageName()
        {
            return PlayerPrefs.GetString("CurrentLanguageString", SystemLanguage.English.ToString());
        }
    }
}
