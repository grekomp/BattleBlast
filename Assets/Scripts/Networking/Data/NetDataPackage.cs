using System;
using Utils;

namespace Networking
{
	/// <summary>
	/// The class that is sent over the network by NetworkingCore.
	/// </summary>
	[Serializable]
	public class NetDataPackage
	{
		public string id = Guid.NewGuid().ToString();
		public string dataType;
		public byte[] serializedData;

		public NetDataPackage(string id, string dataType, byte[] serializedData)
		{
			this.id = id;
			this.dataType = dataType;
			this.serializedData = serializedData;
		}
		public NetDataPackage(string dataType, byte[] serializedData)
		{
			this.dataType = dataType;
			this.serializedData = serializedData;
		}
		public static NetDataPackage CreateFrom(object serializableData, string id = null)
		{
			if (serializableData.GetType().IsSerializable == false)
				throw new ArgumentException($"{nameof(NetDataPackage)}: Error: Cannot create data package from {serializableData}, as it is not serializable.");

			string dataType = serializableData.GetType().FullName;
			byte[] serializedData = Utils.ObjectSerializationExtension.SerializeToByteArray(serializableData);
			if (id == null) id = Guid.NewGuid().ToString();

			return new NetDataPackage(id, dataType, serializedData);
		}

		public static NetDataPackage DeserializeFrom(int receivedConnectionId, byte[] serializedData)
		{
			return Utils.ObjectSerializationExtension.Deserialize<NetDataPackage>(serializedData);
		}

		public byte[] SerializeToByteArray()
		{
			return Utils.ObjectSerializationExtension.SerializeToByteArray(this);
		}

		public T GetDataAs<T>()
		{
			return (T)serializedData.Deserialize();
		}
	}
}