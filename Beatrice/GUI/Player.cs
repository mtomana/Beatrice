using Beatrice.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Beatrice.GUI
{
    public partial class Player : Form
    {
        public Player()
        {
            InitializeComponent();
            SlideManager.ActionSlideChanged += SlideManager_SlideChanged;
            SlideManager.ActionPlay += SlideManager_Play;
            SlideManager.ActionPause += SlideManager_Pause;
            SlideManager.ActionSkipForward += SlideManager_ActionSkipForward;
            SlideManager.ActionSkipBackward += SlideManager_ActionSkipBackward;
            SlideManager.ActionVolumeUp += SlideManager_ActionVolumeUp;
            SlideManager.ActionVolumeDown += SlideManager_ActionVolumeDown;
            axWindowsMediaPlayer.uiMode = "none";
        }

        private void SlideManager_ActionVolumeDown()
        {
            axWindowsMediaPlayer.settings.volume -= 10;
        }

        private void SlideManager_ActionVolumeUp()
        {
            axWindowsMediaPlayer.settings.volume += 10;
        }

        private void SlideManager_ActionSkipBackward()
        {
            axWindowsMediaPlayer.Ctlcontrols.currentPosition -= 10;
        }

        private void SlideManager_ActionSkipForward()
        {
            axWindowsMediaPlayer.Ctlcontrols.currentPosition += 10;
        }

        private void SlideManager_Pause()
        {
            axWindowsMediaPlayer.Ctlcontrols.pause();
            labelTitle.Visible = true;
        }

        private void SlideManager_Play()
        {
            axWindowsMediaPlayer.Ctlcontrols.play();
            labelTitle.Visible = false;
        }

        public WMPLib.WMPPlayState PlayState
        {
            get
            {
                return axWindowsMediaPlayer.playState;
            }
        }

        private void SlideManager_SlideChanged(Slide slide)
        {
            axWindowsMediaPlayer.Ctlcontrols.stop();
            axWindowsMediaPlayer.URL = slide.MoviePath;
            axWindowsMediaPlayer.Ctlcontrols.stop();
            labelTitle.Text = slide.Title;
            labelTitle.Visible = true;
        }

        private void Player_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.BeginInit();
            SlideManager.Init(this);

            foreach (Control ctrl in Controls)
            {
                ctrl.TabStop = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            var keyCode = keyData;

            switch (keyCode)
            {
                case Keys.Left:
                    SlideManager.SkipToPreviousSlide();
                    return true;
                case Keys.Right:
                    SlideManager.SkipToNextSlide();
                    return true;
                case Keys.Up:
                    SlideManager.VolumeUp();
                    return true;
                case Keys.Down:
                    SlideManager.VolumeDown();
                    return true;
                case Keys.Enter | Keys.Alt:
                    MinimizeMaximizeForm();
                    return true;
                case Keys.Control | Keys.Space:
                    SlideManager.SayTitle();
                    return true;
                case Keys.Space:
                    SlideManager.PlayPause();
                    return true;
                case Keys.Shift | Keys.Right:
                    SlideManager.SkipForward();
                    return true;
                case Keys.Shift | Keys.Left:
                    SlideManager.SkipBackward();
                    return true;
                default:
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }

        private void MinimizeMaximizeForm()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;

            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }
    }
}
