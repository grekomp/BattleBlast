﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public static class ObjectSerializationExtension
	{

		public static byte[] SerializeToByteArray(this object obj)
		{
			if (obj == null)
			{
				return null;
			}
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static T Deserialize<T>(this byte[] byteArray) where T : class
		{
			if (byteArray == null)
			{
				return null;
			}
			return (T)byteArray.Deserialize();
		}
		public static object Deserialize(this byte[] byteArray)
		{
			if (byteArray == null)
			{
				return null;
			}
			using (var memStream = new MemoryStream())
			{
				var binForm = new BinaryFormatter();
				memStream.Write(byteArray, 0, byteArray.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				return binForm.Deserialize(memStream);
			}
		}
	}
}
