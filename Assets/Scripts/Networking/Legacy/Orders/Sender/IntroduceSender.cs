using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceSender : OrderSenderBase<OrderData> {

    protected override Connection.Channel GetChannel() {
        return Connection.Channel.Reliable;
    }

    protected override Order GetOrder() {
        return Order.Introduce;
    }

    public void SendIntroduction(int clientId) { 
        SendData(clientId);
    }

    protected override OrderData InitNewData() {
        return new OrderData();
    }
}
