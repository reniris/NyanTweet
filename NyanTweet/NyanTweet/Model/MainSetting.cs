using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace NyanTweet.Model
{
	class MainSetting
	{
		private const string DEF_TWEET = "にゃーん";

		//public string TweetWord { get; set; }
		public ReactiveProperty<string> TweetWord { get; }

		public MainSetting()
		{
			this.TweetWord = new ReactiveProperty<string>(ImmediateScheduler.Instance, DEF_TWEET);
		}
	}
}
