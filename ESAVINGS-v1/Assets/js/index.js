$(document).ready(function () {
	$('.modal').modal();
	
});



//$(".search_selected_status option[value='0']").on("click", function () {
//	//var selectedStatu = $(this).val();

//	console.log($(".search_selected_status option[value='0']").prop("selected"));
//	if ($(".search_selected_status option[value='0']").prop("selected")) {

//		for (var i = 1; i < $(".search_selected_status option").length; i++) {
//			$(".search_selected_status option[value='" + i + "']").prop("selected", true).change();
//		}
//	}
	
//});


function Display_Proposals(data) {
	
	var display = "";

	var background_color = "";

	$.each(data, function (i, proposal) {


		if (proposal.OAStatus == "1") {
			background_color = "PROJECT_PROPOSAL lime lighten-4";

		} else if (proposal.OAStatus == "2") { 
			background_color = "COST_ANALYST_REVIEW_IN_PROGRESS lime lighten-4";

		} else if (proposal.OAStatus == "3") { 
			background_color = "COST_FUNNEL_IDENTIFIED light-blue lighten-4";

		} else if (proposal.OAStatus == "4") { 
			background_color = "FINANCE_REVIEW_IN_PROGRESS purple lighten-4";

		} else if (proposal.OAStatus == "5") {
			background_color = "COST_FUNNEL_EVALUATING green lighten-4";

		} else if (proposal.OAStatus == "6") {
			background_color = "INVALID red lighten-4";

		} else if (proposal.OAStatus == "7") {
			background_color = "COST_AVOIDANCE brown lighten-4";

		} else if (proposal.OAStatus == "8") {
			background_color = "EXISTING_PROJECT grey lighten-3";

		}else if (proposal.OAStatus == "9") {
			background_color = "DUPLICATE_ENTRY blue-grey lighten-4";

		} else if (proposal.OAStatus == "10") {
			background_color = "REALIZATION teal lighten-4";

		}
		else if (proposal.OAStatus == "11") {
			background_color = "ACTIVE light-green lighten-4";

		} else if (proposal.OAStatus == "12") {
			background_color = "";

		} else if (proposal.OAStatus == "13") {
			background_color = "CANCELED grey lighten-1";

		}

		display += "<tr class='" + background_color + "'>";
		//display += "<td>" + proposal.ProposalTicket + "</td>";
		display += "<td><a class='proposal_title_link' href='" + base_url + "Home/Details/" + proposal.Id + "'>" + proposal.ProjectTitle + "</a></td>";

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
		display += "<td>" + proposal.AreaDeptBeneficiary + " - " + proposal.DeptName + "</td>";

		display += "<td>" + proposal.ProjectedDollarImpact + "</td>";
		display += "<td>" + ((proposal.ExpectedStartDateStr != "0001-01-01") ? proposal.ExpectedStartDateStr : "") + "</td>";

		display += "<td>" + proposal.OAStatusIndicator + "</td>";

		display += "</tr>";


		//console.log(proposal.SubmittedDate);
	});

	$("#proposals_table").html(display);

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

	$('select').formSelect();

	function Get_All_Proposals() {
		$.post(
			base_url + "ESavings/Get_all_Proposals",
			function (data) {
				//console.log(data);
				displayProposalCounterAndDollarImpactComputation(data);
			}
		);
	}


	function Get_Pending_Proposals() {
		$.post(
			base_url + "ESavings/Search_Proposals",
			{
				OAStatus: 1
			},
			function (data) {
				console.log(data);
				pagination_and_display(data["proposals"]);
			}
		);
	}

	Get_All_Proposals();
	Get_Pending_Proposals();

})


//$(".btn-view-all-proposal-by-status").on("click", function () {

//	var status = $(this).attr("data-status");

//	$.post(
//		base_url + "ESavings/Search_Proposals",
//		{
//			OAStatus: status
//		},
//		function (data) {
//			//console.log(data);

//			pagination_and_display(data);
//		}
//	);

//});


