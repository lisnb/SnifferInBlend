using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnifferInBlend
{
    public class Communication
    {
        public static MainWindow SiHongWindow;
        public static Models.Interface Interface;
        public static string CaptureFilter;
        public static Dictionary<string, string> DefaultFilters = new Dictionary<string, string>();
        public static string PreFilter = "";
        private static Dictionary<string, int> _ProtocalStatistics = new Dictionary<string, int>();

        public static object StatisticLock = new object();

        public static Dictionary<string, int> ProtocalStatistics
        {
            get { return Communication._ProtocalStatistics; }
            set { Communication._ProtocalStatistics = value; }
        }

        public static void StartCapture(Models.Interface inter)
        {
            Communication.SiHongWindow.StartCapture(inter);
        }
    }
}
