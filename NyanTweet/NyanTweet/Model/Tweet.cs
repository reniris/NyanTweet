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
		public Tokens Token { get; private set; } = null;

		/// <summary>
		/// Gets the last error message.
		/// </summary>
		/// <value>
		/// The last error message.
		/// </value>
		public string LastErrMessage { get; private set; } = null;

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
		/// トークンの初期化
		/// </summary>
		/// <param name="token">アクセストークン文字列</param>
		/// <param name="secret">アクセストークンsecret</param>
		/// <returns></returns>
		public async Task<bool> InitTokenAsync(string token, string secret)
		{
			//トークンがすでに存在する場合には何もしない
			if (this.Token != null)
				return true;

			bool t1, t2;

			//トークンが復元できる場合はそれをつかう
			if (this.Token == null)
				t1 = await CreateTokenAsync(token, secret);

			//トークンがない場合は認証する
			if (this.Token == null)
				t2 = await OAuthAsync();

			return (this.Token != null);
		}

		/// <summary>
		/// アクセストークンを作成
		/// </summary>
		/// <param name="token">accesstoken</param>
		/// <param name="secret">accesssecret</param>
		/// <returns>トークンを作成できればtrueそうでなければfalse</returns>
		private async Task<bool> CreateTokenAsync(string token, string secret)
		{
			Tokens t = null;
			if (string.IsNullOrEmpty(token) == true || string.IsNullOrEmpty(secret) == true)
			{
				t = null;
			}
			else
			{
				try
				{
					//トークンを作成
					t = Tokens.Create(this.APIkey, this.APIsecret, token, secret);
					// トークン有効性確認
					var res = await t.Account.VerifyCredentialsAsync();

					this.Token = t;
				}
				catch (Exception ex)
				{
					t = null;
					this.LastErrMessage = ex.Message;
				}
			}

			this.Token = t;
			return (t != null);
		}

		/// <summary>
		/// ツイートする
		/// </summary>
		/// <param name="text">ツイート文</param>
		/// <returns></returns>
		public async Task<StatusResponse> TweetTextAsync(string text)
		{
			if (this.Token == null)
				return null;

			StatusResponse res = null;
			try
			{
				res = await this.Token.Statuses.UpdateAsync(new { status = text });
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				this.LastErrMessage = ex.Message;
			}
			return res;
		}

		/// <summary>
		/// OAuth認証
		/// </summary>
		/// <returns>認証が成功して有効なトークンが取得できればtrue、そうでない場合はfalse</returns>
		private async Task<bool> OAuthAsync()
		{
			//コールバック用カスタムURLスキーマ
			string callback = CustomScheme + "://" + CustomSchemeHost;
			//認証
			var session = await OAuth.AuthorizeAsync(this.APIkey, this.APIsecret, callback);

			//ブラウザで認証ページを開いてコールバックでパラメータを取得
			var launchedUri = await DependencyService.Get<IAppHandler>().LaunchUri(session.AuthorizeUri.AbsoluteUri);
			if (string.IsNullOrEmpty(launchedUri) == true)
				return false;

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
			var o1 = parameters.TryGetValue("oauth_token", out oauth_token);
			var o2 = parameters.TryGetValue("oauth_verifier", out oauth_verifier);
			//パラメータが存在しなければnullを返す
			if (o1 == false || o2 == false)
				return false;

			Tokens t = null;
			try
			{
				//トークン取得
				t = await session.GetTokensAsync(oauth_verifier);
				// トークン有効性確認
				var res = await t.Account.VerifyCredentialsAsync();
			}
			catch (Exception ex)
			{
				//トークンが有効でないときはnullに
				t = null;
				this.LastErrMessage = ex.Message;
			}

			this.Token = t;
			return (t != null);
		}
	}
}
