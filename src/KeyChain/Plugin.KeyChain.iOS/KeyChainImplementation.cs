using Plugin.KeyChain.Abstractions;
using System;
using Security;
using Foundation;

namespace Plugin.KeyChain
{
	/// <summary>
	/// Implementation for KeyChain
	/// </summary>
	public class KeyChainImplementation : IKeyChain
	{
		public string serviceId = "KeyChainAccountStore";

		public void SetKey(string keyName, string keyValue)
		{
			var statusCode = SecStatusCode.Success;

			//
			// Remove any existing record
			///
			var existing = GetKey(keyName);

			if (!string.IsNullOrEmpty(existing))
			{
				DeleteKey(keyName);
			}

			//
			// Add this record
			//
			var data = NSData.FromString(keyValue);
			var record = new SecRecord(SecKind.GenericPassword);
			record.Service = serviceId;
			record.Account = keyName;
			record.Generic = data;
			record.Accessible = SecAccessible.WhenUnlocked;

			statusCode = SecKeyChain.Add(record);

			if (statusCode != SecStatusCode.Success)
			{
				throw new Exception("Could not save key to KeyChain: " + statusCode);
			}
		}

		public string GetKey(string keyName)
		{
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = serviceId;
			query.Account = keyName;

			SecStatusCode statusCode;
			var record = SecKeyChain.QueryAsRecord(query, out statusCode);

			return statusCode == SecStatusCode.Success ? Uri.UnescapeDataString(record.Generic.ToString()) : "";
		}

		public void DeleteKey(string keyName)
		{
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = serviceId;
			query.Account = keyName;

			var statusCode = SecKeyChain.Remove(query);

			if (statusCode != SecStatusCode.Success)
			{
				throw new Exception("Could not delete key from KeyChain: " + statusCode);
			}
		}
	}
}