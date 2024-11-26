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
        public static ILogger Logger;

        static Logging()
        {
            Logger = new LoggerConfiguration() // Создаю поведение логгера
                .MinimumLevel.Information() // С какого уровня будут логироваться сообщения
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // До какого уровня будет логировать
                .Enrich.FromLogContext() // Добавляет комментарии к логу, т.е. что произошло
                .WriteTo.File("gamelogs/logger.txt", rollingInterval: RollingInterval.Day) // В какой файлик производится запись c интервалом времени 1 день
                .CreateLogger();
        }

        /// <summary>
        /// Получает статически инициализированный экземпляр логгера
        /// </summary>
        /// <returns>Экземпляр ILogger для логирования</returns>
        public static ILogger GetLog()
        {
            return Logger;
        }
    }
}
