namespace USBLDC.ViewModel
{
    public class ViewModelLocator
    {
        public MainFrameViewModel _mainFrameViewModel;
        public HomePageViewModel _homwPageViewModel;
        public SonarConfigViewModel _SonarConfigViewModel;
        /// <summary>
        /// Gets the MainFrame ViewModel
        /// </summary>
        public MainFrameViewModel MainFrameViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_mainFrameViewModel == null)
                {
                    _mainFrameViewModel = new MainFrameViewModel();
                    _mainFrameViewModel.Initialize();
                }
                return _mainFrameViewModel;
            }
        }
        public HomePageViewModel HomePageViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_homwPageViewModel == null)
                {
                    _homwPageViewModel = new HomePageViewModel();
                    _homwPageViewModel.Initialize();
                }
                return _homwPageViewModel;
            }
        }
      
        public SonarConfigViewModel SonarConfigViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_SonarConfigViewModel == null)
                {
                    _SonarConfigViewModel = new SonarConfigViewModel();
                    _SonarConfigViewModel.Initialize();
                }
                return _SonarConfigViewModel;
            }
        }
    }
}
