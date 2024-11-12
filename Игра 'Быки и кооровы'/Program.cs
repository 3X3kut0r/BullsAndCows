using System.Diagnostics;

namespace Игра__Быки_и_кооровы_
{
    internal class Program
    {
        public static string UsersFilePath = "users.txt"; // Файл с информацией о пользователях
        public static string ResultsFilePath = "results.txt"; // Файл с информацией о играх пользователей
        static void Main(string[] args)
        {
            do
            {
                string _secretNumber = GenerateSecretNumber(4);
                string _guess;
                int _attempts = 0;
                Stopwatch _time = new Stopwatch();
                Dictionary<string, (int Wins, int Attempts)> _leaderboard = LoadLeaderboard();
                Dictionary<string, string> _infoList = LoadUserInfo();
                Console.WriteLine("Добро пожаловать в игру 'Быки и коровы'");
                string _playerName = Authorization(_infoList);
                Console.Clear();
                Console.WriteLine($"{_secretNumber}");
                _time.Start();
                do
                {
                    int _cows = 0;
                    int _bulls = 0;
                    _attempts++;
                    Console.Write($"Введите ваше предположение {_playerName}: ");
                    _guess = Console.ReadLine();
                    if (_guess.Length != _secretNumber.Length || _guess.Distinct().Count() != 4 || !_guess.All(char.IsDigit))
                    {
                        Console.WriteLine("Введите корректное 4-значное число без повторяющихся цифр.");
                        continue;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (_secretNumber[i] == _guess[i])
                        {
                            _bulls++;
                        }
                        else if (_secretNumber.Contains(_guess[i]))
                        {
                            _cows++;
                        }
                    }
                    Console.WriteLine($"Быки: {_bulls}, Коровы: {_cows}");
                } while (_guess != _secretNumber);
                _time.Stop();
                Console.WriteLine($"\nВы угадали число {_secretNumber} за {_attempts} попыток.");
                Console.WriteLine($"Потрачено времени: {_time.ElapsedMilliseconds / 1000} секунд.");
                UpdateLeaderboard(_playerName, _attempts, _leaderboard);
                DisplayLeaderboard(_leaderboard);
                Console.WriteLine("\nХотите сыграть еще? (да/нет)");
                Console.Write("Ваш ответ: ");
                string _answer = Console.ReadLine();
                if (_answer.ToLower() == "нет")
                {
                    return;
                }
                else
                {
                    Console.Clear();
                }
            } while (true);
        }

        static string GenerateSecretNumber(int length) // Генерация числа
        {
            Random _random = new Random();
            string _result = "";
            while (_result.Length < length)
            {
                string _nextDigit = _random.Next(0, 10).ToString();
                if (!_result.Contains(_nextDigit))
                {
                    _result += _nextDigit;
                }
            }
            return _result;
        }

        static Dictionary<string, (int Wins, int Attempts)> LoadLeaderboard() // Загрузка таблицы рекордов
        {
            Dictionary<string, (int Wins, int Attempts)> _leaderboard = new Dictionary<string, (int Wins, int Attempts)>();
            if (File.Exists(ResultsFilePath))
            {
                using (StreamReader _reader = new StreamReader(ResultsFilePath))
                {
                    string _line;
                    while ((_line = _reader.ReadLine()) != null)
                    {
                        var _linesarray = _line.Split('-');
                        if (_linesarray.Length == 3)
                        {
                            string _playerName = _linesarray[0];
                            int _wins = int.Parse(_linesarray[1]);
                            int _attempts = int.Parse(_linesarray[2]);
                            _leaderboard[_playerName] = (_wins, _attempts);
                        }
                    }
                }
            }
            return _leaderboard;
        }

        static void UpdateLeaderboard(string playerName, int attempts, Dictionary<string, (int Wins, int Attempts)> leaderboard) // Обновление таблицы рекордов
        {
            if (leaderboard.ContainsKey(playerName))
            {
                var _playerStats = leaderboard[playerName];
                _playerStats.Wins++;
                _playerStats.Attempts += attempts;
                leaderboard[playerName] = _playerStats;
            }
            else
            {
                leaderboard[playerName] = (1, attempts);
            }
            using (StreamWriter _writer = new StreamWriter(ResultsFilePath))
            {
                foreach (var _player in leaderboard)
                {
                    _writer.WriteLine($"{_player.Key}-{_player.Value.Wins}-{_player.Value.Attempts}");
                }
            }
        }

        static void DisplayLeaderboard(Dictionary<string, (int Wins, int Attempts)> leaderboard) // Отображение таблицы рекордов в консоли
        {
            Console.WriteLine("\nТаблица рекордов:");
            Console.WriteLine("Имя:\t\tПобеды:\t\tПопытки:\tЭффективность:");
            var _sortedLeaderboard = leaderboard.OrderByDescending(p => (double)p.Value.Wins / p.Value.Attempts).ThenBy(p => p.Value.Attempts);
            foreach (var _player in _sortedLeaderboard)
            {
                double _winRate = 100 * _player.Value.Wins / _player.Value.Attempts;
                Console.WriteLine($"{_player.Key}\t\t{_player.Value.Wins}\t\t{_player.Value.Attempts}\t\t({_winRate}%)");
            }
        }

