using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

public class EmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string resetLink)
    {
        string emailBody = await LoadEmailTemplateAsync(resetLink);

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = emailBody };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:SmtpUsername"], _config["EmailSettings:SmtpPassword"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }

    private async Task<string> LoadEmailTemplateAsync(string resetLink)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", "ResetPassword.cshtml");
        string templateContent = await File.ReadAllTextAsync(templatePath);
        return templateContent.Replace("{RESET_LINK}", resetLink);
    }
}
