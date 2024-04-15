using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinqToDB;
using Microsoft.IdentityModel.Tokens;

public static class Auth
{
    public static void Reg(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", UserLogin);
        app.MapPost("/reg", CreateUser);
    }

    static async Task<IResult> UserLogin(string name, string passwd, Db db, TokenGenerator generator)
    {
        var user = await db.Users.FirstOrDefaultAsync(t => t.Name == name);
        if (user is null)
            return TypedResults.NotFound();
        if (passwd == user.Passwd)
        {
            return JsonResult.Ok(generator.GenerateAccessToken(user));
        }

        return TypedResults.Forbid();
    }

    static async Task<IResult> CreateUser(Models.User user, Db db)
    {
        var id = await db.InsertAsync(user);

        return TypedResults.Created($"/user/{id}", user);
    }
}
