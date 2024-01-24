using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TumbleBackend.InternalModels;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.StringConstants;

namespace TumbleBackend.Utilities;

public class JwtUtil
{
    readonly string jwtEncryptionKey;
    readonly string jwtSignatureKey;
    readonly int refreshTokenExpireTime;

    public JwtUtil([FromServices] IConfiguration configuration)
    {
        string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
        string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
        string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
        if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
            throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

        jwtEncryptionKey = jwtEncKey;
        jwtSignatureKey = jwtSigKey;
        refreshTokenExpireTime = int.Parse(refreshTokenExpiration);
    }

    public string GenerateRefreshToken(string username, string password)
    {
        JwtSecurityTokenHandler jwtHandler = new();
        byte[] ecKeyTemp = Encoding.UTF8.GetBytes(jwtEncryptionKey);
        byte[] scKey = Encoding.UTF8.GetBytes(jwtSignatureKey);

        byte[] ecKey = new byte[256 / 8];
        Array.Copy(ecKeyTemp, ecKey, 256 / 8);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", username), new Claim("password", password) }),
            Expires = DateTime.UtcNow + TimeSpan.FromSeconds(refreshTokenExpireTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(scKey), SecurityAlgorithms.HmacSha256Signature),
            EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(ecKey), SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.Aes256CbcHmacSha512),
            Issuer = "TumbleBackend",
            Audience = "TumbleApp",
        };

        return jwtHandler.CreateEncodedJwt(tokenDescriptor);
    }

    public RefreshTokenResponseModel? ValidateAndReadRefreshToken(string token)
    {
        if (token == null)
            return null;

        JwtSecurityTokenHandler tokenHandler = new();
        byte[] ecKeyTemp = Encoding.UTF8.GetBytes(jwtEncryptionKey);
        byte[] scKey = Encoding.UTF8.GetBytes(jwtSignatureKey);

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
