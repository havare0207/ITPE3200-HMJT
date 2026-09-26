using System.ComponentModel.DataAnnotations;
using HMJT.Models;
namespace HMJT.Tests.Models;

// Tests validation rules for the Question model.

public class QuestionTests{

// Validates a Question object using its DataAnnotation attributes.
 private static IList<ValidationResult> ValidateModel(Question question)
    {
        var validationContext = new ValidationContext(question);
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateObject(
            question,
            validationContext,
            validationResults,
            validateAllProperties: true);

        return validationResults;
    }

    // A valid question should pass all validation rules.

    [Fact]
    public void ValidQuestion_ShouldPassValidation()
    {
        var question = new Question
        {
            QuestionText = "What is 2 + 2?",
            Category = "Math",
            AnswerA = "3",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6",
            CorrectAnswer = AnswerOption.B
        };

        var results = ValidateModel(question);

        Assert.Empty(results);
    }
    // Question text is required.

    [Fact]
    public void EmptyQuestionText_ShouldFailValidation()
    {
        var question = new Question
        {
            QuestionText = "",
            Category = "Math",
            AnswerA = "3",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6"
        };

        var results = ValidateModel(question);

        Assert.Contains(results,
            r => r.ErrorMessage == "Question text is required.");
    }
        // Category is required.


    [Fact]
    public void EmptyCategory_ShouldFailValidation()
    {
        var question = new Question
        {
            QuestionText = "What is 2 + 2?",
            Category = "",
            AnswerA = "3",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6"
        };

        var results = ValidateModel(question);

        Assert.Contains(results,
            r => r.ErrorMessage == "Category is required.");
    }
    // Answer A is required.

    [Fact]
    public void EmptyAnswerA_ShouldFailValidation()
    {
        var question = new Question
        {
            QuestionText = "What is 2 + 2?",
            Category = "Math",
            AnswerA = "",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6"
        };

        var results = ValidateModel(question);

        Assert.Contains(results,
            r => r.ErrorMessage == "Answer A is required.");
    }
    
    
    // Question text cannot be longer than 500 characters.

    [Fact]
    public void QuestionTextOver500Characters_ShouldFailValidation()
    {
        var question = new Question
        {
            QuestionText = new string('a', 501),
            Category = "Math",
            AnswerA = "3",
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6"
        };

        var results = ValidateModel(question);

        Assert.Contains(results,
            r => r.ErrorMessage == "Question text cannot exceed 500 characters.");
    }

    //Answers can not be over 300 characters.
    [Fact]
    public void AnswerOver300Characters_ShouldFailValidation()
    {
        var question = new Question
        {
            QuestionText = "What is 2 + 2?",
            Category = "Math",
            AnswerA = new string('a', 301),
            AnswerB = "4",
            AnswerC = "5",
            AnswerD = "6"
        };

        var results = ValidateModel(question);

        Assert.Contains(results,
            r => r.ErrorMessage == "Answer A cannot exceed 300 characters.");
    }
} 