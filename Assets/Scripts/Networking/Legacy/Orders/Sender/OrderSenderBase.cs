using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrderSenderBase<Data> : MonoBehaviour where Data : OrderData {

    protected abstract Connection.Channel GetChannel();
    protected abstract Order GetOrder();
    protected abstract Data InitNewData();
    protected Data data;

    protected virtual void Awake() {
        data = InitNewData();
    }

    protected virtual void SendData() {
        if (Connection.instanceExists && Connection.baseInstance.IsReady) {
            Connection.baseInstance.Send(GetOrder(), GetChannel(), data);
        }
        else {
            DebugNet.LogWarning("No connection instance or no connection estabilished.");
        }
    }

    protected virtual void SendData(int clientId) {
        if (Connection.instanceExists && Connection.baseInstance.IsReady) {
            Connection.baseInstance.Send(clientId, GetOrder(), GetChannel(), data);
        }
        else {
            DebugNet.LogWarning("No connection instance or no connection estabilished.");
        }
    }
}
