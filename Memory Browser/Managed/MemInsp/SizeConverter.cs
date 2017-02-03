using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MemoryMap {
	public class CustomSizeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

			decimal retval = 0;

			if (decimal.TryParse(value.ToString(), out retval)) {
				if (retval >= 0 && retval <= 99999)
					retval /= 625;
				else if (retval >= 100000 && retval <= 999999)
					retval /= 1250;
				else
					retval /= 2500;
			}
			return retval;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			decimal retval = 0;

			if (decimal.TryParse(value.ToString(), out retval)) {
				if (retval >= 0 && retval <= 99999)
					retval *= 625;
				else if (retval >= 100000 && retval <= 999999)
					retval *= 1250;
				else
					retval *= 2500;
			}
			return retval;
		}
	}
}