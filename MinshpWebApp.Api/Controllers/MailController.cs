using AutoMapper.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using OpenIddict.Validation.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("mail")]
    public class MailController : Controller
    {
        IMailViewModelBuilder _mailViewModelBuilder;

        public MailController(IMailViewModelBuilder mailViewModelBuilder)
        {
            _mailViewModelBuilder = mailViewModelBuilder ?? throw new ArgumentNullException(nameof(mailViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpPost("/send/registration")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Mail pour l'engistrement")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> SendRegistrationMailAsync([FromBody] MailRequest request)
        {
            var result = await _mailViewModelBuilder.SendMailRegistration(request);

            if (result == null)
                return null;
            else
                return Ok("mail envoyé");
        }


        [HttpPost("/send/payment")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Mail pour le Paiement")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> SendPaymentMailAsync([FromBody] MailRequest request)
        {
            var result = await _mailViewModelBuilder.SendMailPayment(request.Customer.Email, request.Items, null,null,null, null, 0, 0);

            if (result == null)
                return null;
            else
                return Ok("mail envoyé");
        }


        [HttpPost("password-reset")]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendPasswordReset([FromBody] PasswordResetRequest req)
        {
            // À implémenter dans le builder si pas déjà fait :
            //  -> lit Templates/Passwords/reset.html et remplace {{RESET_LINK}}
            var r = await _mailViewModelBuilder.SendMailPasswordReset(req.To, req.ResetLink);
            return Ok(new { result = r });
        }

        //[HttpPost("/send/seminaire/group")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "seminaire envoie de masse")]
        //[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        //public async Task<IActionResult> SendSeminaireMailGroupAsync([FromBody] MailGroupRequest recipients)
        //{

        //    var result = await _mailViewModelBuilder.SendMailGroupSeminaire(recipients);
        //    return Ok(result);
        //}





        //[HttpPost("/send/payment/Group")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Paiement envoie de masse")]
        //[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        //public async Task<IActionResult> SendPaymentMailGroupAsync([FromBody] string recipients)
        //{

        //    var result = await _mailViewModelBuilder.SendMailGroupPayment(recipients);
        //    return Ok(result);
        //}




        //[HttpPost("/send/registration/group")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Engistrement pour entretien")]
        //[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        //public async Task<IActionResult> SendRegistrationMailGroupAsync()
        //{

        //    return Ok();
        //}
    }
}
