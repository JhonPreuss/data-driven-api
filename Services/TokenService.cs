using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shop.Models;

namespace Shop.Services
{
    public static class TokenService
    {//Classe estática, não precisa instanciar "New"
    //Aqui vamos ter um método estático que retorna uma string Token
        public static string GenerateToken(User user)
        {
            //Recebe um User , e o método TokenHanler irá gerar o token
            var tokenHander = new JwtSecurityTokenHandler();

            //Decode da chave definida
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            //Responsável por descrever tudo que contém o token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity ( new Claim[]
                {
                    //Claim -> são os dados do usuário e são emitidas por uma fonte confiável. Se estivermos trabalhando com autenticação baseada em token, uma declaração pode ser adicionada a um token pelo servidor que gera o token
                //Acaba retornando uma face "nome/valor"
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(2), //Duração do token
                //aqui geramos o token
                SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHander.CreateToken(tokenDescriptor);
            return tokenHander.WriteToken(token);
        }
        
    }
}