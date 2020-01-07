using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class GameEventData {
	public GameEvent gameEvent;
	public object data;
	public object caller;

	public GameEventData(GameEvent gameEvent, object data = null, object caller = null) {
		this.gameEvent = gameEvent;
		this.data = data;
		this.caller = caller;
	}
}
