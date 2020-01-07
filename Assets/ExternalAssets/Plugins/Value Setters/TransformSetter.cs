using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TransformSetter : ValueSetter
{
	[Header("Variables")]
	[HandleChanges] public Vector3Reference position = new Vector3Reference(Vector3.zero);
	[HandleChanges] public Vector3Reference rotation = new Vector3Reference(Vector3.zero);
	[HandleChanges] public Vector3Reference localScale = new Vector3Reference(Vector3.one);

	protected override void ApplySet()
	{
		transform.position = position;
		transform.rotation = Quaternion.Euler(rotation);
		transform.localScale = localScale;
	}

	protected override void Init() { }
}
