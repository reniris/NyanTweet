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
                _TweetWord = value;

                SaveJson(this);
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MainSetting"/> class from being created.
        /// </summary>
        private MainSetting()
        {

        }

        /// <summary>
        /// ファイル名
        /// </summary>
        private const string TextFileName = "setting.json";

        /// <summary>
        /// JSONに書き出す
        /// </summary>
        /// <param name="data">書き出すデータ</param>
        static private void SaveJson(MainSetting data)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var task = SaveTextAsync(json);
            task.Wait();
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
        /// JSONからロード
        /// </summary>
        /// <returns></returns>
        static public MainSetting LoadJson()
        {
            var jsonstring = LoadTextAsync().Result;
            MainSetting data;
            if (String.IsNullOrEmpty(jsonstring) == false)
                data = JsonConvert.DeserializeObject<MainSetting>(jsonstring);
            else
                data = new MainSetting();

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
