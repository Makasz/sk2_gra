namespace TicTacToe_SK2
{
    partial class TicTacToe
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TicTacToe));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.muteSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.send_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.a2 = new System.Windows.Forms.Button();
            this.c1 = new System.Windows.Forms.Button();
            this.c3 = new System.Windows.Forms.Button();
            this.a1 = new System.Windows.Forms.Button();
            this.c2 = new System.Windows.Forms.Button();
            this.b1 = new System.Windows.Forms.Button();
            this.b3 = new System.Windows.Forms.Button();
            this.a3 = new System.Windows.Forms.Button();
            this.b2 = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.teamLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.timerLabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(423, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem1,
            this.quitToolStripMenuItem1});
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.generalToolStripMenuItem.Text = "General";
            // 
            // newGameToolStripMenuItem1
            // 
            this.newGameToolStripMenuItem1.Name = "newGameToolStripMenuItem1";
            this.newGameToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.newGameToolStripMenuItem1.Text = "New game";
            this.newGameToolStripMenuItem1.Click += new System.EventHandler(this.NewGameToolStripMenuItem1_Click);
            // 
            // quitToolStripMenuItem1
            // 
            this.quitToolStripMenuItem1.Name = "quitToolStripMenuItem1";
            this.quitToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.quitToolStripMenuItem1.Text = "Quit";
            this.quitToolStripMenuItem1.Click += new System.EventHandler(this.quitToolStripMenuItem1_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.muteSoundToolStripMenuItem,
            this.restartToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.optionsToolStripMenuItem.Text = "More";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // muteSoundToolStripMenuItem
            // 
            this.muteSoundToolStripMenuItem.Name = "muteSoundToolStripMenuItem";
            this.muteSoundToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.muteSoundToolStripMenuItem.Text = "Mute sound";
            this.muteSoundToolStripMenuItem.Click += new System.EventHandler(this.muteSoundToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.RestartToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 406);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(300, 95);
            this.listBox1.TabIndex = 14;
            // 
            // send_button
            // 
            this.send_button.Location = new System.Drawing.Point(318, 507);
            this.send_button.Name = "send_button";
            this.send_button.Size = new System.Drawing.Size(58, 23);
            this.send_button.TabIndex = 16;
            this.send_button.Text = "Send";
            this.send_button.UseVisualStyleBackColor = true;
            this.send_button.Click += new System.EventHandler(this.Send_Click);
            this.send_button.KeyDown += new System.Windows.Forms.KeyEventHandler(this.send_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.a2);
            this.groupBox1.Controls.Add(this.c1);
            this.groupBox1.Controls.Add(this.c3);
            this.groupBox1.Controls.Add(this.a1);
            this.groupBox1.Controls.Add(this.c2);
            this.groupBox1.Controls.Add(this.b1);
            this.groupBox1.Controls.Add(this.b3);
            this.groupBox1.Controls.Add(this.a3);
            this.groupBox1.Controls.Add(this.b2);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 341);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // a2
            // 
            this.a2.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.a2.Location = new System.Drawing.Point(145, 19);
            this.a2.Name = "a2";
            this.a2.Size = new System.Drawing.Size(100, 100);
            this.a2.TabIndex = 28;
            this.a2.UseVisualStyleBackColor = true;
            this.a2.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // c1
            // 
            this.c1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.c1.Location = new System.Drawing.Point(38, 231);
            this.c1.Name = "c1";
            this.c1.Size = new System.Drawing.Size(100, 100);
            this.c1.TabIndex = 27;
            this.c1.UseVisualStyleBackColor = true;
            this.c1.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // c3
            // 
            this.c3.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.c3.Location = new System.Drawing.Point(250, 230);
            this.c3.Name = "c3";
            this.c3.Size = new System.Drawing.Size(100, 100);
            this.c3.TabIndex = 33;
            this.c3.UseVisualStyleBackColor = true;
            this.c3.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // a1
            // 
            this.a1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.a1.Location = new System.Drawing.Point(39, 19);
            this.a1.Name = "a1";
            this.a1.Size = new System.Drawing.Size(100, 100);
            this.a1.TabIndex = 25;
            this.a1.UseVisualStyleBackColor = true;
            this.a1.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // c2
            // 
            this.c2.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.c2.Location = new System.Drawing.Point(144, 230);
            this.c2.Name = "c2";
            this.c2.Size = new System.Drawing.Size(100, 100);
            this.c2.TabIndex = 32;
            this.c2.UseVisualStyleBackColor = true;
            this.c2.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // b1
            // 
            this.b1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.b1.Location = new System.Drawing.Point(39, 125);
            this.b1.Name = "b1";
            this.b1.Size = new System.Drawing.Size(100, 100);
            this.b1.TabIndex = 26;
            this.b1.UseVisualStyleBackColor = true;
            this.b1.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // b3
            // 
            this.b3.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.b3.Location = new System.Drawing.Point(250, 125);
            this.b3.Name = "b3";
            this.b3.Size = new System.Drawing.Size(100, 100);
            this.b3.TabIndex = 31;
            this.b3.UseVisualStyleBackColor = true;
            this.b3.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // a3
            // 
            this.a3.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.a3.Location = new System.Drawing.Point(251, 19);
            this.a3.Name = "a3";
            this.a3.Size = new System.Drawing.Size(100, 100);
            this.a3.TabIndex = 29;
            this.a3.UseVisualStyleBackColor = true;
            this.a3.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // b2
            // 
            this.b2.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.b2.Location = new System.Drawing.Point(144, 125);
            this.b2.Name = "b2";
            this.b2.Size = new System.Drawing.Size(100, 100);
            this.b2.TabIndex = 30;
            this.b2.UseVisualStyleBackColor = true;
            this.b2.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(382, 507);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(27, 23);
            this.clearButton.TabIndex = 18;
            this.clearButton.Text = "C";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(339, 406);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Your team:";
            // 
            // teamLabel
            // 
            this.teamLabel.AutoSize = true;
            this.teamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.teamLabel.Location = new System.Drawing.Point(340, 419);
            this.teamLabel.Name = "teamLabel";
            this.teamLabel.Size = new System.Drawing.Size(57, 55);
            this.teamLabel.TabIndex = 20;
            this.teamLabel.Text = "X";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(67, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "Time left:";
            // 
            // timerLabel
            // 
            this.timerLabel.AutoSize = true;
            this.timerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.timerLabel.Location = new System.Drawing.Point(175, 31);
            this.timerLabel.Name = "timerLabel";
            this.timerLabel.Size = new System.Drawing.Size(82, 25);
            this.timerLabel.TabIndex = 22;
            this.timerLabel.Text = "15 sec";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 509);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(300, 21);
            this.comboBox1.TabIndex = 23;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(318, 477);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(91, 23);
            this.startButton.TabIndex = 24;
            this.startButton.Text = "START";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // TicTacToe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 536);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.timerLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.teamLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.send_button);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TicTacToe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TicTacToe";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button send_button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button a2;
        private System.Windows.Forms.Button c1;
        private System.Windows.Forms.Button c3;
        private System.Windows.Forms.Button a1;
        private System.Windows.Forms.Button c2;
        private System.Windows.Forms.Button b1;
        private System.Windows.Forms.Button b3;
        private System.Windows.Forms.Button a3;
        private System.Windows.Forms.Button b2;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label teamLabel;
        private System.Windows.Forms.ToolStripMenuItem muteSoundToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label timerLabel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button startButton;
    }
}