function displayProposalCounterAndDollarImpactComputation(data) {

	console.log(data);

	var counterTree = data["counter"]["CounterTree"];
	var TreeKeyDesc = data["counter"]["TreeKeyDesc"];


	var display = "";

	display += "<tr>";
	display += "<td>" + TreeKeyDesc["NewProductEntry"] + "</td>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}

	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["NewProductEntry"]["NA"]["PROJECT_PROPOSAL"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";




	display += "<tr>";
	display += "<td rowspan='7'>" + TreeKeyDesc["CostFunnelIdentified"] + "</td>";
	display += "<td rowspan='3'>" + TreeKeyDesc["INProgressActionRequired"] + "</td>";

	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_ANALYST_REVIEW_IN_PROGRESS"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";




	display += "<tr>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["COST_FUNNEL_IDENTIFIED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";




	display += "<tr>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["INProgressActionRequired"]["FINANCE_REVIEW_IN_PROGRESS"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";


	display += "<tr>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["ExistingProject"]["EXISTING_PROJECT"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	
	display += "</tr>";



	display += "<tr>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}

	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}

	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["DoubleEntry"]["DUPLICATE_ENTRY"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";



	display += "<tr>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}


	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}

	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Cancelled"]["CANCELED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";
	


	display += "<tr>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["Invalid"]["INVALID"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelIdentified"]["Invalid"]["INVALID"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Invalid"]["INVALID"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Invalid"]["INVALID"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelIdentified"]["Invalid"]["INVALID"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Invalid"]["INVALID"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Invalid"]["INVALID"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelIdentified"]["Invalid"]["INVALID"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";



	display += "<tr>";
	display += "<td>" + TreeKeyDesc["CostFunnelEvaluation"] + "</td>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["CostFunnelEvaluation"]["NA"]["COST_FUNNEL_EVALUATING"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";



	display += "<tr>";
	display += "<td>" + TreeKeyDesc["Active"] + "</td>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["Active"]["NA"]["ACTIVE"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["Active"]["NA"]["ACTIVE"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["Active"]["NA"]["ACTIVE"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["Active"]["NA"]["ACTIVE"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["Active"]["NA"]["ACTIVE"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["Active"]["NA"]["ACTIVE"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["Active"]["NA"]["ACTIVE"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["Active"]["NA"]["ACTIVE"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	
	display += "</tr>";



	display += "<tr>";
	display += "<td>" + TreeKeyDesc["Completed"] + "</td>";
	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td>" + counterTree[global_project_type_for_identified]["Completed"]["NA"]["COMPLETED"]["statusDesc"] + "</td>";
		display += "<td class='center'>" + counterTree[global_project_type_for_identified]["Completed"]["NA"]["COMPLETED"]["counter"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_savings]["Completed"]["NA"]["COMPLETED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}
	
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_savings]["Completed"]["NA"]["COMPLETED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_savings]["Completed"]["NA"]["COMPLETED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["Completed"]["NA"]["COMPLETED"]["statusDesc"] + "</td>";
		display += "<td></td>";
	}

	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'>" + counterTree[global_project_type_cost_avoidance]["Completed"]["NA"]["COMPLETED"]["counter"] + "</td>";
		display += "<td>" + counterTree[global_project_type_cost_avoidance]["Completed"]["NA"]["COMPLETED"]["dollarImpact"] + "</td>";
	} else {
		display += "<td></td>";
		display += "<td></td>";
	}
	display += "</tr>";


	display += "<tr>";
	display += "<td></td>";
	display += "<td></td>";
	display += "<td><h6>Total</h6></td>";
	if (counterTree.hasOwnProperty(global_project_type_for_identified)) {
		display += "<td class='center'><h6>" + counterTree[global_project_type_for_identified]["TotalCount"]["NA"]["total"]["counter"] + "</h6></td>";
	} else {
		display += "<td></td>";
	}
	if (counterTree.hasOwnProperty(global_project_type_cost_savings)) {
		display += "<td class='center'><h6>" + counterTree[global_project_type_cost_savings]["TotalCount"]["NA"]["total"]["counter"] + "</h6></td>";
	} else {
		display += "<td></td>";
	}

	display += "<td></td>";
	if (counterTree.hasOwnProperty(global_project_type_cost_avoidance)) {
		display += "<td class='center'><h6>" + counterTree[global_project_type_cost_avoidance]["TotalCount"]["NA"]["total"]["counter"] + "</h6></td>";
	} else {
		display += "<td></td>";
	}
	display += "<td></td>";
	display += "</tr>";

	$("#proposal_counter_and_dollarImpact_calculation").html(display);

}


$(".btn-search-by-keyword-andor-status").on("click", function () {

	$.post(
		base_url + "ESavings/Search_Proposals_by_keyword_andor_status",
		{
			projectType: $("input[name='search_project_type']:checked").attr("data-project-type"),
			searchKeyword: $("#searchKeyword").val(),
			startDate: $("#startDate").val(),
			endDate: $("#endDate").val(),
			statusList: $(".search_selected_status").val().toString(),
			deptList: $(".search_selected_departments").val().toString(),
			isBPI: ($("#btn-mark-proposal-as-bpi").prop("checked") === true) ? 1 : 0
		},
		function (data) {
			//console.log(data);
			pagination_and_display(data["proposals"]);
			displayProposalCounterAndDollarImpactComputation(data);
		}
	);

});



$(".btn-export-to-excel").on("click", function () {


	var format = $(this).attr("data-format");

	var projectType = $("input[name='search_project_type']:checked").attr("data-project-type");
	var searchKeyword = $("#searchKeyword").val();
	var startDate = $("#startDate").val();
	var endDate = $("#endDate").val();
	var statusList = $(".search_selected_status").val().toString();
	var deptList = $(".search_selected_departments").val().toString();

	//$.get(
	//	base_url + "ESavings/TestingExportToExcel",
	//	{
	//		searchKeyword: searchKeyword,
	//		startDate: startDate,
	//		endDate: endDate,
	//		statusList: statusList,
	//		deptList: deptList
	//	},
	//	function (data) {
	//		console.log(data);
	//	}
	//);

	//console.log(statusList);
	//console.log(deptList);

	var parameters = "";
	var paramLen = 0;
	var exportFile = base_url + ((format === "esavings") ? "ESavings/ProposalExportToExcel" : "ESavings/ProposalExportToExcelQlikviewFormat");
	
	if (projectType != "") {
		parameters += "projectType=" + projectType;
		paramLen += 1;
	}

	if (searchKeyword != "") {
		if (paramLen > 0) {
			parameters += "&";
		}
		parameters += "searchKeyword=" + searchKeyword;
		paramLen += 1;
	}

	if (startDate != "") {
		if (paramLen > 0) {
			parameters += "&";
		}
		parameters += "startDate=" + startDate;
		paramLen += 1;
	}

	if (endDate != "") {
		if (paramLen > 0) {
			parameters += "&";
		}
		parameters += "endDate=" + endDate;
		paramLen += 1;
	}


	if (statusList != "") {
		if (paramLen > 0) {
			parameters += "&";
		}
		parameters += "statusList=" + statusList;
		paramLen += 1;
	}


	if (deptList != "") {
		if (paramLen > 0) {
			parameters += "&";
		}
		parameters += "deptList=" + deptList;
		paramLen += 1;
	}

	if (paramLen > 0) {
		exportFile += "?" + parameters;
	}

	//console.log(exportFile);
	$.fileDownload(exportFile);

	//window.location = exportFile;

});