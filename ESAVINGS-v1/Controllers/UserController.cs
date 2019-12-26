using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESavingsFactory;
using Domain;
using System.Configuration;
using System.DirectoryServices;
using Helpers;

namespace ESAVINGS_v1.Controllers
{
	public static class StringExtensions
	{

		// defines a String extension method 
		// which includes a StringComparison parameter 
		public static bool Contains (this String str,
									String substr,
									StringComparison cmp)
		{
			if (substr == null)
				throw new ArgumentNullException("substring substring",
												" cannot be null.");

			else if (!Enum.IsDefined(typeof(StringComparison), cmp))
				throw new ArgumentException("comp is not a member of",
											"StringComparison, comp");

			return str.IndexOf(substr, cmp) >= 0;
		}
	}

	public class UserController : BaseController
	{
		public ActionResult Logout ()
		{
			Session.Abandon();

			return RedirectToAction("Index", "Home");
		}


		public ActionResult UserLogin ()
		{
			if (IsUserSuccessfullyLoggedIn())
			{
				return RedirectToAction("MyProfile", "Home");
			}

			GetUserDataCookiesViewBag();

			ViewBag.DefaultPassword = this.GetDefaultPassword();
			//else
			//{
			//	//this.SetUserCookiesToSession();

			//	//if (IsUserSuccessfullyLoggedIn())
			//	//{
			//	//	return RedirectToAction("Proposal", "Home");
			//	//}
			//}

			return View();
		}

		public JsonResult SearchEmployee (string searchStr)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["msg"] = "<strong class='error'>Can't connect to Active Directory server (LDAP)</strong>";

			try
			{
				var employeeInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, searchStr);

				return Json(employeeInfo);
			}
			catch (Exception ex)
			{
				results["msg"] = ex.Message;
			}

