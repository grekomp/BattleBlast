[System.Serializable]
public class OrderHandler {

	public Order orderType;
	public StringUnityEvent OnDataReceived;
	
	public OrderHandler(Order orderType, StringUnityEvent OnDataReceived) {
		this.orderType = orderType;
		this.OnDataReceived = OnDataReceived;
	}

	public OrderHandler(Order orderType, System.Action<string> OnDataReceived) {
		this.orderType = orderType;
		StringUnityEvent evnt = new StringUnityEvent();
		evnt.AddListener((x) => OnDataReceived(x));
		this.OnDataReceived = evnt;

	}
}
