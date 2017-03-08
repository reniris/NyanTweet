using NyanTweet.Model;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NyanTweet.ViewModel
{
	class MainPageViewModel
	{
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "エラー")]
        public ReactiveProperty<string> TweetWord { get; }

        public ReactiveProperty<string> Message { get; }

        /// <summary>
        /// 
        /// </summary>
        public ReactiveCommand TweetCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        private MainSetting _setting = new MainSetting();

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public MainPageViewModel()
		{
			this.TweetWord = this._setting.TweetWord.ToReactiveProperty().SetValidateAttribute(() => this.TweetWord);

            this.Message = new ReactiveProperty<string>("");

            this.TweetCommand = this.TweetWord.Select(s => 0 < s.Length && s.Length <= 140).ToReactiveCommand();

            this.TweetCommand.Subscribe(_ => this.Message.Value = this.TweetWord.Value);

        }
	}
}
