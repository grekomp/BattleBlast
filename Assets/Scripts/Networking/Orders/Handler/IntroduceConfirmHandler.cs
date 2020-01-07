using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IntroduceConfirmHandler : OrderHandlerBase<OrderData> {

    public static Action<OrderData> introductionFinished;

    protected override Order GetOrder() {
        return Order.IntroduceConfirm;
    }

    protected override void OnOrderReceived(OrderData data) {
        introductionFinished?.Invoke(data);
    }
}
