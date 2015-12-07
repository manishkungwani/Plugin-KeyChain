using System;

namespace Plugin.KeyChain.Abstractions
{
  /// <summary>
  /// Interface for KeyChain
  /// </summary>
  public interface IKeyChain
  {
        bool SetKey(string name, string value);
        bool SaveKey(string name, string value);
        string GetKey(string name);
        bool DeleteKey(string name);
    }
}
