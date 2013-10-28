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
	/// PacketDetail.xaml 的交互逻辑
	/// </summary>
	public partial class PacketDetail : Window
	{
		public PacketDetail()
		{
			this.InitializeComponent();
			
			// 在此点之下插入创建对象所需的代码。
		}

		private void Button_Expand_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
		}

		private void Button_Expand_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
            Expand(this.TreeView_Packet,(bool)Button_Expand.IsChecked);
            this.Button_Expand.Content = (bool)this.Button_Expand.IsChecked ? "+" : "-";
		}

        void Expand(ItemsControl control, bool what)
        {
            if (control.Items.Count == 0)
            {
                return;
            }
            else
            {
                foreach (object o in control.Items)
                {
                    TreeViewItem tvi = (TreeViewItem)o;
                    tvi.IsExpanded = what;
                    Expand(tvi, what);
                }
            }
        }
	}
}