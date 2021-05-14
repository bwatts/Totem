using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Totem
{
    public static class ErrorInfoExtensions
    {
        public static void LogErrorInfo(this ILogger logger, ErrorInfo error)
        {
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            if(error == null)
                throw new ArgumentNullException(nameof(error));

            logger.LogError("{Code} - {Name} - {Level}", error.Code, error.Name, error.Level);
        }

        public static void LogErrorInfo(this ILogger logger, IEnumerable<ErrorInfo> errors)
        {
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            if(errors == null)
                throw new ArgumentNullException(nameof(errors));

            var list = errors.ToList();

            if(list.Count == 0)
            {
                return;
            }
            else if(list.Count == 1)
            {
                logger.LogErrorInfo(list[0]);
            }
            else
            {
                logger.LogError("{Errors}", errors.Select(error => $"{error.Code} - {error.Name} - {error.Level}"));
            }
        }

        public static void LogErrorInfo(this ILogger logger, params ErrorInfo[] errors) =>
            logger.LogErrorInfo(errors.AsEnumerable());
    }
}