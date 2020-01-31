namespace WorldStressMapTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMaxVect = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPickRadius = new System.Windows.Forms.TextBox();
            this.textBoxFILTER = new System.Windows.Forms.TextBox();
            this.labelRAD = new System.Windows.Forms.Label();
            this.labelLONG = new System.Windows.Forms.Label();
            this.labelLAT = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPICK = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsbStress = new System.Windows.Forms.ToolStripButton();
            this.tsbNulls = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 130);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1102, 425);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox1.Resize += new System.EventHandler(this.pictureBox1_Resize);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.textBoxMaxVect);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxPickRadius);
            this.panel1.Controls.Add(this.textBoxFILTER);
            this.panel1.Controls.Add(this.labelRAD);
            this.panel1.Controls.Add(this.labelLONG);
            this.panel1.Controls.Add(this.labelLAT);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.trackBar3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1102, 105);
            this.panel1.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(991, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "header [!]value, etc";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(250, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Max Vectors used";
            // 
            // textBoxMaxVect
            // 
            this.textBoxMaxVect.Location = new System.Drawing.Point(348, 74);
            this.textBoxMaxVect.Name = "textBoxMaxVect";
            this.textBoxMaxVect.Size = new System.Drawing.Size(78, 20);
            this.textBoxMaxVect.TabIndex = 10;
            this.textBoxMaxVect.TextChanged += new System.EventHandler(this.textBoxPickRadius_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Stress Pick Radius (km)";
            // 
            // textBoxPickRadius
            // 
            this.textBoxPickRadius.Location = new System.Drawing.Point(137, 74);
            this.textBoxPickRadius.Name = "textBoxPickRadius";
            this.textBoxPickRadius.Size = new System.Drawing.Size(78, 20);
            this.textBoxPickRadius.TabIndex = 10;
            this.textBoxPickRadius.TextChanged += new System.EventHandler(this.textBoxPickRadius_TextChanged);
            // 
            // textBoxFILTER
            // 
            this.textBoxFILTER.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFILTER.Location = new System.Drawing.Point(86, 3);
            this.textBoxFILTER.Name = "textBoxFILTER";
            this.textBoxFILTER.Size = new System.Drawing.Size(899, 20);
            this.textBoxFILTER.TabIndex = 10;
            this.textBoxFILTER.TextChanged += new System.EventHandler(this.textBoxFILTER_TextChanged);
            this.textBoxFILTER.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFILTER_KeyPress);
            // 
            // labelRAD
            // 
            this.labelRAD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRAD.AutoSize = true;
            this.labelRAD.Location = new System.Drawing.Point(1195, 88);
            this.labelRAD.Name = "labelRAD";
            this.labelRAD.Size = new System.Drawing.Size(14, 13);
            this.labelRAD.TabIndex = 9;
            this.labelRAD.Text = " *";
            // 
            // labelLONG
            // 
            this.labelLONG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLONG.AutoSize = true;
            this.labelLONG.Location = new System.Drawing.Point(1195, 52);
            this.labelLONG.Name = "labelLONG";
            this.labelLONG.Size = new System.Drawing.Size(14, 13);
            this.labelLONG.TabIndex = 8;
            this.labelLONG.Text = " *";
            // 
            // labelLAT
            // 
            this.labelLAT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLAT.AutoSize = true;
            this.labelLAT.Location = new System.Drawing.Point(1195, 16);
            this.labelLAT.Name = "labelLAT";
            this.labelLAT.Size = new System.Drawing.Size(14, 13);
            this.labelLAT.TabIndex = 7;
            this.labelLAT.Text = " *";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Zoom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Filter";
            // 
            // trackBar3
            // 
            this.trackBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar3.AutoSize = false;
            this.trackBar3.Location = new System.Drawing.Point(70, 34);
            this.trackBar3.Maximum = 10000;
            this.trackBar3.Minimum = 10;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(1020, 30);
            this.trackBar3.TabIndex = 5;
            this.trackBar3.TickFrequency = 100;
            this.trackBar3.Value = 6000;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar_Scroll);
            this.trackBar3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Longitude";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Latitude";
            // 
            // trackBar2
            // 
            this.trackBar2.AutoSize = false;
            this.trackBar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar2.Location = new System.Drawing.Point(54, 0);
            this.trackBar2.Maximum = 18000;
            this.trackBar2.Minimum = -18000;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(1003, 33);
            this.trackBar2.TabIndex = 4;
            this.trackBar2.TickFrequency = 1000;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar_Scroll);
            this.trackBar2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar_MouseUp);
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.LargeChange = 500;
            this.trackBar1.Location = new System.Drawing.Point(0, 13);
            this.trackBar1.Maximum = 9000;
            this.trackBar1.Minimum = -9000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 412);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.TickFrequency = 1000;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar_Scroll);
            this.trackBar1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ibDOT.png");
            this.imageList1.Images.SetKeyName(1, "ting.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPICK,
            this.toolStripButton3,
            this.toolStripLabel1,
            this.tsbStress,
            this.tsbNulls,
            this.toolStripLabel2,
            this.toolStripDropDownButton1,
            this.toolStripButton2,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1102, 25);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonPICK
            // 
            this.toolStripButtonPICK.CheckOnClick = true;
            this.toolStripButtonPICK.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPICK.Image")));
            this.toolStripButtonPICK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPICK.Name = "toolStripButtonPICK";
            this.toolStripButtonPICK.Size = new System.Drawing.Size(118, 22);
            this.toolStripButtonPICK.Text = "Pick Stress Vector";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(166, 22);
            this.toolStripButton3.Text = "Clear Picked Stress Vectors";
            this.toolStripButton3.Click += new System.EventHandler(this.button2_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(25, 22);
            this.toolStripLabel1.Text = "      ";
            // 
            // tsbStress
            // 
            this.tsbStress.Checked = true;
            this.tsbStress.CheckOnClick = true;
            this.tsbStress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbStress.Image = ((System.Drawing.Image)(resources.GetObject("tsbStress.Image")));
            this.tsbStress.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStress.Name = "tsbStress";
            this.tsbStress.Size = new System.Drawing.Size(100, 22);
            this.tsbStress.Text = "Show Stresses";
            this.tsbStress.CheckedChanged += new System.EventHandler(this.toolStripButton1_CheckedChanged);
            // 
            // tsbNulls
            // 
            this.tsbNulls.Checked = true;
            this.tsbNulls.CheckOnClick = true;
            this.tsbNulls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbNulls.Image = ((System.Drawing.Image)(resources.GetObject("tsbNulls.Image")));
            this.tsbNulls.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNulls.Name = "tsbNulls";
            this.tsbNulls.Size = new System.Drawing.Size(125, 22);
            this.tsbNulls.Text = "Show Null Stresses";
            this.tsbNulls.CheckedChanged += new System.EventHandler(this.toolStripButton1_CheckedChanged);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(65, 22);
            this.toolStripButton2.Text = "Update";
            this.toolStripButton2.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(64, 22);
            this.toolStripButton1.Text = "Table...";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLabel2.Image")));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(55, 22);
            this.toolStripLabel2.Text = "Cities:";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripDropDownButton1.Items.AddRange(new object[] {
            "None",
            "Major",
            "All"});
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(75, 25);
            this.toolStripDropDownButton1.SelectedIndexChanged += new System.EventHandler(this.toolStripDropDownButton1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.trackBar1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(1057, 130);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(45, 425);
            this.panel2.TabIndex = 13;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.trackBar2);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 555);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1102, 33);
            this.panel3.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.Location = new System.Drawing.Point(1057, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 33);
            this.label5.TabIndex = 7;
            this.label5.Text = "     ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 588);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Stress Map tool";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelRAD;
        private System.Windows.Forms.Label labelLONG;
        private System.Windows.Forms.Label labelLAT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBoxFILTER;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonPICK;
        private System.Windows.Forms.ToolStripButton tsbStress;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton tsbNulls;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPickRadius;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMaxVect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripComboBox toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    }
}

