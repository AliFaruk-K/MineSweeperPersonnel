using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using WMPLib;
using System.IO;
using System.Security.Policy;
using System.Runtime.CompilerServices;


namespace MineSweeperPersonnel
{
    public class GameGrid
    {
        public int GridSize;
        public Button[,] Buttons;
        private Button soldier; 
        private int soldierX = -1; 
        private int soldierY = -1;
        private int totalMines;
        private List<(int, int)> mineLocations;
        public Panel[,] littlePanel;
        public Label sayacLabel;
        public int sayac;
        private int anlikMayin = 0;
        private WindowsMediaPlayer backgroundPlayer;
        private WindowsMediaPlayer beepPlayer;
        private int mineCount = 25;
        public ProgressBar ilerleme;
        public Label ilerlemeLabel;
        public TrackBar volumeControl;
        public Label volumeCheckLabel;
        public Label guideLabel;

        public GameGrid(int gridSize, int buttonSize, Control container)
        {
            sayac = mineCount;
            GridSize = gridSize;
            totalMines = mineCount;
            mineLocations = new List<(int, int)>();
            Buttons = new Button[gridSize, gridSize];
            littlePanel = new Panel[3, 3];

            string backgroundMusic = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Age of War - Theme Soundtrack.wav");

            backgroundPlayer = new WindowsMediaPlayer
            {
                URL = backgroundMusic,
                settings = { volume = 70 },

            };
            backgroundPlayer.settings.setMode("loop", true);
            backgroundPlayer.controls.play();

            guideLabel = new Label
            {
                AutoSize = true,
                Text = "Kontroller:\n\nW,A,S,D = Askerin bulunduğu\n yerin basılan tuşa göre yönündeki\n 3 parçanın taranmasını sağlar.\nDedektör mayın sayısı kadar öter.\n\nSol Tık = Askerin hareketini sağlar.\n\nSağ Tık = Mayın olduğu düşünülen\nparçayı işaretlemek için kullanılır.",
                Location = new Point(610, 460)

            };
            container.Controls.Add(guideLabel); 

            sayacLabel = new Label
            {
                Width = 130,
                Height = 30,
                Location = new Point(630, 100),
                Text = $"Kalan Mayın İşareti: {sayac}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,

            };
            container.Controls.Add(sayacLabel);

            volumeCheckLabel = new Label
            {
                Width = 130,
                Height = 30,
                Location = new Point(650, 280),
                Text = "Ses Düzeyi: 70",
                Font = new Font("Arial", 10, FontStyle.Bold),

            };
            container.Controls.Add(volumeCheckLabel);

            ilerleme = new ProgressBar
            {
                Width = 130,
                Height = 30,
                Location = new Point(650, 50),
                Value = 0,
                Maximum = 75,

            };
            container.Controls.Add(ilerleme);

            ilerlemeLabel = new Label
            {
                Text = "Anlık İlerleme",
                Width = 130,
                Height = 30,
                Location = new Point(660, 30),
                AutoSize = true,
                Font = new Font("Arial", 10 , FontStyle.Bold),
            };

            container.Controls.Add(ilerlemeLabel);

            volumeControl = new TrackBar
            {
                SmallChange = 0,
                LargeChange = 0,
                Width = 130,
                Height = 30,
                Location = new Point(650,310),
                Value = 7
            };
            volumeControl.Scroll += VolumeControl_Scroll;
            container.Controls.Add(volumeControl);


            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    Panel panel = new Panel
                    {
                        Width = 30,
                        Height = 30,
                        Location = new Point(650 + (i * 35), 350 + (j * 35)),
                        BackColor = Color.SandyBrown,
                        Tag = (i, j),
                    };

                    if(i == 1 && j == 1) 
                    { 
                        panel.BackgroundImage = Properties.Resources.soldier; 
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        panel.BackColor = Color.SpringGreen; 
                    }

                    container.Controls.Add(panel);
                    littlePanel[i, j] = panel;
                }
            }

            for (int i = 0; i < gridSize; i++)
            {

                for (int j = 0; j < gridSize; j++)
                {
                    Button button = new Button
                    {
                        Width = buttonSize,
                        Height = buttonSize,
                        Location = new Point(j * (buttonSize+1) , i * (buttonSize+1)),
                        BackColor = Color.SandyBrown,
                        Tag = (i, j),
                        TabStop = false,
                        FlatStyle = FlatStyle.Flat
                    };
                    button.FlatAppearance.BorderSize = 0;
                    button.Click += Button_Click;
                    button.MouseDown += Panel_MouseDown;

                    container.Controls.Add(button);
                    Buttons[i, j] = button;
                }
            }

           
            soldier = new Button
            {
                Size = new Size(buttonSize, buttonSize),
                BackColor = Color.Transparent,
                BackgroundImage = Properties.Resources.soldier, 
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false 
            };

