using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DXVcs2Git.Core.Git {
    public static class ExceptionExtensions {
        static readonly Lazy<Regex> stackFrameRegex = new Lazy<Regex>(() => new Regex("^\\s+\\S+\\s+\\S+\\(.*?\\)", RegexOptions.Multiline | RegexOptions.Compiled));

        public static Exception InnermostException(this Exception exception) {
            Exception innermostException = null;
            exception.VisitPostOrder((Action<Exception>)(e => innermostException = innermostException ?? e));
            return innermostException;
        }

        //public static Exception UnwrapCompositionException(this Exception exception) {
        //    CompositionException compositionException1 = exception as CompositionException;
        //    if (compositionException1 == null)
        //        return exception;
        //    Exception exception1;
        //    for (CompositionException compositionException2 = compositionException1; compositionException2 != null; compositionException2 = exception1 as CompositionException) {
        //        CompositionError compositionError = ((IEnumerable<CompositionError>)compositionException2.Errors).FirstOrDefault();
        //        if (compositionError != null) {
        //            exception1 = compositionError.Exception;
        //            if (exception1 != null) {
        //                ComposablePartException composablePartException = exception1 as ComposablePartException;
        //                if (composablePartException != null && composablePartException.InnerException != null) {
        //                    CompositionException compositionException3 = composablePartException.InnerException as CompositionException;
        //                    if (compositionException3 == null)
        //                        return exception1.InnerException ?? exception;
        //                    exception1 = (Exception)compositionException3;
        //                }
        //            }
        //            else
        //                break;
        //        }
        //        else
        //            break;
        //    }
        //    return exception;
        //}

        public static void VisitPostOrder(this Exception exception, Action<Exception> visitor) {
            ReflectionTypeLoadException typeLoadException = exception as ReflectionTypeLoadException;
            if (typeLoadException != null)
                typeLoadException.LoaderExceptions.ForEach(e => VisitPostOrder(e, visitor));
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
                aggregateException.InnerExceptions.ForEach(e => VisitPostOrder(e, visitor));
            else if (exception.InnerException != null)
                VisitPostOrder(exception.InnerException, visitor);
            visitor(exception);
        }

        public static string FullStackTrace(this Exception exception) {
            StringBuilder builder = new StringBuilder();
            VisitPostOrder(exception, e => {
                if (builder.Length > 0)
                    builder.AppendLine("---------------");
                builder.AppendLine(e.VerboseMessage(true));
                builder.AppendLine(e.StackTrace);
            });
            return builder.ToString();
        }

        public static string VerboseMessage(this Exception exception, bool includeExceptionType = false) {
            string input = exception.ToString();
            if (!includeExceptionType) {
                string fullName = exception.GetType().FullName;
                if (input.StartsWith(fullName, StringComparison.Ordinal)) {
                    string str = input.Substring(fullName.Length);
                    if (str.StartsWith(": ", StringComparison.Ordinal))
                        str = str.Substring(2);
                    input = str.TrimStart();
                }
            }
            int length = input.IndexOf(" --->", StringComparison.Ordinal);
            if (length >= 0)
                return input.Substring(0, length);
            Match match = stackFrameRegex.Value.Match(input);
            if (match.Success)
                return input.Substring(0, match.Index);
            return input;
        }

        public static bool CanRetry(this Exception exception) {
            if (!exception.IsCriticalException())
                return !(exception is ObjectDisposedException);
            return false;
        }

        public static bool IsCriticalException(this Exception exception) {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (!exception.IsFatalException() && !(exception is AppDomainUnloadedException) && !(exception is BadImageFormatException) && !(exception is CannotUnloadAppDomainException) &&
                !(exception is InvalidProgramException) && !(exception is NullReferenceException))
                return exception is ArgumentException;
            return true;
        }

        public static bool IsFatalException(this Exception exception) {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (!(exception is StackOverflowException) && !(exception is OutOfMemoryException) && !(exception is ThreadAbortException))
                return exception is AccessViolationException;
            return true;
        }

        public static T AddData<T>(this T exception, string key, object value) where T : Exception {
            if (exception == null)
                throw new ArgumentNullException("exception");
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null || value.GetType().IsSerializable)
                exception.Data.Add(key, value);
            return exception;
        }

        public static T AddData<T>(this T exception, object values) where T : Exception {
            if (values != null) {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
                    exception.AddData(propertyDescriptor.Name, propertyDescriptor.GetValue(values));
            }
            return exception;
        }
    }
}