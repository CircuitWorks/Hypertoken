﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using Anotar.NLog;
using Terminal.Interface;
using Terminal.Interface.Enums;
using Terminal.Interface.Events;

namespace Terminal
{
    public class SerialPortDataHandler : ISerialPort
    {
        private readonly CurrentPacketParser _handler;
        private readonly SerialPort _port;
        private readonly byte[] _receiveBuffer;

        public SerialPortDataHandler(CurrentPacketParser handler)
        {
            _handler = handler;
            _port = new SerialPort();
            _port.DataReceived += PortOnDataReceived;

            _receiveBuffer = new byte[1024];

            DeviceName = "COM1";
            Baud = 115200;
            DataBits = 8;
            StopBits = Interface.Enums.StopBits.One;
            Parity = Interface.Enums.Parity.None;
            FlowControl = FlowControl.None;
        }

        public event DataReceivedEventHandler DataReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Baud
        {
            get { return _port.BaudRate; }
            set { _port.BaudRate = value; }
        }

        public string CurrentDevice { get; set; }

        public int DataBits
        {
            get { return _port.DataBits; }
            set
            {
                _port.DataBits = value;
                DeviceStatus = "";
            }
        }

        public string DeviceName
        {
            get { return _port.PortName; }
            set
            {
                if(_port.IsOpen)
                    PortState = PortState.Closed;

                _port.PortName = value;
            }
        }

        public string[] Devices
        {
            get { return SerialPort.GetPortNames(); }
        }

        public string FriendlyName
        {
            get { return string.Format("{0}", _port.PortName); }
        }

        public string DeviceStatus
        {
            get
            {
                return string.Format("{0};{1};{2}",
                    _port.DataBits,
                    _port.Parity.ToString()[0],
                    (float)((int)_port.StopBits + 1) / 2);
            }
            set
            {
                LogTo.Info("Invalidated DeviceStatus; {0}", value);
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return DeviceType.SerialPort;
            }
        }

        public FlowControl FlowControl
        {
            get { return (Interface.Enums.FlowControl)_port.Handshake; }
            set { _port.Handshake = (System.IO.Ports.Handshake)value; }
        }

        public Interface.Enums.Parity Parity
        {
            get { return (Interface.Enums.Parity)_port.Parity; }
            set
            {
                _port.Parity = (System.IO.Ports.Parity)value;
                DeviceStatus = "";
            }
        }

        public PortState PortState
        {
            get { return _port.IsOpen ? PortState.Open : PortState.Closed; }
            set
            {
                LogTo.Info("Port being set to {0}", value);
                if (value == PortState.Open)
                    try
                    {
                        _port.Open();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        LogTo.Error("{0} is in use", DeviceName);
                        throw;
                    }
                else
                    _port.Close();
            }
        }

        public Interface.Enums.StopBits StopBits
        {
            get { return (Interface.Enums.StopBits)_port.StopBits; }
            set
            {
                _port.StopBits = (System.IO.Ports.StopBits)value;
                DeviceStatus = "";
            }
        }

        public IEnumerable<string> ListAvailableDevices()
        {
            return Devices.AsEnumerable();
        }

        public int Write(byte[] data)
        {
            int length = data.Length;
            if (length > _port.WriteBufferSize)
                length = _port.WriteBufferSize;

            _port.Write(data, 0, length);
            return length;
        }

        public int Write(byte data)
        {
            _port.Write(new byte[] { data }, 0, 1);
            return 1;
        }

        public int Write(string data)
        {
            // TODO handle long string sending

            _port.WriteLine(data);
            return data.Length;
        }

        public int Write(char data)
        {
            _port.Write(new[] { data }, 0, 1);
            return 1;
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            LogTo.Info("Received {0} bytes", _port.BytesToRead);
            if (!_port.IsOpen)
                return;

            var count = _port.BytesToRead;

            if (count == 0)
                return;
            
            if (count > 1024)
                count = 1024;

            _port.Read(_receiveBuffer, 0, count);

            if (DataReceived != null)
            {
                var actualData = new byte[count];
                Array.Copy(_receiveBuffer, actualData, count);
                string packet = _handler.CurrentParser.InterpretPacket(actualData);

                var args = new DataReceivedEventArgs(packet);
                DataReceived(this, args);
            }
        }
    }
}