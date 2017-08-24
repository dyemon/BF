using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MD5 {

	public static string Hash( string s) {
		using (var provider = System.Security.Cryptography.MD5.Create())
		{
			StringBuilder builder = new StringBuilder();                           

			foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
				builder.Append(b.ToString("x2").ToLower());

			return builder.ToString();
		}
	}
}
