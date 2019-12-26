using System;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
	public static class Hashing
	{
		public static string GetSHA512 (string str)
		{
			string hashStr = "";

			byte[] byteSourceText = Encoding.ASCII.GetBytes(str);

			var SHA512Hast = new SHA512CryptoServiceProvider();

			byte[] byteHash = SHA512Hast.ComputeHash(byteSourceText);

			foreach (byte b in byteHash)
			{
				hashStr += b.ToString("x2");
			}

			return hashStr;
		}

		public static string GetHashMD5 (string str)
		{
			try
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(str);
				Byte[] encodedBytes = md5.ComputeHash(originalBytes);

				string hash = BitConverter.ToString(encodedBytes).Replace("-", "").ToUpper();

				return hash;
			}
			catch (Exception ex)
			{
				throw new System.InvalidOperationException(ex.Message);
			}
		}


	}
}
