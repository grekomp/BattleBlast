using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSilesiaPlugins.Utility {
	public class ContextMenuPlayerPrefsResetter : MonoBehaviour {

		[ContextMenu("Reset Player Prefs")]
		public void ResetPlayerPrefs() {
			PlayerPrefs.DeleteAll();
		}
	}
}