using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using LinqToDB.AspNet;
using LinqToDB;
using LinqToDB.AspNet.Logging;


var builder = WebApplication.CreateBuilder(args);

var accessTokenSecret = builder.Configuration["Jwt:AccessTokenSecret"]!;

builder.Services.AddLinqToDBContext<Db>(
	(provider, options) =>
		options
			.UseSQLite(builder.Configuration["ConnectionStrings:Default"]!)
			.UseDefaultLogging(provider)
);

builder.Services.AddSingleton<TokenGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition(
		"Bearer",
		new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey,
			In = ParameterLocation.Header,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			Description =
				"JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		}
	);

	options.AddSecurityRequirement(
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				new string[] { }
			}
		}
	);
});

builder.Services.AddCors();
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(
		"admin",
		policy => policy.RequireRole("admin")
	);
});


builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret)),
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			ValidateIssuerSigningKey = true,
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
		options.SaveToken = true;
	});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    if (!File.Exists("app.db")) {
        var dataConnection = scope.ServiceProvider.GetService<Db>()!;
        dataConnection.CreateTable<Models.Todo>();
        dataConnection.CreateTable<Models.User>();
    }
}

TodoItems.Reg(app).RequireAuthorization("admin");
User.Reg(app).RequireAuthorization();
Auth.Reg(app);

app.Run();
