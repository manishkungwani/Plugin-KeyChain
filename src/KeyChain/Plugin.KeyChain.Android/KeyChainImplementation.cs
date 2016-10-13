using System;
using Java.Security;
using Javax.Crypto;
using Java.IO;
using Android.Content;
using Android.Runtime;
using Plugin.KeyChain.Abstractions;
using Android.App;

namespace Plugin.KeyChain
{
	/// <summary>
	/// Implementation for Feature
	/// </summary>
	public class KeyChainImplementation : IKeyChain
	{
		readonly KeyStore _androidKeyStore;
		static readonly object _fileLock = new object();
		static string _serviceId = "AndroidAccountStore";

		public KeyChainImplementation()
		{
			_androidKeyStore = KeyStore.GetInstance(KeyStore.DefaultType);

			try
			{
				lock (_fileLock)
				{
					using (var s = Application.Context.OpenFileInput(_serviceId))
					{
						_androidKeyStore.Load(s, _serviceId.ToCharArray());
					}
				}
			}
			catch (FileNotFoundException)
			{
				LoadEmptyKeyStore(_serviceId.ToCharArray());
			}
		}

		public void SetKey(string keyName, string keyValue)
		{
			var existing = GetKey(keyName);

			if (!string.IsNullOrEmpty(existing))
			{
				DeleteKey(keyName);
			}

			var alias = MakeAlias(keyName, _serviceId);
			var secretKey = new SecretAccount(keyValue);
			var entry = new KeyStore.SecretKeyEntry(secretKey);
			_androidKeyStore.SetEntry(alias, entry, new KeyStore.PasswordProtection(_serviceId.ToCharArray()));

			Save();
		}

		public string GetKey(string keyName)
		{
			var wantedAlias = MakeAlias(keyName, _serviceId).ToLower();

			var aliases = _androidKeyStore.Aliases();
			while (aliases.HasMoreElements)
			{
				var alias = aliases.NextElement().ToString();
				if (alias.ToLower().Contains(wantedAlias))
				{
					var e = _androidKeyStore.GetEntry(alias, new KeyStore.PasswordProtection(_serviceId.ToCharArray())) as KeyStore.SecretKeyEntry;
					if (e != null)
					{
						var bytes = e.SecretKey.GetEncoded();
						//return System.Text.Encoding.UTF8.GetString(bytes);
						return bytes.GetString();
					}
				}
			}
			return "";
		}

		public void DeleteKey(string keyName)
		{
			var alias = MakeAlias(keyName, _serviceId);
			_androidKeyStore.DeleteEntry(alias);

			Save();
		}

		private void Save()
		{
			lock (_fileLock)
			{
				using (var s = Application.Context.OpenFileOutput(_serviceId, FileCreationMode.Private))
				{
					_androidKeyStore.Store(s, _serviceId.ToCharArray());
				}
			}
		}

		private static string MakeAlias(string keyName, string serviceId)
		{
			return keyName + "-" + serviceId;
		}

		class SecretAccount : Java.Lang.Object, ISecretKey
		{
			byte[] bytes;
			public SecretAccount(string password)
			{
				//bytes = System.Text.Encoding.UTF8.GetBytes(password);
				bytes = password.GetBytes();
			}
			public byte[] GetEncoded()
			{
				return bytes;
			}
			public string Algorithm
			{
				get
				{
					return "RAW";
				}
			}
			public string Format
			{
				get
				{
					return "RAW";
				}
			}
		}

		static IntPtr id_load_Ljava_io_InputStream_arrayC;

		/// <summary>
		/// Work around Bug https://bugzilla.xamarin.com/show_bug.cgi?id=6766
		/// </summary>
		void LoadEmptyKeyStore(char[] password)
		{
			if (id_load_Ljava_io_InputStream_arrayC == IntPtr.Zero)
			{
				id_load_Ljava_io_InputStream_arrayC = JNIEnv.GetMethodID(_androidKeyStore.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = JNIEnv.NewArray(password);
			JNIEnv.CallVoidMethod(_androidKeyStore.Handle, id_load_Ljava_io_InputStream_arrayC, new JValue[]
				{
					new JValue (intPtr),
					new JValue (intPtr2)
				});
			JNIEnv.DeleteLocalRef(intPtr);
			if (password != null)
			{
				JNIEnv.CopyArray(intPtr2, password);
				JNIEnv.DeleteLocalRef(intPtr2);
			}
		}
	}
}