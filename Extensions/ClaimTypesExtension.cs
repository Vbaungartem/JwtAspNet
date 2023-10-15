using System.Security.Claims;

namespace JwtAspNet.Extensions;

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

    public static string Name(this ClaimsPrincipal user)
    {
        try
        {
            var name = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value ?? "0";
            return name;
        }
        catch (System.Exception)
        {

            return "Nome não encontrado";
        }
    }

    public static string GivenName(this ClaimsPrincipal user)
    {
        try
        {
            var givenName = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value ?? "0";
            return givenName;
        }
        catch (System.Exception)
        {

            return "Apelido não encontrado";
        }
    }

    public static string Email(this ClaimsPrincipal user)
    {
        try
        {
            var email = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ?? "0";
            return email;
        }
        catch (System.Exception)
        {

            return "Email não encontrado";
        }
    }

    public static string Image(this ClaimsPrincipal user)
    {
        try
        {
            var image = user.Claims.FirstOrDefault(claim => claim.Type == "Image")?.Value ?? "0";
            return image;
        }
        catch (System.Exception)
        {

            return "Imagem de Usuário não informada/encontrada.";
        }
    }
}