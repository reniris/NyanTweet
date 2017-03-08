using NyanTweet.Model;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NyanTweet.ViewModel
{
	class MainPageViewModel
	{
		public ReactiveProperty<string> TweetWord { get; }

		private MainSetting _setting = new MainSetting();

		public MainPageViewModel()
		{
			this.TweetWord = this._setting.TweetWord.ToReactiveProperty();
		}
	}
}
