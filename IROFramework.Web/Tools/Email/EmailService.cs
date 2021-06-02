using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace IROFramework.Web.Tools.Email
{
    public class EmailService:IDisposable, IEmailService
    {
        readonly EmailOptions _opt;
        SmtpClient _client;
        bool _isDisposed;

        public EmailService(EmailOptions opt)
        {
            _opt = opt;
        }

        public async Task SendEmailAsync(string email, string subject, string text)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Администрация сайта", "login@yandex.ru"));
            mimeMessage.To.Add(new MailboxAddress("", email));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = text
            };

            await SendEmailAsync(mimeMessage);
        }

        async Task SendEmailAsync(MimeMessage mimeMessage)
        {
            ThrowIfDisposed();
            await LazyInit();
            await _client.SendAsync(mimeMessage);
        }

        async Task LazyInit()
        {
            if (_client == null)
            {
                _client = new SmtpClient();
                await _client.ConnectAsync(_opt.Server, _opt.Port, true);
                await _client.AuthenticateAsync(_opt.Login, _opt.Password);
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _client?.Dispose();
        }

        void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EmailService));
        }
    }
}
