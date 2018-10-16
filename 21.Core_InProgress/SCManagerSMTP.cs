using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-19 오후 1:20:39
   Description : 
   Edit Log    : 
   ============================================ */

#pragma warning disable 0618

public class SCManagerSMTP : CSingletonNotMonoBase<SCManagerSMTP>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */

    
    static private string _strMyEmail_Adress = "noonbarm@gmail.com";
    static private string _strMyEmail_Password = "Snsqkfka!2";

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    static public void DoSendEmail(string strSendAdress, string strHeader, string strSendMessage)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(_strMyEmail_Adress);
        mail.To.Add(strSendAdress);
        mail.Subject = strHeader;
        mail.Body = strSendMessage;

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
        smtpClient.Port = 587;
        smtpClient.Credentials = new System.Net.NetworkCredential(_strMyEmail_Adress, _strMyEmail_Password) as ICredentialsByHost;
        smtpClient.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        smtpClient.Send(mail);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
