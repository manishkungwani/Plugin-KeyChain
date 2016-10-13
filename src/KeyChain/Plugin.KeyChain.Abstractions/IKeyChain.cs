using System;

namespace Plugin.KeyChain.Abstractions
{
    /// <summary>
    /// Interface for KeyChain
    /// </summary>
    public interface IKeyChain
    {
		void SetKey(string keyName, string keyValue);
		string GetKey(string keyName);
		void DeleteKey(string keyName);
    }
}
