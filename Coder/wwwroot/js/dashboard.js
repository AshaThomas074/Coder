// toggle class show hide text section
$(document).on('click', '.toggle-title', function () {
    $(this).parent()
        .toggleClass('toggled-on')
        .toggleClass('toggled-off');
});
