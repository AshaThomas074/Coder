// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    tinymce.init({
        selector: '#txtAreaQuestion'
    });

    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/java");

    $("#btnSubmit").on("click", function (event) {
        $("#hdnAnswer").val(editor.getValue());
    });
});
