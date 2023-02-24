using UnityEngine;

namespace Prototype.AudioCore
{
    public static class AudioSettings
    {
        private static bool actual;

        private static bool musicON = true;
        private static bool soundsON = true;
        private static float musicVol = 1;
        private static float soundsVol = 1;
        private static int currentLang = -1;
        
        public static void UpdateSettings()
        {
            if (actual) return;
            
            musicON = PlayerPrefs.GetInt("music_on", 1) != 0;
            soundsON = PlayerPrefs.GetInt("sounds_on", 1) != 0;
            musicVol = PlayerPrefs.GetFloat("music_vol", 1);
            soundsVol = PlayerPrefs.GetFloat("sounds_vol", 1);
            currentLang = PlayerPrefs.GetInt("CurrentLanguage", -1);
            actual = true;
        }
        
        public static bool IsMusicEnabled()
        {
            return musicON;
        }
        
        public static void EnableMusic(bool param)
        {
            musicON = param;
            AudioController.EnableMusic(musicON);
            PlayerPrefs.SetInt("music_on", (musicON == false ? 0 : 1));
            SaveSettings();
        }
        
        public static bool IsSoundsEnabled()
        {
            return soundsON;
        }
        
        public static void EnableSounds(bool param)
        {
            soundsON = param;
            AudioController.EnableSounds(soundsON);
            PlayerPrefs.SetInt("sounds_on", (soundsON == false ? 0 : 1));
            SaveSettings();
        }
        
        public static float GetMusicVol()
        {
            return musicVol;
        }
        
        public static void SetMusicVol(float param)
        {
            musicVol = param;
            AudioController.SetMusicVolume(musicVol);
            PlayerPrefs.SetFloat("music_vol", musicVol);
            SaveSettings();
        }
        
        public static float GetSoundsVol()
        {
            return soundsVol;
        }
        
        public static void SetSoundsVol(float param)
        {
            soundsVol = param;
            AudioController.SetSoundsVolume(soundsVol);
            PlayerPrefs.SetFloat("sounds_vol", soundsVol);
            SaveSettings();
        }
        
        internal static int GetCurrentLang()
        {
            return currentLang;
        }
        
        internal static void SetCurrentLang(int lang)
        {
            currentLang = lang;
            PlayerPrefs.SetInt("CurrentLanguage", currentLang);
            SaveSettings();
        }

        private static void SaveSettings()
        {
            PlayerPrefs.Save();
        }
    }
}
