using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Athanor
{
	public class CoroutineWithData<T>
	{
		private IEnumerator _target;
		public T result;
		public Coroutine Coroutine { get; private set; }

		public CoroutineWithData(MonoBehaviour owner_, IEnumerator target_)
		{
			_target = target_;
			Coroutine = owner_.StartCoroutine(Run());
		}

		private IEnumerator Run()
		{
			while (_target.MoveNext())
			{
				result = (T)_target.Current;
				yield return result;
			}
		}
	}
}
