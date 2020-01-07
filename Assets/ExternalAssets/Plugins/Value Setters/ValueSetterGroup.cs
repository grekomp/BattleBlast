using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ValueSetterGroup : ValueSetter {
	[Header("Components")]
	public List<ValueSetter> valueSetters = new List<ValueSetter>();

	protected override void ApplySet() {
		foreach (var valueSetter in valueSetters) {
			valueSetter.Set();
		}
	}

	protected override void Init() { }
}
