using Android.App;
using Android.OS;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.DroidNative.Services;
using OfflineMediaV3.View.ViewModels;
using OfflineMediaV3.View.ViewModels.Global;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

namespace OfflineMediaV3.DroidNative.Activities
{
    [Activity(Label = "OfflineMediaV3.DroidNative", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ActivityBase, AdapterView.IOnItemClickListener
    {
        private static bool _initialized;

        public MainPageViewModel MainPageViewModel { get { return BaseViewModelLocator.Instance.MainPageViewModel; } }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (!_initialized)
            {
                _initialized = true;
                RuntimeObjects.Context = this;

                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

                var nav = new NavigationService();
                nav.Configure(PageKeys.Main.ToString(), typeof(MainActivity));
                nav.Configure(PageKeys.Article.ToString(), typeof(ArticleActivity));

                SimpleIoc.Default.Register<INavigationService>(() => nav);

                // Create design time view services and models
                SimpleIoc.Default.Register<IProgressService, ProgressService>();
                SimpleIoc.Default.Register<IStorageService, StorageService>();
                SimpleIoc.Default.Register<IDialogService, DialogService>();

                SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformAndroid>();

                //create global viewmodels
                SimpleIoc.Default.Register<ProgressViewModel>();
                
                Messenger.Default.Register<Messages>(this, EvaluateMessages);


                var vm = BaseViewModelLocator.Instance.MainPageViewModel;
            }
        }

        private void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.MainPageInitialized)
            {
                var view = FindViewById<ListView>(Resource.Id.SourceView);
                view.Adapter = MainPageViewModel.Sources[0].FeedList[0].ArticleList.GetAdapter(ArticleAdapter);
                view.OnItemClickListener = this;
            }
        }

        private global::Android.Views.View ArticleAdapter(int arg1, ArticleModel article, global::Android.Views.View view)
        {
            // Not reusing views here
            view = LayoutInflater.Inflate(Resource.Layout.ArticleLayout, null);

            var title = view.FindViewById<TextView>(Resource.Id.TitleTextView);
            title.Text = article.Title;

            var desc = view.FindViewById<TextView>(Resource.Id.SubtitleTextView);
            desc.Text = article.SubTitle;
            

            return view;
        }

        public EditText QueryEditText { get; private set; }
        public void OnItemClick(AdapterView parent, global::Android.Views.View view, int position, long id)
        {
            SimpleIoc.Default.GetInstance<NavigationService>().NavigateTo(PageKeys.Article.ToString());
            Messenger.Default.Send(MainPageViewModel.Sources[0].FeedList[0].ArticleList[position], Messages.Select);
        }
    }
}

