﻿using System;
using System.Windows.Forms;
using Bukkitgui2.AddOn;
using Bukkitgui2.AddOn.Starter;
using Bukkitgui2.Core;
using Bukkitgui2.MinecraftInterop.ProcessHandler;
using Bukkitgui2.UI;

namespace Bukkitgui2.Controls.QuickButtons
{
	public partial class QuickButtons : UserControl
	{
		public QuickButtons()
		{
			InitializeComponent();
			ProcessHandler.ServerStarting += HandleServerStarting;
			ProcessHandler.ServerStarted += HandleServerStarted;
			ProcessHandler.ServerStopped += HandleServerStopped;
			ProcessHandler.ServerStopping += HandleServerStopping;
		}

		private void HandleServerStarting()
		{
			//suport for calls from other threads
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (HandleServerStarting));
			}
			else
			{
				btnStartStop.Enabled = false;
				btnStartStop.Text = Locale.Tr("Starting...");
			}
		}

		private void HandleServerStarted()
		{
			//suport for calls from other threads
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (HandleServerStarted));
			}
			else
			{
				btnStartStop.Enabled = true;
				btnStartStop.Text = Locale.Tr("Stop");
			}
		}

		private void HandleServerStopping()
		{
			//suport for calls from other threads
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (HandleServerStopping));
			}
			else
			{
				btnStartStop.Enabled = false;
				btnStartStop.Text = Locale.Tr("Stopping...");
			}
		}

		private void HandleServerStopped()
		{
			//suport for calls from other threads
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (HandleServerStopped));
			}
			else
			{
				btnStartStop.Enabled = true;
				btnStartStop.Text = Locale.Tr("Start");
			}
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			//suport for calls from other threads
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (() => btnStartStop_Click(sender, e)));
			}
			else
			{
				if (ProcessHandler.IsRunning)
				{
					ProcessHandler.StopServer();
				}
				else
				{
					if (!(ParentForm is MainForm)) return; //check if the parent form is a mainform
					MainForm parentForm = (MainForm) ParentForm;
					Starter starter = (Starter) parentForm.GetRequiredAddon(RequiredAddon.Starter); // Get the starter addon
					starter.LaunchServerFromTab(); // Launch with tab settings
				}
			}
		}
	}
}