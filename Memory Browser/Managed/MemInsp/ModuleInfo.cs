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
	public class ModuleInfo : DependencyObject {

		#region "Properties"

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return GetValue(NameProperty).ToString();
			}
			set {
				SetValue(NameProperty, value);
			}
		}
		public static readonly DependencyProperty NameProperty = Utilities.RegisterProperty<string>("Name", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the size of image.
		/// </summary>
		/// <value>The size of image.</value>
		public int SizeOfImage {
			get {
				return ((int)GetValue(SizeOfImageProperty));
			}
			set {
				SetValue(SizeOfImageProperty, value);
			}
		}

		public static readonly DependencyProperty SizeOfImageProperty = Utilities.RegisterProperty<int>("SizeOfImage", typeof(ModuleInfo));

		/// <summary>
		/// Gets or sets the base of DLL in dec.
		/// </summary>
		/// <value>The base of DLL in dec.</value>
		public int BaseOfDllInDec {
			get {
				return ((int)GetValue(BaseOfDllInDecProperty));
			}
			set {
				SetValue(BaseOfDllInDecProperty, value);
			}
		}

		public static readonly DependencyProperty BaseOfDllInDecProperty = Utilities.RegisterProperty<int>("BaseOfDllInDec", typeof(ModuleInfo));

		/// <summary>
		/// Gets or sets the entry point in dec.
		/// </summary>
		/// <value>The entry point in dec.</value>
		public int EntryPointInDec {
			get {
				return ((int)GetValue(EntryPointInDecProperty));
			}
			set {
				SetValue(EntryPointInDecProperty, value);
			}
		}

		public static readonly DependencyProperty EntryPointInDecProperty = Utilities.RegisterProperty<int>("EntryPointInDec", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the base of DLL in hex.
		/// </summary>
		/// <value>The base of DLL in hex.</value>
		public string BaseOfDllInHex {
			get {
				return GetValue(BaseOfDllInHexProperty).ToString();
			}
			set {
				SetValue(BaseOfDllInHexProperty, value);
			}
		}

		public static readonly DependencyProperty BaseOfDllInHexProperty = Utilities.RegisterProperty<string>("BaseOfDllInHex", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the entry point in hex.
		/// </summary>
		/// <value>The entry point in hex.</value>
		public string EntryPointInHex {
			get {
				return GetValue(EntryPointInHexProperty).ToString();
			}
			set {
				SetValue(EntryPointInHexProperty, value);
			}
		}

		public static readonly DependencyProperty EntryPointInHexProperty = Utilities.RegisterProperty<string>("EntryPointInHex", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public SolidColorBrush Color {
			get {
				return ((SolidColorBrush)GetValue(ColorProperty));
			}
			set {
				SetValue(ColorProperty, value);
			}
		}

		public static readonly DependencyProperty ColorProperty = Utilities.RegisterProperty<SolidColorBrush>("Color", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the image path.
		/// </summary>
		/// <value>The image path.</value>
		public string ImagePath {
			get {
				return GetValue(ImagePathProperty).ToString();
			}
			set {
				SetValue(ImagePathProperty, value);
			}
		}

		public static readonly DependencyProperty ImagePathProperty = Utilities.RegisterProperty<string>("ImagePath", typeof(ModuleInfo));


		/// <summary>
		/// Gets or sets the module details.
		/// </summary>
		/// <value>The module details.</value>
		public XElement ModuleDetails {
			get {
				return ((XElement)GetValue(ModuleDetailsProperty));
			}
			set {

				

				SetValue(ModuleDetailsProperty, value);
			}
		}

		public static readonly DependencyProperty ModuleDetailsProperty = Utilities.RegisterProperty<XElement>("ModuleDetails", typeof(ModuleInfo));

		#endregion
	}
}