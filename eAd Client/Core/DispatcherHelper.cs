namespace ClientApp.Core
{
    using System;
    using System.Security.Permissions;
    using System.Windows.Threading;

    public static class DispatcherHelper
    {
        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame arg = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DispatcherHelper.ExitFrames), arg);
            try
            {
                Dispatcher.PushFrame(arg);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame) frame).Continue = false;
            return null;
        }
    }
}

