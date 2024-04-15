public class UserDTO
{
    public string? Name { get; set; }
    public string? Data { get; set; }
    public string? Role { get; set; }

    public UserDTO(Models.User user) => (Name, Data, Role) = (user.Name, user.Data, user.Role);
}
