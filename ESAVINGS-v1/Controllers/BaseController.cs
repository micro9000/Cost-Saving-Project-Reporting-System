using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using System.IO;
using ESavingsFactory;
using System.Threading;
using System.Globalization;


namespace ESAVINGS_v1.Controllers
{

	public class UserSession
	{
		public string ffid
		{
			get;
			set;
		}
		public string fullName
		{
			get;
			set;
		}
		public string email
		{
			get;
			set;
		}

		public string department
		{
			get;
			set;
		}

		public StaticData.UserTypes type
		{
			get;
			set;
		}

		public bool isMaster
		{
			get;
			set;
		}

		public bool isMultiPermission
		{
			get;
			set;
		}

		public string mgrFFID
		{
			get;
			set;
		}

		private string _password = "";
		public string password
		{
			get
			{
				return this._password;
			}

			set
			{
				this._password = value;
			}
		}

		private string costAnalystID;

		public string CostAnalystID
		{
			get
			{
				return costAnalystID;
			}
			set
			{
				costAnalystID = value;
			}
		}

		private string financeID;

		public string FinanceID
		{
			get
			{
				return financeID;
			}
			set
			{
				financeID = value;
			}
		}

		private bool isDL;

		public bool IsDL
		{
			get
			{
				return isDL;
			}
			set
			{
				isDL = value;
			}
		}


	}

	public class BaseController : Controller
	{
		public string ldapAddress = ConfigurationManager.AppSettings["ldapAddress"];

		public string emailMsgFooter = @"<br/><br/>
							<strong style='color:red'>Please do not reply.</strong><br/>
							Applications Engineering | E-Savings";

		public string emailSenderName = ConfigurationManager.AppSettings["sender_uname"];
		public string emailSenderEmail = ConfigurationManager.AppSettings["sender_email"];
		public string emailDefaultRecipient = ConfigurationManager.AppSettings["cc_default_recipients"];

		public bool expected_start_date_is_optional = false;
		public bool number_of_months_project_as_active_is_optional = false;
		public int max_num_of_months_to_active = 0;
		//public string costSiteInChargeFFID = ConfigurationManager.AppSettings["cost_site_in_charge_ffid"];


		#region Session and Cookies get and set methods

		public string SESSION_COOKIE_USER_FFID = "user_ffID";
		public string SESSION_COOKIE_USER_NAME = "user_fullName";
		public string SESSION_COOKIE_USER_EMAIL = "user_email";
		public string SESSION_COOKIE_USER_TYPE = "user_type";
		public string SESSION_COOKIE_USER_DEPT = "user_dept";
		public string SESSION_COOKIE_USER_IS_MASTER = "user_is_master";
		public string SESSION_COOKIE_USER_IS_MULTI_PERMISSION = "user_is_multi_permission";
		public string SESSION_COOKIE_USER_MGR_FFID = "user_mgr_ffid";
		public string SESSION_COOKIE_USER_COST_ANALYST_ID = "user_cost_analyst_id";
		public string SESSION_COOKIE_USER_FINANCE_ID = "user_finance_id";
		public string SESSION_COOKIE_PASSWORD = "user_password";
		public string SESSION_COOKIE_USER_IS_DL = "user_is_DL";
		//public string SET_SESSION_ERROR = "errr";


		//public bool IsCostSiteInChargeExistsInAD ()
		//{
		//	var costSiteInchargeInfo =  Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, this.costSiteInChargeFFID);

		//	return (costSiteInchargeInfo == null || costSiteInchargeInfo.Count == 0) ? false : true;
		//}

		public BaseController ()
		{

			//Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ms-MY");
			//Thread.CurrentThread.CurrentUICulture = new CultureInfo("ms-MY");

			if (int.TryParse(ConfigurationManager.AppSettings["max_num_of_months_to_active"], out this.max_num_of_months_to_active) == false)
			{
				this.max_num_of_months_to_active = 12;
			}

			if (ConfigurationManager.AppSettings["expected_start_date_is_optional"].ToLower() == "true")
			{
				expected_start_date_is_optional = true;
			}

			if (ConfigurationManager.AppSettings["number_of_months_project_as_active_is_optional"].ToLower() == "true")
			{
				number_of_months_project_as_active_is_optional = true;
			}
		}


