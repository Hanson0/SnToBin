using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;

namespace MyControls
{
    public partial class StopWatcher : UserControl
    {
        private string stopWatcherPath = "./SetUp.ini";

        [Browsable(true), Description("设置结果保存的文件位置")]
        public string StopWatcherPath
        {
            get { return stopWatcherPath; }
            set { stopWatcherPath = value; }
        }

        [Browsable(true), Description("设置超时时间，-1表示不设置，以毫秒ms为单位")]
        public int otMs = -1;  //超时时间

        public int OverTimeMs
        {
            get 
            {
                try
                {
                    otMs = int.Parse(INIHelper.Read("Time", "OverTime", StopWatcherPath));
                }
                catch (Exception)
                {
                    otMs=30;
                    return otMs; 
                }
                return otMs; 
            }
            set
            {
                otMs = value;
                INIHelper.Write("Time", "OverTime", otMs.ToString(), StopWatcherPath);
            }
        }
        public delegate void OverTime(object sender, string msg);
        [Browsable(true), Description("超时时发生")]
        public event OverTime OverTimeAlarm;

        private bool isStarted = false;

        public bool IsStarted
        {
            get { return isStarted; }
        }
        private Stopwatch stopwatch = new Stopwatch();
        private System.Timers.Timer timer = new System.Timers.Timer();
        public StopWatcher()
        {
            InitializeComponent();
        }
        public void Start()
        {
            isStarted = true;
            InitTimer();
            stopwatch.Start();//计时的功能
            timer.Start();//定时器刷新界面的功能
            Invoked();
        }
        public void Stop()
        {
            isStarted = false;
            timer.Stop();
            stopwatch.Stop();
            stopwatch.Reset();//复位但不显示，在下一次启动后timer的事件中显示
            //Invoked ( );
        }
        public void Reset()
        {
            isStarted = false;
            stopwatch.Reset();
            timer.Stop();
            Invoked();
        }
        public void Restart()
        {
            isStarted = true;
            stopwatch.Restart();
            timer.Start();
            Invoked();
        }
        private void InitTimer()
        {
            timer.Stop();
            timer = new System.Timers.Timer();
            timer.Interval = 50;
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoked();
            if (OverTimeMs > 0 && stopwatch.Elapsed.TotalMilliseconds > OverTimeMs && OverTimeAlarm!=null)//超时才会调用这个事件——stopWatcher1_OverTimeAlarm(object sender, string msg)
            {
                OverTimeAlarm(this, "计数已超时");//事件的代理
            }
        }
        private void Invoked()
        {
            try
            {
                if (label_time.InvokeRequired)
                {
                    label_time.Invoke((EventHandler)delegate { Invoked(); });
                }
                else
                {
                    if (IsStarted == false) return;

                    label_time.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:fff");
                    if (OverTimeMs > 0 && stopwatch.Elapsed.TotalMilliseconds > OverTimeMs)
                    {
                        label_notice.Text = "超时";
                    }
                    else
                    {
                        label_notice.Text = "";
                    }
                }
            }
            catch
            {
            }
        }
    }
}
