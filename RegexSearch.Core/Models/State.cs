namespace RegexSearch.Core.Models
{
    /// <summary>
    /// Перечисляемый тип для указания состояний конечного автомата
    /// </summary>
    public enum State
    {
        Start,       // Стартовое состояние
        LetterU,     // Прочитана буква U
        LetterS,     // Прочитано US
        LetterE,     // Прочитано USE
        LetterR,     // Прочитано USER
        Underscore,  // Прочитано USER_
        Digits,      // Читаем цифры (от 5 до 8)
        Accept,      // Принято
        Error        // Ошибка
    }
}
