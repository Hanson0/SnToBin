namespace SnToBin
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSn = new System.Windows.Forms.TextBox();
            this.labSn = new System.Windows.Forms.Label();
            this.labMac = new System.Windows.Forms.Label();
            this.txtMac = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.stopWatcher1 = new MyControls.StopWatcher();
            this.resultCounter1 = new MyControls.ResultCounter();
            this.SuspendLayout();
            // 
            // txtSn
            // 
            this.txtSn.BackColor = System.Drawing.Color.White;
            this.txtSn.Font = new System.Drawing.Font("宋体", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSn.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtSn.Location = new System.Drawing.Point(220, 94);
            this.txtSn.Multiline = true;
            this.txtSn.Name = "txtSn";
            this.txtSn.ReadOnly = true;
            this.txtSn.Size = new System.Drawing.Size(508, 56);
            this.txtSn.TabIndex = 1;
            this.txtSn.TextChanged += new System.EventHandler(this.textSN_TextChanged);
            // 
            // labSn
            // 
            this.labSn.AutoSize = true;
            this.labSn.BackColor = System.Drawing.Color.White;
            this.labSn.Font = new System.Drawing.Font("宋体", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labSn.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labSn.Location = new System.Drawing.Point(241, 103);
            this.labSn.Name = "labSn";
            this.labSn.Size = new System.Drawing.Size(131, 38);
            this.labSn.TabIndex = 1;
            this.labSn.Text = "分配SN";
            // 
            // labMac
            // 
            this.labMac.AutoSize = true;
            this.labMac.BackColor = System.Drawing.Color.White;
            this.labMac.Font = new System.Drawing.Font("宋体", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labMac.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labMac.Location = new System.Drawing.Point(241, 43);
            this.labMac.Name = "labMac";
            this.labMac.Size = new System.Drawing.Size(150, 38);
            this.labMac.TabIndex = 5;
            this.labMac.Text = "标签MAC";
            // 
            // txtMac
            // 
            this.txtMac.BackColor = System.Drawing.Color.White;
            this.txtMac.Font = new System.Drawing.Font("宋体", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMac.Location = new System.Drawing.Point(220, 34);
            this.txtMac.Multiline = true;
            this.txtMac.Name = "txtMac";
            this.txtMac.Size = new System.Drawing.Size(508, 56);
            this.txtMac.TabIndex = 0;
            this.txtMac.TextChanged += new System.EventHandler(this.txtMac_TextChanged);
            // 
            // txtLog
            // 
            this.txtLog.Font = new System.Drawing.Font("新宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.Location = new System.Drawing.Point(220, 181);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(508, 266);
            this.txtLog.TabIndex = 6;
            // 
            // stopWatcher1
            // 
            this.stopWatcher1.Location = new System.Drawing.Point(12, 380);
            this.stopWatcher1.Name = "stopWatcher1";
            this.stopWatcher1.OverTimeMs = -1;
            this.stopWatcher1.Size = new System.Drawing.Size(183, 75);
            this.stopWatcher1.StopWatcherPath = "./SetUp.ini";
            this.stopWatcher1.TabIndex = 8;
            // 
            // resultCounter1
            // 
            this.resultCounter1.Cnt_faild = 0;
            this.resultCounter1.Cnt_pass = 0;
            this.resultCounter1.Location = new System.Drawing.Point(11, 181);
            this.resultCounter1.Name = "resultCounter1";
            this.resultCounter1.ResultPath = "./SetUp.ini";
            this.resultCounter1.Size = new System.Drawing.Size(203, 193);
            this.resultCounter1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 484);
            this.Controls.Add(this.resultCounter1);
            this.Controls.Add(this.stopWatcher1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.labMac);
            this.Controls.Add(this.txtMac);
            this.Controls.Add(this.labSn);
            this.Controls.Add(this.txtSn);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SnToBinV1.1.1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSn;
        private System.Windows.Forms.Label labSn;
        private System.Windows.Forms.Label labMac;
        private System.Windows.Forms.TextBox txtMac;
        private System.Windows.Forms.TextBox txtLog;
        private MyControls.StopWatcher stopWatcher1;
        private MyControls.ResultCounter resultCounter1;
    }
}

