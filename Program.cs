using JwtAspNet.Services;
using JwtAspNet.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtAspNet;
using System.Security.Claims;
using JwtAspNet.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TokenService>();

builder.Services.AddAuthentication(x =>
{

    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters{
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("admin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/login", (TokenService tokenService) =>
{
    var user = new User(
        Id: 1,
        Name: "Vitório Baungartem",
        Email: "teste@baungartem.com",
        Password: "123xyz",
        Image: "https://balta.io",
        Roles: new[] { "student", "premium" });

    return tokenService.Create(user);
});

app.MapGet("/restrict", (ClaimsPrincipal user) => new {
    id = user.Id(),
    name = user.Name(),
    email = user.Email(),
    image = user.Image(),
    givenName = user.GivenName()

}  ).RequireAuthorization();


app.MapGet("/admin", () => "Você possui acesso de Administrador!" ).RequireAuthorization("admin");
app.Run();
