$('.carousel.carousel-slider').carousel({
	fullWidth: true
});



$(document).ready(function () {
	$('.modal').modal();
	$('.collapsible').collapsible();
	$('.tabs').tabs();
	$('select').formSelect();
});


$(".btn-browse-cost-analyst-supporting-documents").on("click", function () {
	$("#cost_analyst_supporting_documents_file_browser").click();
});

var cost_analyst_supporting_docs = [];


function display_cost_analyst_supporting_documents() {
	var display = "<div class='md-chips'>";
	for (var i = 0; i < cost_analyst_supporting_docs.length; i++) {
		display += "<div class='md-chip'>";
		display += "<span>" + cost_analyst_supporting_docs[i].name + "</span>";
		display += "<button type='button' class='md-chip-remove btn-remove-cost-analyst-supporting-docs' data-idx='" + i + "'></button>";
		display += "</div>";
	}
	display += "</div>";

	$("#cost_analyst_supporting_documents").html(display);
}

$("#cost_analyst_supporting_documents_file_browser").on("change", function (e) {

	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		cost_analyst_supporting_docs.push($(this)[0].files[i]);
	}
	display_cost_analyst_supporting_documents();
});



$(document).on("click", ".btn-remove-cost-analyst-supporting-docs", function () {

	var idx = $(this).attr("data-idx");

	if (idx != "") {

		cost_analyst_supporting_docs.splice(idx, 1);
		display_cost_analyst_supporting_documents();
	}

});



$(".btn-cost-analyst-verification").on("click", function () {


	$("#cost-analyst-verification-loader").css("display", "block");

	var costAnalystID = $(this).attr("data-cost-analyst-id");
	var verificationID = $(this).attr("data-proposal-cost-analyst-verification-id");
	var remarks = $("#deptCostAnalystComments_" + verificationID).val();
	var projectType = $("input[name='project_type_" + verificationID + "']:checked").attr("data-project-type");
	var dollarImpact = $("#dollarImpact_" + verificationID).val();
	var expectedStartDate = $("#expectedStartDate_" + verificationID).val();
	var numberOfMonthsToBeActive = $("#numberOfMonthsToBeActive_" + verificationID).val();
	var sltdFinanceApprverFFID = $("#finance_approver_" + verificationID).val();

	var formData = new FormData();

	formData.append("proposalID", global_proposal_id);
	formData.append("remarks", remarks);
	formData.append("isVerified", $(this).attr("data-status"));
	formData.append("OAStatus", $(this).attr("data-oa-status"));
	formData.append("projectType", projectType);
	formData.append("dollarImpactStr", dollarImpact);
	formData.append("expectedStartDate", expectedStartDate);
	formData.append("numberOfMonthsToBeActiveStr", numberOfMonthsToBeActive);
	formData.append("selectedFinanceFFID", sltdFinanceApprverFFID);
	formData.append("costAnalystID", costAnalystID);

	formData.append("supportingDocsLen", cost_analyst_supporting_docs.length);
	for (var i = 0; i < cost_analyst_supporting_docs.length; i++) {
		formData.append("supporting_docs_" + i, cost_analyst_supporting_docs[i]);
	}


	var request = $.ajax({
		url: base_url + "ESavings/CostAnalystApproval",
		type: "POST",
		data: formData,
		contentType: false,
		cache: false,
		processData: false
	});

	request.done(function (data) {
		//console.log(data);

		M.toast({
			html: "Processing...Please wait...",
			completeCallback: function () {
				M.toast({
					html: data.msg,
					completeCallback: function () {
						window.location.reload();
					}
				})

			}
		})

	});

});


$("#search_action_owner").on("keypress", function (e) {

	if (e.which == 13) {

		$("#verifier-assign-action-to-owner-loader").css("display", "block");

		var searchStr = $(this).val();

		$.post(
			base_url + "User/SearchEmployee",
			{
				searchStr: searchStr
			},
			function (data) {
				console.log(data);

				var display = "";

				$.each(data, function (i, person) {
					display += "<p>";
					display += "<label>";
					display += "<input type='radio' class='selected_owner' data-owner-ffid='" + person.FFID + "' data-owner-fullname='" + person.DisplayName +"' name='action_owner'/>";
					display += "<span>" + person.DisplayName + "/" + person.FFID +"</span>";
					display += "</label>";
					display += "</p>";
				});

				$("#search_employee_list").html(display);
				$("#verifier-assign-action-to-owner-loader").css("display", "none");

			}
		);

	}

});


