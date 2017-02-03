// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : MainToolWindow.cs
   Framework version: .NET Framework version 4.0
   C# compiler      : Microsoft (R) Visual C# 2010 Compiler version 4.0.30319.1
   Creation date    : 30/03/2011
   Developer        : Angel Hernández Matos
   e-m@il           : angel@bonafideideas.com 
				    : angeljesus14@hotmail.com
   Website          : http://www.bonafideideas.com
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using MemAddIn.Code;
using System.Diagnostics;
using System.IO;

namespace MemAddIn.UI.Windows {
	/// <summary>
	/// 
	/// </summary>
	public partial class MainToolWindow : UserControl {
		#region "Members"

		private DTE2 _application;
		private IntPtr _wpfWindowHwnd = IntPtr.Zero;

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets or sets the application.
		/// </summary>
		/// <value>The application.</value>
		public DTE2 Application {
			get {
				return _application;
			}
			internal set {
				_application = value;
			}
		}

		/// <summary>
		/// Gets or sets the WPF window HWND.
		/// </summary>
		/// <value>The WPF window HWND.</value>
		public IntPtr wpfWindowHwnd {
			get {
				return _wpfWindowHwnd;
			}
			set {
				if (!IsWPFWindowPresent) {
					using (System.Diagnostics.Process memoryMap = new System.Diagnostics.Process() {
						StartInfo = new ProcessStartInfo(GetAddInPath())
					}) {
						new System.Threading.Thread(() => {
							memoryMap.Start();
						}).Start();
						System.Threading.Thread.Sleep(500); // half a secs should be enough
						_wpfWindowHwnd = memoryMap.MainWindowHandle;
						Interop.SetParent(_wpfWindowHwnd, Handle);
						Interop.ShowWindow(_wpfWindowHwnd, (int)Interop.CmdShow.SW_MAXIMIZE);
					}
				}
			}
		}


		/// <summary>
		/// Gets a value indicating whether this instance is WPF window present.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is WPF window present; otherwise, <c>false</c>.
		/// </value>
		public bool IsWPFWindowPresent {
			get {
				return (wpfWindowHwnd != IntPtr.Zero && Interop.IsWindow(wpfWindowHwnd));
			}
		}

		#endregion

		#region "Private Methods"
		/// <summary>
		/// Gets the add in path.
		/// </summary>
		/// <returns></returns>
		private string GetAddInPath() {
			string path = Application.AddIns.Cast<AddIn>()
							.Where(x => x.ProgID.Equals("MemAddIn.Connect", StringComparison.OrdinalIgnoreCase))
							.FirstOrDefault().SatelliteDllPath;

			return (string.Format(@"{0}\MemInsp.exe", Path.GetDirectoryName(path)));
		}
		#endregion



		#region "Ctor"

		/// <summary>
		/// Initializes a new instance of the <see cref="MainToolWindow"/> class.
		/// </summary>
		public MainToolWindow() {
			InitializeComponent();
		}

		#endregion


		#region "Event Handlers"

		/// <summary>
		/// Handles the SizeChanged event of the MainToolWindow control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MainToolWindow_SizeChanged(object sender, EventArgs e) {
			if (IsWPFWindowPresent) {
				Interop.ShowWindow(wpfWindowHwnd, (int)Interop.CmdShow.SW_HIDE);
				System.Threading.Thread.Sleep(150);
				Interop.ShowWindow(wpfWindowHwnd, (int)Interop.CmdShow.SW_MAXIMIZE);
			}
			Invalidate(true);
		}

		#endregion


	}
}
