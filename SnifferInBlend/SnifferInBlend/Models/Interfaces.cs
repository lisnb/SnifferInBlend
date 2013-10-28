using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace SnifferInBlend.Models
{
    public class Interface
    {
        public Interface() { }
        public Interface(PcapDevice  e)
        {
            this.Name = e.Name;
            this.Description = e.Description;
            this.IP = e.Interface.Addresses[0].Addr.ToString ();
            try
            {
                this.MAC = PacketDotNet.Utils.HexPrinter.PrintMACAddress(e.MacAddress);

            }
            catch (SharpPcap.PcapException ex)
            {
                this.MAC = "Device Not Ready";
            }
            this.Device = e;
        }

        string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        string _IP;

        public string IP
        {
            get { return _IP; }
            set { _IP = value; }
        }
        string _MAC;

        public string MAC
        {
            get { return _MAC; }
            set { _MAC = value; }
        }
        PcapDevice  _Device;

        public PcapDevice  Device
        {
            get { return _Device; }
            set { _Device = value; }
        }
    }
}
