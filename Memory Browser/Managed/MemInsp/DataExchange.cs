// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : DataExchange.cs
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
using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Xml.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using MemoryMapObjects;
using System.Threading;
using System.Diagnostics;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Threading;
using System.Reflection;

namespace MemInsp {
	/// <summary>
	/// 
	/// </summary>
	internal class DataExchange {

		#region "Consts"

		/// <summary>
		/// 
		/// </summary>
		private const string CUSTOM_HELPER_LIBRARY = "Win32Support.dll";


		/// <summary>
		/// 
		/// </summary>
		public const string Status_Information = "Selected Process: {0} - PID: {1} - Modules Loaded: {2} - " +
												  "Working Set: {3:0,000} Kb - Threads: {4} - Handles: {5}";


		/// <summary>
		/// 
		/// </summary>
		public const int SHOP_FILEPATH = 0x00000002;

		public const int MAXCHARS = 0x400000;

		#endregion

		#region "Enums"
		/// <summary>
		/// 
		/// </summary>
		public enum uCmd : uint {
			/// <summary>
			/// 
			/// </summary>
			GW_HWNDNEXT = 2,
			/// <summary>
			/// 
			/// </summary>
			GW_HWNDPREV = 3
		}

		/// <summary>
		/// 
		/// </summary>
		public enum Operation : uint {
			GetImageInformation = 0,
			GetSymbols,
			GetAllocations
		}

		#endregion

		#region "Imported functions"

