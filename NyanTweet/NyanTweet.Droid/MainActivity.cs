using System;

using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;

namespace NyanTweet.Droid
{
	[Activity(Label = "NyanTweet.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTask)]
	[IntentFilter(
		actions: new[] { Intent.ActionView }
		, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }
		, DataScheme = NyanTweet.Model.Tweet.CustomScheme
	)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		/// <summary>
		/// Called when [create].
		/// </summary>
		/// <param name="bundle">The bundle.</param>
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
		}

		/// <summary>
		/// This is called for activities that set launchMode to "singleTop" in
		/// their package, or if a client used the <c><see cref="F:Android.Content.ActivityFlags.SingleTop" /></c>
		/// flag when calling <c><see cref="M:Android.Content.ContextWrapper.StartActivity(Android.Content.Intent)" /></c>.
		/// </summary>
		/// <param name="intent">The new intent that was started for the activity.</param>
		/// <remarks>
		/// <para tool="javadoc-to-mdoc">This is called for activities that set launchMode to "singleTop" in
		/// their package, or if a client used the <c><see cref="F:Android.Content.ActivityFlags.SingleTop" /></c>
		/// flag when calling <c><see cref="M:Android.Content.ContextWrapper.StartActivity(Android.Content.Intent)" /></c>.  In either case, when the
		/// activity is re-launched while at the top of the activity stack instead
		/// of a new instance of the activity being started, onNewIntent() will be
		/// called on the existing instance with the Intent that was used to
		/// re-launch it.
		/// </para>
		/// <para tool="javadoc-to-mdoc">An activity will always be paused before receiving a new intent, so
		/// you can count on <c><see cref="M:Android.App.Activity.OnResume" /></c> being called after this method.
		/// </para>
		/// <para tool="javadoc-to-mdoc">Note that <c><see cref="P:Android.App.Activity.Intent" /></c> still returns the original Intent.  You
		/// can use <c><see cref="P:Android.App.Activity.Intent" /></c> to update it to this new Intent.</para>
		/// <para tool="javadoc-to-mdoc">
		///   <format type="text/html">
		///     <a href="http://developer.android.com/reference/android/app/Activity.html#onNewIntent(android.content.Intent)" target="_blank">[Android Documentation]</a>
		///   </format>
		/// </para>
		/// </remarks>
		/// <since version="Added in API level 1" />
		/// <altmember cref="P:Android.App.Activity.Intent" />
		/// <altmember cref="P:Android.App.Activity.Intent" />
		/// <altmember cref="M:Android.App.Activity.OnResume" />
		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			this.Intent = intent;
		}

		/// <summary>
		/// OnResumeでURLがnullでない場合にイベント発生
		/// </summary>
		public event Action<string> Resume;
		
		/// <summary>
		/// 叩かれたURLをアプリ側で解析
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();

			var intent = this.Intent;

			if (Intent.ActionView.Equals(intent.Action))
			{
				var uri = intent.Data;

				if (uri != null)
				{
					if (this.Resume != null)
						this.Resume(uri.ToString());
				}
			}
		}
	}
}

