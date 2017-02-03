// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : MemoryAllocation.xaml.cs
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
using System.Collections.ObjectModel;
using MemoryMapObjects;
using System.Xml.Linq;
using System.ComponentModel;

namespace MemInsp {
	/// <summary>
	/// Interaction logic for MemoryAllocation.xaml
	/// </summary>
	public partial class MemoryAllocation : Window {
		/// <summary>
		/// Gets or sets the module.
		/// </summary>
		/// <value>
		/// The module.
		/// </value>
		private string Module {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the process name or id.
		/// </summary>
		/// <value>
		/// The process name or id.
		/// </value>
		private string ProcessNameOrId {
			get;
			set;
		}

		public MemoryAllocation() {
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryAllocation"/> class.
		/// </summary>
		/// <param name="allocations">The allocations.</param>
		/// <param name="selectedModule">The selected module.</param>
		/// <param name="processName">Name of the process.</param>
		public MemoryAllocation(IEnumerable<AllocationInformation> allocations, string selectedModule, string processName)
			: this() {

			Module = selectedModule;
			ProcessNameOrId = processName;
			lstMemAllocations.ItemsSource = allocations;
			Title = string.Format("There are {0} memory allocations in use by \"{1}\" ",
				new object[] { allocations.Count(), selectedModule });
		}

		/// <summary>
		/// Handles the Clicked event of the menuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void menuItem_Clicked(object sender, RoutedEventArgs e) {
			XDocument dump = null;
			AllocationInformation selected = lstMemAllocations.SelectedItem as AllocationInformation;

			using (BackgroundWorker worker = new BackgroundWorker()) {
				// Work to do
				worker.DoWork += delegate(object a, DoWorkEventArgs b) {

					Dispatcher.Invoke(new Action(() => {
						((MainWindow)App.Current.MainWindow).ManageVisualState(MainWindow.AnimationState.Loading);
					}));

					if (selected != null && (dump = DataExchange.ReadProcessMemory(ProcessNameOrId, selected)) != null)
						b.Result = dump;
				};

				// Code executed when Backgroundworker is completed
				worker.RunWorkerCompleted += delegate(object a, RunWorkerCompletedEventArgs b) {
					XDocument dumpAsXml = b.Result as XDocument;

					Dispatcher.Invoke(new Action(() => {
						((MainWindow)App.Current.MainWindow).ManageVisualState(MainWindow.AnimationState.Loaded);
					}));

					if (dumpAsXml != null)
						(new MemoryDump(selected, Module, dump) {
							Owner = this.Owner
						}).ShowDialog();
					else
						System.Windows.MessageBox.Show("Unable to read the specified memory region", "Information",
													   MessageBoxButton.OK, MessageBoxImage.Information);
				};
				worker.RunWorkerAsync();
			}
		}
	}
}