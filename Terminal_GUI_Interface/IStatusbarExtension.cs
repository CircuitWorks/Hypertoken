﻿using System.Windows.Forms;

namespace Terminal_GUI_Interface
{
	public interface IStatusbarExtension
	{
		ToolStripItem StatusBarItem { get; }
	}
}