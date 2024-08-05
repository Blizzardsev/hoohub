using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using hoohub.Configuration;

namespace hoohub.Services
{
    /// <summary>
    /// Handles SMTP activity, such as sending emails.
    /// </summary>
    public class SmtpService
    {
        /// <summary>
        /// The <see cref="SmtpSettings"/> from the configuration file to use for sending messages.
        /// </summary>
        private readonly SmtpSettings _smtpSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="SmtpService"/> class.
        /// </summary>
        /// <param name="smtpSettings"></param>
        public SmtpService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        /// <summary>
        /// Sends an email to a given target address, with a specified subject and body.
        /// If emails are disabled in the <see cref="AppSettings"/>, then nothing will happen.
        /// </summary>
        /// <param name="name">The name to address the receiver by.</param>
        /// <param name="address">The address to send the email message to.</param>
        /// <param name="subject">The subject of the email message.</param>
        /// <param name="body">The body of the email message.</param>
        public void SendEmail(
            string name,
            string address,
            string subject,
            string body)
        {
            if (!_smtpSettings.EnableEmails)
            {
                return;
            }
            using (var _smtpClient = new SmtpClient())
            {
                _smtpClient.Connect(
                    host: _smtpSettings.Host,
                    port: _smtpSettings.Port,
                    useSsl: _smtpSettings.EnableSsl);
                _smtpClient.Authenticate(new NetworkCredential(userName: _smtpSettings.Username, password: _smtpSettings.Password, domain: _smtpSettings.Domain));

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(name: _smtpSettings.SenderName, address: _smtpSettings.SenderAddress));
                message.To.Add(new MailboxAddress(name: name, address: address));
                message.Subject = $"{subject}";

                message.Body = new BodyBuilder()
                {
                    HtmlBody = body
                }.ToMessageBody();

                _smtpClient.Send(message);
                _smtpClient.Disconnect(true);
            }
        }

        /// <summary>
        /// Releases resources and destroys this class.
        /// </summary>
        public void Dispose()
        {
            Dispose();
        }
    }
}