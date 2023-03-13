
$(document).ready(function () {

    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/java");

    editor.setValue($("#hdnAnswer").val());

    if ($("#TestCaseInput1").hasClass("input-validation-error"))
        $("#TestCaseInput1").removeClass("input-validation-error");

    tinymce.init({
        selector: '#txtAreaQuestion'
    });

    $("#btnSubmit").on("click", function (event) {
        if ($("#TestCaseInput1").hasClass("input-validation-error"))
            $("#TestCaseInput1").removeClass("input-validation-error");
        $("#hdnAnswer").val(editor.getValue());
    });
});