using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInDebugBuild : MonoBehaviour
{
	public GameObject target;

	private void Awake()
	{


		target.SetActive(Debug.isDebugBuild);
	}
}
