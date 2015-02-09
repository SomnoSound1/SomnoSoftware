﻿namespace SomnoSoftware
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
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.zedGraphAudio = new ZedGraph.ZedGraphControl();
            this.labelStatus = new System.Windows.Forms.Label();
            this.pb_spec = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(12, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Enabled = false;
            this.buttonSave.Location = new System.Drawing.Point(246, 12);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(91, 23);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Start Recording";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // zedGraphAudio
            // 
            this.zedGraphAudio.Location = new System.Drawing.Point(10, 70);
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
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(93, 17);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(136, 13);
            this.labelStatus.TabIndex = 4;
            this.labelStatus.Text = "Press to Connect to Sensor";
            // 
            // pb_spec
            // 
            this.pb_spec.BackColor = System.Drawing.SystemColors.Window;
            this.pb_spec.Location = new System.Drawing.Point(10, 330);
            this.pb_spec.Name = "pb_spec";
            this.pb_spec.Size = new System.Drawing.Size(720, 250);
            this.pb_spec.TabIndex = 5;
            this.pb_spec.TabStop = false;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 592);
            this.Controls.Add(this.pb_spec);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.zedGraphAudio);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonConnect);
            this.Name = "View";
            this.Text = "SomnoSoftware";
            this.Resize += new System.EventHandler(this.View_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pb_spec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonSave;
        private ZedGraph.ZedGraphControl zedGraphAudio;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.PictureBox pb_spec;
    }
}

