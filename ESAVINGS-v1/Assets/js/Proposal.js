

$(document).ready(function () {

	$('#currentImgsCarousel').carousel({
		fullWidth: true
	});

	$('#proposalImgsCarousel').carousel({
		fullWidth: true
	});

	$('select').formSelect();
});


$("#browse_image_for_current").on("click", function () {
	$("#current_images").click();
});


$("#browse_image_for_proposal").on("click", function () {
	$("#proposal_images").click();
});


function preview_situation_image(event, id_for_display) {
	var reader = new FileReader();

	reader.onload = function () {
		var output = document.getElementById(id_for_display.toString());

		var fileType = event.target.files[0].type;
		var fileTypeArr = fileType.split("/");

		if (fileTypeArr[0] === "image") {
			output.src = reader.result;
		} else if (fileTypeArr[0] === "application") {
			output.src = "assets/imgs/no-image-available.png";
		} else if (fileTypeArr[0] === "text") {
			output.src = "assets/imgs/no-image-available.png";
		} else {
			output.src = "assets/imgs/no-image-available.png";
		}
	}

	reader.readAsDataURL(event.target.files[0]);
	$(event.target.files).val("");
}


var current_images = [];



$("#current_images").on("change", function (e) {

	preview_situation_image(e, "image_for_current");
	current_images = [];
	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		current_images.push($(this)[0].files[i]);
	}

});


var proposal_images = [];


$("#proposal_images").on("change", function (e) {

	preview_situation_image(e, "image_for_proposal");

	proposal_images = [];
	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		proposal_images.push($(this)[0].files[i]);
	}

});




$(".btn-browse-supporting-documents").on("click", function () {
	$("#supporting_documents").click();
});

var supporting_docs = [];

function display_browse_supporting_docs() {
	var display = "<div class='md-chips'>";
	for (var i = 0; i < supporting_docs.length; i++) {
		display += "<div class='md-chip'>";
		display += "<span>" + supporting_docs[i].name + "</span>";
		display += "<button type='button' class='md-chip-remove btn-remove-supporting-docs' data-idx='" + i + "'></button>";
		display += "</div>";
	}
	display += "</div>";

	$("#supporting_documents_attachments").html(display);
}

$("#supporting_documents").on("change", function (e) {

	var fileLen = $(this)[0].files.length;

	for (var i = 0; i < fileLen; i++) {
		supporting_docs.push($(this)[0].files[i]);
	}
	display_browse_supporting_docs();

});

$(document).on("click", ".btn-remove-supporting-docs", function () {
	var indx = $(this).attr("data-idx");

	supporting_docs.splice(indx, 1);
	display_browse_supporting_docs();
});



$(document).on("click", ".btn-delete-supporting-docs", function () {

	var id = $(this).attr("data-id");

	var ans = confirm("Are you sure you want to delete item?");

	if (ans) {
		$.post(
			base_url + "ESavings/DeleteProposalFilesBy",
			{
				target: "supporting",
				fileID: id,
				proposalID: global_proposal_id
			},
			function (data) {
				M.toast({
					html: data.msg,
					completeCallback: function () {
						$.post(
							base_url + "ESavings/GetProposalSupportingDocs",
							{
								proposalID: global_proposal_id
							},
							function (data) {
								//console.log(data);

								var display = "<div class='md-chips'>";
								$.each(data, function (i, doc) {
									display += "<div class='md-chip'>";
									display += "<span>" + doc.OrigFileName + "</span>";
									display += "<button type='button' class='md-chip-remove btn-delete-supporting-docs' data-id='" + doc.Id + "'></button>";
									display += "</div>";
								});
								display += "</div>";
								$("#supporting_documents_attachments").html(display);

							}
						);
					}
				})
			}
		);
	}

});


//$(document).ready(function () {
//	const elem = document.getElementById('submissionConfirmationModal');
//	const instance = M.Modal.init(elem, { dismissible: false });

//	instance.open();
//});

