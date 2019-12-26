using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ESAVINGS_v1
{
	public class RouteConfig
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


			routes.MapRoute(
				name: "Proposal",
				url: "{controller}/{action}/{proposalID}",
				defaults: new
				{
					controller = "Home",
					action = "Proposal",
					proposalID = UrlParameter.Optional
				}
			);

			routes.MapRoute(
				name: "Details",
				url: "{controller}/{action}/{proposalID}",
				defaults: new
				{
					controller = "Home",
					action = "Details",
					proposalID = UrlParameter.Optional
				}
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new
				{
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional
				}
			);



		}
	}
}