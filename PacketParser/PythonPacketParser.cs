﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Terminal_Interface;

namespace PacketParser
{
    public class PythonPacketParser : IPacketInterpreter
    {
        private ScriptRuntime runtime;
        private ScriptEngine engine;
        private string scriptStr;
        private ScriptSource source;
        private ScriptScope scope;
        private dynamic script;

        public PythonPacketParser()
        {
            runtime = Python.CreateRuntime();

            //engine = Python.CreateEngine();
            //scope = engine.CreateScope();

            var watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.py");
            watcher.Changed += (sender, args) => UpdateScript();
            UpdateScript();
        }

        private void UpdateScript()
        {
            //scriptStr = File.ReadAllText("interpreter.py");
            //source = engine.CreateScriptSourceFromString(scriptStr, "py");
            script = runtime.UseFile("interpreter.py");
        }

        public string InterpretPacket(byte[] packet)
        {
            try
            {
                return script.Parse(packet);

                //return engine.Operations.InvokeMember(scope.GetVariable("parse"), "parse", new object[] { packet });
                //scope.SetVariable("packet", packet);
                //return engine.Execute<string>(scriptStr, scope);
            }
            catch (MemberAccessException e)
            {
                return string.Format("Script error: {0}\r\n", e.Message);
            }
            catch (Exception e)
            {
                return e.Message + Environment.NewLine;
            }
        }
    }
}