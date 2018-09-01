using System;
using System.Configuration;
using Esmart.Permission.Application.Properties;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Services
{
    public class MailService
    {
        public MailService()
        {
            //WebMail.SmtpServer = ConfigurationManager.AppSettings["smtpHost"];
            //WebMail.SmtpPort = int.Parse(ConfigurationManager.AppSettings["smptPort"]);
            //WebMail.UserName = ConfigurationManager.AppSettings["smtpCredentialAccount"];
            //WebMail.Password = ConfigurationManager.AppSettings["smtpCredentialPassword"];
            //WebMail.From = WebMail.UserName;
        }

        public void SendRegisterInfo(string toEmail, int userId, string password)
        {
            try
            {
                var html = Resources.UserRegisterMail;
                html = string.Format(html, toEmail, ConfigurationManager.AppSettings["modifyPasswordUrl"], password, userId);
               // WebMail.Send(toEmail, "小站用户注册", html);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("邮箱不可用"))
                    throw new TpoBaseException("邮箱不可用");
                throw new TpoBaseException("注册邮件发送失败");
            }
        }

        public void SendPasswordReseted(string toEmail, int userId, string password)
        {
            try
            {
                var html = Resources.UserPasswordReseted;
                html = string.Format(html, toEmail, ConfigurationManager.AppSettings["modifyPasswordUrl"], password, userId);
              //  WebMail.Send(toEmail, "小站用户管理", html);
            }
            catch
            {
                throw new Exception("注册邮件发送失败");
            }
        }
    }
}
