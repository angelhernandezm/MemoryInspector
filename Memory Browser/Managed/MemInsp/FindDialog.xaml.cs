// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : FindDialog.xaml.cs
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
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using MemoryMapObjects;
using System.Reflection;

namespace MemInsp {
	/// <summary>
	/// Interaction logic for FindDialog.xaml
	/// </summary>
	public partial class FindDialog : Window {
		#region "Properties"

		/// <summary>
		/// Gets or sets the parent window.
		/// </summary>
		/// <value>
		/// The parent window.
		/// </value>
		private MainWindow ParentWindow {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the selected module.
		/// </summary>
		/// <value>
		/// The selected module.
		/// </value>
		internal static Border SelectedModule {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the found.
		/// </summary>
		/// <value>
		/// The found.
		/// </value>
		internal object Found {
			get;
			set;
		}

		#endregion

		#region "Ctors"

		/// <summary>
		/// Initializes a new instance of the <see cref="FindDialog"/> class.
		/// </summary>
		public FindDialog() {
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FindDialog"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public FindDialog(MainWindow parent)
			: this() {
			ParentWindow = parent;
		}

		#endregion

		#region "Event Handlers"

		/// <summary>
		/// Handles the Click event of the btnFindNext control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void btnFindNext_Click(object sender, RoutedEventArgs e) {
			object item = null;
			DependencyObject element;
			ObservableCollection<ModuleInfo> items = (ObservableCollection<ModuleInfo>)ParentWindow.loadedModules.ItemsSource;

			if (!string.IsNullOrEmpty(txtFindStr.Text)) {
				if (SelectedModule != null) // Restore selected module (if there's one selected)
					SelectedModule.BorderThickness = new Thickness(0.5);

				if (chkMatchCase.IsChecked.HasValue && chkMatchCase.IsChecked.Value) {
					item = items.Where(x => x.Name.Equals(txtFindStr.Text) && x != Found).FirstOrDefault(); 
				} else {
					item = items.Where(x => x.Name.StartsWith(txtFindStr.Text,
						StringComparison.OrdinalIgnoreCase) && x != Found).FirstOrDefault();
				}

				if (item != null) {
					Found = item;
					element = ParentWindow.loadedModules.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
					ParentWindow.loadedModules.BringItemIntoView(item);
				}
			}
		}

		/// <summary>
		/// Handles the Closed event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Window_Closed(object sender, EventArgs e) {
			FindDialog.SelectedModule.BorderThickness = new Thickness(0.5);
		}

		/// <summary>
		/// Handles the Click event of the btnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		/// <summary>
		/// Handles the KeyUp event of the txtFindStr control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void txtFindStr_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Enter))
				btnFindNext_Click(this, new RoutedEventArgs());
			else if (e.Key.Equals(Key.Escape))
				Close();
		}

		/// <summary>
		/// Handles the Loaded event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			txtFindStr.Focus();
		}

		#endregion

	}

	#region "Extension Method for ItemsControl"

	/// <summary>
	/// 
	/// </summary>
	public static class ItemsControlExtension {
		/// <summary>
		/// Brings the item into view.
		/// </summary>
		/// <param name="itemsControl">The items control.</param>
		/// <param name="item">The item.</param>
		public static void BringItemIntoView(this ItemsControl itemsControl, object item) {
			ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;

			if (!TryBringContainerIntoView(generator, item)) {
				EventHandler handler = null;
				handler = (sender, e) => {
					if (generator.Status.Equals(GeneratorStatus.ContainersGenerated))
						TryBringContainerIntoView(generator, item);
					else if (generator.Status.Equals(GeneratorStatus.Error))
						generator.StatusChanged -= handler;
					else
						return;
				};
				generator.StatusChanged += handler;
			}
		}

		/// <summary>
		/// Tries the bring container into view.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private static bool TryBringContainerIntoView(ItemContainerGenerator generator, object item) {
			bool retval = false;
			Type containerType = null;
			PropertyInfo pInfo = null;
			var container = generator.ContainerFromItem(item) as FrameworkElement;

			if (container != null) {
				container.BringIntoView(new Rect(new Size() {
					Height = 150, Width = 150
				}));
				containerType = container.GetType();
				pInfo = containerType.GetProperty("TemplateChild", BindingFlags.NonPublic | BindingFlags.GetProperty |
										BindingFlags.Instance | BindingFlags.SetProperty);

				FindDialog.SelectedModule = pInfo.GetValue(container, null) as Border;

				FindDialog.SelectedModule.BorderThickness = new Thickness(5);

				retval = true;
			}
			return retval;
		}
	}

	#endregion
}
