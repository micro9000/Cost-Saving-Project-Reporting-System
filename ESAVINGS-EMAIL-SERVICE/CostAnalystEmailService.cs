using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using SendEmailServiceInterfaces;
using Domain;
using ESavingsFactory;

namespace ESAVINGS_EMAIL_SERVICE
{
	public class CostAnalystEmailService : BaseEmailService//, ISendEmailService
	{
		//async Task<bool>s
		public bool sendNotification ()
		{
			var cost_analysts = Factory.CostAnalystFactory().GetAll().Where(c => c.IsDeleted == 0);

			List<Proposal> forApprovalProposals = new List<Proposal>();


			foreach (var cost_analyst in cost_analysts)
			{
				forApprovalProposals = Factory.ProposalFactory().GetProposalByCostAnalystApprovalPending(cost_analyst.Id);

				forApprovalProposals = this.GetProposalAdditionalInfo(forApprovalProposals);

				foreach (var forApproval in forApprovalProposals)
				{
					Console.WriteLine(forApproval.ProjectTitle);
				}

			}


			//			string emailMsgFooter = @"<br/><br/>
			//							<strong style='color:red'>Please do not reply.</strong><br/>
			//							Applications Engineering | E-SAVINGS";

			//			Helpers.SendEmail sendEmail = new Helpers.SendEmail("test", "Reminder", emailMsgFooter, "E-SAVINGS", "apps.donotreply@onsemi.com");
			//			sendEmail.Add_To_Recipient("Raniel.Garcia@onsemi.com");

			//			if (await sendEmail.send() == false)
			//			{
			//				Console.WriteLine("Failed to send email !");
			//			}
			//			else
			//			{
			//				Console.WriteLine("Send successfully");
			//			}

			return false;
		}
	}
}