$("#esavings_proposal_form").on("submit", function (e) {

	e.preventDefault();

	var neededAction = $(this).attr("data-needed-action");


	$("#submit-esavings-proposal-loader").css("display", "block");
	$(".btn-submit-esavings-proposal").addClass("disabled");

	var formData = new FormData();

	formData.append("AreaDept", $("#Project_dept_area_beneficiary").val());
	formData.append("ProjectTitle", $("#Project_title").val());
	formData.append("CurrentDescription", $("#current_description").val());
	formData.append("ProposalDescription", $("#propose_description").val());
	formData.append("Remarks", $("#user_remarks").val());
	

	if (isDL === "false") {
		formData.append("ProjectType", $("input[name='project_type']:checked").attr("data-project-type"));
		formData.append("NumberOfMonthsToBeActive", $("#NumberOfMonthsToBeActive").val());
		formData.append("DollarImpact", $("#DollarImpact").val());
		formData.append("ExpectedStartDate", $("#ExpectedStartDate").val());
	}

	formData.append("supporting_docs_len", supporting_docs.length);
	for (var i = 0; i < supporting_docs.length; i++) {
		formData.append("supporting_docs_" + i, supporting_docs[i]);
	}


	if (neededAction === "create_new_proposal" && global_proposal_id == 0) {

		formData.append("current_imgs_len", current_images.length);
		for (var i = 0; i < current_images.length; i++) {
			formData.append("current_imgs_" + i, current_images[i]);
		}


		formData.append("proposal_imgs_len", proposal_images.length);
		for (var i = 0; i < proposal_images.length; i++) {
			formData.append("proposal_imgs_" + i, proposal_images[i]);
		}


		var request = $.ajax({
			url: base_url + "ESavings/Submit_Proposal",
			type: "POST",
			data: formData,
			contentType: false,
			cache: false,
			processData: false
		});

		request.done(function (data) {
			console.log(data);


			M.toast({
				html: "Sending...Please wait...",
				completeCallback: function () {

					$("#submit-esavings-proposal-loader").css("display", "none");
					$(".btn-submit-esavings-proposal").removeClass("disabled");


					M.toast({
						html: data.msg,
						completeCallback: function () {
							//$("#image_for_current").attr("src", "assets/imgs/no-image-available.png");
							//$("#image_for_proposal").attr("src", "assets/imgs/no-image-available.png");
							//$("#Project_title").val("");
							//$("#current_description").val("");
							//$("#propose_description").val("");
							//$("#user_remarks").val("");
							//current_images = [];
							//proposal_images = [];
							//supporting_docs = [];

							//$("#supporting_documents_attachments").html("");

							if (data.done === "TRUE") {
								//window.location = base_url + "Home/Details/" + data.proposalID;

								$("#btn_view_proposal_details").attr("href", base_url + "Home/Details/" + data.proposalID);

								const elem = document.getElementById('submissionConfirmationModal');
								const instance = M.Modal.init(elem, { dismissible: false });

								instance.open();
							}

						}
					})

				}
			})

		});

	} else if (neededAction === "update_proposal" && global_proposal_id > 0) {

		formData.append("proposalID", global_proposal_id);

		var request = $.ajax({
			url: base_url + "ESavings/Update_Proposal",
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

					$("#submit-esavings-proposal-loader").css("display", "none");
					$(".btn-submit-esavings-proposal").removeClass("disabled");


					M.toast({
						html: data.msg,
						completeCallback: function () {
							window.location.reload();
						}
					})

				}
			})

		});

	}



	


});






