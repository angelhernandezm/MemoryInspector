// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : ModuleInfo.cs
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
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace MemoryMapObjects {
	/// <summary>
	/// 
	/// </summary>
	public class ModuleInfo {

		#region "Properties"

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the size of image.
		/// </summary>
		/// <value>The size of image.</value>
		public int SizeOfImage {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the base of DLL in dec.
		/// </summary>
		/// <value>The base of DLL in dec.</value>
		public int BaseOfDllInDec {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the entry point in dec.
		/// </summary>
		/// <value>The entry point in dec.</value>
		public int EntryPointInDec {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the base of DLL in hex.
		/// </summary>
		/// <value>The base of DLL in hex.</value>
		public string BaseOfDllInHex {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the entry point in hex.
		/// </summary>
		/// <value>The entry point in hex.</value>
		public string EntryPointInHex {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public SolidColorBrush Color {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the image path.
		/// </summary>
		/// <value>The image path.</value>
		public string ImagePath {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the module details.
		/// </summary>
		/// <value>The module details.</value>
		public XElement ModuleDetails {
			get;
			set;
		}


		#endregion
	}
}