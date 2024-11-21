using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Игра__Быки_и_кооровы_
{
    public class AuthorizationAndAdministration
    {
        public static string UsersFilePath = "users.txt"; // Файл с информацией о пользователях
        public static string ResultsFilePath = "results.txt"; // Файл с информацией о играх пользователей

        /// <summary>
        /// Авторизация игрока в системе, с возможностью регистрации
        /// </summary>
        /// <param name="infoList">Словарь содержащий имена пользователей и их пароли</param>
        /// <returns>Возвращает имя пользователя после успешной авторизации или регистрации</returns>
        public static string Authorization(Dictionary<string, string> infoList)
        {
            while (true)
            {
                Console.Write("Введите ваше имя: ");
                string _inputName = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string _inputPassword = Console.ReadLine();

                if (infoList.ContainsKey(_inputName) && infoList[_inputName] == _inputPassword) // Если содержит имя и пароль
                {
                    if (_inputName == "Влад")
                    {
                        Console.Clear();
                        string _answer;
                        Console.WriteLine("Добро пожаловать в систему админ!");
                        do
                        {
                            AdminOperations();
                            Console.WriteLine("Для подтверждения начала игры напишите - да.");
                            _answer = Console.ReadLine();
                            Console.Clear();
                            infoList = LoadUserInfo();
                        } while (_answer.ToLower() != "да");
                        return _inputName;
                    }
                    else
                    {
                        return _inputName;
                    }
                }
                else // Иначе осуществление регистрации
                {
                    Console.WriteLine("Хотите зарегистрироваться? Для подтверждения напишите - да.");
                    string _answer = Console.ReadLine();
                    if (_answer.ToLower() == "да")
                    {
                        var _lines = new List<string>(File.ReadAllLines(UsersFilePath));
                        Console.WriteLine("\nДобавьте нового пользователя:");
                        Console.Write("Имя: ");
                        _inputName = Console.ReadLine();
                        if (_inputName.ToLower() == "влад" || _inputName.ToLower() == "vlad")
                        {
                            Console.WriteLine("Нельзя добавить 2-го администратора.\n");
                            continue;
                        }
                        if (_lines.Exists(line => line.StartsWith(_inputName + "-")))
                        {
                            Console.WriteLine("Такой пользователь уже есть\n");
                            continue;
                        }
                        Console.Write("Пароль: ");
                        string _userPassword = Console.ReadLine();
                        using (StreamWriter _writer = new StreamWriter(UsersFilePath, true))
                        {
                            _writer.WriteLine($"{_inputName}-{_userPassword}");
                        }
                        Console.WriteLine("Пользователь добавлен.\n");
                        infoList = AuthorizationAndAdministration.LoadUserInfo();
                        Console.Clear();
                        return _inputName;
                    }
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Выполняет операции администратора, такие как добавление и удаление пользователей, начало игры или выход из игры
        /// </summary>
        public static void AdminOperations()
        {
            while (true)
            {
                Console.WriteLine("1) Добавить пользователя");
                Console.WriteLine("2) Удалить пользователя");
                Console.WriteLine("3) Играть");
                Console.WriteLine("4) Выход\n");
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
                        if (_userName.ToLower() == "влад" || _userName.ToLower() == "vlad")
                        {
                            Console.WriteLine("Нельзя добавить 2-го администратора.\n");
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
                        if (_userName.ToLower() == "влад" || _userName.ToLower() == "vlad")
                        {
                            Console.WriteLine("Невозможно удалить Администратора.\n");
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
                    case "3": // Начало игры
                        return;
                    default: // Ввод неправильной операции
                        Console.WriteLine("Неверная операция, пожалуйста, выберите снова.\n");
                        break;
                    case "4": // Выход
                        Environment.Exit(0);
                        break;
                }
            }
        }

        /// <summary>
        /// Загружает информацию о пользователях из файла, сохраняет в виде словаря
        /// </summary>
        /// <returns>Словарь с именами пользователей и их паролями</returns>
        public static Dictionary<string, string> LoadUserInfo()
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
