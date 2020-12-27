using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace TokenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var randomStringGenerator = new RandomStringGenerator();
            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var ECDsaSecurityKey = new ECDsaSecurityKey(secret)
            {
                KeyId = randomStringGenerator.GetRandomString(20)
            };

            var jsonWebKey = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(ECDsaSecurityKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("sub", "subjectId"),
                        }),
                TokenType = "at+jwt",
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "MyIssuer",
                IssuedAt = DateTime.UtcNow,
                Audience = "MyAudience",
                SigningCredentials = new SigningCredentials(jsonWebKey, SecurityAlgorithms.EcdsaSha256),
            };

            accessTokenDescriptor.Claims = new Dictionary<string, object> { { "client_id", "clientId" } };
            accessTokenDescriptor.Claims.Add("scope", "scope1");

            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            var tokenString = tokenHandler.WriteToken(accessToken);

            var a = tokenHandler.CanReadToken(tokenString);

            var b = tokenHandler.ReadJwtToken(tokenString);

            var validationParameters = new TokenValidationParameters();
            //validationParameters.IssuerSigningKeys = new List<SecurityKey> { publicKey };
            validationParameters.IssuerSigningKeys = new List<JsonWebKey> { jsonWebKey };
            validationParameters.ValidAudience = "MyAudience";
            validationParameters.ValidIssuer = "MyIssuer";

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            tokenHandler.ValidateToken(tokenString, validationParameters, out var securityToken);

            //var randomStringGenerator = new RandomStringGenerator();
            //var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            //var key2 = new ECDsaSecurityKey(secret)
            //{
            //    KeyId = randomStringGenerator.GetRandomString(20)
            //};

            //var exportedKey = key2.ECDsa.ExportParameters(true);

            //var data = JsonSerializer.Serialize(exportedKey);

            //var importedParameters = JsonSerializer.Deserialize<ECParameters>(data);

            //var secret2 = ECDsa.Create(importedParameters);

            //var key = new ECDsaSecurityKey(secret2)
            //{
            //    KeyId = randomStringGenerator.GetRandomString(20)
            //};

            //var publicParameters = key.ECDsa.ExportParameters(false);

            //var publicECDsa = ECDsa.Create(publicParameters);
            //var publicKey = new ECDsaSecurityKey(publicECDsa)
            //{
            //    KeyId = key.KeyId
            //};

            //var jwkSet = new JsonWebKeySet($"{{\"keys\":[{{\"kty\":\"EC\", \"use\":\"sig\", \"kid\":\"{key.KeyId}\", \"x\":\"{Base64UrlEncoder.Encode(publicParameters.Q.X)}\", \"y\":\"{Base64UrlEncoder.Encode(publicParameters.Q.Y)}\", \"crv\":\"P-256\", \"alg\":\"ES256\"}}]}}");

            //var keys = jwkSet.GetSigningKeys();
            //var jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(key);
            ////jwk.Alg = SecurityAlgorithms.EcdsaSha256;


            //var tokenHandler = new JwtSecurityTokenHandler();
            //var accessTokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //            {
            //                new Claim("sub", "subjectId"),
            //            }),
            //    TokenType = "at+jwt",
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    Issuer = "MyIssuer",
            //    IssuedAt = DateTime.UtcNow,
            //    Audience = "MyAudience",
            //    SigningCredentials = new SigningCredentials(key2, SecurityAlgorithms.EcdsaSha256),
            //};

            //accessTokenDescriptor.Claims = new Dictionary<string, object> { { "client_id", "clientId" } };
            //accessTokenDescriptor.Claims.Add("scope", "scope1");

            //var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            //var tokenString = tokenHandler.WriteToken(accessToken);

            //var a = tokenHandler.CanReadToken(tokenString);

            //var b = tokenHandler.ReadJwtToken(tokenString);

            //var validationParameters = new TokenValidationParameters();
            ////validationParameters.IssuerSigningKeys = new List<SecurityKey> { publicKey };
            //validationParameters.IssuerSigningKeys = keys;
            //validationParameters.ValidAudience = "MyAudience";
            //validationParameters.ValidIssuer = "MyIssuer";

            //Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            //tokenHandler.ValidateToken(tokenString, validationParameters, out var securityToken);

        }
    }

    public class RandomStringGenerator
    {
        private readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public string GetRandomString(int length)
        {
            var bytes = new byte[length];

            CryptoRandom.GetBytes(bytes);

            return Base64UrlTextEncoder.Encode(bytes);
        }
    }
}
