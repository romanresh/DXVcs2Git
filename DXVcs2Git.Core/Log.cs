using System;
using System.Globalization;
using log4net;

namespace DXVcs2Git.Core {
    public static class Log {
        static Log() {
            log4net.Config.XmlConfigurator.Configure();
        }
        static readonly ILog log = LogManager.GetLogger(typeof(Log));
        public static void Message(string message, Exception ex = null) {
            log.Info(message, ex);
        }
        public static void Error(string message, Exception exception = null) {
            log.Error(message, exception);
        }
        public static void DoOrWarnException(Action action, string failureMessage, params object[] args) {
            try {
                action();
            }
            catch (Exception ex) {
                Message(string.Format(CultureInfo.InvariantCulture, failureMessage, args), ex);
            }
        }
    }
}
