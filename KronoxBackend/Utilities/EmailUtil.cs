using Amazon.SimpleEmailV2;
using Amazon.Runtime;
using Amazon;
using Amazon.SimpleEmailV2.Model;
using System.Net;

namespace TumbleBackend.Utilities;

public class EmailUtil
{
    private static string? access;
    private static string? secret;

    public static void Init(string accessKey, string secretKey)
    {
        access = accessKey;
        secret = secretKey;
    }

    public async static Task<HttpStatusCode> SendNewIssueEmail(string issueTitle, string issueDesc)
    {
        AmazonSimpleEmailServiceV2Client emailClient = new(new BasicAWSCredentials(access, secret), RegionEndpoint.EUNorth1);

        string data = $@"{{
            ""issueTitle"": ""{issueTitle}"",
            ""issueDesc"": ""{issueDesc}""
        }}";

        SendEmailRequest sendRequest = new SendEmailRequest()
        {
            FromEmailAddress = "tumblestudios.app@gmail.com",
            Destination = new Destination()
            {
                ToAddresses = new List<string>()
                {
                    "tumblestudios.app@gmail.com"
                }
            },
            Content = new EmailContent()
            {
                Template = new Template()
                {
                    TemplateName = "issue",
                    TemplateData = data
                }
            }
        };

        try
        {
            SendEmailResponse response = await emailClient.SendEmailAsync(sendRequest);
            return response.HttpStatusCode;
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }


    }
}
