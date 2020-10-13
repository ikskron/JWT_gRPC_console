using System;
using System.Net;
using System.Linq.Expressions;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Security;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
//using System.​Security.​Principal;




namespace JWT_gRPC_console
{

    class Program
    {
        static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
        static void Main(string[] args)
        {
            var stringToken = GenerateToken();
            ValidateToken(stringToken);

            //            Console.WriteLine("Hello World!");
        }
               


        private static string GenerateToken()
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var secToken = new JwtSecurityToken(
                signingCredentials: credentials,
                issuer: "Sample",
                audience: "Sample",
                claims: new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, "meziantou")
                },
                expires: DateTime.UtcNow.AddDays(1));

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(secToken);
        }

        private static bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            System.Security.Principal.IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // The same key as the one that generate the token
            };
        }




    }
}
