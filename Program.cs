using JwtAspNet.Services;
using JwtAspNet.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TokenService>();


var app = builder.Build();

app.MapGet("/", (TokenService tokenService) => {
    var user = new User(
        Id: 1, 
        Name: "Vitório Baungartem", 
        Email: "teste@baungartem.com",
        Password: "123xyz",
        Image: "https://balta.io",
        Roles: new [] {"student", "premium"});
    
    return tokenService.Create(user);
});

app.Run();
