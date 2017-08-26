using System;
using System.Globalization;
using Xamarin.Forms;

namespace AgogaSim
{
	public class ExpandedTimelineIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            bool expanded = (bool)value;
            return expanded ? "ic_timeline_on.png" : "ic_timeline_off.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ExpandedIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool expanded = (bool)value;
			return expanded ? "ic_arrow_down.png" : "ic_arrow_left.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
