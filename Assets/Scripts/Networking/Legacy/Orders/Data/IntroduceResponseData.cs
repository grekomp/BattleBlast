using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceResponseData : OrderData
{
    public string deviceId;
    public string serialNumber;

    public override string ToString() {
        return JsonUtility.ToJson(this);
    }
}
