using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Игра__Быки_и_кооровы_
{
    public class GameMechanics
    {
        public static string ResultsFilePath = "results.txt"; // Файл с информацией о играх пользователей

        /// <summary>
        /// Генерирует секретное число заданной длины, состоящее из уникальных цифр
        /// </summary>
        /// <param name="numberLength">Количество цифр в секретном числе</param>
        /// <returns>Сгенерированное секретное число в виде строки</returns>
        public static string GenerateSecretNumber(int numberLength)
        {
            Random _random = new Random();
            string _result = "";
            while (_result.Length < numberLength)
            {
                string _nextDigit = _random.Next(0, 10).ToString();
                if (!_result.Contains(_nextDigit))
                {
                    _result += _nextDigit;
                }
            }
            return _result;
        }

        /// <summary>
        /// Загружает таблицу рекордов из файла в словарь
        /// </summary>
        /// <returns>
        /// Словарь, где ключ - имя игрока, а значение - кортеж с количеством побед и попыток
        /// </returns>
        public static Dictionary<string, (int Wins, int Attempts)> LoadLeaderboard()
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

        /// <summary>
        /// Обновляет таблицу рекордов новыми данными о игроке и сохраняет их в файл
        /// </summary>
        /// <param name="playerName">Имя игрока, данные которого необходимо обновить</param>
        /// <param name="playerAttempts">Количество попыток в последней игре для этого игрока</param>
        /// <param name="leaderBoard">Словарь рекордов, который необходимо обновить</param>
        public static void UpdateLeaderboard(string playerName, int playerAttempts, Dictionary<string, (int Wins, int Attempts)> leaderBoard)
        {
            try
            {
                if (leaderBoard.ContainsKey(playerName)) // Если содержит имя игрока
                {
                    var _playerStats = leaderBoard[playerName];
                    _playerStats.Wins++;
                    _playerStats.Attempts += playerAttempts;
                    leaderBoard[playerName] = _playerStats;
                }
                else // Иначе создается новвый игрок
                {
                    leaderBoard[playerName] = (1, playerAttempts);
                }
                using (StreamWriter _writer = new StreamWriter(ResultsFilePath))
                {
                    foreach (var _player in leaderBoard)
                    {
                        _writer.WriteLine($"{_player.Key}-{_player.Value.Wins}-{_player.Value.Attempts}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обновлении таблицы рекордов");
                Logging.Logger.Error(ex, "Ошибка при обновлении таблицы рекордов");
            }
        }

        /// <summary>
        /// Отображает таблицу рекордов в консоли, отсортированную по эффективности (процент побед)
        /// </summary>
        /// <param name="leaderBoard">Словарь с данными рекордов для отображения</param>
        public static void DisplayLeaderboard(Dictionary<string, (int Wins, int Attempts)> leaderBoard)
        {
            Console.WriteLine("\nТаблица рекордов:");
            Console.WriteLine("Имя:\t\tПобеды:\t\tПопытки:\tЭффективность:");
            var _sortedLeaderBoard = leaderBoard.OrderByDescending(p => (double)p.Value.Wins / p.Value.Attempts).ThenBy(p => p.Value.Attempts);
            foreach (var _player in _sortedLeaderBoard)
            {
                double _winRate = 100 * _player.Value.Wins / _player.Value.Attempts;
                Console.WriteLine($"{_player.Key}\t\t{_player.Value.Wins}\t\t{_player.Value.Attempts}\t\t({_winRate}%)");
            }
        }      
    }
}
