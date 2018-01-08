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
*/


namespace WindowsFormsApp5
{
    public partial class TicTacToe : Form
    {
        string _addr = "127.0.0.1";
        int _port = 12345;
        string _name = "New Player";
        //  'n' is none, 'X' is team1, 'O' is team2
        char[] _boardLocal  = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };   //size 9
        char[] _boardRemote = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };

        bool _turn = true;
        bool _someoneWon = false;
        IPAddress _ipAddr;
        Socket _soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread _newThread;
        byte[] _recBuffer = new byte[20];
        bool _closeThread = false;

        delegate void StringArgReturningVoidDelegate(string text);

        public static class GetIp {
            public static string getIP { get; set; }
        }
        public static class GetPort {
            public static int getPort { get; set; }
        }
        public static class GetName {
            public static string getName { get; set; }
        }

        public TicTacToe()
        {
            InitializeComponent();
            GetIp.getIP = _addr;
            GetPort.getPort = _port;
            GetName.getName = _name;

            textBox1.Select();
            _newThread = new Thread(ExecuteInForeground);
            _newThread.Start();
        }

        private void ExecuteInForeground()
        {
            ConnectToServer();
            ReceiveData(_soc);
        }

        private void ConnectToServer()
        {
            _ipAddr = IPAddress.Parse(GetIp.getIP);
            IPEndPoint remoteEp = new IPEndPoint(_ipAddr, GetPort.getPort);
            //ipAddr = IPAddress.Parse(addr);
            //IPEndPoint remoteEP = new IPEndPoint(ipAddr, port);
            try
            {
                _soc.Connect(remoteEp);
            }
            catch (SocketException e)
            {
                //MessageBox.Show(e.Message);
                SetText("connectToServer() error");
                //soc.Close();
            }
                
        }

        private void ReceiveData(Socket soc)
        {
            int delay = 1000 * 20;
            while (!_closeThread)
            {
                try
                {
                    if (soc.Receive(_recBuffer) > 0)
                        SetText(Encoding.ASCII.GetString(_recBuffer));
                }
                catch (Exception e)
                {
                    if (!(e is ObjectDisposedException) && !(e is SocketException)) continue;
                    SetText($"couldn't receive data. Waiting for {delay/1000} sec.");
                    //MessageBox.Show($"{e.Message}");
                    Thread.Sleep(delay);
                    //throw;                        
                }                
            }
            //MessageBox.Show($"close_thread val: {close_thread}, exiting socket, {}")
            soc.Disconnect(true);
        }

        private int SendData(Socket soc, string input)
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
                StringArgReturningVoidDelegate d = SetText;
                Invoke(d, text);
            }
            else
            {
                listBox1.Items.Add(text);
                listBox1.TopIndex = listBox1.Items.Count - 1;
            }
            
        }

        /*
        private bool requestMove()
        {
            if ( sendData(soc, new string(board_local)) > 0 )
        }
        */

        private void MapMovement(char starts, int ends, char[] board)
        {
            char player;
            if (_turn) player = 'X';
            else              player = 'O';

            if      (starts == 'a') board[ends + 0 - 1] = player;
            else if (starts == 'b') board[ends + 3 - 1] = player;
            else if (starts == 'c') board[ends + 6 - 1] = player;
            else
            {
                SetText("mapMovement() error");
            }
            //SetText(new string(board));
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            var b = (Button)sender;
            if (!string.IsNullOrEmpty(b.Text))
                return;

            b.Text = _turn ? "X" : "O";

            MapMovement(b.Name[0], b.Name[1] - '0', _boardLocal);    // - '0' converts to int value instead of ascii
            b.ForeColor = Color.Red;          // send vote to server, red means not approved yet, local board changed
            
            /*if(gotVoteResult)
                  b.ForeColor = Color.Black;

              store the result in board_remote, compare it to board_local,
              update the UI and board_local = board_remote
            */

            CheckWinner(b);     //send a signal to the server, albo server bedzie sprawdzal czy ktos wygral
            _turn = !_turn;
            textBox1.Select();
        }
        private bool ButtonComparison(Button b1, Button b2, Button b3)
        {
            return (b1.Text == b2.Text) && (b2.Text == b3.Text) && b1.Text.Length != 0;
        }

        private void CheckWinner(Button b)
        {
            bool fullBoard = true;
            //horizontal
            if (ButtonComparison(a1, a2, a3) || ButtonComparison(b1, b2, b3) || ButtonComparison(c1, c2, c3))
                _someoneWon = true;
            //vertical
            if (ButtonComparison(a1, b1, c1) || ButtonComparison(a2, b2, c2) || ButtonComparison(a3, b3, c3))
                _someoneWon = true;
            //diagonal
            if (ButtonComparison(a1, b2, c3) || ButtonComparison(a3, b2, c1))
                _someoneWon = true;

            if (_someoneWon) PlayerWon(b);

            foreach (Control c in Controls)
                if (c is GroupBox)
                    foreach (Control x in c.Controls)
                        if (x is Button && x.Text.Length == 0)      //if theres at least one button empty
                            fullBoard = false;

            if (fullBoard)
                groupBox1.Enabled = false;
        }

        private void PlayerWon(Button b)
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
            /*TODO
             send some kind of a message to the server that will request a new game, 
             then wait for server response
             */
            
            foreach (Control c in Controls)                 //clear the controls
                if (c is GroupBox)
                    foreach (Control b in c.Controls)
                        if (b is Button) b.Text = "";

            for (int i = 0; i < _boardLocal.Length; i++)    //clear the local board
                _boardLocal[i] = 'n';

            //listBox1.Items.Clear();
            SetText("New game started");
            _someoneWon = false;
            groupBox1.Enabled = true;
            _turn = true;                                    // 'X' always starts
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) return;
            SetText($"you: {textBox1.Text}");
            SendData(_soc, textBox1.Text);
            textBox1.Clear();
            textBox1.Select();
        }

        private void send_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) 13) return;
            send_Click(sender, e);
            e.Handled = true;       //żeby nie pikało
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OptionsBox.Show();
            _addr = GetIp.getIP;
            _port = GetPort.getPort;
            _name = GetName.getName;
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Select();
        }

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _closeThread = true;
            _newThread.Abort();
            //newThread.Join();
            _newThread = new Thread(ExecuteInForeground);
            _closeThread = false;
            _newThread.Start();
        }
    }
}
