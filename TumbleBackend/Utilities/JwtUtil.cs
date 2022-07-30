using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TumbleBackend.InternalModels;

namespace TumbleBackend.Utilities;

public static class JwtUtil
{
    public static string GenerateRefreshToken(string encKey, string sigKey, int refreshTokenExpiration, string username, string password)
    {
        JwtSecurityTokenHandler jwtHandler = new();
        byte[] ecKeyTemp = Encoding.UTF8.GetBytes(encKey);
        byte[] scKey = Encoding.UTF8.GetBytes(sigKey);

        byte[] ecKey = new byte[256 / 8];
        Array.Copy(ecKeyTemp, ecKey, 256 / 8);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username), new Claim("password", password) }),
            Expires = DateTime.UtcNow + TimeSpan.FromSeconds(refreshTokenExpiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(scKey), SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(ecKey), SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.Aes256CbcHmacSha512),
            Issuer = "TumbleBackend",
            Audience = "TumbleApp",
        };

        return jwtHandler.CreateEncodedJwt(tokenDescriptor);
    }

    public static RefreshTokenResponseModel? ValidateAndReadRefreshToken(string encKey, string sigKey, string token)
    {
        if (token == null)
            return null;

        JwtSecurityTokenHandler tokenHandler = new();
        byte[] ecKeyTemp = Encoding.UTF8.GetBytes(encKey);
        byte[] scKey = Encoding.UTF8.GetBytes(sigKey);

        byte[] ecKey = new byte[256 / 8];
        Array.Copy(ecKeyTemp, ecKey, 256 / 8);

        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "TumbleApp",
                    ValidIssuer = "TumbleBackend",
                    IssuerSigningKey = new SymmetricSecurityKey(scKey),
                    TokenDecryptionKey = new SymmetricSecurityKey(ecKey),
                },
                out SecurityToken validatedToken
            );

            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            string username = jwtToken.Claims.First(x => x.Type == "username").Value;
            string password = jwtToken.Claims.First(x => x.Type == "password").Value;

            return new(username, password);
        }
        catch
        {
            return null;
        }
    }
}
