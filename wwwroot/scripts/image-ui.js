$('#photoDiv').on('click', function () {
	$('#photoFile').click();
});

$("#photoFile").change(function () {
	const inputElement = $("#photoFile");
	const errorElement = $("#fileError");
	errorElement.html("");
	const file = this.files[0];
	const fileSize = file.size;
	if (fileSize > 300 * 1024) {
		errorElement.html("Размер файла не может превышать 300KB");
		inputElement.wrap('<form>').parent('form').trigger('reset');
		inputElement.unwrap();
		return;
	}
	const reader = new FileReader();
	reader.onloadend = function () {
		$("#photoDiv").css("background-image", `url(${this.result})`);
	}
	if (file) {
		reader.readAsDataURL(file);
	}
});