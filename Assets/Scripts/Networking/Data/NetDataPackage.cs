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
		public bool responseRequired = false;


		#region Creation
		public NetDataPackage(string id, string dataType, byte[] serializedData, bool responseRequired = false)
		{
			this.id = id;
			this.dataType = dataType;
			this.serializedData = serializedData;
			this.responseRequired = responseRequired;
		}
		public NetDataPackage(string dataType, byte[] serializedData, bool responseRequired = false)
		{
			this.dataType = dataType;
			this.serializedData = serializedData;
			this.responseRequired = responseRequired;
		}
		public static NetDataPackage CreateFrom(object serializableData, string id = null, bool responseRequired = false)
		{
			if (serializableData.GetType().IsSerializable == false)
				throw new ArgumentException($"{nameof(NetDataPackage)}: Error: Cannot create data package from {serializableData}, as it is not serializable.");

			string dataType = serializableData.GetType().FullName;
			byte[] serializedData = Utils.ObjectSerializationExtension.SerializeToByteArray(serializableData);
			if (id == null) id = Guid.NewGuid().ToString();

			return new NetDataPackage(id, dataType, serializedData, responseRequired);
		}
		#endregion


		#region Serialization
		public byte[] SerializeToByteArray()
		{
			return Utils.ObjectSerializationExtension.SerializeToByteArray(this);
		}
		public static NetDataPackage DeserializeFrom(byte[] serializedData)
		{
			return Utils.ObjectSerializationExtension.Deserialize<NetDataPackage>(serializedData);
		}
		#endregion


		#region Data retrieval
		public T GetDataAs<T>()
		{
			return (T)serializedData.Deserialize();
		}
		#endregion
	}
}