$(document).ready(function () {


    var componentId, plist;
    
    var content = $('#MediaName').val();

    $('#MediaName').keyup(function () {
        if ($('#MediaName').val() != content) {
            content = $('#MediaName').val();
            $("#results").load("../Media/PickerList?name=" + $("#MediaName").val() + "&type=" + $("#Types").val() + "&theme=" + $("#Themes").val());

        }
    });

    $("#MediaName").change(function () {
    });


    $("#Themes").change(function () {
        $("#results").load("../Media/PickerList?name=" + $("#MediaName").val() + "&type=" + $("#Types").val() + "&theme=" + $("#Themes").val());
    });

    $("#Types").change(function () {
        $("#results").load("../Media/PickerList?name=" + $("#MediaName").val() + "&type=" + $("#Types").val() + "&theme=" + $("#Themes").val());
    });
});

$(function () {
    $("[id^=Accept]").button();
    $("[id^=CancelChoose]").button();
    $('[id^=CancelChoose]').click(function () {
        window.componentId = $('#componentId');
        window.parent.CloseMediaPicker(null, window.componentId.val());
    });
    $('[id^=Accept]').click(function () {
        var no = 0;

        var selections = $("[id^=select-]");

        selections = jQuery.unique(selections);
        window.plist = [];
        selections.each(function () {
            if ($(this).attr('checked')) {
                no++;
                var img = $(this).parent().parent().find("img");
                var name = $(this).parent().find("[id^=hidden]");
                window.plist.push({ id: $(this).attr('id').split('-')[1], img: img.attr('src'), name: name.val() });
            }

        });
        window.componentId = $('#componentId');
        LoadResizer();
    });

    function LoadResizer() {
        //Load Resizer, if failed close
        ShowModalPage("ResizeImage", 'ResizeImage', '../Media/ResizeAndSave?id=' + plist[0].id + "&width=" + $("#positionWidth").val() + "&height=" + $("#positionHeight").val(), null, LoadFailed);

        function LoadFailed() {
            $('#ResizeImage').dialog('destroy');
            $('#ResizeImage').remove();
            window.parent.CloseMediaPicker(plist, componentId.val());
        }

        function LoadComplete(newImage) {
            $('#ResizeImage').dialog('destroy');
            $('#ResizeImage').remove();
            plist[0].img = newImage;
            window.parent.CloseMediaPicker(plist, componentId.val());
        }
    }
});