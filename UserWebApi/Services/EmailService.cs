using MimeKit;

namespace UserWebApi.Services;

using System.Threading.Tasks;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com"; // Thay bằng server SMTP của bạn
    private readonly int _smtpPort = 587; // Cổng SMTP
    private readonly string _smtpUser = "letrongluc.cv@gmail.com"; // Email người gửi
    private readonly string _smtpPass = "Kensizima123"; // Mật khẩu email

    // public async Task SendEmailAsync(string toEmail, string subject, string body)
    // {
    //     var mailMessage = new MailMessage
    //     {
    //         From = new MailAddress(_smtpUser),
    //         Subject = subject,
    //         Body = body,
    //         IsBodyHtml = false,
    //     };
    //     mailMessage.To.Add(toEmail);
    //
    //     using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
    //     {
    //         smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
    //         smtpClient.EnableSsl = true;
    //         await smtpClient.SendMailAsync(mailMessage);
    //     }
    // }
    
    

 
    
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Your Password nè!", "phakefacebook.404@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                 await smtp.AuthenticateAsync("phakefacebook.404@gmail.com", "nnrq twmd mwrt lmqc"); // App password nếu dùng xác thực 2 yếu tố
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }

