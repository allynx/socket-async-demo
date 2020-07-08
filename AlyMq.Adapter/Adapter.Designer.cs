namespace AlyMq
{
    partial class Adapter
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
            this.lbIp = new System.Windows.Forms.Label();
            this.lbPort = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.lvBroker = new System.Windows.Forms.ListView();
            this.chBroler = new System.Windows.Forms.ColumnHeader();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnAllSend = new System.Windows.Forms.Button();
            this.btnOnlySend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbIp
            // 
            this.lbIp.AutoSize = true;
            this.lbIp.Location = new System.Drawing.Point(9, 18);
            this.lbIp.Name = "lbIp";
            this.lbIp.Size = new System.Drawing.Size(20, 17);
            this.lbIp.TabIndex = 0;
            this.lbIp.Text = "Ip";
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Location = new System.Drawing.Point(235, 18);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(32, 17);
            this.lbPort.TabIndex = 1;
            this.lbPort.Text = "Port";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(30, 15);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(184, 23);
            this.txtIp.TabIndex = 2;
            this.txtIp.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(273, 15);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(56, 23);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "6060";
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(355, 15);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(75, 23);
            this.btnInit.TabIndex = 4;
            this.btnInit.Text = "Start";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // lvBroker
            // 
            this.lvBroker.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chBroler});
            this.lvBroker.FullRowSelect = true;
            this.lvBroker.GridLines = true;
            this.lvBroker.HideSelection = false;
            this.lvBroker.Location = new System.Drawing.Point(13, 56);
            this.lvBroker.Name = "lvBroker";
            this.lvBroker.Size = new System.Drawing.Size(417, 208);
            this.lvBroker.TabIndex = 5;
            this.lvBroker.UseCompatibleStateImageBehavior = false;
            this.lvBroker.View = System.Windows.Forms.View.Details;
            this.lvBroker.SelectedIndexChanged += new System.EventHandler(this.lvBroker_SelectedIndexChanged);
            // 
            // chBroler
            // 
            this.chBroler.Name = "chBroler";
            this.chBroler.Text = "Broker";
            this.chBroler.Width = 200;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 280);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtLog.Size = new System.Drawing.Size(417, 118);
            this.txtLog.TabIndex = 6;
            // 
            // txtMsg
            // 
            this.txtMsg.Enabled = false;
            this.txtMsg.Location = new System.Drawing.Point(13, 413);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(222, 23);
            this.txtMsg.TabIndex = 7;
            // 
            // btnAllSend
            // 
            this.btnAllSend.Enabled = false;
            this.btnAllSend.Location = new System.Drawing.Point(259, 413);
            this.btnAllSend.Name = "btnAllSend";
            this.btnAllSend.Size = new System.Drawing.Size(75, 23);
            this.btnAllSend.TabIndex = 8;
            this.btnAllSend.Text = "AllSend";
            this.btnAllSend.UseVisualStyleBackColor = true;
            this.btnAllSend.Click += new System.EventHandler(this.btnAllSend_Click);
            // 
            // btnOnlySend
            // 
            this.btnOnlySend.Enabled = false;
            this.btnOnlySend.Location = new System.Drawing.Point(354, 413);
            this.btnOnlySend.Name = "btnOnlySend";
            this.btnOnlySend.Size = new System.Drawing.Size(75, 23);
            this.btnOnlySend.TabIndex = 9;
            this.btnOnlySend.Text = "OnlySend";
            this.btnOnlySend.UseVisualStyleBackColor = true;
            this.btnOnlySend.Click += new System.EventHandler(this.btnOnlySend_Click);
            // 
            // Adapter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 450);
            this.Controls.Add(this.btnOnlySend);
            this.Controls.Add(this.btnAllSend);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lvBroker);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.lbPort);
            this.Controls.Add(this.lbIp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Adapter";
            this.Text = "Adapter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbIp;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.ListView lvBroker;
        private System.Windows.Forms.ColumnHeader chBroler;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Button btnAllSend;
        private System.Windows.Forms.Button btnOnlySend;
    }
}