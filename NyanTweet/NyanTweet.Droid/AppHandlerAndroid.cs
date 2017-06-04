using Android.App;
using Android.Content;
using NyanTweet.Droid;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AppHandlerAndroid))]
namespace NyanTweet.Droid
{
	/// <summary>
	/// アプリ連携
	/// 参考URL
	/// https://xamarinhelp.com/launching-mobile-app-via-uri-scheme/
	/// https://forums.xamarin.com/discussion/81278/how-to-handle-the-result-of-startactivityforresult-in-forms
	/// </summary>
	/// <seealso cref="NyanTweet.Model.IAppHandler" />
	public class AppHandlerAndroid : NyanTweet.Model.IAppHandler
	{
		/// <summary>
		/// リクエストコード
		/// </summary>
		public const int RCODE = 2;

		/// <summary>
		/// URLを開いて応答を待つ
		/// </summary>
		/// <param name="uri">URI</param>
		/// <returns></returns>
		public Task<string> LaunchUri(string uri)
		{
			var aUri = Android.Net.Uri.Parse(uri);
			var intent = new Intent(Intent.ActionView, aUri);
			var activity = (MainActivity)Forms.Context;
			var listener = new ResumeListener(activity);
			
			activity.StartActivityForResult(intent, RCODE);
			//Xamarin.Forms.Forms.Context.StartActivity(intent);

			return listener.Task;
		}

		/// <summary>
		/// Resumeイベントリスナー
		/// </summary>
		private class ResumeListener
		{
			/// <summary>
			/// The complete
			/// </summary>
			private TaskCompletionSource<string> Complete = new TaskCompletionSource<string>();
			/// <summary>
			/// Gets the task.
			/// </summary>
			/// <value>
			/// The task.
			/// </value>
			public Task<string> Task { get { return this.Complete.Task; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="ResumeListener"/> class.
			/// </summary>
			/// <param name="activity">The activity.</param>
			public ResumeListener(MainActivity activity)
			{
				activity.Resume += OnResume;
			}

			/// <summary>
			/// Called when [resume].
			/// </summary>
			/// <param name="uri">The URI.</param>
			private void OnResume(string uri)
			{
				var context = Forms.Context;
				var activity = (MainActivity)context;
				activity.Resume -= OnResume;
				
				this.Complete.TrySetResult(uri);
			}
		}
	}
}