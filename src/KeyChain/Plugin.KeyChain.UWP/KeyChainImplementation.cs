using System;
using Windows.Security.Credentials;
using Plugin.KeyChain.Abstractions;
using System.Diagnostics;

namespace Plugin.KeyChain
{
    /// <summary>
    /// Implementation for KeyChain
    /// </summary>
    public class KeyChainImplementation : IKeyChain
  {
        readonly PasswordVault _passVault;
        private const string _RESOURCE = "PasswordVault";

        public KeyChainImplementation()
        {
            _passVault = new PasswordVault();
        }

        public void SetKey(string keyName, string keyValue)
        {
            var cred = new PasswordCredential(_RESOURCE, keyName, keyValue);
            _passVault.Add(cred);
        }

        public string GetKey(string keyName)
        {
            try
            {
                var cred = _passVault.Retrieve(_RESOURCE, keyName);
                cred.RetrievePassword();
                return cred.Password;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void DeleteKey(string name)
        {
            PasswordCredential cred = null;
            try
            {
                cred = _passVault.Retrieve(_RESOURCE, name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not delete key from KeyChain: " + ex.Message);
            }

            _passVault.Remove(cred);
        }
    }
}