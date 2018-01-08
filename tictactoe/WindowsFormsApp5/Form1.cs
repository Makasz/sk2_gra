using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/* todo
    - interpretacja polecen odbieranych przez serwer
    - wysylanie stanu gry do serwera
    - wybrany kandydat na ruch, zatwierdzenie przez serwer (zmiana koloru pola)
    - czat?
    - opcje
*/


namespace TicTacToe_SK2
{
    public partial class TicTacToe : Form
    {
        string _addr = "127.0.0.1";
        int _port = 12345;
        //  'n' is none, 'X' is team1, 'O' is team2
        char[] _boardLocal  = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };   //size 9
        char[] _boardRemote = { 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n' };

        bool _turn = true;
        bool _someoneWon = false;
        IPAddress _ipAddr;
        Socket _soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] _recBuffer = new byte[20];

        delegate void StringArgReturningVoidDelegate(string text);

        public TicTacToe()
        {
            InitializeComponent();
            textBox1.Select();
            Thread newThread = new Thread(ExecuteInForeground);
            newThread.Start();
        }

        private void ExecuteInForeground()
        {
            ConnectToServer();
            ReceiveData(_soc);
        }

        private void ConnectToServer()
        {
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
            int delay = 1000 * 20;
            while (true)
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
            else listBox1.Items.Add(text);
        }

        /*
        private bool requestMove()
        {
            if ( sendData(soc, new string(board_local)) > 0 )
        }
        */

        private void MapMovement(char starts, int ends, char[] board)
        {
            var player = _turn ? 'X' : 'O';

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

            //SetText(new string(board));
            //listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (!String.IsNullOrEmpty(b.Text))
                return;

            b.Text = _turn ? "X" : "O";

            MapMovement(b.Name[0], b.Name[1] - '0', _boardLocal);    // - '0' converts to int value instead of ascii
            b.ForeColor = Color.Red;          // sending vote to server, red means not approved yet, local board changed
            
            /*if(gotVoteResult)
                  b.ForeColor = Color.Black;

              store the result in board_remote, compare it to board_local,
              update the UI and board_local = board_remote
            */

            checkWinner(b);     //send a signal to the server, albo server bedzie sprawdzal czy ktos wygral
            _turn = !_turn;
            textBox1.Select();
        }
        private bool ButtonComparison(Button b1, Button b2, Button b3)
        {
            return (b1.Text == b2.Text) && (b2.Text == b3.Text) && b1.Text.Length != 0;
        }

        private void checkWinner(Button b)
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
            foreach (Control c in Controls)                 //clear the controls
                if (c is GroupBox)
                    foreach (Control b in c.Controls)
                        if (b is Button) b.Text = "";

            for (int i = 0; i < _boardLocal.Length; i++)    //and the local board
                _boardLocal[i] = 'n';

            //listBox1.Items.Clear();
            SetText("New game started");
            listBox1.TopIndex = listBox1.Items.Count - 1;   //scrolls down listbox items
            _someoneWon = false;
            groupBox1.Enabled = true;
            _turn = true;
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0) return;
            SetText($"you: {textBox1.Text}");
            SendData(_soc, textBox1.Text);
            listBox1.TopIndex = listBox1.Items.Count - 1; 
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
    }
}
