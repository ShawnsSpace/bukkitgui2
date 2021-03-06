﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Bukkitgui2.Core.Logging;

namespace Bukkitgui2.Core.Util.Web
{
	public partial class FileDownloader : Form
	{
		/// <summary>
		///     Dictionary containing URLs of files to download and their target locations
		/// </summary>
		private readonly Dictionary<string, string> _files;

		/// <summary>
		///     Dictionary containing URLs of files to download and their controls
		/// </summary>
		private readonly Dictionary<string, FileDownloadProgressBar> _downloads;

		public FileDownloader()
		{
			InitializeComponent();
			Size = new Size(500, 50);
			_files = new Dictionary<string, string>();
			_downloads = new Dictionary<string, FileDownloadProgressBar>();
		}

		/// <summary>
		///     Add a files to the list of files to be downloaded
		/// </summary>
		/// <param name="url">the URL to download the file from</param>
		/// <param name="targetlocation">the target location to save the file to</param>
		/// <returns></returns>
		public Boolean AddFile(string url, string targetlocation)
		{
			_files.Add(url, targetlocation);
			Size = new Size(Size.Width, Size.Height + 50); // make room for new control

			// create new control
			FileDownloadProgressBar control = new FileDownloadProgressBar
			{
				Location =
					new Point(0, _files.Count*50 - 50)
			};
			control.CreateDownload(url, targetlocation);
			_downloads.Add(url, control);
			Controls.Add(control);

			return true;
		}

		/// <summary>
		///     Start the download of the files
		/// </summary>
		public void StartDownload()
		{
			Logger.Log(LogLevel.Info, "FileDownloader", "Starting " + _downloads.Count + " download(s)");
			foreach (FileDownloadProgressBar control in _downloads.Values)
			{
				control.DownloadCompleted += HandleDownloadFinished;
				control.StartDownload();
			}
		}


		private int _finishedDownloads;

		private void HandleDownloadFinished(object sender, DownloadDataCompletedEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke((MethodInvoker) (() => HandleDownloadFinished(sender, e)));
			}
			else
			{
				_finishedDownloads++;
				if (_finishedDownloads == _files.Count)
				{
					Close();
				}
			}
		}
	}
}