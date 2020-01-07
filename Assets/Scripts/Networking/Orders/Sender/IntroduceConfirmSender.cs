using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceConfirmSender : OrderSenderBase<OrderData> {
    protected override Connection.Channel GetChannel() {
        return Connection.Channel.Reliable;
    }

    protected override Order GetOrder() {
        return Order.IntroduceConfirm;
    }

    public void Send(int clientId) {
        SendData(clientId);
    }

    protected override OrderData InitNewData() {
        return new OrderData();
    }

}
