
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

    if ($("#ddlRole option:selected").text() == "Student") {
        $("#divBatch").show();
    }

    $("#ddlRole").on("change", function () {
        if ($("#ddlRole option:selected").text() == "Student") {
            $("#divBatch").show();
        }
        else {
            $("#divBatch").hide();
        }
    
         });

    $("#registerSubmit").on("click", function () {
        if ($("#ddlRole option:selected").text() == "Student") {
            if ($("#ddlBatch").val() == "") {
                return false;
            }
            $("#hdnStudBatchId").val($("#ddlBatch").val());
        }
        else {
            $("#hdnStudBatchId").val(0);
        }
    });


    $(".toggleSwitch").on("change", function () {
        if (this.checked) {
            contestId = $(this).attr("data-contestid");
            if (confirm("Are you sure you want to submit this contest?")) {

                $.ajax({
                    type: 'POST',
                    url: '/DashboardStudent/UpdateStudentContestFinishStatus?contestId=' + contestId,
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
            else {
                $(this).prop('checked', false);
                return false;
            }
        }
    });

    $(".toggle-contest-switch").on("change", function () {
        if (this.checked) {
            contestId = $(this).attr("data-contestid");
            if (confirm("Are you sure you want to publish this contest?")) {


                $.ajax({
                    type: 'POST',
                    url: '/Contests/PublishContest?contestId=' + contestId,
                    contentType: 'application/json',
                    dataType: 'json',
                    success: function (response) {
                        if (response != null && response == 1) {
                            $("#sliderContestRound_" + contestId).hide();
                            $("#status_" + contestId).text("Active");
                            $(".delete" + contestId).hide();
                        }
                    },
                    error: function (error) { console.log(error); }
                });
            }
            else {
                $(this).prop('checked', false);
                return false;
            }
        }
    });

    window.setTimeout(function () {
        $(".alert").hide();
    }, 5000); 

    $("#btnStudentView").on("click", function () {
        if ($("#ddlBatches").val() == "") {
            $("#spnValid").show();
            $("#divStudentList").hide();
            return false;
        }
    });

    

    $(".user-div").on("click", function () {
        var userid=$(this).children("div").attr("id").split("_")[1];
        var editor = ace.edit("editor_"+userid);
        editor.setTheme("ace/theme/monokai");
        editor.session.setMode("ace/mode/java");
        $("#editor_" + userid+"").show();        
    });
});
