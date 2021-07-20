﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace gbajax.Service
{
    public class MailService
    {

        private string gmail_account = "";
        private string gmail_password = "";
        private string gmail_mail = "@gmail.com";


        public string GetValidatcode()
        {
            string[] Code = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string ValidateCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i<10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }

            return ValidateCode;
        }

        public string GetRegisterEmailBody(string TempString, string UserName, string ValidateUrl)
        {
            TempString = TempString.Replace("{UserName}", UserName);
            TempString = TempString.Replace("{ValidateUrl}", ValidateUrl);
            return TempString;
        }

        public void SendRegisterMail(string MailBody, string ToEamil)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEamil);
            mail.Subject = "會員註冊確認信";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);

        }



    
    }
}