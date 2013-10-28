using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnifferInBlend.Models.Packets;
using SharpPcap;
using PacketDotNet;

namespace SnifferInBlend.Models
{
    public enum Protocals
    {
        TCP,
        IP,
        ICMP,
        UDP,
        ARP,
        Ethernet,
        HTTP
    }
    public class LithiumPacket:AbstractLithiumPacket 
    {

        public LithiumPacket():base()
        {
        }
        public void SetColor(AbstractLithiumPacket e )
        {
            this.Background = e.Background;
            this.Background_Selected = e.Background_Selected;
            this.Foreground = e.Foreground;
            this.Foreground_Selected = e.Foreground_Selected;
        }

        public void GetData(byte[] data)
        {
            
            //this.DataInHex = PacketDotNet.Utils.HexPrinter.GetString(data, 0, data.Length);
            //this.DataInString = "";
            //this.DataInHex ="";
            //int i = 0;
            //foreach (byte b in data)
            //{
            //    i++;
            //    this.DataInHex += b.ToString("X2");
            //    if(b<=126&&b>=32)
            //    {
            //        this.DataInString +=((char)b).ToString ();
            //    }
            //    else 
            //    {
            //        this.DataInString += ".";
            //    }
            //    if (i / 2 == 0)
            //        this.DataInString += " ";
            //}
            this.Data = data;
        }


        byte[] _Data;

        public byte[] Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        string _DataInHex;

        public string DataInHex
        {
            get { return _DataInHex; }
            set { _DataInHex = value; }
        }

        string _DataInString;

        public string DataInString
        {
            get { return _DataInString; }
            set { _DataInString = value; }
        }

        List<string> _Protocols = new List<string>();

        public List<string> Protocols
        {
            get { return _Protocols; }
            set { _Protocols = value; }
        }


        private int _Number;

        public int Number
        {
            get { return _Number; }
            set { _Number = value; }
        }
        private double  _Time;

        public double Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
        private string _Source;

        public string Source
        {
            get 
            {
                
                return _Source; 
            }
            set { _Source = value; }
        }
        private string _Destination;

        public string Destination
        {
            get 
            {
                if (_Destination  == "00:00:00:00:00:00" || _Destination  == "ff:ff:ff:ff:ff:ff")
                    return "Broadcast";
                return _Destination; 
            }
            set { _Destination = value; }
        }
        private string _Protocal;

        public string Protocal
        {
            get { return _Protocal; }
            set 
            { 
                _Protocal = value;
                this.Protocols.Add(value);
            }
        }
        private int _Length;

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
//        private string _Info;
//
//        public string Info
//        {
//            get { return _Info; }
//            set { _Info = value; }
//        }


        private EthernetLithiumPacket _EthernetPacket;

        public EthernetLithiumPacket EthernetPacket
        {
            get { return _EthernetPacket; }
            set { _EthernetPacket = value; }
        }

        //private System.Windows.Media.Brush _Foreground;

        //public System.Windows.Media.Brush Foreground
        //{
        //    get { return _Foreground; }
        //    set { _Foreground = value; }
        //}

        //private System.Windows.Media.Brush _Background;

        //public System.Windows.Media.Brush Background
        //{
        //    get { return _Background; }
        //    set { _Background = value; }
        //}

        //private System.Windows.Media.Brush _Background_Selected;

        //public System.Windows.Media.Brush Background_Selected
        //{
        //    get { return _Background_Selected; }
        //    set { _Background_Selected = value; }
        //}
        //private System.Windows.Media.Brush _Foreground_Selected;

        //public System.Windows.Media.Brush Foreground_Selected
        //{
        //    get { return _Foreground_Selected; }
        //    set { _Foreground_Selected = value; }
        //}

        private InternetLithiumPacket _InnerPacket;

        public InternetLithiumPacket InnerPacket
        {
            get { return _InnerPacket; }
            set { _InnerPacket = value; }
        }
    }
}
