$(document).ready(function () {
    var lang = $("#language").val();
    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/" + lang);

});