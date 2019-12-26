using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helpers
{
	public class SendEmail
	{
		private bool successfullySend = false;
		private string senderName = "";
		private string senderEmail = "";
		private MailMessage message = new MailMessage();


		public void Add_To_Recipient (string email)
		{
			if (email != "")
				this.message.To.Add(email.Trim(' '));
		}

		public void Add_CC_Recipient (string email)
		{
			if (email != "")
				this.message.CC.Add(email.Trim(' '));
		}

		public void Add_BCC_Recipient (string email)
		{
			if (email != "")
				this.message.Bcc.Add(email.Trim(' '));
		}

		public SendEmail (string msg, string subject, string msgFooter, string senderName, string senderEmail, string defaultRecipients = "", bool isBodyHmtl = true)
		{
			if (defaultRecipients != "")
			{
				this.Add_CC_Recipient(defaultRecipients);
			}

			this.senderName = senderName;
			this.senderEmail = senderEmail;

			var body = "<p>Hi Good Day!, </p>";
			body += msg;
			body += msgFooter;

			this.message.IsBodyHtml = isBodyHmtl;
			this.message.Subject = subject;
			this.message.Body = string.Format(body, this.senderName, this.senderEmail, msg);
		}

		public async Task<bool> send ()
		{
			using (var smtp = new SmtpClient())
			{

				await smtp.SendMailAsync(this.message);
				this.successfullySend = true;
			}

			return this.successfullySend;
		}

		public bool isSend ()
		{
			return this.successfullySend;
		}
	}
}
