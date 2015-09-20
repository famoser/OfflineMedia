using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Enums.Settings;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Repositories;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources;
using OfflineMediaV3.Common.Enums.View;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.View.Enums;

namespace OfflineMediaV3.View.ViewModels
{
    public class ArticlePageViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IVariaService _variaService;

        public ArticlePageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository, IVariaService variaService)
        {
            Messenger.Default.Register<ArticleModel>(this, Messages.Select, EvaluateMessage);

            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;
            _variaService = variaService;

            if (IsInDesignMode)
            {
                _displayState = DisplayState.Spritz;
                _article = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0].ArticleList[0];
                _similarCathegoriesArticles = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];
                _similarTitlesArticles = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];

                _fontSize = new SettingModel()
                {
                    Description = "Font Size",
                    ValueType = ValueTypeEnum.Int,
                    IntValue = 15
                };

                _spritzSpeed = new SettingModel()
                {
                    Description = "Spritz Speed",
                    ValueType = ValueTypeEnum.Int,
                    IntValue = 350
                };
            }
            else
            {
                Initialize();
            }

            _setDisplayState = new RelayCommand<DisplayState>(SetDisplayState);

            _makeFontBiggerCommand = new RelayCommand(MakeFontBigger, () => CanMakeFontBigger);
            _makeFontSmallerCommand = new RelayCommand(MakeFontSmaller, () => CanMakeFontSmaller);
            _favoriteCommand = new RelayCommand(Favorite, () => CanFavorite);
            _openInBrowserCommand = new RelayCommand(OpenInBrowser, () => CanOpenInBrowser);

            _goToStartCommand = new RelayCommand(GoToStart, () => CanGoToStart);
            _goLeftCommand = new RelayCommand(GoLeft, () => CanGoLeft);
            _goRightCommand = new RelayCommand(GoRight, () => CanGoRight);

            _startCommand = new RelayCommand(Start, () => CanStart);

            _increaseSpeedCommand = new RelayCommand(IncreaseSpeed, () => CanIncreaseSpeed);
            _decreaseSpeedCommand = new RelayCommand(DecreaseSpeed, () => CanDecreaseSpeed);
        }

        private async void EvaluateMessage(ArticleModel obj)
        {
            SetDisplayState(DisplayState.Article);
            if (!obj.IsStatic)
            {
                Article = await _articleRepository.GetCompleteArticle(obj.Id);
                if (Article.State == ArticleState.New)
                {
                    Article.State = ArticleState.Loading;

                    IMediaSourceHelper sh = ArticleHelper.Instance.GetMediaSource(Article);
                    if (sh == null)
                    {
                        LogHelper.Instance.Log(LogLevel.Warning, this,
                            "ArticleHelper.DownloadArticle: Tried to Download Article which cannot be downloaded");
                        Article.State = ArticleState.WrongSourceFaillure;
                    }
                    else
                    {
                        if (sh.NeedsToEvaluateArticle())
                            Article = await _articleRepository.ActualizeArticle(Article, sh);
                    }
                    
                    Article.State = ArticleState.Read;
                    await _articleRepository.UpdateArticle(Article);
                }
                else if (Article.State == ArticleState.Loaded)
                {
                    Article.State = ArticleState.Read;
                    await _articleRepository.UpdateArticle(Article);
                }

                Messenger.Default.Send(Article.Id, Messages.FeedArticleRefresh);

                InitializeSpritz();

                SimilarCathegoriesArticles =
                    new FeedModel()
                    {
                        CustomTitle = "Ähnliche Kategorien"
                    };

                SimilarTitlesArticles =
                    new FeedModel()
                    {
                        CustomTitle = "Ähnlicher Inhalt"
                    };

                SimilarCathegoriesArticles.ArticleList =
                    await _articleRepository.GetSimilarCathegoriesArticles(Article, 5);

                SimilarTitlesArticles.ArticleList = await _articleRepository.GetSimilarTitlesArticles(Article, 5);
            }
            else
            {
                Article = obj;
                InitializeSpritz();
            }
        }

        private async void Initialize()
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                _fontSize = await _settingsRepository.GetSettingByKey(SettingKeys.BaseFontSize, await unitOfWork.GetDataService());
                _spritzSpeed = await _settingsRepository.GetSettingByKey(SettingKeys.WordsPerMinute, await unitOfWork.GetDataService());
            }

            RaisePropertyChanged(() => FontSize);
            RaisePropertyChanged(() => ReadingSpeed);
        }

        private ArticleModel _article;
        public ArticleModel Article
        {
            get { return _article; }
            set
            {
                if (Set(ref _article, value))
                    _openInBrowserCommand.RaiseCanExecuteChanged();
            }
        }

        private DisplayState _displayState;
        public DisplayState DisplayState
        {
            get { return _displayState; }
            set { Set(ref _displayState, value); }
        }

        #region SetStateCommand
        private readonly RelayCommand<DisplayState> _setDisplayState;
        public ICommand SetDisplayStateCommand => _setDisplayState;

        private void SetDisplayState(DisplayState state)
        {
            DisplayState = state;
        }

        public DisplayState DisplayStateArticle => DisplayState.Article;
        public DisplayState DisplayStateSpritz => DisplayState.Spritz;
        public DisplayState DisplayStateInfo => DisplayState.Info;
        #endregion

        #region Article View
        
        private SettingModel _fontSize;
        public int FontSize => (_fontSize == null || _fontSize.IntValue == 0) ? 15 : _fontSize.IntValue;

        #region MakeFontBiggerCommand
        private readonly RelayCommand _makeFontBiggerCommand;
        public ICommand MakeFontBiggerCommand => _makeFontBiggerCommand;

        private bool CanMakeFontBigger => _fontSize.IntValue < 40;

        private void MakeFontBigger()
        {
            _fontSize.IntValue += 2;
            RaisePropertyChanged(() => FontSize);
            _settingsRepository.SaveSetting(_fontSize);
            _makeFontBiggerCommand.RaiseCanExecuteChanged();
            _makeFontSmallerCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region MakeFontSmallerCommand
        private readonly RelayCommand _makeFontSmallerCommand;
        public ICommand MakeFontSmallerCommand => _makeFontSmallerCommand;

        private bool CanMakeFontSmaller => _fontSize.IntValue > 5;

        private void MakeFontSmaller()
        {
            _fontSize.IntValue -= 2;
            RaisePropertyChanged(() => FontSize);
            _settingsRepository.SaveSetting(_fontSize);
            _makeFontBiggerCommand.RaiseCanExecuteChanged();
            _makeFontSmallerCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region FavoriteCommand
        private readonly RelayCommand _favoriteCommand;
        public ICommand FavoriteCommand => _favoriteCommand;

        private bool CanFavorite => true;

        private async void Favorite()
        {
            Article.IsFavorite = !Article.IsFavorite;
            await _articleRepository.UpdateArticle(Article);
            Messenger.Default.Send(Messages.FavoritesChanged);
        }
        #endregion

        #endregion

        #region Spritz View

        private SettingModel _spritzSpeed;

        public int ReadingSpeed => _spritzSpeed.IntValue;

        private string _beforeText = "S";
        public string BeforeText
        {
            get { return _beforeText; }
            set { Set(ref _beforeText, value); }
        }

        private char _middleText = 'p';
        public char MiddleText
        {
            get { return _middleText; }
            set { Set(ref _middleText, value); }
        }

        private string _afterText = "ritz";
        public string AfterText
        {
            get { return _afterText; }
            set { Set(ref _afterText, value); }
        }

        public void DisplayWord(SpritzWord sw)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BeforeText = sw.Before;
                MiddleText = sw.Middle;
                AfterText = sw.After;
            });
        }

        private bool _isSpritzReady;

        private int _activeIndexSave;
        private int _activeIndex
        {
            get { return _activeIndexSave; }
            set
            {
                if (Set(ref _activeIndexSave,value))
                {
                    RaisePropertyChanged(() => ActiveWord);
                }
            }
        }
        public int ActiveWord => _spritzWords != null ? _activeIndexSave + 1 : 0;

        public int TotalWords => _spritzWords?.Count ?? 0;

        private List<SpritzWord> _spritzWords;
        private void InitializeSpritz()
        {
            _isSpritzReady = false;
            _activeIndex = 0;
            _spritzState = SpritzState.Ready;
            _spritzWords = new List<SpritzWord>();

            DisplayWord(new SpritzWord()
            {
                After = "ding",
                Before = "Lo",
                Middle = 'd',
                Lenght = 1000
            });
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _goLeftCommand.RaiseCanExecuteChanged();
                _goRightCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
                _startCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => TotalWords);
            });

            //inititialize Wordlists
            if (Article.Content != null && Article.Content.Any())
            {
                _spritzWords = SpritzHelper.GenerateList(Article.Content);

                //prepare User Interface
                if (_spritzWords != null && _spritzWords.Count > 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _goLeftCommand.RaiseCanExecuteChanged();
                        _goRightCommand.RaiseCanExecuteChanged();
                        RaisePropertyChanged(() => TotalWords);
                    });

                    DisplayWord(_spritzWords[0]);

                    _isSpritzReady = true;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _startCommand.RaiseCanExecuteChanged();
                    });
                }
            }
        }

        private async void DisplayNextWord()
        {
            if (_spritzWords == null)
                _spritzWords = SpritzHelper.GenerateList(Article.Content);

            int delay = (60 * 1000) / ReadingSpeed;

            if (SpritzState == SpritzState.Running && _activeIndex < _spritzWords.Count)
            {
                if (++_activeIndex < _spritzWords.Count)
                {
                    DisplayWord(_spritzWords[_activeIndex]);
                    await Task.Delay(delay);
                    DisplayNextWord();
                }
                else
                {
                    SpritzState = SpritzState.Finished;
                }
            }
        }

        #region buttons

        #region Go ToStart Button
        private readonly RelayCommand _goToStartCommand;
        public ICommand GoToStartCommand => _goToStartCommand;

        private bool CanGoToStart => _activeIndex > 0 && SpritzState != SpritzState.Running;

        private void GoToStart()
        {
            _activeIndex = 0;
            DisplayWord(_spritzWords[_activeIndex]);

            _goRightCommand.RaiseCanExecuteChanged();
            _goLeftCommand.RaiseCanExecuteChanged();
            _goToStartCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Go Left Button
        private readonly RelayCommand _goLeftCommand;
        public ICommand GoLeftCommand => _goLeftCommand;

        private bool CanGoLeft => _activeIndex > 0 && SpritzState != SpritzState.Running;

        private void GoLeft()
        {
            if (--_activeIndex > 0)
            {
                DisplayWord(_spritzWords[_activeIndex]);
                _goRightCommand.RaiseCanExecuteChanged();
            }
            else
            {
                DisplayWord(_spritzWords[0]);
                _goLeftCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Go Right Button
        private readonly RelayCommand _goRightCommand;
        public ICommand GoRightCommand => _goRightCommand;

        private bool CanGoRight => _spritzWords != null && _activeIndex < _spritzWords.Count - 1 && SpritzState != SpritzState.Running;

        private void GoRight()
        {
            if (++_activeIndex < _spritzWords.Count - 1)
            {
                DisplayWord(_spritzWords[_activeIndex]);
                _goLeftCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
            }
            else
            {
                DisplayWord(_spritzWords[_activeIndex]);
                _goRightCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Open In Browser Button
        private readonly RelayCommand _openInBrowserCommand;
        public ICommand OpenInBroserCommand => _openInBrowserCommand;

        private bool CanOpenInBrowser => Article?.PublicUri != null;

        private void OpenInBrowser()
        {
            _variaService.OpenInBrowser(Article.PublicUri);
        }
        #endregion

        #region Increase Speed Button
        private readonly RelayCommand _increaseSpeedCommand;
        public ICommand IncreaseSpeedCommand => _increaseSpeedCommand;

        private bool CanIncreaseSpeed => true;

        private void IncreaseSpeed()
        {
            _spritzSpeed.IntValue += 50;
            RaisePropertyChanged(() => ReadingSpeed);
            _settingsRepository.SaveSetting(_spritzSpeed);
        }
        #endregion

        #region Decrease Speed Button
        private readonly RelayCommand _decreaseSpeedCommand;
        public ICommand DecreaseSpeedCommand => _decreaseSpeedCommand;

        private bool CanDecreaseSpeed => ReadingSpeed > 50;

        private void DecreaseSpeed()
        {
            _spritzSpeed.IntValue -= 50;
            RaisePropertyChanged(() => ReadingSpeed);
            _settingsRepository.SaveSetting(_spritzSpeed);

            _decreaseSpeedCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Start Button
        private readonly RelayCommand _startCommand;
        public ICommand StartCommand => _startCommand;

        private bool CanStart => _isSpritzReady;

        private SpritzState _spritzState;
        public SpritzState SpritzState
        {
            get { return _spritzState; }
            set { Set(ref _spritzState, value); }
        }

        private void Start()
        {
            if (SpritzState == SpritzState.Running)
            {
                SpritzState = SpritzState.Paused;
            }
            else if (SpritzState == SpritzState.Paused || SpritzState == SpritzState.Ready)
            {
                SpritzState = SpritzState.Running;
                DisplayNextWord();
            }
            else if (SpritzState == SpritzState.Finished)
            {
                SpritzState = SpritzState.Ready;
                _activeIndex = 0;
                if (_spritzWords.Count > 0)
                    DisplayWord(_spritzWords[0]);
            }

            _goLeftCommand.RaiseCanExecuteChanged();
            _goToStartCommand.RaiseCanExecuteChanged();
            _goRightCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #endregion

        #endregion

        #region Info View

        private FeedModel _similarCathegoriesArticles;
        public FeedModel SimilarCathegoriesArticles
        {
            get { return _similarCathegoriesArticles; }
            set { Set(ref _similarCathegoriesArticles, value); }
        }

        private FeedModel _similarTitlesArticles;
        public FeedModel SimilarTitlesArticles
        {
            get { return _similarTitlesArticles; }
            set { Set(ref _similarTitlesArticles, value); }
        }

        #endregion

    }
}
