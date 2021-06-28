using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Graph.Bayesian.WPF.Infrastructure
{
	public class DefaultConverter : IValueConverter
	{
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static DefaultConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int i && i > 0)
				return true;
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static DefaultConverter Instance { get; } = new DefaultConverter();
	}
}
