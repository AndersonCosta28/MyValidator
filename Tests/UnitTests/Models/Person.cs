namespace UnitTests.Models;

internal class Person
{
    public string? Name { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; } = default!;
    public Gender? Gender { get; set; } = default!;
}