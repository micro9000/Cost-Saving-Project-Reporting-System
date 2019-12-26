
function Display_Proposals(data, method) {

	var display = "";

	$.each(data, function (i, proposal) {
		display += "<tr class=''>";
		display += "<td>" + proposal.SiteIndicator + "</td>";
		display += "<td><a class='proposal_title_link' href='" + proposal.SiteBaseURL + "Home/Details/" + proposal.Id + "'>" + proposal.ProjectTitle + "</a></td>";

		display += "<td>" + proposal.ProjectTypeIndicator + "</td>";

		if (proposal.CurrentImgs.length > 0) {
			display += "<td><img class='materialboxed esaving-image-in-table' src='" + proposal.SiteBaseURL + proposal.CurrentImgsPath + proposal.CurrentImgs[0].ServerFileName + "'/></td>";
		} else {
			display += "<td></td>";
		}

		if (proposal.ProposalImgs.length > 0) {
			display += "<td><img class='materialboxed esaving-image-in-table' src='" + proposal.SiteBaseURL + proposal.ProposalImgsPath + proposal.ProposalImgs[0].ServerFileName + "'/></td>";
		} else {
			display += "<td></td>";
		}

		display += "<td>" + proposal.SubmittedBy + "</td>";
		display += "<td>" + proposal.DeptName + "</td>";

		display += "<td>$" + (proposal.DollarImpact * proposal.NumberOfMonthsToBeActive) + "</td>";
		display += "<td>" + ((proposal.ExpectedStartDateStr != "0001-01-01") ? proposal.ExpectedStartDateStr : "") + "</td>";

		display += "<td>" + proposal.OAStatusIndicator + "</td>";

		display += "</tr>";


		//console.log(proposal.SubmittedDate);
	});
	
	if (method === "display"){
		$("#proposals_table").html(display);

	}else if (method === "append"){
		$("#proposals_table").append(display);

	}

}



function pagination_and_display(dataToDisplay) {

	$("#number_of_proposal_search_results").html("Results: " + dataToDisplay.length);

	$('#proposals_pagination').pagination({
		dataSource: dataToDisplay,
		pageSize: 5,
		autoHidePrevious: true,
		autoHideNext: true,
		// showGoInput: true,
		// showGoButton: true,
		callback: function (data, pagination) {
			Display_Proposals(data);
		}
	});
}



$(document).ready(function () {

	var BPIsData = {};

	//$.post(
	//	base_url + "ESavings/Get_all_BPI_Proposals",
	//	function (data) {
	//		//console.log(data);
	//		BPIsData["tarlac"] = data;
	//		//pagination_and_display(data);
	//	}
	//);

	var siteUrls = {
		tarlac: "http://phsm01ws014.ad.onsemi.com:2828/Esavings/Get_all_BPI_Proposals",
		carmona: "http://phca01ws119.ad.onsemi.com:2828/Esavings/Get_all_BPI_Proposals",
		sbn: "http://myse01ws014.ad.onsemi.com:8090/Esavings/Get_all_BPI_Proposals",
		osv: "http://vnbh01ws4064.ad.onsemi.com:8000/Esavings/Get_all_BPI_Proposals",
		osbd: "http://vnbh01ws4064.ad.onsemi.com:9000/Esavings/Get_all_BPI_Proposals"
	}

	Object.keys(siteUrls).map(prop => {
		BPIsData[prop] = fetch(siteUrls[prop])
						.then(response => response.json())
						.then(bpis => {
							return bpis;
						});
	});

	
	Object.keys(BPIsData).map(prop => {
		BPIsData[prop].then(d => {
			console.log(d);
			Display_Proposals(d, "append");
			}).catch(error => {
				
				var display = "<tr><td colspan='10' style='text-align: center;'>"+ prop.toLocaleString().toUpperCase() + " : " + error +"</td></tr>";
				$("#proposals_table").append(display);
		})
	});
})