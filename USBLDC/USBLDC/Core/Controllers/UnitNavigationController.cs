using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;
using USBLDC.Events;

namespace USBLDC.Core.Controllers
{
    /// <summary>
    /// 和页面导航相关消息处理函数，包括页面导航，导航传值，关闭页面，页面呈现
    /// BaseController已完成系统基础导航信息以及一些空间消息处理机制。
    /// </summary>
    internal class UnitNavigationController : BaseController,
        IHandleMessage<GoConnectViewEvent>,
        IHandleMessage<GoSonarConfigEvent>,
        IHandleMessage<GoHomePageNavigationEvent>
    {

        public void Handle(GoSonarConfigEvent message)
        {
            NavigateToPage("Views/SonarConfig.xaml");
        }
        public void Handle(GoConnectViewEvent message)
        {
            NavigateToPage("Views/ConnectView.xaml");
        }
        public void Handle(GoHomePageNavigationEvent message)
        {
            NavigateToPage("Views/HomePageView.xaml");
        }
    }
}