// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : SymbolInfo.cs
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
	public class SymbolInfo  {
		#region "Properties"

		/// <summary>
		/// Gets or sets the name of the symbol.
		/// </summary>
		/// <value>The name of the symbol.</value>
		public string SymbolName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the symbol id.
		/// </summary>
		/// <value>The symbol id.</value>
		public int SymbolId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the address in hex.
		/// </summary>
		/// <value>The address in hex.</value>
		public string AddressInHex {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the address in dec.
		/// </summary>
		/// <value>The address in dec.</value>
		public int AddressInDec {
			get;
			set;
		}

		#endregion

	}
}