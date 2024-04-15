using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinqToDB;
using Microsoft.IdentityModel.Tokens;

public static class User
{
    public static RouteGroupBuilder Reg(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/user");

        users.MapGet("/", GetAllUsers);
        users.MapGet("/{id}", GetUser);

        return users;
    }

    static async Task<IResult> GetAllUsers(Db db)
    {
        return JsonResult.Ok(await db.Users.Select(x => new UserDTO(x)).ToArrayAsync());
    }

    static async Task<IResult> GetUser(int id, Db db)
    {
        return await db.Users.SingleOrDefaultAsync(v => v.Id == id) is Models.User user
            ? JsonResult.Ok(new UserDTO(user))
            : TypedResults.NotFound();
    }
}
