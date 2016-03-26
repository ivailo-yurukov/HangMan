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
using System.Xml.Serialization;

namespace HangMan
{
    public partial class HangMan : Form
    {
        private Game game;
        private Label[] labels;
        private Font lettersFont = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        private bool hasHang = false;
        private Button[] buttons;


        public HangMan()
        {
            InitializeComponent();

            buttons = new Button[] { btnA, btnB, btnC, btnD, btnE, btnF, btnG, btnH, btnI, btnJ, btnK, btnL, btnM,
                btnN, btnO, btnP, btnQ, btnR, btnS, btnT, btnU, btnV, btnW, btnX, btnY, btnZ };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Click += LetterButtonClick;
            }
        }

        private void DrawHang()
        {
            Graphics g = pnlHangMan.CreateGraphics();
            Pen pen = new Pen(Color.Red, 10);
            g.DrawLine(pen, new Point(160, 200), new Point(160, 30));
            g.DrawLine(pen, new Point(165, 30), new Point(85, 30));
            g.DrawLine(pen, new Point(80, 25), new Point(80, 65));
            g.DrawLine(pen, new Point(120, 200), new Point(200, 200));

            g.Dispose();
        }

        private void DrawBody(Graphics g)
        {
            if (game == null)
            {
                return;
            }
            Pen pen = new Pen(Color.YellowGreen, 3);

            if (game.WrongLetterCounter >= 1)
            {
                g.DrawEllipse(pen, 60, 65, 40, 40);
            }

            if (game.WrongLetterCounter >= 2)
            {
                g.DrawLine(pen, new Point(80, 105), new Point(80, 160));
            }

            if (game.WrongLetterCounter >= 3)
            {
                g.DrawLine(pen, new Point(80, 130), new Point(40, 85));
            }

            if (game.WrongLetterCounter >= 4)
            {
                g.DrawLine(pen, new Point(80, 130), new Point(120, 85));
            }

            if (game.WrongLetterCounter >= 5)
            {
                g.DrawLine(pen, new Point(80, 158), new Point(120, 200));
            }

            if (game.WrongLetterCounter >= 6)
            {
                g.DrawLine(pen, new Point(80, 158), new Point(40, 200));
            }
        }

        private void HangMan_Shown(object sender, EventArgs e)
        {
            hasHang = true;
            DrawHang();
        }

        private void newGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            game = new Game();
            EnableButtons();
            txtWrongLetters.Clear();
            pnlHangMan.CreateGraphics().Clear(Color.FromKnownColor(KnownColor.Control));
            DrawHang();
            CreateLabels();
        }

        private void EnableButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Enabled = true;
            }
        }

        private void CreateLabels()
        {
            pnlRevealedLetters.Controls.Clear();
            labels = new Label[game.RevealedLetters.Length];
            int betweenLabels = 30;
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new Label();
                labels[i].Font = lettersFont;
                labels[i].Text = game.RevealedLetters[i].ToString().ToLower();
                pnlRevealedLetters.Controls.Add(labels[i]);
                labels[i].Location = new Point((i * betweenLabels), 0);
                labels[i].BringToFront();
                labels[i].CreateControl();
            }
        }

        private void LetterButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == sender)
                {
                    buttons[i].Enabled = false;
                }
            }

            if (game == null)
            {
                MessageBox.Show("Please start New Game !");
                return;
            }
            if (game.IsRightLetter(button.Text[0]))
            {
                UpdateLabels();

                if (game.IsWon())
                {
                    MessageBox.Show("You win !");
                    DisableButtons();
                }
            }
            else
            {
                MessageBox.Show("Wrong letter !");
                txtWrongLetters.Text = game.WrongLettersResult.ToString();               
                pnlHangMan.Invalidate();
                if (game.WrongLetterCounter == 6)
                {
                    MessageBox.Show("You are dead !");
                    DisableButtons();
                }
            }
        }

        private void DisableButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Enabled = false;
            }
        }

        private void UpdateLabels()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = game.RevealedLetters[i].ToString();
            }
        }

        private void pnlHangMan_Paint(object sender, PaintEventArgs e)
        {
            if (hasHang)
            {
                DrawHang();
            }

            DrawBody(e.Graphics);
        }

        private void saveGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "|*.hng";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = dialog.OpenFile())
                {
                    GameSaveInfo saveInfo = new GameSaveInfo();
                    saveInfo.Word = game.word;
                    saveInfo.RevealedLetters = game.RevealedLetters;
                    saveInfo.WrongLetterCounter = game.WrongLetterCounter;
                    saveInfo.WrongLettersResult = game.WrongLettersResult;

                    XmlSerializer serializer = new XmlSerializer(typeof(GameSaveInfo));
                    serializer.Serialize(file, saveInfo);
                }
            }

        }

        private void loadGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "|*.hng";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GameSaveInfo saveInfo = null;
                    using (Stream file = dialog.OpenFile())
                    {
                        if (dialog.FileName.ToLower().EndsWith(".hng"))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveInfo));
                            saveInfo = (GameSaveInfo)serializer.Deserialize(file);
                        }
                        else
                        {
                            throw new InvalidOperationException("Unknown file type");
                        }
                    }
                    game = new Game(saveInfo);
                    CreateLabels();
                    UpdateLabels();
                    txtWrongLetters.Text = game.WrongLettersResult.ToString();                    
                    pnlHangMan.Invalidate();

                }
                catch (Exception)
                {

                    MessageBox.Show("Cannot load file");
                }
            }
        }
    }
}
