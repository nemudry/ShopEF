
namespace TechClasses;
public static class Feedback
{
    //проверка условий на ввод данных игроком
    public static bool CheckСonditions(int answerInput, int MaxRange, int MinRange, params int[] exeptions)
    {

        if (!exeptions.Contains(answerInput))
        {
            if (!(answerInput >= MinRange && answerInput <= MaxRange))
            {
                Color.Red("Введенное значение неверно.");
                Console.WriteLine();
                return false;
            }
        }
        return true;
    }
    public static bool CheckСonditionsString(string answerInput, params int[] exeptions)
    {
        int.TryParse(answerInput, out int exeption);

        if (answerInput == null && !exeptions.Contains(exeption))
        {
            Color.Red("Введенное значение неверно.");
            Console.WriteLine();
            return false;
        }
        return true;
    }

    //Ввод данных игроком
    public static int PlayerAnswer()
    {
        Color.CyanShort("Ваш ответ: ");
        int.TryParse(Console.ReadLine(), out int answer);
        Console.WriteLine();
        return answer;
    }
    public static string PlayerAnswerString()
    {
        Color.CyanShort("Ваш ответ: ");
        string answer = Console.ReadLine();
        Console.WriteLine();
        return answer;
    }

    //ожидание нажатия клавиши
    public static void ReadKey()
    {
        Console.WriteLine();
        Console.WriteLine($"Нажмите клавишу для продолжения.");
        Console.ReadKey();
    }
}
