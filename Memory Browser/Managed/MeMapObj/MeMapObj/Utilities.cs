// Copyright (C) 2011 Angel Hernández Matos / Bonafide Ideas.
// You can redistribute this software and/or modify it under the terms of the 
// Microsoft Reciprocal License (Ms-RL).  This program is distributed in the hope 
// that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See License.txt for more details. 

/* File             : Utilities.cs
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
using System.Diagnostics;
using System.Threading;

namespace MemoryMapObjects {
	/// <summary>
	/// 
	/// </summary>
	public struct Dimension {
		private double scale, width, renderHeight, height, renderWidth;

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		public double Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		public double Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of the render.
		/// </summary>
		/// <value>
		/// The height of the render.
		/// </value>
		public double RenderHeight {
			get {
				return renderHeight;
			}
			set {
				renderHeight = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the render.
		/// </summary>
		/// <value>
		/// The width of the render.
		/// </value>
		public double RenderWidth {
			get {
				return renderWidth;
			}
			set {
				renderWidth = value;
			}
		}

		/// <summary>
		/// Gets the scale.
		/// </summary>
		public double Scale {
			get {
				return scale;
			}
		}

		/// <summary>
		/// Prevents a default instance of the <see cref="Dimension"/> struct from being created.
		/// </summary>
		/// <param name="x">The x.</param>
		private Dimension(object x) {
			scale = 0;
			width = height = renderHeight = renderWidth = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Dimension"/> struct.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="Scale">The scale.</param>
		public Dimension(double width, double height, double Scale): this(null) {
			scale = Scale;
			Width = width;
			Height = height;
			RenderWidth = Width * scale;
			RenderHeight = Height * scale;
		}
	}


	//public static class 

	/// <summary>
	/// 
	/// </summary>
	public class Utilities {

		#region "Consts"

		private const string ONLINE_SEARCH = "http://www.google.com/search?hl=en&q={0}";

		#endregion

		#region "Methods"

		/// <summary>
		/// Registers the property.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="ownerClass">The owner class.</param>
		/// <returns></returns>
		public static DependencyProperty RegisterProperty<T>(string propertyName, Type ownerClass) {
			return DependencyProperty.Register(propertyName, typeof(T), ownerClass, new PropertyMetadata());
		}


		/// <summary>
		/// Searches the online.
		/// </summary>
		/// <param name="expr">The expr.</param>
		public static void SearchOnline(string expr) {
			new Thread(new ThreadStart(delegate() {
				using (Process newProcess = new Process() {
					StartInfo = new ProcessStartInfo(string.Format(ONLINE_SEARCH, expr))
				})
					newProcess.Start();
			})).Start();

		}

		#endregion
	}
}
