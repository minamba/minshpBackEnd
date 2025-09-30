using MailKit.Net.Smtp;
using MimeKit;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using Stripe;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MinshpWebApp.Api.Builders.impl
{
    public class MailViewModelBuilder : IMailViewModelBuilder
    {
        //folders
        const string paymentFolder = "Payments";
        const string registrationFolder = "Registrations";
        const string resetFolder = "ForgotPassword";



        //templates names
        const string paymentFile = "payment.html";
        const string registrationFile = "registration.html";
        const string resetFile = "reset.html";


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
            return await SendMail(mail, registrationFolder, registrationFile, registrationSubject, null, null ,null, null, null, null, null);
        }


        public async Task<string> SendMailPayment(string mail, List<Utils.InvoiceItem> items, CustomerViewModel customer, BillingAddress billing, DeliveryAddress delivery, Order order, decimal tva, decimal totalTaxes)
        {
            paymentSubject = paymentSubject;
            return await SendMail(mail, paymentFolder, paymentFile, paymentSubject, items, customer,billing,delivery, order, tva, totalTaxes);
        }

        public async Task<string> SendMailGroupRegistration(List<string> recipients)
        {
            foreach (var r in recipients)
            {
                await SendMail(r, registrationFolder, registrationFile, registrationSubject, null, null, null, null, null,null, null);
            }

            return "L'envoie des mails en masse s'est bien passé !";
        }



        //Cette methode sert pour tous les envois de mail sauf pour le reset password (mot de passe oublié) qui a sa propre méthode
        private async Task<string> SendMail(string recipient, string folder, string fileName, string subject, List<Utils.InvoiceItem> items, CustomerViewModel customer, BillingAddress billing, DeliveryAddress delivery, Order order, decimal? tva, decimal? totalTaxes)
        {
            string basePath = AppContext.BaseDirectory;
            string filePath = Path.Combine(basePath, "Templates", folder, fileName);
            string htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
            decimal subTotal = 0;

            //POUR LA COMMANDE (les lignes produits) *************************************
            // culture pour le format monétaire (ex: "12,34 €")
            if (items != null)
            {
                var culture = CultureInfo.GetCultureInfo("fr-FR");

                // 1) Template d'une ligne
                var rowTpl = @"
                        <tr>
                          <td>
                            <div style=""font-weight:bold"">{{ITEM_NAME}}</div>
                          </td>
                          <td>{{ITEM_QTY}}</td>
                          <td>{{ITEM_REMISE}}</td>
                          <td class=""price"">{{ITEM_TOTAL}}</td>
                        </tr>";

                // 2) Construire toutes les lignes
                var rows = new StringBuilder();
                foreach (var line in items)
                {
                    // Sécuriser le HTML
                    var name = WebUtility.HtmlEncode(line.Description ?? "");
                    var qty = line.Quantity;
                    var remise = line.Reduction;
                    var total = line.TotalPriceItemProduct.ToString("C", culture);
                    subTotal += line.TotalPriceItemProduct;

                    rows.AppendLine(
                        rowTpl
                            .Replace("{{ITEM_NAME}}", name)
                            .Replace("{{ITEM_QTY}}", qty.ToString())
                            .Replace("{{ITEM_REMISE}}", remise.ToString())
                            .Replace("{{ITEM_TOTAL}}", total)
                    );
                }

                subTotal = Math.Round(subTotal, 2);

                htmlContent = htmlContent.Replace("{{ORDER_LINES}}", rows.ToString());
                htmlContent = htmlContent.Replace("{{CUSTOMER_FIRSTNAME}}", customer.FirstName);
                htmlContent = htmlContent.Replace("{{CUSTOMER_LASTNAME}}", customer.LastName);
                htmlContent = htmlContent.Replace("{{ORDER_NUMBER}}", order.OrderNumber.ToString());
                htmlContent = htmlContent.Replace("{{ORDER_DATE}}", order.Date?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "");
                htmlContent = htmlContent.Replace("{{SUBTOTAL}}", subTotal.ToString());
                htmlContent = htmlContent.Replace("{{ECO_PARTICIPATION}}", totalTaxes.ToString());
                htmlContent = htmlContent.Replace("{{TAX_TOTAL}}", tva.ToString());
                htmlContent = htmlContent.Replace("{{ORDER_TOTAL}}", order.Amount.ToString());

                htmlContent = htmlContent.Replace("{{SHIP_NAME}}", delivery.FirstName + " " + delivery.LastName);
                htmlContent = htmlContent.Replace("{{SHIP_ADDRESS1}}", delivery.Address);
                htmlContent = htmlContent.Replace("{{SHIP_ZIP}}", delivery.PostalCode.ToString());
                htmlContent = htmlContent.Replace("{{SHIP_CITY}}", delivery.City);
                htmlContent = htmlContent.Replace("{{SHIP_COUNTRY}}", delivery.Country);

                htmlContent = htmlContent.Replace("{{BILL_NAME}}", customer.FirstName + " " + customer.LastName);
                htmlContent = htmlContent.Replace("{{BILL_ADDRESS1}}", billing.Address);
                htmlContent = htmlContent.Replace("{{BILL_ZIP}}", billing.PostalCode.ToString());
                htmlContent = htmlContent.Replace("{{BILL_CITY}}", billing.City);
                htmlContent = htmlContent.Replace("{{BILL_COUNTRY}}", billing.Country);

                htmlContent = htmlContent.Replace("{{ORDER_LINK}}", "http://localhost:3000/account");
                htmlContent = htmlContent.Replace("{{SUPPORT_EMAIL}}", "noreply@minshp.com");
            }
            //POUR LA COMMANDE **********************************************************


            //POUR LA REGISTRATION ******************************************************
            htmlContent = htmlContent.Replace("{{CURRENT_YEAR}}", DateTime.Now.Year.ToString());
            htmlContent = htmlContent.Replace("{{BASE_URL}}", "https://i.imgur.com/ub3X3gK.png");
            htmlContent = htmlContent.Replace("{{LOGIN_LINK}}", "http://localhost:3000/login");
            htmlContent = htmlContent.Replace("{{FIRSTNAME}}", "Minamba");
            htmlContent = htmlContent.Replace("{{LASTNAME}}", "Camara");
            //POUR LA REGISTRATION ******************************************************

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Min's shop", "noreply@minshp.com"));
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
                await client.AuthenticateAsync("noreply@minshp.com", "Cdjeneba19882025shp@");
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


        public async Task<string> SendMailPasswordReset(string recipient, string resetLink)
        {
            string basePath = AppContext.BaseDirectory;
            string filePath = Path.Combine(basePath, "Templates", resetFolder, resetFile);
            string htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
            htmlContent = htmlContent.Replace("{{RESET_LINK}}", resetLink);
            htmlContent = htmlContent.Replace("{{CURRENT_YEAR}}", DateTime.Now.Year.ToString());
            htmlContent = htmlContent.Replace("{{BASE_URL}}", "https://i.imgur.com/ub3X3gK.png");


            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Min's shop", "contact@minshp.com"));
            msg.To.Add(new MailboxAddress("", recipient));
            msg.Subject = "Réinitialisation de votre mot de passe";

            var body = new BodyBuilder { HtmlBody = htmlContent };
            msg.Body = body.ToMessageBody();

            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync("smtp.hostinger.com", 587, false);
                await client.AuthenticateAsync("noreply@minshp.com", "Cdjeneba19882025shp@");
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
                return "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}
