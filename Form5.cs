using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace MineSweeperPersonnel
{
    public partial class Form5 : Form
    {
        private int storySayac = 3;
        static string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infantry Drum Cadence.mp3");
        WindowsMediaPlayer backmusic = new WindowsMediaPlayer
        {
            URL = musicPath,
            settings = { volume = 50 }
        };
        public Form5()
        {
            InitializeComponent();
            backmusic.settings.setMode("loop",true);
            backmusic.controls.play();
        }

        private void Form5_MouseDown(object sender, MouseEventArgs e)
        {
            if(storySayac == 3)
            {
                BackgroundImage = Properties.Resources.STARTINGMISSION;
                storySayac--;
            }
            else if(storySayac == 2)
            {
                BackgroundImage= Properties.Resources.CONTROLLER1;
                storySayac--;
            }
            else if(storySayac == 1)
            {
                BackgroundImage = Properties.Resources.CONTROLLER2;
                storySayac--;
            }
            else
            {
                backmusic.controls.stop();
                Dispose();
            }

        }
    }
}
