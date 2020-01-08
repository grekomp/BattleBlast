using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializator {

	public static byte[] GetBytes(System.Object obj) {
		if (obj == null) {
			return null;
		}
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream()) {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
	}

	public static System.Object GetObject(byte[] byteArray) {
        using (MemoryStream memStream = new MemoryStream()) {
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            System.Object obj = null;
            obj = binForm.Deserialize(memStream);
            return obj;
        }
	}

	public static T GetObject<T>(byte[] arrBytes) {
		return (T)GetObject(arrBytes);
	}
}
