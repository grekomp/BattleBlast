using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class OrderHandlerBase<T> : MonoBehaviour where T : OrderData, new()
{
	OrderHandler handler;

	protected virtual void OnEnable()
	{
		if (handler == null)
		{
			handler = new OrderHandler(GetOrder(), OnOrderReceived);
			OrderHandlerManager.orderHandlers.Add(handler);
		}
	}

	protected virtual void OnDisable()
	{
		if (handler != null)
		{
			OrderHandlerManager.orderHandlers.Remove(handler);
		}
	}

	void OnOrderReceived(string data)
	{
		T typeData = JsonUtility.FromJson<T>(data);
		OnOrderReceived(typeData);
	}

	protected abstract Order GetOrder();
	protected abstract void OnOrderReceived(T data);
}