		public bool SetUserSession (UserSession userSession)
		{
			Session[this.SESSION_COOKIE_USER_FFID] = userSession.ffid;
			Session[this.SESSION_COOKIE_USER_NAME] = userSession.fullName;
			Session[this.SESSION_COOKIE_USER_EMAIL] = userSession.email;
			Session[this.SESSION_COOKIE_USER_TYPE] = ((int)userSession.type).ToString();
			Session[this.SESSION_COOKIE_USER_DEPT] = userSession.department;
			Session[this.SESSION_COOKIE_USER_IS_MASTER] = userSession.isMaster;
			Session[this.SESSION_COOKIE_USER_IS_MULTI_PERMISSION] = userSession.isMultiPermission;
			Session[this.SESSION_COOKIE_USER_MGR_FFID] = userSession.mgrFFID;
			Session[this.SESSION_COOKIE_USER_COST_ANALYST_ID] = userSession.CostAnalystID;
			Session[this.SESSION_COOKIE_USER_FINANCE_ID] = userSession.FinanceID;
			Session[this.SESSION_COOKIE_USER_IS_DL] = userSession.IsDL;

			return IsUserSuccessfullyLoggedIn();
		}


		public bool IsUserSuccessfullyLoggedIn ()
		{

			var result = (Session[SESSION_COOKIE_USER_FFID] != null 
						&& Session[SESSION_COOKIE_USER_NAME] != null
						&& Session[SESSION_COOKIE_USER_EMAIL] != null 
						&& Session[SESSION_COOKIE_USER_TYPE] != null) ? true : false;

			return result;
		}


