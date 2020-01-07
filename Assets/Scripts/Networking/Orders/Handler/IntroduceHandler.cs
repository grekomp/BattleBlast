using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(IntroduceResponseSender))]
public class IntroduceHandler : OrderHandlerBase<OrderData>
{

	public static Action<OrderData> onIntroduce;

	IntroduceResponseSender responder;

	void Awake()
	{
		responder = GetComponent<IntroduceResponseSender>();
	}

	protected override Order GetOrder()
	{
		return Order.Introduce;
	}

	protected override void OnOrderReceived(OrderData data)
	{
		ConnectionClient.instance.clientId = data.clientId;
		responder.SendDeviceData();
		onIntroduce?.Invoke(data);
	}
}
