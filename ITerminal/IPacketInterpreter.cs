﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminal_Interface
{
    public interface IPacketInterpreter
    {
        string InterpretPacket(byte[] packet);
    }
}