using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using PacketDotNet;
using SnifferInBlend.Models;
using System.Windows.Documents;
using SnifferInBlend.Models.Packets;

namespace SnifferInBlend.Models
{
    public class Utils
    {
        private static  string GetData(LithiumPacket packet)
        {
            int lineNumber = packet.Data.Length / 16 + 1;
            int digitNumber = string.Format("{0:x}", packet.Data.Length).Length + 1;
            //string offsetFormatString = new string('0', digitNumber);
            //offsetFormatString = "{0:" + offsetFormatString + "}";
            StringBuilder hex = new StringBuilder();
            StringBuilder ascii = new StringBuilder();
            string offset = "";
            byte currentByte;
            StringBuilder DataInString = new StringBuilder();
            for (int i = 0; i < packet.Data.Length; i++)
            {
                currentByte = packet.Data[i];
                hex.Append(currentByte.ToString("X2") + " ");

                ascii.Append((currentByte <= 126 && currentByte >= 32) ? (char)currentByte : '.');
                if (i % 16 == 15)
                {
                    offset = string.Format("{0:x}", i - 15);
                    offset = (new string('0', digitNumber - offset.Length)) + offset + "  ";
                    DataInString.Append(offset);
                    DataInString.Append(hex.ToString() + "  ");
                    DataInString.Append(ascii.ToString() + "\r\n");
                    hex.Clear();
                    ascii.Clear();
                }
            }

            return DataInString.ToString();


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
        public static void SetDetail(TreeView tv, Paragraph Paragraph_Data, SnifferInBlend.Models.LithiumPacket packet)
        {
            if (packet != null)
            {
                //MessageBox.Show(packet.Info);
                Paragraph_Data.Inlines.Clear();
                Run run = new Run(GetData(packet));
                Paragraph_Data.Inlines.Add(run);

                TreeView treeroot =tv;

                treeroot.Items.Clear();
                //ethernet

                treeroot.Items.Add(packet.EthernetPacket.GetViewItem());

                //ip
                if (packet.Protocal == "ARP")
                {
                    ARPLithiumPacket arpp = (ARPLithiumPacket)packet.InnerPacket;
                    treeroot.Items.Add(GetTreeView(arpp, IPProtocolType.NONE));
                    //Grid_TreeViews.Children.Add(treeroot);
                    return;

                }
                else
                {
                    IPLithiumPacket ipp = (IPLithiumPacket)packet.InnerPacket;
                    treeroot.Items.Add(GetTreeView(ipp, IPProtocolType.IP));
                }

                //transmisson
                IPLithiumPacket ippp = (IPLithiumPacket)packet.InnerPacket;
                if (ippp.InnerPacketProtocal == IPProtocolType.ICMP)
                {
                    ICMPLithiumPacket icmpp = (ICMPLithiumPacket)ippp.InnerPacket;
                    treeroot.Items.Add(GetTreeView(icmpp, IPProtocolType.ICMP));
                }
                else if (ippp.InnerPacketProtocal == IPProtocolType.TCP)
                {
                    TCPLithiumPacket tcpp = (TCPLithiumPacket)ippp.InnerPacket;
                    treeroot.Items.Add(GetTreeView(tcpp, IPProtocolType.TCP));

                    if (tcpp.InnerProtocal == Protocals.HTTP.ToString())
                    {
                        HTTPLithiumPacket hlp = (HTTPLithiumPacket)tcpp.InnerPacket;
                        if (hlp.IsRequest)
                        {
                            treeroot.Items.Add(GetTreeView((HTTPRequestLithiumPacket)hlp, IPProtocolType.NONE));
                        }
                        else
                        {
                            treeroot.Items.Add(GetTreeView((HTTPResponseLithiumPacket)hlp, IPProtocolType.NONE));
                        }
                    }

                }
                else if (ippp.InnerPacketProtocal == IPProtocolType.UDP)
                {
                    UDPLithiumPacket udpp = (UDPLithiumPacket)ippp.InnerPacket;
                    treeroot.Items.Add(GetTreeView(udpp, IPProtocolType.UDP));
                }


                //Grid_TreeViews.Children.Add(treeroot);
            }
        }

        private static  TreeViewItem GetTreeView(AbstractLithiumPacket packet, IPProtocolType protocal)
        {



            TreeViewItem tvi = packet.GetViewItem();
            return tvi;
            //return null;
        }

        public static string[] PostFilters = new string[] {"tcp","arp","udp","ethernet","ip","http","","icmp"};

        public static System.Windows.Controls.TreeViewItem GetViewItem(string formatstring, object param)
        {
            return new System.Windows.Controls.TreeViewItem() { Header = string.Format(formatstring, param),Foreground = System.Windows.Media.Brushes.White  };
        }

        private  static bool ValidateFilter(string filter)
        {
            filter = filter.ToLower();
            if (Utils.PostFilters.Contains<string>(filter))
            {
                return true;
            }
            else
            {
                IPAddress ip;
                if (filter.StartsWith("ip.src==") || filter.StartsWith("ip.dest==")||filter.StartsWith ("ip=="))
                {
                    string ipstring = filter.Substring(filter.IndexOf('=') + 2);
                    if (IPAddress.TryParse(ipstring, out ip))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    PhysicalAddress mac;
                    if (filter.StartsWith("mac.src==") || filter.StartsWith("mac.dest=="))
                    {
                        string macstring = filter.Substring(filter.IndexOf('=') + 2);
                        Regex r = new Regex(@"^([0-9A-F][0-9A-F]-[0-9A-F][0-9A-F]-[0-9A-F][0-9A-F]-[0-9A-F][0-9A-F]-[0-9A-F][0-9A-F]-[0-9A-F][0-9A-F])$");
                        return r.IsMatch(macstring);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static bool ValidatePostFilter(string filter)
        {
            string[] filters = filter.Split(new string[] { "or", "and", "not" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in filters)
            {
                if (Utils.ValidateFilter(s.Trim()))
                {
                    continue;
                }
                else return false;
            }
            return true;
        }
    }
}
