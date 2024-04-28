using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;
using MailKit.Net.Smtp;
using MediatR;
using MimeKit;

namespace KM_ClientApp.Endpoint.Email;

public record EmailHelpdesk(MailHelpdeskRequest Request) : INotification;

public class EmailHelpdeskHandler : INotificationHandler<EmailHelpdesk>
{
    private readonly IEmailRepository _emailRepository;

    public EmailHelpdeskHandler(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task Handle(EmailHelpdesk notification, CancellationToken cancellationToken)
    {
        var emailHelpdesk = new EmailHelpdeskFilter()
        {
            Session_Id = Guid.Parse(notification.Request.Session_Id),
            Content = notification.Request.Message,
            Send_By = notification.Request.Send_By
        };

        await _emailRepository.PostMailHelpdeskAsync(emailHelpdesk, cancellationToken);

        var emailTask = _emailRepository.GetEmailHelpdeskConfigAsync(cancellationToken);
        var formatTask = _emailRepository.GetEmalHelpdeskFormat(notification.Request.Send_By, cancellationToken);

        await Task.WhenAll(formatTask, emailTask);

        var emailConfig = await emailTask;
        var emailFormat = await formatTask;

        if (emailConfig != null && emailFormat != null)
        {
            var emailMessage = CreateEmailMessage(emailConfig, emailFormat, notification.Request.Message);

            using var client = new SmtpClient();
            await client.ConnectAsync(emailConfig.MAIL_CONFIG_SERVER, emailConfig.MAIL_CONFIG_PORT, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(emailConfig.MAIL_CONFIG_USERNAME, emailConfig.MAIL_CONFIG_PASSWORD, cancellationToken);
            await client.SendAsync(emailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }

    private static MimeMessage CreateEmailMessage(EmailHelpdeskConfig emailConfig, EmailHelpdeskFormat format, string userMessage)
    {
        var message = new MimeMessage();
        var mailboxFrom = new MailboxAddress(emailConfig.MAIL_HELPDESK_FROM, emailConfig.MAIL_CONFIG_USERNAME);
        var mailboxTo = new MailboxAddress(emailConfig.MAIL_HELPDESK_TO, emailConfig.MAIL_HELPDESK_TO);
        message.From.Add(mailboxFrom);
        message.To.Add(mailboxTo);
        message.Subject = emailConfig.MAIL_HELPDESK_SUBJECT;


        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@messageid"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@messageid", DateTime.Now.ToString("yyyyMMdd-HH"));
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@description"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@description", userMessage);
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@username"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@username", format.Login_Name);
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@fullname"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@fullname", format.Full_Name);
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@fullname"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@fullname", format.Full_Name);
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@jobtitle"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@jobtitle", format.Job_Title);
        }
        if (emailConfig.MAIL_HELPDESK_CONTENT_HTML.Contains("@location"))
        {
            emailConfig.MAIL_HELPDESK_CONTENT_HTML = emailConfig.MAIL_HELPDESK_CONTENT_HTML.Replace("@location", format.Office_Location);
        }

        var bodyEmail = new BodyBuilder();
        bodyEmail.HtmlBody = $@"
        <!DOCTYPE html>
        <html lang='en' xmlns:v='urn:schemas-microsoft-com:vml'>
        <head>
            <meta charset='utf-8'>
            <meta name='x-apple-disable-message-reformatting'>
            <meta name='viewport' content='width=device-width, initial-scale=1'>
            <meta name='format-detection' content='telephone=no, date=no, address=no, email=no, url=no'>
            <meta name='color-scheme' content='light dark'>
            <meta name='supported-color-schemes' content='light dark'>
            <!--[if mso]>
            <noscript>
            <xml>
                <o:OfficeDocumentSettings xmlns:o='urn:schemas-microsoft-com:office:office'>
                <o:PixelsPerInch>96</o:PixelsPerInch>
                </o:OfficeDocumentSettings>
            </xml>
            </noscript>
            <style>
            td,th,div,p,a,h1,h2,h3,h4,h5,h6 {{font-family: 'Segoe UI', sans-serif; mso-line-height-rule: exactly;}}
            </style>
            <![endif]-->
            <style>
            ol,
            ul {{
              margin: 0;
            }}
            h1,
            h2,
            h3,
            h4,
            h5 {{
              margin: 0;
              font-weight: normal;
            }}
            .visited-text-gray-500:visited {{
                color: #6b7280 !important
            }}
            @media (max-width: 600px) {{
                .sm-my-8 {{
                    margin-top: 32px !important;
                    margin-bottom: 32px !important
                }}
                .sm-px-4 {{
                    padding-left: 16px !important;
                    padding-right: 16px !important
                }}
                .sm-px-6 {{
                    padding-left: 24px !important;
                    padding-right: 24px !important
                }}
            }}</style>
        </head>
        <body style='margin: 0; width: 100%; background-color: #f8fafc; padding: 0; -webkit-font-smoothing: antialiased; word-break: break-word'>
            <div role='article' aria-roledescription='email' aria-label lang='en'>  <div class='sm-px-4' style='background-color: #f8fafc; font-family: ui-sans-serif, system-ui, -apple-system, 'Segoe UI', sans-serif'>
            <table align='center' cellpadding='0' cellspacing='0' role='none'>
                <tr>
                <td style='width: 900px; max-width: 100%'>
                    <div class='sm-my-8' style='margin-top: 48px; margin-bottom: 48px; text-align: center'>                  
                    &nbsp;
                    </div>
                    <table style='width: 100%;' cellpadding='0' cellspacing='0' role='none'>
                    <tr>
                        <td class='sm-px-6' style='border-radius: 4px; background-color: #fff; padding: 48px; font-size: 16px; color: #334155; box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05)'>
                        {emailConfig.MAIL_HELPDESK_CONTENT_HTML}
                        <div role='separator' style='background-color: #e2e8f0; height: 1px; line-height: 1px; margin: 32px 0'>&zwj;</div><p style='font-size: 14px; font-style: italic; line-height: 18px'>
                            This is automatic email, please do not reply
                        </p>
                        </td>
                    </tr>
                    <tr role='separator'>
                        <td style='line-height: 48px'>&zwj;</td>
                    </tr>
                    </table>
                </td>
                </tr>
            </table>
            </div>
            </div>
        </body>
        </html>";

        message.Body = bodyEmail.ToMessageBody();
        return message;
    }
}
