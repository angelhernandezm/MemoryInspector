using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace MemoryMap {
	/// <summary>
	/// 
	/// </summary>
	public class Utilities {

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

		#endregion
	}
}
