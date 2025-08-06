using System;
using System.IO;

namespace Lesson
{
    public class Program
    {
        static void Main(string[] args)
        {
            ILogger fileLogWritter = new FileLogWritter();
            ILogger consoleLogWritter = new ConsoleLogWritter();
            ILogger secureFileLogWritter = new SecureLogWritter(fileLogWritter);
            ILogger secureConsoleLogWritter = new SecureLogWritter(consoleLogWritter);

            List<Pathfinder> pathfinders = new List<Pathfinder>
            {
                new Pathfinder(fileLogWritter),
                new Pathfinder(consoleLogWritter),
                new Pathfinder(secureFileLogWritter),
                new Pathfinder(secureConsoleLogWritter),
                new Pathfinder(consoleLogWritter, secureFileLogWritter)
            };

            string message = "message";

            foreach (Pathfinder pathfinder in pathfinders)
                pathfinder.Find(message);
        }
    }

    public interface ILogger
    {
        public void WriteError(string message);
    }

    public class Pathfinder
    {
        private IEnumerable<ILogger> _loggers;

        public Pathfinder(params ILogger[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        public void Find(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            foreach (ILogger logger in _loggers)
            {
                logger.WriteError(message);
            }
        }
    }

    public class FileLogWritter : ILogger
    {
        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            File.WriteAllText("log.txt", message);
        }
    }

    public class ConsoleLogWritter : ILogger
    {
        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            Console.WriteLine(message);
        }
    }

    public class SecureLogWritter : ILogger
    {
        private ILogger _logger;

        public SecureLogWritter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                _logger.WriteError(message);
            }
        }
    }
}