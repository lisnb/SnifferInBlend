using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SnifferInBlend.Models;
using System.Data;
using System.ComponentModel;
using SnifferInBlend.Models.Packets;
using System.Threading;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Management;
using System.Collections.Specialized;
using SharpPcap.AirPcap;
using SharpPcap.WinPcap;

namespace SnifferInBlend
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        List<LithiumPacket> AllPackets = new List<LithiumPacket>();
        Thread CollectThread;
        PcapDevice Device;
        List<Models.Interface> AllInterfaces;
        DateTime BeginTime;
        int PacketNumber=0;
        Statistics StatisticsWindow ;
        bool IsStatisticsWindowShow = false;
        bool IsMaximum = false;
        double WindowWidth = 0;
        double WindowHeight = 0;

        public MainWindow()
        {
            InitializeComponent();

            Communication.SiHongWindow = this;

            PrepareCommunicationsStatistics();

            PrepareInterfaces();

            PrepareDefaultFilters();


            //MessageBox.Show(Properties.Settings.Default.DefaultFilters["IP Only"]);


        }

        void PrepareWindows()
        {
            this.StatisticsWindow.Closed += StatisticsWindow_Closed;
        }

        void StatisticsWindow_Closed(object sender, EventArgs e)
        {
            this.IsStatisticsWindowShow = false;
            //throw new NotImplementedException();
        }

        void PrepareCommunicationsStatistics()
        {
            Communication.ProtocalStatistics["IP"] = 0;
            Communication.ProtocalStatistics["ARP"] = 0;
            Communication.ProtocalStatistics["ICMP"] = 0;
            Communication.ProtocalStatistics["UDP"] = 0;
            Communication.ProtocalStatistics["TCP"] = 0;
            Communication.ProtocalStatistics["HTTP"] = 0;
        }

        void PrepareInterfaces()
        {

            AllInterfaces = new List<Interface>();
            foreach (LibPcapLiveDevice apd in LibPcapLiveDeviceList.Instance) 
            {
                AllInterfaces.Add(new Interface(apd));
            }
            //AllInterfaces = new List<Models.Interface>();
            //foreach (LibPcapLiveDevice pd in LibPcapLiveDeviceList.Instance)
            //{
            //    AllInterfaces.Add(new Models.Interface(pd));
            //}
        }

        void PrepareDefaultFilters()
        {
            string filterString = Properties.Resources.DefaultFilters;
            string[] filters = filterString.Split('|');
            for (int i = 0; i < filters.Length ; i += 2)
            {
                Communication.DefaultFilters.Add(filters[i], filters[i + 1]);
            }
        }

        public void bravo(object o)
        {

            Device = (o as Models.Interface ).Device ;
            Device.OnPacketArrival += device_OnPacketArrival;
            int readTimeoutMillisecond = 1000;
            Device.Open(SharpPcap.DeviceMode.Promiscuous, readTimeoutMillisecond);

            Device.Filter = Communication.PreFilter;

            BeginTime = DateTime.Now;

            //if (this.IsStatisticsWindowShow)
            //{
            //    this.StatisticsWindow.StartUpdate();
            //    //this.Dispatcher.BeginInvoke((Action)delegate() { this.StatisticsWindow.StartUpdate(); });
            //}
            Device.StartCapture();


        }

        void device_OnPacketArrival(object sender, SharpPcap.CaptureEventArgs e)
        {
            //throw new NotImplementedException();
            PacketHandler(e.Packet);
        }

        void PacketHandler(SharpPcap.RawCapture packet)
        {
            PacketNumber++;
            LithiumPacket lithiumPacket = new LithiumPacket();
            lithiumPacket.Time = (packet.Timeval.Date.ToLocalTime() - BeginTime).TotalSeconds;
            lithiumPacket.Length = packet.Data.Length;
            lithiumPacket.Number = PacketNumber;

            var layer = packet.LinkLayerType;

            var innerpacket = PacketDotNet.Packet.ParsePacket(layer, packet.Data);


            //只抓因特网包，否则丢弃
            if (layer == LinkLayers.Ethernet)
            {

                var ethernetPacket = (PacketDotNet.EthernetPacket)innerpacket;
                if (ethernetPacket == null)
                    return;
                EthernetLithiumPacket ethPacket = new EthernetLithiumPacket(ethernetPacket);
                lithiumPacket.Source = ethPacket.Source ;
                lithiumPacket.Destination = ethPacket.Destination ;
                lithiumPacket.Protocal = Protocals.Ethernet.ToString();
                lithiumPacket.EthernetPacket = ethPacket;

                lithiumPacket.Filters.Add(string.Format("mac.src=={0}", lithiumPacket.Source));
                lithiumPacket.Filters.Add(string.Format("mac.dest=={0}", lithiumPacket.Destination));


                if (ethernetPacket.Type == EthernetPacketType.Arp)
                {
                    //如果是arp报文的话
                    ARPPacket arpPacket = innerpacket.Extract(typeof(ARPPacket)) as ARPPacket;
                    if (arpPacket != null)
                    {
                        ARPLithiumPacket a = new ARPLithiumPacket(arpPacket);
                        //if (a.Source == "000000000000" || a.Destination == "000000000000")
                        //  return;
                        lithiumPacket.Info = a.Operation ==ARPOperation.Request ? string.Format("who has {0}  Tell {1}", a.Target, a.Send) : string.Format("Reply  {0} at {1}", a.Target, a.Destination); //a.Info; //string.Format("Address Resolution Protocol ({0})", a.Operation);
                        lithiumPacket.Source = a.Source;
                        lithiumPacket.Destination = a.Destination;
                        lithiumPacket.Protocal = Protocals.ARP.ToString();
                        lithiumPacket.InnerPacket = a;
                        lithiumPacket.SetColor(a);

                        lithiumPacket.Filters.Add(Protocals.ARP.ToString().ToLower());



                    }
                }
                else if (ethernetPacket.Type == EthernetPacketType.IpV4 || ethernetPacket.Type == EthernetPacketType.IpV6)
                {
                    //如果是IP报文的话
                    IpPacket ipPacket = innerpacket.Extract(typeof(IpPacket)) as IpPacket;

                    if (ipPacket != null)
                    {
                        lithiumPacket.Protocal = Protocals.IP.ToString();
                        IPLithiumPacket ipp = new IPLithiumPacket(ipPacket);
                        lithiumPacket.InnerPacket = ipp;
                        lithiumPacket.Source = ipPacket.SourceAddress.ToString();
                        lithiumPacket.Destination = ipPacket.DestinationAddress.ToString();
                        IPProtocolType ipprotocal = ipPacket.Protocol;
                        lithiumPacket.Background = Brushes.Yellow;
                        lithiumPacket.Info = string.Format("From   {0}  to  {1}", lithiumPacket.Source, lithiumPacket.Destination);

                        lithiumPacket.Filters.Add(Protocals.IP.ToString().ToLower ());
                        lithiumPacket.Filters.Add(string.Format("ip.src=={0}", ipp.Source));
                        lithiumPacket.Filters.Add(string.Format("ip.dest=={0}", ipp.Destination));
                        lithiumPacket.Filters.Add(string.Format("ip=={0}", ipp.Source));
                        lithiumPacket.Filters.Add(string.Format("ip=={0}", ipp.Destination));


                        switch (ipprotocal)
                        {
                            case IPProtocolType.TCP:
                                {
                                    //如果是TCP报文的话
                                    TcpPacket tcpPacket = ipPacket.Extract(typeof(TcpPacket)) as TcpPacket;
                                    if (tcpPacket != null)
                                    {
                                        lithiumPacket.Filters.Add(Protocals.TCP.ToString().ToLower ());
                                        TCPLithiumPacket tcpP = new TCPLithiumPacket(tcpPacket);
                                        lithiumPacket.Protocal = IPProtocolType.TCP.ToString();
                                        lithiumPacket.SetColor(tcpP);
                                        ipp.InnerPacket = tcpP;
                                        ipp.InnerPacketProtocal = IPProtocolType.TCP;
                                        lithiumPacket.Info = string.Format("Source Port: {0} Destination Port: {1}", tcpP.Source, tcpP.Destination);


                                        HTTPLithiumPacket hlp=null;
                                        if (tcpP.Destination == 80)
                                        {
                                            //request
                                            byte[] data = tcpPacket.PayloadData;
                                            if (data.Length > 0)
                                            {
                                                string dataInString = System.Text.Encoding.Default.GetString(data);
                                                hlp = new HTTPRequestLithiumPacket(data);
                                            }
                                            else
                                            {
                                                hlp = new HTTPRequestLithiumPacket() { IsData = true,IsRequest =true };
                                            }
                                        }
                                        else if (tcpP.Source == 80)
                                        {
                                            //response
                                            byte[] data = tcpPacket.PayloadData;
                                            if (data.Length == 0)
                                            {
                                                hlp = new HTTPResponseLithiumPacket() { IsData = true,IsRequest =false };
                                            }
                                            else
                                            {
                                                hlp = new HTTPResponseLithiumPacket(data);
                                            }
                                        }

                                        if (hlp != null)
                                        {
                                            lithiumPacket.Protocols.Add(Protocals.HTTP.ToString().ToLower ());
                                            lithiumPacket.Info = string.Format("Hypertext Transfer protocol ({0})",hlp.IsData ?"data":(hlp.IsRequest ?"Request":"Response"));
                                            lithiumPacket.Protocal = Protocals.HTTP.ToString();
                                            tcpP.InnerProtocal = Protocals.HTTP.ToString();
                                            tcpP.InnerPacket = hlp;

                                            lithiumPacket.Filters.Add(Protocals.HTTP.ToString().ToLower ());

                                        }
                                    }
                                    break;
                                }
                            case IPProtocolType.UDP:
                                {
                                    UdpPacket udpPacket = ipPacket.Extract(typeof(UdpPacket)) as UdpPacket;
                                    if (udpPacket != null)
                                    {
                                        UDPLithiumPacket udpP = new UDPLithiumPacket(udpPacket);
                                        lithiumPacket.Protocal = IPProtocolType.UDP.ToString();
                                        lithiumPacket.SetColor(udpP);
                                        ipp.InnerPacket = udpP;
                                        ipp.InnerPacketProtocal = IPProtocolType.UDP;
                                        lithiumPacket.Info = string.Format("Source Port: {0} Destination Port: {1}", udpP.Source, udpP.Destination);
                                        lithiumPacket.Filters.Add(Protocals.UDP.ToString().ToLower ());
                                    }
                                    break;
                                }
                            case IPProtocolType.ICMP:
                                {
                                    ICMPv4Packet icmpPacket = ipPacket.Extract(typeof(ICMPv4Packet)) as ICMPv4Packet;
                                    if (icmpPacket != null)
                                    {
                                        ICMPLithiumPacket icmpP = new ICMPLithiumPacket(icmpPacket);
                                        lithiumPacket.Protocal = IPProtocolType.ICMP.ToString();
                                        lithiumPacket.SetColor(icmpP);
                                        ipp.InnerPacket = icmpP;
                                        ipp.InnerPacketProtocal = IPProtocolType.ICMP;

                                        lithiumPacket.Info = string.Format("Type:  {0},     id= {1} seq= {2}", icmpP.Type, icmpP.IdentifierBE, icmpP.SequenceBE);

                                        lithiumPacket.Filters.Add(Protocals.ICMP.ToString().ToLower ());

                                    }
                                    break;
                                }

                            case IPProtocolType.IGMP:
                                {
                                    lithiumPacket.Filters.Add("igmp");
                                    break;
                                }

                        }


                    }

                }


                lithiumPacket.GetData(packet.Data);

                AllPackets.Add(lithiumPacket);
                if (Communication.ProtocalStatistics.ContainsKey(lithiumPacket.Protocal))
                {
                    lock (Communication.StatisticLock)
                    {
                        Communication.ProtocalStatistics[lithiumPacket.Protocal]++;
                    }
                    if (this.PacketNumber % 100 == 0&&this.IsStatisticsWindowShow)
                    {
                        this.StatisticsWindow.Update();
                    }
                }


                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    ListView_Packets.Items.Add(lithiumPacket);
                    ListView_Packets.ScrollIntoView(lithiumPacket);
                });

            }
            else
            {
                PacketNumber--;
                return;
            }
        }

        private void ListView_Packets_Click_1(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                    SortDescriptionCollection sdc = ListView_Packets.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();

                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }
        }

        private void Button_StopCapture_Click_1(object sender, RoutedEventArgs e)
        {
            if (Device == null)
                return;
            if (Device.Started)
            {
                Device.StopCapture();
                Device.Close();
            }
            if (CollectThread.IsAlive)
            {
                CollectThread.Abort();
            }
        }

        private void ListView_Packets_SourceUpdated_1(object sender, DataTransferEventArgs e)
        {
            ListView_Packets.ScrollIntoView(e);
        }

        private string GetData(LithiumPacket packet)
        {
            int lineNumber = packet.Data.Length / 16 + 1;
            int digitNumber = string.Format("{0:x}", packet.Data.Length).Length + 1;
            //string offsetFormatString = new string('0', digitNumber);
            //offsetFormatString = "{0:" + offsetFormatString + "}";
            StringBuilder hex = new StringBuilder();
            StringBuilder ascii = new StringBuilder();
            string offset = "";
            byte currentByte;
            StringBuilder DataInString=new StringBuilder ();
            for (int i = 0; i < packet.Data.Length; i++)
            {
                currentByte = packet.Data[i];
                hex.Append(currentByte.ToString("X2")+" ");
                
                ascii.Append((currentByte <= 126 && currentByte >= 32) ? (char)currentByte : '.');
                if(i%16==15)
                {
                    offset = string.Format ("{0:x}",i-15);
                    offset = (new string('0', digitNumber - offset.Length )) + offset+"  ";
                    DataInString.Append (offset );
                    DataInString.Append (hex.ToString ()+"  ");
                    DataInString.Append (ascii.ToString ()+"\r\n");
                    hex.Clear ();
                    ascii.Clear ();
                }
            }

            return DataInString.ToString ();


                //string data = "";
                //string currentLine = "";

                //int lineNumber = packet.DataInString.Length / 16 + 1;

                //int digitNumber = string.Format("{0:x}", packet.DataInString.Length).Length;

                //string offsetFormat = new string('0', digitNumber);
                //offsetFormat ="{0:"+offsetFormat +"}";

                //string offset = "";
                //StringBuilder hex = new StringBuilder();
                //StringBuilder ascii = new StringBuilder();

                //for (int i = 0; i < packet.DataInString.Length; i += 16)
                //{
                //    hex.Clear();
                //    ascii.Clear();
                //    offset = string.Format(offsetFormat, i);
                //    for (int j = 0; j < 24; j += 2)
                //    {

                //    }
                //}



        }



        private void ListView_Packets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LithiumPacket packet = ListView_Packets.SelectedItem as LithiumPacket;
            Utils.SetDetail(TreeView_Packet, Paragraph_Data, packet);
            //if (packet != null)
            //{
            //    //MessageBox.Show(packet.Info);
            //    Paragraph_Data.Inlines.Clear();
            //    Run run = new Run(GetData(packet));
            //    Paragraph_Data.Inlines.Add(run);

            //    TreeView treeroot = TreeView_Packet;

            //    treeroot.Items.Clear();
            //    //ethernet
                
            //    treeroot.Items.Add(packet.EthernetPacket.GetViewItem());

            //    //ip
            //    if (packet.Protocal == "ARP")
            //    {
            //        ARPLithiumPacket arpp = (ARPLithiumPacket)packet.InnerPacket;
            //        treeroot.Items.Add(GetTreeView(arpp, IPProtocolType.NONE));
            //        //Grid_TreeViews.Children.Add(treeroot);
            //        return;

            //    }
            //    else
            //    {
            //        IPLithiumPacket ipp = (IPLithiumPacket)packet.InnerPacket;
            //        treeroot.Items.Add(GetTreeView(ipp, IPProtocolType.IP));
            //    }

            //    //transmisson
            //    IPLithiumPacket ippp = (IPLithiumPacket)packet.InnerPacket;
            //    if (ippp.InnerPacketProtocal == IPProtocolType.ICMP)
            //    {
            //        ICMPLithiumPacket icmpp = (ICMPLithiumPacket)ippp.InnerPacket;
            //        treeroot.Items.Add(GetTreeView(icmpp, IPProtocolType.ICMP));
            //    }
            //    else if (ippp.InnerPacketProtocal == IPProtocolType.TCP)
            //    {
            //        TCPLithiumPacket tcpp = (TCPLithiumPacket)ippp.InnerPacket;
            //        treeroot.Items.Add(GetTreeView(tcpp, IPProtocolType.TCP));

            //        if (tcpp.InnerProtocal == Protocals.HTTP.ToString())
            //        {
            //            HTTPLithiumPacket hlp = (HTTPLithiumPacket)tcpp.InnerPacket;
            //            if (hlp.IsRequest)
            //            {
            //                treeroot.Items.Add(GetTreeView((HTTPRequestLithiumPacket)hlp, IPProtocolType.NONE));
            //            }
            //            else
            //            {
            //                treeroot.Items.Add(GetTreeView((HTTPResponseLithiumPacket)hlp, IPProtocolType.NONE));
            //            }
            //        }

            //    }
            //    else if (ippp.InnerPacketProtocal == IPProtocolType.UDP)
            //    {
            //        UDPLithiumPacket udpp = (UDPLithiumPacket)ippp.InnerPacket;
            //        treeroot.Items.Add(GetTreeView(udpp, IPProtocolType.UDP));
            //    }


            //    //Grid_TreeViews.Children.Add(treeroot);
            //}
        }

        private TreeViewItem GetTreeView(AbstractLithiumPacket packet, IPProtocolType protocal)
        {



            TreeViewItem tvi = packet.GetViewItem();
            return tvi;
            //return null;
        }

        private void MenuItem_Interfaces_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            Interfaces interfaceWindow = new Interfaces();
            interfaceWindow.ListView_Interfaces.ItemsSource = AllInterfaces;
            interfaceWindow.Owner = this;
            interfaceWindow.Show();
            
        }

        private void Button_StarCapture_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。

            if (Communication.Interface != null)
            {
                StartCapture(Communication.Interface);
            }
            else
            {
                if (MessageBox.Show("Please choose an Interface first!") != MessageBoxResult.None)
                {
                    Interfaces i = new Interfaces();
                    i.ListView_Interfaces.ItemsSource = AllInterfaces;
                    i.Show();
                }
            }
           
        }

        public  void StartCapture(Models.Interface inter)
        {
            PacketNumber = 0;
            AllPackets.Clear();
            PrepareCommunicationsStatistics();
            ListView_Packets.Items.Clear();
            CollectThread = new Thread(new ParameterizedThreadStart(bravo));
            CollectThread.IsBackground = true;
            CollectThread.Start(inter);
            
        }

        public void ApplyPostFilter(string filter)
        {
            filter = filter.ToLower();
            if (filter != string.Empty)
            {
                this.ComboBox_Filter.Items.Add(filter);
            }
            string[] filters = filter.Split(new string[] { "or", "and", "not" }, StringSplitOptions.RemoveEmptyEntries);

            List<LithiumPacket> postPackets = new List<LithiumPacket> ();

            if (filter.Contains("not"))
            {
                var tempPackets = from p in this.AllPackets where !p.Filters.Contains(filters[0].Trim()) select p;
                postPackets = tempPackets.ToList<LithiumPacket>();
            }
            else if (filter.Contains("and"))
            {
                var tempPackets = from p in this.AllPackets where p.Filters.Contains(filters[0].Trim ()) && p.Filters.Contains(filters[1].Trim ()) select p;
                postPackets = tempPackets.ToList<LithiumPacket>();
            }
            else if (filter.Contains("or"))
            {
                var tempPackets = from p in this.AllPackets where p.Filters.Contains(filters[0].Trim ()) || p.Filters.Contains(filters[1].Trim ()) select p;
                postPackets = tempPackets.ToList<LithiumPacket>();
            }
            else if (filter != "")
            {
                var tempPackets = from p in this.AllPackets where p.Filters.Contains(filter.Trim ()) select p;
                postPackets = tempPackets.ToList<LithiumPacket>();
            }
            else
            {
                postPackets = AllPackets;
            }

            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                this.ListView_Packets.Items.Clear();
                foreach (LithiumPacket lp in postPackets)
                {
                    this.ListView_Packets.Items.Add(lp);
                }
            });
            
            //var postPackets = filter==""?AllPackets : from p in this.AllPackets where p.Filters.Contains(filter.ToUpper ()) select p;
            //this.Dispatcher.BeginInvoke((Action)delegate()
            //{
            //    this.ListView_Packets.Items.Clear();
            //    foreach(LithiumPacket lp in postPackets.ToList<LithiumPacket>())
            //    {
            //        this.ListView_Packets.Items.Add (lp);
            //    }
            //    //this.ListView_Packets.ItemsSource = postPackets.ToList<LithiumPacket >();
            //    //this.ListView_Packets.ItemsSource  = (from p in this.AllPackets where p.Protocols.Contains(filter.ToUpper()) select p).ToList();
            //});
            
        }

        private void ComboBox_Filter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(ComboBox_Filter.Background == Brushes.Yellow)
                {
                    ApplyPostFilter (this.ComboBox_Filter.Text.Trim () );
                }
            }
            else
            {
                if (Utils.ValidatePostFilter (ComboBox_Filter.Text ))
                {
                    if (ComboBox_Filter.Text.Trim() == "")
                    {
                        ComboBox_Filter.Background = Brushes.DarkBlue;

                    }
                    ComboBox_Filter.Background = Brushes.Yellow;
                }
                else
                {
                    ComboBox_Filter.Background = Brushes.Green;

                }
            }
        }

        private void MenuItem_CaptureFilters_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            PreFilters prefiterWindow = new PreFilters();
            prefiterWindow.Owner = this;
            prefiterWindow.ListView_Filters.ItemsSource = Communication.DefaultFilters ;
            prefiterWindow.ShowDialog();
        }

        private void MenuItem_Statistics_Click_1(object sender, RoutedEventArgs e)
        {
            this.IsStatisticsWindowShow = true;
            StatisticsWindow = new Statistics();
            StatisticsWindow.Show();
            StatisticsWindow.Update();
        }

        private void Button_FilterApply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            if (ComboBox_Filter.Background == Brushes.Yellow)
            {
                ApplyPostFilter(this.ComboBox_Filter.Text.Trim());
            }
        }

        private void Button_FilterClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            this.ComboBox_Filter.Text = string.Empty;
            ApplyPostFilter(this.ComboBox_Filter.Text.Trim());
        }

        private void SiHong_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
			base.OnMouseLeftButtonDown(e);
			this.DragMove();
        }

        private void Button_Minn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            this.WindowState = System.Windows.WindowState.Minimized;
            this.Focus();

        }

        private void Button_Max_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            this.IsMaximum = !this.IsMaximum;

            if (this.IsMaximum)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                this.Button_Max.Content = "品";
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.Button_Max.Content = "口";
            }
            

            

        }

        private void Button_Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            this.Close();
        }

        private void ListView_Packets_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            PacketDetail pd = new PacketDetail();
            LithiumPacket packet = this.ListView_Packets.SelectedItem as LithiumPacket;
            Utils.SetDetail(pd.TreeView_Packet, pd.Paragraph_Data, packet);
            pd.Show();
        }

        
    }
}
