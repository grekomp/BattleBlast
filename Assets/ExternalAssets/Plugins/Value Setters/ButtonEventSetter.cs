using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEventSetter : ValueSetter {
	[Header("Components")]
	public Button button;

	[Header("Events")]
	public GameEventHandlerGroup OnClick;

	protected override void ApplySet() {
		button?.onClick.RemoveListener(HandleClick);
		button?.onClick.AddListener(HandleClick);
	}

	public void HandleClick() {
		OnClick.Raise();
	}

	protected override void Init() {
		if (button == null) button = GetComponent<Button>();
	}
}
