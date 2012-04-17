namespace DesktopClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;

    public class SingleInstance : IDisposable
    {
        public EventHandler<ArgumentsReceivedEventArgs> ArgumentsReceived;
        private bool disposed;
        private Guid identifier = Guid.Empty;
        private Mutex mutex;
        private bool ownsMutex;

     
        public SingleInstance(Guid identifier)
        {
            this.identifier = identifier;
            this.mutex = new Mutex(true, identifier.ToString(), out this.ownsMutex);
        }

        private void CallOnArgumentsReceived(object state)
        {
            this.OnArgumentsReceived((string[]) state);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if ((this.mutex != null) && this.ownsMutex)
                {
                    try
                    {
                        this.mutex.ReleaseMutex();
                        this.mutex = null;
                    }
                    catch (Exception)
                    {
                    }
                }
                this.disposed = true;
            }
        }

        ~SingleInstance()
        {
            this.Dispose(false);
        }

        private void ListenForArguments(object state)
        {
            try
            {
                using (NamedPipeServerStream stream = new NamedPipeServerStream(this.identifier.ToString()))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        stream.WaitForConnection();
                        List<string> list = new List<string>();
                        while (stream.IsConnected)
                        {
                            list.Add(reader.ReadLine());
                        }
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this.CallOnArgumentsReceived), list.ToArray());
                    }
                }
            }
            catch (IOException)
            {
            }
            finally
            {
                this.ListenForArguments(null);
            }
        }

        public void ListenForArgumentsFromSuccessiveInstances()
        {
            if (!this.IsFirstInstance)
            {
                throw new InvalidOperationException("This is not the first instance.");
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenForArguments));
        }

        private void OnArgumentsReceived(string[] arguments)
        {
            if (this.ArgumentsReceived != null)
            {
                ArgumentsReceivedEventArgs e = new ArgumentsReceivedEventArgs {
                    Args = arguments
                };
                this.ArgumentsReceived(this, e);
            }
        }

        public bool PassArgumentsToFirstInstance(string[] arguments)
        {
            if (this.IsFirstInstance)
            {
                throw new InvalidOperationException("This is the first instance.");
            }
            try
            {
                using (NamedPipeClientStream stream = new NamedPipeClientStream(this.identifier.ToString()))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        stream.Connect(50);
                        foreach (string str in arguments)
                        {
                            writer.WriteLine(str);
                        }
                    }
                }
                return true;
            }
            catch (TimeoutException)
            {
            }
            catch (IOException)
            {
            }
            return false;
        }

        public bool IsFirstInstance
        {
            get
            {
                return this.ownsMutex;
            }
        }
    }
}

