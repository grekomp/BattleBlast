using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceResponseSender : OrderSenderBase<IntroduceResponseData> {

    public void SendDeviceData() {
        UpdateData();
        SendData();
    }

    protected override ConnectionBase.Channel GetChannel() {
        return ConnectionBase.Channel.Reliable;
    }

    protected override Order GetOrder() {
        return Order.IntroduceResponse;
    }

    protected override IntroduceResponseData InitNewData() {
        return new IntroduceResponseData();
    }

     void UpdateData() {
        string serial = SystemInfo.deviceName;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
        serial = jo.GetStatic<string>("SERIAL");
#endif
        data.deviceId = SystemInfo.deviceUniqueIdentifier;
        data.serialNumber = serial;
    }
}
