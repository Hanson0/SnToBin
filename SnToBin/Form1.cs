using CmdHelp;
using Production;
using Production.AllForms;
using Production.ProductionTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows;

namespace SnToBin
{
    public partial class Form1 : Form
    {
        private static string configPath = ConfigInfo.ConfigPath;            //配置文件路径

        StringBuilder tempStringBuilder = new StringBuilder();      //全局可变字符串实例，用于读取配置文件内容
        CmdProcess cmdProcess = new CmdProcess();   //cmd类
        private string strCurrentDirectory;                         //应用程序路径
        private string strBinPath;                      //生成bin的路径
        private string strLogPath;                      //生成Log的路径


        private string espApplicationPath;

        private string readToolFolderName;//flash读取软件名称
        private string readToolAppName;
        private int waitintTime;
        private int checkSn;



        private string comPort;

        private bool isDiff;
        private bool hasSn;


        public enum SystemType
        {
            Offline = 0,
            GSMMES = 1,
            iMES = 2,
        }
        public static SystemType Type
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("System", "Type", "", stringBuilder, 256, configPath);
                return (SystemType)Enum.Parse(typeof(SystemType), stringBuilder.ToString());
            }
        }


        public string ReadToolFolderName
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("SoftWare", "ReadToolFolderName", "", stringBuilder, 256, strCurrentDirectory);
                readToolFolderName = stringBuilder.ToString();
                return readToolFolderName;
            }
        }
        public string ReadToolAppName
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("SoftWare", "ReadToolAppName", "", stringBuilder, 256, strCurrentDirectory);
                readToolAppName = stringBuilder.ToString();
                return readToolAppName;
            }
        }

        public string ComPort
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("SerialPort", "COM", "", stringBuilder, 256, strCurrentDirectory);
                comPort = stringBuilder.ToString();
                return comPort;
            }

        }
        /// <summary>
        /// 扫码后的等待时间
        /// </summary>
        public int WaitintTime
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("Time", "WaitingTime", "", stringBuilder, 256, strCurrentDirectory);
                try
                {
                    waitintTime = int.Parse(stringBuilder.ToString());
                }
                catch (Exception)
                {
                    waitintTime = 5000;
                    return waitintTime;
                }
                return waitintTime;
            }
        }

        public int CheckSn
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("TestItem", "CheckSnHasWrite", "", stringBuilder, 256, strCurrentDirectory);
                try
                {
                    checkSn = int.Parse(stringBuilder.ToString());
                }
                catch (Exception)
                {
                    checkSn = 1;
                    return checkSn;
                }
                return checkSn;
            }
        }



        public Form1()
        {
            InitializeComponent();
            InitGlobleVariable();
        }
        public void InitGlobleVariable()
        {
            strCurrentDirectory = System.Environment.CurrentDirectory + "\\SetUp.ini";
            strBinPath = System.Environment.CurrentDirectory + "\\BinFile\\";
            if (!Directory.Exists(strBinPath))
            {
                Directory.CreateDirectory(strBinPath);
            }
            strLogPath = System.Environment.CurrentDirectory + "\\Log\\";
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }


            Win32API.GetPrivateProfileString("Path", "EspApplication", "", tempStringBuilder, 256, strCurrentDirectory);
            espApplicationPath = tempStringBuilder.ToString();

        }
        private void txtMac_TextChanged(object sender, EventArgs e)
        {
            labMac.Visible = txtMac.TextLength < 1;
            int macLength = 12;

            if (txtMac.Text.Length == macLength)
            {
                //合法性检测
                string pattern = @"[0-9A-Z]{12}";
                if (string.IsNullOrEmpty(txtMac.Text) || !Regex.IsMatch(txtMac.Text, pattern))
                {
                    MessageBox.Show("IMEI号不合法");
                    //清除状态
                    ClearUILastTestState();
                    return;
                }

                //初始化测试状态
                txtMac.ReadOnly = true;
                ClearUILastTestState();

                //ProductionTestFactory productionTestFactory = ProductionTestFactory.GetProductionTestFactory(this);
                //开启线程
                Thread thread = new Thread(CheckProductionTestState);
                thread.IsBackground = true;
                thread.Start(txtMac.Text.Trim());
            }
            else if (txtMac.Text.Length > macLength)
            {
                //大于12时，截取12
                txtMac.Text = txtMac.Text.Substring(macLength);
                txtMac.Select(txtMac.Text.Length, 0);
            }









            //if (txtMac.Text.Length > 11)
            //{
            //    #region 开始写入
            //    stopWatcher1.Start();


            //    //TestTaskMain
            //    //执行流程
            //    int ret = TestTaskFlow();

            //    //结果判断
            //    //Result.ResultJudge resultJudge = Result.ResultJudge.GetResultJudge(frmMain);
            //    //if (ProductionInfo.Type == ProductionInfo.SystemType.iMES || ProductionInfo.Type == ProductionInfo.SystemType.GSMMES)
            //    //{
            //    //    resultJudge.Imei = labelImei;
            //    //    resultJudge.Sn = snRead;
            //    //    resultJudge.Eid = eidRead;
            //    //    resultJudge.Iccid = iccidRead;
            //    //}
            //    //resultJudge.PutResult(eidRead, ret);



            //    //int ret = TestFlow();
            //    if (ret != 0)
            //    {
            //        PutResult(txtSn.Text, -1);
            //    }
            //    else
            //    {
            //        PutResult(txtSn.Text, 0);
            //    }

            //    stopWatcher1.Stop();
            //    txtMac.Text = "";
            //    txtSn.Text = "";
            //    txtMac.Focus();
            //    #endregion
            //}


        }

        /// <summary>
        /// 循环检测测试线程状态并循环开启测试线程
        /// </summary>
        public void CheckProductionTestState(object labelMacIn)
        {
            //秒表伴随测试线程
            stopWatcher1.Start();
            //flow.StartTimeoutTimer();

            string labelMac = labelMacIn as string;

            //txtLog.Text = "Log:\r\n";
            //循环检测模块上电
            //flow.CheckModulePowerOn();

            //开始测试
            TestTaskMain(labelMac);
            //flow.StopTimeOutTimer();
            stopWatcher1.Stop();
            SetTextBoxReadOnly(EnumControlWidget.txtMac.ToString(), false);

            ////循环检测模块掉电
            //frmMain.DisplayLog("正在检测模块掉电，请拔下模块...\r\n");
            //flow.CheckModulePowerOff();
            //frmMain.DisplayLog("模块已拔出\r\n");
            //frmMain.ClearUILastTestState();
        }
        /// <summary>
        /// 任务入口及任务流执行后处理
        /// </summary>
        public void TestTaskMain(string labelMac)
        {
            //this.labelImei = labelImei;
            //runState = true;
            //flagCreateNewRow = false;

            //readWriteIdHandle.FlagDisplayUart = true;

            //执行流程
            int ret = TestTaskFlow();

            if (ret != 0)
            {
                PutResult(txtSn.Text, -1);
            }
            else
            {
                PutResult(txtSn.Text, 0);
            }

            //frmMain.DisplayLog(string.Format("标签IMEI：{0}\r\n", labelImei));
            //结果判断
            //Result.ResultJudge resultJudge = Result.ResultJudge.GetResultJudge(frmMain);
            //if (ProductionInfo.Type == ProductionInfo.SystemType.iMES || ProductionInfo.Type == ProductionInfo.SystemType.GSMMES)
            //{
            //    resultJudge.Imei = labelImei;
            //    resultJudge.Sn = snRead;
            //    resultJudge.Eid = eidRead;
            //    resultJudge.Iccid = iccidRead;
            //}
            //resultJudge.PutResult(eidRead, ret);

            //runState = false;
            //readWriteIdHandle.FlagDisplayUart = false;
        }



        private int TestTaskFlow()
        {
            int ret = -1;
            try
            {
                #region
                //离线查重MAC
                if (ProductionInfo.Type == ProductionInfo.SystemType.Offline)
                {

                    JiaHao.ExcelHelp.ExcelHelper excel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance();
                    int retCheck = excel.SearchImei(txtMac.Text);
                    if (retCheck != 0)
                    {
                        DisplayLog(string.Format("MAC查重: {0} 重复 FAIL\r\n", txtMac.Text));
                        //return ret;


                        //stopWatcher1.Stop();
                        return retCheck;

                    }
                    else
                    {
                        DisplayLog(string.Format("MAC查重: {0} 本机查重 PASS\r\n", txtMac.Text));
                    }
                }
                //分配SN
                JiaHao.ExcelHelp.ExcelHelper snExcel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance(true);

                string sn = snExcel.GetSn();//读最后一行
                if (string.IsNullOrEmpty(sn))
                {
                    DisplayLog(string.Format("分配SN: MAC:{0}分配SN失败 FAIL\r\n", txtMac.Text));
                    ret = -1;
                    return ret;
                }
                DisplayLog(string.Format("分配SN: {0}成功 PASS\r\n", sn));

                SetText(EnumControlWidget.txtSn.ToString(),sn,false);

                //txtSn.Text = sn;
                //txtSn.Refresh();

                string strTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string strBinFilePath = strBinPath + txtSn.Text.ToUpper() + "_" + strTimeNow + ".bin";
                FileStream fs = new FileStream(strBinFilePath, FileMode.Create);//@"C:\Users\ZJH\Desktop\test2.bin"
                BinaryWriter bw = new BinaryWriter(fs);
                //byte[] byteArray = StringToHex(textSN.Text);

                //

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(txtSn.Text);

                for (int i = 0; i < byteArray.Length; i++)
                {
                    bw.Write((byte)byteArray[i]);
                }
                bw.Close();
                fs.Close();

                StringBuilder SN = new StringBuilder(256);
                string path;

                IntPtr ptrTestSoftFormMain = FindWindow(espApplicationPath);

                IntPtr ptrChild1 = Win32API.FindWindowEx(ptrTestSoftFormMain, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_1 = Win32API.FindWindowEx(ptrChild1, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_2 = Win32API.FindWindowEx(ptrChild1, ptrChild1_1, null, null);//
                Win32API.SendMessage(ptrChild1_2, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();


                IntPtr ptrChild1_3S = Win32API.FindWindowEx(ptrChild1, ptrChild1_2, null, null);//

                IntPtr ptrChild1_3S_1 = Win32API.FindWindowEx(ptrChild1_3S, IntPtr.Zero, null, null);//

                IntPtr ptrChild1_3S_1_1 = Win32API.FindWindowEx(ptrChild1_3S_1, IntPtr.Zero, null, null);//

                IntPtr ptrChild1_3S_1_1_1 = Win32API.FindWindowEx(ptrChild1_3S_1_1, IntPtr.Zero, null, null);//

                IntPtr ptrChild3S_1_1_1_1text = Win32API.FindWindowEx(ptrChild1_3S_1_1_1, IntPtr.Zero, null, null);//

                if (ptrChild3S_1_1_1_1text.Equals(IntPtr.Zero))
                {
                    DisplayLog("烧录软件检查: 无法抓取到烧录软件路径栏 Edit FAIL\r\n");
                    return ret;
                }
                Win32API.SendMessage(ptrChild3S_1_1_1_1text, Win32API.WM_SETTEXT, 0, strBinFilePath);

                //抓取Button
                IntPtr ptrChild1_3S_3 = Win32API.FindWindowEx(ptrChild1_3S, IntPtr.Zero, null, "ESP FLASH DOWNLOAD TOOL V0.9.7");//
                Win32API.SendMessage(ptrChild1_3S_3, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1 = Win32API.FindWindowEx(ptrChild1_3S_3, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_3S_3_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1_1 = Win32API.FindWindowEx(ptrChild1_3S_3_1, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_3S_3_1_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1_1_5start = Win32API.FindWindowEx(ptrChild1_3S_3_1_1, IntPtr.Zero, null, "START");//
                Win32API.SendMessage(ptrChild1_3S_3_1_1_5start, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                if (ptrChild1_3S_3_1_1_5start.Equals(IntPtr.Zero))
                {
                    //MessageBox.Show("未抓取到START按键");
                    DisplayLog("烧录软件检查: 无法抓取到START按键 FAIL\r\n");
                    return ret;
                }
                if (CheckSn == 1)
                {
                    DisplayLog(string.Format("等待{0}s ....,请上电进入下载模式 进行查询SN\r\n", WaitintTime / 1000));
                    //扫码等待5s后，查询SN
                    Thread.Sleep(WaitintTime);
                    #region 查询产品是否有SN
                    // 读取SN生产bin文件
                    string strCmdReturn1;
                    DisplayLog("开始读取芯片0xc1000地址...\r\n");

                    int retCmd1 = cmdProcess.ExeCommand(ReadToolFolderName, ReadToolAppName, ComPort, out strCmdReturn1);
                    //使用工具读取addr并生成bin成功
                    if (retCmd1 != 0)
                    {

                        Process[] processes = Process.GetProcesses();  //创建并实例化一个操作进程的类：Process
                        foreach (var item in processes)
                        {
                            if (item.ProcessName == "read_bin_addr")
                            {
                                item.Close();
                                //item.Kill();
                                break;
                            }

                        }
                        DisplayLog("SN查询:读取地址失败 FAIL，提示:超过等待时间，设备需3次进入下载模式\r\n");
                        return retCmd1;
                        //goto END;//END 处理收尾:显示烧写结果,清空
                    }
                    DisplayLog("读取芯片0xc1000地址生成bin成功\r\n");
                    DisplayLog("开始读取bin中的SN...\r\n");
                    //获取bin文件路径
                    int po;
                    string idKeySubstr1 = "save data to : ";

                    if ((po = strCmdReturn1.IndexOf(idKeySubstr1)) >= 0)
                    {
                        string createdBinPath = strCmdReturn1.Substring(po + idKeySubstr1.Length);
                        //读取bin文件,前7个字节
                        createdBinPath = createdBinPath.Replace("./", "./" + ReadToolFolderName + "/");
                        byte[] readHex = ReadFile(createdBinPath);
                        //将读取与写入对比，一致则写入成功
                        string strRead = Encoding.UTF8.GetString(readHex);
                        //if (strRead != "")
                        //{

                        //}
                        DisplayLog("读取bin中的SN成功\r\n");

                        for (int i = 0; i < readHex.Length; i++)
                        {
                            if (readHex[i] != 0xFF)
                            {
                                hasSn = true;
                            }

                        }
                        if (hasSn)
                        {
                            hasSn = false;
                            DisplayLog("SN查询:" + string.Format("产品中已有SN:{0} FAIL\r\n", strRead));
                            return ret;
                        }
                        DisplayLog("SN查询:" + "产品中未写过SN PASS\r\n");

                    }
                    #endregion
                }

                DisplayLog(string.Format("等待{0}s ....,请上电进入下载模式,进行下载\r\n", WaitintTime / 1000));
                //扫码等待5s后点击Start按键
                Thread.Sleep(WaitintTime);
                //点击START按键
                Win32API.SendMessage(ptrChild1_3S_3_1_1_5start, Win32API.BM_CLICK, 0, "0");
                //需等烧录完成后，并按板子进入烧录模式，再启动读取软件，读取数据


                DisplayLog(string.Format("等待{0}s ....,请在下载完成后再次上电进入下载模式，进行校验\r\n", WaitintTime / 1000));
                Thread.Sleep(WaitintTime);
                // 读取SN生产bin文件
                #region
                DisplayLog("开始读取芯片0xc1000地址...\r\n");
                string strCmdReturn;
                int retCmd = cmdProcess.ExeCommand(ReadToolFolderName, ReadToolAppName, ComPort, out strCmdReturn);
                //使用工具读取addr并生成bin成功
                if (retCmd != 0)
                {

                    Process[] processes = Process.GetProcesses();  //创建并实例化一个操作进程的类：Process
                    foreach (var item in processes)
                    {
                        if (item.ProcessName == "read_bin_addr")
                        {
                            item.Close();
                            //item.Kill();
                            break;
                        }

                    }
                    DisplayLog("SN校验:校验失败，提示:超过等待时间，设备需2次进入下载模式\r\n");
                    return retCmd;
                    //goto END;//END 处理收尾:显示烧写结果,清空
                }
                DisplayLog("读取芯片0xc1000地址生成bin成功\r\n");
                DisplayLog("开始读取bin中的SN...\r\n");
                #endregion
                //获取bin文件路径
                int positon;
                string idKeySubstr = "save data to : ";

                if ((positon = strCmdReturn.IndexOf(idKeySubstr)) >= 0)
                {
                    string createdBinPath = strCmdReturn.Substring(positon + idKeySubstr.Length);
                    //读取bin文件,前7个字节
                    createdBinPath = createdBinPath.Replace("./", "./" + ReadToolFolderName + "/");
                    byte[] readHex = ReadFile(createdBinPath);

                    //将读取与写入对比，一致则写入成功
                    for (int i = 0; i < readHex.Length; i++)
                    {
                        if (readHex[i] != byteArray[i])
                        {
                            isDiff = true;
                        }

                    }
                    DisplayLog("读取bin中的SN成功\r\n");

                    if (isDiff)
                    {
                        isDiff = false;
                        DisplayLog("SN校验:" + txtSn.Text + "  写入错误,写入失败 FAIL\r\n");
                        return ret;
                    }
                    DisplayLog("SN校验:" + txtSn.Text + "  写入正确 PASS\r\n");

                    #region 离线 记录到Excel
                    if (ProductionInfo.Type == ProductionInfo.SystemType.Offline)
                    {
                        JiaHao.ExcelHelp.ExcelHelper excel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance();
                        excel.ExportExcelOneByOne(new JiaHao.ExcelHelp.ExcelHelper.TrayInfoInExcel()
                        {
                            num = 1,
                            mac = txtMac.Text,
                            sn = txtSn.Text,
                            cuatomerName = "厦门阳光"
                        });
                        //Log添加 写入Excel成功
                        DisplayLog("写入过站记录表: MAC:" + txtMac.Text + " SN:" + txtSn.Text + " PASS\r\n");

                        //删除最后一行SN
                        ret = snExcel.DeleteLastRow();
                        if (ret != 0)
                        {
                            DisplayLog(string.Format("删除SN: MAC({0})删除最后一行SN失败  FAIL\r\n", txtMac.Text));
                            return ret;
                        }
                        else
                        {
                            DisplayLog(string.Format("删除SN: MAC({0})删除最后一行SN成功  PASS\r\n", txtMac.Text));
                        }


                    }
                    #endregion
                    ret = 0;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(txtSn.Text + ex.Message);
                //txtSn.Text = "";
                return ret;
            }


            return ret;
        }

        /// <summary>
        /// 设置文本框的只读
        /// </summary>
        /// <param name="txtName"></param>
        /// <param name="isReadonly"></param>
        public void SetTextBoxReadOnly(string txtName, bool isReadonly)
        {
            if (!this.InvokeRequired)
            {
                object obj = this.GetType().GetField(txtName, BindingFlags.NonPublic
                     | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(this);
                if (obj != null && obj is TextBox)
                {
                    (obj as TextBox).ReadOnly = isReadonly;
                }

            }
            else
            {
                Action<string, bool> updateUI = (string txtName1, bool isReadonly1)
                     => SetTextBoxReadOnly(txtName1, isReadonly1);
                this.Invoke(updateUI, txtName, isReadonly);
                //updateUI.Invoke(txtName, isReadonly);
            }
        }




        private void textSN_TextChanged(object sender, EventArgs e)
        {
            labSn.Visible = txtSn.TextLength < 1;

            //if (txtSn.Text.Length > 13)
            //{
            //    //stopWatcher1.Start();
            //    ////
            //    //int result = TestFlow();
            //    //if (result != 0)
            //    //{
            //    //    PutResult(txtSn.Text, -1);
            //    //}
            //    //else
            //    //{
            //    //    PutResult(txtSn.Text, 0);
            //    //}
            //    //stopWatcher1.Stop();
            //    //txtMac.Text = "";
            //    //txtSn.Text = "";
            //    //txtMac.Focus();
            //}

        }
        private int TestFlow()
        {
            int ret = -1;
            string pattern = null;
            try
            {
                #region
                txtLog.Text = "Log:\r\n";
                txtLog.BackColor = Color.White;
                txtLog.Refresh();
                //开启线程
                Thread Procedure = new Thread(new ThreadStart(procedureThread));
                Procedure.IsBackground = false;
                Procedure.Start();


                //合法性检测
                pattern = @"[0-9A-Z]{12}";
                if (string.IsNullOrEmpty(txtMac.Text) || !Regex.IsMatch(txtMac.Text, pattern))
                {
                    DisplayLog(string.Format("MAC合法性检查: {0} 不合法 FAIL\r\n", txtMac.Text));
                    return ret;
                }
                //离线查重MAC
                if (ProductionInfo.Type == ProductionInfo.SystemType.Offline)
                {

                    JiaHao.ExcelHelp.ExcelHelper excel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance();
                    int retCheck = excel.SearchImei(txtMac.Text);
                    if (retCheck != 0)
                    {
                        DisplayLog(string.Format("MAC查重: {0} 重复 FAIL\r\n", txtMac.Text));
                        //return ret;


                        //stopWatcher1.Stop();
                        return retCheck;

                    }
                    else
                    {
                        DisplayLog(string.Format("MAC查重: {0} 本机查重 PASS\r\n", txtMac.Text));
                    }
                }
                //分配SN
                JiaHao.ExcelHelp.ExcelHelper snExcel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance(true);

                string sn = snExcel.GetSn();//读最后一行
                if (string.IsNullOrEmpty(sn))
                {
                    DisplayLog(string.Format("分配SN: MAC:{0}分配SN失败 FAIL\r\n", txtMac.Text));
                    ret = -1;
                    return ret;
                }
                DisplayLog(string.Format("分配SN: {0}成功 PASS\r\n", sn));

                txtSn.Text = sn;
                txtSn.Refresh();

                string strTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string strBinFilePath = strBinPath + txtSn.Text.ToUpper() + "_" + strTimeNow + ".bin";
                FileStream fs = new FileStream(strBinFilePath, FileMode.Create);//@"C:\Users\ZJH\Desktop\test2.bin"
                BinaryWriter bw = new BinaryWriter(fs);
                //byte[] byteArray = StringToHex(textSN.Text);

                //

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(txtSn.Text);

                for (int i = 0; i < byteArray.Length; i++)
                {
                    bw.Write((byte)byteArray[i]);
                }
                bw.Close();
                fs.Close();

                StringBuilder SN = new StringBuilder(256);
                string path;

                IntPtr ptrTestSoftFormMain = FindWindow(espApplicationPath);

                IntPtr ptrChild1 = Win32API.FindWindowEx(ptrTestSoftFormMain, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_1 = Win32API.FindWindowEx(ptrChild1, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_2 = Win32API.FindWindowEx(ptrChild1, ptrChild1_1, null, null);//
                Win32API.SendMessage(ptrChild1_2, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();


                IntPtr ptrChild1_3S = Win32API.FindWindowEx(ptrChild1, ptrChild1_2, null, null);//

                IntPtr ptrChild1_3S_1 = Win32API.FindWindowEx(ptrChild1_3S, IntPtr.Zero, null, null);//

                IntPtr ptrChild1_3S_1_1 = Win32API.FindWindowEx(ptrChild1_3S_1, IntPtr.Zero, null, null);//

                IntPtr ptrChild1_3S_1_1_1 = Win32API.FindWindowEx(ptrChild1_3S_1_1, IntPtr.Zero, null, null);//

                IntPtr ptrChild3S_1_1_1_1text = Win32API.FindWindowEx(ptrChild1_3S_1_1_1, IntPtr.Zero, null, null);//

                if (ptrChild3S_1_1_1_1text.Equals(IntPtr.Zero))
                {
                    DisplayLog("烧录软件检查: 无法抓取到烧录软件路径栏 Edit FAIL\r\n");
                    return ret;
                }
                Win32API.SendMessage(ptrChild3S_1_1_1_1text, Win32API.WM_SETTEXT, 0, strBinFilePath);

                //抓取Button
                IntPtr ptrChild1_3S_3 = Win32API.FindWindowEx(ptrChild1_3S, IntPtr.Zero, null, "ESP FLASH DOWNLOAD TOOL V0.9.7");//
                Win32API.SendMessage(ptrChild1_3S_3, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1 = Win32API.FindWindowEx(ptrChild1_3S_3, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_3S_3_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1_1 = Win32API.FindWindowEx(ptrChild1_3S_3_1, IntPtr.Zero, null, null);//
                Win32API.SendMessage(ptrChild1_3S_3_1_1, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                IntPtr ptrChild1_3S_3_1_1_5start = Win32API.FindWindowEx(ptrChild1_3S_3_1_1, IntPtr.Zero, null, "START");//
                Win32API.SendMessage(ptrChild1_3S_3_1_1_5start, Win32API.WM_GETTEXT, 100, SN);
                path = SN.ToString();

                if (ptrChild1_3S_3_1_1_5start.Equals(IntPtr.Zero))
                {
                    //MessageBox.Show("未抓取到START按键");
                    DisplayLog("烧录软件检查: 无法抓取到START按键 FAIL\r\n");
                    return ret;
                }
                if (CheckSn==1)
                {
                    DisplayLog(string.Format("等待{0}s ....,请上电进入下载模式 进行查询SN\r\n", WaitintTime / 1000));
                    //扫码等待5s后，查询SN
                    Thread.Sleep(WaitintTime);
                    #region 查询产品是否有SN
                    // 读取SN生产bin文件
                    string strCmdReturn1;
                    DisplayLog("开始读取芯片0xc1000地址...\r\n");

                    int retCmd1 = cmdProcess.ExeCommand(ReadToolFolderName, ReadToolAppName, ComPort, out strCmdReturn1);
                    //使用工具读取addr并生成bin成功
                    if (retCmd1 != 0)
                    {

                        Process[] processes = Process.GetProcesses();  //创建并实例化一个操作进程的类：Process
                        foreach (var item in processes)
                        {
                            if (item.ProcessName == "read_bin_addr")
                            {
                                item.Close();
                                //item.Kill();
                                break;
                            }

                        }
                        DisplayLog("SN查询:读取地址失败 FAIL，提示:超过等待时间，设备需3次进入下载模式\r\n");
                        return retCmd1;
                        //goto END;//END 处理收尾:显示烧写结果,清空
                    }
                    DisplayLog("读取芯片0xc1000地址生成bin成功\r\n");
                    DisplayLog("开始读取bin中的SN...\r\n");
                    //获取bin文件路径
                    int po;
                    string idKeySubstr1 = "save data to : ";

                    if ((po = strCmdReturn1.IndexOf(idKeySubstr1)) >= 0)
                    {
                        string createdBinPath = strCmdReturn1.Substring(po + idKeySubstr1.Length);
                        //读取bin文件,前7个字节
                        createdBinPath = createdBinPath.Replace("./", "./" + ReadToolFolderName + "/");
                        byte[] readHex = ReadFile(createdBinPath);
                        //将读取与写入对比，一致则写入成功
                        string strRead = Encoding.UTF8.GetString(readHex);
                        //if (strRead != "")
                        //{

                        //}
                        DisplayLog("读取bin中的SN成功\r\n");

                        for (int i = 0; i < readHex.Length; i++)
                        {
                            if (readHex[i] != 0xFF)
                            {
                                hasSn = true;
                            }

                        }
                        if (hasSn)
                        {
                            hasSn = false;
                            DisplayLog("SN查询:" + string.Format("产品中已有SN:{0} FAIL\r\n", strRead));
                            return ret;
                        }
                        DisplayLog("SN查询:" + "产品中未写过SN PASS\r\n");

                    }
                    #endregion
                }

                DisplayLog(string.Format("等待{0}s ....,请上电进入下载模式,进行下载\r\n", WaitintTime / 1000));
                //扫码等待5s后点击Start按键
                Thread.Sleep(WaitintTime);
                //点击START按键
                Win32API.SendMessage(ptrChild1_3S_3_1_1_5start, Win32API.BM_CLICK, 0, "0");
                //需等烧录完成后，并按板子进入烧录模式，再启动读取软件，读取数据


                DisplayLog(string.Format("等待{0}s ....,请在下载完成后再次上电进入下载模式，进行校验\r\n", WaitintTime / 1000));
                Thread.Sleep(WaitintTime);
                // 读取SN生产bin文件
                #region
                DisplayLog("开始读取芯片0xc1000地址...\r\n");
                string strCmdReturn;
                int retCmd = cmdProcess.ExeCommand(ReadToolFolderName, ReadToolAppName, ComPort, out strCmdReturn);
                //使用工具读取addr并生成bin成功
                if (retCmd != 0)
                {

                    Process[] processes = Process.GetProcesses();  //创建并实例化一个操作进程的类：Process
                    foreach (var item in processes)
                    {
                        if (item.ProcessName == "read_bin_addr")
                        {
                            item.Close();
                            //item.Kill();
                            break;
                        }

                    }
                    DisplayLog("SN校验:校验失败，提示:超过等待时间，设备需2次进入下载模式\r\n");
                    return retCmd;
                    //goto END;//END 处理收尾:显示烧写结果,清空
                }
                DisplayLog("读取芯片0xc1000地址生成bin成功\r\n");
                DisplayLog("开始读取bin中的SN...\r\n");
                #endregion
                //获取bin文件路径
                int positon;
                string idKeySubstr = "save data to : ";

                if ((positon = strCmdReturn.IndexOf(idKeySubstr)) >= 0)
                {
                    string createdBinPath = strCmdReturn.Substring(positon + idKeySubstr.Length);
                    //读取bin文件,前7个字节
                    createdBinPath = createdBinPath.Replace("./", "./" + ReadToolFolderName + "/");
                    byte[] readHex = ReadFile(createdBinPath);

                    //将读取与写入对比，一致则写入成功
                    for (int i = 0; i < readHex.Length; i++)
                    {
                        if (readHex[i] != byteArray[i])
                        {
                            isDiff = true;
                        }

                    }
                    DisplayLog("读取bin中的SN成功\r\n");

                    if (isDiff)
                    {
                        isDiff = false;
                        DisplayLog("SN校验:" + txtSn.Text + "  写入错误,写入失败 FAIL\r\n");
                        return ret;
                    }
                    DisplayLog("SN校验:" + txtSn.Text + "  写入正确 PASS\r\n");

                    #region 离线 记录到Excel
                    if (ProductionInfo.Type == ProductionInfo.SystemType.Offline)
                    {
                            JiaHao.ExcelHelp.ExcelHelper excel = JiaHao.ExcelHelp.ExcelHelper.GetExcelHelperInstance();
                            excel.ExportExcelOneByOne(new JiaHao.ExcelHelp.ExcelHelper.TrayInfoInExcel()
                            {
                                num = 1,
                                mac = txtMac.Text,
                                sn = txtSn.Text,
                                cuatomerName = "厦门阳光"
                            });
                            //Log添加 写入Excel成功
                            DisplayLog("写入过站记录表: MAC:" + txtMac.Text + " SN:" + txtSn.Text + " PASS\r\n");

                            //删除最后一行SN
                            ret = snExcel.DeleteLastRow();
                            if (ret != 0)
                            {
                                DisplayLog(string.Format("删除SN: MAC({0})删除最后一行SN失败  FAIL\r\n", txtMac.Text));
                                return ret;
                            }
                            else
                            {
                                DisplayLog(string.Format("删除SN: MAC({0})删除最后一行SN成功  PASS\r\n", txtMac.Text));
                            }


                    }
                    #endregion
                    ret = 0;
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(txtSn.Text + ex.Message);
                txtSn.Text = "";
                return ret;
            }
            return ret;

        }

        private void procedureThread()
        {
            throw new NotImplementedException();
        }

        public void PutResult(string un, int result)
        {
            StringBuilder log = new StringBuilder();

            if (result == 0)
            {
                log.Append("\r\n");
                log.Append("########     ###     ######   ######\r\n");
                log.Append("##     ##   ## ##   ##    ## ##    ##\r\n");
                log.Append("##     ##  ##   ##  ##       ##\r\n");
                log.Append("########  ##     ##  ######   ######\r\n");
                log.Append("##        #########       ##       ##\r\n");
                log.Append("##        ##     ## ##    ## ##    ##\r\n");
                log.Append("##        ##     ##  ######   ######\r\n");

                SetTextBoxColor(EnumControlWidget.txtLog.ToString(),
                    Color.Green);
                //txtLog.BackColor = Color.Green;
                resultCounter1.Cnt_pass = resultCounter1.Cnt_pass+1;

            }
            else
            {
                log.Append("\r\n");
                log.Append("########    ###     ####  ##\r\n");
                log.Append("##         ## ##     ##   ##\r\n");
                log.Append("##        ##   ##    ##   ##\r\n");
                log.Append("######   ##     ##   ##   ##\r\n");
                log.Append("##       #########   ##   ##\r\n");
                log.Append("##       ##     ##   ##   ##\r\n");
                log.Append("##       ##     ##  ####  ########\r\n");

                //txtLog.BackColor = Color.Red;
                SetTextBoxColor(EnumControlWidget.txtLog.ToString(),
    Color.Red);

                resultCounter1.Cnt_faild++;

            }

            this.DisplayLog(log.ToString());
            if (ProductionInfo.Type == ProductionInfo.SystemType.iMES)
            {
                string logTest = ReadLog();
                FactoryAuto.CommonFunc.Common.WriteLogForiMes(null, null, null, null, un, result, logTest);

                //WriteResult(un, result);
            }
            else
            {
                string logTest = ReadLog();
                FactoryAuto.CommonFunc.Common.WriteLogForiMes(null, null, null, null, un, result, logTest);

                //WritePassResult(un, result);
                //WriteResult(un, result);

            }
        }
        public void WritePassResult(string un, int result)
        {
            string logFileName = null;

            if (string.IsNullOrEmpty(un))
            {
                this.DisplayLog("唯一号为空，LOG文件生成异常");
                return;
            }

            if (result == 0)
            {
                logFileName = string.Format("{0}{1}_{2}_PASS.LOG", strLogPath, un.ToUpper(),
                    DateTime.Now.ToString("yyyyMMdd_hhmmss"));
            }
            else
            {
                logFileName = string.Format("{0}{1}_{2}_FAIL.LOG", strLogPath, un.ToUpper(),
                DateTime.Now.ToString("yyyyMMdd_hhmmss"));
            }

            //若有创建LOG路径，则创建
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }

            //读取UI控件文本信息
            string log = txtLog.Text;

            //将文本写入文件流，生成文件
            using (FileStream fs = new FileStream(logFileName, FileMode.OpenOrCreate))
            {
                byte[] byteWrite = Encoding.UTF8.GetBytes(log);
                fs.Write(byteWrite, 0, byteWrite.Length);
            }
        }

        public void WriteResult(string un, int result)
        {
            string logFileName = null;

            if (string.IsNullOrEmpty(un))
            {
                this.DisplayLog("唯一号为空，LOG文件生成异常");
                return;
            }

            if (result == 0)
            {
                logFileName = string.Format("{0}01_----{1}_{2}_pass.log", strLogPath, un.ToUpper(),
                    DateTime.Now.ToString("yyyyMMdd_hhmmss"));
            }
            else
            {
                logFileName = string.Format("{0}01_----{1}_{2}_fail.log", strLogPath, un.ToUpper(),
                DateTime.Now.ToString("yyyyMMdd_hhmmss"));
            }

            //若有创建LOG路径，则创建
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }

            //读取UI控件文本信息
            string log = txtLog.Text;

            //将文本写入文件流，生成文件
            using (FileStream fs = new FileStream(logFileName, FileMode.OpenOrCreate))
            {
                byte[] byteWrite = Encoding.UTF8.GetBytes(log);
                fs.Write(byteWrite, 0, byteWrite.Length);
            }
        }

        /// <summary>
        /// 输出Log
        /// </summary>
        /// <param name="log"></param>
        public void DisplayLog(string log)
        {
            string name = EnumControlWidget.txtLog.ToString();
            SetText(name, log, true);
        }
        /// <summary>
        /// 读取日志
        /// </summary>
        /// <returns></returns>
        public string ReadLog()
        {
            string textRead = string.Empty;

            if (!this.InvokeRequired)
            {
                textRead = string.Format("\r\n{0}", txtLog.Text);
            }
            else
            {
                Func<string> readUI = () => ReadLog();
                //textRead = readUI.Invoke();                   ???无线=限递归
                textRead = (string)this.Invoke(readUI);
            }

            return textRead;
        }

        /// <summary>
        /// 设置文本框文本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="isApend"></param>
        public void SetText(string name, string text, bool isApend)
        {

            if (!this.InvokeRequired)
            {
                object obj = this.GetType().GetField(name, BindingFlags.NonPublic
                     | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(this);
                if (obj != null)
                {
                    if (obj is TextBox)
                    {
                        if (!isApend)
                            (obj as TextBox).Text = text;
                        else
                            (obj as TextBox).AppendText(text);
                    }
                    else if (obj is Label)
                    {
                        (obj as Label).Text = text;
                    }
                    else
                    {
                        MessageBox.Show(string.Format("控件{0}不是TextBox类型或Label类型", name));
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("控件{0}不存在", name));
                }
            }
            else
            {
                Action<string, string, bool> updateUI = (string name1, string text1, bool isApend1)
                      => SetText(name1, text1, isApend1);
                this.Invoke(updateUI, name, text, isApend);
                //updateUI.Invoke(name, text, isApend);
            }
        }

        /// <summary>
        /// 测试流程开始时，清除上一次的测试状态
        /// </summary>
        public void ClearUILastTestState()
        {
            string name = EnumControlWidget.txtLog.ToString();
            SetText(name, string.Empty, false);
            SetTextBoxColor(name, Color.White);
            name = EnumControlWidget.txtSn.ToString();
            SetText(name, string.Empty, false);
            name = EnumControlWidget.txtMac.ToString();
            SetText(name, string.Empty, false);


        }

        #region BackColor

        /// <summary>
        /// 设置文本框颜色
        /// </summary>
        /// <param name="txtName"></param>
        /// <param name="color"></param>
        public void SetTextBoxColor(string txtName, Color color)
        {
            if (!this.InvokeRequired)
            {
                object obj = this.GetType().GetField(txtName, BindingFlags.NonPublic
                     | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(this);
                if (obj != null && obj is TextBox)
                {
                    (obj as TextBox).BackColor = color;
                }

            }
            else
            {
                Action<string, Color> updateUI = (string txtName1, Color color1)
                     => SetTextBoxColor(txtName1, color1);
                this.Invoke(updateUI, txtName, color);
                //updateUI.Invoke(txtName, color);
            }
        }

        #endregion


        public static byte[] StringToHex(string hexStr)
        {
            int i;
            int len;
            byte[] hexByte;

            len = hexStr.Length / 2;
            len = (hexStr.Length % 2 == 1) ? len + 1 : len;
            hexByte = new byte[len];

            for (i = 0; i < len; i++)
            {
                if ((i == len - 1) && (hexStr.Length % 2 == 1))
                {
                    hexByte[i] = Convert.ToByte(hexStr.Substring(i * 2, 1), 16);
                }
                else
                {
                    hexByte[i] = Convert.ToByte(hexStr.Substring(i * 2, 2), 16);
                }
            }
            return hexByte;
        }

        /// 读文件到byte[]
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static byte[] ReadFile(string fileName)
        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes(14);//(int)r.BaseStream.Length
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }


        /// <summary>
        /// 返回窗口名称的句柄
        /// </summary>
        /// <param name="formname">窗口的名称</param>
        /// <returns></returns>
        private static IntPtr FindWindow(string formname)
        {
            int retry = 3;
            IntPtr ptrTestSoftFormMain = IntPtr.Zero;
            for (int i = 0; i < retry; i++)
            {
                ptrTestSoftFormMain = Win32API.FindWindow(null, formname);
                if (ptrTestSoftFormMain != IntPtr.Zero)
                {
                    break;
                }
                Thread.Sleep(50);
            }
            return ptrTestSoftFormMain;
        }

        private void resultCounter1_Load(object sender, EventArgs e)
        {

        }

        //bool overtime = false;

        //private void stopWatcher1_OverTimeAlarm_1(object sender, string msg)
        //{

        //}




    }
}
