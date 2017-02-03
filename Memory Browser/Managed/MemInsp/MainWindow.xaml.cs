// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : MainWindow.xaml.cs
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MemoryMapObjects;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Threading;
using System.ComponentModel;

namespace MemInsp {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		#region "Enums"

		/// <summary>
		/// 
		/// </summary>
		public enum AnimationState : int {
			Loading = 0,
			Loaded
		}
		#endregion

		#region "Ctor"

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow() {
			InitializeComponent();
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Gets or sets the process info collection.
		/// </summary>
		/// <value>The process info collection.</value>
		public ObservableCollection<ProcessInformation> ProcessInfoCollection {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the memory detail as XML.
		/// </summary>
		/// <value>
		/// The memory detail as XML.
		/// </value>
		private string MemoryDetailAsXml {
			get;
			set;
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Finds this instance.
		/// </summary>
		private void Find() {
			if (loadedModules.ItemsSource != null)
				(new FindDialog(this) {
					Owner = this
				}).ShowDialog();


		}

		/// <summary>
		/// Gets the running processes.
		/// </summary>
		private void GetRunningProcesses() {
			ProcessInfoCollection.Clear();

			foreach (Process currentProcess in Process.GetProcesses()) {
				try {
					ProcessInfoCollection.Add(new ProcessInformation() {
						ProcessId = currentProcess.Id,
						ProcessName = currentProcess.ProcessName,
						Icon = DataExchange.ExtractIcon(currentProcess.MainModule.FileName)
					});
				} catch (Exception) {
				}
			}
		}

		/// <summary>
		/// Saves as XML.
		/// </summary>
		private void SaveAsXml() {
			Microsoft.Win32.SaveFileDialog saveFile;

			if (!string.IsNullOrEmpty(MemoryDetailAsXml)) {
				if ((saveFile = new Microsoft.Win32.SaveFileDialog() {
					Title = "Specify location to save the selected process' memory information "
				}).ShowDialog().Value && !string.IsNullOrEmpty(saveFile.FileName)) {
					try {
						using (FileStream fileStream = new FileStream(saveFile.FileName, FileMode.Create, FileAccess.ReadWrite)) {
							fileStream.Write(Encoding.Default.GetBytes(MemoryDetailAsXml), 0, MemoryDetailAsXml.Length);
							fileStream.Close();
						}
					} catch (Exception) {
					}
				}
			} else
				System.Windows.MessageBox.
					Show("Please select a process to analyze first", "Information",
					MessageBoxButton.OK, MessageBoxImage.Information);
		}


		/// <summary>
		/// Manages the state of the visual.
		/// </summary>
		/// <param name="newState">The new state.</param>
		public void ManageVisualState(AnimationState newState) {
			Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
				VisualStateManager.GoToElementState(this,
					(newState.Equals(AnimationState.Loading) ? "IsLoading" : "IsLoaded"), true);
				cboProcesses.IsEnabled = btnRefresh.IsEnabled = newState.Equals(AnimationState.Loaded);
			}));

		}

		/// <summary>
		/// Serializes the image.
		/// </summary>
		/// <param name="image">The image.</param>
		private void SerializeImage(byte[] image) {
			Microsoft.Win32.SaveFileDialog saveFile;

			if ((saveFile = new Microsoft.Win32.SaveFileDialog() {
				Title = "Specify location to save the selected process' memory map"
			}).ShowDialog().Value && !string.IsNullOrEmpty(saveFile.FileName)) {
				try {
					using (FileStream imageStream = new FileStream(saveFile.FileName, FileMode.Create, FileAccess.ReadWrite)) {
						using (BinaryWriter binWriter = new BinaryWriter(imageStream)) {
							binWriter.Write(image);
							binWriter.Close();
						}
					}
				} catch (Exception) {
				}
			}
		}

