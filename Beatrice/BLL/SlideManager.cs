using Beatrice.GUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatrice.BLL
{
    public static class SlideManager
    {
        static Player playerForm;

        static System.Media.SoundPlayer SoundPlayer;
        static System.Media.SoundPlayer ClickSoundPlayer;
        static System.Media.SoundPlayer VolumeSoundPlayer;

        public static List<Slide> Slides { get; set; }

        public static Slide CurrentSlide { get; set; }

        public static event Action ActionVolumeDown;

        public static event Action ActionVolumeUp;

        public static event Action<Slide> ActionSlideChanged;

        public static event Action ActionPlay;

        public static event Action ActionPause;

        public static event Action ActionSkipBackward;

        public static event Action ActionSkipForward;



        public static void Init(Player player)
        {
            SoundPlayer = new System.Media.SoundPlayer();
            ClickSoundPlayer = new System.Media.SoundPlayer(Properties.Resources.clickDefault);
            ClickSoundPlayer.Load();

            VolumeSoundPlayer = new System.Media.SoundPlayer(Properties.Resources.clickVolume);
            VolumeSoundPlayer.Load();

            Slides = new List<Slide>();
            SlideManager.playerForm = player;
            playerForm.FormClosed += (s, e) =>
            {
                //    SlideManager.SaveScenes();
                SlideManager.Dispose();
                SoundPlayer.Dispose();
                ClickSoundPlayer.Dispose();
                VolumeSoundPlayer.Dispose();
            };
            LoadSlidesDefinitions();
        }

        private static void LoadSlidesDefinitions()
        {
            var path = Properties.Settings.Default.ConfigFileLocation;
            if (File.Exists(path))
            {
                using (StreamReader r2 = new StreamReader(path))
                {
                    string json = r2.ReadToEnd();
                    Slides.AddRange(JsonConvert.DeserializeObject<List<Slide>>(json));
                }
            }

            if (Slides.Any())
            {
                LoadSlide(Slides.OrderBy(p => p.Order).First());
            }
        }

        public static bool SkipToNextSlide()
        {
            ClickSound();
            var currentOrder = CurrentSlide.Order;
            var nextScene = Slides.OrderBy(p=>p.Order).FirstOrDefault(p => p.Order > currentOrder);
            if (nextScene != null)
            {
                LoadSlide(nextScene);
                return true;
            }
            return false;
        }

        internal static void PlayPause()
        {
            ClickSound();
            if (playerForm.PlayState != WMPLib.WMPPlayState.wmppsPlaying)
            {
                ActionPlay.Raise();
            }
            else
            {
                ActionPause.Raise();
            }
        }

        public static bool SkipToPreviousSlide()
        {
            ClickSound();
            var currentOrder = CurrentSlide.Order;
            var prevScene = Slides.OrderByDescending(p => p.Order).FirstOrDefault(p => p.Order < currentOrder);
            if (prevScene != null)
            {
                LoadSlide(prevScene);
                return true;
            }
            return false;
        }

        public static void SkipBackward()
        {
            ClickSound();
            //przerost formy nad treścią ;)
            ActionSkipBackward.Raise();
        }

        public static void SkipForward()
        {
            ClickSound();
            //przerost formy nad treścią ;)
            ActionSkipForward.Raise();
        }

        private static void LoadSlide(Slide slide)
        {
            CurrentSlide = slide;
            ActionSlideChanged.Raise(slide);
            SayTitle();
        }

        public static void SayTitle()
        {
            SoundPlayer.SoundLocation = CurrentSlide.TitlePath;
            SoundPlayer.Load();
            SoundPlayer.Play();
        }

        public static void ClickSound()
        {
            ClickSoundPlayer.Play();
        }

        public static void VolumeSound()
        {
            VolumeSoundPlayer.Play();
        }

        public static void Dispose()
        {
            SoundPlayer.Dispose();
        }

        public static void VolumeUp()
        {
            VolumeSound();
            ActionVolumeUp.Raise();
        }

        public static void VolumeDown()
        {
            VolumeSound();
            ActionVolumeDown.Raise();

        }
    }
}
