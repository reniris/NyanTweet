using NyanTweet.Model;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NyanTweet.ViewModel
{
	class MainPageViewModel : IDisposable
    {
        /// <summary>
        /// ツイートする文
        /// </summary>
        [Required(ErrorMessage = "エラー")]
        public ReactiveProperty<string> TweetWord { get; }

        /// <summary>
        /// メッセージテキスト
        /// </summary>
        public ReactiveProperty<string> Message { get; }

        /// <summary>
        /// ツイート文設定エリアの表示・非表示
        /// </summary>
        public ReactiveProperty<bool> IsVisibleSetting { get; }

        /// <summary>
        /// ツイート文設定エリア
        /// </summary>
        public ReactiveProperty<string> SettingText { get; }

        /// <summary>
        /// 設定ボタン表示文
        /// </summary>
        public ReactiveProperty<string> SettingButtonText { get; }

        /// <summary>
        /// ツイート用コマンド
        /// </summary>
        public ReactiveCommand TweetCommand { get; }

        /// <summary>
        /// ツイート文設定コマンド
        /// </summary>
        public ReactiveCommand SettingCommand { get; }

        /// <summary>
        /// 設定用クラス
        /// </summary>
        private MainSetting _setting = new MainSetting();

        /// <summary>
        /// 設定ボタン表示文のリスト
        /// </summary>
        private static readonly string[] SettingButtonTextList = new string[] { "設定", "確定" };
        
        /// <summary>
        /// 設定ボタン表示文のインデックス
        /// </summary>
        private int SettingButtonTextIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public MainPageViewModel()
        {
            //プロパティの初期化
            this.TweetWord = ReactiveProperty.FromObject(this._setting, x => x.TweetWord);
            this.SettingText = new ReactiveProperty<string>(this.TweetWord.Value);
            this.Message = new ReactiveProperty<string>("");
            this.IsVisibleSetting = new ReactiveProperty<bool>(false);
            this.SettingButtonText = new ReactiveProperty<string>(SettingButtonTextList[SettingButtonTextIndex]);

            //コマンドの初期化
            this.TweetCommand = this.TweetWord.Select(s => IsTweet(s)).ToReactiveCommand().AddTo(this.Disposable);
            this.SettingCommand = this.SettingText.Select(s => IsTweet(s)).ToReactiveCommand().AddTo(this.Disposable);

            //ツイートコマンドの実装
            this.TweetCommand.Subscribe(_ => this.Message.Value = this.TweetWord.Value);
            
            //ツイート文設定コマンドの実装
            this.SettingCommand.Subscribe(_ =>
            {
                //ツイート文を変更
                this.TweetWord.Value = this.SettingText.Value;
                //ツイート文設定エリアの表示切替
                this.IsVisibleSetting.Value = !this.IsVisibleSetting.Value;

                //ボタンテキスト設定、確定の切り替え
                this.SettingButtonText.Value = SettingButtonTextList[SettingButtonTextIndex ^= 1];
            });
        }

        /// <summary>
        /// ツイートできるかどうか
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        private static bool IsTweet(string tweet)
        {
            return 0 < tweet.Length && tweet.Length <= 140;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
