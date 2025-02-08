using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace MineSweeperPersonnel
{

    public partial class Form1 : Form
    {
        private GameGrid gameGrid;
        private Button exitbutton;
        public Button restartbutton;
        public static bool isleniyor;

        public Form1()
        {
            Form2 form = new Form2();  
            form.ShowDialog();
            InitializeComponent();
            Form5 story = new Form5();
            story.ShowDialog();
            gameGrid = new GameGrid(10, 60, this);

            restartbutton = new Button
            {
                Text = "Yeniden Başla",
                Size = new Size(120, 50),
                Location = new Point(650, 150),
                Font = new Font(Font.FontFamily, 10 ,FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TabStop = false
            };
            restartbutton.FlatAppearance.BorderSize = 1;
            restartbutton.Click += (s, e) => gameGrid.RestartGame();
            this.Controls.Add(restartbutton);

            exitbutton = new Button
            {
                Text = "Çıkış",
                Size = new Size(120, 50),
                Location = new Point(650, 210),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TabStop = false

            };
            exitbutton.FlatAppearance.BorderSize = 1;
            exitbutton.Click += (s, e) => gameGrid.ExitGame();
            this.Controls.Add(exitbutton);

            this.KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (!isleniyor)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        gameGrid.ShowMineCountsAroundSoldier("up");
                        break;
                    case Keys.S:
                        gameGrid.ShowMineCountsAroundSoldier("down");
                        break;
                    case Keys.A:
                        gameGrid.ShowMineCountsAroundSoldier("left");
                        break;
                    case Keys.D:
                        gameGrid.ShowMineCountsAroundSoldier("right");
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

    }
}
