using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class TrackWorldPosition : ValueSetter
{
	[Header("Options")]
	[HandleChanges] public Vector3Reference trackedPosition;

	protected RectTransform rectTransform;
	protected Canvas parentCanvas;

	protected override void ApplySet()
	{
		Camera camera = parentCanvas.worldCamera ?? Camera.main;
		if (rectTransform)
			rectTransform.position = camera.WorldToScreenPoint(trackedPosition.Value);
	}

	protected override void Init()
	{
		rectTransform = GetComponent<RectTransform>();
		parentCanvas = GetComponentInParent<Canvas>();
	}
}
