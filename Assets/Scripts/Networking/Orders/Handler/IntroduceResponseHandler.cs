using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IntroduceResponseHandler : OrderHandlerBase<IntroduceResponseData>
{

	public static Action<IntroduceResponseData> onIntroduceResponse;

	protected override Order GetOrder()
	{
		return Order.IntroduceResponse;
	}

	protected override void OnOrderReceived(IntroduceResponseData data)
	{
		onIntroduceResponse?.Invoke(data);
	}
}
