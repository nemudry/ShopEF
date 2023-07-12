
namespace TechClasses;
public static class Feedback
{
    //Ввод данных игроком
    public static int AnswerPlayerInt()
    {
        Color.CyanShort("Ваш ответ: ");
        int.TryParse(Console.ReadLine(), out int answer);
        Console.WriteLine();
        return answer;
    }
    public static string AnswerPlayerString()
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
