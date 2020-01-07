using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InputFieldTextSyncerDouble : ValueSyncer
{
	[Header("Components")]
	public TMP_InputField inputField;

	[Header("Variables")]
	[HandleChanges] public DoubleReference doubleValue;
	protected bool isDirty = false;

	[Header("Events")]
	public GameEventHandler OnValueChanged;
	public GameEventHandler OnEndEdit;

	protected override void ApplySet()
	{
		if (isDirty == false && inputField.text != doubleValue.ToString())
		{
			inputField.text = doubleValue.Value.ToString();
		}
	}
	protected override void ApplyGet()
	{
		double result = 0;
		double.TryParse(inputField.text, out result);
		if (doubleValue != result)
			doubleValue.Value = result;
	}

	protected override void Init()
	{
		if (inputField == null) inputField = GetComponent<TMP_InputField>();
	}

	#region Handling input events
	protected override void BindChangeEvents()
	{
		inputField.onValueChanged.AddListener(HandleInputChanged);
		inputField.onEndEdit.AddListener(HandleEndEdit);
	}
	protected override void UnbindChangeEvents()
	{
		inputField.onValueChanged.RemoveListener(HandleInputChanged);
		inputField.onEndEdit.RemoveListener(HandleEndEdit);
	}

	protected void HandleInputChanged(string newValue)
	{
		HandleInputChanged();
	}
	protected void HandleInputChanged()
	{
		if (IsInputExactlyValid() == false)
		{
			isDirty = true;
		}
		if (IsInputValid())
		{
			Get();
			OnValueChanged.Raise(this, doubleValue.Value);
		}
	}

	protected void HandleEndEdit(string newValue)
	{
		HandleInputChanged();

		isDirty = false;
		Set();

		OnEndEdit?.Raise(this, newValue);
	}
	#endregion

	public void RaiseEvent()
	{
		OnValueChanged.Raise(this);
	}

	public bool IsInputValid()
	{
		return TryParseInput(out _);
	}
	public bool IsInputExactlyValid()
	{
		double parsedValue;
		if (TryParseInput(out parsedValue))
		{
			return parsedValue.ToString() == inputField.text;
		}

		return false;
	}

	private bool TryParseInput(out double parsedValue)
	{
		return double.TryParse(inputField.text, out parsedValue);
	}
}
