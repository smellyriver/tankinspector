using log4net;
using System;

namespace Smellyriver.TankInspector
{
	internal class SafeLog : ILog
    {

        public static ILog GetLogger(string name)
        {
            return new SafeLog(name);
        }


        public string Name { get; private set; }
        private readonly ILog _log;

        public SafeLog(string name)
        {
            _log = LogManager.GetLogger(name);
        }


        public void Debug(object message, Exception exception)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Debug(message, exception);
            }
        }

        public void Debug(object message)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Debug(message);
            }
        }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.DebugFormat(provider, format, args);
            }
        }
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.DebugFormat(format, arg0, arg1, arg2);
            }
        }
        public void DebugFormat(string format, object arg0, object arg1)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.DebugFormat(format, arg0, arg1);
            }
        }
        public void DebugFormat(string format, object arg0)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.DebugFormat(format, arg0);
            }
        }
        public void DebugFormat(string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.DebugFormat(format, args);
            }
        }
        public void Error(object message, Exception exception)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Error(message, exception);
            }
        }
        public void Error(object message)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Error(message);
            }
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.ErrorFormat(provider, format, args);
            }
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.ErrorFormat(format, arg0, arg1, arg2);
            }
        }
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.ErrorFormat(format, arg0, arg1);
            }
        }
        public void ErrorFormat(string format, object arg0)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.ErrorFormat(format, arg0);
            }
        }
        public void ErrorFormat(string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.ErrorFormat(format, args);
            }
        }
        public void Fatal(object message, Exception exception)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Fatal(message, exception);
            }
        }
        public void Fatal(object message)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Fatal(message);
            }
        }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.FatalFormat(provider, format, args);
            }
        }
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.FatalFormat(format, arg0, arg1, arg2);
            }
        }
        public void FatalFormat(string format, object arg0, object arg1)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.FatalFormat(format, arg0, arg1);
            }
        }
        public void FatalFormat(string format, object arg0)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.FatalFormat(format, arg0);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.FatalFormat(format, args);
            }
        }

        public void Info(object message, Exception exception)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Info(message, exception);
            }
        }

        public void Info(object message)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Info(message);
            }
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.InfoFormat(provider, format, args);
            }
        }
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.InfoFormat(format, arg0, arg1, arg2);
            }
        }
        public void InfoFormat(string format, object arg0, object arg1)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.InfoFormat(format, arg0, arg1);
            }
        }
        public void InfoFormat(string format, object arg0)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.InfoFormat(format, arg0);
            }
        }
        public void InfoFormat(string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.InfoFormat(format, args);
            }
        }
        public bool IsDebugEnabled => _log.IsDebugEnabled;

	    public bool IsErrorEnabled => _log.IsErrorEnabled;

	    public bool IsFatalEnabled => _log.IsFatalEnabled;

	    public bool IsInfoEnabled => _log.IsInfoEnabled;

	    public bool IsWarnEnabled => _log.IsWarnEnabled;

	    public void Warn(object message, Exception exception)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Warn(message, exception);
            }
        }
        public void Warn(object message)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.Warn(message);
            }
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.WarnFormat(provider, format, args);
            }
        }
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.WarnFormat(format, arg0, arg1, arg2);
            }
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.WarnFormat(format, arg0, arg1);
            }
        }

        public void WarnFormat(string format, object arg0)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.WarnFormat(format, arg0);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                _log.WarnFormat(format, args);
            }
        }

        public log4net.Core.ILogger Logger => _log.Logger;
    }
}
