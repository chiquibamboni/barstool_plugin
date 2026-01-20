namespace BarstoolPlugin
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            seatDepthSLimitLabel = new Label();
            footrestDiameterD2LimitLabel = new Label();
            legDiameterD1LimitLabel = new Label();
            seatDiameterDLimitLabel = new Label();
            footrestHeightH1LimitLabel = new Label();
            stoolHeightHLimitLabel = new Label();
            pictureBox1 = new PictureBox();
            BuildButton = new Button();
            groupBox1 = new GroupBox();
            richTextBox = new RichTextBox();
            stoolHeightHTextBox = new TextBox();
            footrestHeightH1TextBox = new TextBox();
            legDiameterD1TextBox = new TextBox();
            seatDiameterDTextBox = new TextBox();
            seatDepthSTextBox = new TextBox();
            footrestDiameterD2TextBox = new TextBox();
            label13 = new Label();
            legCountCTextBox = new TextBox();
            legCountCLimitLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(117, 20);
            label1.TabIndex = 0;
            label1.Text = "Высота стула H:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 42);
            label2.Name = "label2";
            label2.Size = new Size(157, 20);
            label2.TabIndex = 1;
            label2.Text = "Высота подножки h1:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 108);
            label3.Name = "label3";
            label3.Size = new Size(143, 20);
            label3.TabIndex = 3;
            label3.Text = "Диаметр ножки d1:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 75);
            label4.Name = "label4";
            label4.Size = new Size(150, 20);
            label4.TabIndex = 2;
            label4.Text = "Диаметр сидения D:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 174);
            label5.Name = "label5";
            label5.Size = new Size(128, 20);
            label5.TabIndex = 5;
            label5.Text = "Вылет сидения S:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 141);
            label6.Name = "label6";
            label6.Size = new Size(169, 20);
            label6.TabIndex = 4;
            label6.Text = "Диаметр подножки d2:";
            // 
            // seatDepthSLimitLabel
            // 
            seatDepthSLimitLabel.AutoSize = true;
            seatDepthSLimitLabel.Location = new Point(278, 174);
            seatDepthSLimitLabel.Name = "seatDepthSLimitLabel";
            seatDepthSLimitLabel.Size = new Size(119, 20);
            seatDepthSLimitLabel.TabIndex = 17;
            seatDepthSLimitLabel.Text = "от 20 до 100 мм";
            // 
            // footrestDiameterD2LimitLabel
            // 
            footrestDiameterD2LimitLabel.AutoSize = true;
            footrestDiameterD2LimitLabel.Location = new Point(278, 141);
            footrestDiameterD2LimitLabel.Name = "footrestDiameterD2LimitLabel";
            footrestDiameterD2LimitLabel.Size = new Size(111, 20);
            footrestDiameterD2LimitLabel.TabIndex = 16;
            footrestDiameterD2LimitLabel.Text = "от 10 до 50 мм";
            // 
            // legDiameterD1LimitLabel
            // 
            legDiameterD1LimitLabel.AutoSize = true;
            legDiameterD1LimitLabel.Location = new Point(278, 108);
            legDiameterD1LimitLabel.Name = "legDiameterD1LimitLabel";
            legDiameterD1LimitLabel.Size = new Size(111, 20);
            legDiameterD1LimitLabel.TabIndex = 15;
            legDiameterD1LimitLabel.Text = "от 25 до 70 мм";
            // 
            // seatDiameterDLimitLabel
            // 
            seatDiameterDLimitLabel.AutoSize = true;
            seatDiameterDLimitLabel.Location = new Point(278, 75);
            seatDiameterDLimitLabel.Name = "seatDiameterDLimitLabel";
            seatDiameterDLimitLabel.Size = new Size(127, 20);
            seatDiameterDLimitLabel.TabIndex = 14;
            seatDiameterDLimitLabel.Text = "от 300 до 500 мм";
            // 
            // footrestHeightH1LimitLabel
            // 
            footrestHeightH1LimitLabel.AutoSize = true;
            footrestHeightH1LimitLabel.Location = new Point(278, 42);
            footrestHeightH1LimitLabel.Name = "footrestHeightH1LimitLabel";
            footrestHeightH1LimitLabel.Size = new Size(127, 20);
            footrestHeightH1LimitLabel.TabIndex = 13;
            footrestHeightH1LimitLabel.Text = "от 200 до 400 мм";
            // 
            // stoolHeightHLimitLabel
            // 
            stoolHeightHLimitLabel.AutoSize = true;
            stoolHeightHLimitLabel.Location = new Point(278, 9);
            stoolHeightHLimitLabel.Name = "stoolHeightHLimitLabel";
            stoolHeightHLimitLabel.Size = new Size(127, 20);
            stoolHeightHLimitLabel.TabIndex = 12;
            stoolHeightHLimitLabel.Text = "от 700 до 900 мм";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Чертеж_барный_стул;
            pictureBox1.Location = new Point(411, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(203, 318);
            pictureBox1.TabIndex = 18;
            pictureBox1.TabStop = false;
            // 
            // BuildButton
            // 
            BuildButton.Location = new Point(278, 296);
            BuildButton.Name = "BuildButton";
            BuildButton.Size = new Size(127, 34);
            BuildButton.TabIndex = 19;
            BuildButton.Text = "Построить";
            BuildButton.UseVisualStyleBackColor = true;
            BuildButton.Click += BuildButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(richTextBox);
            groupBox1.Dock = DockStyle.Bottom;
            groupBox1.Location = new Point(0, 336);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(628, 112);
            groupBox1.TabIndex = 20;
            groupBox1.TabStop = false;
            groupBox1.Text = "Строка состояния";
            // 
            // richTextBox
            // 
            richTextBox.BackColor = SystemColors.Control;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.ForeColor = SystemColors.WindowText;
            richTextBox.Location = new Point(3, 23);
            richTextBox.Name = "richTextBox";
            richTextBox.ReadOnly = true;
            richTextBox.Size = new Size(622, 86);
            richTextBox.TabIndex = 0;
            richTextBox.Text = "";
            // 
            // stoolHeightHTextBox
            // 
            stoolHeightHTextBox.Location = new Point(187, 6);
            stoolHeightHTextBox.Name = "stoolHeightHTextBox";
            stoolHeightHTextBox.Size = new Size(85, 27);
            stoolHeightHTextBox.TabIndex = 21;
            // 
            // footrestHeightH1TextBox
            // 
            footrestHeightH1TextBox.Location = new Point(187, 39);
            footrestHeightH1TextBox.Name = "footrestHeightH1TextBox";
            footrestHeightH1TextBox.Size = new Size(85, 27);
            footrestHeightH1TextBox.TabIndex = 22;
            // 
            // legDiameterD1TextBox
            // 
            legDiameterD1TextBox.BackColor = SystemColors.HighlightText;
            legDiameterD1TextBox.Location = new Point(187, 105);
            legDiameterD1TextBox.Name = "legDiameterD1TextBox";
            legDiameterD1TextBox.Size = new Size(85, 27);
            legDiameterD1TextBox.TabIndex = 24;
            // 
            // seatDiameterDTextBox
            // 
            seatDiameterDTextBox.Location = new Point(187, 72);
            seatDiameterDTextBox.Name = "seatDiameterDTextBox";
            seatDiameterDTextBox.Size = new Size(85, 27);
            seatDiameterDTextBox.TabIndex = 23;
            // 
            // seatDepthSTextBox
            // 
            seatDepthSTextBox.Location = new Point(187, 174);
            seatDepthSTextBox.Name = "seatDepthSTextBox";
            seatDepthSTextBox.Size = new Size(85, 27);
            seatDepthSTextBox.TabIndex = 26;
            // 
            // footrestDiameterD2TextBox
            // 
            footrestDiameterD2TextBox.BackColor = SystemColors.Window;
            footrestDiameterD2TextBox.Location = new Point(187, 138);
            footrestDiameterD2TextBox.Name = "footrestDiameterD2TextBox";
            footrestDiameterD2TextBox.Size = new Size(85, 27);
            footrestDiameterD2TextBox.TabIndex = 25;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(12, 210);
            label13.Name = "label13";
            label13.Size = new Size(154, 20);
            label13.TabIndex = 27;
            label13.Text = "Количество ножек C:";
            // 
            // legCountCTextBox
            // 
            legCountCTextBox.Location = new Point(187, 207);
            legCountCTextBox.Name = "legCountCTextBox";
            legCountCTextBox.Size = new Size(85, 27);
            legCountCTextBox.TabIndex = 28;
            // 
            // legCountCLimitLabel
            // 
            legCountCLimitLabel.AutoSize = true;
            legCountCLimitLabel.Location = new Point(278, 210);
            legCountCLimitLabel.Name = "legCountCLimitLabel";
            legCountCLimitLabel.Size = new Size(91, 20);
            legCountCLimitLabel.TabIndex = 29;
            legCountCLimitLabel.Text = "от 3 до 6 шт";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(628, 448);
            Controls.Add(legCountCLimitLabel);
            Controls.Add(legCountCTextBox);
            Controls.Add(label13);
            Controls.Add(seatDepthSTextBox);
            Controls.Add(footrestDiameterD2TextBox);
            Controls.Add(legDiameterD1TextBox);
            Controls.Add(seatDiameterDTextBox);
            Controls.Add(footrestHeightH1TextBox);
            Controls.Add(stoolHeightHTextBox);
            Controls.Add(groupBox1);
            Controls.Add(BuildButton);
            Controls.Add(pictureBox1);
            Controls.Add(seatDepthSLimitLabel);
            Controls.Add(footrestDiameterD2LimitLabel);
            Controls.Add(legDiameterD1LimitLabel);
            Controls.Add(seatDiameterDLimitLabel);
            Controls.Add(footrestHeightH1LimitLabel);
            Controls.Add(stoolHeightHLimitLabel);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "Барный стул";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label seatDepthSLimitLabel;
        private Label footrestDiameterD2LimitLabel;
        private Label legDiameterD1LimitLabel;
        private Label seatDiameterDLimitLabel;
        private Label footrestHeightH1LimitLabel;
        private Label stoolHeightHLimitLabel;
        private PictureBox pictureBox1;
        private Button BuildButton;
        private GroupBox groupBox1;
        private RichTextBox richTextBox;
        private TextBox stoolHeightHTextBox;
        private TextBox footrestHeightH1TextBox;
        private TextBox legDiameterD1TextBox;
        private TextBox seatDiameterDTextBox;
        private TextBox seatDepthSTextBox;
        private TextBox footrestDiameterD2TextBox;
        private Label label13;
        private TextBox legCountCTextBox;
        private Label legCountCLimitLabel;
    }
}