			return Json(results);
		}

		[HttpGet]
		public ActionResult ChangeCurrentUserType ()
		{
			if (this.IsUserSuccessfullyLoggedIn() == true)
			{
				if (this.IsUserMultiPermission == true)
				{
					bool isMultiPermission = false;

					var financeInfo = Factory.FinanceApproverFactory().GetInfoByFFID(this.UserFFID);
					var costAnalystInfo = Factory.CostAnalystFactory().GetInfoByFFID(this.UserFFID);

					if (costAnalystInfo != null && financeInfo != null)
					{
						isMultiPermission = true;
					}

					if (int.Parse(this.UserType) == (int)StaticData.UserTypes.CostAnalyst)
					{
						// Change to finance permission
						if (financeInfo != null)
						{
							if (financeInfo.FFID != null)
							{
								var userSession = new UserSession()
								{
									ffid =  this.UserFFID,
									fullName = this.UserFullName,
									email = this.UserEmail,
									type = StaticData.UserTypes.Finance,
									department = this.UserDepartment,
									FinanceID = financeInfo.Id.ToString(),
									isMaster = financeInfo.IsMaster == 1 ? true : false,
									isMultiPermission = isMultiPermission
								};
								this.SetUserSession(userSession);

							}
						}
					}
					else if (int.Parse(this.UserType) == (int)StaticData.UserTypes.Finance)
					{
						// Change to cost-analyst permission
						if (costAnalystInfo != null)
						{
							if (costAnalystInfo.FFID != null)
							{
								var userSession = new UserSession()
								{
									ffid =  this.UserFFID,
									fullName = this.UserFullName,
									email = this.UserEmail,
									type = StaticData.UserTypes.CostAnalyst,
									department = this.UserDepartment,
									CostAnalystID = costAnalystInfo.Id.ToString(),
									isMaster = costAnalystInfo.IsMaster == 1 ? true : false,
									isMultiPermission = isMultiPermission
								};
								this.SetUserSession(userSession);
							}
						}

					}
				}

			}

			return RedirectToAction("MyProfile", "Home");
		}


		[HttpPost]
		[ValidateInput(true)]
		public JsonResult LoginAction (string ffID, string password, string isDirectLabor="", int isRememberUser=0)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Can't connect to Active Directory server (LDAP)</strong>";

			try
			{

				var userSession = new UserSession();
				//ffID, Helpers.Hashing.GetSHA512(password)
				OperatorUser user = Factory.OperatorFactory().Login(new OperatorUser()
				{
					FFID = ffID,
					Password = Helpers.Hashing.GetSHA512(password)
				});

				if (user != null)
				{
					// get operator department
					var employeeInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, ffID);
					string deptCodeTmp = (employeeInfo != null) ? deptCodeTmp = employeeInfo[0].Department : "";

					userSession = new UserSession()
					{
						ffid =  user.FFID,
						fullName = employeeInfo[0].DisplayName,
						email = "",
						type = StaticData.UserTypes.Client,
						department = deptCodeTmp,
						isMaster = false,
						isMultiPermission = false,
						IsDL = true
					};
				}
				else
				{

					var employeeInfo = Helpers.ONEmployeesLDAP.SignIn(this.ldapAddress, ffID, password);

					// Check if employee is IDL or DL
					//string DepartmentNumberTmp = employeeInfo.DepartmentNumber;
					//StringComparison strCompIgnoreCase = StringComparison.OrdinalIgnoreCase;
					//bool isDL = DepartmentNumberTmp.Contains("DL", strCompIgnoreCase);

					bool isDL = (isDirectLabor == "DL") ? true : false;


					bool doneSearchingValidator = false;
					bool isMultiPermission = false;

					var costAnalystInfo = Factory.CostAnalystFactory().GetInfoByFFID(employeeInfo.FFID);
					var financeInfo = Factory.FinanceApproverFactory().GetInfoByFFID(employeeInfo.FFID);

					if (costAnalystInfo != null && financeInfo != null)
					{
						isMultiPermission = true;
					}

					// Cost Analyst user
					if (costAnalystInfo != null)
					{
						if (costAnalystInfo.FFID != null)
						{
							userSession = new UserSession()
							{
								ffid =  employeeInfo.FFID,
								fullName = employeeInfo.DisplayName,
								email = employeeInfo.Email,
								type = StaticData.UserTypes.CostAnalyst,
								department = employeeInfo.Department,
								CostAnalystID = costAnalystInfo.Id.ToString(),
								isMaster = costAnalystInfo.IsMaster == 1 ? true : false,
								isMultiPermission = isMultiPermission,
								mgrFFID = employeeInfo.ManagerFFID,
								IsDL=isDL
							};

							doneSearchingValidator = true;
						}
					}


					if (doneSearchingValidator == false)
					{

						if (financeInfo != null)
						{
							if (financeInfo.FFID != null)
							{
								userSession = new UserSession()
								{
									ffid =  employeeInfo.FFID,
									fullName = employeeInfo.DisplayName,
									email = employeeInfo.Email,
									type = StaticData.UserTypes.Finance,
									department = employeeInfo.Department,
									FinanceID = financeInfo.Id.ToString(),
									isMaster = financeInfo.IsMaster == 1 ? true : false,
									isMultiPermission = isMultiPermission,
									mgrFFID = employeeInfo.ManagerFFID,
									IsDL=isDL
								};

								doneSearchingValidator = true;
							}
						}
					}

					// Normal
					if (doneSearchingValidator == false)
					{
						userSession = new UserSession()
						{
							ffid = employeeInfo.FFID,
							fullName = employeeInfo.DisplayName,
							email = employeeInfo.Email,
							type = StaticData.UserTypes.Client,
							department = employeeInfo.Department,
							isMaster = false,
							isMultiPermission = false,
							mgrFFID = employeeInfo.ManagerFFID,
							IsDL=isDL
						};

					}



				}

				if (this.SetUserSession(userSession))
				{

					if (isRememberUser == 1)
					{
						userSession.password = password;
						this.SetUserCookies(userSession);
					}
					else
					{
						this.UnSetUserCookies();
					}

					results["done"] = "TRUE";
					results["msg"] = "<strong class='good'>Successfully Logged In</strong>";

				}
				else
				{
					results["done"] = "FALSE";
					results["msg"] = "<strong class='error'>Login failed</strong>";

				}

			}
			catch (Exception ex)
			{
				//results["done"] = "FALSE";
				//results["msg"] = "<strong class='error'>"+ ex.Message +"</strong>";

				//throw new System.Exception(ex.Message);

				results["msg"] = ex.Message;
			}




			return Json(results);

		}



		private string GetDefaultPassword ()
		{
			string defaultPassBase = "Welcome";
			string curYear = DateTime.Now.Year.ToString();
			return defaultPassBase + curYear;
		}


		public JsonResult RegisterOrResetPassword (string ffID)
		{
			IDictionary<string, string> results = new Dictionary<string, string>();
			results["done"] = "FALSE";
			results["msg"] = "<strong class='error'>Internal error</strong>";

			try
			{
				if (string.IsNullOrEmpty(ffID))
				{
					results["msg"] = "<strong class='error'>Please enter your ffid</strong>";
					return Json(results);
				}

				var employeeInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, ffID);
				if (employeeInfo.Count == 0)
				{
					results["msg"] = "<strong class='error'>Your FFID is not existing in AD account server</strong>";
					return Json(results);
				}

				string defPass = this.GetDefaultPassword();

				OperatorUser operatorInfo = new OperatorUser()
				{
					FFID = ffID,
					Password = Hashing.GetSHA512(defPass).ToUpper()
				};

				OperatorUser user = Factory.OperatorFactory().Login(operatorInfo);

				if (user == null)
				{
					if (Factory.OperatorFactory().Add(operatorInfo) > 0)
					{
						results["done"] = "TRUE";
						results["msg"] = "<strong class='good'>You have successfully registered. Your default password is <b>"+ defPass +"</b></strong>";
					}
					else
					{
						results["done"] = "FALSE";
						results["msg"] = "<strong class='error'>Failed to register</strong>";
					}
				}
				else
				{
					if (Factory.OperatorFactory().ChangePassword(operatorInfo) > 0)
					{
						results["done"] = "TRUE";
						results["msg"] = "<strong class='good'>You have successfully reset your password. Your default password is "+ defPass +"</strong>";
					}
					else
					{
						results["done"] = "FALSE";
						results["msg"] = "<strong class='error'>Failed to reset your password</strong>";
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
