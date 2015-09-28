using Beatrice.GUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AxWMPLib;
using System.Threading;

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

        public static event Action ActionSetPositionToStart;

        public static event Action<string> ActionLog;

        public static event Action<Action> ActionExecInGUI;




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
            //todo: sound effect jeżeli to ostatni
            ClickSound();
            var currentOrder = CurrentSlide.Order;
            var nextScene = Slides.OrderBy(p => p.Order).FirstOrDefault(p => p.Order > currentOrder);
            if (nextScene != null)
            {
                LoadSlide(nextScene);
                return true;
            }
            return false;
        }

        internal static void OnPlayStateChanged(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            Log(string.Format("New state: {0} / {1}", e.newState, (WMPLib.WMPPlayState)e.newState));
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                if (CurrentSlide.Loop)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Task.Delay(1000).Wait();
                        ExecuteInGUIThread(() =>
                        {
                            ActionSetPositionToStart.Raise();
                            ActionPlay.Raise();
                        });
                    });


                }
                
            }
        }

        private static void ExecuteInGUIThread(Action a)
        {
            ActionExecInGUI.Raise(a);
        }

        private static void Log(string v)
        {
            ActionLog(v);
        }

        internal static void PlayPause()
        {
            ClickSound();
            if (playerForm.PlayState != WMPLib.WMPPlayState.wmppsPlaying)
            {
                if (playerForm.PlayState == WMPLib.WMPPlayState.wmppsMediaEnded)
                {
                    ActionSetPositionToStart.Raise();
                }
                ActionPlay.Raise();
            }
            else
            {
                ActionPause.Raise();
            }
        }

        public static bool SkipToPreviousSlide()
        {
            //todo: sound effect jeżeli to pierwszy
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
            ActionSkipBackward.Raise();
        }

        public static void SkipForward()
        {
            ClickSound();
            ActionSkipForward.Raise();
        }

        private static void LoadSlide(Slide slide)
        {
            CurrentSlide = slide;
            ActionSlideChanged.Raise(slide);
            SayTitle();
            if (CurrentSlide.AutoPlay)
            {
                ActionPlay.Raise();
            }
        }

        public static void SayTitle()
        {
            if (!string.IsNullOrEmpty(CurrentSlide.TitlePath))
            {
                SoundPlayer.SoundLocation = CurrentSlide.TitlePath;
                SoundPlayer.Load();
                SoundPlayer.Play();
            }
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
            //todo: zmienić na Func<bool> i zwrócić czy już koniec skali, wtedy inny dźwięk
            VolumeSound();
            ActionVolumeUp.Raise();
        }

        public static void VolumeDown()
        {
            //todo: zmienić na Func<bool> i zwrócić czy już koniec skali, wtedy inny dźwięk
            VolumeSound();
            ActionVolumeDown.Raise();

        }
    }
}
