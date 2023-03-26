$(document).ready(function () {

    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/java");

    $("#btncodeRun").on("click", function (event) { 
        $("#hdnAnswer").val(editor.getValue());
    });
});