		public string UserFFID
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_FFID].ToString();
				}
				return "";
			}
		}


		public string UserFullName
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_NAME].ToString();
				}
				return "";
			}
		}


		public string UserEmail
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_EMAIL].ToString();
				}
				return "";
			}
		}

		public string UserType
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_TYPE].ToString();
				}
				return "0";
			}
		}


		public string UserDepartment
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_DEPT].ToString();
				}
				return "";
			}
		}

		public bool IsUserMaster
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return (bool)Session[SESSION_COOKIE_USER_IS_MASTER];
				}
				return false;
			}
		}

		public bool IsUserMultiPermission
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return (bool)Session[SESSION_COOKIE_USER_IS_MULTI_PERMISSION];
				}
				return false;
			}
		}


		public string UserManagerFFID
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_MGR_FFID].ToString();
				}
				return "";
			}
		}

		public string UserCostAnalystID
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_COST_ANALYST_ID].ToString();
				}
				return "";
			}
		}

		public string UserFinanceID
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return Session[SESSION_COOKIE_USER_FINANCE_ID].ToString();
				}
				return "";
			}
		}


		public bool IsDL
		{
			get
			{
				if (IsUserSuccessfullyLoggedIn())
				{
					return (bool)Session[SESSION_COOKIE_USER_IS_DL];
				}
				return false;
			}
		}


		public void ViewBagUserData ()
		{
			ViewBag.UserFFID = this.UserFFID;
			ViewBag.UserFullName = this.UserFullName;
			ViewBag.UserEmail = this.UserEmail;
			ViewBag.UserDepartment = this.UserDepartment;
			ViewBag.UserType = this.UserType;
			ViewBag.IsUserMaster = this.IsUserMaster;
			ViewBag.IsUserMultiPermission = this.IsUserMultiPermission;
			ViewBag.UserTypeStr = StaticData.GetUserTypeStr(int.Parse(this.UserType), this.IsUserMaster);
			ViewBag.IsUserLoggedIn = IsUserSuccessfullyLoggedIn();
			ViewBag.IsDL = this.IsDL;
		}

		public void ViewBagUserTypes ()
		{
			ViewBag.userType_client = ((int)StaticData.UserTypes.Client).ToString();
			ViewBag.userType_costAnalyst = ((int)StaticData.UserTypes.CostAnalyst).ToString();
			ViewBag.userType_finance = ((int)StaticData.UserTypes.Finance).ToString();
			ViewBag.userType_manager = ((int)StaticData.UserTypes.Manager).ToString();
			ViewBag.userType_costSiteInCharge = ((int)StaticData.UserTypes.CostSiteInCharge).ToString();
		}

		public void ViewBagOverallStatus ()
		{
			ViewBag.status_PROJECT_PROPOSAL = ((int)StaticData.OverallStatus.PROJECT_PROPOSAL).ToString();
			ViewBag.status_COST_ANALYST_REVIEW_IN_PROGRESS = ((int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS).ToString();
			ViewBag.status_COST_FUNNEL_IDENTIFIED = ((int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED).ToString();
			ViewBag.status_FINANCE_REVIEW_IN_PROGRESS = ((int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS).ToString();
			ViewBag.status_COST_FUNNEL_EVALUATING = ((int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING).ToString();
			ViewBag.status_INVALID = ((int)StaticData.OverallStatus.INVALID).ToString();
			//ViewBag.status_COST_AVOIDANCE = ((int)StaticData.OverallStatus.COST_AVOIDANCE).ToString();
			ViewBag.status_EXISTING_PROJECT = ((int)StaticData.OverallStatus.EXISTING_PROJECT).ToString();
			ViewBag.status_DUPLICATE_ENTRY = ((int)StaticData.OverallStatus.DUPLICATE_ENTRY).ToString();
			ViewBag.status_REALIZATION = ((int)StaticData.OverallStatus.REALIZATION).ToString();
			ViewBag.status_ACTIVE = ((int)StaticData.OverallStatus.ACTIVE).ToString();
			ViewBag.status_COMPLETED = ((int)StaticData.OverallStatus.COMPLETED).ToString();
			ViewBag.status_CANCELED = ((int)StaticData.OverallStatus.CANCELED).ToString();

		}


		public void ViewBagProjectTypes ()
		{
			ViewBag.types_COST_SAVINGS = ((int)StaticData.ProjectTypes.COST_SAVINGS).ToString();
			ViewBag.types_COST_AVOIDANCE = ((int)StaticData.ProjectTypes.COST_AVOIDANCE).ToString();
		}


		public void SetUserCookies (UserSession userSession)
		{
			HttpCookie userFFID = new HttpCookie(SESSION_COOKIE_USER_FFID);
			userFFID.Value = userSession.ffid;
			userFFID.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(userFFID);

			HttpCookie userName = new HttpCookie(SESSION_COOKIE_USER_NAME);
			userName.Value = userSession.ffid;
			userName.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(userName);

			HttpCookie email = new HttpCookie(SESSION_COOKIE_USER_EMAIL);
			email.Value = userSession.ffid;
			email.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(email);

			HttpCookie userType = new HttpCookie(SESSION_COOKIE_USER_TYPE);
			userType.Value = userSession.ffid;
			userType.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(userType);


			HttpCookie password = new HttpCookie(SESSION_COOKIE_PASSWORD);
			password.Value = userSession.password;
			password.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(password);
		}


		public void UnSetUserCookies ()
		{
			if (Request.Cookies.AllKeys.Contains(SESSION_COOKIE_USER_FFID) 
				&& Request.Cookies.AllKeys.Contains(SESSION_COOKIE_USER_NAME)
				&& Request.Cookies.AllKeys.Contains(SESSION_COOKIE_USER_EMAIL)
				&& Request.Cookies.AllKeys.Contains(SESSION_COOKIE_USER_TYPE)
				&& Request.Cookies.AllKeys.Contains(SESSION_COOKIE_PASSWORD))
			{
				HttpCookie userFFID = new HttpCookie(SESSION_COOKIE_USER_FFID);
				userFFID.Expires = DateTime.Now.AddYears(-1);
				Response.Cookies.Add(userFFID);

				HttpCookie userName = new HttpCookie(SESSION_COOKIE_USER_NAME);
				userName.Expires = DateTime.Now.AddYears(-1);
				Response.Cookies.Add(userName);

				HttpCookie email = new HttpCookie(SESSION_COOKIE_USER_EMAIL);
				email.Expires = DateTime.Now.AddYears(-1);
				Response.Cookies.Add(email);

				HttpCookie userType = new HttpCookie(SESSION_COOKIE_USER_TYPE);
				userType.Expires = DateTime.Now.AddYears(-1);
				Response.Cookies.Add(userType);


				HttpCookie password = new HttpCookie(SESSION_COOKIE_PASSWORD);
				password.Expires = DateTime.Now.AddYears(-1);
				Response.Cookies.Add(password);
			}
		}

		public bool IsUserCookieSet ()
		{
			HttpCookie userFFID = Request.Cookies[SESSION_COOKIE_USER_FFID];
			HttpCookie userName = Request.Cookies[SESSION_COOKIE_USER_NAME];
			HttpCookie email = Request.Cookies[SESSION_COOKIE_USER_EMAIL];
			HttpCookie userType = Request.Cookies[SESSION_COOKIE_USER_TYPE];

			if (userFFID != null && userName != null && email != null && userType != null)
			{
				return true;
			}

			return false;
		}


		public void GetUserDataCookiesViewBag ()
		{

			ViewBag.cookie_userFFID = "";
			ViewBag.cookie_fullName = "";
			ViewBag.cookie_email = "";
			ViewBag.cookie_userType = "";
			ViewBag.cookie_password = "";

			if (IsUserCookieSet())
			{
				HttpCookie userFFID = Request.Cookies[SESSION_COOKIE_USER_FFID];
				HttpCookie userName = Request.Cookies[SESSION_COOKIE_USER_NAME];
				HttpCookie email = Request.Cookies[SESSION_COOKIE_USER_EMAIL];
				HttpCookie userType = Request.Cookies[SESSION_COOKIE_USER_TYPE];
				HttpCookie password = Request.Cookies[SESSION_COOKIE_PASSWORD];

				ViewBag.cookie_userFFID = userFFID.Value;
				ViewBag.cookie_fullName = userName.Value;
				ViewBag.cookie_email = email.Value;
				ViewBag.cookie_userType = userType.Value;
				ViewBag.cookie_password = password.Value;

			}
		}

		public void SetUserCookiesToSession ()
		{

			if (IsUserCookieSet())
			{
				HttpCookie userFFID = Request.Cookies[SESSION_COOKIE_USER_FFID];
				HttpCookie userName = Request.Cookies[SESSION_COOKIE_USER_NAME];
				HttpCookie email = Request.Cookies[SESSION_COOKIE_USER_EMAIL];
				HttpCookie userType = Request.Cookies[SESSION_COOKIE_USER_TYPE];

				Session[this.SESSION_COOKIE_USER_FFID] = userFFID.Value;
				Session[this.SESSION_COOKIE_USER_NAME] = userName.Value;
				Session[this.SESSION_COOKIE_USER_EMAIL] = email.Value;
				Session[this.SESSION_COOKIE_USER_TYPE] = userType.Value;

			}


		}

		#endregion


		#region Uploading methods

		public string[] GetValidDocsFileExtensions ()
		{
			string[] validExtensions = ConfigurationManager.AppSettings["valid_documents"].Trim(' ').Split('|');
			return validExtensions;
		}

		public string[] GetValidImgFileExtensions ()
		{
			string[] validExtensions = ConfigurationManager.AppSettings["valid_imgs"].Trim(' ').Split('|');
			return validExtensions;
		}


		public string[] GetValidDocsMIMETypes ()
		{
			string[] validMIMETypes = new string[]{
				"application/vnd.ms-outlook",
				"application/octet-stream",
				"text/csv",
				"text/plain",
				"application/pdf",
				"application/msword",
				"application/vnd.ms-excel",
				"application/vnd.ms-powerpoint",
				"application/vnd.openxmlformats-officedocument.presentationml.presentation",
				"application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
			};

			return validMIMETypes;
		}

		public string[] GetValidImgsMIMETypes ()
		{
			string[] validMIMETypes = new string[]{
				"image/png", "image/jpg", "image/jpeg", "image/gif", "image/bmp", "image/dds", "image/pspimage",
				"image/tga", "image/thm", "image/tif", "image/tiff", "image/yuv", "image/jif", "image/jfif",
				"image/jp2", "image/jpx", "image/j2k", "image/j2c", "image/fpx", "image/pcd"
			};

			return validMIMETypes;
		}


		public string GetFileNewFileName (string str)
		{
			try
			{
				Random rnd = new Random();
				var nowTime = DateTime.Now.ToString("yyMMddHHmmssffftt");
				int randNum = rnd.Next(1, 1000);

				string newFileName = Helpers.Hashing.GetHashMD5(str + nowTime + randNum.ToString());

				return newFileName;
			}
			catch (Exception ex)
			{
				throw new System.InvalidOperationException(ex.Message);
			}
		}

		[HttpPost]
		public IDictionary<string, string> UploadThisFile (HttpPostedFileBase file, string uploadPath, string expectedFile = "")
		{
			IDictionary<string, string> results = new Dictionary<string, string>();

			try
			{
				string[] validDocsExtension = this.GetValidDocsFileExtensions();
				string[] validImgsExtension = this.GetValidImgFileExtensions();

				string[] validDocsMIMETypes = this.GetValidDocsMIMETypes();
				string[] validImgsMIMETypes = this.GetValidImgsMIMETypes();

				if (file.ContentLength > 0)
				{
					var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
					var fileName = Path.GetFileName(file.FileName);
					var newFileName = this.GetFileNewFileName(fileName) + "." + fileExt;

					if (file.ContentLength <= 20000000)
					{
						if (expectedFile != "")
						{
							if (fileExt != expectedFile)
							{
								results["done"] = "false";
								results["msg"] = "Invalid document type (expected file is ." + expectedFile + ")";
								results["fileType"] = "";
								results["origfileName"] = "";
								results["newFileName"] = "";

								return results;
							}
						}

						if (validDocsExtension.Contains(fileExt.ToLower())) // DOCS UPLOAD
						{
							var fileContentType = file.ContentType;

							if (validDocsMIMETypes.Contains(fileContentType))
							{
								if (uploadPath != "")
								{
									var path = Path.Combine(Server.MapPath(uploadPath), newFileName);
									file.SaveAs(path);
								}


								results["done"] = "TRUE";
								results["msg"] = "Successfully uploaded!";
								results["fileType"] = "doc";
								results["origfileName"] = fileName;
								results["newFileName"] = newFileName;

							}
							else
							{
								results["done"] = "FALSE";
								results["msg"] = "Invalid document type (Not allowed to upload)";
								results["fileType"] = "";
								results["origfileName"] = fileName;
								results["newFileName"] = newFileName;
							}
						}
						else if (validImgsExtension.Contains(fileExt.ToLower())) // IMAGE UPLOAD
						{
							var fileContentType = file.ContentType;

							if (validImgsMIMETypes.Contains(fileContentType))
							{
								if (uploadPath != "")
								{

									//HttpContext.Current.Request.GetBufferlessInputStream(true);

									var path = Path.Combine(Server.MapPath(uploadPath), newFileName);
									file.SaveAs(path);
								}

								results["done"] = "TRUE";
								results["msg"] = "Successfully uploaded!";
								results["fileType"] = "img";
								results["origfileName"] = fileName;
								results["newFileName"] = newFileName;

							}
							else
							{
								results["done"] = "FALSE";
								results["msg"] = "Invalid image type (Not allowed to upload)";
								results["fileType"] = "";
								results["origfileName"] = fileName;
								results["newFileName"] = newFileName;
							}
						}
						else
						{
							results["done"] = "FALSE";
							results["msg"] = "File is invalid (Not allowed to upload)";
							results["fileType"] = "";
							results["origfileName"] = fileName;
							results["newFileName"] = newFileName;
						}
					}
					else
					{
						results["done"] = "FALSE";
						results["msg"] = "Invalid file size";
						results["fileType"] = "";
						results["origfileName"] = fileName;
						results["newFileName"] = newFileName;
					}
				}
			}
			catch (Exception ex)
			{
				results["done"] = "FALSE";
				results["msg"] = ex.ToString();
				results["fileType"] = "";
				results["origfileName"] = "";
				results["newFileName"] = "";
			}

			return results;
		}


		public FileResult Download (string serverFileName, string categoryDIR, string fileName)
		{
			try
			{
				var FileVirtualPath = Server.MapPath("~/App_Data/" + categoryDIR + serverFileName);
				byte[] fileBytes = System.IO.File.ReadAllBytes(@FileVirtualPath);
				return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
			}
			catch (FileNotFoundException ex)
			{
				Response.Write("File not found :" + ex.Message.ToString());

			}
			catch (DirectoryNotFoundException dirEx)
			{
				// Let the user know that the directory did not exist.
				Response.Write("Directory not found: " + dirEx.Message.ToString());
			}
			catch (Exception ex)
			{
				Response.Write("Error found: " + ex.Message.ToString());
			}

			var FileVirtualPath2 = Server.MapPath("~/Assets/imgs/TempFile.txt");
			byte[] fileBytes2 = System.IO.File.ReadAllBytes(@FileVirtualPath2);
			return File(fileBytes2, System.Net.Mime.MediaTypeNames.Application.Octet, "File_not_found.txt");

		}


		#endregion





		#region formdata helpers

		public Tuple<Boolean, string> formData_validation (IDictionary<string, string> dataKeys, FormCollection formData)
		{
			try
			{
				string errorMsg = "";
				string inputName = "";
				string inputDescription = "";

				Boolean isInputsComplete = true;

				foreach (KeyValuePair<string, string> data in dataKeys)
				{
					inputName = data.Key.ToString();
					inputDescription = data.Value.ToString();

					if (!formData.AllKeys.Contains(inputName))
					{
						errorMsg += "<strong>" + inputDescription + "</strong> is required <br/>";
						isInputsComplete = false;
						return Tuple.Create(isInputsComplete, errorMsg);
					}

					if (formData[inputName] == "")
					{
						errorMsg += "<strong class='error'>" + inputDescription + " is required</strong><br/>";
						isInputsComplete = false;
						return Tuple.Create(isInputsComplete, errorMsg);
					}
				}

				return Tuple.Create(isInputsComplete, errorMsg);
			}
			catch (Exception ex)
			{
				return Tuple.Create(false, ex.ToString());
			}

		}


		public Dictionary<string, string> GetFormCollectionData (FormCollection data)
		{
			var list = new Dictionary<string, string>();
			foreach (string key in data.Keys)
			{
				list.Add(key, data[key]);
			}

			return list;
		}

		#endregion


		public bool IsUserIsCostAnalystOnTheCurrentProposal (string proposalUserDeptCode)
		{
			if (IsUserSuccessfullyLoggedIn() == true)
			{
				if (int.Parse(this.UserType) == (int)StaticData.UserTypes.CostAnalyst)
				{

					//if (this.IsUserMaster == true)
					//	return true;

					var costAnalystDeptCodes = Factory.CostAnalystDeptCodesFactory().GetDeptCodesByCostAnalystID(int.Parse(this.UserCostAnalystID));

					foreach (var deptCode in costAnalystDeptCodes)
					{
						if (deptCode.DeptCode == proposalUserDeptCode)
						{
							return true;
						}
					}

				}
			}

			return false;
		}


		public bool IsUserIsFinanceApproverOnTheCurrentProposal (int proposalID)
		{

			if (IsUserSuccessfullyLoggedIn() == true)
			{
				if (int.Parse(this.UserType) == (int)StaticData.UserTypes.Finance)
				{
					var proposalFinanceApprovalResults = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoVerificationResults(proposalID, int.Parse(this.UserFinanceID));

					if (proposalFinanceApprovalResults != null)
					{
						return true;
					}
				}
			}

			return false;
		}

		public List<ProposalManagerVerifier> GetProposalManagers (int proposalID)
		{
			var proposalManagers = Factory.ProposalManagerVerifierRepository().GetProposalManagerVerifiers(proposalID);

			for (int i = 0 ; i<proposalManagers.Count ; i++)
			{
				var manTmp = proposalManagers[i];

				var employeeInfo = Helpers.ONEmployeesLDAP.SearchEmployee(this.ldapAddress, manTmp.ManFFID);

				if (employeeInfo != null && employeeInfo.Count > 0)
				{
					manTmp.FullName = employeeInfo[0].DisplayName;
					manTmp.Email = employeeInfo[0].Email;
				}
				else
				{
					manTmp.FullName = "Unknown";
					manTmp.Email = "Unknown";
				}

				proposalManagers[i] = manTmp;
			}

			return proposalManagers.ToList();
		}


		//public List<ProposalAction> GetProposalNeededActionItemsWithApprovers (int proposalID)
		//{
		//	var 
		//}

		public bool isUserIsManagerVerifierOnTheCurrentProposal (int proposalID)
		{

			if (IsUserSuccessfullyLoggedIn() == true)
			{
				if (int.Parse(this.UserType) == (int)StaticData.UserTypes.Manager)
				{

					var managers = this.GetProposalManagers(proposalID);

					foreach (var man in managers)
					{
						if (man.ManFFID == this.UserFFID)
						{
							return true;
						}
					}

				}
			}
			return false;
		}

		public Dictionary<int, FinanceCategory> GetAllFinanceCategoryById ()
		{
			var newCategories = new Dictionary<int, FinanceCategory>();

			var categories = Factory.FinanceCategoryRepository().GetAllCategories();

			foreach (var cat in categories)
			{
				newCategories.Add(cat.Id, cat);
			}

			return newCategories;
		}

		public List<Proposal> GetProposalAdditionalInfo (List<Proposal> proposals)
		{
			//var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();

			//create dictionary list for Finance category to prevent multiple access in database
			var financeCategories = this.GetAllFinanceCategoryById();

			string site = ConfigurationManager.AppSettings["onsemi_site"];
			string siteBaseUrl = Request.Url.GetLeftPart(UriPartial.Authority) +""+ Url.Content("~");
			string currentImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_current_imgs"]);
			string proposalImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"]);

			for (int i = 0 ; i <proposals.Count ; i++)
			{
				var proposal = proposals[i];
				proposal.CurrentImgs = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposal.Id);
				proposal.ProposalImgs = Factory.ProposalImgFactory().GetProposalImgs(proposal.Id);

				proposal.FinanceCategory = financeCategories[proposal.FinanceCategoryID].Category;
				proposal.SiteIndicator = site;
				proposal.SiteBaseURL = siteBaseUrl;
				proposal.CurrentImgsPath = currentImgsPath;
				proposal.ProposalImgsPath = proposalImgsPath;

				//var deptTmp = departments.Where(d => d.DeptCode == proposal.EmpDeptCode).FirstOrDefault();
				var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(proposal.AreaDeptBeneficiary);
				proposal.DeptName = dept != null ? dept.DeptName : "";

				proposals[i] = proposal;
			}

			return proposals;
		}


		public ActionResult ChangeLanguage (string languageAbbrevation)
		{
			if (languageAbbrevation != null)
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageAbbrevation);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageAbbrevation);
				Domain.StaticData.changeCurrentCulture(languageAbbrevation);
			}

			HttpCookie cookie = new HttpCookie("ESavingsLanguage");
			cookie.Value = languageAbbrevation;
			Response.Cookies.Add(cookie);

			return RedirectToAction("Index", "Home");
		}

	}
}
