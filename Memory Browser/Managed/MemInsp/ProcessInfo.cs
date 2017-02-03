using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace MemoryMap {
	public class ProcessInformation: DependencyObject {
		#region "Properties"

		/// <summary>
		/// Gets or sets the process id.
		/// </summary>
		/// <value>The process id.</value>
		public int ProcessId {
			get {
				return ((int) GetValue(ProcessIdProperty));
			}
			set {
				SetValue(ProcessIdProperty, value);
			}
		}

		public static readonly DependencyProperty ProcessIdProperty = Utilities.RegisterProperty<int>("ProcessId", typeof(ProcessInformation));


		/// <summary>
		/// Gets or sets the name of the process.
		/// </summary>
		/// <value>The name of the process.</value>
		public string ProcessName {
			get {
				return (GetValue(ProcessNameProperty).ToString());
			}
			set {
				SetValue(ProcessNameProperty, value);
			}
		}

		public static readonly DependencyProperty ProcessNameProperty = Utilities.RegisterProperty<string>("ProcessName", typeof(ProcessInformation));
			
		#endregion

	}
}
