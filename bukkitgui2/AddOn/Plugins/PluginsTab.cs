﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bukkitgui2.AddOn.Plugins
{
	public partial class PluginsTab : UserControl, IAddonTab
	{
		public PluginsTab()
		{
			InitializeComponent();
		}

		public IAddon ParentAddon { get; set; }
	}
}
