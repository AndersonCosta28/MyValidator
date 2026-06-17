namespace UnitTests.Models;

internal class Father : Person
{
    public Person? Wife { get; set; } = default!;
    public List<Person>? Children { get; set; } = default!;
}