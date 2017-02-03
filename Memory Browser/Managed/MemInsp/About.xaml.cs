// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : About.xaml.cs
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
using System.Threading;
using System.Diagnostics;

namespace MemInsp {
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window {
		/// <summary>
		/// Initializes a new instance of the <see cref="About"/> class.
		/// </summary>
		public About() {
			InitializeComponent();
		}

		/// <summary>
		/// Handles the MouseUp event of the label1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void label1_MouseUp(object sender, MouseButtonEventArgs e) {
			(new Thread(new ThreadStart(() => {
				using (Process newProc = new Process() {
					StartInfo = new ProcessStartInfo("http://www.bonafideideas.com")
				})
					newProc.Start();
			}))).Start();
		}

		/// <summary>
		/// Handles the KeyUp event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void Window_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape))
				Close();
		}
	}
}
