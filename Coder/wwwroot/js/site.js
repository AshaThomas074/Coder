
$(document).ready(function () {

    //code for contest start

    $("#spnValid").hide();

    $("#btnContestSubmit").on("click", function () {
        if ($("#ddlQuestion").val() == "") {
            $("#ddlQuestion").addClass("input-validation-error");
            $("#ddlQuestion").attr("aria-describedby", "Difficulty-error");
            $("#ddlQuestion").attr("aria-invalid", "true");
            $("#spnValid").show();
            return false;
        }
    });

    $("#ddlQuestion").on("change", function () {
        if ($("#ddlQuestion").val() == "") {
            $("#ddlQuestion").addClass("input-validation-error");
            $("#ddlQuestion").attr("aria-describedby", "Difficulty-error");
            $("#ddlQuestion").attr("aria-invalid", "true");
            $("#spnValid").show();
            return false;
        }
        else {
            $("#spnValid").hide();
        }
    });

    //code for contest ends

    $("#divBatch").hide();

    $("#ddlRole").on("change", function () {
        if ($("#ddlRole option:selected").text() == "Student") {
            $("#divBatch").show();
        }
    
         });

    $("#registerSubmit").on("click", function () {
        if ($("#ddlRole option:selected").text() == "Student") {
            if ($("#ddlBatch").val() == "") {
                return false;
            }
        }
    });

});
