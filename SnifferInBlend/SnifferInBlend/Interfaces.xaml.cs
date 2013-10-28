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
	/// Interfaces.xaml 的交互逻辑
	/// </summary>
	public partial class Interfaces : Window
	{
		public Interfaces()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
		}

		private void Button_Start_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
            Models.Interface inter = ListView_Interfaces.SelectedItem as Models.Interface;
            Communication.Interface = inter;
            Communication.StartCapture(inter);
            this.Close();
		}

        private void Button_Close_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
	}
}