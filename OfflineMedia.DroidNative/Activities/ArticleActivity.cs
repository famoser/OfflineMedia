using Android.App;
using Android.OS;
using Android.Widget;

namespace OfflineMediaV3.DroidNative.Activities
{
    [Activity(Label = "ArticleActivity")]
    public class ArticleActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ArticlePage);

            var button = FindViewById<Button>(Resource.Id.Button2);

        }
    }
}