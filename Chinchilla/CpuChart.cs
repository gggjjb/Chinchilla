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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Threading;

namespace Chinchilla
{
    class CpuChart : Basechart
    {
        public String charttype = "cpu";
        public override double getData(string package)
        {
            double cpudata = 0;
            string cpuinfo;
            Executecmd.ExecuteCommandSync("adb shell dumpsys cpuinfo",out cpuinfo);
           
            Regex cpuReg = new Regex(@"\s*(\d*)%.*"+package+":");
            Match cpuM = cpuReg.Match(cpuinfo);
            if (cpuM.Groups.Count > 1)
            {
                cpudata =  Convert.ToDouble(cpuM.Groups[1].ToString());
            }

            this.currentData = cpudata;
            return cpudata;
        }

        public CpuChart(Dispatcher p, ChartPlotter newchart, Dictionary<string, string> packagelist):base(p, newchart, packagelist)
        {
        }
    }
}
