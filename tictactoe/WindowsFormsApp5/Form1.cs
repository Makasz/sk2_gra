using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using WMPLib;

/*  TODO
- dodać opcje startu gry:
 
- obsłużyć sytuacje gdy jeden gracz nie odda głosu (dodać timer - po upływie czasu zaznaczenie pierwszej możliwej opcji)
 
- dodać funkcję wysyłania obecnego stanu gry do gracza, który dołącza do rozgrywki po jej rozpoczęciu:
chyba nie wymaga żadnych zmian w kliencie
     */

namespace TicTacToe_SK2
{
    public partial class TicTacToe : Form
    {
        //string _addr = "192.168.1.106";
        string _addr = "127.0.0.1";
        int _port = 12345;
        //  'n' is none, 'X' is team1, 'O' is team2
        char[] _boardLocal = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };   //size 9,
        char[] _boardRemote = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };
        private List<Button> _buttonList;

        bool _waitingForVote = false;
        bool _xStarts = true;
        private char _player = 'X';
        bool _someoneWon = false;
        IPAddress _ipAddr;
        readonly Socket _soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int _r;
        private string _msgBuffer;
        int _timeLeft = 15;

        WindowsMediaPlayer Player;

        delegate void StringArgReturningVoidDelegate(string text);

        public TicTacToe()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            InitializeComponent();
            InitializeCombobox();
            PlayFile("soundtrack.mp3");
            _buttonList = new List<Button> { a1, a2, a3, b1, b2, b3, c1, c2, c3 };
            Thread newThread = new Thread(ExecuteInForeground) { IsBackground = true };
            newThread.Start();
        }

        private void InitializeCombobox()
        {
            comboBox1.Items.Add("Greetings");
            comboBox1.Items.Add("Well played");
            comboBox1.Items.Add("Oops");
            comboBox1.Items.Add("Thank you");
            comboBox1.Items.Add("Thanks");
            comboBox1.Items.Add("Sorry");
            comboBox1.SelectedIndex = 0;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            //todo send exit msg to the server
            SetText("Exiting program...");
            //SendData(_soc, "L");
            _soc.Close();
            Application.Exit();
        }

        private void ExecuteInForeground()
        {
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            ConnectToServer();
            ReceiveData(_soc);
        }

        static bool IsSocketConnected(Socket s)
        {
            try
            {
                return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
            }
            catch (Exception e)
            {
                if (e is SocketException || e is ObjectDisposedException)
                    return false;
                throw;
            }
        }

        private void ConnectToServer()
        {
            const int delay = 1000 * 5;
            _ipAddr = IPAddress.Parse(_addr);
            IPEndPoint remoteEp = new IPEndPoint(_ipAddr, _port);
            while (!IsSocketConnected(_soc))
            {
                try
                {
                    _soc.Connect(remoteEp);
                }
                catch (SocketException)
                {
                    SetText($"failed to connect, waiting for {delay / 1000} sec...");
                    //soc.Close();
                    Thread.Sleep(delay);
                }
                catch (InvalidOperationException)
                {
                    SetText("Server turned off. The program will now exit.");
                    Thread.Sleep(3000);
                    Application.Exit();
                }
            }
            SetText("Succesfully connected.");
        }

        private void ReceiveData(Socket soc)
        {
            byte[] recBuffer = new byte[20];
            while (true)
            {
                try
                {
                    _r = soc.Receive(recBuffer);
                    if (_r > 0)
                    {
                        _msgBuffer = Encoding.ASCII.GetString(recBuffer);
                        //SetText("rec:" + _msgBuffer.Length);
                        //SetText(_msgBuffer);
                        RecogniseMsg();
                    }
                }
                catch (Exception e)
                {
                    if (e is SocketException || e is ObjectDisposedException)
                        ConnectToServer();
                }
            }
        }

        private void RecogniseMsg()
        {
            switch (_msgBuffer[0])
            {
                case 'v':
                    {
                        _xStarts = false;
                        _boardRemote = _msgBuffer.Substring(1, _msgBuffer.Length - 2).ToCharArray();
                        for (int i = 0; i < _boardLocal.Length; i++)
                        {
                            _boardLocal[i] = _boardRemote[i];
                        }
                        for (var i = 0; i < _buttonList.Count; i++)
                        {
                            if (_boardRemote[i] == 'X' || _boardRemote[i] == 'O')
                                _buttonList[i].Invoke((Action)delegate { _buttonList[i].Text = _boardRemote[i].ToString(); });
                            else if (_boardRemote[i] == 'n')
                                _buttonList[i].Invoke((Action)delegate { _buttonList[i].Text = ""; });
                        }
                        CheckWinner();

                        if (_msgBuffer[10] != _player)
                        {
                            _timeLeft = 15;
                            timerLabel.Invoke((Action)delegate { timerLabel.Text = _timeLeft.ToString() + " sec"; });
                            timerLabel.Invoke((Action)delegate { timer1.Start(); });
                            _waitingForVote = false;
                        }
                        break;
                    }
                case 't':
                    {
                        _player = _msgBuffer[1];
                        if (_player != 'X' && _player != 'O')
                        {
                            SetText("error while choosing a team");
                            break;
                        }
                        SetText($"Joined team: {_player}");
                        teamLabel.Invoke((Action)delegate { teamLabel.Text = _player.ToString(); });
                        break;
                    }
                case 'm':
                    {
                        SetText(_msgBuffer.Substring(1, _r-1));
                        break;
                    }
                case 'r':
                    {
                        StartNewGame();
                        break;
                    }
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

        private void SendTimeoutVote()
        {
            foreach (Button b in _buttonList)
            {
                if (String.IsNullOrEmpty(b.Text))
                {
                    b.Text = _player.ToString();
                    MapMovement(b.Name[0], b.Name[1] - '0', _boardLocal, _player);
                    b.ForeColor = Color.Red;
                    string s = "v" + new string(_boardLocal) + _player;
                    SendData(_soc, s);
                    return;
                }
            }
            groupBox1.Enabled = false;
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

            if ((_xStarts && _player != 'X') ||
                (!String.IsNullOrEmpty(b.Text) || _waitingForVote || _timeLeft == 0))
            {
                send.Select();
                return;
            }

            _waitingForVote = true;

            b.Text = _player.ToString();
            MapMovement(b.Name[0], b.Name[1] - '0', _boardLocal, _player);
            b.ForeColor = Color.Red;                // do dopracowania

            string s = "v" + new string(_boardLocal) + _player;
            SendData(_soc, s);
            _r = 0;

            timerLabel.Invoke((Action)delegate { timerLabel.Text = "opponents turn"; });
            timerLabel.Invoke((Action)delegate { timer1.Stop(); });
            send.Select();

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

            foreach (Button b in _buttonList)
                if (b.Text.Length == 0)
                    fullBoard = false;

            if (fullBoard)
            {
                groupBox1.Invoke((Action)delegate { groupBox1.Enabled = false; });
                SetText("Draw!");
            }
        }

        private void TeamWon()
        {
            groupBox1.Invoke((Action)delegate { groupBox1.Enabled = false; });
            SetText($"Game over.");
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnProcessExit(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program made by Phil Bugaj and Mike Lesny.\n" +
                "Special thanks to Jan Konczak for being a project supervisor.");
        }

        private void StartNewGame()
        {
            foreach (Button b in _buttonList)
                b.Invoke((Action)delegate { b.Text = ""; });
            for (int i = 0; i < _boardLocal.Length; i++)
            {
                _boardLocal[i] = 'n';
                _boardRemote[i] = 'n';
            }
            _xStarts = true;
            _waitingForVote = false;

            SetText("New game started");
            _someoneWon = false;
            groupBox1.Invoke((Action)delegate { groupBox1.Enabled = true; });
            timerLabel.Invoke((Action)delegate {
                timer1.Stop();
                _timeLeft = 15;
                timerLabel.Text = _timeLeft.ToString();
            });

        }

        private void NewGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SendData(_soc, "r");
        }

        private void Send_Click(object sender, EventArgs e)
        {
            SendData(_soc, $"m{comboBox1.Text}");
            send.Select();
        }

        private void Send_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            Send_Click(sender, e);
            e.Handled = true;       //żeby nie pikało
        }

        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            Close(); //to turn off current app
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void muteSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MuteSound();
        }

        public void MuteSound()
        {
            if (muteSoundToolStripMenuItem.Text == "Mute sound")
            {
                Player.controls.pause();
                muteSoundToolStripMenuItem.Text = "Play sound";
            }
            else
            {
                Player.controls.play();
                muteSoundToolStripMenuItem.Text = "Mute sound";
            }
        }

        private void PlayFile(String url)
        {
            try
            {
                Player = new WindowsMediaPlayer();
                Player.PlayStateChange += Player_PlayStateChange;
                Player.URL = url;
                Player.controls.play();
            }
            catch (Exception e) { SetText(e.Message + "couldn't load sound file"); };
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPPlayState)NewState == WMPPlayState.wmppsStopped)
            {
                Player.controls.play();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= 1;
                timerLabel.Invoke((Action)delegate { timerLabel.Text = _timeLeft.ToString() + " sec"; });
            }
            else
            {
                timer1.Stop();
                timerLabel.Invoke((Action)delegate { timerLabel.Text = "opponents turn"; });
                SendTimeoutVote();
                listBox1.Invoke((Action)delegate { listBox1.Items.Add("a timeout vote has been sent"); });
            }
        }

    }
}