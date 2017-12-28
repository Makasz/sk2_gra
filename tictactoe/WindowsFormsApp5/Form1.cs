using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class TicTacToe : Form
    {
        bool turn = true;
        bool someone_won = false;

        public TicTacToe()
        {
            InitializeComponent();
            textBox1.Select();
        }        

        private void onButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (!String.IsNullOrEmpty(b.Text))
                return;

            if (turn)
                b.Text = "X";
            else
                b.Text = "O";

            checkWinner(b);
            turn = !turn;
            textBox1.Select();
        }
        private bool buttonComparison(Button b1, Button b2, Button b3)
        {
            if ((b1.Text == b2.Text) && (b2.Text == b3.Text) && b1.Text.Length != 0)
                return true;
            else return false;
        }

        private void checkWinner(Button b)
        {
            //horizontal
            if (buttonComparison(a1, a2, a3) || buttonComparison(b1, b2, b3) || buttonComparison(c1, c2, c3))
                someone_won = true;
            //vertical
            if (buttonComparison(a1, b1, c1) || buttonComparison(a2, b2, c2) || buttonComparison(a3, b3, c3))
                someone_won = true;
            //diagonal
            if (buttonComparison(a1, b2, c3) || buttonComparison(a3, b2, c1))
                someone_won = true;

            if (someone_won)
                playerWon(b);
        }

        private void playerWon(Button b)
        {
            groupBox1.Enabled = false;
            listBox1.Items.Add($"Player {b.Text} won.");
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program made by Phil Bugaj and Mike Lesny.\n" +
                "Special thanks to Kan Jonczak for being a project supervisor.");
        }

        private void newGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                if (c is GroupBox)
                {
                    foreach (Control b in c.Controls)
                    {
                        if (b is Button)
                            b.Text = "";
                    }
                }
            }
            //listBox1.Items.Clear();
            listBox1.Items.Add("New game started");
            listBox1.TopIndex = listBox1.Items.Count - 1;   //scrolls down listbox items
            someone_won = false;
            groupBox1.Enabled = true;
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;
            listBox1.Items.Add(textBox1.Text);
            listBox1.TopIndex = listBox1.Items.Count - 1; 
            textBox1.Clear();
        }

        private void send_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                send_Click(sender, e);
                e.Handled = true;       //żeby nie pikało
            }
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet.");
        }
    }
}
