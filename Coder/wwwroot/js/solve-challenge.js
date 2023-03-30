var languageList = [];
var lang;
$(document).ready(function () {    
    var editor = ace.edit("editor");
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/java");

    getLanguageList();

    //editor.setValue($("#hdnAnswer").val());

    getSelectedLanguage();
    setEditorValue();

    $("#codelanguage").on("change", function () {  
        getSelectedLanguage();
        setEditorValue();
    });

    $("#btncodeRun").on("click", function (event) { 
        $("#hdnSubmissionContent").val(editor.getValue());
    });


    function getLanguageList() {
        $.ajax({
            type: 'GET',
            url: 'GetLanguageList',
            dataType: "json",
            success: function (response) {
                if (response != null)
                    languageList = response;

            },
            error: function (error) { console.log(error); }
        });
    }

    function setEditorValue() {
        if (languageList.length == 0)
            getLanguageList();
        var res = languageList.find(item => item.languageId === lang);
        console.log(res);
        editor.setValue(res.initialCode);
    }

    function getSelectedLanguage() {
        $("#hdnSelectedLanguage").val($("#codelanguage :selected").val());
        lang = $("#hdnSelectedLanguage").val();
    }

});

