using LinqToDB.Mapping;

namespace Models;

public class User
{
    [PrimaryKey]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Passwd { get; set; }
    public required string Role { get; set; }
    public string? Data { get; set; }
}