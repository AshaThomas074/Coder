
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


    $(".toggleSwitch").on("change",function () {
        if (this.checked) {
            contestId = $(this).attr("data-contestid");
                       
            $.ajax({
                type: 'POST',
                url: '/SolveQuestion/UpdateStudentContestFinishStatus?contestId=' + contestId,
                contentType: 'application/json',
                dataType: 'json',                
                success: function (response) {
                    if (response != null && response == 1) {
                        $("#sliderRound_" + contestId).hide();
                    }
                },
                error: function (error) { console.log(error); }
            });
        } 
    });

});
