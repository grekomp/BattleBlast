using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NumbersAsDateTimeFormat : IFormatProvider, ICustomFormatter {
	public object GetFormat(Type formatType) {
		if (formatType == typeof(ICustomFormatter))
			return this;
		else
			return null;
	}

	public string Format(string format, object arg, IFormatProvider formatProvider) {
		if (format == null || (!format.StartsWith("asDateTimeSeconds") && !format.StartsWith("asDateTimeSeconds:"))) {
			return HandleOtherFormats(format, arg);
		}

		string trimmedFormat = null;
		if (format.Length > 18) {
			trimmedFormat = format.Substring(18);
		}

		switch (arg) {
			case float _:
			case double _:
			case int _:
			case long _:
				DateTime dateTime = new DateTime().AddSeconds((double)arg);

				if (trimmedFormat != null) return dateTime.ToString(trimmedFormat);
				return dateTime.ToString();
			default:
				return HandleOtherFormats(format, arg);
		}
	}

	private string HandleOtherFormats(string format, object arg) {
		try {
			if (arg is IFormattable)
				return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
			else if (arg != null)
				return arg.ToString();
			else
				return string.Empty;
		}
		catch (FormatException e) {
			throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
		}
	}
}
