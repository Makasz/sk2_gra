using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

/* todo
    - interpretacja polecen odbieranych przez serwer
    - wysylanie stanu gry do serwera
    - wybrany kandydat na ruch, zatwierdzenie przez serwer (zmiana koloru pola)
    - czat?
    - opcje
*/


namespace WindowsFormsApp5
{
    public partial class TicTacToe : Form
    {
        string addr = "127.0.0.1";
        int port = 12345;
        //  'n' is none, 'X' is team1, 'O' is team2
        char[] board_local  = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };   //size 9
        char[] board_remote = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };

        bool turn = true;
        bool someone_won = false;
        IPAddress ipAddr;
        Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] recBuffer = new byte[20];

        delegate void StringArgReturningVoidDelegate(string text);

        public TicTacToe()
        {
            InitializeComponent();
            textBox1.Select();
            Thread newThread = new Thread(executeInForeground);
            newThread.Start();
        }

        private void executeInForeground()
        {
            connectToServer();
            receiveData(soc);
        }

        private void connectToServer()
        {
            ipAddr = IPAddress.Parse(addr);
            IPEndPoint remoteEP = new IPEndPoint(ipAddr, port);
            try
            {
                soc.Connect(remoteEP);
            }
            catch (SocketException e)
            {
                //MessageBox.Show(e.Message);
                SetText("connectToServer() error");
                //soc.Close();
            }
                
        }

        private void receiveData(Socket soc)
        {
            int delay = 1000 * 20;
            while (true)
            {
                try
                {
                    if (soc.Receive(recBuffer) > 0)
                        SetText(Encoding.ASCII.GetString(recBuffer));
                }
                catch (Exception e)
                {
                    if(e is ObjectDisposedException || e is SocketException)
                    {
                        SetText($"couldn't receive data. Waiting for {delay/1000} sec.");
                        //MessageBox.Show($"{e.Message}");
                        Thread.Sleep(delay);
                    }
                    //throw;                        
                }
                
            }
        }

        private int sendData(Socket soc, string input)
        {
            int s = 0;
            try
            {
                s = soc.Send(Encoding.ASCII.GetBytes(input));
            }
            catch (SocketException e)
            {
                //MessageBox.Show($"{e.Message}");
                SetText($"sendData() error");
            }
            return s;            
        }

        private void SetText(string text)
        {
            if (listBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetText);
                Invoke(d, new object[] { text });
            }
            else listBox1.Items.Add(text);
        }

        /*
        private bool requestMove()
        {
            if ( sendData(soc, new string(board_local)) > 0 )
        }
        */

        private void mapMovement(char starts, int ends, char[] board)
        {
            char player;
            if (turn == true) player = 'X';
            else              player = 'O';

            if      (starts == 'a') board[ends + 0 - 1] = player;
            else if (starts == 'b') board[ends + 3 - 1] = player;
            else if (starts == 'c') board[ends + 6 - 1] = player;
            else
            {
                SetText("mapMovement() error");
                return;
            }
            //SetText(new string(board));
            //listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        private void onButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (!String.IsNullOrEmpty(b.Text))
                return;

            if (turn) b.Text = "X";
            else      b.Text = "O";

            mapMovement(b.Name[0], b.Name[1] - '0', board_local);    // - '0' converts to int value instead of ascii
            b.ForeColor = Color.Red;          // sending vote to server, red means not approved yet, local board changed
            
            /*if(gotVoteResult)
                  b.ForeColor = Color.Black;

              store the result in board_remote, compare it to board_local,
              update the UI and board_local = board_remote
            */

            checkWinner(b);     //send a signal to the server, albo server bedzie sprawdzal czy ktos wygral
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
            bool full_board = true;
            //horizontal
            if (buttonComparison(a1, a2, a3) || buttonComparison(b1, b2, b3) || buttonComparison(c1, c2, c3))
                someone_won = true;
            //vertical
            if (buttonComparison(a1, b1, c1) || buttonComparison(a2, b2, c2) || buttonComparison(a3, b3, c3))
                someone_won = true;
            //diagonal
            if (buttonComparison(a1, b2, c3) || buttonComparison(a3, b2, c1))
                someone_won = true;

            if (someone_won) playerWon(b);

            foreach (Control c in Controls)
                if (c is GroupBox)
                    foreach (Control x in c.Controls)
                        if (x is Button && x.Text.Length == 0)      //if theres at least one button empty
                            full_board = false;

            if (full_board)
                groupBox1.Enabled = false;
        }

        private void playerWon(Button b)
        {
            groupBox1.Enabled = false;
            SetText($"Player {b.Text} won.");
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program made by Phil Bugaj and Mike Lesny.\n" +
                "Special thanks to Jan Konczak for being a project supervisor.");
        }

        private void newGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (Control c in Controls)                 //clear the controls
                if (c is GroupBox)
                    foreach (Control b in c.Controls)
                        if (b is Button) b.Text = "";

            for (int i = 0; i < board_local.Length; i++)    //and the local board
                board_local[i] = 'n';

            //listBox1.Items.Clear();
            SetText("New game started");
            listBox1.TopIndex = listBox1.Items.Count - 1;   //scrolls down listbox items
            someone_won = false;
            groupBox1.Enabled = true;
            turn = true;
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) return;
            SetText($"you: {textBox1.Text}");
            sendData(soc, textBox1.Text);
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
