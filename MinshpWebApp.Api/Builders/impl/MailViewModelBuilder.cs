using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MinshpWebApp.Api.Request;

namespace MinshpWebApp.Api.Builders.impl
{
    public class MailViewModelBuilder : IMailViewModelBuilder
    {
        //folders
        const string paymentFolder = "Payments";
        const string registrationFolder = "Registrations";



        //templates names
        const string paymentFile = "payment.html";
        const string registrationFile = "registration.html";


        //subject
        private string paymentSubject = "Confirmation de votre commande";
        private string registrationSubject = "Confirmation d'enregistrement";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public async Task<string> SendMailRegistration(string mail)
        {
            return await SendMail(mail, registrationFolder, registrationFile, registrationSubject);
        }


        public async Task<string> SendMailPayment(string mail)
        {
            paymentSubject = paymentSubject;
            return await SendMail(mail, paymentFolder, paymentFile, paymentSubject);
        }

        public async Task<string> SendMailGroupRegistration(List<string> recipients)
        {
            foreach (var r in recipients)
            {
                await SendMail(r, registrationFolder, registrationFile, registrationSubject);
            }

            return "L'envoie des mails en masse s'est bien passé !";
        }



        private async Task<string> SendMail(string recipient, string folder, string fileName, string subject)
        {
            string basePath = AppContext.BaseDirectory;
            string filePath = Path.Combine(basePath, "Templates", folder, fileName);
            string htmlContent = await File.ReadAllTextAsync(filePath);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Min's shop", "contact@minshp.com"));
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject.ToUpper();

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };

            message.Body = bodyBuilder.ToMessageBody();

            try
            {

                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync("smtp.hostinger.com", 587, false);
                await client.AuthenticateAsync("contact@minshp.com","Cdjeneba19882025shp@");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return "Le mail à bien été envoyé";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.Message;
            }

        }
    }
}
