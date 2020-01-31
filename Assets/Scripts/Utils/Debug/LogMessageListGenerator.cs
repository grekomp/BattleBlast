using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	public class LogMessageListGenerator : DataSource
	{
		[Header("Data Keys")]
		public DataKey messageTextDataKey;

		protected override void Init()
		{
			base.Init();

			Log.OnMessagesChanged += ApplySet;
		}

		protected override void OnEnable()
		{
			Log.OnMessagesChanged -= ApplySet;
			Log.OnMessagesChanged += ApplySet;
			base.OnEnable();
		}
		protected void OnDisable()
		{
			Log.OnMessagesChanged -= ApplySet;
		}

		protected override void ApplySet()
		{
			DataBundleCollection dataBundleCollection = new DataBundleCollection();

			foreach (var message in Log.messages)
			{
				DataBundle messageDataBundle = DataBundle.New(message);

				messageDataBundle.Set(messageTextDataKey, StringVariable.New(message));

				dataBundleCollection.Add(messageDataBundle);
			}

			Output(dataBundleCollection);
		}
	}
}
