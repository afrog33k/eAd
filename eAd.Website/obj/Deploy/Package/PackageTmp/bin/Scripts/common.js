function showAjaxLoader() {
        //find ajax loader div tag
        var loaderDiv = $("#__AjaxLoader");
        if (loaderDiv.length === 0) {
            //create ajax loader div tag, if not present
            loaderDiv = $("<div/>;")
                .attr("id", "__AjaxLoader")
                .css("position", "absolute")
                .css("display", "block")
                .css("z-index", "10000")
                .addClass("ajaxLoader");
            loaderDiv.appendTo("body");
        }

        //center ajax loader div tag in the browser window
        var doc = $(document);
        loaderDiv.css('top', (doc.height() - loaderDiv.height()) / 2);
        loaderDiv.css('left', (doc.width() - loaderDiv.width()) / 2);

        //show it
        loaderDiv.show();
    }

    function hideAjaxLoader() {
        //hide ajax loader div tag, if present
        $("#__AjaxLoader").hide();
    }

    //Initiate delay loading asynchronously
//    showAjaxLoader();
//    $.get('@Url.Action("LoadContents")', function (data) {
//        $("#delayLoadedContents").html(data);
//        hideAjaxLoader();
//    });


function ShowModalPage(name, title, page, sucess, failure) {
    // Build dialog markup
    var win = $('<div id="' + name + '"></div>');
    // Display dialog
   showAjaxLoader();
   win.load(page, function (data) {
       hideAjaxLoader();
   }
    ).dialog({
        modal: true,
        position: ['center', 'top'],
        title: title,
        resizable: false,
        width: 'auto',
        closeOnEscape: false, // Disable close button ('X')
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide(); // Disable close button ('X')
        }
        //            buttons: {
        //                "Close": function () { $(this).dialog("close"); }
        //            }
    });
 //   hideAjaxLoader();

} 
