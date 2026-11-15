# RegexSearch

СРС-1: поиск подстрок двумя способами (регулярные выражения и конечный автомат).
Вариант 1: идентификатор пользователя, формат USER_ и 5-8 цифр.

Регулярное выражение: \bUSER_[0-9]{5,8}\b

Запуск: открыть RegexSearch.sln в Visual Studio 2022, стартовый проект RegexSearch.WPF.
Либо в консоли: dotnet run --project RegexSearch.WPF
