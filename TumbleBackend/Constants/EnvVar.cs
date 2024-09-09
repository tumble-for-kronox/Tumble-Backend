namespace TumbleBackend.Constants;

public static class EnvVar
{
    public static string JwtEncryptionKey => "JwtEncKey";
    public static string JwtSignatureKey => "JwtSigKey";
    public static string JwtRefreshTokenExpiration => "JwtExpiration";
    public static string DbConnection => "DbConnectionString";
    public static string AwsAccessKey => "AwsAccessKey";
    public static string AwsSecretKey => "AwsSecretKey";
    public static string AdminPass => "AdminPass";

    public static string LokiUri => "LokiUri";
}
