using MahApps.Metro.Controls.Dialogs;
using System;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;
using USBLDC.Events;
using USBLDC.Helpers;
using USBLDC.ViewModel;
namespace USBLDC.Core.Controllers
{
    /// <summary>
    /// 模块间消息处理类，包括WriteLog，系统消息广播，报警等
    /// 由于不像导航消息处理类那样已经由BaseController处理了一些基本消息
    /// 因此需要自己将消息处理函数完成并完成IMessageController接口
    /// </summary>
    class UnitMessageController : IMessageController,IHandleMessage<LogEvent>
    {
        #region 构造
        private IEventAggregator eventAggregator;
        public UnitMessageController()
        {
            eventAggregator = Kernel.Instance.EventAggregator;
            //将类实例注册到EventAggregator
            eventAggregator.Subscribe(this);
        }

        

        ~UnitMessageController()
        {
            eventAggregator.Unsubscribe(this);
        }
        #endregion

        #region IMessage接口实现
        //初始化消息处理类
         public void Init()
        {
            
        }
        
        public void SendMessage(string message)
        {
            
        }

        public void WriteLog(string message)
        {
            LogHelper.WriteLog(message);
        }

        public void ErrorLog(string message, Exception ex)
        {
            LogHelper.ErrorLog(message, ex);
        }

        public void Alert(string message)
        {
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "好的";
            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "错误",
                message, MessageDialogStyle.Affirmative, md);
        }

        public void BroadCast(string message)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IHandle
        //负责写入记录文件
        public void Handle(LogEvent message)
        {
            switch (message.Type)
            {
                case LogType.Error:
                    ErrorLog(message.Message, message.Ex);
                    Alert(message.Message);
                    break;
                case LogType.Warning:
                    Alert(message.Message);
                    break;
                default:
                    WriteLog(message.Message);
                    break;
            }
        }
        #endregion
    }
}
