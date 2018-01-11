using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*  TODO
    add input validation
*/

namespace WindowsFormsApp5
{
    public partial class OptionsBox : Form
    {
        public OptionsBox()
        {
            InitializeComponent();
        }
        static OptionsBox myOptionsBox;
        static DialogResult result = DialogResult.No;
        public static DialogResult Show()
        {
            myOptionsBox = new OptionsBox();
            myOptionsBox.ShowDialog();
            return result;
        }

        private void buttonOk_Click_1(object sender, EventArgs e)
        {
            TicTacToe.GetIP.getIP = textBoxIP.Text;
            TicTacToe.GetPort.getPort = Int32.Parse(textBoxPort.Text);
            TicTacToe.GetName.getName = textBoxName.Text;
            result = DialogResult.Yes;
            myOptionsBox.Close();
        }
    }
}
