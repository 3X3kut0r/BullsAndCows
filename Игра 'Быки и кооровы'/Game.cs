using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Игра__Быки_и_кооровы_
{
    public class Game
    {
        /// <summary>
        /// Основной цикл игры "Быки и Коровы", который управляет игровым процессом и взаимодействием с игроком.
        /// </summary>
        public void GamePlay()
        {
            do
            {
                string _secretNumber = GameMechanics.GenerateSecretNumber(4);
                string _guess;
                int _attempts = 0;
                Stopwatch _time = new Stopwatch();
                Dictionary<string, (int Wins, int Attempts)> _leaderboard = GameMechanics.LoadLeaderboard();
                Dictionary<string, string> _infoList = AuthorizationAndAdministration.LoadUserInfo();
                Console.WriteLine("Добро пожаловать в игру 'Быки и коровы'");
                string _playerName = AuthorizationAndAdministration.Authorization(_infoList);
                Console.Clear();
                Console.WriteLine($"{_secretNumber}");
                Logging.Logger.Information($"{_playerName} начал(а) игру");
                _time.Start();
                do
                {
                    int _cows = 0;
                    int _bulls = 0;
                    _attempts++;
                    Console.Write($"Введите ваше предположение {_playerName}: ");
                    _guess = Console.ReadLine();
                    Logging.Logger.Information($"{_playerName} ввел(а) предположение {_guess}");
                    if (_guess.Length != _secretNumber.Length || _guess.Distinct().Count() != 4 || !_guess.All(char.IsDigit))
                    {
                        Console.WriteLine("Введите корректное 4-значное число без повторяющихся цифр.");
                        Logging.Logger.Warning($"{_playerName} ввел(а) неверное предпложение");
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
                Logging.Logger.Information($"{_playerName} выиграл(а) игру");
                GameMechanics.UpdateLeaderboard(_playerName, _attempts, _leaderboard);
                GameMechanics.DisplayLeaderboard(_leaderboard);
                Console.WriteLine("\nХотите сыграть еще? (да/нет)");
                Console.Write("Ваш ответ: ");
                string _answer = Console.ReadLine();
                if (_answer.ToLower() == "нет")
                {
                    Logging.Logger.Information($"{_playerName} закончил(а) игру");
                    return;
                }
                else
                {
                    Logging.Logger.Information("Запуск новой игры");
                    Console.Clear();
                }
            } while (true);
        }
    }
}
