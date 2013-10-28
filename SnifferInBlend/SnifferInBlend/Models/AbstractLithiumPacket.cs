using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferInBlend.Models
{
    public  abstract  class AbstractLithiumPacket
    {
        public AbstractLithiumPacket()
        {
            this.Background_Selected = System.Windows.Media.Brushes.DarkBlue;
            this.Foreground_Selected = System.Windows.Media.Brushes.Wheat;
            this.Background = System.Windows.Media.Brushes.AliceBlue;
            this.Foreground = System.Windows.Media.Brushes.Black ;
            this.Info = "Packet";
            this.Filters = new List<string>();
        }

        public virtual System.Windows.Controls.TreeViewItem GetViewItem() { return new System.Windows.Controls.TreeViewItem();}

        List<string> _Filters;

        public List<string> Filters
        {
            get { return _Filters; }
            set { _Filters = value; }
        }

        string _Info;

        public string Info
        {
            get { return _Info; }
            set { _Info = value; }
        }

        private System.Windows.Media.Brush _Foreground;

        public System.Windows.Media.Brush Foreground
        {
            get { return _Foreground; }
            set { _Foreground = value; }
        }

        private System.Windows.Media.Brush _Background;

        public System.Windows.Media.Brush Background
        {
            get { return _Background; }
            set { _Background = value; }
        }

        private System.Windows.Media.Brush _Background_Selected;

        public System.Windows.Media.Brush Background_Selected
        {
            get { return _Background_Selected; }
            set { _Background_Selected = value; }
        }
        private System.Windows.Media.Brush _Foreground_Selected;

        public System.Windows.Media.Brush Foreground_Selected
        {
            get { return _Foreground_Selected; }
            set { _Foreground_Selected = value; }
        }
    }
}
