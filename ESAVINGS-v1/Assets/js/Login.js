$(document).ready(function () {
	$('.modal').modal();
});

$("#Login_form").on("submit", function (e) {
	e.preventDefault();

	$("#user-login-loader").css("display", "block");

	var isDirectLabor = $("input[name='employeeType']:checked").val();


	console.log(isDirectLabor);

	if (typeof isDirectLabor !== "undefined") {
		$.post(
			base_url + "User/LoginAction",
			{
				ffID : $("#ffID").val(),
				password: $("#password").val(),
				isDirectLabor: isDirectLabor,
				isRememberUser: ($("#remember_me_checkbox").prop("checked") == true) ? 1 : 0
			},
			function (data) {
				console.log(data);

				//$("#login_form_msg").html(data.msg);
				//if (data.done === "TRUE") {
				//	setTimeout(function () {
				//		window.location.reload();
				//	}, 200);
				//}


				M.toast({
					html: "Processing...Please wait...",
					completeCallback: function () {
						M.toast({
							html: data.msg,
							completeCallback: function () {
								window.location.reload();
							},
							inDuration: 150,
							outDuration: 150,
							displayLength: 2000
						})

					},
					inDuration: 150,
					outDuration: 150,
					displayLength: 2000
				})
			}
		);
	} else {
		M.toast({
			html: "<span style='color:red'>Please provide employee type: DL or IDL</span>",
			completeCallback: function(){
				$("#user-login-loader").css("display", "none");
			},
			inDuration: 50,
			outDuration: 50,
			displayLength: 2000
		})
	}


});


$(".btn-register-or-reset-my-password").on("click", function () {
	
	var ffID = $("#OperatorFFID").val();

	$.post(
		base_url + "User/RegisterOrResetPassword",
		{
			ffID: ffID
		},
		function (data) {
			console.log(data);

			M.toast({
				html: "Processing...Please wait...",
				completeCallback: function () {
					M.toast({
						html: data.msg,
						completeCallback: function () {
							$("#OperatorFFID").val("");
							window.location.reload();
						}
					})

				}
			})
		}
	);

});