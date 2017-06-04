using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NyanTweet.Model
{
	/// <summary>
	/// アプリ起動インターフェース
	/// 参考URL https://xamarinhelp.com/launching-mobile-app-via-uri-scheme/
	/// </summary>
	public interface IAppHandler
	{
		/// <summary>
		/// URLを開いて応答を待つ
		/// </summary>
		/// <param name="uri">URI</param>
		/// <returns>コールバックURL</returns>
		Task<string> LaunchUri(string uri);
	}
}
