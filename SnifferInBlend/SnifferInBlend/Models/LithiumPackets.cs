using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using PacketDotNet;
using System.Net;
using System.Windows.Media;
using System.Windows.Controls;
using SnifferInBlend.Models;

namespace SnifferInBlend.Models.Packets
{
    public class EthernetLithiumPacket:DatalinkLithiumPacket
    {
        public EthernetLithiumPacket(){}

        public EthernetLithiumPacket(EthernetPacket e)
        {
            this.Destination =PacketDotNet.Utils.HexPrinter.PrintMACAddress ( e.DestinationHwAddress);
            this.Source = PacketDotNet.Utils.HexPrinter.PrintMACAddress( e.SourceHwAddress);
            this.Protocal = IPProtocolType.NONE;
            this.Length = e.PayloadData == null ? 0 : e.PayloadData.Length;
            this.InnerProtocal = IPProtocolType.NONE;
            this.InnerPacket = null;
            this.Info = string.Format("Ethernet , Src: {0} , Dst: {1}",this.Source,this.Destination );
            
        }

        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();
            tvi.Header = this.Info;

            TreeViewItem destination = new TreeViewItem();
            destination.Header = string.Format("Destination: {0}", this.Destination);
            destination.Foreground = Brushes.White;
            TreeViewItem address = new TreeViewItem();
            address.Header = string.Format("Address: {0}", this.Destination);
            address.Foreground = Brushes.White;
            destination.Items.Add(address);
            
            TreeViewItem source = new TreeViewItem();
            TreeViewItem address_s = new TreeViewItem();
            address_s.Header = string.Format("Address: {0}", this.Source);
            address_s.Foreground = Brushes.White;
            source.Header = string.Format("Source: {0}", this.Source);
            source.Foreground = Brushes.White;
            source.Items.Add(address_s);

            TreeViewItem type = new TreeViewItem();
            type.Header = string.Format("Type: {0}", this.Protocal);
            type.Foreground = Brushes.White;

            tvi.Items.Add(destination);
            tvi.Items.Add(source);
            tvi.Items.Add(type);

            return tvi;

        }


//        string _Info;
//
//        public string Info
//        {
//            get { return _Info; }
//            set { _Info = value; }
//        }


        private string _Source;

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        private string _Destination;

