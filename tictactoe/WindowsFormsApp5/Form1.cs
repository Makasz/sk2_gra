﻿using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

/* todo
    - opcje?
*/


namespace TicTacToe_SK2
{
    public partial class TicTacToe : Form
    {
        //string _addr = "192.168.1.106";
        string _addr = "127.0.0.1";
        int _port = 12345;
        //  'n' is none, 'X' is team1, 'O' is team2
        char[] _boardLocal  = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };   //size 9,
        char[] _boardRemote = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };
        private List<Button> _buttonList;

        bool _waitingForVote = false;
        bool _firstVoteReceived = false;
        bool _xStarts = true;
        //bool _turn = true;
        private char _player = 'X';
        bool _someoneWon = false;
        IPAddress _ipAddr;
        readonly Socket _soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int _r;
        private string _msgBuffer;

        delegate void StringArgReturningVoidDelegate(string text);

        public TicTacToe()
        {
            InitializeComponent();
            _buttonList = new List<Button> { a1, a2, a3, b1, b2, b3, c1, c2, c3 };

            textBox1.Select();
            Thread newThread = new Thread(ExecuteInForeground) { IsBackground = true };
            newThread.Start();
        }

        private void ExecuteInForeground()
        {
            ConnectToServer();
            ReceiveData(_soc);
        }

        /*
        static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }
        */
        private void ConnectToServer()                          //todo while( notConnected) { try soc.connect() } 
        {                                                            //connect nic nie zwraca
            _ipAddr = IPAddress.Parse(_addr);
            IPEndPoint remoteEp = new IPEndPoint(_ipAddr, _port);
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
            const int delay = 1000 * 20;
            byte[] recBuffer = new byte[20];
            while (true)
            {
                try
                {
                    _r = soc.Receive(recBuffer);
                    if (_r > 0)
                    {
                        _msgBuffer = Encoding.ASCII.GetString(recBuffer);
                        SetText(_msgBuffer);
                        RecogniseMsg();
                    }
                }
                catch (Exception e)
                {
                    if (e is SocketException)
                    {
                        SetText($"couldn't receive data. Waiting for {delay / 1000} sec.");
                        //MessageBox.Show($"{e.Message}");
                        Thread.Sleep(delay);
                    }
                    else if (e is ObjectDisposedException){ }

                    //throw;                        
                }
            }
        }

        private void RecogniseMsg()
        {
            if (_msgBuffer.StartsWith("v"))        //received vote
            {
                if (_msgBuffer[10] != _player)
                    _waitingForVote = false;

                _xStarts = false;
                _boardLocal = _boardRemote;
                SetText(_msgBuffer.Substring(1, _msgBuffer.Length - 1));
                _boardRemote = _msgBuffer.Substring(1, _msgBuffer.Length - 2).ToCharArray();  
                if (_boardLocal.Length != _buttonList.Count)
                    SetText("Wrong size of boards");

                SetText("received vote");
                for (var i = 0; i < _buttonList.Count; i++)
                {
                    if (_boardRemote[i] != 'n' && (_boardRemote[i] == 'X' || _boardRemote[i] == 'O'))           //problem with ObjectDisposedException, controls doesnt refresh everytime
                    {
                        _buttonList[i].Text = _boardRemote[i].ToString();
                        _buttonList[i].Refresh();
                    }
                }
                CheckWinner();
            }

            else if (_msgBuffer.StartsWith("m"))
            {
                SetText(_msgBuffer.Substring(1, _msgBuffer.Length - 1));
            }

            else if (_msgBuffer.StartsWith("t"))               //nadawanie teamu
            {
                if (_msgBuffer[1].ToString() == "X")
                {
                    _player = 'X';
                }
                else if (_msgBuffer[1].ToString() == "O")
                {
                    _player = 'O';
                }
                else
                    SetText("error while choosing a team");

            }
        }
        
        private int SendData(Socket soc, string input)
        {
            int s = 0;
            try
            {
                s = soc.Send(Encoding.ASCII.GetBytes(input));
            }
            catch (SocketException)
            {
                //MessageBox.Show($"{e.Message}");
                SetText("sendData() error");
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
                listBox1.TopIndex = listBox1.Items.Count - 1;   //scrolls down listbox items
            }
        }
  
        private void MapMovement(char starts, int ends, char[] board, char player)
        {
            switch (starts)
            {
                case 'a':
                    board[ends + 0 - 1] = player;
                    break;
                case 'b':
                    board[ends + 3 - 1] = player;
                    break;
                case 'c':
                    board[ends + 6 - 1] = player;
                    break;
                default:
                    SetText("mapMovement() error");
                    return;
            }
        }
        
        private void OnButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            /*
            if ( _mvCounter % 2 == 0 )
            {
                textBox1.Select();
                return;
            }*/

            if ( _xStarts && _player != 'X' )
            {
                textBox1.Select();
                return;
            }
            if (!String.IsNullOrEmpty(b.Text) || _waitingForVote)
            {
                textBox1.Select();
                return;
            }
            _waitingForVote = true;

            //var player = _turn ? 'X' : 'O';
            
            b.Text = _player.ToString();
            //var boardLocalCpy = _boardLocal;

            MapMovement(b.Name[0], b.Name[1] - '0', _boardLocal, _player);  
            b.ForeColor = Color.Red;

            
            SendData(_soc, "v" + new string(_boardLocal) + _player);                              
            _r = 0;

            CheckWinner();    
            //_turn = !_turn;
            textBox1.Select();
        }
        private bool ButtonComparison(Button b1, Button b2, Button b3)
        {
            return (b1.Text == b2.Text) && (b2.Text == b3.Text) && b1.Text.Length != 0;
        }

        private void CheckWinner()
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

            if (_someoneWon) TeamWon();

            foreach (Control c in Controls)
                if (c is GroupBox)
                    foreach (Control x in c.Controls)
                        if (x is Button && x.Text.Length == 0)      //if theres at least one button empty
                            fullBoard = false;

            if (fullBoard)
                groupBox1.Enabled = false;
        }

        private void TeamWon()                          
        {
            groupBox1.Enabled = false;
            SetText($"Game over.");
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _soc.Close();
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

            for (int i = 0; i < _boardLocal.Length; i++)    //and the local board
                _boardLocal[i] = 'n';

            //listBox1.Items.Clear();
            SetText("New game started");
            _someoneWon = false;
            groupBox1.Enabled = true;
            //_turn = true;
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) return;
            SetText($"you: {textBox1.Text}");
            SendData(_soc, textBox1.Text);
            textBox1.Clear();
        }

        private void send_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) 13) return;
            send_Click(sender, e);
            e.Handled = true;       //żeby nie pikało
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not implemented yet.");
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            Close(); //to turn off current app
        }
    }
}
