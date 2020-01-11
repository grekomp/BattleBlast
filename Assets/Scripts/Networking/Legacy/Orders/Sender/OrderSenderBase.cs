using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrderSenderBase<Data> : MonoBehaviour where Data : OrderData {

    protected abstract ConnectionBase.Channel GetChannel();
    protected abstract Order GetOrder();
    protected abstract Data InitNewData();
    protected Data data;

    protected virtual void Awake() {
        data = InitNewData();
    }

    protected virtual void SendData() {
        if (ConnectionBase.instanceExists && ConnectionBase.baseInstance.IsReady) {
            ConnectionBase.baseInstance.Send(GetOrder(), GetChannel(), data);
        }
        else {
            DebugNet.LogWarning("No connection instance or no connection estabilished.");
        }
    }

    protected virtual void SendData(int clientId) {
        if (ConnectionBase.instanceExists && ConnectionBase.baseInstance.IsReady) {
            ConnectionBase.baseInstance.Send(clientId, GetOrder(), GetChannel(), data);
        }
        else {
            DebugNet.LogWarning("No connection instance or no connection estabilished.");
        }
    }
}
