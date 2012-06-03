namespace ClientApp.Core
{
    using System;
    using System.Windows;

    public class MessageListener : DependencyObject
    {
        private static MessageListener _mInstance;
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageListener), new UIPropertyMetadata(null));

        private MessageListener()
        {
        }

        public void ReceiveMessage(string message)
        {
            this.Message = message;
            DispatcherHelper.DoEvents();
        }

        public static MessageListener Instance
        {
            get
            {
                if (_mInstance == null)
                {
                    _mInstance = new MessageListener();
                }
                return _mInstance;
            }
        }

        public string Message
        {
            get
            {
                return (string) base.GetValue(MessageProperty);
            }
            set
            {
                base.SetValue(MessageProperty, value);
            }
        }
    }
}