        public string Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }
        private IPProtocolType _Protocal;

        public IPProtocolType Protocal
        {
            get { return _Protocal; }
            set { _Protocal = value; }
        }
        private IPProtocolType _InnerProtocal;

        public IPProtocolType InnerProtocal
        {
            get { return _InnerProtocal; }
            set { _InnerProtocal = value; }
        }
        private InternetLithiumPacket _InnerPacket;

        public InternetLithiumPacket InnerPacket
        {
            get { return _InnerPacket; }
            set { _InnerPacket = value; }
        }

        private int _Length;

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
    }

    public  struct IPFlags
    {
        public bool ReservedBit;
        public bool DontFragment;
        public bool MoreFragments;
    }

    public  struct  TCPFlags
    {
        public bool Reserved;
        public bool Nonce;
        public bool CongestionWindowReduced;
        public bool ECNEcho;
        public bool Urgent;
        public bool Acknowledgement;
        public bool Push;
        public bool Reset;
        public bool Syn;
        public bool Fin;
    }

    public class IPLithiumPacket:InternetLithiumPacket
    {

        public IPLithiumPacket() { }
        public IPLithiumPacket(IpPacket e)
        {
            this.Protocal = e.Protocol;
            this.HeaderLength = e.HeaderLength;
            this.Version = e.Version.ToString ();
            this.TotalLength = e.TotalLength;
            this.Source = e.SourceAddress;
            this.Destination = e.DestinationAddress;
            this.TimeToLive = e.TimeToLive;
            this.InnerPacketProtocal = e.Protocol;
            this.Info = string.Format("Internet Protocol Version {0} , Src : {1} , Dst : {2}", this.Version, this.Source, this.Destination);
        }

        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();

            tvi.Header = this.Info;

            TreeViewItem version = new TreeViewItem();
            version.Header = string.Format("Version: {0}", this.Version);
            version.Foreground = Brushes.White;

            TreeViewItem headerlength = new TreeViewItem();
            headerlength.Header = string.Format("Header Length: {0} bytes", this.HeaderLength);
            headerlength.Foreground = Brushes.White;

            TreeViewItem totallength = new TreeViewItem();
            totallength.Header = string.Format("Total Length: ", this.TotalLength);
            totallength.Foreground = Brushes.White;

            TreeViewItem identification = Utils.GetViewItem("Identification: {0}", this.Identification);
            TreeViewItem timetolive = Utils.GetViewItem("Time to live: {0}", this.TimeToLive);
            TreeViewItem protocal = Utils.GetViewItem("Protocol: {0}", this.Protocal);

            TreeViewItem source = Utils.GetViewItem("Source: {0}", this.Source);
            TreeViewItem destination = Utils.GetViewItem("Destination: {0}", this.Destination);

            tvi.Items.Add(version);
            tvi.Items.Add(headerlength);
            tvi.Items.Add(totallength);
            tvi.Items.Add(identification);
            tvi.Items.Add(timetolive);
            tvi.Items.Add(protocal);
            tvi.Items.Add(source);
            tvi.Items.Add(destination);

            return tvi;
            
        }
        //string _Info;

        //public string Info
        //{
        //    get { return _Info; }
        //    set { _Info = value; }
        //}

        //本层的协议
        private IPProtocolType _Protocal;

        public IPProtocolType Protocal
        {
            get { return _Protocal; }
            set { _Protocal = value; }
        }
        //包含的下一层的协议
        private IPProtocolType _InnerPacketProtocal;

        public IPProtocolType InnerPacketProtocal
        {
            get { return _InnerPacketProtocal; }
            set { _InnerPacketProtocal = value; }
        }
        //下一层的包的信息
        private TransportLithiumPacket _InnerPacket;

        public TransportLithiumPacket InnerPacket
        {
            get { return _InnerPacket; }
            set { _InnerPacket = value; }
        }
        //本层的IP地址
        private IPAddress _Source;

        public IPAddress Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        private IPAddress _Destination;

        public IPAddress Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        private string  _Version;

        public string  Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        private int _HeaderLength;

        public int HeaderLength
        {
            get { return _HeaderLength; }
            set { _HeaderLength = value; }
        }
        private int _TotalLength;

        public int TotalLength
        {
            get { return _TotalLength; }
            set { _TotalLength = value; }
        }
        private int _Identification;


        public int Identification
        {
            get { return _Identification; }
            set { _Identification = value; }
        }
        private int _FragmentOffset;


        public int FragmentOffset
        {
            get { return _FragmentOffset; }
            set { _FragmentOffset = value; }
        }
        private int _FlagsInt;


        public int FlagsInt
        {
            get { return _FlagsInt; }
            set { _FlagsInt = value; }
        }
        private IPFlags _Flags;


        public IPFlags Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }
        private int _TimeToLive;


        public int TimeToLive
        {
            get { return _TimeToLive; }
            set { _TimeToLive = value; }
        }
        private int _Checksum;


        public int Checksum
        {
            get { return _Checksum; }
            set { _Checksum = value; }
        }
    }

    public class TCPLithiumPacket : TransportLithiumPacket
    {

        public TCPLithiumPacket() { }
        public TCPLithiumPacket(TcpPacket e)
        {
            e.UpdateTCPChecksum();
            this.Checksum = e.Checksum;
            this.Destination = e.DestinationPort;
            this.Source = e.SourcePort;

            this.AllFlags = e.AllFlags;

            this.WindowsSizeValue = e.WindowSize;
            this.Protocal = IPProtocolType.TCP;
            this.HeaderLength = e.Header.Length;
            this.SequenceNumber = e.SequenceNumber;
            this.AcknowledgeNumber = e.AcknowledgmentNumber;

            this.Background = Brushes.ForestGreen;
            this.Foreground  = Brushes.Black;

            this.Length = e.PayloadData ==null?0:e.PayloadData.Length;

            this.Info = string.Format("Transmisson Control Protocol, Src Port: {0} , Dst Port: {1} , Seq: {2} , Len: {3}", this.Source, this.Destination, this.SequenceNumber, this.Length);
            this.Flags = new TCPFlags() {  Acknowledgement = e.Ack,
                                           Syn=e.Syn, 
                                           ECNEcho = e.ECN,
                                           Fin=e.Fin ,
                                           Push = e.Psh ,
                                           Reset = e.Rst ,
                                           CongestionWindowReduced = e.CWR ,
                                           Urgent = e.Urg};
        }

        private TreeViewItem Getviewitem(string stringformat, bool set)
        {
            return new TreeViewItem() { Header = string.Format(stringformat.Replace ('2',set?'1':'0'), set ? "set" : "not set"), Foreground = Brushes.White  };
        }
        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi= base.GetViewItem();
            tvi.Header = this.Info;

            TreeViewItem source = Utils.GetViewItem("Source port: {0}", this.Source);
            TreeViewItem destionation = Utils.GetViewItem("Destination port: {0}", this.Destination);
            TreeViewItem sequence = Utils.GetViewItem("Sequence number: {0}", this.SequenceNumber);

            TreeViewItem header = Utils.GetViewItem("Header length:{0}", this.HeaderLength);
            TreeViewItem flags = Utils.GetViewItem("Flags: {0}", "");

            //foreach (System.Reflection.PropertyInfo p in this.Flags.GetType().GetProperties())
            //{
            //    TreeViewItem t = Utils.GetViewItem(p.Name + ": {0}", ((bool)p.GetValue(this.Flags,null )) ? "set" : "Not set");
            //    flags.Items.Add(t);
            //}
            flags.Items.Add(this.Getviewitem(".... 2... .... = Congestion Window Reduced (CWR): {0}", this.Flags.CongestionWindowReduced));
            flags.Items.Add(this.Getviewitem(".... .2.. .... = ECN-Echo: {0}", this.Flags.ECNEcho));
            flags.Items.Add(this.Getviewitem(".... ..2. .... = Urgent: {0}", this.Flags.Urgent));
            flags.Items.Add(this.Getviewitem(".... ...2 .... = Acknowlegement: {0}", this.Flags.Acknowledgement));
            flags.Items.Add(this.Getviewitem(".... .... 2... = Push: {0}", this.Flags.Push));
            flags.Items.Add(this.Getviewitem(".... .... .2.. = Reset: {0}", this.Flags.Reset ));            
            flags.Items.Add(this.Getviewitem(".... .... ..2. = SYN: {0}", this.Flags.Syn));
            flags.Items.Add(this.Getviewitem(".... .... ...2 = Fin: {0}", this.Flags.Fin));

            TreeViewItem windowsize = Utils.GetViewItem("Window size value", this.WindowsSizeValue);
            TreeViewItem checksum = Utils.GetViewItem("Checksum: ", this.Checksum);

            tvi.Items.Add(source);
            tvi.Items.Add(destionation);
            tvi.Items.Add(sequence);
            tvi.Items.Add(header);
            tvi.Items.Add(flags);
            tvi.Items.Add(windowsize);
            tvi.Items.Add(checksum);

            return tvi;
        }


        int _Length;


        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        //string _Info;

        //public string Info
        //{
        //    get { return _Info; }
        //    set { _Info = value; }
        //}

        //private Brush _Background;

        //public Brush Background
        //{
        //    get { return _Background; }
        //    set { _Background = value; }
        //}
        //private Brush _Foreground;

        //public Brush Foreground
        //{
        //    get { return _Foreground; }
        //    set { _Foreground = value; }
        //}
        //private Brush _Background_Selected;

        //public Brush Background_Selected
        //{
        //    get { return _Background_Selected; }
        //    set { _Background_Selected = value; }
        //}
        //private Brush _Foreground_Selected;

        //public Brush Foreground_Selected
        //{
        //    get { return _Foreground_Selected; }
        //    set { _Foreground_Selected = value; }
        //}

        private IPProtocolType _Protocal;


        public IPProtocolType Protocal
        {
            get { return _Protocal; }
            set { _Protocal = value; }
        }
        private string _InnerProtocal;

        public string  InnerProtocal
        {
            get { return _InnerProtocal; }
            set { _InnerProtocal = value; }
        }
        private ApplicationLithiumPacket _InnerPacket;

        public ApplicationLithiumPacket InnerPacket
        {
            get { return _InnerPacket; }
            set { _InnerPacket = value; }
        }

        private int _Source;

        public int Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        private int _Destination;

        public int Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        private uint _SequenceNumber;

        public uint SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }
        private uint _AcknowledgeNumber;

        public uint AcknowledgeNumber
        {
            get { return _AcknowledgeNumber; }
            set { _AcknowledgeNumber = value; }
        }

        private int _HeaderLength;

        public int HeaderLength
        {
            get { return _HeaderLength; }
            set { _HeaderLength = value; }
        }

        private byte  _AllFlags;

        public byte  AllFlags
        {
            get { return _AllFlags; }
            set { _AllFlags = value; }
        }
        private TCPFlags _Flags;

        public TCPFlags Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }

        private int _WindowsSizeValue;

        public int WindowsSizeValue
        {
            get { return _WindowsSizeValue; }
            set { _WindowsSizeValue = value; }
        }

        private int _Checksum;

        public int Checksum
        {
            get { return _Checksum; }
            set { _Checksum = value; }
        }

        private int _TCPSegmentData;

        public int TCPSegmentData
        {
            get { return _TCPSegmentData; }
            set { _TCPSegmentData = value; }
        }
    }

    public class ARPLithiumPacket : InternetLithiumPacket 
    {

        public ARPLithiumPacket(){}
        public ARPLithiumPacket(ARPPacket e)
        {
            this.Operation =e.Operation ;

            this.Send = e.SenderProtocolAddress;
            this.Target = e.TargetProtocolAddress;

            this.Destination = PacketDotNet.Utils.HexPrinter.PrintMACAddress ( e.TargetHardwareAddress);
            this.Source = PacketDotNet.Utils .HexPrinter .PrintMACAddress ( e.SenderHardwareAddress);

            this.Background = Brushes.Pink;


            this.Info =  string.Format("Address Resolution Protocol ({0})", this.Operation);
        }

        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();
            tvi.Header = this.Info;

            TreeViewItem senderip = Utils.GetViewItem("Sender IP address: {0}", this.Send);
            TreeViewItem targetip = Utils.GetViewItem("Target IP address: {0}", this.Target);
            TreeViewItem sendermac = Utils.GetViewItem("Sender MAC address: {0}", this.Source);
            TreeViewItem targetmac = Utils.GetViewItem("Target MAC address:{0}", this.Destination);
            TreeViewItem operation = Utils.GetViewItem("Operation: {0}", this.Operation);

            tvi.Items.Add(operation);
            tvi.Items.Add(sendermac);
            tvi.Items.Add(senderip);
            tvi.Items.Add(targetmac);
            tvi.Items.Add(targetip);


            return tvi;
        }

        string _Source;

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        string _Destination;

        public string Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }
        IPAddress _Send;

        public IPAddress Send
        {
            get { return _Send; }
            set { _Send = value; }
        }
        IPAddress _Target;

        public IPAddress Target
        {
            get { return _Target; }
            set { _Target = value; }
        }

        //string _Info;


        //public string Info
        //{
        //    get { return _Info; }
        //    set { _Info = value; }
        //}


        private ARPOperation  _Operation;

        public ARPOperation  Operation
        {
            get { return _Operation; }
            set { _Operation = value; }
        }



    }

    public class UDPLithiumPacket : TransportLithiumPacket
    {

        public UDPLithiumPacket() { }
        public UDPLithiumPacket(UdpPacket e)
        {

            this.Checksum = e.Checksum;
            this.Source = e.SourcePort;
            this.Destination = e.DestinationPort;
            this.Length = e.Length;
            this.Protocal = IPProtocolType.UDP;
            this.Background = Brushes.CadetBlue;
            this.Foreground = Brushes.Black;


            this.Info = string.Format("User Datagram Protocol, Src Port: {0}, Dst Port: {1}", this.Source, this.Destination);
        }

        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();
            tvi.Header = this.Info;


            TreeViewItem source = Utils.GetViewItem("Source port: {0}", this.Source);
            TreeViewItem dest = Utils.GetViewItem("Destination port: {0}", this.Destination);
            TreeViewItem length = Utils.GetViewItem("Length: {0}", this.Length);
            TreeViewItem checksum = Utils.GetViewItem("Checksum: {0}", this.Checksum);

            tvi.Items.Add(source);
            tvi.Items.Add(dest);
            tvi.Items.Add(length);
            tvi.Items.Add(checksum);


            return tvi;
        }

        //string _Info;

        //public string Info
        //{
        //    get { return _Info; }
        //    set { _Info = value; }
        //}

        private IPProtocolType _Protocal;

        public IPProtocolType Protocal
        {
            get { return _Protocal; }
            set { _Protocal = value; }
        }
        private IPProtocolType _InnerProtocal;

        public IPProtocolType InnerProtocal
        {
            get { return _InnerProtocal; }
            set { _InnerProtocal = value; }
        }
        private ApplicationLithiumPacket _InnerPacket;

        public ApplicationLithiumPacket InnerPacket
        {
            get { return _InnerPacket; }
            set { _InnerPacket = value; }
        }
        private int _Source;

        public int Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        private int _Destination;

        public int Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }
        private int _Length;

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
        private int _Checksum;

        public int Checksum
        {
            get { return _Checksum; }
            set { _Checksum = value; }
        }
    }

    public class ICMPLithiumPacket : TransportLithiumPacket
    {
        public ICMPLithiumPacket() { }
        public ICMPLithiumPacket(ICMPv4Packet e)
        {
            this.Type = e.TypeCode.ToString();
            this.SequenceBE = e.Sequence;
            this.IdentifierBE = e.ID;
            this.Checksum = e.Checksum;
            this.Background = Brushes.MediumPurple;
            this.Foreground = Brushes.Black;

            this.Info = string.Format("ICMP , Type: {0}", this.Type);
        }

        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();
            tvi.Header = this.Info;
            return tvi;
            
        }

        //string _Info;


        //public string Info
        //{
        //    get { return _Info; }
        //    set { _Info = value; }
        //}

        private string _Type;

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        private int _Checksum;

        public int Checksum
        {
            get { return _Checksum; }
            set { _Checksum = value; }
        }
        private int _IdentifierBE;

        public int IdentifierBE
        {
            get { return _IdentifierBE; }
            set { _IdentifierBE = value; }
        }
        private int _IdentifierLE;

        public int IdentifierLE
        {
            get { return _IdentifierLE; }
            set { _IdentifierLE = value; }
        }
        private int _SequenceBE;

        public int SequenceBE
        {
            get { return _SequenceBE; }
            set { _SequenceBE = value; }
        }
        private int _SequenceLE;

        public int SequenceLE
        {
            get { return _SequenceLE; }
            set { _SequenceLE = value; }
        }


    }


    public class HTTPField
    {
        public HTTPField() { }

        public HTTPField(string name, string value, int offset,int length=0)
        {
            this.Name = name;
            this.Value = value;
            this.Offset = offset;
            this.Length = length; 

        }

        int _Length;

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }


        string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }


        int _Offset;

        public int Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }

        
    }


    public class HTTPLithiumPacket : ApplicationLithiumPacket
    {
        public HTTPLithiumPacket() 
        {
           
        }
        List<HTTPField> _AdditionField;

        public List<HTTPField> AdditionField
        {
            get { return _AdditionField; }
            set { _AdditionField = value; }
        }


        bool _IsRequest;

        public bool IsRequest
        {
            get { return _IsRequest; }
            set { _IsRequest = value; }
        }


        bool _IsData;

        public bool IsData
        {
            get { return _IsData; }
            set { _IsData = value; }
        }

        byte[] _PayLoadData;

        public byte[] PayLoadData
        {
            get { return _PayLoadData; }
            set { _PayLoadData = value; }
        }


        public override TreeViewItem GetViewItem()
        {
            TreeViewItem tvi = base.GetViewItem();
            tvi.Foreground = Brushes.White;

            
            tvi.Header = string.Format ("Hypertext Transfer Protocol ({0})",this.IsRequest?"Request":"Response");

            if (this.IsData)
            {
                return tvi;
            }

            foreach (HTTPField hf in this.AdditionField)
            {
                TreeViewItem ttvi = new TreeViewItem();
                ttvi.Header = hf.Name + " : " + hf.Value;
                ttvi.Foreground = Brushes.White;
                tvi.Items.Add (ttvi);

            }

            return tvi;
        }
    }

    public class HTTPRequestLithiumPacket : HTTPLithiumPacket
    {
        public static string[] RequestMethods = new string[] { "GET","POST","HEAD","PUT","DELETE"};

        public HTTPRequestLithiumPacket() : base() { }
        public HTTPRequestLithiumPacket(byte[] httpData)
        {
            try
            {
                this.IsRequest = true;
                this.Info = "Hypertext Transfer Protocol (Request)";
                this.AdditionField = new List<HTTPField>();
                string dataInString = System.Text.Encoding.Default.GetString(httpData);
                if (!HTTPRequestLithiumPacket.RequestMethods.Contains<string>(dataInString.Substring(0, 3)) && !HTTPRequestLithiumPacket.RequestMethods.Contains<string>(dataInString.Substring(0, 4)))
                {
                    this.IsData = true;
                    this.PayLoadData = httpData;
                    return;
                }
                string[] header_data = dataInString.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (header_data.Length >= 1)
                {
                    string[] headers = header_data[0].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (headers.Length >= 1)
                    {
                        string request = headers[0];
                        this.Request = new HTTPField();
                        this.Request.Name = "Request";
                        this.Request.Value = request;
                        this.Request.Offset = 0;
                        this.Request.Length = request.Length;

                        string[] requests = request.Split(' ');
                        if (requests.Length == 3)
                        {
                            this.RequestMethod = new HTTPField();
                            this.RequestMethod.Name = "Request Method";
                            this.RequestMethod.Value = requests[0];
                            this.RequestMethod.Offset = 0;
                            this.RequestMethod.Length = this.RequestMethod.Value.Length;


                            this.RequestURI = new HTTPField();
                            this.RequestURI.Name = "Request URI";
                            this.RequestURI.Value = requests[1];
                            this.RequestURI.Offset = this.RequestMethod.Length + 1;
                            this.RequestURI.Length = this.RequestURI.Value.Length;


                            this.RequestVersion = new HTTPField();
                            this.RequestVersion.Name = "Request Version";
                            this.RequestVersion.Value = requests[2];
                            this.RequestVersion.Offset = requests[0].Length + requests[1].Length + 1;
                            this.RequestVersion.Length = this.RequestVersion.Value.Length;
                        }

                        int i = request.Length;
                        int location = 0;
                        string key;
                        string value;
                        for (int j = 1; j < headers.Length; j++)
                        {
                            location = headers[j].IndexOf(':');
                            key = headers[j].Substring(0, location);
                            value = headers[j].Substring(location + 1);

                            string[] keyvalue = headers[j].Split(':');
                            if (keyvalue.Length == 2)
                                this.AdditionField.Add(new HTTPField(key, value, i + 1, headers[j].Length));
                            i += (headers[j].Length + 1);
                        }

                    }
                }
            }
            catch
            {
                this.IsData = true;
                this.PayLoadData = httpData;
                return;
            }
        }


        public override TreeViewItem GetViewItem()
        {

            TreeViewItem tvibase = base.GetViewItem();

            if (this.IsData)
            {
                tvibase.Items.Add(new TreeViewItem() { Foreground = Brushes.White, Header = "Http Data" });
                return tvibase;
            }
            TreeViewItem request = new TreeViewItem();
            request.Foreground  = Brushes.White;
            request.Header = this.Request.Value;

            TreeViewItem method = new TreeViewItem() { Foreground = Brushes.White, Header = this.RequestMethod.Name + " : " + this.RequestMethod.Value };
            request.Items.Add(method);

            TreeViewItem uri = new TreeViewItem() { Foreground = Brushes.White, Header = this.RequestURI.Name + " : " + this.RequestURI.Value };
            request.Items.Add(uri);

            TreeViewItem version = new TreeViewItem() { Foreground = Brushes.White, Header = this.RequestVersion.Name + " : " + this.RequestVersion.Value };
            request.Items.Add(version);

            tvibase.Items.Insert(0, request);

            return tvibase;

            

        }


        HTTPField _Request;

        public HTTPField Request
        {
            get { return _Request; }
            set { _Request = value; }
        }
        HTTPField _RequestMethod;

        public HTTPField RequestMethod
        {
            get { return _RequestMethod; }
            set { _RequestMethod = value; }
        }
        HTTPField _RequestURI;

        public HTTPField RequestURI
        {
            get { return _RequestURI; }
            set { _RequestURI = value; }
        }
        HTTPField _RequestVersion;

        public HTTPField RequestVersion
        {
            get { return _RequestVersion; }
            set { _RequestVersion = value; }
        }

        HTTPField _Host;

        public HTTPField Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        HTTPField _Connection;

        public HTTPField Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }
        HTTPField _Accept;

        public HTTPField Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        HTTPField _UserAgent;

        public HTTPField UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        HTTPField _Referer;

        public HTTPField Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        HTTPField _AcceptEncoding;

        public HTTPField AcceptEncoding
        {
            get { return _AcceptEncoding; }
            set { _AcceptEncoding = value; }
        }
        HTTPField _AcceptLanguage;

        public HTTPField AcceptLanguage
        {
            get { return _AcceptLanguage; }
            set { _AcceptLanguage = value; }
        }


    }

    public class HTTPResponseLithiumPacket : HTTPLithiumPacket
    {
        public HTTPResponseLithiumPacket() { }

        public HTTPResponseLithiumPacket (byte [] data )
        {
            try
            {
                this.IsRequest = false;
                this.Info = "Hypertext Transfer Protocol (Response)";
                string dataInString = System.Text.Encoding.Default.GetString(data);
                this.AdditionField = new List<HTTPField>();

                if (!dataInString.StartsWith("HTTP"))
                {
                    this.IsData = true;
                    this.PayLoadData = data;
                    return;
                }

                string[] header_data = dataInString.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (header_data.Length >= 1)
                {
                    string[] headers = header_data[0].Split( new string[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries );
                    if (headers.Length >= 1)
                    {

                        string[] status = headers[0].Split(' ');

                        this.Status = new HTTPField("Status", headers[0], 0, headers[0].Length);
                        if (status.Length == 3)
                        {
                            this.Version = new HTTPField("Request Version", status[0], 0, status[0].Length);
                            this.StatusCode = new HTTPField("Status Code", status[1], this.Version.Length + 1, status[1].Length);
                            this.Phrase = new HTTPField("Response Phrase", status[2], this.Version.Length + this.StatusCode.Length + 2, status[2].Length);
                        }

                        int i = this.Status.Length+1;
                        string key;
                        string value;
                        int location = 0;
                        for (int j = 1; j < headers.Length; j++)
                        {
                            location = headers[j].IndexOf(':');
                            key = headers[j].Substring(0, location);
                            value = headers[j].Substring(location + 1);
                            this.AdditionField.Add(new HTTPField() { Name = key, Value = value, Offset = i, Length = headers[j].Length });
                            i += (headers[j].Length  + 1);
                        }


                    }


 
                }






            }
            catch
            {
                this.IsData = true;
                this.PayLoadData = data;
            }
        }


        public override TreeViewItem GetViewItem()
        {

            TreeViewItem tvibase = base.GetViewItem();

            if (this.IsData)
            {
                tvibase.Items.Add(new TreeViewItem() { Foreground = Brushes.White, Header = "Http Data" });
                return tvibase;
            }

            TreeViewItem status = new TreeViewItem() { Foreground = Brushes.White, Header = this.Status.Value };

            TreeViewItem version = new TreeViewItem() { Foreground = Brushes.White, Header = this.Version.Name + " : " + this.Version.Value };
            status.Items.Add(version);

            TreeViewItem code = new TreeViewItem() { Foreground = Brushes.White, Header = this.StatusCode.Name + " : " + this.StatusCode.Value };
            status.Items.Add(code);

            TreeViewItem phrase = new TreeViewItem() { Foreground = Brushes.White, Header = this.Phrase.Name + " : " + this.Phrase.Value };
            status.Items.Add(phrase);

            tvibase.Items.Insert(0, status);

            return tvibase;
        }


        HTTPField _Status;

        public HTTPField Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        HTTPField _Version;

        public HTTPField Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        HTTPField _Phrase;

        public HTTPField Phrase
        {
            get { return _Phrase; }
            set { _Phrase = value; }
        }

        HTTPField _StatusCode;

        public HTTPField StatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }
        }


    }

}
