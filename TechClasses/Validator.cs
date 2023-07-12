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
    //проверка условий на ввод данных игроком, строки
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

    //получение проверенного  ответа
    public static int GetChechedAnswer(string question, string[] answers, params string[] returnAnswers)
    {
        int clientAnswer = 0;
        while (true)
        {
            var range = AskAnswer.ShowQuestionAnswers(question, answers, returnAnswers); //показать вопрос и ответ

            clientAnswer = AskAnswer.AnswerPlayerInt(); // получение ответа клиента

            if (CheckСonditions(clientAnswer, answers.Length, 1, range)) break; // проверка ответа валидатором
        }
        return clientAnswer;
    }
}
