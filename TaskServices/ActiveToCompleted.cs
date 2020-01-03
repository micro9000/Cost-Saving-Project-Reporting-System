using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Helpers;
using ESavingsFactory;

namespace TaskServices
{
	public static class ActiveToCompleted
	{
		public static async Task<bool> MoveActiveProposalsToCompleted ()
		{

			Console.WriteLine("---------------------  E-SAVINGS ACTIVE Proposals to COMPLETED  -----------------------");

			string emailMsgFooter = @"<br/><br/>
							This is a system generated message. <span style='color:red'>Please do not reply.</span> <br/>
							For any concerns, you can email your COST-ANALYST Reviewer";

			var activeProposals = Factory.ProposalFactory().GetProposalByStatus((int)StaticData.OverallStatus.ACTIVE);

			Console.WriteLine(activeProposals.Count);

			string ldapAddrs = "LDAP://ad.onsemi.com";
			//string base_detail_link = "http: //phsm01ws014.ad.onsemi.com:2828/Home/Details/";
			// string base_detail_link = "http: //myse01ws014.ad.onsemi.com:8090/Home/Details/";

			//string base_detail_link = "http:// vnbh01ws4064.ad.onsemi.com:8000/Home/Details/";
			string base_detail_link = "http://vnbh01ws4064.ad.onsemi.com:9000/Home/Details/";


			var CostAnalystList = Factory.CostAnalystFactory().GetAll();
			var FinanceList = Factory.FinanceApproverFactory().GetAll();

			Dictionary<string, string> approvers = new Dictionary<string, string>();

			foreach (var ca in CostAnalystList)
			{
				if (!approvers.ContainsKey(ca.FFID))
				{
					var tempInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, ca.FFID);
					string tempEmail = (tempInfo.Count > 0) ? tempInfo[0].Email : "";

					Console.WriteLine(tempEmail);

					if (tempEmail != "")
						approvers.Add(ca.FFID, tempEmail);
				}
			}


			foreach (var fn in FinanceList)
			{
				if (!approvers.ContainsKey(fn.FFID))
				{
					var tempInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, fn.FFID);
					string tempEmail = (tempInfo.Count > 0) ? tempInfo[0].Email : "";

					Console.WriteLine(tempEmail);

					if (tempEmail != "")
						approvers.Add(fn.FFID, tempEmail);
				}
			}



			foreach (var activeP in activeProposals)
			{
				int numberOfMonths = activeP.NumberOfMonthsToBeActive;
				DateTime expectedStartDate = activeP.ExpectedStartDate;

				//Console.WriteLine(numberOfMonths);
				//Console.WriteLine("Expected: " + expectedStartDate.ToShortDateString());

				DateTime endDate = expectedStartDate.AddMonths(numberOfMonths);

				//Console.WriteLine("End: " + endDate.ToShortDateString());

				if ((endDate == DateTime.Now || endDate < DateTime.Now) && numberOfMonths > 0)
				{
					string msg ="";

					if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.COMPLETED, activeP.Id) > 0)
					{

						// Log the new overall status
						Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
						{
							ProposalID = activeP.Id,
							OAStatus = (int)StaticData.OverallStatus.COMPLETED,
							ApproverFFID = "TaskServices",
							ApproverName = "TaskServices"
						});

						//
						// Update proposal actual completion date when the proposal's status is COMPLETED
						//
						Factory.ProposalFactory().UpdateProposalActualCompletionDate(DateTime.Now, activeP.Id);

						var ownerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, activeP.EmpFFID);
						string ownerEmail = (ownerInfo.Count > 0) ? ownerInfo[0].Email : "";
						string ownerName = (ownerInfo.Count > 0) ? ownerInfo[0].DisplayName : "";

						Console.WriteLine("COMPLETED");

						msg += "<b>" + ownerName + "</b><br/>";
						msg += "Your cost cutting project is now completed<br/>";
						msg += "Below is the information of the proposal<br/><br/>";

						msg += "<hr/><br/>";
						msg += "<table style='width: 100%; background: #fffad9'>";
						msg += "<tr><td style='width: 25%'>Ticket: <td> <td>"+ activeP.ProposalTicket +"</td></tr>";
						msg += "<tr><td style='width: 25%'>Title: <td> <td>"+ activeP.ProjectTitle +"</td></tr>";
						msg += "<tr><td style='width: 25%'>Current Description:<td> <td>"+ activeP.CurrentDescription +"</td></tr>";
						msg += "<tr><td style='width: 25%'>Proposal Description:<td> <td>"+ activeP.ProposalDescription +"</td></tr>";
						msg += "</table><br/>";
						msg += "<b>To view the details, please click this <a href='"+ base_detail_link + activeP.Id +"'>link</a></b><br/><br/>";


						Helpers.SendEmail sendEmail = new Helpers.SendEmail(msg, "E-Savings Reminder", emailMsgFooter, "E-SAVINGS", "apps.donotreply@onsemi.com");
						//sendEmail.Add_CC_Recipient("Raniel.Garcia@onsemi.com");
						sendEmail.Add_To_Recipient(ownerEmail);

						foreach (var apprv in approvers)
						{
							Console.WriteLine(apprv.Value);
							sendEmail.Add_CC_Recipient(apprv.Value);
						}

						try
						{
							Console.WriteLine("Sending.....");
							if (await sendEmail.send() == false)
							{
								Console.WriteLine("Failed to send email !");
							}
							else
							{
								Console.WriteLine("Send successfully");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Second Level" + ex.Message);
							continue;
						}
					}
				}
				else
				{
					Console.WriteLine("NOT YET");
				}

				Console.WriteLine();
			}

			return false;
		}

	}
}
