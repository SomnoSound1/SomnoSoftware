namespace SomnoSoftware
{
    partial class View
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(View));
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.zedGraphAudio = new ZedGraph.ZedGraphControl();
            this.pb_spec = new System.Windows.Forms.PictureBox();
            this.timerDisconnect = new System.Windows.Forms.Timer(this.components);
            this.pb_activity = new System.Windows.Forms.PictureBox();
            this.pb_position = new System.Windows.Forms.PictureBox();
            this.tbData = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pb_logo = new System.Windows.Forms.PictureBox();
            this.pb_rec = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_activity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_position)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_rec)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonConnect.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Image = ((System.Drawing.Image)(resources.GetObject("buttonConnect.Image")));
            this.buttonConnect.Location = new System.Drawing.Point(901, 518);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(171, 70);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Verbindung herstellen";
            this.buttonConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonSave.Enabled = false;
            this.buttonSave.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Image = global::SomnoSoftware.Properties.Resources.logoRec;
            this.buttonSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSave.Location = new System.Drawing.Point(12, 518);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(202, 70);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Aufnahme starten";
            this.buttonSave.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonSave.UseVisualStyleBackColor = false;
            // 
            // zedGraphAudio
            // 
            this.zedGraphAudio.BackColor = System.Drawing.SystemColors.Control;
            this.zedGraphAudio.IsEnableHPan = false;
            this.zedGraphAudio.IsEnableHZoom = false;
            this.zedGraphAudio.IsEnableVPan = false;
            this.zedGraphAudio.IsEnableVZoom = false;
            this.zedGraphAudio.IsEnableWheelZoom = false;
            this.zedGraphAudio.Location = new System.Drawing.Point(12, 54);
            this.zedGraphAudio.Name = "zedGraphAudio";
            this.zedGraphAudio.ScrollGrace = 0D;
            this.zedGraphAudio.ScrollMaxX = 0D;
            this.zedGraphAudio.ScrollMaxY = 0D;
            this.zedGraphAudio.ScrollMaxY2 = 0D;
            this.zedGraphAudio.ScrollMinX = 0D;
            this.zedGraphAudio.ScrollMinY = 0D;
            this.zedGraphAudio.ScrollMinY2 = 0D;
            this.zedGraphAudio.Size = new System.Drawing.Size(1251, 253);
            this.zedGraphAudio.TabIndex = 3;
            // 
            // pb_spec
            // 
            this.pb_spec.BackColor = System.Drawing.SystemColors.Window;
            this.pb_spec.Location = new System.Drawing.Point(12, 222);
            this.pb_spec.Name = "pb_spec";
            this.pb_spec.Size = new System.Drawing.Size(1251, 290);
            this.pb_spec.TabIndex = 5;
            this.pb_spec.TabStop = false;
            // 
            // timerDisconnect
            // 
            this.timerDisconnect.Interval = 5000;
            // 
            // pb_activity
            // 
            this.pb_activity.BackColor = System.Drawing.SystemColors.Window;
            this.pb_activity.Location = new System.Drawing.Point(1269, 313);
            this.pb_activity.Name = "pb_activity";
            this.pb_activity.Size = new System.Drawing.Size(69, 290);
            this.pb_activity.TabIndex = 6;
            this.pb_activity.TabStop = false;
            // 
            // pb_position
            // 
            this.pb_position.BackColor = System.Drawing.SystemColors.Window;
            this.pb_position.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pb_position.Location = new System.Drawing.Point(1269, 54);
            this.pb_position.Name = "pb_position";
            this.pb_position.Size = new System.Drawing.Size(69, 253);
            this.pb_position.TabIndex = 7;
            this.pb_position.TabStop = false;
            // 
            // tbData
            // 
            this.tbData.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbData.Location = new System.Drawing.Point(542, 609);
            this.tbData.Multiline = true;
            this.tbData.Name = "tbData";
            this.tbData.ReadOnly = true;
            this.tbData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbData.Size = new System.Drawing.Size(370, 70);
            this.tbData.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Copperplate Gothic Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 26);
            this.label1.TabIndex = 10;
            this.label1.Text = "SomnoSoftware";
            // 
            // pb_logo
            // 
            this.pb_logo.BackgroundImage = global::SomnoSoftware.Properties.Resources.hs_ulm;
            this.pb_logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pb_logo.Location = new System.Drawing.Point(1138, 3);
            this.pb_logo.Name = "pb_logo";
            this.pb_logo.Size = new System.Drawing.Size(200, 45);
            this.pb_logo.TabIndex = 11;
            this.pb_logo.TabStop = false;
            // 
            // pb_rec
            // 
            this.pb_rec.BackColor = System.Drawing.SystemColors.Control;
            this.pb_rec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pb_rec.ImageLocation = "";
            this.pb_rec.InitialImage = null;
            this.pb_rec.Location = new System.Drawing.Point(308, 624);
            this.pb_rec.Name = "pb_rec";
            this.pb_rec.Size = new System.Drawing.Size(119, 46);
            this.pb_rec.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb_rec.TabIndex = 12;
            this.pb_rec.TabStop = false;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1084, 575);
            this.Controls.Add(this.pb_rec);
            this.Controls.Add(this.pb_logo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.pb_position);
            this.Controls.Add(this.pb_activity);
            this.Controls.Add(this.pb_spec);
            this.Controls.Add(this.zedGraphAudio);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonConnect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1100, 600);
            this.Name = "View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SomnoSoftware";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Resize += new System.EventHandler(this.View_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_activity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_position)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_rec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonSave;
        private ZedGraph.ZedGraphControl zedGraphAudio;
        private System.Windows.Forms.PictureBox pb_spec;
        private System.Windows.Forms.Timer timerDisconnect;
        private System.Windows.Forms.PictureBox pb_activity;
        private System.Windows.Forms.PictureBox pb_position;
        private System.Windows.Forms.TextBox tbData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pb_logo;
        private System.Windows.Forms.PictureBox pb_rec;
    }
}

