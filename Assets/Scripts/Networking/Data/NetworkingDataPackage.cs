using System;

namespace Networking
{
	/// <summary>
	/// The class that is sent over the network by NetworkingCore.
	/// </summary>
	[Serializable]
	public class NetworkingDataPackage
	{
		public string dataType;
		public byte[] serializedData;

		public NetworkingDataPackage(string dataType, byte[] serializedData)
		{
			this.dataType = dataType;
			this.serializedData = serializedData;
		}
		public static NetworkingDataPackage CreateFrom(object serializableData)
		{
			if (serializableData.GetType().IsSerializable == false)
				throw new ArgumentException($"{nameof(NetworkingDataPackage)}: Error: Cannot create data package from {serializableData}, as it is not serializable.");

			string dataType = serializableData.GetType().FullName;
			byte[] serializedData = Utils.ObjectSerializationExtension.SerializeToByteArray(serializableData);
			return new NetworkingDataPackage(dataType, serializedData);
		}
		public static NetworkingDataPackage DeserializeFrom(int receivedConnectionId, byte[] serializedData)
		{
			return Utils.ObjectSerializationExtension.Deserialize<NetworkingDataPackage>(serializedData);
		}

		public byte[] SerializeToByteArray()
		{
			return Utils.ObjectSerializationExtension.SerializeToByteArray(this);
		}
	}
}