$(document).ready(function () {

	$('#currentImgsCarousel').carousel({
		fullWidth: true
	});

	$('#proposalImgsCarousel').carousel({
		fullWidth: true
	});


	//function display_proposal_imgs_here(data, path) {

	//	var display = "";

	//	if (data.length > 1) {

	//		$.each(data, function (i, img) {
	//			display += "<a class='carousel-item' href='#" + img.Id + "'><img src='" + path + img.ServerFileName + "'></a>"
	//		});

	//		$('#edit_proposal_display_imgs').html(display);

	//		$('#edit_proposal_display_imgs_carousel').carousel({
	//			fullWidth: true,
	//			indicators: true
	//		});

	//	} else {

	//		$.each(data, function (i, img) {
	//			display += "<img class='materialboxed' id='image_for_current' src='" + path + img.ServerFileName + "'>";
	//		});

	//		$(".edit_proposal_display_imgs").html(display);

	//	}
	//}


	function display_proposal_imgs_in_table(data, target) {
		var display = "";

		$.each(data, function (i, img) {
			display += "<tr>";
			display += "<td>" + img.OrigFileName + "</td>";
			display += "<td><a href='#delete-" + img.Id + "-" + target + "' class='delete-this-proposal-imgs-by-id' id='" + img.Id + "' data-target='" + target + "'>Delete</a></td>";
			display += "</tr>";
		});

		$("#update_proposal_imgs").html(display);
	}


	function Get_proposal_current_imgs() {
		$.post(
			base_url + "ESavings/GetProposalCurrentImgs",
			{
				proposalID: global_proposal_id
			},
			function (data) {
				//display_proposal_imgs_here(data, dirCurrentImgs);
				display_proposal_imgs_in_table(data, "current");
			}
		);
	}

	function Get_proposal_imgs() {
		$.post(
			base_url + "ESavings/GetProposalImgs",
			{
				proposalID: global_proposal_id
			},
			function (data) {
				//display_proposal_imgs_here(data, dirProposalImgs);
				display_proposal_imgs_in_table(data, "proposal");
			}
		);
	}


	$('#update_proposal_imgs_modal').modal(
		{
			dismissible: true, // Modal can be dismissed by clicking outside of the modal
			onOpenEnd: function (modal, trigger) { // Callback for Modal open. Modal and trigger parameters available.

				var action = $(trigger).attr("data-action");

				$(modal).attr("data-action-needed", action);

				//console.log(global_proposal_id);
				console.log(modal, trigger);

				$('#edit_proposal_display_imgs').html("");

				
				if (action === "update_current_imgs") {
					Get_proposal_current_imgs();
				} else if (action === "update_proposal_imgs") {
					Get_proposal_imgs();
				}

				$(document).on("click", ".delete-this-proposal-imgs-by-id", function () {

					var imgId = $(this).attr("id");
					var target = $(this).attr("data-target");

					var ans = confirm("Continue to delete this item?");

					if (ans) {

						console.log(imgId, target);

						$.post(
							base_url + "ESavings/DeleteProposalFilesBy",
							{
								target: target,
								fileID: imgId,
								proposalID: global_proposal_id
							},
							function (data) {
								console.log(data);

								M.toast({
									html: data.msg,
									completeCallback: function () {

										if (target == "current") {
											Get_proposal_current_imgs();
										} else if (target == "proposal") {
											Get_proposal_imgs();
										}

									}
								})
							}
						);

					}




				});


				//console.log($(trigger).attr("data-action"));
			}
			//,
			//onCloseEnd: function () { // Callback for Modal close
			//	alert('Closed');
			//} 
		}
	);


	$("#browse_additional_imgs").on("click", function () {
		$("#additional_imgs").click();
	});


	var additional_images = [];


	$("#additional_imgs").on("change", function (e) {

		//preview_situation_image(e, "additional_images");
		additional_images = [];
		var fileLen = $(this)[0].files.length;

		for (var i = 0; i < fileLen; i++) {
			additional_images.push($(this)[0].files[i]);
		}

		display_browse_additional_imgs();
	});

	function display_browse_additional_imgs() {

		var display = "";

		for (var i = 0; i < additional_images.length; i++) {
			display += "<tr>";
			display += "<td>" + additional_images[i].name + "</td>";
			display += "<td><a href='#remove-addtional-img-" + i + "' class='remove-this-additional-imgs-by-idx' id='" + i + "'>Remove</a></td>";
			display += "</tr>";
		}

		$("#update_proposal_imgs").html(display);

	}

	$(document).on("click", ".remove-this-additional-imgs-by-idx", function (e) {
		e.preventDefault();
		var idx = $(this).attr("id");
		//console.log(idx);
		var remove = additional_images.splice(idx, 1);
		display_browse_additional_imgs();

		//console.log(remove);
		//console.log(additional_images);
	});



	$("#btn_upload_proposal_additional_imgs_by_target").on("click", function () {

		var target_action = $("#update_proposal_imgs_modal").attr("data-action-needed");

		var formData = new FormData();

		console.log(additional_images);

		if (target_action === "update_proposal_imgs") {
			formData.append("target", "proposal_imgs");

			formData.append("filesLen", additional_images.length);
			for (var i = 0; i < additional_images.length; i++) {
				formData.append("proposal_imgs_" + i, additional_images[i]);
			}


		} else if (target_action === "update_current_imgs") {

			formData.append("target", "current_imgs");
			formData.append("filesLen", additional_images.length);
			for (var i = 0; i < additional_images.length; i++) {
				formData.append("current_imgs_" + i, additional_images[i]);
			}


		}

		formData.append("proposalID", global_proposal_id);

		if (additional_images.length > 0) {

			var request = $.ajax({
				url: base_url + "ESavings/UploadProposalFilesBy",
				type: "POST",
				data: formData,
				contentType: false,
				cache: false,
				processData: false
			});

			request.done(function (data) {

				console.log(data);


				M.toast({
					html: data.msg,
					completeCallback: function () {

						if (target_action === "update_current_imgs") {
							Get_proposal_current_imgs();
						} else if (target_action === "update_proposal_imgs") {
							Get_proposal_imgs();
						}

					}
				})

			});

		}


	});

});



