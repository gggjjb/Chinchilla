﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Threading;
using System.Threading;

namespace Chinchilla
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private Dictionary<string, string> selectedPackage = new Dictionary<string, string>();
        Dictionary<string, string> pkginfo = new Dictionary<string, string>();
        List<Basechart> testchart = new List<Basechart>();
        public List<string> threValue = new List<string>();
        //private DispatcherTimer timerSine;
        public delegate void DeleFunc();
        private ThresholdSetting thresholdSettingWindow;

        public Window2()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initTextBox();
            //updateListview();
            ThreadStart ts = new ThreadStart(updateListview);
            Thread newThread = new Thread(ts);
            newThread.Start();
            updateStausBar();
            testchart.Add(new DatausageChart(Dispatcher, this.chart_datausage, selectedPackage));
            testchart.Add(new CpuChart(Dispatcher, this.chart_cpu, selectedPackage));
            testchart.Add(new MemChart(Dispatcher, this.chart_mem, selectedPackage));
        }

        void initTextBox() {
            //this.textBox1.DataContextChanged += new DependencyPropertyChangedEventHandler(textBox1_DataContextChanged);
        }

        void textBox1_TextChanged(object sender, TextChangedEventArgs e) {
            this.listView1.Items.Clear();
            foreach (KeyValuePair<string, string> pkg in pkginfo) {
                if (pkg.Key.IndexOf(this.textBox1.Text)>= 0)
                {
                    this.listView1.Items.Add(pkg.Key);
                }
            }
        }

        void updateListview() {
            while (listView1.Items.Count == 0) { 
                pkginfo = DeviceInfoHelper.GetPackageInfo();
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                     new DeleFunc(updateListviewDelegate));
                Thread.Sleep(2000);
            }
        }

        void updateListviewDelegate() {

            foreach (KeyValuePair<string, string> pkg in pkginfo) {
                this.listView1.Items.Add(pkg.Key);
            }
        }
            

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 5) {
                listView1.SelectedItems.RemoveAt(5);
                MessageBox.Show("最多选择5个包监测","提示");
            }

            selectedPackage.Clear();
            foreach(var item in this.listView1.SelectedItems)
            {
                selectedPackage[item.ToString()] = pkginfo[item.ToString()];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPackage.Count == 0)
            {
                MessageBox.Show("至少选择1个包监测","提示");
            }else if (selectedPackage.Count > 5) {
                 MessageBox.Show("最多选择5个包监测","提示");
            }
            else
            {
                foreach (Basechart chart in testchart) {
                    chart.updatechart(selectedPackage);
                }
                //testchart.Clear();          
            }
        }

        private void updateStausBar()
        {
            DispatcherTimer timerSine = new DispatcherTimer();
            timerSine.Tick += new EventHandler(updateStatus);
            timerSine.Interval = new TimeSpan(0, 0, 5);
            timerSine.Start();
            
        }

        private void updateStatus(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count > 1) {
                return;
            }
            string selectedProc = "";
            foreach(var pkginfo in this.selectedPackage)
            {
                if (selectedProc !="")
                    selectedProc=selectedProc+";";
                selectedProc = selectedProc+ pkginfo.Key;
            }
            this.status_proname.Content = selectedProc;
            if (this.testchart.Count > 0)
            {
                this.status_datausage.Content = "流量:"+this.testchart[0].CurrentData.ToString("f2")+"KB";
                this.status_cpu.Content = "CPU:" + this.testchart[1].CurrentData+"%";
                this.status_mem.Content = "内存:" + this.testchart[2].CurrentData.ToString("f2") + "MB";
            }
            if (this.threValue.Count > 0)
            {
                this.status_datausage_thre.Content = "阈值:" + this.threValue[0] + "KB";
                this.status_cpu_thre.Content = "阈值:" + this.threValue[1] + "%";
                this.status_mem_thre.Content = "阈值:" + this.threValue[2] + "MB";

                if (this.testchart.Count > 0)
                {
                    if (this.testchart[0].CurrentData > Convert.ToDouble(this.threValue[0]))
                    {
                        this.status_datausage.Background = new SolidColorBrush(Colors.Red);
                    }

                    if (this.testchart[1].CurrentData > Convert.ToDouble(this.threValue[1]))
                    {
                        this.status_cpu.Background = new SolidColorBrush(Colors.Red);
                    }

                    if (this.testchart[2].CurrentData > Convert.ToDouble(this.threValue[2]))
                    {
                        this.status_mem.Background = new SolidColorBrush(Colors.Red);
                    }
                }
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.thresholdSettingWindow == null)
            {
                this.thresholdSettingWindow = new ThresholdSetting();
                this.thresholdSettingWindow.Owner = this;
                this.thresholdSettingWindow.Show();
            }
            else
            {
                this.thresholdSettingWindow.Show();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            MessageBox.Show("1.右键点击图标可保存图片及查看更多操作，滚轮可放大/缩小图片\n 2.目前阈值报警功能仅支持监测一个应用时使用\n3.beta版功能较少，有任何需求请联系邓呈亮&&何韡", "提示");
        }

    }
}