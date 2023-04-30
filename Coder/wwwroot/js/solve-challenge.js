var languageList = [];
var lang;
var viewModel = {};

var editor = ace.edit("editor");
editor.setTheme("ace/theme/monokai");
editor.session.setMode("ace/mode/java");

getLanguageList();    
    
if (languageList.length == 0) {
    getLanguageList();
}

window.setTimeout(function () {
    getSelectedLanguage();
    if (!IsCompiled)
        setEditorValue();

    $("#codelanguage").on("change", function (e) {
        getSelectedLanguage();
        e.preventDefault();
        e.stopPropagation();
        setEditorValue();
    });

    $("#btncodeRun").on("click", function (e) {      
        $("#hdnSubmissionContent").val(editor.getValue());       
    });
}, 0); 


function getLanguageList() {
    $.ajax({
        type: 'GET',
        url: 'GetLanguageList',
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            if (response != null) {
                languageList = response;
                console.log(languageList);
            }
        },
        error: function (error) { console.log(error); }
    });
}

function setEditorValue() {
    if (languageList.length == 0)
        getLanguageList();
    $.each(languageList, function (key, value) {
        if (value.languageId == lang) {
            editor.session.setMode("ace/mode/" + value.aceLanguageCode);
            editor.setValue(value.initialCode);
            $("#hdnLangCode").val(value.jDoodleLanguageCode);
            $("#hdnVersionIndex").val(value.versionIndex);
        }
    });
}

function getSelectedLanguage() {
    $("#hdnSelectedLanguage").val($("#codelanguage :selected").val());
    lang = $("#hdnSelectedLanguage").val();
    if ($("#hdnLangCode").val() == "") {
        $.each(languageList, function (key, value) {
            if (value.languageId == lang) {
                $("#hdnLangCode").val(value.jDoodleLanguageCode);
                $("#hdnVersionIndex").val(value.versionIndex);
            }
        });
    }
}