            container.Controls.Add(soldier);
        }
        private void PlayBeep()
        {
            string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "beep-07a.wav");

            beepPlayer = new WindowsMediaPlayer
            {
                URL = musicPath,
                settings = { volume = 100 }
            };
            beepPlayer.controls.play();
        }

        private void VolumeControl_Scroll(object sender, EventArgs e)
        {
            int volume = volumeControl.Value * 10;
            volumeCheckLabel.Text = $"Ses Düzeyi: {volume}";
            backgroundPlayer.settings.volume = volumeControl.Value * 10;
        }
        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (e.Button == MouseButtons.Right)
            {
                if(clickedButton.BackColor == Color.SandyBrown)
                {
                    if (sayac > 0)
                    {
                        clickedButton.BackColor = Color.Red;
                        sayac -= 1;
                        sayacLabel.Text = $"Kalan Mayın İşareti: {sayac}";
                        CheckWinCondition();
                    }
                }
                else if(clickedButton.BackColor == Color.Red)
                {
                    clickedButton.BackColor = Color.SandyBrown;
                    sayac += 1;
                    sayacLabel.Text = $"Kalan Mayın İşareti: {sayac}";
                }
            }
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton?.Tag is ValueTuple<int, int> coordinates)
            {
                int x = coordinates.Item1;
                int y = coordinates.Item2;


                if (soldierX == -1 && soldierY == -1)
                {
                    
                    PlaceSoldierAndMines(x, y);
                    CheckWinCondition();

                }
                else
                {
                    MoveSoldier(x, y);
                    CheckWinCondition();
                }
            }
        }

        private void PlaceSoldierAndMines(int startX, int startY)
        {
            
            soldierX = startX;
            soldierY = startY;
            soldier.Location = Buttons[startX, startY].Location;
            Buttons[startX, startY].BackgroundImage = Properties.Resources.soldier;
            Buttons[startX, startY].BackgroundImageLayout = ImageLayout.Stretch;
            Buttons[startX, startY].BackColor = Color.SpringGreen;
            soldier.Visible = true;

            
            Random random = new Random();
            int minesPlaced = 0;

            while (minesPlaced < totalMines)
            {
                int randomX = random.Next(0, GridSize);
                int randomY = random.Next(0, GridSize);

               
                if ((randomX != startX || randomY != startY) && Buttons[randomX, randomY].BackgroundImage == null)
                {
                    PlaceMine(randomX, randomY);
                    mineLocations.Add((randomX, randomY));
                    minesPlaced++;
                }
            }
            HideMines();
        }

        private void PlaceMine(int x, int y)
        {
            Buttons[x, y].BackgroundImage = Properties.Resources.Mine; 
            Buttons[x, y].BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void MoveSoldier(int x, int y)
        {
           
            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize && !(x == soldierX && y == soldierY))
            {
              
                if (Math.Abs(x - soldierX) <= 1 && Math.Abs(y - soldierY) <= 1)
                {
                    Buttons[soldierX, soldierY].BackColor = Color.SpringGreen;
                    Buttons[soldierX, soldierY].BackgroundImage = null;

                    soldierX = x;
                    soldierY = y;
                    soldier.Location = Buttons[x, y].Location;

                    if (Buttons[x,y].BackColor == Color.Red)
                    {
                        sayac++;
                        sayacLabel.Text = $"Kalan Mayın İşareti: {sayac}";
                    }

                   
                    Buttons[x, y].BackColor = Color.SpringGreen;
                    Buttons[x, y].BackgroundImage = Properties.Resources.soldier;
                    Buttons[x, y].BackgroundImageLayout = ImageLayout.Stretch;

                    
                    if (mineLocations.Contains((x, y)))
                    {
                        GameOver();
                        ShowMines();

                    }
                }
            }
        }

        public void HideMines()
        {
            foreach (var location in mineLocations)
            {

                Buttons[location.Item1, location.Item2].BackgroundImage = null;
                Buttons[location.Item1, location.Item2].BackColor = Color.SandyBrown;

            }
        }

        public void ShowMines()
        {
            foreach (var location in mineLocations)
            {
                if (Buttons[location.Item1, location.Item2].BackgroundImage == null)
                {
                    Buttons[location.Item1, location.Item2].BackgroundImage = Properties.Resources.Mine;
                }
                else
                {
                    Buttons[location.Item1, location.Item2].BackColor = Color.DarkRed;
                }
            }
        }

        private void ResetGame()
        {
           
            soldierX = -1;
            soldierY = -1;
            soldier.Visible = false;
            mineLocations.Clear();
            foreach (var button in Buttons)
            {
                button.BackColor = Color.SandyBrown;
                button.BackgroundImage = null;
                button.Enabled = true;
                button.TabStop = false;
            }
            ilerleme.Value = 0;
            sayac = 25;
            sayacLabel.Text = $"Kalan Mayın İşareti: {sayac}";


        }

        public void GameOver()
        {
            foreach (var button in Buttons)
            {
                button.Enabled = false;
            }
            string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Explosion - Minecraft Sound Effect (HD).mp3");

            WindowsMediaPlayer boomsound = new WindowsMediaPlayer
            {
                URL = musicPath,
                settings = {volume = 100}
            };
            boomsound.controls.play();
            Form3 form = new Form3();
            form.ShowDialog();

        }
        public void RestartGame()
        {
            ResetGame();


        }

        public void ExitGame()
        {
            Application.Exit();
        }

        public async void ShowMineCountsAroundSoldier(string direction)
        {
            switch (direction)
            {
                case "up":
                    Form1.isleniyor = true;
                    littlePanel[0, 0].BackColor = Color.Red;
                    littlePanel[1, 0].BackColor = Color.Red;
                    littlePanel[2, 0].BackColor = Color.Red;
                    for (int i = -1; i < 2; i++)
                    {
                        if (mineLocations.Contains((soldierX + -1, soldierY + i)))
                        {
                            anlikMayin += 1;
                        }
                    }
                    await Task.Delay(500);
                    for(int i = 0; i<anlikMayin; i++)
                    {
                        PlayBeep();    
                        await Task.Delay(500);
                    }
                    anlikMayin = 0;
                    littlePanel[0, 0].BackColor = Color.SandyBrown;
                    littlePanel[1, 0].BackColor = Color.SandyBrown;
                    littlePanel[2, 0].BackColor = Color.SandyBrown;
                    Form1.isleniyor = false;
                    break;
                case "down":
                    Form1.isleniyor = true;
                    littlePanel[0, 2].BackColor = Color.Red;
                    littlePanel[1, 2].BackColor = Color.Red;
                    littlePanel[2, 2].BackColor = Color.Red;
                    for (int i = -1; i < 2; i++)
                    {
                        if (mineLocations.Contains((soldierX + 1, soldierY + i)))
                        {
                            anlikMayin += 1;
                        }
                    }
                    await Task.Delay(500);
                    for (int i = 0; i < anlikMayin; i++)
                    {
                        PlayBeep();
                        await Task.Delay(500);
                    }
                    anlikMayin = 0;
                    littlePanel[0, 2].BackColor = Color.SandyBrown;
                    littlePanel[1, 2].BackColor = Color.SandyBrown;
                    littlePanel[2, 2].BackColor = Color.SandyBrown;
                    Form1.isleniyor = false;
                    break;
                case "right":
                    Form1.isleniyor = true;
                    littlePanel[2, 0].BackColor = Color.Red;
                    littlePanel[2, 1].BackColor = Color.Red;
                    littlePanel[2, 2].BackColor = Color.Red;
                    for (int i = -1; i < 2; i++)
                    {
                        if (mineLocations.Contains((soldierX + i, soldierY + 1)))
                        {
                            anlikMayin += 1;
                        }
                    }
                    await Task.Delay(500);
                    for (int i = 0; i < anlikMayin; i++)
                    {
                        PlayBeep();
                        await Task.Delay(500);
                    }
                    anlikMayin = 0;
                    littlePanel[2, 0].BackColor = Color.SandyBrown;
                    littlePanel[2, 1].BackColor = Color.SandyBrown;
                    littlePanel[2, 2].BackColor = Color.SandyBrown;
                    Form1.isleniyor = false;
                    break;
                case "left":
                    Form1.isleniyor = true;
                    littlePanel[0, 0].BackColor = Color.Red;
                    littlePanel[0, 1].BackColor = Color.Red;
                    littlePanel[0, 2].BackColor = Color.Red;
                    for (int i = -1; i < 2; i++)
                    {
                        if (mineLocations.Contains((soldierX + i, soldierY + -1)))
                        {
                            anlikMayin += 1;
                        }
                    }
                    await Task.Delay(500);
                    for (int i = 0; i < anlikMayin; i++)
                    {
                        PlayBeep();
                        await Task.Delay(500);
                    }
                    anlikMayin = 0;
                    littlePanel[0, 0].BackColor = Color.SandyBrown;
                    littlePanel[0, 1].BackColor = Color.SandyBrown;
                    littlePanel[0, 2].BackColor = Color.SandyBrown;
                    Form1.isleniyor = false;
                    break;


            }

            
        }

        public void CheckWinCondition()
        {
            int steppedCells = 0;

            for(int i = 0; i < GridSize ; i++)
            {
                for(int j = 0; j < GridSize ; j++)
                {
                    if (Buttons[i,j].BackColor == Color.SpringGreen)
                    {
                        steppedCells++;
                    }

                }
            }

            ilerleme.Value = steppedCells;
            if(steppedCells == (GridSize*GridSize) - mineCount && sayac == 0)
            {
                foreach(var button in Buttons)
                {
                    button.Enabled = false;
                }
                ShowMines();
                string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mount & Blade_ Warband - questsucceeded.mp3");
                WindowsMediaPlayer glorysound = new WindowsMediaPlayer
                {
                    URL = musicPath,
                    settings = {volume = 100}
                };
                glorysound.controls.play();
                Form4 form = new Form4();
                form.ShowDialog();

            }


        }
    }
}
