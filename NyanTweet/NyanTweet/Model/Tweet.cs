using CoreTweet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NyanTweet.Model
{
	/// <summary>
	/// ツイート用クラス
	/// </summary>
	public class Tweet
	{
		/// <summary>
		/// カスタムスキーマ
		/// </summary>
		public const string CustomScheme = "nyantweet";
		/// <summary>
		/// カスタムスキーマのホスト
		/// </summary>
		public const string CustomSchemeHost = "oauth";

		/// <summary>
		/// The apikey
		/// </summary>
		private string APIkey;
		/// <summary>
		/// The apisecret
		/// </summary>
		private string APIsecret;
		/// <summary>
		/// The accesstoken
		/// </summary>
		private string Accesstoken;
		/// <summary>
		/// The accesstokensecret
		/// </summary>
		private string Accesstokensecret;

		/// <summary>
		/// ツイート用トークン
		/// </summary>
		private Tokens token = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tweet"/> class.
		/// </summary>
		public Tweet()
		{
			this.APIkey = TwitterAPIKey.APIKey;
			this.APIsecret = TwitterAPIKey.APISecret;
			this.Accesstoken = TwitterAPIKey.AccessToken;
			this.Accesstokensecret = TwitterAPIKey.AccessTokenSecret;
		}

		/// <summary>
		/// ツイートする
		/// </summary>
		/// <param name="text">ツイート文</param>
		/// <returns></returns>
		public async Task<StatusResponse> TweetTextAsync(string text)
		{
			if (token == null)
			{
				await OAuthAsync();
			}

			if (token != null)
			{
				StatusResponse res = null;
				try
				{
					res = await token.Statuses.UpdateAsync(new { status = text });
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
				return res;

			}
			return null;
		}

		/// <summary>
		/// OAuth認証
		/// </summary>
		/// <returns>認証されたトークン、トークンが取得できなかったりおかしい場合はnull</returns>
		private async Task<Tokens> OAuthAsync()
		{
			//コールバック用カスタムURLスキーマ
			string callback = CustomScheme + "://" + CustomSchemeHost;
			//認証
			var session = await OAuth.AuthorizeAsync(this.APIkey, this.APIsecret,callback);

			//ブラウザで認証ページを開いてコールバックでパラメータを取得
			var launchedUri = await DependencyService.Get<IAppHandler>().LaunchUri(session.AuthorizeUri.AbsoluteUri);
			if (string.IsNullOrEmpty(launchedUri) == true)
				return null;

			//クエリパラメータ文字列を取得
			string query = new Uri(launchedUri).GetComponents(UriComponents.Query, UriFormat.UriEscaped);

			//クエリパラメータをパース
			var parameters = string.IsNullOrEmpty(query)
							   ? new Dictionary<string, string>()
							   : query.Split('&')
										.Select(p => p.Split('='))
										.ToDictionary(p => p[0], p => 1 < p.Length ? Uri.UnescapeDataString(p[1]) : null);

			//oauth_token と oauth_verifierを取得
			string oauth_token, oauth_verifier;
			var o1 = parameters.TryGetValue("oauth_token",out oauth_token);
			var o2 = parameters.TryGetValue("oauth_verifier",out oauth_verifier);
			//パラメータが存在しなければnullを返す
			if (o1 == false || o2 == false)
				return null;

			Tokens t = null;
			try
			{
				//トークン取得
				t = await session.GetTokensAsync(oauth_verifier);
				// トークン有効性確認
				var res = await t.Account.VerifyCredentialsAsync();
			}
			catch (Exception)
			{
				//トークンが有効でないときはnullに
				t = null;
			}

			this.token = t;
			return t;
		}
	}
}
