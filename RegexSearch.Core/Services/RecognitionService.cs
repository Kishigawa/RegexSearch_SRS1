using System.Collections.Generic;
using System.Text.RegularExpressions;
using RegexSearch.Core.Models;

namespace RegexSearch.Core.Services
{
    /// <summary>
    /// Сервис поиска идентификаторов пользователя (USER_ и 5-8 цифр) двумя способами
    /// </summary>
    public class RecognitionService
    {
        public const string Pattern = @"\bUSER_[0-9]{5,8}\b";
        private const int MinDigits = 5;
        private const int MaxDigits = 8;

        // Режим 1: библиотечные регулярные выражения
        public List<MatchResult> FindWithRegex(string text)
        {
            var results = new List<MatchResult>();
            int number = 1;

            foreach (Match m in Regex.Matches(text, Pattern))
            {
                results.Add(new MatchResult
                {
                    Number = number++,
                    Value = m.Value,
                    Position = m.Index,
                    Length = m.Length
                });
            }
            return results;
        }

        // Режим 2: конечный автомат, библиотека Regex не используется
        public List<MatchResult> FindWithAutomaton(string text)
        {
            var results = new List<MatchResult>();
            int number = 1;
            int start = 0;

            while (start < text.Length)
            {
                // левая граница: перед началом не должно быть символа слова
                if (start > 0 && IsWordChar(text[start - 1]))
                {
                    start++;
                    continue;
                }

                State state = State.Start;
                int counter = 0;   // сколько цифр прочитано
                int i = start;
                int end = -1;      // позиция сразу после найденного идентификатора

                while (i < text.Length)
                {
                    state = Next(state, text[i], ref counter);

                    if (state == State.Error)
                    {
                        break;
                    }
                    if (state == State.Accept)
                    {
                        // принят по символу-ограничителю, он в совпадение не входит
                        end = i;
                        break;
                    }
                    i++;
                }

                // текст закончился сразу после цифр, это тоже правая граница
                if (state == State.Digits && i == text.Length && counter >= MinDigits)
                {
                    state = State.Accept;
                    end = text.Length;
                }

                if (state == State.Accept)
                {
                    results.Add(new MatchResult
                    {
                        Number = number++,
                        Value = text.Substring(start, end - start),
                        Position = start,
                        Length = end - start
                    });
                    start = end;
                }
                else
                {
                    start++;
                }
            }
            return results;
        }

        // Функция переходов автомата
        private static State Next(State state, char c, ref int counter)
        {
            switch (state)
            {
                case State.Start:
                    return c == 'U' ? State.LetterU : State.Error;

                case State.LetterU:
                    return c == 'S' ? State.LetterS : State.Error;

                case State.LetterS:
                    return c == 'E' ? State.LetterE : State.Error;

                case State.LetterE:
                    return c == 'R' ? State.LetterR : State.Error;

                case State.LetterR:
                    return c == '_' ? State.Underscore : State.Error;

                case State.Underscore:
                    if (IsDigit(c)) { counter = 1; return State.Digits; }
                    return State.Error;

                case State.Digits:
                    if (IsDigit(c))
                    {
                        // девятая цифра недопустима
                        if (counter < MaxDigits) { counter++; return State.Digits; }
                        return State.Error;
                    }
                    // цифры закончились: принимаем, если их хватает
                    // и следующий символ не часть слова
                    if (counter >= MinDigits && !IsWordChar(c)) return State.Accept;
                    return State.Error;

                default:
                    return State.Error;
            }
        }

        // Только цифры 0-9. В шаблоне тоже используется [0-9] вместо \d,
        // потому что \d в .NET совпадает с цифрами любых алфавитов Unicode
        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsWordChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }
    }
}
