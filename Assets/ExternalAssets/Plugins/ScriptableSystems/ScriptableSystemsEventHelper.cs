using UnityEngine;
using System.Collections;

namespace ScriptableSystems
{
	public class ScriptableSystemsEventHelper : DontDestroySingleton<ScriptableSystemsEventHelper>
	{
		private void Awake()
		{
			ScriptableSystemsManager.Instance.Awake();
		}
		protected override void Start()
		{
			base.Start();
			ScriptableSystemsManager.Instance.Start();
		}
		private void Update()
		{
			ScriptableSystemsManager.Instance.Update();
		}
		private void FixedUpdate()
		{
			ScriptableSystemsManager.Instance.FixedUpdate();
		}
	}
}