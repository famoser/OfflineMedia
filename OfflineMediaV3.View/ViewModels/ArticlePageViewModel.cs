using System.Collections.Generic;
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
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.View.Enums;

namespace OfflineMediaV3.View.ViewModels
{
    public class ArticlePageViewModel : ViewModelBase
    {
        private ISettingsRepository _settingsRepository;
        private IArticleRepository _articleRepository;

        public ArticlePageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository)
        {
            Messenger.Default.Register<ArticleModel>(this, Messages.Select, EvaluateMessage);

            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;

            if (IsInDesignMode)
            {
                _article = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0].ArticleList[0];

                IsSpritzActive = false;
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

            _makeFontBiggerCommand = new RelayCommand(MakeFontBigger, () => CanMakeFontBigger);
            _makeFontSmallerCommand = new RelayCommand(MakeFontSmaller, () => CanMakeFontSmaller);
            _favoriteCommand = new RelayCommand(Favorite, () => CanFavorite);
            _spritzCommand = new RelayCommand(Spritz, () => CanSpritz);

            _goToStartCommand = new RelayCommand(GoToStart, () => CanGoToStart);
            _goLeftCommand = new RelayCommand(GoLeft, () => CanGoLeft);
            _goRightCommand = new RelayCommand(GoRight, () => CanGoRight);

            _startCommand = new RelayCommand(Start, () => CanStart);

            _increaseSpeedCommand = new RelayCommand(IncreaseSpeed, () => CanIncreaseSpeed);
            _decreaseSpeedCommand = new RelayCommand(DecreaseSpeed, () => CanDecreaseSpeed);
        }

        private async void EvaluateMessage(ArticleModel obj)
        {
            Article = await _articleRepository.GetCompleteArticle(obj.Id);
            if (Article.State == ArticleState.New)
            {
                Article = await _articleRepository.ActualizeArticle(Article);
                await _articleRepository.UpdateArticle(Article);
            }
            else if (Article.State == ArticleState.Loaded)
            {
                Article.State = ArticleState.Read;
                await _articleRepository.UpdateArticle(Article);
            }
            InitializeSpritz();
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
            set { Set(ref _article, value); }
        }

        private SettingModel _fontSize;
        public int FontSize
        {
            get
            {
                return (_fontSize == null || _fontSize.IntValue == 0) ? 15 : _fontSize.IntValue;
            }
        }

        #region MakeFontBiggerCommand
        private RelayCommand _makeFontBiggerCommand;
        public ICommand MakeFontBiggerCommand
        {
            get { return _makeFontBiggerCommand; }
        }

        private bool CanMakeFontBigger
        {
            get { return true; }
        }

        private void MakeFontBigger()
        {
            _fontSize.IntValue += 2;
            RaisePropertyChanged(() => FontSize);
        }
        #endregion

        #region MakeFontSmallerCommand
        private RelayCommand _makeFontSmallerCommand;
        public ICommand MakeFontSmallerCommand
        {
            get { return _makeFontSmallerCommand; }
        }

        private bool CanMakeFontSmaller
        {
            get { return true; }
        }

        private void MakeFontSmaller()
        {
            _fontSize.IntValue -= 2;
            RaisePropertyChanged(() => FontSize);
        }
        #endregion

        #region FavoriteCommand
        private RelayCommand _favoriteCommand;
        public ICommand FavoriteCommand
        {
            get { return _favoriteCommand; }
        }

        private bool CanFavorite
        {
            get { return true; }
        }

        private async void Favorite()
        {
            Article.IsFavorite = !Article.IsFavorite;
            await _articleRepository.UpdateArticle(Article);
        }
        #endregion

        #region SpritzCommand
        private RelayCommand _spritzCommand;
        public ICommand SpritzCommand
        {
            get { return _spritzCommand; }
        }

        private bool CanSpritz
        {
            get { return _isSpritzReady; }
        }

        private bool _isSpritzActive;
        public bool IsSpritzActive
        {
            get { return _isSpritzActive; }
            set { Set(ref _isSpritzActive, value); }
        }

        private void Spritz()
        {
            IsSpritzActive = !IsSpritzActive;
        }
        #endregion


        #region Spritz View

        private SettingModel _spritzSpeed;

        public int ReadingSpeed
        {
            get { return _spritzSpeed.IntValue; }
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
        private int _activeIndex;
        private List<SpritzWord> _spritzWords;
        private void InitializeSpritz()
        {
            _isSpritzReady = false;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _spritzCommand.RaiseCanExecuteChanged();
                
            });

            //inititialize Wordlists
            if (Article.Content != null && Article.Content.Any())
            {
                _spritzWords = SpritzHelper.GenerateList(Article.Content);

                //prepare User Interface
                if (_spritzWords != null && _spritzWords.Count > 0)
                {
                    _goLeftCommand.RaiseCanExecuteChanged();
                    _goRightCommand.RaiseCanExecuteChanged();

                    DisplayWord(_spritzWords[0]);

                    _isSpritzReady = true;
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _spritzCommand.RaiseCanExecuteChanged();

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
        public ICommand GoToStartCommand
        {
            get
            {
                return _goToStartCommand;
            }
        }

        private bool CanGoToStart
        {
            get { return _activeIndex > 0 && SpritzState != SpritzState.Running; }
        }

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
        public ICommand GoLeftCommand
        {
            get
            {
                return _goLeftCommand;
            }
        }

        private bool CanGoLeft
        {
            get { return _activeIndex > 0 && SpritzState != SpritzState.Running; }
        }

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
        public ICommand GoRightCommand
        {
            get
            {
                return _goRightCommand;
            }
        }

        private bool CanGoRight
        {
            get { return _activeIndex < _spritzWords.Count - 1 && SpritzState != SpritzState.Running; }
        }

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

        #region Increase Speed Button
        private readonly RelayCommand _increaseSpeedCommand;
        public ICommand IncreaseSpeedCommand
        {
            get
            {
                return _increaseSpeedCommand;
            }
        }

        private bool CanIncreaseSpeed
        {
            get { return true; }
        }

        private void IncreaseSpeed()
        {
            _spritzSpeed.IntValue += 50;
            RaisePropertyChanged(() => ReadingSpeed);
        }
        #endregion

        #region Decrease Speed Button
        private readonly RelayCommand _decreaseSpeedCommand;
        public ICommand DecreaseSpeedCommand
        {
            get
            {
                return _decreaseSpeedCommand;
            }
        }

        private bool CanDecreaseSpeed
        {
            get { return ReadingSpeed > 50; }
        }

        private void DecreaseSpeed()
        {
            _spritzSpeed.IntValue -= 50;
            RaisePropertyChanged(() => ReadingSpeed);

            _decreaseSpeedCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Start Button
        private readonly RelayCommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                return _startCommand;
            }
        }

        private bool CanStart
        {
            get { return true; }
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

    }
}
