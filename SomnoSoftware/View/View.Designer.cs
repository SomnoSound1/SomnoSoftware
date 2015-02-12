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
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_activity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_position)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonConnect.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(12, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(90, 52);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Verbindung herstellen";
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonSave.Enabled = false;
            this.buttonSave.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(108, 12);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(111, 52);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Aufnahme starten";
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
            this.zedGraphAudio.Location = new System.Drawing.Point(12, 70);
            this.zedGraphAudio.Name = "zedGraphAudio";
            this.zedGraphAudio.ScrollGrace = 0D;
            this.zedGraphAudio.ScrollMaxX = 0D;
            this.zedGraphAudio.ScrollMaxY = 0D;
            this.zedGraphAudio.ScrollMaxY2 = 0D;
            this.zedGraphAudio.ScrollMinX = 0D;
            this.zedGraphAudio.ScrollMinY = 0D;
            this.zedGraphAudio.ScrollMinY2 = 0D;
            this.zedGraphAudio.Size = new System.Drawing.Size(720, 250);
            this.zedGraphAudio.TabIndex = 3;
            // 
            // pb_spec
            // 
            this.pb_spec.BackColor = System.Drawing.SystemColors.Window;
            this.pb_spec.Location = new System.Drawing.Point(12, 330);
            this.pb_spec.Name = "pb_spec";
            this.pb_spec.Size = new System.Drawing.Size(795, 250);
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
            this.pb_activity.Location = new System.Drawing.Point(738, 70);
            this.pb_activity.Name = "pb_activity";
            this.pb_activity.Size = new System.Drawing.Size(69, 250);
            this.pb_activity.TabIndex = 6;
            this.pb_activity.TabStop = false;
            // 
            // pb_position
            // 
            this.pb_position.BackColor = System.Drawing.SystemColors.Window;
            this.pb_position.Location = new System.Drawing.Point(622, 11);
            this.pb_position.Name = "pb_position";
            this.pb_position.Size = new System.Drawing.Size(185, 53);
            this.pb_position.TabIndex = 7;
            this.pb_position.TabStop = false;
            // 
            // tbData
            // 
            this.tbData.Location = new System.Drawing.Point(225, 11);
            this.tbData.Multiline = true;
            this.tbData.Name = "tbData";
            this.tbData.ReadOnly = true;
            this.tbData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbData.Size = new System.Drawing.Size(391, 53);
            this.tbData.TabIndex = 9;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(810, 596);
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.pb_position);
            this.Controls.Add(this.pb_activity);
            this.Controls.Add(this.pb_spec);
            this.Controls.Add(this.zedGraphAudio);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonConnect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(826, 634);
            this.Name = "View";
            this.Text = "SomnoSoftware";
            this.Load += new System.EventHandler(this.View_Load);
            this.Resize += new System.EventHandler(this.View_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_activity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_position)).EndInit();
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
    }
}

