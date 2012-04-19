﻿//Page Initialization
$(function () {
    //Main Mosaic File
    ResetDefaults();

    ReloadMosaicList();
    
});
function ResetDefaults() {

  
    
    var container = $("#container");

    container.contextMenu({
        menu: 'MosaicMenu'
    },
                function (action, el, pos) {
                    if (action == "edit") {
                        var mosaic = $("#MosaicID");
                        if (mosaic.val() != "") {
                            ShowModalPage("EditMosaic",'Edit Mosaic', 'Mosaic/EditPartial/' + mosaic.val(), null, null);
                        }
                        else {
                            alert("Please Select A Mosaic to Work on");
                        }
                    }
                    else if (action == "delete") {
                        var mosaic = $("#MosaicID");
                        if (mosaic.val() != "") {
                            ShowModalPage("DeleteMosaic", 'Delete Mosaic', 'Mosaic/Delete/' + mosaic.val(), null, null);
                        }
                        else {
                            alert("Please Select A Mosaic to Work on");
                        }
                    }
                    //Debug Version
//                    else {


//                        alert(

//					                    'Action: ' + action + '\n\n' +

//    					                    'Element ID: ' + $(el).attr('id') + '\n\n' +

//        					                    'X: ' + pos.x + '  Y: ' + pos.y + ' (relative to element)\n\n' +

//            					                    'X: ' + pos.docX + '  Y: ' + pos.docY + ' (relative to document)'

//						        );
//                    }
                });

    // there's the gallery and the trash
    var $gallery = $("#gallery"),
			        $trash = $(".resizable");

    $("#MosaicID").change(function () {
        var val = $(this).val();
        if (val != "") {
            LoadMosaic(val);
        }

    });

    // let the gallery items be draggable
    $("li", $gallery).draggable({
        cancel: "a.ui-icon", // clicking an icon won't initiate dragging
        revert: "invalid", // when not dropped, the item will revert back to its initial position
        containment: $("#container").length ? "#container" : "document", // stick to demo-frame if present
        helper: "clone",
        cursor: "move"
    });
    // let the gallery be droppable as well, accepting items from the trash
    $gallery.droppable({
        accept: ".resizable",
        activeClass: "custom-state-active",
        drop: function (event, ui) {
            recycleImage(ui.draggable);
        }
    });
    $("#dialog:ui-dialog").dialog("destroy");

    $("#dialog-addposition").dialog({
        resizable: false,
        height: 140,
        modal: true,
        buttons: {
            "Add": function () {
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#AddNewPosition").button();
    $('#AddNewPosition').unbind('click');
    $("#AddNewPosition").click(function () { return false; });

    // When the toolbar button is clicked...
    $('#AddNewPosition').click(function () {
        getAndSetNewPosition(null);
    });

    $("#SaveMosaic").button();
    $("#Preview").button();


    // When the toolbar button is clicked...
    $('#SaveMosaic').unbind('click');
    $('#SaveMosaic').click(function () {

        SaveMosaic();
    });

    $('#Preview').click(function () {

        Preview();
    });


    $("#CreateNewMosaic").button();

   // $("#MosaicID").dropp();
    

    // When the toolbar button is clicked...
    $('#CreateNewMosaic').click(function () {
        CreateNewMosaic();
    });


    // resolve the icons behavior with event delegation
    $("ul.gallery > li").click(function (event) {
        var $item = $(this),
				        $target = $(event.target);

        if ($target.is("a.ui-icon-trash")) {
            window.deleteImage($item);
        } else if ($target.is("a.ui-icon-zoomin")) {
            viewLargerImage($target);
        } else if ($target.is("a.ui-icon-refresh")) {
            recycleImage($item);
        }

        return false;
    });

    // let the trash be droppable, accepting the gallery items
    $trash.droppable({
        accept: "#gallery > li",
        activeClass: "ui-state-highlight",
        drop: function (event, ui) {
            addImage(ui.draggable, $(this));
        }
    });

    $(".collapsible").accordion({
        collapsible: true,
        autoHeight: false,
        navigation: true
    });

}

function ShowModalPage(name,title, page, sucess, failure) {
    // Build dialog markup
   // var win = $('<div></div>');
    // Build dialog markup
    var win = $('<div id="'+name+'"></div>');
  // Display dialog
  win.load(page).dialog({
            modal: true,
            title: title,
            resizable: false,
          width:'auto'
//            buttons: {
//                "Close": function () { $(this).dialog("close"); }
//            }
             });

   //     $("#editMosaic").dialog('open'); 
   // });
    // Display dialog
    //        win.load(page, function () {
    //            $(this).dialog({
    //                    title: title,
    //                    'modal': true,
    //                    'buttons': {
    //                        'Ok': function() {

    //                            $(this).dialog('close');
    //                            sucess();
    //                        },
    //                        'Cancel': function() {
    //                            $(this).dialog('close');
    //                            failure();
    //                        }
    //                    }
    //                });
    //    });

}

function addImage($item, $object) {

    $item.fadeOut(function () 
        {
        // image deletion function
        var recycleIcon = "<a href='' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";

        var $list = $("ul", $object.find('.collapsible').find('div')).length ?
					        $("ul", $object.find('.collapsible').find('div')) :
					        $("<ul class='gallery ui-helper-reset'/>").appendTo($object.find('.collapsible').find('div'));


        $object.css("background", "url(" + $item.find("img").attr("src") + ") top left no-repeat");
        $object.css("background-size", "100% 100%");
        $item.find("a.ui-icon-trash").remove();
        $item.append(recycleIcon).appendTo($list).fadeIn(function () {
            $item
						        .animate({ width: "48px" })
						        .find("img")
							        .animate({ height: "36px" });
        });
    });
}

function recycleImage($item) {
    // image recycle function
    var trashIcon = "<a href='link/to/trash/script/when/we/have/js/off' title='Delete this image' class='ui-icon ui-icon-trash'>Delete image</a>";
    var $gallery = $("#gallery");
    $item.fadeOut(function () {
        $item
					        .find("a.ui-icon-refresh")
						        .remove()
					        .end()
					        .css("width", "96px")
					        .append(trashIcon)
					        .find("img")
						        .css("height", "72px")
					        .end()
					        .appendTo($gallery)
					        .fadeIn();
    });
}

// image preview function, demonstrating the ui.dialog used as a modal window
function viewLargerImage($link) {
    var src = $link.attr("href"),
				        title = $link.siblings("img").attr("alt"),
				        $modal = $("img[src$='" + src + "']");

    if ($modal.length) {
        $modal.dialog("open");
    } else {
        var img = $("<img alt='" + title + "' width='384' height='288' style='display: none; padding: 8px;' />")
					        .attr("src", src).appendTo("body");
        setTimeout(function () {
            img.dialog({
                title: title,
                width: 400,
                modal: true
            });
        }, 1);
    }
}
function addPosition($item, $object) {

    // image deletion function
    var recycleIcon = "<a href='link/to/recycle/script/when/we/have/js/off' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";

    $item.fadeOut(function () {
        var $list = $("ul", $object).length ?
					        $("ul", $object) :
					        $("<ul class='gallery ui-helper-reset'/>").appendTo($object);

        $item.find("a.ui-icon-trash").remove();
        $item.append(recycleIcon).appendTo($list).fadeIn(function () {
            $item
						        .animate({ width: "48px" })
						        .find("img")
							        .animate({ height: "36px" });
        });
    });
}



function Preview() {
    window.location.href = "Mosaic/Preview/" + $("#MosaicID").val();

}

function SaveMosaic() {
    var playerScreenPos = $("#container").position();

    var positions = $(".position");

    positions = jQuery.unique(positions);
    var mosaic = $("#MosaicID");
    var plist = [];
    positions.each(function () {
        var position = $(this).position();
        var imglist = new Array();
        var list = $($($(this).find('div.ui-accordion-content')[0]).find('li')).find("img");
        list = jQuery.unique(list);
        list.each(function () {
            imglist.push($(this).attr("src"));
        });

        plist.push({ MosaicID: mosaic.val(), Name: $(this).attr('id'),
            X: ('' + (position.left - playerScreenPos.left)), Y: ('' + (position.top - playerScreenPos.top))
                    , Width: ('' + $(this).width()), Height: ('' + $(this).height()), MediaUri: imglist, Media: null
        });
    });


    $.ajax({
        type: "AJAX",
        url: "Mosaic/SaveMosaic",
        data: JSON.stringify(plist),
        success: function (data) {
            alert(data);
        },
        dataType: "json",
        traditional: true,
        contentType: 'application/json; charset=utf-8'
    });
    mosaic = $("#MosaicID");
    $.ajax({
        url: "Mosaic/MosaicUpdated",
        type: 'AJAX',
        context: document.body,
        data:
                            {
                                id: mosaic.val()
                            },
        success: function (data) {

        },
        error: function (data) {
            //   location.reload();
            alert(data);

        } 
    });

}

function LoadMosaic(id) {
    //      var mosaic = $("#MosaicID");
    $.ajax({
        url: "Mosaic/LoadMosaic",
        type: 'POST',
        context: document.body,
        data:
                            {
                                id: id
                            },
        success: function (data) {
            //   location.reload();
            //    alert(data);
            $('#container').find("div").remove();
            $.each(data,
                                function () {
                                    AddPosition(this.Name, this.X, this.Y, this.Width, this.Height, this.PositionID, this.Media);
                                });
            ResetDefaults();
        },
        error: function (data) {
            //   location.reload();
            alert(data);

        } 
    });
}

function SavePosition(name, px, py, pwidth, pheight, pitems) {
    //  var items = new Array();
    var mosaic = $("#MosaicID");
    $.ajax({
        url: "Mosaic/SavePosition",
        type: 'POST',
        context: document.body,
        async: false,
        traditional: true,
        cache: false,
        timeout: 30000,
        data: { id: mosaic.val(), name: name, x: px, y: py, width: pwidth, height: pheight, items: pitems },
        success: function () {
            //   location.reload();
            //  alert("Saved");
        },
        error: function (data) {
            //   location.reload();
            //    alert(data);

        } 
    });


}


function getAndSetNewPosition(name) {
    if (name == null) {
        getNewPositionName(function (value) {
            getAndSetNewPosition(value);
        });
    }
    else {
        AddPosition(name, 0, 0, 0, 0, 0);
        ResetDefaults();
    }
}


function CreateNewMosaic() {
    // Build dialog markup
    var win = $('<div><p>Enter new mosaic name</p></div>');
    var userInput = $('<input type="text" style="width:100%"></input>');
    userInput.appendTo(win);

    // Display dialog
    $(win).dialog({
        'modal': true,
        'buttons': {
            'Ok': function () {

                $(this).dialog('close');

                $.ajax({
                    url: "Mosaic/Create",
                    type: 'POST',
                    context: document.body,
                    data: { name: $(userInput).val() },
                    success: function () {
                        location.reload();

                    } 
                });

            },
            'Cancel': function () {
                $(this).dialog('close');
            }

        }
    });
}

function getNewPositionName(callback) {
    // Build dialog markup
    var win = $('<div><p>Enter new position name</p></div>');
    var userInput = $('<input type="text" style="width:100%"></input>');
    userInput.appendTo(win);

    // Display dialog
    $(win).dialog({
        'modal': true,
        'buttons': {
            'Ok': function () {

                $(this).dialog('close');
                callback($(userInput).val());
            },
            'Cancel': function () {
                $(this).dialog('close');
            }
        }
    });
}

function removeSpaces(string) {
    return string.split(' ').join('');
}

function checkString(strng) {
    var error = false;
    //var illegalChars = /[\u0021-\u002f\u003a-\u0040\u005b-\u005e\u0060\u007b-\u007e]/g; // The /g (for global) is a goof.
    var illegalChars = /[\u0021-\u002f\u003a-\u0040\u005b-\u005e\u0060\u007b-\u007e]/; // NOT global.
    error = (illegalChars.test(strng));
    return error;
}

function AddPosition(name, x, y, nwidth, nheight, id, media) {


    var recycleIcon = "<a href='' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";


    var jsName = removeSpaces(name);
    if ($('#container').find('#' + jsName).length == 0) {
        $('#container').append('<div id="' + jsName + '" class="resizable ui-state-active position">' +
                                  '<h3 class="ui-widget-header">' + name + '</h3>' + '<div class="collapsible"><a href="#">Media</a></h3>' +
                                  '<div >' +

                                  '</div>' +
                                  '</div> </div>');
    }




    var playerScreenPos = $("#container").position();

    //                      $('#' + jsName).offset({ top: y, left: x });

    $('#' + jsName).css("position", "absolute");
    $('#' + jsName).css("left", (playerScreenPos.left + (x)) + "px");
    $('#' + jsName).css("top", (playerScreenPos.top + (y)) + "px");
    $('#' + jsName).height(nheight);
    $('#' + jsName).width(nwidth);


    $('#' + jsName).draggable({ containment: "#container", scroll: false, snap: ".resizable", snapMode: "outer" });
    $('#' + jsName).resizable({
        // containment: "#container",
        animate: true,
        helper: "ui-resizable-helper",
        grid: 20,
        ghost: true
    });

    $('#' + jsName).contextMenu({

        menu: 'PositionMenu'

    },

					        function (action, el, pos) {

					            if (action == "add") {
					                var mosaic = $("#MosaicID");

					                if (mosaic.val() != "") {

					                    ShowModalPage("MediaPicker", "Pick media to insert", 'Media/Picker?componentId=' + jsName, null, null);

					                }
					            }
					            else if (action == "delete") {
					                var mosaic = $("#MosaicID");
					                if (mosaic.val() != "") {
					                    ShowModalPage("DeletePosition", 'Delete Position', 'Mosaic/DeletePosition/?mosaic=' + mosaic.val() + "&position=" + jsName, null, null);
					                }


					            }
					        });

    $('#' + jsName).append(recycleIcon);
    if (nwidth != 0) {
        //   $('#' + jsName).fadeIn(function() {
        //   $('#' + jsName).css({ "left": (x) + "px", "top": y + "px" });


        //                     $('#' + jsName).animate({ width: nwidth });
        //                      $('#' + jsName).animate({ height: nheight });
        //      $('#' + jsName)
        //  .animate({ width: width })
        //   .animate({ height:height });

        //    });
    }
    else {
        $('#' + jsName).fadeIn(function () {
            $('#' + jsName)
                                     .animate({ width: "100px" })
                                     .animate({ height: "100px" });
        }); ;
    }

    for (var i in media) {

        addImage($("#gi" + media[i]), $('#' + jsName));
        //   $("#gi" + Media[i]).remove(); //Test
        //                                     $("#gi" + Media[i]).animate({"left": $('#' + jsName).offset().left ,"top":  $('#' + jsName).offset().top},
        //             {
        //duration: 500,     specialEasing: {    width: 'linear' },
        //              complete:function()
        //               {

        //                

        //                }
        //            }
        //                                );

    }
    
}
    function CloseMosaicMenu(id) {

        ReloadMosaicList(id);
        $('#EditMosaic').dialog('destroy');
        $('#EditMosaic').remove();
    }

    function CloseDeleteMosaic() {
        ReloadMosaicList();
        $('#DeleteMosaic').dialog('destroy');
        $('#DeleteMosaic').remove();
    }
    function CloseMediaPicker(plist,positionName) {

        //  ReloadMosaicList();
        //     alert("You have selected " + plist.length + " Items : " + plist);
        $('#MediaPicker').dialog('destroy');
        $('#MediaPicker').remove();
        var position = $('#' + positionName);
      
        if (plist != null && plist.length > 0) {

     

                for (var i = 0; i < plist.length; i++) {
                    var item = $('<li class="ui-widget-content ui-corner-tr" id=' + plist[i].id + '>' +
                '<h5 class="ui-widget-header">'+plist[i].name+'</h5>'+
                '<img src="' + plist[i].img + '"  height="72" width="96" />' +
                '</li>');
                       
                       
                   
                
                // image deletion function
                var recycleIcon = "<a href='' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";

                var $list = $("ul", position.find('.collapsible').find('div')).length ?
					        $("ul", position.find('.collapsible').find('div')) :
					        $("<ul class='gallery ui-helper-reset'/>").appendTo(position.find('.collapsible').find('div'));


               
                item.find("a.ui-icon-trash").remove();
                item.append(recycleIcon).appendTo($list).fadeIn(function () {
                    item
						        .animate({ width: "48px" })
						        .find("img")
							        .animate({ height: "36px" });
                });

                position.css("background", "url(" + plist[plist.length-1].img + ") top left no-repeat");
                position.css("background-size", "100% 100%");
            };
            
          
        }
   
    }



    function ReloadMosaicList(id)
    {
        LoadDropDownList("Mosaic/GetMosaicList", "MosaicID");
        if(id>0) {
            $('#MosaicID').val(id);
        }
        else {
          
            var myDDL = $('#MosaicID');
            myDDL[0].selectedIndex = 0;
       
        }
 
    };


       function LoadDropDownList(url, listId) {
           $.getJSON(url, null,
           function (list) {
               $("#" + listId).empty();
               $.each(list, function (i, foo) {
                   $("#" + listId).append(new Option(foo.Text, foo.Value));

               });
           });
       };