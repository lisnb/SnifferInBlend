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
	/// PreFilters.xaml 的交互逻辑
	/// </summary>
	public partial class PreFilters : Window
	{
		public PreFilters()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
            //MessageBox.Show(Communication.DefaultFilters.Count.ToString ());
		}

        private void ListView_Filters_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<string, string> filter = (KeyValuePair <string,string>)ListView_Filters.SelectedItem;
            TextBox_FilterName.Text  = filter.Key;
            TextBox_FilterString.Text = filter.Value;
            //Grid_FilterPropery.DataContext = filter;
        }

        private void Button_OK_Click_1(object sender, RoutedEventArgs e)
        {
            Communication.PreFilter = TextBox_FilterString.Text ;
            this.Close();
        }

        private void Button_Cancel_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
	}
}