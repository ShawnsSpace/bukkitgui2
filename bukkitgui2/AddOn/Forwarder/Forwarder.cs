﻿using System.Windows.Forms;

namespace Bukkitgui2.AddOn.Forwarder
{
	internal class Forwarder : IAddon
	{
		private UserControl _tab;

		/// <summary>
		///     The addon name, ideally this name is the same as used in the tabpage
		/// </summary>
		public string Name
		{
			get { return "Forwarder"; }
		}

        /// <summary>
        /// True if this addon has a tab page
        /// </summary>
	    public bool HasTab {
	        get
	        {
	            return true;
	        }
	    }

        /// <summary>
        /// True if this addon has a config field
        /// </summary>
        public bool HasConfig
        {
            get
            {
                return false;
            }
        }

	    /// <summary>
		///     Initialize all functions and the tabcontrol
		/// </summary>
		void IAddon.Initialize()
		{
			_tab = new ForwarderTab { Text = this.Name, ParentAddon = this};
		}

	    public UserControl TabPage { get; private set; }

	    public UserControl ConfigPage { get; private set; }

	    /// <summary>
		///     The tab control for this addon
		/// </summary>
		/// <returns>Returns the tabpage</returns>
		UserControl IAddon.TabPage
		{
			get { return _tab; }
		}
	}
}