		/// <summary>
		/// Executes the specified image name.
		/// </summary>
		/// <param name="imageName">Name of the image.</param>
		/// <param name="infoAsXml">The info as XML.</param>
		/// <param name="maxChars">The max chars.</param>
		/// <param name="Operation">The operation.</param>
		/// <param name="libraryName">Name of the library.</param>
		/// <returns></returns>
		[DllImport(CUSTOM_HELPER_LIBRARY, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int Execute(string imageName, StringBuilder infoAsXml, int maxChars, uint Operation, string libraryName);

		/// <summary>
		/// Reads the memory.
		/// </summary>
		/// <param name="imageName">Name of the image.</param>
		/// <param name="infoAsXml">The info as XML.</param>
		/// <param name="maxChars">The max chars.</param>
		/// <param name="lpBaseAddress">The lp base address.</param>
		/// <param name="bytesToRead">The bytes to read.</param>
		/// <returns></returns>
		[DllImport(CUSTOM_HELPER_LIBRARY, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int ReadMemory(string imageName, StringBuilder infoAsXml, int maxChars, long lpBaseAddress, int bytesToRead);

		/// <summary>
		/// Gets the foreground window.
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();


		/// <summary>
		/// Gets the length of the window text.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		/// <summary>
		/// Gets the window text.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="lpString">The lp string.</param>
		/// <param name="nMaxCount">The n max count.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		/// <summary>
		/// Gets the window.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="uCmd">The u CMD.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		/// <summary>
		/// SHs the object properties.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="shopObjectType">Type of the shop object.</param>
		/// <param name="pszObjectName">Name of the PSZ object.</param>
		/// <param name="pszPropertyPage">The PSZ property page.</param>
		/// <returns></returns>
		[DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SHObjectProperties(IntPtr hWnd, int shopObjectType, string pszObjectName, string pszPropertyPage);


		/// <summary>
		/// Runs the file DLG.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="hIcon">The h icon.</param>
		/// <param name="lpszDirectory">The LPSZ directory.</param>
		/// <param name="lpszTitle">The LPSZ title.</param>
		/// <param name="lpszDescription">The LPSZ description.</param>
		/// <param name="uFlags">The u flags.</param>
		/// <returns></returns>
		[DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "#61")]
		public static extern int RunFileDlg(IntPtr hWnd, IntPtr hIcon, string lpszDirectory, string lpszTitle, string lpszDescription, uint uFlags);

		/// <summary>
		/// Shells the about.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="szApp">The sz app.</param>
		/// <param name="szOtherStuff">The sz other stuff.</param>
		/// <param name="hIcon">The h icon.</param>
		/// <returns></returns>
		[DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int ShellAbout(IntPtr hWnd, string szApp, string szOtherStuff, IntPtr hIcon);

		#endregion

		#region "Private Methods"

		/// <summary>
		/// Gets the allocations.
		/// </summary>
		/// <param name="infoAsXml">The info as XML.</param>
		/// <returns></returns>
		private static ObservableCollection<AllocationInformation> GetAllocationsHelper(XDocument infoAsXml) {
			ObservableCollection<AllocationInformation> retval = new ObservableCollection<AllocationInformation>();

			if (infoAsXml != null) {
				var allocation = (from data in infoAsXml.Element("allocations").Elements().AsParallel()
								  select new {
									  AllocationId = int.Parse(data.Attribute("id").Value),
									  BaseAddress = data.Attribute("baseAddressInHex").Value,
									  BaseAddressInDec = long.Parse(data.Attribute("baseAddressInDec").Value),
									  AllocationBase = data.Attribute("allocationBaseInHex").Value,
									  AllocationProtect = data.Attribute("allocationProtect").Value,
									  RegionSize = int.Parse(data.Attribute("regionSize").Value),
									  State = data.Attribute("state").Value,
									  Type = data.Attribute("type").Value,
								  }).OrderBy(sorted => sorted.BaseAddress);

				if (allocation.Any()) {
					foreach (var item in allocation) {
						retval.Add(new AllocationInformation() {
							AllocationBaseInHex = item.AllocationBase,
							AllocationId = item.AllocationId,
							AllocationProtect = item.AllocationProtect,
							BaseAddressInHex = item.BaseAddress,
							BaseAddressInDec = item.BaseAddressInDec,
							RegionSize = item.RegionSize,
							State = item.State,
							Type = item.Type
						});
					}
				}
			}
			return retval;
		}


		/// <summary>
		/// Gets the modules helper.
		/// </summary>
		/// <param name="infoAsXml">The info as XML.</param>
		/// <returns></returns>
		private static ObservableCollection<ModuleInfo> GetModulesHelper(XDocument infoAsXml) {
			ObservableCollection<ModuleInfo> retval = new ObservableCollection<ModuleInfo>();

			if (infoAsXml != null) {
				var memAllocation = (from data in infoAsXml.Element("memoryMap").Element("modules").Elements().AsParallel()
									 select new {
										 Details = data,
										 Path = data.Attribute("path").Value,
										 Name = data.Attribute("name").Value,
										 EntryPointInHex = data.Attribute("entryPointInHex").Value,
										 EntryPointInDec = int.Parse(data.Attribute("entryPointInDec").Value),
										 BaseOfDllInHex = data.Attribute("baseOfDllInHex").Value,
										 BaseOfDllInDec = int.Parse(data.Attribute("baseOfDllInDec").Value),
										 SizeOfImage = int.Parse(data.Attribute("sizeOfImage").Value)
									 }).OrderBy(sorted => sorted.BaseOfDllInDec);

				if (memAllocation.Any()) {
					foreach (var module in memAllocation) {
						retval.Add(new ModuleInfo() {
							BaseOfDllInDec = module.BaseOfDllInDec,
							BaseOfDllInHex = module.BaseOfDllInHex,
							EntryPointInDec = module.EntryPointInDec,
							EntryPointInHex = module.EntryPointInHex,
							Name = module.Name,
							SizeOfImage = module.SizeOfImage,
							ModuleDetails = module.Details,
							Color = GetColor(module.Path),
							ImagePath = module.Path
						});
					}
				}
			}
			return retval;
		}



		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <param name="selectedFile">The selected file.</param>
		/// <returns></returns>
		private static System.Windows.Media.SolidColorBrush GetColor(string selectedFile) {
			string extension = System.IO.Path.GetExtension(selectedFile);
			string currentDir = System.IO.Path.GetDirectoryName(selectedFile);
			string windows = string.Format(@"{0}\", Environment.GetEnvironmentVariable("windir"));
			string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			string gac = string.Format(@"{0}\Assembly", Environment.GetEnvironmentVariable("windir"));
			string system = string.Format(@"{0}\System", Environment.GetEnvironmentVariable("windir"));
			string programFilesx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			string system32 = string.Format(@"{0}\System32", Environment.GetEnvironmentVariable("windir"));
			string dotNet = string.Format(@"{0}\Microsoft.NET", Environment.GetFolderPath(Environment.SpecialFolder.Windows));

			if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)) {
				if (currentDir.IndexOf(dotNet, StringComparison.OrdinalIgnoreCase) >= 0 ||
					currentDir.IndexOf(gac, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.Green;
				else if (currentDir.IndexOf(system, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.LightBlue;
				else if (currentDir.IndexOf(system32, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.Gray;
				if (currentDir.IndexOf(windows, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.Cyan;
				else if (currentDir.IndexOf(programFiles, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.Tan;
				else if (currentDir.IndexOf(programFilesx86, StringComparison.OrdinalIgnoreCase) >= 0)
					return System.Windows.Media.Brushes.Yellow;
			} else if (extension.Equals(".ocx", StringComparison.OrdinalIgnoreCase))
				return System.Windows.Media.Brushes.Salmon;
			else if (!extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
				return System.Windows.Media.Brushes.Red;

			return System.Windows.Media.Brushes.Orange;
		}

		/// <summary>
		/// Gets the symbols helper.
		/// </summary>
		/// <param name="infoAsXml">The info as XML.</param>
		/// <returns></returns>
		private static ObservableCollection<SymbolInfo> GetSymbolsHelper(XDocument infoAsXml) {
			ObservableCollection<SymbolInfo> retval = new ObservableCollection<SymbolInfo>();

			if (infoAsXml != null) {
				var symbols = (from data in infoAsXml.Element("symbols").Element("exports").Elements().AsParallel()
							   select new {
								   SymbolName = data.Attribute("name").Value,
								   SymbolId = int.Parse(data.Attribute("id").Value),
								   AddressInHex = data.Attribute("AddressInHex").Value,
								   AddressInDec = int.Parse(data.Attribute("AddressInDec").Value)
							   }).OrderBy(sorted => sorted.SymbolName);

				if (symbols.Any()) {
					foreach (var symbol in symbols) {
						retval.Add(new SymbolInfo() {
							AddressInDec = symbol.AddressInDec,
							AddressInHex = symbol.AddressInHex,
							SymbolId = symbol.SymbolId,
							SymbolName = symbol.SymbolName
						});
					}
				}
			}
			return retval;
		}


		#endregion

		#region "Public Methods"

		/// <summary>
		/// Gets the modules.
		/// </summary>
		/// <param name="processName">Name of the process.</param>
		/// <returns></returns>
		public static ObservableCollection<ModuleInfo> GetModules(string processName, out string memoryDetailAsXml) {
			memoryDetailAsXml = string.Empty;
			ObservableCollection<ModuleInfo> retval = null;
			StringBuilder infoAsXml = new StringBuilder(MAXCHARS + 1);
			int result = Execute(processName, infoAsXml, infoAsXml.Capacity, (uint)Operation.GetImageInformation, string.Empty);

			if (result == 0) {
				memoryDetailAsXml = infoAsXml.ToString();
				retval = GetModulesHelper(infoAsXml.GetXDocument());
			}

			return retval;
		}


		/// <summary>
		/// Reads the process memory.
		/// </summary>
		/// <param name="processName">Name of the process.</param>
		/// <param name="selected">The selected.</param>
		/// <returns></returns>
		public static XDocument ReadProcessMemory(string processName, AllocationInformation selected) {
			XDocument retval = null;
			StringBuilder infoAsXml = new StringBuilder(MAXCHARS + 1);
			int result = ReadMemory(processName, infoAsXml, infoAsXml.Capacity,
									selected.BaseAddressInDec, selected.RegionSize);

			if (result == 0 && (retval = infoAsXml.GetXDocument()) != null &&
				infoAsXml.GetXDocument().Element("memoryDump").DescendantNodes().Count() == 0)
				retval = null;

			return retval;
		}

		/// <summary>
		/// Extracts the icon.
		/// </summary>
		/// <param name="imagePath">The image path.</param>
		/// <returns></returns>
		public static ImageSource ExtractIcon(string imagePath) {
			ImageSource retval = null;

			try {
				using (Icon hIcon = Icon.ExtractAssociatedIcon(imagePath))
					retval = Imaging.CreateBitmapSourceFromHIcon(hIcon.Handle,
						Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			} catch (Exception) {

			}
			return retval;
		}

		/// <summary>
		/// Gets the allocations.
		/// </summary>
		/// <param name="processName">Name of the process.</param>
		/// <param name="selectedModule">The selected module.</param>
		/// <returns></returns>
		public static ObservableCollection<AllocationInformation> GetAllocations(string processName, string moduleName) {
			ObservableCollection<AllocationInformation> retval = null;
			StringBuilder infoAsXml = new StringBuilder(MAXCHARS + 1);
			int result = Execute(processName, infoAsXml, infoAsXml.Capacity, (uint)Operation.GetAllocations, moduleName);

			if (result == 0)
				retval = GetAllocationsHelper(infoAsXml.GetXDocument());

			return retval;
		}


		/// <summary>
		/// Gets the symbols.
		/// </summary>
		/// <param name="processName">Name of the process.</param>
		/// <param name="selectedModule">The selected module.</param>
		/// <returns></returns>
		public static ObservableCollection<SymbolInfo> GetSymbols(string processName, ModuleInfo selectedModule) {
			ObservableCollection<SymbolInfo> retval = null;
			StringBuilder infoAsXml = new StringBuilder(MAXCHARS + 1);
			int result = Execute(processName, infoAsXml, infoAsXml.Capacity, (uint)Operation.GetSymbols, selectedModule.ImagePath);

			if (result == 0)
				retval = GetSymbolsHelper(infoAsXml.GetXDocument());

			return retval;
		}

		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	internal static class StringBuilderExtension {
		/// <summary>
		/// Gets the X document.
		/// </summary>
		/// <param name="xmlAsString">The XML as string.</param>
		/// <returns></returns>
		internal static XDocument GetXDocument(this StringBuilder xmlAsString) {
			XDocument retval = null;

			try {
				using (StringReader reader = new StringReader(xmlAsString.ToString()))
					retval = XDocument.Load(reader);
			} catch (Exception) {
				MessageBox.Show("Access is denied", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			return retval;
		}
	}
}