using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Игра__Быки_и_кооровы_
{
    public class Logging
    {
        public static readonly ILogger _logger;

        static Logging()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File("gamelogs/logger.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        /// <summary>
        /// Получает статически инициализированный экземпляр логгера
        /// </summary>
        /// <returns>Экземпляр ILogger для логирования</returns>
        public static ILogger GetLog()
        {
            return _logger;
        }
    }
}
