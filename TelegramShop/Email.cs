using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramShop
{
    public struct SendMailInfo
    {
        public string username;
        public string password;
        public string toEmail;
        public string subject;
        public string body;
        public bool isBodyHtml;
        public string fileName;
    }
    public class Email
    {
        SendMailInfo sendMailInfo;

        //"asfdsafasd324@mailinator.com", "TEMA", "BODY"

        public Email(string username, string password, string toEmail, string subject, string body, bool isBodyHtml = false, string fileName = "")
        {
            sendMailInfo = new SendMailInfo();
            sendMailInfo.username = username;
            sendMailInfo.password = password;
            sendMailInfo.toEmail = toEmail;
            sendMailInfo.subject = subject;
            sendMailInfo.body = body;
            sendMailInfo.isBodyHtml = isBodyHtml;
            sendMailInfo.fileName = fileName;
        }

        public void Send()
        {
            try
            {
                Thread thread = new Thread(SendEmail) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void SendEmail()
        {
            //Smpt Client Details
            //gmail >> smtp server : smtp.gmail.com, port : 587 , ssl required
            //yahoo >> smtp server : smtp.mail.yahoo.com, port : 587 , ssl required
            SmtpClient clientDetails = new SmtpClient();
            clientDetails.Port = 587;
            clientDetails.Host = "smtp.gmail.com";
            clientDetails.EnableSsl = true;
            clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
            clientDetails.UseDefaultCredentials = false;
            clientDetails.Credentials = new NetworkCredential(sendMailInfo.username, sendMailInfo.password);

            //Message Details
            MailMessage mailDetails = new MailMessage();
            mailDetails.From = new MailAddress(sendMailInfo.username);
            mailDetails.To.Add(sendMailInfo.toEmail);
            mailDetails.Subject = sendMailInfo.subject;
            mailDetails.IsBodyHtml = sendMailInfo.isBodyHtml;
            mailDetails.Body = sendMailInfo.body;


            //file attachment
            if (sendMailInfo.fileName.Length > 0)
            {
                Attachment attachment = new Attachment(sendMailInfo.fileName);
                mailDetails.Attachments.Add(attachment);
            }


            // !!!
            //clientDetails.Send(mailDetails);



            //MessageBox.Show("Your mail has been sent.");
            sendMailInfo.fileName = "";
        }
    }
}
