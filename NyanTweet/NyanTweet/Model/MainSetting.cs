using Newtonsoft.Json;
using PCLStorage;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace NyanTweet.Model
{
    /// <summary>
    /// 
    /// </summary>
    class MainSetting
    {
        /// <summary>
        /// デフォルトのツイート文字列
        /// </summary>
        private const string DEF_TWEET = "にゃーん";

        /// <summary>
        /// The tweet word
        /// </summary>
        private string _TweetWord = DEF_TWEET;

        /// <summary>
        /// ツイート文字列
        /// </summary>
        /// <value>
        /// The tweet word.
        /// </value>
        [DefaultValue(DEF_TWEET)]
        public string TweetWord
        {
            get
            {
                return _TweetWord;
            }
            set
            {
				if (_TweetWord != value)
				{
					_TweetWord = value;
					Save(this);
				}
            }
        }
		
		/// <summary>
		/// The access token
		/// </summary>
		private string _AccessToken = null;

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>
		/// The access token.
		/// </value>
		[JsonProperty]
		public string AccessToken
		{
			get { return _AccessToken; }
			private set { _AccessToken = value; }
		}

		/// <summary>
		/// The access token secret
		/// </summary>
		private string _AccessTokenSecret = null;

		/// <summary>
		/// Gets or sets the access token secret.
		/// </summary>
		/// <value>
		/// The access token secret.
		/// </value>
		[JsonProperty]
		public string AccessTokenSecret
		{
			get { return _AccessTokenSecret; }
			private set { _AccessTokenSecret = value; }
		}

		/// <summary>
		/// Prevents a default instance of the <see cref="MainSetting"/> class from being created.
		/// </summary>
		private MainSetting()
        {

        }

		/// <summary>
		/// アクセストークンをセット
		/// </summary>
		/// <param name="token">accesstoken</param>
		/// <param name="secret">accesssecret</param>
		public void SetAccessToken(string token, string secret)
		{
			//変更がない場合は何もしない
			if (this.AccessToken == token && this.AccessTokenSecret == secret)
				return;

			this.AccessToken = token;
			this.AccessTokenSecret = secret;

			//ファイルに書き込む
			Save(this);
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		private const string TextFileName = "setting.json";

		/// <summary>
		/// データをセーブ
		/// </summary>
		static public void Save(MainSetting data)
		{
			var json = SaveJson(data);
			var task = SaveTextAsync(json);
			task.Wait();
		}

        /// <summary>
        /// JSONに書き出す
        /// </summary>
        /// <param name="data">書き出すデータ</param>
        static private string SaveJson(MainSetting data)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			return json;
        }
        
        /// <summary>
        /// ユーザーデータをファイルに書き出す
        /// </summary>
        /// <param name="text">書き出す文字列</param>
        /// <returns></returns>
        static private async Task<string> SaveTextAsync(string text)
        {
            // ユーザーデータ保存フォルダー
            PCLStorage.IFolder localFolder = PCLStorage.FileSystem.Current.LocalStorage;

            // ファイルを作成、または、取得する
            PCLStorage.IFile file
              = await localFolder.CreateFileAsync(TextFileName,
                                  PCLStorage.CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

            // テキストをファイルに書き込む
            await file.WriteAllTextAsync(text).ConfigureAwait(false);

            return file.Path;
        }

		/// <summary>
		/// データをロード
		/// </summary>
		/// <returns>ロードしたデータ　ロードできなかった場合は初期値でインスタンスを作成</returns>
		static public MainSetting Load()
		{
			var jsonstring = LoadTextAsync();
			MainSetting data = LoadJson(jsonstring.Result);
			if(data == null)
				data = new MainSetting();

			return data;
		}

        /// <summary>
        /// JSONからロード
        /// </summary>
        /// <returns></returns>
        static private MainSetting LoadJson(string jsonstring)
        {
            MainSetting data = null;
            if (String.IsNullOrEmpty(jsonstring) == false)
                data = JsonConvert.DeserializeObject<MainSetting>(jsonstring);

			return data;
        }
        
        /// <summary>
        /// ユーザーデータをファイルから読み取る
        /// </summary>
        /// <returns></returns>
        static private async Task<string> LoadTextAsync()
        {
            // ユーザーデータ保存フォルダー
            PCLStorage.IFolder localFolder = PCLStorage.FileSystem.Current.LocalStorage;

            ExistenceCheckResult res = await localFolder.CheckExistsAsync(TextFileName).ConfigureAwait(false);
            if (res == ExistenceCheckResult.NotFound) { return null; }

            // ファイルを取得する
            PCLStorage.IFile file = await localFolder.GetFileAsync(TextFileName).ConfigureAwait(false);

            // テキストファイルを読み込む
            return await file.ReadAllTextAsync().ConfigureAwait(false);
        }
    }
}