$(".btn-manager-verification").on("click", function () {

	$("#manager-verification-loader").css("display", "block");

	var status = $(this).attr("data-status");
	var verificationID = $(this).attr("data-proposal-manager-verification-id");
	var remarks = $("#deptManagerVerifier_" + verificationID).val();

	$.post(
		base_url + "ESavings/ManagerProposalVerification",
		{
			proposalID: global_proposal_id,
			remarks: remarks,
			isVerified: status
		},
		function (data) {
			//console.log(data);

			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})
		}
	);

});


var actionOwnerInfo = {
	ffID: "",
	fullName: ""
}

$(document).on("click", ".selected_owner", function () {
	var fullName = $(this).attr("data-owner-fullname");
	var ffID = $(this).attr("data-owner-ffid");

	actionOwnerInfo.ffID = ffID;
	actionOwnerInfo.fullName = fullName;
});


$("#btn_assign_action_to_employee").on("click", function () {

	$("#verifier-assign-action-to-owner-loader").css("display", "block");

	var action = $("#proposal_action_description").val();

	
	$.post(
		base_url + "ESavings/CreateNewAction",
		{
			proposalID: global_proposal_id,
			actionDesc: action,
			ownerFFID: actionOwnerInfo.ffID,
			ownerFullname: actionOwnerInfo.fullName
		},
		function (data) {
			//console.log(data);

			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {

					$("#verifier-assign-action-to-owner-loader").css("display", "none");

					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})

			
		}
	);


});









$(".btn-browse-supporting-documents").on("click", function () {
	$("#supporting_documents").click();
});

var supporting_docs = [];

$("#supporting_documents").on("change", function (e) {

	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		supporting_docs.push($(this)[0].files[i]);
	}

	var display = "";
	for (var i = 0; i < supporting_docs.length; i++) {

		display += "<div class='chip'>";
		display += supporting_docs[i].name + "<i class='close material-icons'>close</i>";
		display += "</div>";
	}

	$("#action_supporting_documents").html(display);

});



$(".btn-action-owner-save-response").on("click", function () {

	$("#approver-verification-loader").css("display", "block");

	var formData = new FormData();

	var remarks = $("#assignees_remarks").val();
	var actionID = $(this).attr("data-action-id");

	formData.append("proposalID", global_proposal_id);
	formData.append("actionID", actionID);
	formData.append("remarks", remarks);

	formData.append("supportingDocsLen", supporting_docs.length);
	for (var i = 0; i < supporting_docs.length; i++) {
		formData.append("supporting_docs_" + i, supporting_docs[i]);
	}

	var request = $.ajax({
		url: base_url + "ESavings/SaveActionOwnerResponse",
		type: "POST",
		data: formData,
		contentType: false,
		cache: false,
		processData: false
	});

	request.done(function (data) {
		//console.log(data);


		M.toast({
			html: "Processing...Please wait...",
			completeCallback: function () {
				M.toast({
					html: data.msg,
					completeCallback: function () {
						window.location.reload();

					}
				})

			}
		})
		
	});

});




$(".btn-approver-action-verification").on("click", function () {


	$("#approver-verification-loader").css("display", "block");

	var actionID = $(this).attr("data-action-id");
	var status = $(this).attr("data-status");
	var remarks = $("#verifierRemarks_" + actionID).val();


	$.post(
		base_url + "ESavings/ProposalActionApproval",
		{
			proposalID: global_proposal_id,
			actionID: actionID,
			verifierRemarks: remarks,
			verificationStatus: status
		},
		function (data) {
			//console.log(data);

			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();

						}
					})

				}
			})
		}
	);

});



//$(".btn-cost-site-incharge-verification").on("click", function () {

//	var costSiteInCharageVerificationID = $(this).attr("data-proposal-cost-site-in-charge-verification-id");
//	var status = $(this).attr("data-status");
//	var remarks = $("#costSiteInChargeRemarks").val();

