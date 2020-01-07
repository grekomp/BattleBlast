using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class TrackWorldObject : MonoBehaviour
{
	[Header("Options")]
	public Transform trackedTransform;

	protected RectTransform rectTransform;
	protected Canvas parentCanvas;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		parentCanvas = GetComponentInParent<Canvas>();
	}

	public void LateUpdate()
	{
		if (trackedTransform)
		{
			Camera camera = parentCanvas.worldCamera ?? Camera.main;
			rectTransform.position = camera.WorldToScreenPoint(trackedTransform.position);
		}
	}
}

