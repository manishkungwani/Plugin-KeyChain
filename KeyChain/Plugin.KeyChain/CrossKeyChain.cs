using Plugin.KeyChain.Abstractions;
using System;

namespace Plugin.KeyChain
{
  /// <summary>
  /// Cross platform KeyChain implemenations
  /// </summary>
  public class CrossKeyChain
  {
    static Lazy<IKeyChain> Implementation = new Lazy<IKeyChain>(() => CreateKeyChain(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IKeyChain Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IKeyChain CreateKeyChain()
    {
#if PORTABLE
        return null;
#else
		return new KeyChainImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
