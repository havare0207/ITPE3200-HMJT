// Gives this file access to Question, Difficulty and AnswerOption.
using HMJT.Models;

// Places MockQuestionData in the HMJT.MockData namespace.
namespace HMJT.MockData;

// Contains temporary question data used before the database is implemented.
public static class MockQuestionData
{
    // Returns a list of mock questions that can be used by QuestionService.
    public static List<Question> GetQuestions()
    {
        return new List<Question>
        {

            // 1. Java

            new Question
            {
                QuestionId = 1,
                QuestionText = "Which keyword is used to create a class in Java?",
                Category = "Java",
                Difficulty = Difficulty.Easy,

                AnswerA = "class",
                AnswerB = "new",
                AnswerC = "object",
                AnswerD = "define",

                CorrectAnswer = AnswerOption.A
            },

            new Question
            {
                QuestionId = 2,
                QuestionText = "Which method is the entry point of a Java application?",
                Category = "Java",
                Difficulty = Difficulty.Medium,

                AnswerA = "start",
                AnswerB = "run",
                AnswerC = "main",
                AnswerD = "init",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 3,
                QuestionText = "Which keyword prevents a method from being overridden in Java?",
                Category = "Java",
                Difficulty = Difficulty.Hard,

                AnswerA = "private",
                AnswerB = "final",
                AnswerC = "static",
                AnswerD = "sealed",

                CorrectAnswer = AnswerOption.B
            },

            // 2. JavaScript
           
            new Question
            {
                QuestionId = 4,
                QuestionText = "Which keyword can be used to declare a variable in JavaScript?",
                Category = "JavaScript",
                Difficulty = Difficulty.Easy,

                AnswerA = "int",
                AnswerB = "string",
                AnswerC = "let",
                AnswerD = "define",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 5,
                QuestionText = "Which operator checks both value and type equality in JavaScript?",
                Category = "JavaScript",
                Difficulty = Difficulty.Medium,

                AnswerA = "==",
                AnswerB = "===",
                AnswerC = "=",
                AnswerD = "!=",

                CorrectAnswer = AnswerOption.B
            },

            new Question
            {
                QuestionId = 6,
                QuestionText = "Which array method creates a new array by transforming every element?",
                Category = "JavaScript",
                Difficulty = Difficulty.Hard,

                AnswerA = "forEach",
                AnswerB = "filter",
                AnswerC = "find",
                AnswerD = "map",

                CorrectAnswer = AnswerOption.D
            },

            // 3. HTML/CSS

            new Question
            {
                QuestionId = 7,
                QuestionText = "Which HTML element is used for the largest heading?",
                Category = "HTML/CSS",
                Difficulty = Difficulty.Easy,

                AnswerA = "h6",
                AnswerB = "header",
                AnswerC = "h1",
                AnswerD = "title",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 8,
                QuestionText = "Which CSS property changes the text color?",
                Category = "HTML/CSS",
                Difficulty = Difficulty.Medium,

                AnswerA = "text-color",
                AnswerB = "font-color",
                AnswerC = "foreground",
                AnswerD = "color",

                CorrectAnswer = AnswerOption.D
            },

            new Question
            {
                QuestionId = 9,
                QuestionText = "Which CSS layout system uses rows and columns?",
                Category = "HTML/CSS",
                Difficulty = Difficulty.Hard,

                AnswerA = "Grid",
                AnswerB = "Float",
                AnswerC = "Position",
                AnswerD = "Inline",

                CorrectAnswer = AnswerOption.A
            },

            // 4. Python

            new Question
            {
                QuestionId = 10,
                QuestionText = "Which keyword is used to define a function in Python?",
                Category = "Python",
                Difficulty = Difficulty.Easy,

                AnswerA = "function",
                AnswerB = "func",
                AnswerC = "def",
                AnswerD = "method",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 11,
                QuestionText = "Which Python data type stores key-value pairs?",
                Category = "Python",
                Difficulty = Difficulty.Medium,

                AnswerA = "List",
                AnswerB = "Tuple",
                AnswerC = "Set",
                AnswerD = "Dictionary",

                CorrectAnswer = AnswerOption.D
            },

            new Question
            {
                QuestionId = 12,
                QuestionText = "Which keyword creates an anonymous function in Python?",
                Category = "Python",
                Difficulty = Difficulty.Hard,

                AnswerA = "lambda",
                AnswerB = "anonymous",
                AnswerC = "func",
                AnswerD = "delegate",

                CorrectAnswer = AnswerOption.A
            },

            // 5. Game History

            new Question
            {
                QuestionId = 13,
                QuestionText = "Which company created the Mario series?",
                Category = "Game History",
                Difficulty = Difficulty.Easy,

                AnswerA = "Sony",
                AnswerB = "Nintendo",
                AnswerC = "Sega",
                AnswerD = "Microsoft",

                CorrectAnswer = AnswerOption.B
            },

            new Question
            {
                QuestionId = 14,
                QuestionText = "Which company created the original PlayStation?",
                Category = "Game History",
                Difficulty = Difficulty.Medium,

                AnswerA = "Nintendo",
                AnswerB = "Sega",
                AnswerC = "Sony",
                AnswerD = "Atari",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 15,
                QuestionText = "In which decade was the original Pong released?",
                Category = "Game History",
                Difficulty = Difficulty.Hard,

                AnswerA = "1960s",
                AnswerB = "1970s",
                AnswerC = "1980s",
                AnswerD = "1990s",

                CorrectAnswer = AnswerOption.B
            },

            // 6. C#

            new Question
            {
                QuestionId = 16,
                QuestionText = "Which keyword is used to create a class in C#?",
                Category = "C#",
                Difficulty = Difficulty.Easy,

                AnswerA = "class",
                AnswerB = "object",
                AnswerC = "type",
                AnswerD = "define",

                CorrectAnswer = AnswerOption.A
            },

            new Question
            {
                QuestionId = 17,
                QuestionText = "Which keyword is used to create an object from a class in C#?",
                Category = "C#",
                Difficulty = Difficulty.Medium,

                AnswerA = "create",
                AnswerB = "make",
                AnswerC = "new",
                AnswerD = "instance",

                CorrectAnswer = AnswerOption.C
            },

            new Question
            {
                QuestionId = 18,
                QuestionText = "Which keyword prevents a class from being inherited in C#?",
                Category = "C#",
                Difficulty = Difficulty.Hard,

                AnswerA = "final",
                AnswerB = "private",
                AnswerC = "static",
                AnswerD = "sealed",

                CorrectAnswer = AnswerOption.D
            }
        };
    }
}