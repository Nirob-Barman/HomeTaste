using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HomeTaste.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }


        // Private method to send the email (common setup logic)
        private async Task SendEmailAsyncCore(MailMessage mailMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                    throw new Exception("SenderEmail is not configured.");

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }



        public async Task SendEmailAsync(string subject, string message, List<string> toEmails, 
            List<string>? ccEmails = null,
            List<string>? bccEmails = null,
            List<Attachment>? attachments = null,
            bool isHtml = true,
            Dictionary<string, string>? templateVariables = null)
        {
            try
            {
                // If template variables are provided, replace them in the message
                if (templateVariables != null)
                {
                    foreach (var variable in templateVariables)
                    {
                        message = message.Replace($"{{{{{variable.Key}}}}}", variable.Value);
                    }
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = isHtml
                };
                
                // Add all recipients from the list
                foreach (var toEmail in toEmails)
                {
                    mailMessage.To.Add(toEmail);
                }

                // Add CC if provided
                if (ccEmails != null)
                {
                    foreach (var cc in ccEmails)
                    {
                        mailMessage.CC.Add(cc);
                    }
                }

                // Add BCC if provided
                if (bccEmails != null)
                {
                    foreach (var bcc in bccEmails)
                    {
                        mailMessage.Bcc.Add(bcc);
                    }
                }

                // Add attachments if provided
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                // Send the email using the common method
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            }
        }




        //public async Task SendEmailAsync(string toEmail, string subject, string message)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
        //            throw new Exception("SenderEmail is not configured.");

        //        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        //        {
        //            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
        //            EnableSsl = _emailSettings.EnableSsl
        //        };

        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
        //            Subject = subject,
        //            Body = message,
        //            IsBodyHtml = true
        //        };

        //        mailMessage.To.Add(toEmail);
        //        await smtpClient.SendMailAsync(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Failed to send email", ex);
        //    }
        //}


        // Send basic email (standard)
        //public async Task SendEmailAsync(string toEmail, string subject, string message)
        //{
        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
        //        Subject = subject,
        //        Body = message,
        //        IsBodyHtml = true
        //    };
        //    mailMessage.To.Add(toEmail);
        //    await SendEmailAsyncCore(mailMessage);
        //}


        //Send Email with Attachments
        public async Task SendEmailWithAttachmentsAsync(string toEmail, string subject, string message, List<Attachment> attachments)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // Attach each file
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }

                //await smtpClient.SendMailAsync(mailMessage);
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        //Send Plain Text Email
        public async Task SendPlainTextEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false // Plain text
                };

                mailMessage.To.Add(toEmail);
                //await smtpClient.SendMailAsync(mailMessage);
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                //throw new Exception("Failed to send plain text email", ex);
                throw new Exception(ex.Message);
            }
        }


        //Send Email to Multiple Recipients
        public async Task SendEmailToMultipleRecipientsAsync(List<string> toEmails, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                foreach (var toEmail in toEmails)
                {
                    mailMessage.To.Add(toEmail);
                }

                //await smtpClient.SendMailAsync(mailMessage);
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                //throw new Exception("Failed to send email to multiple recipients", ex);
                throw new Exception(ex.Message);
            }
        }


        //Send Email with CC/BCC
        public async Task SendEmailWithCcBccAsync(string toEmail, string subject, string message, List<string>? ccEmails = null, List<string>? bccEmails = null)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                // Add CC
                if (ccEmails != null)
                {
                    foreach (var cc in ccEmails)
                    {
                        mailMessage.CC.Add(cc);
                    }
                }

                // Add BCC
                if (bccEmails != null)
                {
                    foreach (var bcc in bccEmails)
                    {
                        mailMessage.Bcc.Add(bcc);
                    }
                }

                //await smtpClient.SendMailAsync(mailMessage);
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                //throw new Exception("Failed to send email with CC/BCC", ex);
                throw new Exception(ex.Message);
            }
        }


        //Send Email with HTML Template
        public async Task SendEmailWithTemplateAsync(string toEmail, string subject, string template, Dictionary<string, string> templateVariables)
        {
            try
            {                
                // Replace placeholders in the template
                foreach (var variable in templateVariables)
                {
                    template = template.Replace($"{{{{{variable.Key}}}}}", variable.Value);
                }                

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail!, _emailSettings.SenderName),
                    Subject = subject,
                    Body = template,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                //await smtpClient.SendMailAsync(mailMessage);
                await SendEmailAsyncCore(mailMessage);
            }
            catch (Exception ex)
            {
                //throw new Exception("Failed to send email with template", ex);
                throw new Exception(ex.Message);
            }
        }


    }
}
