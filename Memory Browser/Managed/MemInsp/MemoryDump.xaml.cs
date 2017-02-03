// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : MemoryDump.xaml.cs
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MemoryMapObjects;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Win32;
using System.IO;

namespace MemInsp {
	/// <summary>
	/// Interaction logic for MemoryDump.xaml
	/// </summary>
	public partial class MemoryDump : Window {
		/// <summary>
		/// Gets or sets the selected.
		/// </summary>
		/// <value>The selected.</value>
		private AllocationInformation Selected {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mem dump.
		/// </summary>
		/// <value>
		/// The mem dump.
		/// </value>
		private string memDump {
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryDump"/> class.
		/// </summary>
		public MemoryDump() {
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryDump"/> class.
		/// </summary>
		/// <param name="selected">The selected.</param>
		/// <param name="module">The module.</param>
		/// <param name="data">The data.</param>
		public MemoryDump(AllocationInformation selected, string module, XDocument data)
			: this() {

			Selected = selected;

			Title = string.Format("Displaying memory dump of \"{0}\" | Address: {1}  - Bytes: {2} (0x{3:x8})",
				new object[] { module, selected.BaseAddressInHex, selected.RegionSize, selected.RegionSize });

			memDump = data.ToString();
		}

		/// <summary>
		/// Handles the Click event of the btnSaveDump control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void btnSaveDump_Click(object sender, RoutedEventArgs e) {
			SaveFileDialog saveFile;

			if ((saveFile = new SaveFileDialog() {
				Title = "Specify location to save memory dump"
			}).ShowDialog().Value && !string.IsNullOrEmpty(saveFile.FileName)) {
				using (StreamWriter writer = File.CreateText(saveFile.FileName)) {
					writer.Write(memDump);
					writer.Flush();
				}
			}
		}

		/// <summary>
		/// Handles the Loaded event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			XmlDocument xmlDoc = new XmlDocument();
			XmlDataProvider xmlMemoryDump = TryFindResource("xmlMemoryDump") as XmlDataProvider;

			if (xmlMemoryDump != null && !string.IsNullOrEmpty(memDump)) {
				xmlDoc.LoadXml(memDump);
				xmlMemoryDump.Document = xmlDoc;
				xmlMemoryDump.XPath = "memoryDump/dumpBlock";
			}
		}
	}
}