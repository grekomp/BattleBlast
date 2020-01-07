﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ScriptableConditional : ScriptableObject {
	public abstract bool CheckCondition(List<ScriptableVariable> parameters);
}
