using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TumbleBackend.InternalModels;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.StringConstants;

namespace TumbleBackend.Utilities
{
    public class JwtUtil
    {
        readonly string _jwtEncryptionKey;
        readonly string _jwtSignatureKey;
        readonly int _refreshTokenExpireTime;
        readonly string _testToken;
        readonly string _testPassword;
        readonly string _testUsername;

        public JwtUtil([FromServices] IConfiguration configuration)
        {
            string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
            string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
            string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
            string? configuredTestToken = configuration[UserSecrets.TestUserSessionToken] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserSessionToken);
            string? testUserPass = configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);
            string? testUserEmail = configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);

            if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
                throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

            _jwtEncryptionKey = jwtEncKey;
            _jwtSignatureKey = jwtSigKey;
            _refreshTokenExpireTime = int.Parse(refreshTokenExpiration);
            _testToken = configuredTestToken ?? throw new NullReferenceException("Test token not configured");
            _testPassword = testUserPass ?? throw new NullReferenceException("Test user password not configured");
            _testUsername = testUserEmail ?? throw new NullReferenceException("Test user email not configured");
        }

        public string GenerateRefreshToken(string username, string password)
        {
            JwtSecurityTokenHandler jwtHandler = new();
            byte[] ecKeyTemp = Encoding.UTF8.GetBytes(_jwtEncryptionKey);
            byte[] scKey = Encoding.UTF8.GetBytes(_jwtSignatureKey);

            byte[] ecKey = new byte[256 / 8];
            Array.Copy(ecKeyTemp, ecKey, 256 / 8);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[] { new Claim("username", username), new Claim("password", password) }),
                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(_refreshTokenExpireTime),
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

            // Check if the token is a test token (allows reuse and bypasses expiration/signature checks)
            if (token == _testToken)
                return new RefreshTokenResponseModel(_testUsername, _testPassword);

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] ecKeyTemp = Encoding.UTF8.GetBytes(_jwtEncryptionKey);
            byte[] scKey = Encoding.UTF8.GetBytes(_jwtSignatureKey);

            byte[] ecKey = new byte[256 / 8];
            Array.Copy(ecKeyTemp, ecKey, 256 / 8);

            try
            {
                // Normal token validation
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

                return new RefreshTokenResponseModel(username, password);
            }
            catch
            {
                return null;
            }
        }
    }
}
