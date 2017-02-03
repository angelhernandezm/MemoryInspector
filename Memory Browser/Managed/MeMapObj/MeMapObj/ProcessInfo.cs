// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : ProcessInfo.cs
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
	public class ProcessInformation {
		#region "Properties"

		/// <summary>
		/// Gets or sets the process id.
		/// </summary>
		/// <value>The process id.</value>
		public int ProcessId {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the name of the process.
		/// </summary>
		/// <value>The name of the process.</value>
		public string ProcessName {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>
		/// The icon.
		/// </value>
		public ImageSource Icon {
			get;
			set;
		}


		#endregion
	}
}
