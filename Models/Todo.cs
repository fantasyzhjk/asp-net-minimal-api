using LinqToDB.Mapping;

namespace Models;

public class Todo
{
    [PrimaryKey]
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}