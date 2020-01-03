using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain;
using ESavingsFactory;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;
using System.Text.RegularExpressions;
using Helpers;
using System.Globalization;

namespace ESAVINGS_v1.Controllers
{
	public class ProposalCounterTree
	{
		public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> CounterTree
		{
			get;
			set;
		}

		public Dictionary<string, string> TreeKeyDesc
		{
			get;
			set;
		}
	}

	public class ESavingsController : BaseController
	{
		private string base_url
		{
			get
			{
				return Request.Url.GetLeftPart(UriPartial.Authority);
			}
		}

		public string Generate_new_proposal_ticket ()
		{
			string hash = Helpers.Hashing.GetHashMD5(DateTime.Now.ToString("yyMMddHHmmssffftt"));

			return "ES" + DateTime.Now.ToString("yyMMdd") + "" + hash.Substring(0, 5).ToUpper();
		}



		private string UploadProposalFiles (HttpFileCollectionBase files, int proposalID, int filesLen, string target)
		{
			string errorMsg = "";


			IDictionary<string, string> upload_results = new Dictionary<string, string>();

			int insertedRows = 0;

			if (target == "current_imgs")
			{
				List<CurrentImg> imgs = new List<CurrentImg>();

				for (int i = 0 ; i<filesLen ; i++)
				{
					HttpPostedFileBase file = files["current_imgs_"+i];
					upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_current_imgs"]);

					if (upload_results["done"] == "TRUE")
					{
						imgs.Add(new CurrentImg
						{
							ProposalId = proposalID,
							ServerFileName = upload_results["newFileName"],
							OrigFileName = upload_results["origfileName"]
						});
					}
					else
					{
						errorMsg += upload_results["origfileName"] + "<br/>";
					}

				}

				insertedRows = Factory.CurrentImgFactory().InsertProposalCurrentImgs(imgs);

				if (insertedRows < filesLen)
				{
					errorMsg = "Current Images (Can't upload): <br/>" + errorMsg;
				}
			}
			else if (target == "proposal_imgs")
			{
				List<ProposalImg> imgs = new List<ProposalImg>();
				for (int i = 0 ; i<filesLen ; i++)
				{
					HttpPostedFileBase file = files["proposal_imgs_"+i];
					upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"]);

					if (upload_results["done"] == "TRUE")
					{
						imgs.Add(new ProposalImg
						{
							ProposalId = proposalID,
							ServerFileName = upload_results["newFileName"],
							OrigFileName = upload_results["origfileName"]
						});
					}
					else
					{
						errorMsg += upload_results["origfileName"] + "<br/>";
					}

				}

				insertedRows = Factory.ProposalImgFactory().InsertProposalImgs(imgs);

				if (insertedRows < filesLen)
				{
					errorMsg = "Proposal Images (Can't upload): <br/>" + errorMsg;
				}
			}
			else if (target == "supporting_docs")
			{
				List<SupportingDoc> supportingDocs = new List<SupportingDoc>();

				for (int i = 0 ; i<filesLen ; i++)
				{
					HttpPostedFileBase file = files["supporting_docs_"+i];
					upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_supporting_docs"]);

					if (upload_results["done"] == "TRUE")
					{
						supportingDocs.Add(new SupportingDoc
						{
							ProposalId = proposalID,
							ServerFileName = upload_results["newFileName"],
							OrigFileName = upload_results["origfileName"],
							AttachedBy = this.UserFullName
						});
					}
					else
					{
						errorMsg += upload_results["origfileName"] + "<br/>";
					}

				}

				insertedRows = Factory.SupportingDocFactory().InsertProposalSupportingDocs(supportingDocs);
				if (insertedRows < filesLen)
				{
					errorMsg = "Supporting Images (Can't upload): <br/>" + errorMsg;
				}
			}

			return errorMsg;
		}


		public JsonResult UploadProposalFilesBy (int proposalID, int filesLen, string target)
		{

			//List<CurrentImg> imgs = new List<CurrentImg>();
			//IDictionary<string, string> upload_results = new Dictionary<string, string>();
			//string errorMsg = "";

			//for (int i = 0 ; i<filesLen ; i++)
			//{
			//	HttpPostedFileBase file = Request.Files["current_imgs_"+i];
			//	upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_current_imgs"]);

			//	//return Json(upload_results);

			//	if (upload_results["done"] == "TRUE")
			//	{
			//		imgs.Add(new CurrentImg
			//		{
			//			ProposalId = proposalID,
			//			ServerFileName = upload_results["newFileName"],
			//			OrigFileName = upload_results["origfileName"]
			//		});
			//	}
			//	else
			//	{
			//		errorMsg += upload_results["origfileName"] + "<br/>";
			//	}

			//}

			//return Json(errorMsg);


			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";


			var currentImgsError = this.UploadProposalFiles(Request.Files, proposalID, filesLen, target);

			if (currentImgsError == "")
			{
				results["done"] = "TRUE";
				results["msg"] = "<strong class='good'>Successfully uploaded!</strong>";
			}
			else
			{
				results["msg"] = "<strong class='error'>"+ currentImgsError +"</strong>";
			}

			try
			{

			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}

			return Json(results);
		}


		public async Task<JsonResult> Submit_Proposal (FormCollection data)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";
			results["proposalID"] = "0";


