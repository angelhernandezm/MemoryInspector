using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MemoryMap {
	public class MemoryConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

			decimal kb = 0;
			string retval = string.Empty;

			if (decimal.TryParse(value.ToString(), out kb))
				retval = string.Format("{0} Kb", kb / 1024);

			return retval;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int retval;
			string tempVal = string.Empty;

			if (value != null && !string.IsNullOrEmpty(tempVal = value.ToString().Replace("Kb", string.Empty)) &&
				int.TryParse(tempVal, out retval))
				retval *= 1024;
			else
				retval = 0;

			return retval;
		}
	}
}