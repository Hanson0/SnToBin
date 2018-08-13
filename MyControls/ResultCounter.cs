/*********************************************************************************
  *Copyright(C),2016-2019
  *FileName:  // 文件名
  *Author:  majianbo、zengjiahao
  *Version:  1.0
  *Date:  20180806
  *Description:  用于生产的计数器，统计通过，失败，通过率等信息
  *Others:  
  *Function List:  
  *History:   修改历史记录列表，每条修改记录应包含修改日期、修改者及修改内容简介
     1.Date:  20180806               Author:      majianbo 
     2.Date:  20180806               Author:      zengjiahao 
**********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyControls
{
    public partial class ResultCounter : UserControl
    {
        //Result 
        private int cnt_pass;
        private int cnt_faild;
        private int cnt_total;
        private string resultPath = "./SetUp.ini";


        #region 属性
        [Browsable(true), Description("设置结果保存的文件位置")]
        public string ResultPath
        {
            get { return resultPath; }
            set { resultPath = value; }
        }


        [Browsable(false), Description("设置通过的计数")]
        public int Cnt_pass
        {
            get { return cnt_pass; }
            set
            { 
                cnt_pass = value;
                INIHelper.Write("RESULT", "Pass", cnt_pass.ToString(), ResultPath);
                SetTotal();
            }
        }

        [Browsable(false), Description("设置失败的计数")]
        public int Cnt_faild
        {
            get { return cnt_faild; }
            set
            {
                cnt_faild = value;
                INIHelper.Write("RESULT", "Faild", cnt_faild.ToString(), ResultPath);
                SetTotal();
            }
        }
        #endregion

        /// <summary>
        /// 更新总数并刷新界面
        /// </summary>
        private void SetTotal()
        {
            cnt_total = cnt_pass + cnt_faild;
            UpdateResult();
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void UpdateResult()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((EventHandler)delegate { UpdateResult(); });
            }
            else
            {
                label_pass.Text = cnt_pass.ToString();
                label_faild.Text = cnt_faild.ToString();
                label_total.Text = cnt_total.ToString();
                label_percent.Text = ((float)100 * cnt_pass / (cnt_total)).ToString("0.#") + "%";
            }

        }

        public ResultCounter()
        {
            InitializeComponent();
            ReadResultFromIni();
            //InitGlobleVariable();
        }
        public void ReadResultFromIni()
        {
            Cnt_pass = INIHelper.GetInt32("RESULT", "Pass", ResultPath);
            Cnt_faild = INIHelper.GetInt32("RESULT", "Faild", ResultPath);
        }
        /// <summary>
        /// 保存result 信息到ini文件
        /// </summary>
        /// <returns></returns>
        public void SaveResultToIni()
        {
            INIHelper.Write("RESULT", "Pass", Cnt_pass.ToString(), ResultPath);
            INIHelper.Write("RESULT", "Faild", Cnt_faild.ToString(), ResultPath);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("清除统计数据，请确认？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.Yes)
            {
                Cnt_pass = 0;
                Cnt_faild = 0;
            }
        }


    }


}
