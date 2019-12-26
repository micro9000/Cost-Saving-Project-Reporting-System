using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace Helpers
{

	public class Employee
	{
		private string email = "";

		public string Email
		{
			get
			{
				return email;
			}
			set
			{
				email = value;
			}
		}


		private string userName = "";

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
			}
		}

		private string displayName = "";

		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}

		private string department = "";

		public string Department
		{
			get
			{
				return department;
			}
			set
			{
				department = value;
			}
		}

		private string accPhoto = "";

		public string AccPhoto
		{
			get
			{
				return accPhoto;
			}
			set
			{
				accPhoto = value;
			}
		}

		private string pCode = "";

		public string postalCode
		{
			get
			{
				return pCode;
			}
			set
			{
				pCode = value;
			}
		}


		private string cid = "";

		public string CID
		{
			get
			{
				return cid;
			}
			set
			{
				cid = value;
			}
		}

		private string ffID = "";

		public string FFID
		{
			get
			{
				return ffID;
			}
			set
			{
				ffID = value;
			}
		}

		private string position = "";

		public string Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}


		private string mgrFFID = "";

		public string ManagerFFID
		{
			get
			{
				return mgrFFID;
			}
			set
			{
				mgrFFID = value;
			}
		}

		private string departmentNumber = "";

		public string DepartmentNumber
		{
			get
			{
				return departmentNumber;
			}
			set
			{
				departmentNumber = value;
			}
		}


		private string pdofficeName = "";

		public string physicalDeliveryOfficeName
		{
			get
			{
				return pdofficeName;
			}
			set
			{
				pdofficeName = value;
			}
		}

		private string strtAddrs = "";

		public string streetAddress
		{
			get
			{
				return strtAddrs;
			}
			set
			{
				strtAddrs = value;
			}
		}


		private string cpnyCity = "";

		public string companyCity
		{
			get
			{
				return cpnyCity;
			}
			set
			{
				cpnyCity = value;
			}
		}



		private string cpnyStaet = "";

		public string companyState
		{
			get
			{
				return cpnyStaet;
			}
			set
			{
				cpnyStaet = value;
			}
		}

		private string cpnyCountry = "";

		public string companyCountry
		{
			get
			{
				return cpnyCountry;
			}
			set
			{
				cpnyCountry = value;
			}
		}

		private bool _isMapeed = false;

		public bool isMapped
		{
			get
			{
				return _isMapeed;
			}
			set
			{
				_isMapeed = value;
			}
		}

	}

	public static class ONEmployeesLDAP
	{

		public static Employee SignIn (string ldapAddress, string ffID, string password)
		{
			Employee employee = new Employee();
			try
			{
				DirectoryEntry directoryEntry = new DirectoryEntry(ldapAddress, ffID, password);

				DirectorySearcher ds = new DirectorySearcher(directoryEntry);

				ds.Filter = "(sAMAccountName=" + ffID + ")";
				ds.SearchScope = SearchScope.Subtree;
				SearchResult rs = ds.FindOne();



				if (rs.GetDirectoryEntry().Properties.Values.Count > 0)
				{

					if (rs.GetDirectoryEntry().Properties.Contains("samaccountname"))
					{
						employee.FFID = rs.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("employeenumber"))
					{
						employee.CID = rs.GetDirectoryEntry().Properties["employeenumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("manager"))
					{
						employee.ManagerFFID = rs.GetDirectoryEntry().Properties["manager"].Value.ToString().Substring(3, 6);
					}

					if (rs.GetDirectoryEntry().Properties.Contains("departmentNumber"))
					{
						employee.DepartmentNumber = rs.GetDirectoryEntry().Properties["departmentNumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("mail"))
					{
						employee.Email = rs.GetDirectoryEntry().Properties["mail"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("department"))
					{
						employee.Department = rs.GetDirectoryEntry().Properties["department"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("displayname"))
					{
						employee.DisplayName = rs.GetDirectoryEntry().Properties["displayname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("title"))
					{
						employee.Position = rs.GetDirectoryEntry().Properties["title"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("postalCode"))
					{
						employee.postalCode = rs.GetDirectoryEntry().Properties["postalCode"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("streetAddress"))
					{
						employee.streetAddress = rs.GetDirectoryEntry().Properties["streetAddress"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("l"))
					{
						employee.companyCity = rs.GetDirectoryEntry().Properties["l"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("st"))
					{
						employee.companyState = rs.GetDirectoryEntry().Properties["st"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("c"))
					{
						employee.companyCountry = rs.GetDirectoryEntry().Properties["c"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("physicalDeliveryOfficeName"))
					{
						employee.physicalDeliveryOfficeName = rs.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("thumbnailPhoto"))
					{
						employee.AccPhoto = rs.GetDirectoryEntry().Properties["thumbnailPhoto"].Value.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				throw new System.Exception(ex.Message);
			}



			return employee;
		}

		public static Employee SearchDeptManager (string ldapAddress, string departmentCode, string generalMgrFFID="fg6xht")
		{
			Employee employee = new Employee();

			try
			{

				DirectoryEntry directoryEntry = new DirectoryEntry(ldapAddress);
				DirectorySearcher ds = new DirectorySearcher(directoryEntry);
				ds.Filter = @"(&(objectClass=user)(objectCategory=person)(manager=CN="+ generalMgrFFID +",OU=SSMP,OU=ON_Users,OU=PHSM01,OU=Asia,DC=ad,DC=onsemi,DC=com)(department="+ departmentCode +"))";
				ds.SearchScope = SearchScope.Subtree;
				SearchResult rs = ds.FindOne();

				if (rs.GetDirectoryEntry().Properties.Values.Count > 0)
				{

					if (rs.GetDirectoryEntry().Properties.Contains("samaccountname"))
					{
						employee.FFID = rs.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("employeenumber"))
					{
						employee.CID = rs.GetDirectoryEntry().Properties["employeenumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("manager"))
					{
						employee.ManagerFFID = rs.GetDirectoryEntry().Properties["manager"].Value.ToString().Substring(3, 6);
					}

					if (rs.GetDirectoryEntry().Properties.Contains("departmentNumber"))
					{
						employee.DepartmentNumber = rs.GetDirectoryEntry().Properties["departmentNumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("mail"))
					{
						employee.Email = rs.GetDirectoryEntry().Properties["mail"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("department"))
					{
						employee.Department = rs.GetDirectoryEntry().Properties["department"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("displayname"))
					{
						employee.DisplayName = rs.GetDirectoryEntry().Properties["displayname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("title"))
					{
						employee.Position = rs.GetDirectoryEntry().Properties["title"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("postalCode"))
					{
						employee.postalCode = rs.GetDirectoryEntry().Properties["postalCode"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("streetAddress"))
					{
						employee.streetAddress = rs.GetDirectoryEntry().Properties["streetAddress"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("l"))
					{
						employee.companyCity = rs.GetDirectoryEntry().Properties["l"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("st"))
					{
						employee.companyState = rs.GetDirectoryEntry().Properties["st"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("c"))
					{
						employee.companyCountry = rs.GetDirectoryEntry().Properties["c"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("physicalDeliveryOfficeName"))
					{
						employee.physicalDeliveryOfficeName = rs.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("thumbnailPhoto"))
					{
						employee.AccPhoto = rs.GetDirectoryEntry().Properties["thumbnailPhoto"].Value.ToString();
					}
				}

			}
			catch (Exception ex)
			{
				throw new System.Exception(ex.Message);
			}

			return employee;
		}



		public static Employee GetEmployeeInfo (string ldapAddress, string ffID)
		{
			Employee employee = new Employee();

			try
			{
				if (ffID == "")
					return employee;

				DirectoryEntry directoryEntry = new DirectoryEntry(ldapAddress);
				DirectorySearcher ds = new DirectorySearcher(directoryEntry);
				ds.Filter = @"(&(objectClass=user)(objectCategory=person)(cn="+ ffID +"))";
				ds.SearchScope = SearchScope.Subtree;
				SearchResult rs = ds.FindOne();

				if (rs.GetDirectoryEntry().Properties.Values.Count > 0)
				{

					if (rs.GetDirectoryEntry().Properties.Contains("samaccountname"))
					{
						employee.FFID = rs.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("employeenumber"))
					{
						employee.CID = rs.GetDirectoryEntry().Properties["employeenumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("manager"))
					{
						employee.ManagerFFID = rs.GetDirectoryEntry().Properties["manager"].Value.ToString().Substring(3, 6);
					}

					if (rs.GetDirectoryEntry().Properties.Contains("departmentNumber"))
					{
						employee.DepartmentNumber = rs.GetDirectoryEntry().Properties["departmentNumber"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("mail"))
					{
						employee.Email = rs.GetDirectoryEntry().Properties["mail"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("department"))
					{
						employee.Department = rs.GetDirectoryEntry().Properties["department"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("displayname"))
					{
						employee.DisplayName = rs.GetDirectoryEntry().Properties["displayname"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("title"))
					{
						employee.Position = rs.GetDirectoryEntry().Properties["title"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("postalCode"))
					{
						employee.postalCode = rs.GetDirectoryEntry().Properties["postalCode"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("streetAddress"))
					{
						employee.streetAddress = rs.GetDirectoryEntry().Properties["streetAddress"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("l"))
					{
						employee.companyCity = rs.GetDirectoryEntry().Properties["l"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("st"))
					{
						employee.companyState = rs.GetDirectoryEntry().Properties["st"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("c"))
					{
						employee.companyCountry = rs.GetDirectoryEntry().Properties["c"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("physicalDeliveryOfficeName"))
					{
						employee.physicalDeliveryOfficeName = rs.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value.ToString();
					}

					if (rs.GetDirectoryEntry().Properties.Contains("thumbnailPhoto"))
					{
						employee.AccPhoto = rs.GetDirectoryEntry().Properties["thumbnailPhoto"].Value.ToString();
					}
				}

			}
			catch (Exception ex)
			{
				throw new System.Exception(ex.Message);
			}

			return employee;
		}


		public static List<Employee> SearchEmployee (string ldapAddress, string searchStr)
		{
			List<Employee> lstADUsers = new List<Employee>();

			try
			{
				DirectoryEntry searchRoot = new DirectoryEntry(ldapAddress);
				DirectorySearcher search = new DirectorySearcher(searchRoot);
				search.Filter = @"(&(objectClass=user)(objectCategory=person)(|(givenName="+ searchStr +")(sn="+ searchStr +")(displayname="+ searchStr +")(samaccountname="+ searchStr +")))";
				//search.Filter = @"(&(objectClass=user)(objectCategory=person)(manager=CN=fg7xdr,OU=SSMP,OU=ON_Users,OU=PHSM01,OU=Asia,DC=ad,DC=onsemi,DC=com))";
				// "(&(objectClass=user)(objectCategory=person))";
				// "(&(objectClass=user)(objectCategory=person)(samaccountname=zbdynv))";
				search.PropertiesToLoad.Add("samaccountname");
				search.PropertiesToLoad.Add("mail");
				search.PropertiesToLoad.Add("usergroup");
				search.PropertiesToLoad.Add("displayname");//first name
				SearchResult result;
				SearchResultCollection resultCol = search.FindAll();

				if (resultCol != null)
				{
					for (int counter = 0 ; counter < resultCol.Count ; counter++)
					{
						string UserNameEmailString = string.Empty;
						result = resultCol[counter];

						Employee employee = new Employee();

						if (result.GetDirectoryEntry().Properties.Contains("samaccountname"))
						{
							employee.FFID = result.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("employeenumber"))
						{
							employee.CID = result.GetDirectoryEntry().Properties["employeenumber"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("manager"))
						{
							employee.ManagerFFID = result.GetDirectoryEntry().Properties["manager"].Value.ToString().Substring(3, 6);
						}

						if (result.GetDirectoryEntry().Properties.Contains("departmentNumber"))
						{
							employee.DepartmentNumber = result.GetDirectoryEntry().Properties["departmentNumber"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("mail"))
						{
							employee.Email = result.GetDirectoryEntry().Properties["mail"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("department"))
						{
							employee.Department = result.GetDirectoryEntry().Properties["department"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("displayname"))
						{
							employee.DisplayName = result.GetDirectoryEntry().Properties["displayname"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("title"))
						{
							employee.Position = result.GetDirectoryEntry().Properties["title"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("postalCode"))
						{
							employee.postalCode = result.GetDirectoryEntry().Properties["postalCode"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("streetAddress"))
						{
							employee.streetAddress = result.GetDirectoryEntry().Properties["streetAddress"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("l"))
						{
							employee.companyCity = result.GetDirectoryEntry().Properties["l"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("st"))
						{
							employee.companyState = result.GetDirectoryEntry().Properties["st"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("c"))
						{
							employee.companyCountry = result.GetDirectoryEntry().Properties["c"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("physicalDeliveryOfficeName"))
						{
							employee.physicalDeliveryOfficeName = result.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value.ToString();
						}

						if (result.GetDirectoryEntry().Properties.Contains("thumbnailPhoto"))
						{
							employee.AccPhoto = result.GetDirectoryEntry().Properties["thumbnailPhoto"].Value.ToString();
						}

						lstADUsers.Add(employee);
					}
				}

			}
			catch (Exception ex)
			{
				throw new System.Exception(ex.Message);
			}


			return lstADUsers;
		}

	}
}