//	$("#cost-site-incharge-verification-loader").css("display", "block");

//	$.post(
//		base_url + "ESavings/ProposalClosureApproval",
//		{
//			proposalID: global_proposal_id,
//			costSiteInChargeVerificationID: costSiteInCharageVerificationID,
//			remarks: remarks,
//			verificationStatus: status
//		},
//		function (data) {
//			console.log(data);

//			M.toast({
//				html: "Processing...Please wait...",
//				completeCallback: function () {
//					M.toast({
//						html: data.msg,
//						completeCallback: function () {
//							window.location.reload();

//						}
//					})

//				}
//			})
//		}
//	);

//});




$(".btn-browse-finance-supporting-documents").on("click", function () {
	$("#finance_supporting_documents_file_browser").click();
});

var finance_supporting_docs = [];


function display_finance_supporting_documents() {
	var display = "<div class='md-chips'>";
	for (var i = 0; i < finance_supporting_docs.length; i++) {
		display += "<div class='md-chip'>";
		display += "<span>" + finance_supporting_docs[i].name + "</span>";
		display += "<button type='button' class='md-chip-remove btn-remove-finance-supporting-docs' data-idx='" + i + "'></button>";
		display += "</div>";
	}
	display += "</div>";

	$("#finance_supporting_documents").html(display);
}

$("#finance_supporting_documents_file_browser").on("change", function (e) {

	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		finance_supporting_docs.push($(this)[0].files[i]);
	}
	display_finance_supporting_documents();
});



$(document).on("click", ".btn-remove-finance-supporting-docs", function () {

	var idx = $(this).attr("data-idx");

	if (idx != "") {

		finance_supporting_docs.splice(idx, 1);
		display_finance_supporting_documents();
	}

});

$(".btn-finance-verification").on("click", function () {


	$("#finance-verification-loader").css("display", "block");

	var financeApproverID = $(this).attr("data-finance-approver-id");
	var verificationID = $(this).attr("data-proposal-finance-verification-id");
	var remarks = $("#financeRemarks").val();
	var projectType = $("input[name='finance_project_type']:checked").attr("data-project-type");

	var dollarImpact = $("#finance_dollarImpact").val();
	var expectedStartDate = $("#finance_expectedStartDate").val();
	var numberOfMonthsToBeActive = $("#finance_numberOfMonthsToBeActive").val();

	var formData = new FormData();

	formData.append("proposalID", global_proposal_id);
	formData.append("remarks", remarks);
	formData.append("projectType", projectType);
	formData.append("isVerified", $(this).attr("data-status"));
	formData.append("OAStatus", $(this).attr("data-oa-status"));
	formData.append("dollarImpactStr", dollarImpact);
	formData.append("numberOfMonthsToBeActiveStr", numberOfMonthsToBeActive);
	formData.append("expectedStartDate", expectedStartDate);
	formData.append("financeApproverID", financeApproverID);

	formData.append("supportingDocsLen", finance_supporting_docs.length);
	for (var i = 0; i < finance_supporting_docs.length; i++) {
		formData.append("supporting_docs_" + i, finance_supporting_docs[i]);
	}


	var request = $.ajax({
		url: base_url + "ESavings/FinanceApproval",
		type: "POST",
		data: formData,
		contentType: false,
		cache: false,
		processData: false
	});

	request.done(function (data) {
		console.log(data);

		M.toast({
			html: "Processing...Please wait...",
			completeCallback: function () {
				M.toast({
					html: data.msg,
					completeCallback: function () {
						window.location.reload();
					}
				})

			}
		})

	});

});

//$("#reassign-project-to-cost-analyst-modal").modal('open');

// ######################################
// Reassign Cost-Analyst
//

$(".btn-reassign-project-to-new-cost-analyst").on("click", function () {
	const elem = document.getElementById('reassign-project-to-cost-analyst-modal');
	const instance = M.Modal.init(elem, { dismissible: true });

	var cost_analyst_id = $(this).attr("data-current-cost-analyst-id");
	$("#btn_assign_project_to_new_cost_analyst").attr("data-current-cost-analyst-id", cost_analyst_id);
	instance.open();
});


