$(document).ready(function () {

    var content = $('#MediaName').val();

    $('#MediaName').keyup(function () {
        if ($('#MediaName').val() != content) {
            content = $('#MediaName').val();
            $("#results").load("Media/PickerList?name=" + $("#MediaName").val() + "&type=" + $("#Types").val());

        }
    });

    $("#MediaName").change(function () {
    });

    $("#Types").change(function () {
        $("#results").load("Media/PickerList?name=" + $("#MediaName").val() + "&type=" + $("#Types").val());
    });
});

$(function () {
    $("[id^=Accept]").button();
    $("[id^=CancelChoose]").button();
    $('[id^=CancelChoose]').click(function () {
        var componentId = $('#componentId');
        window.parent.CloseMediaPicker(null, componentId.val());
    });
    $('[id^=Accept]').click(function () {
        var no = 0;

        var selections = $("[id^=select-]");

        selections = jQuery.unique(selections);
        var plist = [];
        selections.each(function () {
            if ($(this).attr('checked')) {
                no++;
                var img = $(this).parent().parent().find("img");
                var name = $(this).parent().find("[id^=hidden]");
                plist.push({ id: $(this).attr('id').split('-')[1], img: img.attr('src'), name: name.val() });
            }

        });
        var componentId = $('#componentId');
        window.parent.CloseMediaPicker(plist, componentId.val());
    });
});