			try
			{

				string newProposalTicketNo = this.Generate_new_proposal_ticket();

				if (IsUserSuccessfullyLoggedIn())
				{
					IDictionary<string, string> dataKeys = new Dictionary<string, string>();
					dataKeys["AreaDept"]				= "Area/Department Beneficiary";
					dataKeys["ProjectTitle"]            = "Project Title";
					dataKeys["CurrentDescription"]      = "Current Description";
					dataKeys["ProposalDescription"]     = "Proposal Description";
					dataKeys["Remarks"]                 = "Remarks";
					//dataKeys["current_imgs_len"]        = "Current Images";
					//dataKeys["proposal_imgs_len"]       = "Proposal Images";
					//dataKeys["supporting_docs_len"]     = "Supporting Documents";


					if (this.IsDL == false)
					{
						if (expected_start_date_is_optional == false)
						{
							dataKeys["ExpectedStartDate"] = "Expected Start Date";
						}

						if (number_of_months_project_as_active_is_optional == false)
						{
							dataKeys["NumberOfMonthsToBeActive"] = "Number of months to be active";
						}

						dataKeys["DollarImpact"] = "Dollar impact";
						dataKeys["ProjectType"]	= "Project Type";
					}


					Tuple<Boolean, string> form_validation_result = this.formData_validation(dataKeys, data);

					if (form_validation_result.Item1 == false)
					{
						results["done"] = "FALSE";
						results["msg"] = form_validation_result.Item2.ToString();
					}
					else
					{

						int current_imgs_len;
						// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
						if (!int.TryParse(data["current_imgs_len"], out current_imgs_len)) // inline declaration
						{
							results["msg"] = "<strong class='error'>Invalid current image length</strong>";
							return Json(results);
						}

						//if (current_imgs_len == 0)
						//{
						//	results["msg"] = "<strong class='error'>Current image required</strong>";
						//	return Json(results);
						//}

						int proposal_imgs_len;
						// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
						if (!int.TryParse(data["proposal_imgs_len"], out proposal_imgs_len))
						{
							results["msg"] = "<strong class='error'>Invalid proposal image length</strong>";
							return Json(results);
						}

						//if (proposal_imgs_len == 0)
						//{
						//	results["msg"] = "<strong class='error'>Proposal image required</strong>";
						//	return Json(results);
						//}

						int supporting_docs_len;
						// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
						if (!int.TryParse(data["supporting_docs_len"], out supporting_docs_len))
						{
							results["msg"] = "<strong class='error'>Invalid supporting documents length</strong>";
							return Json(results);
						}

						DateTime ExpectedStartDate = new DateTime();
						decimal dollarImpact = 0;
						int projectType = 0;
						int numberOfMonthsToBeActive = 0;

						if (this.IsDL == false)
						{
							// Expected Start date
							if (data.AllKeys.Contains("ExpectedStartDate") && data["ExpectedStartDate"] != "")
							{
								if (DateTime.TryParse(data["ExpectedStartDate"], out ExpectedStartDate) == false)
								{
									results["msg"] = "<strong class='error'>Invalid expected start date</strong>";
									return Json(results);
								}

								if (ExpectedStartDate.Date <= DateTime.Now.Date)
								{
									results["msg"] = "<strong class='error'>Invalid expected start date</strong>";
									return Json(results);
								}
							}


							// Dollar impact
							if (Decimal.TryParse(data["DollarImpact"], out dollarImpact) == false)
							{
								results["msg"] = "<strong class='error'>Invalid dollar impact</strong>";
								return Json(results);
							}

							if (dollarImpact <= 0)
							{
								results["msg"] = "<strong class='error'>Dollar impact is required</strong>";
								return Json(results);
							}


							// Project Type
							if (int.TryParse(data["ProjectType"].ToString(), out projectType) == false)
							{
								results["msg"] = "<strong class='error'>Invalid project type</strong>";
								return Json(results);
							}

							if (StaticData.GetProjectTypesStringArray().Contains(projectType) == false)
							{
								results["msg"] = "<strong class='error'>Invalid project type</strong>";
								return Json(results);
							}


							// Number of months
							if (data.AllKeys.Contains("NumberOfMonthsToBeActive") && data["NumberOfMonthsToBeActive"] != "")
							{
								if (int.TryParse(data["NumberOfMonthsToBeActive"], out numberOfMonthsToBeActive) == false)
								{
									results["msg"] = "<strong class='error'>Invalid number of months to be active</strong>";
									return Json(results);
								}

								if ((numberOfMonthsToBeActive <= this.max_num_of_months_to_active && numberOfMonthsToBeActive >= 1) == false)
								{
									results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
									return Json(results);
								}

								if (numberOfMonthsToBeActive > this.max_num_of_months_to_active)
								{
									results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
									return Json(results);
								}

							}
							else if (number_of_months_project_as_active_is_optional == true && data["NumberOfMonthsToBeActive"] == "")
							{
								numberOfMonthsToBeActive = this.max_num_of_months_to_active;
							}
						}

						//if (supporting_docs_len == 0)
						//{
						//	results["msg"] = "<strong class='error'>Supporting Documents required</strong>";
						//	return Json(results);
						//}

						// Check if selected area/department is valid
						//var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();
						var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(data["AreaDept"]);
						if (dept == null)
						{
							results["msg"] = "<strong class='error'>Invalid Department/Area beneficiary...</strong>";
							return Json(results);
						}


						#region Insert proposal basic info


						Proposal newProposal = new Proposal()
						{
							ProposalTicket = newProposalTicketNo,
							ProjectTitle = data["ProjectTitle"],
							CurrentDescription = data["CurrentDescription"],
							ProposalDescription = data["ProposalDescription"],
							Remarks = data["Remarks"],
							AreaDeptBeneficiary = dept.DeptCode,
							SubmittedBy = this.UserFullName,
							EmpFFID = this.UserFFID,
							EmpDeptCode = this.UserDepartment,
							OAStatus = (int)StaticData.OverallStatus.PROJECT_PROPOSAL
						};

						// If the user is IDL, all of this attributes is required
						if (this.IsDL == false)
						{
							newProposal.ProjectType = projectType;
							newProposal.DollarImpact = dollarImpact;
							newProposal.ExpectedStartDate = (data["ExpectedStartDate"] != "") ? ExpectedStartDate : new DateTime(0001, 01, 01);
							newProposal.PlannedProjectStartDate = newProposal.ExpectedStartDate;
							newProposal.NumberOfMonthsToBeActive = numberOfMonthsToBeActive;
							newProposal.OriginalDueDate = newProposal.ExpectedStartDate.AddMonths(newProposal.NumberOfMonthsToBeActive);
							newProposal.CurrentDueDate = newProposal.ExpectedStartDate.AddMonths(newProposal.NumberOfMonthsToBeActive);
						}

						//
						// Actual insertion of proposal details
						//
						int proposalID = Factory.ProposalFactory().Add(newProposal);

						#endregion

						if (proposalID > 0)
						{

							Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
							{
								ProposalID = proposalID,
								OAStatus = (int)StaticData.OverallStatus.PROJECT_PROPOSAL,
								ApproverFFID = this.UserFFID,
								ApproverName = this.UserFullName
							});


							results["done"] = "TRUE";
							results["msg"] = "<strong class='good'>Successfully submitted</strong><br/>";
							results["proposalID"] = proposalID.ToString();

							// Uploading current image/s
							var currentImgsError = this.UploadProposalFiles(Request.Files, proposalID, current_imgs_len, "current_imgs");
							results["msg"] += currentImgsError;

							// Uploading proposal image/s
							var proposalImgsError = this.UploadProposalFiles(Request.Files, proposalID, proposal_imgs_len, "proposal_imgs");
							results["msg"] += proposalImgsError;

							// Uploading supporting document/s
							var supportingDocsError = this.UploadProposalFiles(Request.Files, proposalID, supporting_docs_len, "supporting_docs");
							results["msg"] += supportingDocsError;

							#region Email notification for Cost Analyst and Manager

							string emailMsg = string.Format(@"E-Savings <b>NEW</b> proposal Ticket #{0}. Please click the link below to view the details <br/>
											<a href='{1}/Home/Details/{2}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{3}</td></tr>
												<tr><td>Project Title</td><td>{4}</td></tr>
												<tr><td>Current Description</td><td>{5}</td></tr>
												<tr><td>Proposal Description</td><td>{6}</td></tr>
												<tr><td>Proposed By</td><td>{7}</td></tr>
												<tr><td>Department</td><td>{8}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{9}</td></tr>
											</table>",
													 newProposalTicketNo,
													 this.base_url,
													 proposalID,
													 StaticData.GetProjectTypeStr(newProposal.ProjectType),
													 newProposal.ProjectTitle,
													 newProposal.CurrentDescription,
													 newProposal.ProposalDescription,
													 newProposal.SubmittedBy,
													 newProposal.EmpDeptCode,
													 newProposal.AreaDeptBeneficiary);

							Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "New E-Savings Entry", this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

							// Cost Analysts
							// Changes: use the selected department/area beneficiary to select cost analyst instead of using the user department
							// last: this.UserDepartment
							var CostAnlaysts = Factory.CostAnalystFactory().GetInfoByDeptCode(data["AreaDept"]);
							foreach (var costAnalyst in CostAnlaysts)
							{
								var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, costAnalyst.FFID);

								if (empInfo.Count > 0)
								{
									foreach (var emp in empInfo)
									{
										sendEmail.Add_To_Recipient(emp.Email);

									}
								}
								// Insert Proposal Cost analyst
								Factory.ProposalCostAnalystRepository().Add(new ProposalCostAnalyst()
								{
									ProposalID = proposalID,
									CostAnalystID = costAnalyst.Id
								});

							}


							var upon_submission_recipients = ConfigurationManager.AppSettings["upon_submission_recipients"];
							if (upon_submission_recipients != "")
							{
								sendEmail.Add_CC_Recipient(upon_submission_recipients);
							}

							var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, this.UserManagerFFID);
							if (empDirectSupv != null)
								sendEmail.Add_CC_Recipient(empDirectSupv.Email);

							sendEmail.Add_CC_Recipient(this.UserEmail);


							//var selectedDeptMgr = Helpers.ONEmployeesLDAP.SearchDeptManager(ldapAddress, data["AreaDept"]);

							//if (selectedDeptMgr != null)
							//{
							//	sendEmail.Add_CC_Recipient(selectedDeptMgr.Email);
							//}

							// TODO: change this selection of manager, use the LDAP
							// Manager on selected department/area beneficiary
							//var SelectedDeptManagers = Factory.SOBFactory().SearchManagerByDeptCode(data["AreaDept"]);
							//foreach (var manager in SelectedDeptManagers)
							//{
							//	sendEmail.Add_CC_Recipient(manager.email);
							//}
							//if (this.UserDepartment != data["AreaDept"])
							//{
							//	// User Manager
							//	var UserManagers = Factory.SOBFactory().SearchManagerByDeptCode(this.UserDepartment);
							//	foreach (var manager in UserManagers)
							//	{
							//		sendEmail.Add_CC_Recipient(manager.email);
							//	}
							//}

							try
							{
								await sendEmail.send();
							}
							catch (Exception ex)
							{
								results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
							}


							#endregion

						}
						else
						{
							results["msg"] = "<strong class='error'>Error submitting your proposal, kindly contact APPs team to check the error, Thank you!</strong>";

						}

					}

				}

			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}



			return Json(results);
		}

		public async Task<JsonResult> Update_Proposal (FormCollection data)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";


			try
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					IDictionary<string, string> dataKeys = new Dictionary<string, string>();
					dataKeys["proposalID"]         = "Project ID";
					dataKeys["AreaDept"]		   = "Area/Department Beneficiary";
					dataKeys["ProjectTitle"]       = "Project Title";
					dataKeys["CurrentDescription"] = "Current Description";
					dataKeys["ProposalDescription"]= "Proposal Description";
					dataKeys["Remarks"]            = "Remarks";
					//dataKeys["supporting_docs_len"] = "Supporting documents";


					Tuple<Boolean, string> form_validation_result = this.formData_validation(dataKeys, data);

					if (form_validation_result.Item1 == false)
					{
						results["done"] = "FALSE";
						results["msg"] = form_validation_result.Item2.ToString();
					}
					else
					{
						Proposal proposalDetails = new Proposal();
						int proposalID;
						if (!int.TryParse(data["proposalID"], out proposalID)) // inline declaration
						{
							results["msg"] = "<strong class='error'>Invalid proposal id</strong>";
							return Json(results);
						}

						if (proposalID == 0)
						{
							results["msg"] = "<strong class='error'>Invalid proposal id</strong>";
							return Json(results);
						}

						proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

						if (proposalDetails == null)
						{
							results["msg"] = "<strong class='error'>Proposal details not found</strong>";
							return Json(results);
						}

						if (proposalDetails.EmpFFID == this.UserFFID || 
							this.IsUserIsCostAnalystOnTheCurrentProposal(proposalDetails.AreaDeptBeneficiary) ||
							this.IsUserIsFinanceApproverOnTheCurrentProposal(proposalDetails.Id) ||
							this.IsUserMaster == true)
						{

							#region Update proposal basic info

							// Check if selected area/department is valid
							//var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();
							var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(data["AreaDept"]);
							if (dept == null)
							{
								results["msg"] = "<strong class='error'>Invalid Department/Area beneficiary...</strong>";
								return Json(results);
							}

							// Before updating the AreaDeptBeneficiary
							// check if AreaDeptBeneficiary assigned new value
							bool isAreaDeptBeneficiaryChanged = false;
							string changeAreaDeptBeneficiaryMsg = "";
							if (proposalDetails.AreaDeptBeneficiary != dept.DeptCode)
							{
								isAreaDeptBeneficiaryChanged = true;
								changeAreaDeptBeneficiaryMsg = "and assigned new cost-analyst approver";
							}


							proposalDetails.AreaDeptBeneficiary = dept.DeptCode; //data["AreaDept"];
							proposalDetails.ProjectTitle = data["ProjectTitle"];
							proposalDetails.CurrentDescription = data["CurrentDescription"];
							proposalDetails.ProposalDescription = data["ProposalDescription"];
							proposalDetails.Remarks = data["Remarks"];

							int rowsUpdated = Factory.ProposalFactory().UpdateProposalDetails(proposalDetails);

							#endregion

							if (rowsUpdated > 0)
							{
								results["done"] = "TRUE";
								results["msg"] = "<strong class='good'>Successfully updated "+ changeAreaDeptBeneficiaryMsg +"</strong><br/>";

								int supporting_docs_len;
								// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
								if (!int.TryParse(data["supporting_docs_len"], out supporting_docs_len))
								{
									results["msg"] = "<strong class='error'>Invalid supporting documents length</strong>";
									return Json(results);
								}


								var supportingDocsError = this.UploadProposalFiles(Request.Files, proposalID, supporting_docs_len, "supporting_docs");
								results["msg"] += supportingDocsError;


								#region Email notification for Cost Analyst and Manager

								string emailMsg = string.Format(@"E-Savings proposal Ticket #{0} <b>Update by {10} {11}</b> . Please click the link below to view the details <br/>
											<a href='{1}/Home/Details/{2}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{3}</td></tr>
												<tr><td>Project Title</td><td>{4}</td></tr>
												<tr><td>Current Description</td><td>{5}</td></tr>
												<tr><td>Proposal Description</td><td>{6}</td></tr>
												<tr><td>Proposed By</td><td>{7}</td></tr>
												<tr><td>Department</td><td>{8}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{9}</td></tr>
											</table>",
														 proposalDetails.ProposalTicket,
														 this.base_url,
														 proposalID,
														 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
														 proposalDetails.ProjectTitle,
														 proposalDetails.CurrentDescription,
														 proposalDetails.ProposalDescription,
														 proposalDetails.SubmittedBy,
														 proposalDetails.EmpDeptCode,
														 proposalDetails.AreaDeptBeneficiary,
														 this.UserFullName,
														 changeAreaDeptBeneficiaryMsg);


								Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "New E-Savings Entry Updated", this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);


								var proposal_currentCostAnalyst = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystInfoAndVerificationInfo(proposalID);

								foreach (var curCostAnalyst in proposal_currentCostAnalyst)
								{
									if (isAreaDeptBeneficiaryChanged)
									{
										Factory.ProposalCostAnalystRepository().DeleteProposalCostAnalyst(proposalID, curCostAnalyst.CostAnalystInfo.Id, "Reassigned by "+ this.UserFullName +" -> Update the proposal details");
									}

									var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, curCostAnalyst.CostAnalystInfo.FFID);
									foreach (var emp in empInfo)
									{
										sendEmail.Add_CC_Recipient(emp.Email);
									}
								}


								if (isAreaDeptBeneficiaryChanged)
								{
									// Cost Analysts
									var CostAnlaysts = Factory.CostAnalystFactory().GetInfoByDeptCode(data["AreaDept"]);
									foreach (var costAnalyst in CostAnlaysts)
									{
										var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, costAnalyst.FFID);

										if (empInfo.Count > 0)
										{
											foreach (var emp in empInfo)
											{
												sendEmail.Add_CC_Recipient(emp.Email);
											}

											// TODO: If needed, enable this block of code and add checking for existing cost analyst approver (avoid duplication)
											// Insert Proposal Cost analyst
											Factory.ProposalCostAnalystRepository().Add(new ProposalCostAnalyst()
											{
												ProposalID = proposalID,
												CostAnalystID = costAnalyst.Id
											});
										}
									}
								}

								var ownerInfo = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalDetails.EmpFFID);
								if (ownerInfo != null)
								{
									sendEmail.Add_To_Recipient(ownerInfo.Email);
									if (ownerInfo.Email.ToLower() != this.UserEmail.ToLower())
									{
										sendEmail.Add_CC_Recipient(this.UserEmail);
									}

									var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, ownerInfo.ManagerFFID);
									if (empDirectSupv != null)
										sendEmail.Add_To_Recipient(empDirectSupv.Email);
								}



								try
								{
									await sendEmail.send();
								}
								catch (Exception ex)
								{
									results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
								}
								#endregion

							}
							else
							{
								results["msg"] = "<strong class='error'>Can't update your proposal details</strong>";
							}
						}
						else
						{
							results["msg"] = "<strong class='error'>Permission denied...</strong>";
							return Json(results);
						}

					}

				}

			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
				Console.WriteLine("\nMessage ---\n{0}", ex.Message);
				Console.WriteLine(
					"\nHelpLink ---\n{0}", ex.HelpLink);
				Console.WriteLine("\nSource ---\n{0}", ex.Source);
				Console.WriteLine(
					"\nStackTrace ---\n{0}", ex.StackTrace);
				Console.WriteLine(
					"\nTargetSite ---\n{0}", ex.TargetSite);
			}



			return Json(results);
		}

		public JsonResult GetProposalCurrentImgs (int proposalID)
		{
			var results = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposalID);
			return Json(results);
		}


		public JsonResult GetProposalImgs (int proposalID)
		{
			var results = Factory.ProposalImgFactory().GetProposalImgs(proposalID);
			return Json(results);
		}

		public JsonResult GetProposalSupportingDocs (int proposalID)
		{
			var results = Factory.SupportingDocFactory().GetProposalSupportingDocs(proposalID);
			return Json(results);
		}

		public JsonResult DeleteProposalFilesBy (string target, int fileID, int proposalID)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";

			string[] targetList = { "current", "proposal", "supporting" };

			if (IsUserSuccessfullyLoggedIn())
			{
				var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

				if (proposalDetails != null)
				{
					if (proposalDetails.EmpFFID == this.UserFFID)
					{
						if (targetList.Contains<string>(target))
						{

							bool isDelete = false;

							if (target == targetList[0]) // current
							{
								if (Factory.CurrentImgFactory().DeleteProposalCurrentImg(fileID, proposalID) > 0)
								{
									isDelete = true;

									// audit trail
								}
							}
							else if (target == targetList[1]) // proposal
							{
								if (Factory.ProposalImgFactory().DeleteProposalImg(fileID, proposalID) > 0)
								{
									isDelete = true;
									// audit trail
								}
							}
							else if (target == targetList[2]) // supporting
							{
								if (Factory.SupportingDocFactory().DeleteProposalSupportingDoc(fileID, proposalID) > 0)
								{
									isDelete = true;
									// audit trail
								}
							}

							if (isDelete == true)
							{
								results["msg"] = "<strong class='good'>Deleted successfully</strong>";
							}

						}
						else
						{
							results["msg"] = "<strong class='error'>Invalid target</strong>";
						}
					}
					else
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
					}
				}

			}

			return Json(results);
		}


		[HttpGet]
		public FileResult DownloadSupportingDocument (string fileNameInServer, string origFileName)
		{
			return this.Download(fileNameInServer, "Uploads/SupportingDocs/", origFileName);
		}

		public JsonResult Get_all_Proposals ()
		{
			List<Proposal> results = Factory.ProposalFactory().GetAllProposals().ToList();

			results = this.GetProposalAdditionalInfo(results);

			var searchResults = new
			{
				counter = this.GetStatusCounterTree(results)
			};

			return Json(searchResults);
		}

		public JsonResult Get_all_BPI_Proposals ()
		{
			Response.AppendHeader("Access-Control-Allow-Origin", "*");
			List<Proposal> results = Factory.ProposalFactory().GetAllBPIProposals().ToList();
			results = this.GetProposalAdditionalInfo(results);
			return Json(results, JsonRequestBehavior.AllowGet);
		}



		public JsonResult Search_Proposals (FormCollection data)
		{

			try
			{
				Proposal searchProposal = new Proposal();

				if (data.AllKeys.Contains<string>("ProposalTicket"))
				{
					searchProposal.ProposalTicket = data["ProposalTicket"];
				}

				if (data.AllKeys.Contains<string>("ProjectTitle"))
				{
					searchProposal.ProjectTitle = data["ProjectTitle"];
				}


				if (data.AllKeys.Contains<string>("CurrentDescription"))
				{
					searchProposal.CurrentDescription = data["CurrentDescription"];
				}

				if (data.AllKeys.Contains<string>("ProposalDescription"))
				{
					searchProposal.ProposalDescription = data["ProposalDescription"];
				}

				if (data.AllKeys.Contains<string>("Remarks"))
				{
					searchProposal.Remarks = data["Remarks"];
				}

				if (data.AllKeys.Contains<string>("SubmittedBy"))
				{
					searchProposal.SubmittedBy = data["SubmittedBy"];
				}

				if (data.AllKeys.Contains<string>("OAStatus"))
				{
					int oastatus = 0;
					int.TryParse(data["OAStatus"], out oastatus);
					searchProposal.OAStatus = oastatus;
				}

				if (data.AllKeys.Contains<string>("isBPI"))
				{
					searchProposal.IsBPI = 1;
				}

				List<Proposal> results = Factory.ProposalFactory().SearchProposal(searchProposal).ToList(); // Default is all pending cost analyst review

				results = this.GetProposalAdditionalInfo(results);

				var searchResults = new
				{
					proposals =  results,
					counter = this.GetStatusCounterTree(results)
				};

				return Json(searchResults);

			}
			catch
			{
				return Json("Internal errors.");
			}
		}


		private List<string> GetDeptCodesBySelectedDepartments (string deptList)
		{
			List<string> departmentCodes = new List<string>();

			if (deptList != "")
			{
				var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();

				var deptListArr = deptList.Split(',');

				if (deptListArr[0] == "ALL")
				{
					foreach (var dept in departments)
					{
						departmentCodes.Add(dept.DeptCode);
					}
				}
				else
				{
					foreach (string dept in deptListArr)
					{
						var deptCodes = from d in departments
										where d.DeptName.Trim(' ') == dept.Trim(' ') || d.DeptCode.Trim(' ') == dept.Trim(' ')
										select d.DeptCode;

						foreach (string deptCode in deptCodes)
						{
							departmentCodes.Add(deptCode);
						}
					}
				}


			}

			return departmentCodes;
		}


		private ProposalCounterTree GetStatusCounterTree (List<Proposal> proposals)
		{

			//var proposalCounter = proposals.GroupBy(p => p.OAStatus).Select(g => new
			//{
			//	status = g.Key,
			//	counter = g.ToList().Count(),
			//	dollarImpact = g.Sum(a => (a.NumberOfMonthsToBeActive > 0) ? a.DollarImpact * a.NumberOfMonthsToBeActive : a.DollarImpact) //
			//});

			//var totalCount = proposalCounter.Sum(p => p.counter);

			//var counterTree = StaticData.GetStatusCounterTree();
			//var counterTreeTemp = StaticData.GetStatusCounterTree();


			//foreach (var first in counterTree)
			//{
			//	foreach (var second in first.Value)
			//	{

			//		foreach (var status in second.Value)
			//		{
			//			var temp = status.Value;
			//			foreach (var proposal in proposalCounter)
			//			{
			//				//if (proposal.projectType == (int)StaticData.ProjectTypes.COST_SAVINGS)
			//				//{

			//				//}
			//				//else if (proposal.projectType == (int)StaticData.ProjectTypes.COST_AVOIDANCE)
			//				//{

			//				//}

			//				if (temp["status"] == proposal.status.ToString())
			//				{
			//					counterTreeTemp[first.Key][second.Key][status.Key]["dollarImpact"] = proposal.dollarImpact.ToString("C");
			//					counterTreeTemp[first.Key][second.Key][status.Key]["counter"] = proposal.counter.ToString();
			//				}
			//			}

			//		}
			//	}

			//}


			//counterTreeTemp["TotalCount"]["NA"]["total"]["counter"] = totalCount.ToString();


			var proposalsByProjectTypes = proposals.GroupBy(p => p.ProjectType).ToList();

			Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> proposalsByProjectTypesTmp = 
				new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();

			foreach (var proposalByPType in proposalsByProjectTypes)
			{
				var proposalsTmp = proposalByPType.GroupBy(p => p.OAStatus).Select(g => new
				{
					status = g.Key,
					counter = g.ToList().Count(),
					dollarImpact = g.Sum(a => (a.NumberOfMonthsToBeActive > 0) ? a.DollarImpact * a.NumberOfMonthsToBeActive : a.DollarImpact) //
				});


				var totalCount = proposalsTmp.Sum(p => p.counter);

				var counterTree = StaticData.GetStatusCounterTree();
				var counterTreeTemp = StaticData.GetStatusCounterTree();


				foreach (var first in counterTree)
				{
					foreach (var second in first.Value)
					{

						foreach (var status in second.Value)
						{
							var temp = status.Value;
							foreach (var proposal in proposalsTmp)
							{
								if (temp["status"] == proposal.status.ToString())
								{
									counterTreeTemp[first.Key][second.Key][status.Key]["dollarImpact"] = string.Format(new CultureInfo("en-US"), "{0:C}", proposal.dollarImpact);//proposal.dollarImpact.ToString("C");
									counterTreeTemp[first.Key][second.Key][status.Key]["counter"] = proposal.counter.ToString();
								}
							}

						}
					}

				}


				counterTreeTemp["TotalCount"]["NA"]["total"]["counter"] = totalCount.ToString();

				proposalsByProjectTypesTmp.Add(proposalByPType.Key.ToString(), counterTreeTemp);

			}


			var tree = new ProposalCounterTree
			{
				CounterTree = proposalsByProjectTypesTmp,
				TreeKeyDesc = StaticData.GetStatusCounterTreeKeyDesc()
			};

			return tree;
		}



		public JsonResult testingJson ()
		{
			var departmentCodes = this.GetDeptCodesBySelectedDepartments("");
			//List<Proposal> proposals = Factory.ProposalFactory().SearchProposalByKeywordAndOrStatusAndOrDepts("", "", "", ("11").Split(','), departmentCodes.ToArray<string>());
			List<Proposal> proposals = Factory.ProposalFactory().GetAllProposals().ToList();

			var proposalsByProjectTypes = proposals.GroupBy(p => p.ProjectType).ToList();

			Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> proposalsByProjectTypesTmp = 
				new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();

			foreach (var proposalByPType in proposalsByProjectTypes)
			{
				var proposalsTmp = proposalByPType.GroupBy(p => p.OAStatus).Select(g => new
				{
					status = g.Key,
					counter = g.ToList().Count(),
					dollarImpact = g.Sum(a => (a.NumberOfMonthsToBeActive > 0) ? a.DollarImpact * a.NumberOfMonthsToBeActive : a.DollarImpact) //
				});


				var totalCount = proposalsTmp.Sum(p => p.counter);

				var counterTree = StaticData.GetStatusCounterTree();
				var counterTreeTemp = StaticData.GetStatusCounterTree();


				foreach (var first in counterTree)
				{
					foreach (var second in first.Value)
					{

						foreach (var status in second.Value)
						{
							var temp = status.Value;
							foreach (var proposal in proposalsTmp)
							{
								if (temp["status"] == proposal.status.ToString())
								{
									counterTreeTemp[first.Key][second.Key][status.Key]["dollarImpact"] = proposal.dollarImpact.ToString("C");
									counterTreeTemp[first.Key][second.Key][status.Key]["counter"] = proposal.counter.ToString();
								}
							}

						}
					}

				}


				counterTreeTemp["TotalCount"]["NA"]["total"]["counter"] = totalCount.ToString();

				proposalsByProjectTypesTmp.Add(proposalByPType.Key.ToString(), counterTreeTemp);

			}

			return Json(proposalsByProjectTypesTmp, JsonRequestBehavior.AllowGet);
		}


		// TODO: rename this method (andOr)
		public JsonResult Search_Proposals_by_keyword_andor_status (string projectType, string searchKeyword, string startDate="", string endDate="", string statusList="", string deptList="", string isBPI="")
		{
			try
			{
				var departmentCodes = this.GetDeptCodesBySelectedDepartments(deptList);

				int isBPITmp = (isBPI == "1") ? 1 : 0;

				List<Proposal> results = Factory.ProposalFactory().SearchProposalByKeywordAndOrStatusAndOrDepts(projectType, searchKeyword, startDate, endDate, statusList.Split(','), departmentCodes.ToArray<string>(), isBPITmp);

				results = this.GetProposalAdditionalInfo(results);

				var searchResults = new
				{
					proposals =  results,
					counter = this.GetStatusCounterTree(results)
				};

				return Json(searchResults);
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}
		}


		public string CleanInvalidXmlChars (string StrInput)
		{
			//Returns same value if the value is empty.
			if (string.IsNullOrWhiteSpace(StrInput))
			{
				return StrInput;
			}
			// From xml spec valid chars:
			// #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]    
			// any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
			string RegularExp = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
			return Regex.Replace(StrInput, RegularExp, String.Empty);
		}

		public string GetPropertyValue (object x, string propName)
		{
			string[] propNameArgs = propName.Split('-');

			PropertyInfo property = x.GetType().GetProperty(propNameArgs[0]);

			string val = "";

			if (property != null)
			{
				val = (property.GetValue(x, null) != null) ? property.GetValue(x, null).ToString() : "";

				DateTime tempDate;
				if (DateTime.TryParse(val, out tempDate))
				{
					DateTime defaultDate = new DateTime(0001, 1, 1);
					int result = DateTime.Compare(defaultDate, tempDate);

					if (result != 0)
					{
						if (propNameArgs.Length == 2)
						{
							if (propNameArgs[1].ToLower() == "monthonly")
							{
								val = tempDate.ToString("MMMM");
							}
							else
							{
								val = tempDate.ToLongDateString();
							}
						}
						else
						{
							val = tempDate.ToLongDateString();
						}
					}
					else
					{
						val = "";
					}

				}

			}

			return CleanInvalidXmlChars(val);

		}


		//public JsonResult TestingExportToExcel (string searchKeyword, string startDate="", string endDate="", string statusList="", string deptList="")
		//{
		//	var departmentCodes = this.GetDeptCodesBySelectedDepartments(deptList);
		//	//List<Proposal> results = Factory.ProposalFactory().SearchProposalByKeywordAndOrStatusAndOrDepts(searchKeyword, startDate, endDate, statusList.Split(','), departmentCodes.ToArray<string>());

		//	//results = this.GetProposalAdditionalInfo(results);
		//	List<Proposal> results = Factory.ProposalFactory().SearchProposalByKeywordAndOrStatusAndOrDepts(searchKeyword, startDate, endDate, statusList.Split(','), departmentCodes.ToArray<string>());
		//	results = this.GetProposalAdditionalInfo(results);

		//	string str = "";

		//	string[] propertiesNeeded = {
		//								"ProposalTicket", "ProjectTitle", "CurrentDescription", "ProposalDescription", "Remarks", 
		//								"OAStatusIndicator", "SubmittedDate", "SubmittedBy", "EmpFFID", "DeptName", "EmpDeptCode", "DollarImpact"
		//							  };


		//	foreach (var proposal in results)
		//	{
		//		for (int i=0 ; i< propertiesNeeded.Length ; i++)
		//		{
		//			str += this.GetPropertyValue(proposal, propertiesNeeded[i]);
		//		}
		//	}


		//	return Json(str, JsonRequestBehavior.AllowGet);
		//}

		public FileResult ProposalExportToExcel (string projectType, string searchKeyword, string startDate="", string endDate="", string statusList="", string deptList="")
		{
			// Reference code:
			// https: //docs.microsoft.com/en-us/office/open-xml/working-with-sheets 


			string logsDir = "DummyFiles/";
			var nowTime = DateTime.Now.ToString("yyMMddHHmmssffftt");
			string newFileName = nowTime + ".xlsx";

			var FileVirtualPath = Server.MapPath("~/App_Data/" + logsDir + newFileName);

			// Create a spreadsheet document by supplying the filepath.
			// By default, AutoSave = true, Editable = true, and Type = xlsx.
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(FileVirtualPath, SpreadsheetDocumentType.Workbook);

			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

			// Add a WorksheetPart to the WorkbookPart.
			WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
			worksheetPart.Worksheet = new Worksheet(new SheetData());

			// Add Sheets to the Workbook.
			Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

			// Append a new worksheet and associate it with the workbook.
			Sheet sheet = new Sheet()
			{
				Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
				SheetId = 1,
				Name = nowTime
			};
			sheets.Append(sheet);


			// Get the sheetData cell table.
			SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


			// ############################
			// Header row
			// ############################


			#region Excel headers

			Row headerRow;
			headerRow = new Row()
			{
				RowIndex = 1
			};
			sheetData.Append(headerRow);



			string[] headers = { 
								   "Ticket", "Type", "Project Title", 
								   "Current Description", "Proposal Description", 
								   "Remarks", "Status", "Submission Date", "Submission WW",
								   "Employee Name", "Employee FFID", "Department", "Dept. Code", 
								   "Monthly Dollar impact", "Number of Months as Active", "Total Dollar impact", 
								   "Expected Start Month", "Action Owners-Assign Date", "Cost Analyst Remarks", "Finance Remarks"
							   };

			int letterA = 65;

			int secondLevelLetterA = letterA;

			string HeaderChars = "";

			int level = 0;

			for (int i=0 ; i< headers.Length ; i++)
			{
				if (letterA <= 90 && level == 0)
				{
					HeaderChars = (char)letterA + " ";
					letterA += 1;

					if (letterA == 91)
					{
						level += 1;
						letterA = 65;
					}
				}
				else
				{
					HeaderChars = (char)secondLevelLetterA +""+ (char)letterA + " ";
					letterA += 1;

					if (letterA == 91)
					{
						level += 1;
						secondLevelLetterA += 1;
						letterA = 65;
					}
				}

				headerRow.Append(new Cell()
				{
					CellReference = HeaderChars + (i + 1).ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(headers[i])
				});////
			}

			#endregion



			// ############################
			// GET DATA
			// ############################

			var departmentCodes = this.GetDeptCodesBySelectedDepartments(deptList);
			List<Proposal> results = Factory.ProposalFactory().SearchProposalByKeywordAndOrStatusAndOrDepts(projectType, searchKeyword, startDate, endDate, statusList.Split(','), departmentCodes.ToArray<string>());

			results = this.GetProposalAdditionalInfo(results);


			// ############################
			// SORT THE DATA PER COLUMN
			// ############################

			UInt32 row = 2; // since the row 1 is the header
			Row rowData;

			//string[] propertiesNeeded = {
			//							"ProposalTicket", "ProjectTitle", "CurrentDescription", "ProposalDescription", "Remarks", 
			//							"OAStatusIndicator", "SubmittedDate", "SubmittedBy", "EmpFFID", "DeptName", "AreaDeptBeneficiary", "DollarImpact", "ExpectedStartDate-monthonly"
			//						  };

			foreach (var proposal in results)
			{
				rowData = new Row()
				{
					RowIndex = row
				};
				sheetData.Append(rowData);

				rowData.Append(new Cell()
				{
					CellReference = "A" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.ProposalTicket)
				});

				rowData.Append(new Cell()
				{
					CellReference = "B" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.ProjectTypeIndicator)
				});

				rowData.Append(new Cell()
				{
					CellReference = "C" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(this.CleanInvalidXmlChars(proposal.ProjectTitle))
				});

				rowData.Append(new Cell()
				{
					CellReference = "D" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(this.CleanInvalidXmlChars(proposal.CurrentDescription))
				});

				rowData.Append(new Cell()
				{
					CellReference = "E" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(this.CleanInvalidXmlChars(proposal.ProposalDescription))
				});


				rowData.Append(new Cell()
				{
					CellReference = "F" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(this.CleanInvalidXmlChars(proposal.Remarks))
				});

				rowData.Append(new Cell()
				{
					CellReference = "G" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.OAStatusIndicator)
				});


				rowData.Append(new Cell()
				{
					CellReference = "H" + row.ToString(),
					DataType = CellValues.Date,
					CellValue = new CellValue(proposal.SubmittedDate.ToShortDateString())
				});

				var submittedDateWW = Helpers.ONCalendar.GetWWOnlyByDate(proposal.SubmittedDate);


				rowData.Append(new Cell()
				{
					CellReference = "I" + row.ToString(),
					DataType = CellValues.Number,
					CellValue = new CellValue(submittedDateWW.ToString())
				});


				rowData.Append(new Cell()
				{
					CellReference = "J" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.SubmittedBy)
				});

				rowData.Append(new Cell()
				{
					CellReference = "K" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.EmpFFID)
				});

				rowData.Append(new Cell()
				{
					CellReference = "L" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.DeptName)
				});

				rowData.Append(new Cell()
				{
					CellReference = "M" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(proposal.AreaDeptBeneficiary)
				});


				rowData.Append(new Cell()
				{
					CellReference = "N" + row.ToString(),
					DataType = CellValues.Number,
					CellValue = new CellValue(proposal.DollarImpact.ToString())
				});


				rowData.Append(new Cell()
				{
					CellReference = "O" + row.ToString(),
					DataType = CellValues.Number,
					CellValue = new CellValue(proposal.NumberOfMonthsToBeActive.ToString())
				});

				rowData.Append(new Cell()
				{
					CellReference = "P" + row.ToString(),
					DataType = CellValues.Number,
					CellValue = new CellValue((proposal.DollarImpact * proposal.NumberOfMonthsToBeActive).ToString())
				});


				DateTime defaultDate = new DateTime(0001, 1, 1);
				int result = DateTime.Compare(defaultDate, proposal.ExpectedStartDate);
				rowData.Append(new Cell()
				{
					CellReference = "Q" + row.ToString(),
					DataType = CellValues.Date,// proposal.ExpectedStartDate.ToString("yyyy")
					CellValue = new CellValue((result != 0) ? proposal.ExpectedStartDate.ToShortDateString() : "")
				});

				string[] approvalStatus = { "No-verification", "Verified closed", "Verify not done" };

				proposal.NeededActions = Factory.ProposalActionRepository().GetProposalActionsWithApprovers(proposal.Id);
				string actionOwnersWithAssignDate = "";
				foreach (var action in proposal.NeededActions)
				{
					actionOwnersWithAssignDate += action.OwnerFullname +"("+ approvalStatus[action.IsClosed] +"), ";
				}
				rowData.Append(new Cell()
				{
					CellReference = "R" + row.ToString(),
					DataType = CellValues.String,
					CellValue = new CellValue(actionOwnersWithAssignDate)
				});


				proposal.CostAnalysts = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystInfoAndVerificationInfo(proposal.Id);

				if (proposal.CostAnalysts.Count > 0)
				{
					rowData.Append(new Cell()
					{
						CellReference = "S" + row.ToString(),
						DataType = CellValues.String,
						CellValue = new CellValue(proposal.CostAnalysts[0].Remarks)
					});
				}

				proposal.FinanceApproval = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoAndVerificationInfo(proposal.Id);


				if (proposal.FinanceApproval.Count > 0)
				{
					rowData.Append(new Cell()
					{
						CellReference = "T" + row.ToString(),
						DataType = CellValues.String,
						CellValue = new CellValue(proposal.FinanceApproval[0].Remarks)
					});
				}



				row += 1;

				//sheetData.Append(rowData);

				//letterA = 65;
				//secondLevelLetterA = letterA;
				//HeaderChars = "";
				//level = 0;

				//for (int i=0 ; i< propertiesNeeded.Length ; i++)
				//{
				//	if (letterA <= 90 && level == 0)
				//	{
				//		HeaderChars = (char)letterA + " ";
				//		letterA += 1;

				//		if (letterA == 91)
				//		{
				//			level += 1;
				//			letterA = 65;
				//		}
				//	}
				//	else
				//	{
				//		HeaderChars = (char)secondLevelLetterA +""+ (char)letterA + " ";
				//		letterA += 1;

				//		if (letterA == 91)
				//		{
				//			level += 1;
				//			secondLevelLetterA += 1;
				//			letterA = 65;
				//		}
				//	}


				//	rowData.Append(new Cell()
				//	{
				//		CellReference = HeaderChars + row.ToString(),
				//		DataType = CellValues.String,
				//		CellValue = new CellValue(this.GetPropertyValue(proposal, propertiesNeeded[i]))
				//	});////
				//}

				//row += 1;
			}


			// Save and Close the document.
			workbookpart.Workbook.Save();
			spreadsheetDocument.Close();

			//try
			//{

			//}
			//catch (Exception ex)
			//{
			//	throw new Exception(ex.Message);
			//}

			return this.Download(newFileName, logsDir, newFileName);

		}


		public async Task<JsonResult> CostAnalystApproval (string proposalID, string remarks, string isVerified, string OAStatus, int costAnalystID,
															string projectType="", string dollarImpactStr="", string expectedStartDate="", string numberOfMonthsToBeActiveStr="",
															int supportingDocsLen=0, string selectedFinanceFFID="")
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";


			try
			{
				decimal dollarImpact = 0;
				int numberOfMonthsToBeActive = 0;
				int projectTypeIntParse = 0;

				if (IsUserSuccessfullyLoggedIn())
				{
					if (this.UserType != ((int)StaticData.UserTypes.CostAnalyst).ToString())
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}


					int proposalIDIntParse;
					if (int.TryParse(proposalID, out proposalIDIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid Proposal</strong>";
						return Json(results);
					}

					if (proposalIDIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid Proposal</strong>";
						return Json(results);
					}

					if (string.IsNullOrEmpty(remarks))
					{
						results["msg"] = "<strong class='error'>Remarks is required</strong>";
						return Json(results);
					}




					int isVerifiedIntParse;
					if (int.TryParse(isVerified, out isVerifiedIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}
					if (isVerifiedIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}
					if (isVerifiedIntParse != 1 && isVerifiedIntParse != 2 && isVerifiedIntParse != 3)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}



					int OAStatusIntParse;
					if (int.TryParse(OAStatus, out OAStatusIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid OA status</strong>";
						return Json(results);
					}
					if (OAStatusIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid OA status</strong>";
						return Json(results);
					}



					// If Saved, just check if dollar impact or number of months to be active has data
					if (isVerifiedIntParse == 3)
					{

						if (projectType != "")
						{
							if (int.TryParse(projectType, out projectTypeIntParse) == false)
							{
								results["msg"] = "<strong class='error'>Invalid Project type</strong>";
								return Json(results);
							}

							if (StaticData.GetProjectTypesStringArray().Contains(projectTypeIntParse) == false)
							{
								results["msg"] = "<strong class='error'>Invalid Project type</strong>";
								return Json(results);
							}
						}


						if (dollarImpactStr != "")
						{
							if (decimal.TryParse(dollarImpactStr, out dollarImpact) == false)
							{
								results["msg"] = "<strong class='error'>Invalid dollar impact (must be whole/decimal number)</strong>";
								return Json(results);
							}

						}

						if (numberOfMonthsToBeActiveStr != "")
						{
							if (int.TryParse(numberOfMonthsToBeActiveStr, out numberOfMonthsToBeActive) == false)
							{
								results["msg"] = "<strong class='error'>Invalid number of months to be active</strong>";
								return Json(results);
							}

							if ((numberOfMonthsToBeActive <= this.max_num_of_months_to_active && numberOfMonthsToBeActive >= 1) == false)
							{
								results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
								return Json(results);
							}

							if (numberOfMonthsToBeActive > this.max_num_of_months_to_active)
							{
								results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
								return Json(results);
							}
						}
					}

					// Require all
					if (isVerifiedIntParse == 1) // Verified or Finance Approval
					{

						if (projectType == "")
						{
							results["msg"] = "<strong class='error'>Project type is required</strong>";
							return Json(results);
						}

						if (int.TryParse(projectType, out projectTypeIntParse) == false)
						{
							results["msg"] = "<strong class='error'>Invalid Project type</strong>";
							return Json(results);
						}

						if (StaticData.GetProjectTypesStringArray().Contains(projectTypeIntParse) == false)
						{
							results["msg"] = "<strong class='error'>Invalid Project type</strong>";
							return Json(results);
						}

						if (dollarImpactStr == "")
						{
							results["msg"] = "<strong class='error'>Dollar impact is required</strong>";
							return Json(results);
						}

						if (decimal.TryParse(dollarImpactStr, out dollarImpact) == false)
						{
							results["msg"] = "<strong class='error'>Invalid dollar impact (must be whole/decimal number)</strong>";
							return Json(results);
						}


						if (numberOfMonthsToBeActiveStr == "")
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active</strong>";
							return Json(results);
						}

						if (int.TryParse(numberOfMonthsToBeActiveStr, out numberOfMonthsToBeActive) == false)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active</strong>";
							return Json(results);
						}

						if ((numberOfMonthsToBeActive <= this.max_num_of_months_to_active && numberOfMonthsToBeActive >= 1) == false)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
							return Json(results);
						}

						if (numberOfMonthsToBeActive > this.max_num_of_months_to_active)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
							return Json(results);
						}

						if (expected_start_date_is_optional == false)
						{
							if (string.IsNullOrEmpty(expectedStartDate))
							{
								results["msg"] = "<strong class='error'>Expected start date is required</strong>";
								return Json(results);
							}
						}


						if (string.IsNullOrWhiteSpace(selectedFinanceFFID))
						{
							results["msg"] = "<strong class='error'>Please select finance approver</strong>";
							return Json(results);
						}
					}


					if (this.IsUserMaster == true)
					{
						// If the user is master approver for Cost Analyst, we need to verify if the cost analyst (id) is allowed to approve
						var costAnalystApprovalResults = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystVerificationResults(proposalIDIntParse, costAnalystID);
						if (costAnalystApprovalResults == null)
						{
							results["msg"] = "<strong class='error'>Cost Analyst approver is not allowed to make changes on this approval.</strong>";
							return Json(results);
						}
					}



					FinanceApprover financeInfo = new FinanceApprover();
					if (string.IsNullOrWhiteSpace(selectedFinanceFFID) == false)
					{
						financeInfo = Factory.FinanceApproverFactory().GetInfoByFFID(selectedFinanceFFID);

						if (financeInfo == null)
						{
							results["msg"] = "<strong class='error'>Selected finance approver not found</strong>";
							return Json(results);
						}
					}


					var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalIDIntParse);

					if (this.IsUserIsCostAnalystOnTheCurrentProposal(proposalDetails.AreaDeptBeneficiary) == true || this.IsUserMaster == true)
					{
						//var costAnalystVerificationResults = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystVerificationResults(proposalIDIntParse, int.Parse(this.UserCostAnalystID));
						//costAnalystVerificationResults.CostAnalystID

						//if (proposalDetails.OAStatus != (int)StaticData.OverallStatus.PENDING_COST_ANALYST_REVIEW && proposalDetails.OAStatus != (int)StaticData.OverallStatus.IN_PROGRESS)
						//{
						//	results["msg"] = "<strong class='error'>Permission Denied...11</strong>";
						//	return Json(results);
						//}

						int UserCostAnalystIDTmp = (this.IsUserMaster == true) ? costAnalystID : int.Parse(this.UserCostAnalystID);


						//
						// Actual updating and approval
						//
						if (Factory.ProposalCostAnalystRepository().UpdateProposalCostAnalystVerification(proposalIDIntParse, UserCostAnalystIDTmp, remarks, isVerifiedIntParse) > 0)
						{

							// TODO: if current status is INPROGRESS and the cost analyst review the action provided


							string message = "";

							if (dollarImpact > 0 && proposalDetails.DollarImpact != dollarImpact)
							{
								//
								// Update dollar impact
								//
								if (Factory.ProposalFactory().UpdateProposalDollarImpact(dollarImpact, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't add/update dolar impact";
								}
								else
								{
									message += "<br/> Added dollar impact";
								}
							}

							if (projectTypeIntParse > 0 && proposalDetails.ProjectType != projectTypeIntParse)
							{
								//
								// Update Project Type
								//
								if (Factory.ProposalFactory().UpdateProposalProjectType(projectTypeIntParse, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't add/update project type";
								}
								else
								{
									message += "<br/> Added project type";
								}
							}

							if (numberOfMonthsToBeActive > 0 && proposalDetails.NumberOfMonthsToBeActive != numberOfMonthsToBeActive)
							{
								//
								// Update Proposal number of months to be active
								//
								if (Factory.ProposalFactory().UpdateProposalNummberOfMonthsToBeActive(numberOfMonthsToBeActive, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't add/update number of months to be active";
								}
								else
								{
									message += "<br/> Added number of months to be active";

									// Update the current due date
									// when they update the expected start date and number of months to active
									proposalDetails.CurrentDueDate = proposalDetails.ExpectedStartDate.AddMonths(numberOfMonthsToBeActive);
									Factory.ProposalFactory().UpdateProposalCurrentDueDate(proposalDetails.CurrentDueDate, proposalIDIntParse);

								}
							}


							if (expectedStartDate != "")
							{
								DateTime expectedStartDateParsed;
								if (DateTime.TryParse(expectedStartDate, out expectedStartDateParsed) && proposalDetails.ExpectedStartDate != expectedStartDateParsed)
								{
									//
									// Update expected start date
									//
									if (Factory.ProposalFactory().UpdateProposalExpectedStartDate(expectedStartDateParsed, proposalIDIntParse) == 0)
									{
										message += "<br/> Can't add/update expected project start date";
									}
									else
									{
										message += "<br/> Added expected project start date";

										// Update the current due date
										// when they update the expected start date and number of months to active
										proposalDetails.CurrentDueDate = expectedStartDateParsed.AddMonths(proposalDetails.NumberOfMonthsToBeActive);
										Factory.ProposalFactory().UpdateProposalCurrentDueDate(proposalDetails.CurrentDueDate, proposalIDIntParse);
									}
								}
							}


							#region Insert Supporting documents

							if (supportingDocsLen > 0 && (isVerifiedIntParse == 1 || isVerifiedIntParse == 3))
							{
								IDictionary<string, string> upload_results = new Dictionary<string, string>();

								List<SupportingDoc> supportingDocs = new List<SupportingDoc>();
								upload_results = new Dictionary<string, string>();
								string supportingDocsErr = "Supporting Documents (Can't upload): <br/>";

								for (int i = 0 ; i<supportingDocsLen ; i++)
								{
									HttpPostedFileBase file = Request.Files["supporting_docs_"+i];

									upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_supporting_docs"]);

									if (upload_results["done"] == "TRUE")
									{
										supportingDocs.Add(new SupportingDoc
										{
											ProposalId = proposalIDIntParse,
											ServerFileName = upload_results["newFileName"],
											OrigFileName = upload_results["origfileName"],
											AttachedBy = this.UserFullName
										});
									}
									else
									{
										supportingDocsErr += upload_results["origfileName"] + "<br/>";
									}
									//
								}

								if (supportingDocs.Count > 0)
								{
									int supportingDocsInsertedRows = Factory.SupportingDocFactory().InsertProposalSupportingDocs(supportingDocs);

									if (supportingDocsInsertedRows < supportingDocsLen)
									{
										message += supportingDocsErr;
									}
									else if (supportingDocsInsertedRows == supportingDocsLen)
									{
										message += "<br/>Supporting documents uploaded";
									}
								}

							}

							#endregion


							string[] verificationApprovalStatusList = { "Unknown", "Verified", "Invalid", "Saved" };

							var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);


							string proposalOwnerEmail = "";
							string ownerManagerEmail = "";

							if (proposalOwnerInfo.Count > 0)
							{
								proposalOwnerEmail = (proposalOwnerInfo.Count > 0) ? proposalOwnerInfo[0].Email : "";
								var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
								ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
							}


							int[] invalid_status_list = { (int)StaticData.OverallStatus.CANCELED,
														(int)StaticData.OverallStatus.INVALID,
														//(int)StaticData.OverallStatus.COST_AVOIDANCE,
														(int)StaticData.OverallStatus.EXISTING_PROJECT,
														(int)StaticData.OverallStatus.DUPLICATE_ENTRY };


							//
							// When Invalid or disapproved
							//
							if (isVerifiedIntParse == 2 && invalid_status_list.Contains(OAStatusIntParse))
							{
								int OAStatusIndex = Array.IndexOf(invalid_status_list, OAStatusIntParse);
								int invalid_status = invalid_status_list[OAStatusIndex];

								if (Factory.ProposalFactory().UpdateProposalStatus(invalid_status, proposalIDIntParse) > 0)
								{
									// Log the new overall status
									Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
									{
										ProposalID = proposalIDIntParse,
										OAStatus = invalid_status,
										ApproverFFID = this.UserFFID,
										ApproverName = this.UserFullName
									});

									results["done"] = "FALSE";
									results["msg"] = "<strong class='good'>Successfully move to <i>"+ StaticData.GetOverallStatusStr(invalid_status) +"</i> the status!</strong>";


									string emailMsg = "";
									emailMsg = string.Format(@"E-Savings Ticket #{0}. {1} by {2}., Please click the link below to view the details <br/>
											<a href='{3}/Home/Details/{4}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{5}</td></tr>
												<tr><td>Project Title</td><td>{6}</td></tr>
												<tr><td>Current Description</td><td>{7}</td></tr>
												<tr><td>Proposal Description</td><td>{8}</td></tr>
												<tr><td>Proposed By</td><td>{9}</td></tr>
												<tr><td>Department</td><td>{10}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{11}</td></tr>
											</table>",
													 proposalDetails.ProposalTicket,
													 verificationApprovalStatusList[isVerifiedIntParse],
													 this.UserFullName,
													 this.base_url,
													 proposalID,
													 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
													 proposalDetails.ProjectTitle,
													 proposalDetails.CurrentDescription,
													 proposalDetails.ProposalDescription,
													 proposalDetails.SubmittedBy,
													 proposalDetails.EmpDeptCode,
													 proposalDetails.AreaDeptBeneficiary);

									Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings-"+ verificationApprovalStatusList[isVerifiedIntParse] +"-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

									sendEmail.Add_To_Recipient(proposalOwnerEmail);
									sendEmail.Add_To_Recipient(ownerManagerEmail);
									sendEmail.Add_CC_Recipient(this.UserEmail);// current user: cost-analyst approver

									try
									{
										await sendEmail.send();
									}
									catch (Exception ex)
									{
										results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
									}

									return Json(results);
								}
							}
							//
							// When Saving
							//
							else if (isVerifiedIntParse == 3 && OAStatusIntParse == (int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS)
							{

								if (proposalDetails.OAStatus == (int)StaticData.OverallStatus.PROJECT_PROPOSAL)
								{
									if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS, proposalIDIntParse) == 0)
									{
										message += "<br/> Can't update overall status (IN-PROGRESS)";
									}
									else
									{
										// Log the new overall status
										Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
										{
											ProposalID = proposalIDIntParse,
											OAStatus = (int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS,
											ApproverFFID = this.UserFFID,
											ApproverName = this.UserFullName
										});
									}
								}

								// Add finance approver
								if (string.IsNullOrWhiteSpace(selectedFinanceFFID) == false)
								{
									var LastFinanceInfo = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoAndVerificationInfo(proposalIDIntParse);
									if (LastFinanceInfo.Count > 0)
									{

										int lastFinanceInfoID = (LastFinanceInfo.Count > 0) ? LastFinanceInfo[0].FinanceID : 0;

										// reassign
										if (lastFinanceInfoID > 0 && financeInfo.Id != lastFinanceInfoID)
										{
											if (Factory.ProposalFinanceApprovalRepository().ReassignProposalFinanceApproval(proposalIDIntParse, financeInfo.Id, lastFinanceInfoID) == 0)
											{
												message += "<br/> Can't reassign finance approver";
											}
											else
											{
												message += "<br/> Done changing finance approver.";
											}
										}
									}
									else
									{
										// add
										var proposalFinanceApproval = new ProposalFinanceApproval()
										{
											ProposalID = proposalIDIntParse,
											FinanceID = financeInfo.Id,
											Remarks = "na"
										};

										if (Factory.ProposalFinanceApprovalRepository().Add(proposalFinanceApproval) == 0)
										{
											message += "<br/> Can't add finance approver";
										}
										else
										{
											message += "<br/> Added finance approver";
										}
									}


								}



								string emailMsg = "";
								emailMsg = string.Format(@"E-Savings Ticket #{0}. Moved status to {1} by {2}., Please click the link below to view the details <br/>
											<a href='{3}/Home/Details/{4}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{5}</td></tr>
												<tr><td>Project Title</td><td>{6}</td></tr>
												<tr><td>Current Description</td><td>{7}</td></tr>
												<tr><td>Proposal Description</td><td>{8}</td></tr>
												<tr><td>Proposed By</td><td>{9}</td></tr>
												<tr><td>Department</td><td>{10}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{11}</td></tr>
											</table>",
												 proposalDetails.ProposalTicket,
												 StaticData.GetOverallStatusStr((int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS),
												 this.UserFullName,
												 this.base_url,
												 proposalID,
												 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
												 proposalDetails.ProjectTitle,
												 proposalDetails.CurrentDescription,
												 proposalDetails.ProposalDescription,
												 proposalDetails.SubmittedBy,
												 proposalDetails.EmpDeptCode,
												 proposalDetails.AreaDeptBeneficiary);

								Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings-Move-status-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

								sendEmail.Add_To_Recipient(proposalOwnerEmail);
								sendEmail.Add_To_Recipient(ownerManagerEmail);
								sendEmail.Add_CC_Recipient(this.UserEmail);// current user: cost-analyst approver

								try
								{
									await sendEmail.send();
								}
								catch (Exception ex)
								{
									message += "<br/><span class='error'>" + ex.Message + "</span>";
								}


								results["done"] = "TRUE";
								results["msg"] = "<strong class='good'>Successfully Saved!</strong>" + message;


							}
							//
							// When cost analyst moved the proposal status to finance approval
							//
							else if (isVerifiedIntParse == 1 && OAStatusIntParse == (int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED)
							{

								var neededActions = Factory.ProposalActionApproverRepository().GetProposalActionApprovers(proposalDetails.Id);
								var countClosedActionItems = neededActions.Where(a => a.ApprovalStatus == 1).Sum(a => a.Id);
								var countAllActionItems = neededActions.Sum(a => a.Id);

								// all action items must be closed before moving the finance approval
								if (countClosedActionItems == countAllActionItems)
								{
									bool doneAddingFinance = true;

									var proposalFinanceApproval = new ProposalFinanceApproval()
									{
										ProposalID = proposalIDIntParse,
										FinanceID = financeInfo.Id,
										Remarks = "na"
									};


									var LastFinanceInfo = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoAndVerificationInfo(proposalIDIntParse);
									if (LastFinanceInfo.Count > 0)
									{
										int lastFinanceInfoID = (LastFinanceInfo.Count > 0) ? LastFinanceInfo[0].FinanceID : 0;

										if (lastFinanceInfoID > 0 && financeInfo.Id != lastFinanceInfoID)
										{
											// Reassign
											if (Factory.ProposalFinanceApprovalRepository().ReassignProposalFinanceApproval(proposalIDIntParse, financeInfo.Id, lastFinanceInfoID) == 0)
											{
												message += "<br/> Can't reassign finance approver";
												doneAddingFinance = false;
											}
											else
											{
												message += "<br/> Done changing finance approver.";
											}
										}

									}
									else
									{
										// add
										if (Factory.ProposalFinanceApprovalRepository().Add(proposalFinanceApproval) == 0)
										{
											message += "<br/> Can't add finance approver";
											doneAddingFinance = false;
										}
										else
										{
											message += "<br/> Done adding finance approver.";
										}
									}

									if (doneAddingFinance == true)
									{
										if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED, proposalIDIntParse) == 0)
										{
											message += "<br/> Can't update overall status " + StaticData.GetOverallStatusStr((int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED);

										}
										else
										{
											// Log the new overall status
											Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
											{
												ProposalID = proposalIDIntParse,
												OAStatus = (int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED,
												ApproverFFID = this.UserFFID,
												ApproverName = this.UserFullName
											});


											var selectedFinanceInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, financeInfo.FFID);
											string selectedFinanceEmail = (selectedFinanceInfo.Count > 0) ? selectedFinanceInfo[0].Email : "";

											string emailMsg = "";
											emailMsg = string.Format(@"E-Savings Ticket #{0}. {1} by {2}. <b>Need your approval</b>, Please click the link below to view the details <br/>
														<a href='{3}/Home/Details/{4}'>Details</a>
														<table>
															<tr><td>Project Type</td><td>{5}</td></tr>
															<tr><td>Project Title</td><td>{6}</td></tr>
															<tr><td>Current Description</td><td>{7}</td></tr>
															<tr><td>Proposal Description</td><td>{8}</td></tr>
															<tr><td>Proposed By</td><td>{9}</td></tr>
															<tr><td>Department</td><td>{10}</td></tr>
															<tr><td>Department/Area beneficiary</td><td>{11}</td></tr>
														</table>",
															 proposalDetails.ProposalTicket,
															 verificationApprovalStatusList[isVerifiedIntParse],
															 this.UserFullName,
															 this.base_url,
															 proposalID,
															 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
															 proposalDetails.ProjectTitle,
															 proposalDetails.CurrentDescription,
															 proposalDetails.ProposalDescription,
															 proposalDetails.SubmittedBy,
															 proposalDetails.EmpDeptCode,
															 proposalDetails.AreaDeptBeneficiary);

											Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings-"+ verificationApprovalStatusList[isVerifiedIntParse] +"-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

											sendEmail.Add_To_Recipient(selectedFinanceEmail);
											sendEmail.Add_CC_Recipient(proposalOwnerEmail);
											sendEmail.Add_CC_Recipient(ownerManagerEmail);
											sendEmail.Add_CC_Recipient(this.UserEmail);// current user: cost-analyst approver

											try
											{
												await sendEmail.send();
											}
											catch (Exception ex)
											{
												message += "<br/><span class='error'>" + ex.Message + "</span>";
											}
										}
									}
									else
									{
										message += "<br/>Can't insert finance approvers";
									}

									results["done"] = "TRUE";
									results["msg"] = "<strong class='good'>Successfully verified!</strong>" + message;
								}
								else
								{
									results["done"] = "FALSE";
									results["msg"] = "<strong class='error'>Please close all action items before moving the status to finance approval</strong>" + message;
								}

							}
							else
							{
								results["done"] = "FALSE";
								results["msg"] = "<strong class='error'>Verification status not found</strong>" + message;
							}


						}
						else
						{
							results["done"] = "FALSE";
							results["msg"] = "<strong class='error'>Can't verify this proposal</strong>";
						}

					}
					else
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}

				}

			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}





			return Json(results);
		}


		public async Task<JsonResult> MarkProposalAsBPI (string status, string proposalID)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";

			try
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					int proposalIDIntParse;
					if (int.TryParse(proposalID, out proposalIDIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid Proposal</strong>";
						return Json(results);
					}

					int statusintTmp;
					if (int.TryParse(status, out statusintTmp) == false)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}

					if (statusintTmp == 1 || statusintTmp == 0)
					{


						string statusStr = "unmark";

						if (statusintTmp == 1)
						{
							statusStr = "mark";
						}

						if (Factory.ProposalFactory().MarkProposalAsBPI(statusintTmp, proposalIDIntParse) > 0)
						{

							var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalIDIntParse);

							string emailMsg = string.Format(@"E-Savings proposal Ticket #{0} <b>Marked as BPI</b>. Please click the link below to view the details <br/>
											<a href='{1}/Home/Details/{2}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{3}</td></tr>
												<tr><td>Project Title</td><td>{4}</td></tr>
												<tr><td>Current Description</td><td>{5}</td></tr>
												<tr><td>Proposal Description</td><td>{6}</td></tr>
												<tr><td>Proposed By</td><td>{7}</td></tr>
												<tr><td>Department</td><td>{8}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{9}</td></tr>
											</table>",
											proposalDetails.ProposalTicket,
											this.base_url,
											proposalID,
											StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
											proposalDetails.ProjectTitle,
											proposalDetails.CurrentDescription,
											proposalDetails.ProposalDescription,
											proposalDetails.SubmittedBy,
											proposalDetails.EmpDeptCode,
											proposalDetails.AreaDeptBeneficiary);

							Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings project marked as BPI", this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);


							var upon_submission_recipients = ConfigurationManager.AppSettings["upon_submission_recipients"];
							if (upon_submission_recipients != "")
							{
								sendEmail.Add_CC_Recipient(upon_submission_recipients);
							}

							var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);
							if (proposalOwnerInfo.Count > 0)
							{
								string proposalOwnerEmail = (proposalOwnerInfo.Count > 0) ? proposalOwnerInfo[0].Email : "";

								sendEmail.Add_To_Recipient(proposalOwnerEmail);

								var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
								string ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";

								sendEmail.Add_To_Recipient(ownerManagerEmail);


							}

							string errMsg = "";

							try
							{
								await sendEmail.send();
							}
							catch (Exception ex)
							{
								errMsg += "<br/><span class='error'>" + ex.Message + "</span>";
							}



							results["done"] = "TRUE";
							results["msg"] = "<strong class='good'>Successfully "+ statusStr +" proposal as BPI <br/>" + errMsg +"</strong>";
						}
						else
						{
							results["done"] = "FALSE";
							results["msg"] = "<strong class='error'>Error "+ statusStr +" proposal as BPI</strong>";
						}
					}
					else
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}

				}
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}

			return Json(results);
		}


		public async Task<JsonResult> FinanceApproval (string proposalID, string remarks, string isVerified, string OAStatus,
													int financeApproverID, string projectType="", string dollarImpactStr="",
													string numberOfMonthsToBeActiveStr="", string expectedStartDate="", int financeCategoryID=0, int supportingDocsLen=0)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";


			try
			{
				decimal dollarImpact = 0;
				int numberOfMonthsToBeActive = 0;
				int projectTypeIntParse = 0;

				if (IsUserSuccessfullyLoggedIn())
				{
					if (this.UserType != ((int)StaticData.UserTypes.Finance).ToString())
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}
					int proposalIDIntParse;
					if (int.TryParse(proposalID, out proposalIDIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid Proposal</strong>";
						return Json(results);
					}

					if (proposalIDIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid Proposal</strong>";
						return Json(results);
					}

					if (string.IsNullOrWhiteSpace(remarks))
					{
						results["msg"] = "<strong class='error'>Remarks is required</strong>";
						return Json(results);
					}


					int isVerifiedIntParse;
					if (int.TryParse(isVerified, out isVerifiedIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}

					if (isVerifiedIntParse > 4 || isVerifiedIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid status</strong>";
						return Json(results);
					}


					// 1-Verified or cost-funnel-evaluation
					// 4-closed or active, 
					// 3-saved
					if (isVerifiedIntParse == 1 || isVerifiedIntParse == 4 || isVerifiedIntParse == 3)
					{

						if (projectType == "")
						{
							results["msg"] = "<strong class='error'>Project type is required</strong>";
							return Json(results);
						}


						if (int.TryParse(projectType, out projectTypeIntParse) == false)
						{
							results["msg"] = "<strong class='error'>Invalid Project type</strong>";
							return Json(results);
						}


						if (StaticData.GetProjectTypesStringArray().Contains(projectTypeIntParse) == false)
						{
							results["msg"] = "<strong class='error'>Invalid Project type</strong>";
							return Json(results);
						}


						if (dollarImpactStr == "")
						{
							results["msg"] = "<strong class='error'>Dollar Impact is required</strong>";
							return Json(results);
						}

						if (decimal.TryParse(dollarImpactStr, out dollarImpact) == false)
						{
							results["msg"] = "<strong class='error'>Invalid dollar impact (must be whole/decimal)</strong>";
							return Json(results);
						}

						if (numberOfMonthsToBeActiveStr == "")
						{
							results["msg"] = "<strong class='error'>Number of months to be active is required</strong>";
							return Json(results);
						}

						if (int.TryParse(numberOfMonthsToBeActiveStr, out numberOfMonthsToBeActive) == false)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active</strong>";
							return Json(results);
						}

						if ((numberOfMonthsToBeActive <= this.max_num_of_months_to_active && numberOfMonthsToBeActive >= 1) == false)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
							return Json(results);
						}

						if (numberOfMonthsToBeActive > this.max_num_of_months_to_active)
						{
							results["msg"] = "<strong class='error'>Invalid number of months to be active (less than or equal to "+ this.max_num_of_months_to_active +" OR greater than 0 only)</strong>";
							return Json(results);
						}


						if (string.IsNullOrWhiteSpace(expectedStartDate))
						{
							results["msg"] = "<strong class='error'>Expected start date is required</strong>";
							return Json(results);
						}


						if (financeCategoryID == 0)
						{
							results["msg"] = "<strong class='error'>Finance Category is required</strong>";
							return Json(results);
						}

						var financeCategories = this.GetAllFinanceCategoryById();
						if (financeCategories.ContainsKey(financeCategoryID) == false)
						{
							results["msg"] = "<strong class='error'>Finance Category is required</strong>";
							return Json(results);
						}

					}


					int OAStatusIntParse;
					if (int.TryParse(OAStatus, out OAStatusIntParse) == false)
					{
						results["msg"] = "<strong class='error'>Invalid OA status</strong>";
						return Json(results);
					}
					if (OAStatusIntParse <= 0)
					{
						results["msg"] = "<strong class='error'>Invalid OA status</strong>";
						return Json(results);
					}


					var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalIDIntParse);

					if (this.IsUserIsFinanceApproverOnTheCurrentProposal(proposalDetails.Id) == true || this.IsUserMaster == true)
					{
						///financeApproverID
						///
						int UserFinanceIDTmp = (this.IsUserMaster == true) ? financeApproverID : int.Parse(this.UserFinanceID);

						if (Factory.ProposalFinanceApprovalRepository().UpdateProposalFinanceInfoVerification(proposalIDIntParse, UserFinanceIDTmp, remarks, isVerifiedIntParse) > 0)
						{
							string message = "";

							if (projectTypeIntParse > 0)
							{
								if (Factory.ProposalFactory().UpdateProposalProjectType(projectTypeIntParse, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't add/update project type";
								}
								else
								{
									message += "<br/> Added project type";
								}
							}

							if (dollarImpact > 0)
							{
								if (Factory.ProposalFactory().UpdateProposalDollarImpact(dollarImpact, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't update dolar impact";
								}
								else
								{
									message += "<br/> Update dollar impact";
								}
							}


							if (numberOfMonthsToBeActive > 0)
							{
								if (Factory.ProposalFactory().UpdateProposalNummberOfMonthsToBeActive(numberOfMonthsToBeActive, proposalIDIntParse) == 0)
								{
									message += "<br/> Can't update number of months to be active";
								}
								else
								{
									message += "<br/> Update number of months to be active";
								}
							}


							if (expectedStartDate != "")
							{
								DateTime expectedStartDateParsed;
								if (DateTime.TryParse(expectedStartDate, out expectedStartDateParsed))
								{
									if (Factory.ProposalFactory().UpdateProposalExpectedStartDate(expectedStartDateParsed, proposalIDIntParse) == 0)
									{
										message += "<br/> Can't add/update expected project start date";
									}
									else
									{
										message += "<br/> Added expected project start date";
									}
								}
							}


							//if (financeCategoryID > 0)
							//{
							//	if (Factory.ProposalFactory().UpdateProposalFinanceCategory(financeCategoryID, proposalIDIntParse) == 0)
							//	{
							//		message += "<br/> Can't update expected project start date";
							//	}
							//	else
							//	{
							//		message += "<br/> Added finance category";
							//	}
							//}

							#region Insert Supporting documents

							if (supportingDocsLen > 0 && (isVerifiedIntParse == 1 || isVerifiedIntParse == 3))
							{
								IDictionary<string, string> upload_results = new Dictionary<string, string>();

								List<SupportingDoc> supportingDocs = new List<SupportingDoc>();
								upload_results = new Dictionary<string, string>();
								string supportingDocsErr = "Supporting Documents (Can't upload): <br/>";

								for (int i = 0 ; i<supportingDocsLen ; i++)
								{
									HttpPostedFileBase file = Request.Files["supporting_docs_"+i];

									upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_supporting_docs"]);

									if (upload_results["done"] == "TRUE")
									{
										supportingDocs.Add(new SupportingDoc
										{
											ProposalId = proposalIDIntParse,
											ServerFileName = upload_results["newFileName"],
											OrigFileName = upload_results["origfileName"],
											AttachedBy = this.UserFullName
										});
									}
									else
									{
										supportingDocsErr += upload_results["origfileName"] + "<br/>";
									}
									//
								}

								if (supportingDocs.Count > 0)
								{
									int supportingDocsInsertedRows = Factory.SupportingDocFactory().InsertProposalSupportingDocs(supportingDocs);

									if (supportingDocsInsertedRows < supportingDocsLen)
									{
										message += supportingDocsErr;
									}
									else if (supportingDocsInsertedRows == supportingDocsLen)
									{
										message += "<br/>Supporting documents uploaded";
									}
								}

							}

							#endregion

							//string[] verificationApprovalStatusList = { "Unknown", "Verified", "Invalid", "Saved", "Closed" };

							var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);

							string proposalOwnerEmail = "";
							string ownerManagerEmail = "";

							if (proposalOwnerInfo.Count > 0)
							{
								proposalOwnerEmail = (proposalOwnerInfo.Count > 0) ? proposalOwnerInfo[0].Email : "";
								var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
								ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
							}

							int[] invalid_status_list = { (int)StaticData.OverallStatus.CANCELED,
														(int)StaticData.OverallStatus.INVALID,
														//(int)StaticData.OverallStatus.COST_AVOIDANCE,
														(int)StaticData.OverallStatus.EXISTING_PROJECT,
														(int)StaticData.OverallStatus.DUPLICATE_ENTRY };

							bool isApprovalDone = false;
							string invalidOrClosed = "";

							var neededActions = Factory.ProposalActionApproverRepository().GetProposalActionApprovers(proposalDetails.Id);
							var countClosedActionItems = neededActions.Where(a => a.ApprovalStatus == 1).Sum(a => a.Id);
							var countAllActionItems = neededActions.Sum(a => a.Id);

							if (isVerifiedIntParse == 1 && OAStatusIntParse == (int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING)
							{
								if (countClosedActionItems != countAllActionItems)
								{
									results["msg"] = "<strong class='error'>Please close all action items before moving the cost funnel evaluating</strong>";
									return Json(results);
								}

								if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING, proposalIDIntParse) > 0)
								{
									isApprovalDone = true;
									invalidOrClosed = StaticData.GetOverallStatusStr((int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING);

									// Log the new overall status
									Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
									{
										ProposalID = proposalIDIntParse,
										OAStatus = (int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING,
										ApproverFFID = this.UserFFID,
										ApproverName = this.UserFullName
									});

									// Update funell status to evaluating
									Factory.ProposalFactory().UpdateProposalFunnelStatus((int)StaticData.GlobalFunnelStatus.Evaluating, proposalIDIntParse);
								}

							}
							else if (isVerifiedIntParse == 2 && invalid_status_list.Contains(OAStatusIntParse)) // Invalid
							{
								int OAStatusIndex = Array.IndexOf(invalid_status_list, OAStatusIntParse);
								int invalid_status = invalid_status_list[OAStatusIndex];

								if (Factory.ProposalFactory().UpdateProposalStatus(invalid_status, proposalIDIntParse) > 0)
								{
									isApprovalDone = true;
									invalidOrClosed = StaticData.GetOverallStatusStr(invalid_status);

									// Log the new overall status
									Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
									{
										ProposalID = proposalIDIntParse,
										OAStatus = invalid_status,
										ApproverFFID = this.UserFFID,
										ApproverName = this.UserFullName
									});

									// Update funell status to cancelled
									Factory.ProposalFactory().UpdateProposalFunnelStatus((int)StaticData.GlobalFunnelStatus.Cancelled, proposalIDIntParse);
								}
							}
							else if (isVerifiedIntParse == 3 && OAStatusIntParse == (int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS) // Saved
							{

								if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS, proposalIDIntParse) > 0)
								{
									isApprovalDone = true;
									invalidOrClosed = StaticData.GetOverallStatusStr((int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS);

									// Log the new overall status
									Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
									{
										ProposalID = proposalIDIntParse,
										OAStatus = (int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS,
										ApproverFFID = this.UserFFID,
										ApproverName = this.UserFullName
									});

									// Update Global funnel status to Identified
									Factory.ProposalFactory().UpdateProposalFunnelStatus((int)StaticData.GlobalFunnelStatus.Identified, proposalIDIntParse);

								}

							}
							else if (isVerifiedIntParse == 4 && OAStatusIntParse == (int)StaticData.OverallStatus.ACTIVE)
							{
								if (countClosedActionItems != countAllActionItems)
								{
									results["msg"] = "<strong class='error'>Please close all action items before moving the active</strong>";
									return Json(results);
								}
								if (Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.ACTIVE, proposalIDIntParse) > 0)
								{
									isApprovalDone = true;
									invalidOrClosed = StaticData.GetOverallStatusStr((int)StaticData.OverallStatus.ACTIVE);


									// Update plannedProjectStartDate to DATE NOW when proposal's status change to ACTIVE status
									Factory.ProposalFactory().UpdateProposalPlannedProjectStartDate(DateTime.Now, proposalIDIntParse);


									// Log the new overall status
									Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
									{
										ProposalID = proposalIDIntParse,
										OAStatus = (int)StaticData.OverallStatus.ACTIVE,
										ApproverFFID = this.UserFFID,
										ApproverName = this.UserFullName
									});

									// Update funnel status to Active
									Factory.ProposalFactory().UpdateProposalFunnelStatus((int)StaticData.GlobalFunnelStatus.Active, proposalIDIntParse);
								}
							}

							if (isApprovalDone == true)
							{
								results["done"] = "FALSE";
								results["msg"] = "<strong class='good'>Successfully move to <i>"+ invalidOrClosed +"</i> the status!</strong>" + message;


								string emailMsg = "";
								emailMsg = string.Format(@"E-Savings Ticket #{0}. {1} by {2}., Please click the link below to view the details <br/>
											<a href='{3}/Home/Details/{4}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{5}</td></tr>
												<tr><td>Project Title</td><td>{6}</td></tr>
												<tr><td>Current Description</td><td>{7}</td></tr>
												<tr><td>Proposal Description</td><td>{8}</td></tr>
												<tr><td>Proposed By</td><td>{9}</td></tr>
												<tr><td>Department</td><td>{10}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{11}</td></tr>
											</table>",
												 proposalDetails.ProposalTicket,
												 invalidOrClosed,
												 this.UserFullName,
												 this.base_url,
												 proposalID,
												 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
												 proposalDetails.ProjectTitle,
												 proposalDetails.CurrentDescription,
												 proposalDetails.ProposalDescription,
												 proposalDetails.SubmittedBy,
												 proposalDetails.EmpDeptCode,
												 proposalDetails.AreaDeptBeneficiary);

								Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings-"+ invalidOrClosed +"-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

								sendEmail.Add_To_Recipient(proposalOwnerEmail);
								sendEmail.Add_To_Recipient(ownerManagerEmail);
								sendEmail.Add_CC_Recipient(this.UserEmail);// current user: finance approver


								try
								{
									await sendEmail.send();
								}
								catch (Exception ex)
								{
									results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
								}

								return Json(results);
							}

						}
						else
						{
							results["done"] = "FALSE";
							results["msg"] = "<strong class='error'>Can't verify this proposal</strong>";
						}
					}
					else
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}


				}
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}


			return Json(results);
		}





		public async Task<JsonResult> CreateNewAction (int proposalID, string actionDesc, string ownerFFID, string ownerFullname)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Internal error, kindly report this error.</strong>";

			try
			{
				#region authentication

				if (!IsUserSuccessfullyLoggedIn())
				{
					results["msg"] = "<strong class='error'>Please login...</strong>";
					return Json(results);
				}

				if (this.UserType == ((int)StaticData.UserTypes.Client).ToString())
				{
					results["msg"] = "<strong class='error'>Permission Denied...</strong>";
					return Json(results);
				}

				#endregion

				if (string.IsNullOrEmpty(proposalID.ToString()) || string.IsNullOrEmpty(actionDesc) || string.IsNullOrEmpty(ownerFFID) || string.IsNullOrEmpty(ownerFullname))
				{
					results["msg"] = "<strong class='error'>Please complete the requirements, (Action Description and action owner info)</strong>";
					return Json(results);
				}


				var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

				if (proposalDetails == null)
				{
					results["msg"] = "<strong class='error'>Proposal not found</strong>";
					return Json(results);
				}


				if (this.IsUserMaster == false) // not a master approver
				{
					if (this.IsUserIsCostAnalystOnTheCurrentProposal(proposalDetails.AreaDeptBeneficiary) == false && this.IsUserIsFinanceApproverOnTheCurrentProposal(proposalID) == false)
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}
				}


				//List<ProposalCostAnalyst> proposal_costAnalyst = new List<ProposalCostAnalyst>();

				//if (int.Parse(this.UserType) == (int)StaticData.UserTypes.CostAnalyst)
				//{
				//	proposal_costAnalyst = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystInfoAndVerificationInfo(proposalID);

				//	if (proposal_costAnalyst.Count <= 0)
				//	{
				//		results["msg"] = "<strong class='error'>No cost analyst approver, please contact site admin to fix this error.</strong>";
				//		return Json(results);
				//	}
				//}

				var newAction = new ProposalAction()
				{
					ProposalID = proposalID,
					NeededAction = actionDesc,
					OwnerFFID = ownerFFID,
					OwnerFullname = ownerFullname
				};

				int actionID = Factory.ProposalActionRepository().Add(newAction);



				if (actionID > 0)
				{
					results["done"] = "TRUE";
					results["msg"] = "<strong class='good'>Action successfully assign to action owner</strong>";

					// Cost Analyst as approver
					//if (int.Parse(this.UserType) == (int)StaticData.UserTypes.CostAnalyst)
					//{
					//	ProposalActionApprover approver;

					//	foreach (var costAnalyst in proposal_costAnalyst)
					//	{
					//		approver = new ProposalActionApprover()
					//		{
					//			ProposalID = proposalID,
					//			ActionID = actionID,
					//			ApproverFFID = costAnalyst.CostAnalystInfo.FFID,
					//			ApproverFullname = costAnalyst.CostAnalystInfo.FullName
					//		};

					//		Factory.ProposalActionApproverRepository().Add(approver);
					//	}
					//}

					// Finance as approver
					//if (int.Parse(this.UserType) == (int)StaticData.UserTypes.Finance)
					//{
					//}

					// Cost analyst and Finance as approver (or the master) - the user who assign the action item is the approver
					ProposalActionApprover approver = new ProposalActionApprover()
					{
						ProposalID = proposalID,
						ActionID = actionID,
						ApproverFFID = this.UserFFID,
						ApproverFullname = this.UserFullName
					};

					Factory.ProposalActionApproverRepository().Add(approver);



					// Change the status in COST_ANALYST_REVIEW_IN_PROGRESS when cost analyst assign and current status is PROJECT_PROPOSAL
					if (proposalDetails.OAStatus == (int)StaticData.OverallStatus.PROJECT_PROPOSAL && this.UserType == ((int)StaticData.UserTypes.CostAnalyst).ToString())
					{
						Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS, proposalID);

						// Log the new overall status
						Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
						{
							ProposalID = proposalID,
							OAStatus = (int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS,
							ApproverFFID = this.UserFFID,
							ApproverName = this.UserFullName
						});
					}


					if (proposalDetails.OAStatus == (int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED && this.UserType == ((int)StaticData.UserTypes.Finance).ToString())
					{
						Factory.ProposalFactory().UpdateProposalStatus((int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS, proposalID);

						// Log the new overall status
						Factory.ProposalStatusLogRepository().Add(new ProposalStatusLog()
						{
							ProposalID = proposalID,
							OAStatus = (int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS,
							ApproverFFID = this.UserFFID,
							ApproverName = this.UserFullName
						});
					}


					#region Email notification



					var actionOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, ownerFFID);
					var actionOwnerEmail = (actionOwnerInfo.Count > 0) ? actionOwnerInfo[0].Email : "";

					var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);

					string proposalOwnerEmail = "";
					string ownerManagerEmail = "";

					if (proposalOwnerInfo.Count > 0)
					{
						proposalOwnerEmail = (proposalOwnerInfo.Count > 0) ? proposalOwnerInfo[0].Email : "";
						var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
						ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
					}

					string emailMsg = string.Format(@"Action Needed from E-Savings Ticket #{0} assign to you. Please click the link below to view the details <br/>
											<a href='{1}/Home/Details/{2}'>Details</a>
											<table>
												<tr><td>Project Type</td><td>{3}</td></tr>
												<tr><td>Project Title</td><td>{4}</td></tr>
												<tr><td>Current Description</td><td>{5}</td></tr>
												<tr><td>Proposal Description</td><td>{6}</td></tr>
												<tr><td>Proposed By</td><td>{7}</td></tr>
												<tr><td>Department</td><td>{8}</td></tr>
												<tr><td>Department/Area beneficiary</td><td>{9}</td></tr>
											</table>",
												 proposalDetails.ProposalTicket,
												 this.base_url,
												 proposalID,
												 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
												 proposalDetails.ProjectTitle,
												 proposalDetails.CurrentDescription,
												 proposalDetails.ProposalDescription,
												 proposalDetails.SubmittedBy,
												 proposalDetails.EmpDeptCode,
												 proposalDetails.AreaDeptBeneficiary);

					Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "Action Needed from E-Savings-Ticket#" + proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);
					sendEmail.Add_To_Recipient(actionOwnerEmail);
					sendEmail.Add_CC_Recipient(proposalOwnerEmail);
					sendEmail.Add_CC_Recipient(ownerManagerEmail);
					sendEmail.Add_CC_Recipient(this.UserEmail);


					try
					{
						await sendEmail.send();
					}
					catch (Exception ex)
					{
						results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
					}

					#endregion



				}
				else
				{
					results["msg"] = "<strong class='error'>Failed to add new action, please try again</strong>";
				}

			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}




			return Json(results);
		}



		public async Task<JsonResult> SaveActionOwnerResponse (int proposalID, int actionID, string remarks, string supportingDocsLen)
		{

			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Internal error, kindly report this error.</strong>";

			try
			{

				#region Validation and authentication
				if (string.IsNullOrEmpty(proposalID.ToString()) || string.IsNullOrEmpty(actionID.ToString()) || string.IsNullOrEmpty(remarks) || string.IsNullOrEmpty(supportingDocsLen))
				{
					results["msg"] = "<strong class='error'>Please complete the requirements, (Remarks and supporting documents)</strong>";
					return Json(results);
				}

				if (!IsUserSuccessfullyLoggedIn())
				{
					results["msg"] = "<strong class='error'>Please login...</strong>";
					return Json(results);
				}
				#endregion
				int supporting_docs_len;
				if (!int.TryParse(supportingDocsLen, out supporting_docs_len))
				{
					results["msg"] = "<strong class='error'>Invalid supporting documents length</strong>";
					return Json(results);
				}

				var proposalNeededActions = Factory.ProposalActionRepository().GetProposalActionWithApprovers(proposalID, actionID);

				if (this.UserFFID == proposalNeededActions.OwnerFFID)
				{
					proposalNeededActions.OwnerRemarks = remarks;

					if (Factory.ProposalActionRepository().UpdateProposalActionOwnerResponse(proposalNeededActions) > 0)
					{

						results["done"] = "TRUE";
						results["msg"] = "<strong class='good'>Successfully saved! <br/></strong>";

						#region Insert Supporting documents

						IDictionary<string, string> upload_results = new Dictionary<string, string>();

						List<SupportingDoc> supportingDocs = new List<SupportingDoc>();
						upload_results = new Dictionary<string, string>();
						string supportingDocsErr = "Supporting Documents (Can't upload): <br/>";

						for (int i = 0 ; i<supporting_docs_len ; i++)
						{
							HttpPostedFileBase file = Request.Files["supporting_docs_"+i];

							upload_results = this.UploadThisFile(file, ConfigurationManager.AppSettings["dir_for_upload_supporting_docs"]);

							if (upload_results["done"] == "TRUE")
							{
								supportingDocs.Add(new SupportingDoc
								{
									ProposalId = proposalID,
									ServerFileName = upload_results["newFileName"],
									OrigFileName = upload_results["origfileName"],
									AttachedBy = this.UserFullName
								});
							}
							else
							{
								supportingDocsErr += upload_results["origfileName"] +" - "+ upload_results["msg"] + "<br/>";
							}
							//
						}


						if (supportingDocs.Count > 0)
						{
							int supportingDocsInsertedRows = Factory.SupportingDocFactory().InsertProposalSupportingDocs(supportingDocs);

							if (supportingDocsInsertedRows < supporting_docs_len)
							{
								results["msg"] += supportingDocsErr;
							}
						}

						#endregion

						#region Email notification

						var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);
						proposalDetails.CostAnalysts = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystInfoAndVerificationInfo(proposalID);
						// TODO: Enable this block of code if the initial approval work flow is being brought back
						//proposalDetails.Managers = this.GetProposalManagers(proposalID);

						if (proposalDetails != null)
						{

							var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);
							var proposalOwnerEmail = "";

							if (proposalOwnerInfo.Count > 0)
							{
								proposalOwnerEmail = (proposalOwnerInfo.Count > 0) ? proposalOwnerInfo[0].Email : "";
								var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
								string ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";


								string emailMsg = string.Format(@"E-Savings Ticket #{0} Action item responded by action Owner [{1}]. Please click the link below to view the details <br/>
																	<a href='{2}/Home/Details/{3}'>Details</a>
																	<table>
																		<tr><td>Project Type</td><td>{4}</td></tr>
																		<tr><td>Project Title</td><td>{5}</td></tr>
																		<tr><td>Current Description</td><td>{6}</td></tr>
																		<tr><td>Proposal Description</td><td>{7}</td></tr>
																		<tr><td>Proposed By</td><td>{8}</td></tr>
																		<tr><td>Department</td><td>{9}</td></tr>
																		<tr><td>Department/Area beneficiary</td><td>{10}</td></tr>
																	</table>",
													 proposalDetails.ProposalTicket,
													 this.UserFullName,
													 this.base_url,
													 proposalID,
													 StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
													 proposalDetails.ProjectTitle,
													 proposalDetails.CurrentDescription,
													 proposalDetails.ProposalDescription,
													 proposalDetails.SubmittedBy,
													 proposalDetails.EmpDeptCode,
													 proposalDetails.AreaDeptBeneficiary);

								Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "Action Needed from E-Savings-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);
								sendEmail.Add_CC_Recipient(proposalOwnerEmail);
								sendEmail.Add_CC_Recipient(ownerManagerEmail);

								foreach (var approver in proposalNeededActions.Approvers)
								{
									var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, approver.ApproverFFID);
									if (empInfo.Count > 0)
									{
										sendEmail.Add_To_Recipient(empInfo[0].Email);
									}
								}


								foreach (var costAnalyst in proposalDetails.CostAnalysts)
								{
									var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, costAnalyst.CostAnalystInfo.FFID);
									if (empInfo.Count > 0)
									{
										sendEmail.Add_To_Recipient(empInfo[0].Email);
									}

								}

								try
								{
									await sendEmail.send();
								}
								catch (Exception ex)
								{
									results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
								}
							}

						}

						#endregion

					}

				}
				else
				{
					results["msg"] = "<strong class='error'>Permission Denied...</strong>";
					return Json(results);
				}


			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}



			return Json(results);

		}


		public async Task<JsonResult> ProposalActionApproval (int proposalID, int actionID, string verifierRemarks, int verificationStatus)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Internal error, kindly report this error.</strong>";

			try
			{

				#region authentication

				if (!IsUserSuccessfullyLoggedIn())
				{
					results["msg"] = "<strong class='error'>Please login...</strong>";
					return Json(results);
				}

				if (this.UserType == ((int)StaticData.UserTypes.Client).ToString())
				{
					results["msg"] = "<strong class='error'>Permission Denied...</strong>";
					return Json(results);
				}

				#endregion


				if (string.IsNullOrEmpty(proposalID.ToString()) || string.IsNullOrEmpty(actionID.ToString()) || string.IsNullOrEmpty(verifierRemarks) || string.IsNullOrEmpty(verificationStatus.ToString()))
				{
					results["msg"] = "<strong class='error'>Please complete the requirements, (Remarks and verification status)</strong>";
					return Json(results);
				}

				int verificationStatusParse;
				if (!int.TryParse(verificationStatus.ToString(), out verificationStatusParse))
				{
					results["msg"] = "<strong class='error'>Invalid verification status indicator...</strong>";
					return Json(results);
				}

				if (verificationStatusParse != 1 && verificationStatusParse != 2)
				{
					results["msg"] = "<strong class='error'>Invalid verification status indicator...</strong>";
					return Json(results);
				}

				var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

				if (proposalDetails == null)
				{
					results["msg"] = "<strong class='error'>Proposal entry not found!</strong>";
					return Json(results);
				}


				var proposalNeededActions = Factory.ProposalActionRepository().GetProposalActionWithApprovers(proposalID, actionID);


				if (proposalNeededActions != null)
				{

					#region Email notification needed data
					var ownerFFID = proposalNeededActions.OwnerFFID;
					var actionOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, ownerFFID);
					var actionOwnerEmail = (actionOwnerInfo.Count > 0) ? actionOwnerInfo[0].Email : "";

					//var costSiteInChargeInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, this.costSiteInChargeFFID);
					//var costSiteInChargeInfoEmail = (costSiteInChargeInfo.Count > 0) ? costSiteInChargeInfo[0].Email : "";

					var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);
					string proposalOwnerEmail = "";
					string ownerManagerEmail = "";

					if (proposalOwnerInfo.Count > 0)
					{
						proposalOwnerEmail = proposalOwnerInfo[0].Email;
						var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
						ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
					}

					#endregion

					var approvers = proposalNeededActions.Approvers;
					foreach (var approver in approvers)
					{
						if (approver.ApproverFFID == this.UserFFID || this.IsUserMaster == true)
						{
							// Update action owner data here
							approver.Remarks = verifierRemarks;
							approver.ApprovalStatus = verificationStatusParse;

							if (Factory.ProposalActionApproverRepository().UpdateActionsApproverVerification(approver) > 0)
							{
								results["done"] = "TRUE";
								results["msg"] = "<strong class='good'>Submitted successfully!</strong>";



								#region mark as Closed the action
								int numberOfApproversInCurrentAction = approvers.Sum(x => x.Id);
								int numberOfApproversApprovedInCurrentAction = approvers.Where(a => a.ApprovalStatus == 1).Sum(a => a.Id);
								if (numberOfApproversInCurrentAction == numberOfApproversApprovedInCurrentAction)
								{
									// Close the action
									proposalNeededActions.IsClosed = 1;
									Factory.ProposalActionRepository().UpdateProposalActionOwnerResponse(proposalNeededActions);
								}

								#endregion




								//#region Check if all action items and approvers are already approved
								//var allNeededActions = Factory.ProposalActionRepository().GetProposalActionsWithApprovers(proposalID);
								//bool isAllActionsIsVerifiedDone = false;
								//// check if all approvers in every action is already approved
								//foreach (var action in allNeededActions)
								//{


								//	int numberOfApprovers = action.Approvers.Sum(x => x.Id);
								//	var numberOfApproversApproved = action.Approvers.ToList()
								//									.Where(a => a.ApprovalStatus == 1)
								//									.Sum(a => a.Id);
								//	if (numberOfApprovers == numberOfApproversApproved)
								//	{
								//		isAllActionsIsVerifiedDone = true; // true until the last action
								//	}
								//	else
								//	{
								//		isAllActionsIsVerifiedDone = false;
								//		break;
								//	}


								//}
								//#endregion




								#region Email notification

								string[] verificationApprovalStatusList = { "Unknown", "Verified closed", "Verify not closed" };

								// Notify the action owner
								string emailMsg = string.Format(@"E-Savings Ticket #{0}, {1} by {2} . Please click the link below to view the details <br/>
													<a href='{3}/Home/Details/{4}'>Details</a>
													<table>
														<tr><td>Project Type</td><td>{5}</td></tr>
														<tr><td>Project Title</td><td>{6}</td></tr>
														<tr><td>Current Description</td><td>{7}</td></tr>
														<tr><td>Proposal Description</td><td>{8}</td></tr>
														<tr><td>Proposed By</td><td>{9}</td></tr>
														<tr><td>Department</td><td>{10}</td></tr>
														<tr><td>Department/Area beneficiary</td><td>{11}</td></tr>
													</table>",
														proposalDetails.ProposalTicket,
														verificationApprovalStatusList[verificationStatusParse],
														this.UserFullName,
														this.base_url,
														proposalID,
														StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
														proposalDetails.ProjectTitle,
														proposalDetails.CurrentDescription,
														proposalDetails.ProposalDescription,
														proposalDetails.SubmittedBy,
														proposalDetails.EmpDeptCode,
														proposalDetails.AreaDeptBeneficiary);

								Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, verificationApprovalStatusList[verificationStatusParse]+" E-Savings-Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);
								sendEmail.Add_To_Recipient(actionOwnerEmail);
								sendEmail.Add_CC_Recipient(ownerManagerEmail);
								sendEmail.Add_CC_Recipient(proposalOwnerEmail);
								sendEmail.Add_CC_Recipient(this.UserEmail);

								try
								{
									await sendEmail.send();
								}
								catch (Exception ex)
								{
									results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
								}

								#endregion
							}
							else
							{
								results["msg"] = "<strong class='error'>Can't update verification results</strong>";
							}

							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}

			return Json(results);
		}


		public async Task<JsonResult> ReAssignProjectCostAnalyst (int proposalID, int currentCostAnalystID, string newCostAnalystFFID, string remarks)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";

			try
			{
				if (IsUserSuccessfullyLoggedIn())
				{

					int UserCostAnalystIDTmp = (this.IsUserMaster == true) ? currentCostAnalystID : int.Parse(this.UserCostAnalystID);

					if (this.UserType == ((int)StaticData.UserTypes.Client).ToString())
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}

					var costAnalystApprovalResults = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystVerificationResults(proposalID, UserCostAnalystIDTmp);
					if (costAnalystApprovalResults == null)
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}


					var costAnalyst = Factory.CostAnalystFactory().GetInfoByFFID(newCostAnalystFFID);
					// insert new cost analyst
					if (Factory.ProposalCostAnalystRepository().Add(new ProposalCostAnalyst()
					{
						ProposalID = proposalID,
						CostAnalystID = costAnalyst.Id
					}) > 0)
					{
						Factory.ProposalCostAnalystRepository().DeleteProposalCostAnalyst(proposalID, UserCostAnalystIDTmp, "Reassigned by "+ this.UserFullName +" -> " + remarks);


						results["done"] = "TRUE";
						results["msg"] = "<strong class='good'>Successfully assigned new cost-analyst</strong>";

						var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);


						var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);
						string proposalOwnerEmail = "";
						string ownerManagerEmail = "";

						if (proposalOwnerInfo.Count > 0)
						{
							proposalOwnerEmail = proposalOwnerInfo[0].Email;
							var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
							ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
						}

						string emailMsg = string.Format(@"E-Savings Ticket #{0}, is re-assign to you as cost-analyst approver by {1}. Please click the link below to view the details <br/>
												<a href='{2}/Home/Details/{3}'>Details</a>
												<table>
													<tr><td>Project Type</td><td>{4}</td></tr>
													<tr><td>Project Title</td><td>{5}</td></tr>
													<tr><td>Current Description</td><td>{6}</td></tr>
													<tr><td>Proposal Description</td><td>{7}</td></tr>
													<tr><td>Proposed By</td><td>{8}</td></tr>
													<tr><td>Department</td><td>{9}</td></tr>
													<tr><td>Department/Area beneficiary</td><td>{10}</td></tr>
												</table>
												<br/>
												<b>{1}'s remarks: </b> {11}
											",
											proposalDetails.ProposalTicket,
											this.UserFullName,
											this.base_url,
											proposalID,
											StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
											proposalDetails.ProjectTitle,
											proposalDetails.CurrentDescription,
											proposalDetails.ProposalDescription,
											proposalDetails.SubmittedBy,
											proposalDetails.EmpDeptCode,
											proposalDetails.AreaDeptBeneficiary,
											remarks);

						Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

						var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, costAnalyst.FFID);
						if (empInfo.Count > 0)
						{
							foreach (var emp in empInfo)
							{
								sendEmail.Add_To_Recipient(emp.Email);
							}
						}
						sendEmail.Add_CC_Recipient(this.UserEmail);
						sendEmail.Add_CC_Recipient(proposalOwnerEmail);
						sendEmail.Add_CC_Recipient(ownerManagerEmail);

						try
						{
							await sendEmail.send();
						}
						catch (Exception ex)
						{
							results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
						}
						return Json(results);
					}
					else
					{
						results["done"] = "FALSE";
						results["msg"] = "<strong class='error'>Error on assigning new cost-analyst, please contact site admin</strong>";
					}
				}
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}

			return Json(results);

		}


		public async Task<JsonResult> ReAssignProjectFinace (int proposalID, int currentFinanceID, string newFinanceFFID, string remarks)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Please login...</strong>";

			try
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					int UserFinanceIDTmp = (this.IsUserMaster == true) ? currentFinanceID : int.Parse(this.UserFinanceID);

					if (this.UserType == ((int)StaticData.UserTypes.Client).ToString())
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}

					var financeApprovalResults = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoVerificationResults(proposalID, UserFinanceIDTmp);
					if (financeApprovalResults == null)
					{
						results["msg"] = "<strong class='error'>Permission Denied...</strong>";
						return Json(results);
					}

					var newFinance = Factory.FinanceApproverFactory().GetInfoByFFID(newFinanceFFID);


					var proposalFinanceApproval = new ProposalFinanceApproval()
					{
						ProposalID = proposalID,
						FinanceID = newFinance.Id,
						Remarks = "na"
					};

					if (Factory.ProposalFinanceApprovalRepository().Add(proposalFinanceApproval) > 0)
					{
						Factory.ProposalFinanceApprovalRepository().DeleteProposalFinance(proposalID, currentFinanceID, "Reassigned by "+ this.UserFullName +" -> " + remarks);

						results["done"] = "TRUE";
						results["msg"] = "<strong class='good'>Successfully assigned new cost-analyst</strong>";

						var proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

						var proposalOwnerInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, proposalDetails.EmpFFID);
						string proposalOwnerEmail = "";
						string ownerManagerEmail = "";

						if (proposalOwnerInfo.Count > 0)
						{
							proposalOwnerEmail = proposalOwnerInfo[0].Email;
							var empDirectSupv = Helpers.ONEmployeesLDAP.GetEmployeeInfo(ldapAddress, proposalOwnerInfo[0].ManagerFFID);
							ownerManagerEmail = (empDirectSupv.Email != null) ? empDirectSupv.Email : "";
						}

						string emailMsg = string.Format(@"E-Savings Ticket #{0}, is re-assign to you as finance approver by {1}. Please click the link below to view the details <br/>
												<a href='{2}/Home/Details/{3}'>Details</a>
												<table>
													<tr><td>Project Title</td><td>{4}</td></tr>
													<tr><td>Project Title</td><td>{5}</td></tr>
													<tr><td>Current Description</td><td>{6}</td></tr>
													<tr><td>Proposal Description</td><td>{7}</td></tr>
													<tr><td>Proposed By</td><td>{8}</td></tr>
													<tr><td>Department</td><td>{9}</td></tr>
													<tr><td>Department/Area beneficiary</td><td>{10}</td></tr>
												</table>
												<br/>
												<b>{1}'s remarks: </b> {11}
											",
											proposalDetails.ProposalTicket,
											this.UserFullName,
											this.base_url,
											proposalID,
											StaticData.GetProjectTypeStr(proposalDetails.ProjectType),
											proposalDetails.ProjectTitle,
											proposalDetails.CurrentDescription,
											proposalDetails.ProposalDescription,
											proposalDetails.SubmittedBy,
											proposalDetails.EmpDeptCode,
											proposalDetails.AreaDeptBeneficiary,
											remarks);

						Helpers.SendEmail sendEmail = new Helpers.SendEmail(emailMsg, "E-Savings Ticket#"+proposalDetails.ProposalTicket, this.emailMsgFooter, this.emailSenderName, this.emailSenderEmail, this.emailDefaultRecipient);

						var empInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, newFinance.FFID);
						if (empInfo.Count > 0)
						{
							foreach (var emp in empInfo)
							{
								sendEmail.Add_To_Recipient(emp.Email);
							}
						}

						sendEmail.Add_CC_Recipient(this.UserEmail);
						sendEmail.Add_CC_Recipient(proposalOwnerEmail);
						sendEmail.Add_CC_Recipient(ownerManagerEmail);

						try
						{
							await sendEmail.send();
						}
						catch (Exception ex)
						{
							results["msg"] += "<br/><span class='error'>" + ex.Message + "</span>";
						}
						return Json(results);

					}
					else
					{
						results["done"] = "FALSE";
						results["msg"] = "<strong class='error'>Error on assigning new finance, please contact site admin</strong>";
					}

				}
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}



			return Json(results);
		}

	}
}
