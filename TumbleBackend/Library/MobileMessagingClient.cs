using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace TumbleBackend.Library;

public class MobileMessagingClient
{
    private readonly FirebaseMessaging messaging;

    public MobileMessagingClient()
    {
        var app = FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile("serviceAccountKey.json").CreateScoped("https://www.googleapis.com/auth/firebase.messaging") });
        messaging = FirebaseMessaging.GetMessaging(app);

    }

    private static Message CreateNotification(string topic, string title, string notificationBody)
    {
        return new Message()
        {
            Topic = topic,
            Notification = new Notification()
            {
                Body = notificationBody,
                Title = title
            }
        };
    }

    public async Task<string> SendNotification(string topic, string title, string body)
    {
        return await messaging.SendAsync(CreateNotification(topic, title, body));
    }
}