        static string Authorization(Dictionary<string, string> infoList) // Авторизация игрока
        {
            while (true)
            {
                Console.Write("Введите ваше имя: ");
                string _inputName = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string _inputPassword = Console.ReadLine();
                if (infoList.ContainsKey(_inputName) && infoList[_inputName] == _inputPassword)
                {
                    if (_inputName == "Admin")
                    {
                        Console.Clear();
                        string _answer;
                        Console.WriteLine("Добро пожаловать в систему админ!");
                        do
                        {
                            AdminOperations();
                            Console.WriteLine("Для подтверждения выхода напишите - да.");
                            _answer = Console.ReadLine();
                            Console.Clear();
                            infoList = LoadUserInfo();
                        } while (_answer.ToLower() != "да");
                        continue;
                    }
                    else
                    {
                        return _inputName;
                    }
                }
                else
                {
                    Console.WriteLine("Неверное имя пользователя или пароль.\n");
                }
            }
        }

        static void AdminOperations() // Операции админа
        {
            while (true)
            {
                Console.WriteLine("1) Добавить пользователя");
                Console.WriteLine("2) Удалить пользователя");
                Console.WriteLine("3) Выход\n");
                Console.Write("Выберите действие: ");
                string _choice = Console.ReadLine();
                switch (_choice)
                {
                    case "1": // Добавление игрока
                        var _lines = new List<string>(File.ReadAllLines(UsersFilePath));
                        Console.WriteLine("Список пользователей:\n");
                        foreach (var _line in _lines)
                        {
                            Console.WriteLine(_line);
                        }
                        Console.WriteLine("\nДобавьте нового пользователя:");
                        Console.Write("Имя: ");
                        string _userName = Console.ReadLine();
                        if (_userName.ToLower() == "admin")
                        {
                            Console.WriteLine("Нельзя добавить 2-го админа.\n");
                            break;
                        }
                        if (_lines.Exists(line => line.StartsWith(_userName + "-")))
                        {
                            Console.WriteLine("Такой пользователь уже есть\n");
                            break;
                        }
                        Console.Write("Пароль: ");
                        string _userPassword = Console.ReadLine();
                        using (StreamWriter _writer = new StreamWriter(UsersFilePath, true))
                        {
                            _writer.WriteLine($"{_userName}-{_userPassword}");
                        }
                        Console.WriteLine("Пользователь добавлен.\n");
                        break;
                    case "2": // Удаление игрока
                        _lines = new List<string>(File.ReadAllLines(UsersFilePath));
                        Console.WriteLine("Список пользователей:\n");
                        foreach (var _line in _lines)
                        {
                            Console.WriteLine(_line);
                        }
                        Console.WriteLine("\nУдалите нужного пользователя:");
                        Console.Write("Имя: ");
                        _userName = Console.ReadLine();
                        if (_userName.ToLower() == "admin")
                        {
                            Console.WriteLine("Невозможно удалить Admin.\n");
                            break;
                        }
                        if (!_lines.Exists(line => line.StartsWith(_userName + "-")))
                        {
                            Console.WriteLine("Такого пользователя нет.\n");
                            break;
                        }
                        _lines.RemoveAll(line => line.StartsWith(_userName + "-"));
                        File.WriteAllLines(UsersFilePath, _lines);
                        _lines = new List<string>(File.ReadAllLines(ResultsFilePath));
                        _lines.RemoveAll(line => line.StartsWith(_userName + "-"));
                        File.WriteAllLines(ResultsFilePath, _lines);
                        Console.WriteLine("Пользователь удален.\n");
                        break;
                    default: // Ввод неправильной операции
                        Console.WriteLine("Неверная операция, пожалуйста, выберите снова.\n");
                        break;
                    case "3": // Выход из меню админа
                        return;
                }
            }
        }

        static Dictionary<string, string> LoadUserInfo() // Загрузка информации о пользователях
        {
            Dictionary<string, string> _infoList = new Dictionary<string, string>();
            if (File.Exists(UsersFilePath))
            {
                using (StreamReader _reader = new StreamReader(UsersFilePath))
                {
                    string _line;
                    while ((_line = _reader.ReadLine()) != null)
                    {
                        var _linesarray = _line.Split('-');
                        if (_linesarray.Length == 2)
                        {
                            string _playerName = _linesarray[0];
                            string _password = _linesarray[1];
                            _infoList[_playerName] = _password;
                        }
                    }
                }
            }
            return _infoList;
        }
    }
}