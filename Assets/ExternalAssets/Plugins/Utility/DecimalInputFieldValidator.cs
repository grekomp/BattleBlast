using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "TextMeshPro/Custom Validators/Custom Decimal Validator")]
public class DecimalInputFieldValidator : TMP_InputValidator
{
	public override char Validate(ref string text, ref int pos, char ch)
	{
		if (ch == 0) return ch;

		bool cursorBeforeDash = (pos == 0 && text.Length > 0 && text[0] == '-');
		if (!cursorBeforeDash)
		{
			if (char.IsDigit(ch) || (ch == '-' && pos == 0) || (ch == DecimalSeparator && !text.Contains(DecimalSeparator)))
			{
				text = text.Insert(pos, ch.ToString());
				return ch;
			}
		}

		return '\0';
	}

	private static char DecimalSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
}

