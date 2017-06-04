using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using NyanTweet.iOS;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandleriOS))]
namespace NyanTweet.iOS
{
	/// <summary>
	/// アプリ連携
	/// 参考URL https://xamarinhelp.com/launching-mobile-app-via-uri-scheme/
	/// 未実装部分多数のため注意
	/// </summary>
	public class AppHandleriOS : NyanTweet.Model.IAppHandler
	{
		/// <summary>
		/// URLを開いて応答を待つ
		/// </summary>
		/// <param name="uri">URI</param>
		/// <returns></returns>
		public Task<string> LaunchUri(string uri)
		{
			var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri));

			if (!canOpen)
				return Task.FromResult("");

			throw new NotImplementedException();
			//UIApplication.SharedApplication.OpenUrl(new NSUrl(uri))
		}
	}
}