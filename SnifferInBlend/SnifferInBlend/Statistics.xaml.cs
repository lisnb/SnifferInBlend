using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using System.Data;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

using System.Timers;

namespace SnifferInBlend
{
	/// <summary>
	/// Statistics.xaml 的交互逻辑
	/// </summary>
	public partial class Statistics : Window
	{

        System.Timers.Timer TimerUpdateData;
		public Statistics()
		{
			this.InitializeComponent();
            PieChartSeries["PieLabelStyle"] = "Outside";
            PieChartSeries.Legend = ProtocolLegend.Name;
            //PrepareChart();
			
			// 在此点之下插入创建对象所需的代码。
		}


        public void StartUpdate()
        {
            TimerUpdateData = new System.Timers.Timer(1000);
            TimerUpdateData.Elapsed += TimerUpdateData_Elapsed;
            TimerUpdateData.Enabled = true;
            TimerUpdateData.AutoReset = true;
            TimerUpdateData.Start();
        }

        public void Update()
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                lock (Communication.StatisticLock)
                {
                    PieChartSeries.Points.DataBind(Communication.ProtocalStatistics, "Key", "Value", "LegendText=Key");
                }
            });
        }

        public void StopUpdate()
        {
            TimerUpdateData.Stop();
        }

        void TimerUpdateData_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            lock (Communication.StatisticLock)
            {
                PieChartSeries.Points.DataBind(Communication.ProtocalStatistics, "Key", "Value", "LegendText=Key");
            }
        }
	}
}