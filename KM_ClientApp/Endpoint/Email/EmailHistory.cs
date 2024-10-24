﻿using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;
using MailKit.Net.Smtp;
using MediatR;
using MimeKit;

namespace KM_ClientApp.Endpoint.Email;

public record EmailHistory(string LoginName, string SessionId, EmailHistoryRequest MailHistory) : INotification;

public class EmailHistoryHandler : INotificationHandler<EmailHistory>
{
    private readonly IEmailRepository _emailRepository;

    public EmailHistoryHandler(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task Handle(EmailHistory notification, CancellationToken cancellationToken)
    {
        var emailLog = new EmailLog();

        try
        {
            var emaiLTask = _emailRepository.GetMailConfigAsync(cancellationToken);
            var recepientTask = _emailRepository.GetMailRecepientAsync(notification.LoginName, cancellationToken);
            var userFeedbackTask = _emailRepository.GetUserFeedback(notification.SessionId, cancellationToken);

            await Task.WhenAll(emaiLTask, recepientTask, userFeedbackTask);

            var emailConfig = await emaiLTask;
            var recepient = await recepientTask;
            var userFeedback = await userFeedbackTask;

            if (emailConfig == null && recepient == null && !emailConfig!.MAIL_HISTORY_STATUS)
            {
                throw new Exception("Mail configuration invalid or disabled by system");
            }

            var emailMessage = CreateEmailMessage(emailConfig!, recepient!, notification.MailHistory, userFeedback!);

            emailLog.To = recepient!.Email;
            emailLog.Subject = emailMessage.Subject;
            emailLog.Body = emailMessage.HtmlBody.ToString().Trim();

            using var client = new SmtpClient();
            await client.ConnectAsync(emailConfig!.MAIL_CONFIG_SERVER, emailConfig.MAIL_CONFIG_PORT, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(emailConfig.MAIL_CONFIG_USERNAME, emailConfig.MAIL_CONFIG_PASSWORD, cancellationToken);
            await client.SendAsync(emailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            emailLog.SendStatus = "Failed";
            emailLog.ErrorMsg = ex.Message;
        }

        await _emailRepository.PostMailLogAsync(emailLog, cancellationToken);
    }

    private static string GetStarRating(int rating)
    {
        // Menghasilkan bintang berdasarkan rating
        string stars = string.Empty;
        for (int i = 0; i < rating; i++)
        {
            // Bintang berwarna kuning 
            stars += "<span style='color: #FFD700; font-size: 35px;'>&#9733;</span>";
        }
        for (int i = rating; i < 4; i++)
        {
            // Bintang kosong 
            stars += "<span style='color: #FFD700; font-size: 35px;'>&#9734;</span>";
        }
        return stars;
    }

    private static MimeMessage CreateEmailMessage(EmailHistoryConfig emailConfig, EmailHistoryRecipient recipient, EmailHistoryRequest history, UserFeedback userFeedback)
    {
        var message = new MimeMessage();
        var mailboxFrom = new MailboxAddress(emailConfig.MAIL_HISTORY_FROM, emailConfig.MAIL_CONFIG_USERNAME);
        var mailboxTo = new MailboxAddress(recipient.Full_Name, recipient.Email);

        message.From.Add(mailboxFrom);
        message.To.Add(mailboxTo);
        message.Subject = emailConfig.MAIL_HISTORY_SUBJECT;

        var headerEmail = string.Empty;
        var bodyEmail = new BodyBuilder();
        var bodyHistories = string.Empty;
        var bodyFeedback = string.Empty;


        headerEmail = $@"
         <p>
         <span style='overflow-wrap: break-word; text-indent: 20px;color:#888888;'>Subject   : </span>
         <span style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color:#707070;'>{emailConfig.MAIL_HISTORY_FROM}</span>
         </p>
         <p>
         <span style='overflow-wrap: break-word; text-indent: 20px;color:#888888;'>To              : </span>
         <span style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #16a34a'>{recipient.Email}</span>
         </p>
         ";

        bodyFeedback = $@"
        
        <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #707070; padding-top:10px; padding-bottom:10px;'>Rating     : </p>
        <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;padding-bottom:20px;'>{GetStarRating(userFeedback.Rating)}</p>
        
        <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #707070; padding-top:10px; padding-bottom:10px;'>Feedback   : </p>
        <p style='margin: 0; overflow-wrap: break-word; line-height: 24px;color:#888888; padding-bottom:20px;'>{userFeedback.Remark}</p>

        <div role='separator' style='line-height: 10px'>&zwj;</div>";



        foreach (var item in history.Histories)
        {
            var createAt = DateTime.Parse(item.Time);

            if (item.Actor == "bot")
            {
                bodyHistories += $@"
                <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #707070;'>{emailConfig.MAIL_HISTORY_FROM}, {createAt.ToString("HH:mm")} :</p>
                <p style='overflow-wrap: break-word; text-indent: 20px; color:#888888;'>{item.Message}</p>
                <div role='separator' style='line-height: 10px'>&zwj;</div>";
                continue;
            }

            if (item.Actor == "content")
            {
                bodyHistories += $@"
                <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #707070;'>{emailConfig.MAIL_HISTORY_FROM}, {createAt.ToString("HH:mm")} :</p>
                <p style='overflow-wrap: break-word; text-indent: 20px; color:#888888;'>{item.Message}</p>
                <div role='separator' style='line-height: 10px'>&zwj;</div>
                <div style='overflow-wrap: break-word; font-size: 14px'>
                    {item.Content}
                    <div role='separator' style='line-height: 10px'>&zwj;</div>
                    <a class='visited-text-gray-500' href='{item.Link}' target='_blank' style='color: #16a34a'>
                        <i>Read More...</i>
                    </a>
                    <div role='separator' style='line-height: 14px'>&zwj;</div>
                </div>";
                continue;
            }

            if (item.Actor == "user")
            {
                bodyHistories += $@"
                <p style='margin: 0; overflow-wrap: break-word; font-weight: 600; line-height: 24px;color: #707070'>{recipient.Full_Name}, {createAt.ToString("HH:mm")} :</p>    
                <p style='overflow-wrap: break-word; text-indent: 20px; color:#888888;'>{item.Message}</p>
                <div role='separator' style='line-height: 10px'>&zwj;</div>";
                continue;
            }
        }

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
                   
                    <table style='width: 100%;' cellpadding='0' cellspacing='0' role='none'>
                     <tr>
                        <td class='sm-px-6' style='border-radius: 4px; background-color: #fff; padding-left: 48px ; padding-right:48px; padding-bottom:10px; padding-top:10px; font-size: 16px; color: #334155; box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05)'>
                        {headerEmail}
                        <div role='separator' style='background-color: #e2e8f0; height: 1px; line-height: 1px; margin: 32px 0'>&zwj;</div>
                        </td>
                    </tr>
                    <tr>
                        <td class='sm-px-6' style='border-radius: 4px; background-color: #fff; padding-left: 48px ; padding-right:48px; padding-bottom:10px; padding-top:10px; font-size: 16px; color: #334155; box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05)'>
                        {bodyHistories}
                        <div role='separator' style='background-color: #e2e8f0; height: 1px; line-height: 1px; margin: 32px 0'>&zwj;</div>
                        {bodyFeedback}
                        <p style='font-size: 14px; font-style: italic; line-height: 18px'>
                            This is automatic email from your {emailConfig.MAIL_HISTORY_FROM},
                            <br>
                            please do not reply
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