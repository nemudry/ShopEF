namespace TechClasses;
public class Validator
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
}
