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

namespace SnifferInBlend
{
	/// <summary>
	/// SplashWindow.xaml 的交互逻辑
	/// </summary>
	public partial class SplashWindow : Window
	{
		public SplashWindow()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
            
		}

		private void calendarDayButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
		}

	}
}