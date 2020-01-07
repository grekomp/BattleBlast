using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderHandlerManager  {

    public static List<OrderHandler> orderHandlers = new List<OrderHandler>();

    public OrderHandlerManager() {
	}

	public void ParseData(int connectionId, Order order, string parsedData) {
		List<OrderHandler> handlers = orderHandlers.FindAll(x => x.orderType == order);
        if (handlers.Count > 0) {          
			foreach (OrderHandler h in handlers) {
                h.OnDataReceived.Invoke(parsedData);
			}
		} else {
			DebugNet.Log("Order handler not found for order: " + order.ToString());
		}
	}
}