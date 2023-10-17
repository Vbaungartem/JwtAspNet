# Implementando Segurança e Autenticação de API’s [ASP.NET](http://ASP.NET) com JWT e Bearer authentication 🚀

Meu Linkedin: [https://www.linkedin.com/in/vitório-baungartem-221041192/](https://www.linkedin.com/in/vit%C3%B3rio-baungartem-221041192/)

Meu github: [https://github.com/Vbaungartem](https://github.com/Vbaungartem)

Repositório do Projeto: https://github.com/Vbaungartem/JwtAspNet

Repositório do Projeto final: https://github.com/Vbaungartem/JwtStore/tree/main !!!!!
---

### Não esqueça de deixar sua ⭐!!!

**OBSERVAÇÕES IMPORTANTES:** 

A medida que o projeto for evoluindo, todas as descrições presentes aqui no README irão ser atualizadas. Por hora, elas são unicamente para registrar minhas anotações e entendimentos sobre o projeto do curso. Ainda não há uma publicação preparada nesse ponto do projeto. 

[Work in progress!]

O projeto deste repositório está voltado para a conclusão do curso **Segurança em APIs ASP.NET com JWT e Bearer Authentication** ministrada pelo Professor André Baltieri, proprietário da plataforma Balta.io. 

Tanto os código encontrados aqui, quanto as anotações dispostas no Readme e no docs.zip do projeto tem o objetivo de implementar, explicar e validar meus conhecimentos na área de segurança e desenvolvimento com C# e .Net Core.

---

Comandos iniciais utilizados:

1. Primeiramente é inicializado um projeto com o padrão Minimal API que é um padrão totalmente reduzido e otimizado do AspNet Core. 

```sql
dotnet new web -o "nomeProjeto" (sem aspas)

cd "nomeProjeto" 
```

1. Logo após é adicionado um Package responsável pelos recursos de autenticação que serão utilizados: 

```sql
donet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

## Implementação:

Inicialmente foi criado um arquivo “Configuration.cs” no diretório principal do projeto:

Esse arquivo consiste na criação de uma static class “Configuration” cujo carrega nossa tão importante privateKey. Obs.: nesse ponto da implementação, a privateKey está sendo atribuída de forma simples, entretanto isso irá melhorar no desenvolvimento do projeto e do curso. 

```csharp
namespace JwtAspNet;

public static   class Configuration
{
    public static string PrivateKey { get; set; } = "65*fefef[+asd1]1421s32RESEF_1FDX_34(AS12)";

}
```

---

Logo após isso, criamos dentro de um diretório “Models” um arquivo/class chamado “User”:

```csharp
namespace JwtAspNet.Models;

public record User(
    int Id,
    string Email,
    string Name,
    string Password,
    string[] Roles,
    string Image
)
{

}
```

O User irá carregar algumas informações essenciais para a validação, afirmação e identificação do usuário logado, e portará essas informações que serão disponibilizadas para o JWT, e inclusive, sua senha. 

**É interessante apontar que no curso o Balta (professor) nos instrui a utilizar o tipo “record” e não “class”.** 

O recurso Record é um recurso que entrou com o C# 9.0, cujo é um tipo por referência que se comporta como um tipo por valor. Onde ele irá apontar para um endereço de memória onde a classe foi instanciada porém irá se comportar com as seguintes condições:

- Imutável
- Suporte construtores e desconstrutores
- Suporte  a heranças
- Suporte à expressão with
- Vale dizer também que ao definir um tipo Record o compilador vai estar disponibilizando outros métodos comumente utilizados em tipos como struct e class, como “GetHashCode()”, PrintMembers(), ToString() e outros.

---

Prosseguindo geramos uma Service **(Services/TokenService)** que será responsável pelas principais construções envolvendo nosso Token, Roles e Claims do usuário. 

Ela será tratada por partes nos próximos parágrafos. 

1. **CreateToken**:

```csharp
public string Create(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(Configuration.PrivateKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key), 
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(4),
            Subject = GenerateClaims(user)
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
```

O createToken é composto pelos passos que irão gerar e retornar nosso token propriamente dito! 

Inicialmente criamos um handler equivalente a um JwtSecurityTokenHandler():

- que pode ser definido como um criador e validador de Json Web Tokens, um objeto pertencente ao System IdentityModels Tokens que carrega funções essenciais já desenvolvidas para trabalhar na autenticação e geração de web tokens.

Logo em seguida realizamos o enconding para um byte[] da nossa PrivateKey definida na Configuration do nosso projeto. Fazemos isso pois ao realizar a criação de nossas Credentials, devemos entregar um tipo byte[] e não um tipo string para a construção do SigningCredentials. 

Na construção das credentials, instanciamos um tipo **SigningCredentials** e seu construtor recebe uma SymetricSecurityKey(byte[] key) e o algoritmo de encriptação ( como descrito nos métodos acima). A **signingCredentials** nada mais representa que a própria chave de criptografia e os algoritmos de encriptação utilizados para gerar a assinatura, mais uma das implementações já tratadas do IdentityModel. 

Após isso seguimos com a sequência lógica de criar um TokenDescriptor que carrega as credentials, o tempo de vigência do Token (Expires) e por fim o Subject, onde são especificadas as Roles e Claims. Para popular nosso **Subject**, utilizamos um método interno chamado **GenerateClaims()** que será tratado mais a frente neste texto. 

Com nossos parâmetros que compõe o token definido, iremos por fim instancia-lo através do **token = handler.CreateToken()** que recebe o tokenDescriptor gerado acima. e por fim, retornamos como resultado do método, o **handler.WriteToken(token),** finalizando o clico de vida do nosso CreateToken.

1. **GenerateClaims()**

```csharp
private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();

        claims.AddClaim(new Claim("Id", user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        claims.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
        claims.AddClaim(new Claim("Image", user.Image));
        
        foreach(var role in user.Roles)
            claims.AddClaim(new Claim(ClaimTypes.Role, role));
        
        return claims;
    }
```

Esse método é unicamente responsável pela criação das nossas claims, que nada mais são do que afirmações que entregam importantes parâmetros do nosso usuário logado. 

Sendo assim, instanciamos uma variável chamada claims que recebe o tipo **ClaimsIndeity** que nada mais é do que uma classe preparada para carregar corretamente as claims do usuário. 

São adicionados 5 tipos de Claims, sendo 2 personalizadas e 3 padronizadas nos tipos previstos **ClaimTypes** (um enum que visa padronizar as types mais comumente utilizadas em sistemas de autenticação. 

Além das Claims, são adicionadas também cada Role presente em nosso atributo Roles[] como uma ClaimType.Role. 

E por fim, retornando a ClaimsIdentity claims. 

---

### **Configurando a Program:**

Assim como feito acima, deixarei a configuração da Program.cs divida em duas partes:

1. Configurando Builder e Autenticação:

```csharp
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
```

Adicionamos ao Builder nosso TokenService que construímos anteriormente, e através do método AddAuthentication definimos tanto o Scheme default de autenticação quanto a forma como ele irá questionar isso ao user (defaultChalangeScheme) para o JwtBearerDefaults.AuthenticationScheme que irá tratar isso automaticamente para nós. 

Com um ganho também já configuramos o AddJwtBearer passando a nossa chave (Configuration.PrivateKey) também como um byte[], e desativando as pções de validationIssuer e validationAudience, pois se trata de um método mais simplificado de autenticação. 

Abaixo vemos um exemplo citado no curso que seria a forma de adicionar uma Policy no Authorization nomeada como “admin” que para sua liberação, necessita que o usuário instanciado possua uma Role específica, nesse caso, nomeada como “Admin”. (este foi apenas um trecho informativo, até o ponto em questão esse trecho de código não é totalmente utilizado). 

Após isso geramos nosso app  com o Build() de forma padrão. E seguimos com as opções UseAuthentication(); e UserAuthorization(); (note que aqui não utilizamos Notations por se tratar de uma MinimalApi, sendo um projeto otimizado e minimizado). 

1. Definindo rota de Login e geração do Token, e rota para exibir os resultados de nossas implementações: 
2. 

```csharp
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
```

Na rota de /login, podemos observar que geramos um user estático para teste que carrega suas principais claims e as roles “student” e “premium”. após instanciar o user, já enviamos o mesmo para o tokenService.Create() que será responsável por retornar nosso token. o resultado dessa rota é literalmente o token escrito.

E por fim temos a rota /restrict que é responsável por validar o usuário (necessitando da entrega do Token na requisição) e retornando o valor de cada informação do nosso usuário logado. 

Nesse ponto é importantíssimo notar a criação de um novo diretório do nosso projeto chamado **Extensions**.

Esse diretório comporta algumas extensões de Tipos que trabalharão com validações específicas que podem nos ajudar a tratar exceptions de forma organizada, como o exemplo do tipo **ClaimsPrincipal**.

Observe a implementação: 

```csharp
public static class ClaimTypesExtension
{
    public static int Id(this ClaimsPrincipal user)
    {
        try
        {
            var id = user.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? "0";
            return int.Parse(id);
        }
        catch (System.Exception)
        {

            return 0;
        }
    }
//[others implementations...]
```

Aqui são tratadas as formas de retornar o valor propriamente dito das ClaimsPrincipal user, sem que hajam riscos de um break na compilação por um Null Value Object por exemplo. 

A primeira parte do projeto previsto pelo curso termina aqui, e o próximo passo é a implementação de um sistema de login completo.
