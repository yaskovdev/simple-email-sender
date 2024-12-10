// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleEmailSender;

internal static class Program
{
    public static void Main(string[] args)
    {
        var jsonFilePath = GetResourcePath("Resources/Config.json");
        var body = File.ReadAllText(GetResourcePath("Resources/EmailBody.html"));
        var subject = "";

        if (string.IsNullOrEmpty(subject))
        {
            throw new Exception("Subject is empty");
        }

        if (body.Contains("intro-to-genetic-programming-can-evolution-write-computer-programs"))
        {
            throw new Exception("Body has not been updated"); // TODO: improve validation
        }

        var jsonString = File.ReadAllText(jsonFilePath);

        var config = JsonSerializer.Deserialize<Config>(jsonString, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        });

        using var smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;
        smtp.EnableSsl = true;
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(config.Sender, config.SenderPassword);

        Console.WriteLine($"Sending to {config.TestRecipients.Count} test recipients...");
        foreach (var recipient in config.TestRecipients)
        {
            Console.WriteLine($"Sending email to test recipient {recipient}");
            using var message = CreatEmailMessage(config.Sender, recipient, subject, body);
            smtp.Send(message);
        }

        Console.WriteLine($"Sending to {config.Recipients.Count} real recipients...");
        foreach (var recipient in config.Recipients)
        {
            Console.WriteLine($"Sending email to real recipient {recipient}, confirm by pressing any key");
            Console.ReadKey();
            using var message = CreatEmailMessage(config.Sender, recipient, subject, body);
            smtp.Send(message);
        }
    }

    private static MailMessage CreatEmailMessage(string sender, string recipient, string subject, string body) =>
        new(sender, recipient)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

    private static string GetResourcePath(string name)
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(location);
        return Path.Combine(directory ?? "", name);
    }
}