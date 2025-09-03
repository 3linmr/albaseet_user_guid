using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Extensions
{
	public static class SessionExtension
	{
		public static void SetString(this ISession session, string key, string value)
		{
			session.Set(key, Encoding.UTF8.GetBytes(value));
		}
		public static string GetString(this ISession session, string key)
		{
			return session.GetString(key);
		}

		//public static string GetString(this ISession session, string key)
		//{
		//	var data = session.Get(key);
		//	if (data == null)
		//	{
		//		return null;
		//	}
		//	return Encoding.UTF8.GetString(data);
		//}
	}
}
