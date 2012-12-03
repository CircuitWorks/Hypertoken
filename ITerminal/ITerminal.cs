﻿using System.ComponentModel;
using Terminal_Interface.Enums;
using Terminal_Interface.Events;

namespace Terminal_Interface
{
	public delegate void SendFileEventHandler(object sender, SendFileEventArgs e);

	public delegate void SetLoggingPathEventHandler(object sender, SetLoggingPathEventArgs e);

	public delegate void OnKeyPressedEventHandler(object sender, OnKeyPressedEventArgs e);

	public delegate void SaveSessionEventHandler(object sender, SaveSessionEventArgs e);

	//TODO Add dropped file event

	public interface ITerminal : INotifyPropertyChanged
	{
		// Required functions

		void AddLine(string line);

		void AddChar(char c);

		void Run();

		void TrimLines(int trimTo); // TODO stopgap measure to trim lines

		void SetBackend(ISerialBackend backend);

		// Required (?) events
		event SendFileEventHandler OnSendFile;

		event OnKeyPressedEventHandler OnKeyPressed;

		// Old API

		event SetLoggingPathEventHandler OnSetLoggingPath;

		event SaveSessionEventHandler OnSaveSession;

		string GetLoggingFilePath();

		void SetFileSendState(FileSendState fileSendState);
	}
}