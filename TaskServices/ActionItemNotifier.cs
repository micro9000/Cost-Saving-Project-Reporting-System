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
	public static class ActionItemNotifier
	{
		public static async Task<bool> sendEmailNotification ()
		{
			//string base_detail_link = "http: //phsm01ws014.ad.onsemi.com:2828/Home/Details/";
			// string base_detail_link = "http:// myse01ws014.ad.onsemi.com:8090/Home/Details/"; // SBN site1

			//string base_detail_link = "http:// vnbh01ws4064.ad.onsemi.com:8000/Home/Details/"; // OSV
			string base_detail_link = "http://vnbh01ws4064.ad.onsemi.com:9000/Home/Details/"; // OSBD

			Console.WriteLine("---------------------  E-SAVINGS Notifier  -----------------------");

			try
			{

				string emailMsgFooter = @"<br/><br/>
							This is a system generated message. <span style='color:red'>Please do not reply.</span> <br/>
							For any concerns, you can email your COST-ANALYST Reviewer
							";

				List<ProposalAction> openActionItems = Factory.ProposalActionRepository().GetAllOpenActionItems();

				var ownerActions = openActionItems.GroupBy(ai => ai.OwnerFFID);

				string ldapAddrs = "LDAP://ad.onsemi.com";
				string msg = "";
				foreach (var ai in ownerActions)
				{
					Console.WriteLine("---------------------------------> " + ai.Key);
					var ownerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, ai.Key);
					string ownerEmail = (ownerInfo.Count > 0) ? ownerInfo[0].Email : "";
					string ownerName = (ownerInfo.Count > 0) ? ownerInfo[0].DisplayName : "";


					msg += "<b>" +ownerName + "</b><br/>";
					msg += "Action items assigned to you from <b>E-Savings</b> was waiting for your respose. Please Take necessary action. <br/>";
					msg += "Below are the information of each action:<br/><br/>";

					Console.WriteLine("----> " + ownerEmail);

					Dictionary<string, string> approversTemp = new Dictionary<string, string>();

					foreach (var action in ai)
					{
						string apprverFFID = action.Approvers[0].ApproverFFID;

						var approverInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, apprverFFID);
						string approverEmail = (approverInfo.Count > 0) ? (approverInfo[0].Email != null ? approverInfo[0].Email : "") : "";
						string approverName = (approverInfo.Count > 0) ? approverInfo[0].DisplayName : "";

						if (!approversTemp.ContainsKey(apprverFFID))
						{
							approversTemp.Add(action.Approvers[0].ApproverFFID, approverEmail);
						}

						msg += "<hr/><br/>";
						msg += "<table style='width: 100%; background: #fffad9'>";
						msg += "<tr><td style='width: 25%'>DATE Assinged:<td> <td>"+ action.DateAssign.ToShortDateString() +"</td></tr>";
						msg += "<tr><td style='width: 25%'>Assigned By:<td> <td>"+ approverName +"</td></tr>";
						msg += "<tr><td style='width: 25%'>Action needed:<td> <td>"+ action.NeededAction +"</td></tr>";
						msg += "</table><br/>";
						msg += "<b>To respond, please click this <a href='"+ base_detail_link + action.ProposalID +"'>link</a></b><br/><br/>";


						Console.WriteLine("ActionItem " + action.NeededAction);


					}


					Helpers.SendEmail sendEmail = new Helpers.SendEmail(msg, "E-Savings Reminder", emailMsgFooter, "E-SAVINGS", "apps.donotreply@onsemi.com");
					sendEmail.Add_To_Recipient(ownerEmail);

					if (ownerEmail == "" && ownerInfo.Count > 0)
					{
						var supvInfo = Helpers.ONEmployeesLDAP.SearchEmployee(ldapAddrs, ownerInfo[0].ManagerFFID);
						string supvEmail = (supvInfo.Count > 0) ? (supvInfo[0].Email != null ? supvInfo[0].Email : "") : "";
						Console.WriteLine("Supervisor ----> " + supvEmail);
						sendEmail.Add_To_Recipient(supvEmail);
					}


					//sendEmail.Add_CC_Recipient("Raniel.Garcia@onsemi.com");
					Console.WriteLine("======== Approvers ======");

					foreach (var approver in approversTemp)
					{
						sendEmail.Add_CC_Recipient(approver.Value);
						Console.WriteLine(approver.Value);
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

					Console.WriteLine("##############################################################");
					Console.WriteLine("##############################################################");

					msg = "";
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("First Level: " + e.Message);
			}


			Console.WriteLine("DONE!!!!");
			Console.WriteLine("");
			Console.WriteLine("");

			return false;
		}
	}
}
