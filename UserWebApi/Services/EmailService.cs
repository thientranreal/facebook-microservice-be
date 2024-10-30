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
        
        public async Task SendConfirmationEmailAsync(string toEmail, string userName)
        {
            // Tạo nội dung email với liên kết xác nhận
            var confirmationLink = GenerateConfirmationLink(toEmail); // Hàm tạo link xác nhận, ví dụ như bạn đã thấy
            var subject = "Confirm Your Account";
            var body = $"Hello {userName},\n\nPlease confirm your account by clicking the following link: {confirmationLink}.\n\nIf you did not sign up, please ignore this email.";

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Facebook", "phakefacebook.404@gmail.com"));
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

// Phương thức tạo liên kết xác nhận (có thể thêm token hoặc các giá trị khác để bảo mật)
        private string GenerateConfirmationLink(string email)
        {
            var token = Guid.NewGuid().ToString(); // Tạo token ngẫu nhiên để xác thực
            // Bạn có thể lưu token này vào cơ sở dữ liệu để kiểm tra khi xác thực
            return $"https://yourwebsite.com/confirm-email?email={email}&token={token}";
        }
}

