// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : Interop.cs
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
using System.Runtime;
using System.Runtime.InteropServices;

namespace MemAddIn.Code {
	internal class Interop {
		private const string KERNEL_LIB = "Kernel32.dll";
		private const string USER32_LIB = "User32.dll";

		internal enum CmdShow : int {
			SW_HIDE = 0,
			SW_MAXIMIZE = 3,
			SW_RESTORE = 9
		}


		/// <summary>
		/// Determines whether the specified h WND is window.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <returns>
		/// 	<c>true</c> if the specified h WND is window; otherwise, <c>false</c>.
		/// </returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool IsWindow(IntPtr hWnd);

		/// <summary>
		/// Shows the window.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <param name="nCmdShow">The n CMD show.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


		/// <summary>
		/// Sets the parent.
		/// </summary>
		/// <param name="hWndChild">The h WND child.</param>
		/// <param name="hWndNewParent">The h WND new parent.</param>
		/// <returns></returns>
		[DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
	}
}
