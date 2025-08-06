using System;
using System.IO;

namespace Lesson
{
    public class Program
    {
        static void Main(string[] args)
        {
            string filePath = "log.txt";
            
            ILogger fileLogWriter = new FileLogWriter(filePath);
            ILogger consoleLogWriter = new ConsoleLogWriter();
            ILogger secureFileLogWritter = new SecureLogWritter(fileLogWriter, DayOfWeek.Friday);
            ILogger secureConsoleLogWriter = new SecureLogWritter(consoleLogWriter, DayOfWeek.Friday);
            ILogger consoleAndSecureFileLogWriter = new MultipleLogWriter(consoleLogWriter, secureFileLogWritter);

            List<Pathfinder> pathfinders = new List<Pathfinder>
            {
                new Pathfinder(fileLogWriter),
                new Pathfinder(consoleLogWriter),
                new Pathfinder(secureFileLogWritter),
                new Pathfinder(secureConsoleLogWriter),
                new Pathfinder(consoleAndSecureFileLogWriter)
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
        private ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Find(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            _logger.WriteError(message);
        }
    }

    public class MultipleLogWriter : ILogger
    {
        private IEnumerable<ILogger> _loggers;

        public MultipleLogWriter(params ILogger[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
            
            foreach (ILogger logger in loggers)
            {
                if (logger == null)
                    throw new ArgumentNullException(nameof(logger));
            }
        }

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            foreach (ILogger logger in _loggers)
            {
                logger.WriteError(message);
            }
        }
    }

    public class FileLogWriter : ILogger
    {
        private readonly string  _path;
        
        public FileLogWriter(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));

            _path = filePath;
        }
        
        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            File.WriteAllText(_path, message);
        }
    }

    public class ConsoleLogWriter : ILogger
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
        
        private readonly DayOfWeek _dayOfLogRecording;

        public SecureLogWritter(ILogger logger, DayOfWeek dayOfLogRecording)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _dayOfLogRecording = dayOfLogRecording;
        }

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(nameof(message));

            if (DateTime.Now.DayOfWeek == _dayOfLogRecording)
            {
                _logger.WriteError(message);
            }
        }
    }
}