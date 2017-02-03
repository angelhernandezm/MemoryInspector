// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : AllocationInfo.cs
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
	public class AllocationInformation {
		#region "Properties"


		/// <summary>
		/// Gets or sets the allocation id.
		/// </summary>
		/// <value>
		/// The allocation id.
		/// </value>
		public int AllocationId {
			get;
			set;
		}



		/// <summary>
		/// Gets or sets the base address in hex.
		/// </summary>
		/// <value>
		/// The base address in hex.
		/// </value>
		public string BaseAddressInHex {
			get;
			set;
		}




		/// <summary>
		/// Gets or sets the base address in dec.
		/// </summary>
		/// <value>
		/// The base address in dec.
		/// </value>
		public long BaseAddressInDec {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the allocation base in hex.
		/// </summary>
		/// <value>
		/// The allocation base in hex.
		/// </value>
		public string AllocationBaseInHex {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the allocation protect.
		/// </summary>
		/// <value>
		/// The allocation protect.
		/// </value>
		public string AllocationProtect {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the size of the region.
		/// </summary>
		/// <value>
		/// The size of the region.
		/// </value>
		public int RegionSize {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		public string State {
			get;
			set;
		}



		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		public string Type {
			get;
			set;
		}

		#endregion
	}
}
