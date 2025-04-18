internal class Father : Person
{
    public required Person Wife { get; set; }
    public required List<Person> Children { get; set; }
}
