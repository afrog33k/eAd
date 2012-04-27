function ShowModalPage(name, title, page, sucess, failure) {
    // Build dialog markup
    var win = $('<div id="' + name + '"></div>');
    // Display dialog
    win.load(page).dialog({
        modal: true,
        title: title,
        resizable: false,
        width: 'auto'
        //            buttons: {
        //                "Close": function () { $(this).dialog("close"); }
        //            }
    });
}