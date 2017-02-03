// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : Symbols.xaml.cs
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
using System.Collections.ObjectModel;
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

namespace MemInsp {
	/// <summary>
	/// Interaction logic for Symbols.xaml
	/// </summary>
	public partial class Symbols : Window {
		#region "Ctors"

		/// <summary>
		/// Initializes a new instance of the <see cref="Symbols"/> class.
		/// </summary>
		public Symbols() {
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Symbols"/> class.
		/// </summary>
		/// <param name="symbolsFound">The symbols found.</param>
		/// <param name="selectedModule">The selected module.</param>
		public Symbols(IEnumerable<SymbolInfo> symbolsFound, ModuleInfo selectedModule)
			: this() {

			DataContext = this;
			Title = string.Format("There are {0} exported symbols found in \"{1}\"",
				new object[] { symbolsFound.Count(), selectedModule.ImagePath.ToUpper() });
			lstSymbols.ItemsSource = symbolsFound;
		}

		#endregion

		#region "Event Handlers"

		/// <summary>
		/// Handles the Clicked event of the menuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void menuItem_Clicked(object sender, RoutedEventArgs e) {
			SymbolInfo selected = lstSymbols.SelectedItem as SymbolInfo;

			if (selected != null)
				Utilities.SearchOnline(selected.SymbolName);

		}

		#endregion
	}
}