$("#btn_assign_project_to_new_cost_analyst").on("click", function () {

	var remarks = $("#current_cost_analyst_remarks").val();
	var newCostAnalystFFID = $("#cost_analyst_approver").val();
	var cost_analyst_id = $(this).attr("data-current-cost-analyst-id");


	$("#verifier-reassign-cost-analyst-loader").css("display", "block");


	$.post(
		base_url + "ESavings/ReAssignProjectCostAnalyst",
		{
			"proposalID": global_proposal_id,
			"currentCostAnalystID": cost_analyst_id,
			"newCostAnalystFFID": newCostAnalystFFID,
			"remarks": remarks
		},
		function (data) {
			//console.log(data);
			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})
		}
	);

});

// ######################################
// Reassign Finance
//


$(".btn-reassign-project-to-new-finance").on("click", function () {
	const elem = document.getElementById('reassign-project-to-finance-modal');
	const instance = M.Modal.init(elem, { dismissible: true });

	var finance_id = $(this).attr("data-current-finance-id");
	$("#btn_assign_project_to_new_finance").attr("data-current-finance-id", finance_id);
	instance.open();
});


$("#btn_assign_project_to_new_finance").on("click", function () {

	var remarks = $("#current_finance_remarks").val();
	var newFinanceFFID = $("#finance_approver").val();
	var curFinanceID = $(this).attr("data-current-finance-id");

	$.post(
		base_url + "ESavings/ReAssignProjectFinace",
		{
			"proposalID": global_proposal_id,
			"currentFinanceID": curFinanceID,
			"newFinanceFFID": newFinanceFFID,
			"remarks": remarks
		},
		function (data) {
			//console.log(data);

			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})
		}
	);

})


$("#btn-mark-proposal-as-bpi").on("change", function () {
	if (this.checked === true) {
		
		$.post(
			base_url + "ESavings/MarkProposalAsBPI",
			{
				"proposalID": global_proposal_id,
				"status": 1
			},
			function (data) {
				//console.log(data);

				$("#btn-mark-proposal-as-bpi").attr("disabled", "disabled");
				M.toast({
					html: "Processing...Please wait...",
					completeCallback: function () {
						M.toast({
							html: data.msg,
							completeCallback: function () {
								window.location.reload();
							}
						})

					}
				})
			}
		);

	} else {


		$.post(
			base_url + "ESavings/MarkProposalAsBPI",
			{
				"proposalID": global_proposal_id,
				"status": 0
			},
			function (data) {
				//console.log(data);

				$("#btn-mark-proposal-as-bpi").attr("disabled", "disabled");
				M.toast({
					html: "Processing...Please wait...",
					completeCallback: function () {
						M.toast({
							html: data.msg,
							completeCallback: function () {
								window.location.reload();
							}
						})

					}
				})
			}
		);


	}
});


$("#btn_lets_update_proposal_details").on("click", function () {
	const elem = document.getElementById('updateProposalWarningModal');
	const instance = M.Modal.init(elem);

	instance.open();
})


$("#btn-qlik-view-save-details").on("click", function () {

	$("#qlik-view-forms-loader").css("display", "block");

	var financeCategory = $("#financeCategory").val();
	var origDueDate = $("#qlik_view_original_due_date").val();
	var curDueDate = $("#qlik_view_current_due_date").val();
	var plannedProjectStartDate = $("#qlik_view_planned_project_start_date").val();
	var plannedSavingStartDate = $("#qlik_view_planned_saving_start_date").val();
	var actualCompletionDate = $("#qlik_view_actual_completion_date").val();
	var globalFunnelStatus = $("#globalFunnelStatus").val();


	var data = {
		proposalID: global_proposal_id,
		FinanceCategoryId: financeCategory,
		OriginalDueDate: origDueDate,
		CurrentDueDate: curDueDate,
		PlannedProjectStartDate: plannedProjectStartDate,
		PlannedSavingStartDate: plannedSavingStartDate,
		ActualCompletionDate: actualCompletionDate,
		GlobalFunnelStatusIndicator: globalFunnelStatus
	}


	$.post(
		base_url + "ESavings/UpdateQlikViewData",
		data,
		function (data) {
			console.log(data);

			$("#btn-qlik-view-save-details").attr("disabled", "disabled");
			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})
		}
	);

});