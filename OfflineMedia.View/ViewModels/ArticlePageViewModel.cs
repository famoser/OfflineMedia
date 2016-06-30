﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.Configuration.Base;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Business.Services;
using OfflineMedia.Data.Enums;
using OfflineMedia.View.Enums;
using ValueType = OfflineMedia.Business.Enums.Settings.ValueType;

namespace OfflineMedia.View.ViewModels
{
    public class ArticlePageViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IVariaService _variaService;

        public ArticlePageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository, IVariaService variaService)
        {
            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;
            _variaService = variaService;

            _setDisplayState = new RelayCommand<DisplayState>(SetDisplayState);

            _makeFontBiggerCommand = new RelayCommand(MakeFontBigger, () => CanMakeFontBigger);
            _makeFontSmallerCommand = new RelayCommand(MakeFontSmaller, () => CanMakeFontSmaller);
            _favoriteCommand = new RelayCommand(Favorite);
            _openInBrowserCommand = new RelayCommand(OpenInBrowser, () => CanOpenInBrowser);
            _reloadArticleCommand = new RelayCommand(ReloadArticle, () => CanReloadArticle);

            _goToStartCommand = new RelayCommand(GoToStart, () => CanGoToStart);
            _goLeftCommand = new RelayCommand(GoLeft, () => CanGoLeft);
            _goRightCommand = new RelayCommand(GoRight, () => CanGoRight);

            _startCommand = new RelayCommand(Start, () => CanStart);

            _increaseSpeedCommand = new RelayCommand(IncreaseSpeed, () => CanIncreaseSpeed);
            _decreaseSpeedCommand = new RelayCommand(DecreaseSpeed, () => CanDecreaseSpeed);

            if (IsInDesignMode)
            {
                _displayState = DisplayState.Spritz;
                _article = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0].ArticleList[0];
                _similarCathegoriesArticles = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];
                _similarTitlesArticles = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];

                _fontSize = new SettingModel()
                {
                    Description = "Font Size",
                    ValueType = ValueType.Int,
                    IntValue = 15
                };

                _spritzSpeed = new SettingModel()
                {
                    Description = "Spritz Speed",
                    ValueType = ValueType.Int,
                    IntValue = 350
                };

                //Article = _articleRepository.GetInfoArticle();
            }
            else
            {
                Initialize();
            }

            Messenger.Default.Register<ArticleModel>(this, Messages.Select, EvaluateMessage);
        }

        private void EvaluateMessage(ArticleModel obj)
        {
            SelectArticle(obj);
        }

        private async void SelectArticle(ArticleModel am)
        {
            SetDisplayState(DisplayState.Article);
            if (!am.IsStatic && am.IsInDatabase())
            {
                Article = am;
                if (Article.State == ArticleState.New)
                {
                    _articleRepository.ActualizeArticle(Article);
                }
                else
                {
                    if (!Article.IsLoadedCompletely())
                    {
                        await _articleRepository.LoadMoreArticleContent(Article, true);
                    }
                    if (Article.State == ArticleState.Loaded)
                    {
                        Article.State = ArticleState.Read;
                        _reloadArticleCommand.RaiseCanExecuteChanged();

                        await _articleRepository.UpdateArticleState(Article);
                    }
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
            }
            else
            {
                Article = am;
            }

            InitializeSpritz();
        }

        private async void Initialize()
        {
            _fontSize = await _settingsRepository.GetSettingByKey(SettingKey.FontSize);
            RaisePropertyChanged(() => FontSize);
            _makeFontBiggerCommand.RaiseCanExecuteChanged();
            _makeFontSmallerCommand.RaiseCanExecuteChanged();

            _spritzSpeed = await _settingsRepository.GetSettingByKey(SettingKey.WordsPerMinute);
            RaisePropertyChanged(() => ReadingSpeed);
            _increaseSpeedCommand.RaiseCanExecuteChanged();
            _decreaseSpeedCommand.RaiseCanExecuteChanged();
            
            RaisePropertyChanged(() => FontSize);
            RaisePropertyChanged(() => ReadingSpeed);
        }

        private ArticleModel _article;
        public ArticleModel Article
        {
            get { return _article; }
            set
            {
                var oldarticle = _article;
                if (Set(ref _article, value))
                {
                    if (_article != null)
                        _article.PropertyChanged += ArticleOnPropertyChanged;

                    if (oldarticle != null)
                        oldarticle.PropertyChanged -= ArticleOnPropertyChanged;

                    _reloadArticleCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void ArticleOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Content")
                //fix failed redering of Content
                RaisePropertyChanged(() => Article);
        }


        private DisplayState _displayState;
        public DisplayState DisplayState
        {
            get { return _displayState; }
            set
            {
                if (Set(ref _displayState, value))
                {
                    RaisePropertyChanged(() => DisplayStateIndex);
                }
            }
        }

        public int DisplayStateIndex
        {
            get { return (int)_displayState; }
            set
            {
                var newdisplaystatae = (DisplayState)value;
                if (Set(ref _displayState, newdisplaystatae))
                {
                    RaisePropertyChanged(() => DisplayState);
                }
            }
        }

        #region SetStateCommand
        private readonly RelayCommand<DisplayState> _setDisplayState;
        public ICommand SetDisplayStateCommand
        {
            get { return _setDisplayState; }
        }

        private void SetDisplayState(DisplayState state)
        {
            DisplayState = state;
        }

        public DisplayState DisplayStateArticle
        {
            get { return DisplayState.Article; }
        }

        public DisplayState DisplayStateSpritz
        {
            get { return DisplayState.Spritz; }
        }

        public DisplayState DisplayStateInfo
        {
            get { return DisplayState.Info; }
        }

        #endregion

        #region Article View

        private BaseSettingModel _fontSize;
        public int FontSize
        {
            get { return (_fontSize == null || _fontSize.IntValue == 0) ? 15 : _fontSize.IntValue; }
        }

        #region MakeFontBiggerCommand
        private readonly RelayCommand _makeFontBiggerCommand;
        public ICommand MakeFontBiggerCommand
        {
            get { return _makeFontBiggerCommand; }
        }

        private bool CanMakeFontBigger
        {
            get { return FontSize < 40; }
        }

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
        public ICommand MakeFontSmallerCommand
        {
            get { return _makeFontSmallerCommand; }
        }

        private bool CanMakeFontSmaller
        {
            get { return FontSize > 5; }
        }

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
        public ICommand FavoriteCommand
        {
            get { return _favoriteCommand; }
        }

        private void Favorite()
        {
            Article.IsFavorite = !Article.IsFavorite;
            _articleRepository.UpdateArticleFavorite(Article);
        }
        #endregion

        #endregion

        #region Spritz View

        private BaseSettingModel _spritzSpeed;

        public int ReadingSpeed
        {
            get { return _spritzSpeed != null ? _spritzSpeed.IntValue : 300; }
        }

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
        private int ActiveIndex
        {
            get { return _activeIndexSave; }
            set
            {
                if (Set(ref _activeIndexSave, value))
                {
                    RaisePropertyChanged(() => ActiveWord);
                }
            }
        }
        public int ActiveWord
        {
            get { return _spritzWords != null ? _activeIndexSave + 1 : 0; }
        }

        public int TotalWords
        {
            get { return _spritzWords != null ? _spritzWords.Count : 0; }
        }

        private List<SpritzWord> _spritzWords;
        private void InitializeSpritz()
        {
            try
            {
                _isSpritzReady = false;
                ActiveIndex = 0;
                _spritzState = SpritzState.Ready;
                _spritzWords = new List<SpritzWord>();

                DisplayWord(new SpritzWord()
                {
                    After = "ding",
                    Before = "Lo",
                    Middle = 'a',
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
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, "Spritz failed",this, ex);
            }
        }

        private async void DisplayNextWord()
        {
            if (_spritzWords == null)
                _spritzWords = SpritzHelper.GenerateList(Article.Content);

            int delay = (60 * 1000) / ReadingSpeed;

            if (SpritzState == SpritzState.Running && ActiveIndex < _spritzWords.Count)
            {
                if (++ActiveIndex < _spritzWords.Count)
                {
                    DisplayWord(_spritzWords[ActiveIndex]);
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
        public ICommand GoToStartCommand
        {
            get { return _goToStartCommand; }
        }

        private bool CanGoToStart
        {
            get { return ActiveIndex > 0 && SpritzState != SpritzState.Running; }
        }

        private void GoToStart()
        {
            ActiveIndex = 0;
            DisplayWord(_spritzWords[ActiveIndex]);

            _goRightCommand.RaiseCanExecuteChanged();
            _goLeftCommand.RaiseCanExecuteChanged();
            _goToStartCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Go Left Button
        private readonly RelayCommand _goLeftCommand;
        public ICommand GoLeftCommand
        {
            get { return _goLeftCommand; }
        }

        private bool CanGoLeft
        {
            get { return ActiveIndex > 0 && SpritzState != SpritzState.Running; }
        }

        private void GoLeft()
        {
            if (--ActiveIndex > 0)
            {
                DisplayWord(_spritzWords[ActiveIndex]);
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
        public ICommand GoRightCommand
        {
            get { return _goRightCommand; }
        }

        private bool CanGoRight
        {
            get
            {
                return _spritzWords != null && ActiveIndex < _spritzWords.Count - 1 &&
                       SpritzState != SpritzState.Running;
            }
        }

        private void GoRight()
        {
            if (++ActiveIndex < _spritzWords.Count - 1)
            {
                DisplayWord(_spritzWords[ActiveIndex]);
                _goLeftCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
            }
            else
            {
                DisplayWord(_spritzWords[ActiveIndex]);
                _goRightCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Open In Browser Button
        private readonly RelayCommand _reloadArticleCommand;
        public ICommand ReloadArticleCommand
        {
            get { return _reloadArticleCommand; }
        }

        private bool CanReloadArticle
        {
            get { return Article.State == ArticleState.Loaded || Article.State == ArticleState.Read; }
        }

        private void ReloadArticle()
        {
            Article.State = ArticleState.New;
            _reloadArticleCommand.RaiseCanExecuteChanged();
            _articleRepository.UpdateArticleState(Article);
        }
        #endregion

        #region Open In Browser Button
        private readonly RelayCommand _openInBrowserCommand;
        public ICommand OpenInBrowserCommand
        {
            get { return _openInBrowserCommand; }
        }

        private bool CanOpenInBrowser
        {
            get { return Article != null && Article.PublicUri != null; }
        }

        private void OpenInBrowser()
        {
            _variaService.OpenInBrowser(Article.PublicUri);
        }
        #endregion

        #region Increase Speed Button
        private readonly RelayCommand _increaseSpeedCommand;
        public ICommand IncreaseSpeedCommand
        {
            get { return _increaseSpeedCommand; }
        }

        private bool CanIncreaseSpeed
        {
            get { return ReadingSpeed < 2000; }
        }

        private void IncreaseSpeed()
        {
            _spritzSpeed.IntValue += 50;
            RaisePropertyChanged(() => ReadingSpeed);
            _settingsRepository.SaveSetting(_spritzSpeed);

            _decreaseSpeedCommand.RaiseCanExecuteChanged();
            _increaseSpeedCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Decrease Speed Button
        private readonly RelayCommand _decreaseSpeedCommand;
        public ICommand DecreaseSpeedCommand
        {
            get { return _decreaseSpeedCommand; }
        }

        private bool CanDecreaseSpeed
        {
            get { return ReadingSpeed > 50; }
        }

        private void DecreaseSpeed()
        {
            _spritzSpeed.IntValue -= 50;
            RaisePropertyChanged(() => ReadingSpeed);
            _settingsRepository.SaveSetting(_spritzSpeed);

            _decreaseSpeedCommand.RaiseCanExecuteChanged();
            _increaseSpeedCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Start Button
        private readonly RelayCommand _startCommand;
        public ICommand StartCommand
        {
            get { return _startCommand; }
        }

        private bool CanStart
        {
            get { return _isSpritzReady; }
        }

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
                ActiveIndex = 0;
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
