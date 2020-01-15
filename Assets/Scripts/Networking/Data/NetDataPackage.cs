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
		public byte[] serializedError;

		#region Creation
		protected NetDataPackage(string id, string dataType, byte[] serializedData, bool responseRequired = false, byte[] serializedError = null)
		{
			this.id = id;
			this.dataType = dataType;
			this.serializedData = serializedData;
			this.responseRequired = responseRequired;
			this.serializedError = serializedError;
		}
		public static NetDataPackage CreateFrom(object serializableData, string id = null, bool responseRequired = false, Error error = null)
		{
			if (serializableData != null && serializableData.GetType().IsSerializable == false)
				throw new ArgumentException($"{nameof(NetDataPackage)}: Error: Cannot create data package from {serializableData}, as it is not serializable.");

			string dataType = serializableData?.GetType().FullName;
			byte[] serializedData = serializableData?.SerializeToByteArray();
			byte[] serializedError = error?.SerializeToByteArray();
			if (id == null) id = Guid.NewGuid().ToString();

			return new NetDataPackage(id, dataType, serializedData, responseRequired, serializedError);
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