		/// <summary>
		/// Saves as image.
		/// </summary>
		private void SaveAsImage() {
			UIElement dataUI;
			Dimension dimInfo;
			DrawingVisual drVisual;
			RenderTargetBitmap target;
			VisualBrush selectedBrush;
			MessageBoxResult screenShotOption;
			JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder() {
				QualityLevel = 100
			};

			if (loadedModules.ItemsSource != null) {
				if (!(screenShotOption = System.Windows.MessageBox.Show("Press \"Yes\" for saving the current window, \"No\" " +
				"for saving the the modules section or \"Cancel\" for not saving anything at all",
				"Confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)).Equals(MessageBoxResult.Cancel)) {

					dataUI = screenShotOption.Equals(MessageBoxResult.Yes) ? this as UIElement : loadedModules as UIElement;

					dimInfo = new Dimension(dataUI.RenderSize.Width, dataUI.RenderSize.Height, 2.0);
					target = new RenderTargetBitmap((int)dimInfo.RenderWidth, (int)dimInfo.RenderHeight, 96, 96, PixelFormats.Pbgra32);
					selectedBrush = new VisualBrush(dataUI);
					drVisual = new DrawingVisual();

					using (DrawingContext drContext = drVisual.RenderOpen()) {
						drContext.PushTransform(new ScaleTransform(dimInfo.Scale, dimInfo.Scale));
						drContext.DrawRectangle(selectedBrush, null,
							new Rect(new Point(0, 0), new Point(dimInfo.Width, dimInfo.Height)));
					}
					target.Render(drVisual);
					jpgEncoder.Frames.Add(BitmapFrame.Create(target));
					using (MemoryStream imageBytes = new MemoryStream()) {
						jpgEncoder.Save(imageBytes);
						SerializeImage(imageBytes.ToArray());
					}
				}
			} else
				System.Windows.MessageBox.
					Show("Please select a process to analyze first", "Information",
					MessageBoxButton.OK, MessageBoxImage.Information);
		}


		/// <summary>
		/// Checks if running inside VS.
		/// </summary>
		private void CheckIfRunningInsideVS() {
			IntPtr hWnd = IntPtr.Zero;
			StringBuilder buffer = new StringBuilder();

			if ((hWnd = DataExchange.GetForegroundWindow()) != IntPtr.Zero) {
				buffer.Capacity = DataExchange.GetWindowTextLength(hWnd) + 1;
				if (DataExchange.GetWindowText(hWnd, buffer, buffer.Capacity) > 0
					&& buffer.ToString().Equals(Title, StringComparison.Ordinal) &&
					 DataExchange.GetWindow(hWnd, (uint)DataExchange.uCmd.GW_HWNDNEXT) == IntPtr.Zero &&
					   DataExchange.GetWindow(hWnd, (uint)DataExchange.uCmd.GW_HWNDPREV) == IntPtr.Zero)
					WindowStyle = System.Windows.WindowStyle.None;
			}
		}

		#endregion

		#region "Backgroundworker code"

		/// <summary>
		/// Displays the memory helper.
		/// </summary>
		/// <param name="process">The process.</param>
		/// <param name="selected">The selected.</param>
		private void DisplayMemoryHelper(ProcessInformation process, ModuleInfo selected) {
			using (BackgroundWorker worker = new BackgroundWorker()) {
				// Work to do
				worker.DoWork += delegate(object a, DoWorkEventArgs b) {
					ManageVisualState(AnimationState.Loading);

					var allocations = DataExchange.GetAllocations(process.ProcessId.ToString(), selected.Name)
						.Where(x => !string.IsNullOrEmpty(x.AllocationBaseInHex) &&
									x.AllocationBaseInHex.Equals(selected.BaseOfDllInHex,
									StringComparison.OrdinalIgnoreCase));

					b.Result = new object[] { allocations, selected.Name, process.ProcessId.ToString() };
				};

				// Code executed when Backgroundworker is completed
				worker.RunWorkerCompleted += delegate(object a, RunWorkerCompletedEventArgs b) {
					ManageVisualState(AnimationState.Loaded);
					string v = ((object[])b.Result)[1].ToString(), w = ((object[])b.Result)[2].ToString();
					IEnumerable<AllocationInformation> allocations = (IEnumerable<AllocationInformation>)((object[])b.Result)[0];

					if (allocations.Any()) {
						(new MemoryAllocation(allocations, v, w)).ShowDialog();
					} else
						System.Windows.MessageBox.Show("Unable to retrieve allocation information", "Information",
														MessageBoxButton.OK, MessageBoxImage.Information);
				};
				worker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Shows the exported funtions helper.
		/// </summary>
		/// <param name="process">The process.</param>
		/// <param name="selected">The selected.</param>
		private void ShowExportedFuntionsHelper(ProcessInformation process, ModuleInfo selected) {
			using (BackgroundWorker worker = new BackgroundWorker()) {
				// Work to do
				worker.DoWork += delegate(object a, DoWorkEventArgs b) {
					ManageVisualState(AnimationState.Loading);
					var symbols = DataExchange.GetSymbols(process.ProcessId.ToString(), selected);
					b.Result = new object[] { symbols, selected };
				};

				// Code executed when Backgroundworker is completed
				worker.RunWorkerCompleted += delegate(object a, RunWorkerCompletedEventArgs b) {
					ManageVisualState(AnimationState.Loaded);
					ModuleInfo module = (ModuleInfo)((object[])b.Result)[1];
					IEnumerable<SymbolInfo> symbols = (IEnumerable<SymbolInfo>)((object[])b.Result)[0];

					if (symbols.Any()) {
						(new Symbols(symbols, module) {
							Owner = this
						}).ShowDialog();

					} else
						System.Windows.MessageBox.Show("The selected module does not export any symbol", "Information",
														MessageBoxButton.OK, MessageBoxImage.Information);
				};
				worker.RunWorkerAsync();
			}
		}

		#endregion

		#region "Event handlers"

		/// <summary>
		/// Handles the Loaded event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			DataContext = this;
			ProcessInfoCollection = new ObservableCollection<ProcessInformation>();
			GetRunningProcesses();
		}


		/// <summary>
		/// Handles the SelectionChanged event of the cboProcesses control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
		private void cboProcesses_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			string memAsXml = string.Empty;
			ProcessInformation selected = cboProcesses.SelectedItem as ProcessInformation;

			if (selected != null) {
				loadedModules.ItemsSource = DataExchange.GetModules(selected.ProcessId.ToString(), out memAsXml);
				MemoryDetailAsXml = memAsXml;
				RefreshStatusBar();
			}
		}

		/// <summary>
		/// Refreshes the status bar.
		/// </summary>
		private void RefreshStatusBar() {
			ProcessInformation process = cboProcesses.SelectedItem as ProcessInformation;
			try {
				using (Process selected = Process.GetProcessById(process.ProcessId))
					lblStatus.Text = string.Format(DataExchange.Status_Information,
						new object[] {process.ProcessName, process.ProcessId,
									  loadedModules.Items.Count,
									  Math.Abs(((selected.WorkingSet64 - selected.PrivateMemorySize64) / 1024)), 
									  selected.Threads.Count, selected.HandleCount });
			} catch (Exception) {
				lblStatus.Text = "Unable to retrieve process information";
			}
		}

		/// <summary>
		/// Handles the Click event of the btnRefresh control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			GetRunningProcesses();
			loadedModules.ItemsSource = null;
			MemoryDetailAsXml = lblStatus.Text = string.Empty;
		}


		/// <summary>
		/// Handles the Clicked event of the menuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void menuItem_Clicked(object sender, RoutedEventArgs e) {
			ProcessInformation process = cboProcesses.SelectedItem as ProcessInformation;
			ModuleInfo selected = ((FrameworkElement)(e.Source)).DataContext as ModuleInfo;

			switch (((System.Windows.Controls.MenuItem)sender).Name) {
				case "ShowInExplorer":
					Dispatcher.BeginInvoke(DispatcherPriority.Normal,
						(DispatcherOperationCallback)delegate(object arg) {
						return Process.Start("explorer.exe", String.Format("/select,\"{0}\"",
							selected.ImagePath));
					}, null);
					break;

				case "ShowExportedFunctions":
					ShowExportedFuntionsHelper(process, selected);
					break;

				case "DisplayMemory":
					DisplayMemoryHelper(process, selected);
					break;

				case "FileProperties":
					DataExchange.SHObjectProperties(IntPtr.Zero, DataExchange.SHOP_FILEPATH, selected.ImagePath, null);
					break;

				case "SearchOnline":
					Utilities.SearchOnline(selected.Name);
					break;

				case "RunDialog":
					DataExchange.RunFileDlg((new WindowInteropHelper(this)).Handle, IntPtr.Zero, string.Empty, "Run",
						"Type the name of a program, folder, document, or Internet\nresource, and " +
						"Windows will open it for you.", 0);
					break;

				case "AboutBox":
					DataExchange.ShellAbout((new WindowInteropHelper(this)).Handle,
											"Memory Inspector", "Memory inspector is a tool for analyzing memory usage and allocations made " +
											"by an application and its dependent modules.", IntPtr.Zero);
					break;

				case "_Find":
					Find();
					break;

				case "Exit":
					Close();
					break;

				case "AboutThisApp":
					(new About()).ShowDialog();
					break;

				case "SaveImage":
					SaveAsImage();
					break;

				case "SaveXml":
					SaveAsXml();
					break;

				case "ColorKey":
					(new ColorKey()).ShowDialog();
					break;
			}
		}

		/// <summary>
		/// Handles the Activated event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Window_Activated(object sender, EventArgs e) {
			CheckIfRunningInsideVS();
		}

		/// <summary>
		/// Handles the KeyUp event of the Window control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
			if (e.Key.Equals(Key.F3))
				Find();
		}
		#endregion

	}
}
