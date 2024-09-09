namespace TumbleBackend.Constants;

public static class UserSecrets
{
    public static string JwtEncryptionKey => "Jwt:EncKey";
    public static string JwtSignatureKey => "Jwt:SigKey";
    public static string JwtRefreshTokenExpiration => "Jwt:Expiration";
    public static string DbConnection => "DbConnectionString";
    public static string AwsAccessKey => "Aws:AccessKey";
    public static string AwsSecretKey => "Aws:SecretKey";
    public static string AdminPass => "Admin:Pass";
}
