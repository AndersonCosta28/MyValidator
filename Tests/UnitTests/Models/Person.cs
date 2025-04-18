internal enum Gender
{
    Masculino, Feminino
}

internal class Person
{
    public required string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
}
