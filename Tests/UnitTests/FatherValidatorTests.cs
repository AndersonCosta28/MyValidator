namespace UnitTests;

public class FatherValidatorTests
{
    [Fact]
    public void Should_ReturnError_WhenWifeNameIsEmpty()
    {
        // Arrange
        var father = new Father
        {
            Name = "Carlos",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Masculino,
            Wife = new Person { Name = "" }, // Invalid
            Children =
        [
            new Person { Name = "João", DateOfBirth = DateTime.Today.AddYears(-10), Gender = Gender.Masculino },
            new Person { Name = "Maria", DateOfBirth = DateTime.Today.AddDays(1), Gender = Gender.Feminino } // Invalid
        ]
        };

        var validator = new FatherValidator();

        // Act
        var result = validator.Validate(father);

        // Assert
        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message == "Wife's name is required.");
    }

    [Fact]
    public void Should_ReturnError_WhenChildHasFutureDateOfBirth()
    {
        // Arrange
        var father = new Father
        {
            Name = "Carlos",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Masculino,
            Wife = new Person { Name = "Ana" },
            Children =
        [
            new Person { Name = "João", DateOfBirth = DateTime.Today.AddYears(-10), Gender = Gender.Masculino },
            new Person { Name = "Maria", DateOfBirth = DateTime.Today.AddDays(1), Gender = Gender.Feminino } // Invalid
        ]
        };

        var validator = new FatherValidator();

        // Act
        var result = validator.Validate(father);

        // Assert
        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message == "Child's date of birth cannot be in the future.");
    }

    [Fact]
    public void Should_ReturnError_WhenChildrenAreNotAllFemale()
    {
        // Arrange
        var father = new Father
        {
            Name = "Carlos",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Masculino,
            Wife = new Person { Name = "Ana" },
            Children =
        [
            new Person { Name = "João", DateOfBirth = DateTime.Today.AddYears(-10), Gender = Gender.Masculino } // Invalid
        ]
        };

        var validator = new FatherValidator();

        // Act
        var result = validator.Validate(father);

        // Assert
        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message == "All children must be female.");
    }

    [Fact]
    public void Should_ReturnError_WhenWifeIsNull()
    {
        // Arrange
        var father = new Father
        {
            Name = "Carlos",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Masculino,
            Wife = null!,
            Children = new List<Person>()
        };

        var validator = new FatherValidator();

        // Act
        var result = validator.Validate(father);

        // Assert - NotNull extension produces a message containing "is null"
        Assert.Contains(result.SelectMany(r => r.Errors), e => e.Message.Contains("is null"));
    }

    [Fact]
    public void Should_Pass_WhenAllValid()
    {
        // Arrange
        var father = new Father
        {
            Name = "Carlos",
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Masculino,
            Wife = new Person { Name = "Ana" },
            Children =
        [
            new Person { Name = "Maria", DateOfBirth = DateTime.Today.AddYears(-5), Gender = Gender.Feminino }
        ]
        };

        var validator = new FatherValidator();

        // Act
        var result = validator.Validate(father);

        // Assert - no errors
        Assert.All(result, r => Assert.True(r.IsValid));
    }
}