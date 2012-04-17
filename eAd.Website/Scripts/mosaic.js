    function ResetDefaults()
    {

        var container = $("#container");

        container.contextMenu({

            menu: 'MosaicMenu'

        },

					function (action, el, pos) {

					    if (action == "edit") {
					        var mosaic = $("#MosaicID");

					        if (mosaic.val() != "") {
					            
					       
					        // Build dialog markup
					        var win = $('<div id="editMosaic"></div>');

                            win.load('Mosaic/Edit/' + mosaic.val());

					        // Display dialog
					        $(win).dialog({
					            'modal': true,
					            'buttons': {
					                'Ok': function () {

					                //    $(this).dialog('**destroy**');
					                    $('#editMosaic').close();
					                },
					                'Cancel': function () {
					                    $('#editMosaic').close();
					                }
					            }
					        });
					    }
					    }
					    else {


					        alert(

					            'Action: ' + action + '\n\n' +

    					            'Element ID: ' + $(el).attr('id') + '\n\n' +

        					            'X: ' + pos.x + '  Y: ' + pos.y + ' (relative to element)\n\n' +

            					            'X: ' + pos.docX + '  Y: ' + pos.docY + ' (relative to document)'

						);
					    }
					});

				
//				// Disable menus

//				$("#disableMenus").click( function() {

//					$('#myDiv, #myList UL LI').disableContextMenu();

//					$(this).attr('disabled', true);

//					$("#enableMenus").attr('disabled', false);

//				});

//				

//				// Enable menus

//				$("#enableMenus").click( function() {

//					$('#myDiv, #myList UL LI').enableContextMenu();

//					$(this).attr('disabled', true);

//					$("#disableMenus").attr('disabled', false);

//				});

				

//				// Disable cut/copy

//				$("#disableItems").click( function() {

//					$('#myMenu').disableContextMenuItems('#cut,#copy');

//					$(this).attr('disabled', true);

//					$("#enableItems").attr('disabled', false);

//				});

				

//				// Enable cut/copy

//				$("#enableItems").click( function() {

//					$('#myMenu').enableContextMenuItems('#cut,#copy');

//					$(this).attr('disabled', true);

//					$("#disableItems").attr('disabled', false);

//				});				

				


        // there's the gallery and the trash
        var $gallery = $("#gallery"),
			$trash = $(".resizable");

        $("#MosaicID").change(function () {
            var val = $(this).val();
            if (val!="") {
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
        $("#AddNewPosition").click(function () { return false; });

        // When the toolbar button is clicked...
        $('#AddNewPosition').click(function () {
            getAndSetNewPosition(null);
        });

        $("#SaveMosaic").button();
         $("#Preview").button();

      
        // When the toolbar button is clicked...
     //   $('#SaveMosaic').unbind('click');
        $('#SaveMosaic').click(function () {

            SaveMosaic();
        });

          $('#Preview').click(function () {

            Preview();
        });
        
          
        $("#CreateNewMosaic").button();


       
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
  
    function addImage($item, $object)
    {

        $item.fadeOut(function () {
            // image deletion function
            var recycleIcon = "<a href='link/to/recycle/script/when/we/have/js/off' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";

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

	    $(function () {
	        ResetDefaults();
//	        $("#player1").draggable({ containment: "#container", scroll: false, snap: ".resizable", snapMode: "outer" });
//	        $("#player1").resizable({
//	            //    containment: "#container",
//	            animate: true,
//	            helper: "ui-resizable-helper",
//	            grid: 20,
//	            ghost: true,
//	            stop: function () {
//	                //  $(this).find("img")("background", $(this).css("background"));
//	            }
//	        });
            });
       
	      function Preview()
	      {
	          window.location.href = "Mosaic/Preview/" + $("#MosaicID").val();
	      
	      }
	      
	      function SaveMosaic()
      {
      var playerScreenPos = $("#container").position();
       
              var positions =     $(".position");
               
           positions=jQuery.unique(positions);
          var mosaic = $("#MosaicID");
             var plist = [];
       
     positions.each(function() 
           {
                var position = $(this).position();
              var imglist = new Array();
           
           //$($($(this).find('div.ui-accordion-content')[0]).find('li')).find("img")
           var list =   $($($(this).find('div.ui-accordion-content')[0]).find('li')).find("img");
               
           list=jQuery.unique(list);
             list.each(function()
  {
               imglist.push($(this).attr("src"));
           });
           
//                SavePosition($(this).attr('id'),
//                    position.left-playerScreenPos.left,
//                    position.top-playerScreenPos.top,
//                    $(this).width(),
//                $(this).height(),imglist);
        
    plist.push({ MosaicID:  mosaic.val(), Name: $(this).attr('id'),
            X: (''+(position.left-playerScreenPos.left)), Y:   (''+(position.top-playerScreenPos.top))
            , Width:  (''+$(this).width()), Height:  (''+$(this).height()), MediaUri: imglist, Media: null });
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

	          
	          
	          
	           $.ajax({
                    url: "Mosaic/MosaicUpdated",
                    type: 'POST',
                    context: document.body,
                    data: 
                    { 
                        id: id
                    },
                    success: function (data) 
                    {
               
                    },
            error:  function (data) {
                //   location.reload();
                 alert(data);

            }}); 
	        
      }
   
    function  LoadMosaic(id) {
     //      var mosaic = $("#MosaicID");
        $.ajax({
                    url:"Mosaic/LoadMosaic",
                    type: 'POST',
                    context: document.body,
                    data: 
                    { 
                        id: id
                    },
                    success: function (data) 
                    {
                 //   location.reload();
               //    alert(data);
                      $('#container').find("div").remove();
                      $.each(data,
                        function()
                        {
                            AddPosition(this.Name, this.X, this.Y, this.Width, this.Height, this.PositionID, this.Media);
                        });
                            ResetDefaults();
                    },
            error:  function (data) {
                //   location.reload();
                 alert(data);

            }});
    }

   function SavePosition(name,px,py,pwidth,pheight,pitems) {
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
                    data: { id: mosaic.val(), name: name, x:px ,y:py, width:pwidth, height:pheight, items:pitems },
                    success: function () {
                 //   location.reload();
                //  alert("Saved");
                    },
            error:  function (data) {
                //   location.reload();
             //    alert(data);

            }});
        
       
   }


   function getAndSetNewPosition(name) {
      if (name==null) {
          getNewPositionName(function (value) {
            getAndSetNewPosition(value);
        });
      }
    else {
        AddPosition(name,0,0,0,0,0);
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
                    url:  "Mosaic/Create" ,
                    type: 'POST',
                    context: document.body,
                    data: { name: $(userInput).val()},
                    success: function () {
                    location.reload();

                    }});
                
            },
            'Cancel': function () {
                $(this).dialog('close');
            }
        
    }});
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
            'Cancel': function() {
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
         
         function AddPosition(name,x,y,nwidth,nheight,id,media) {

            
                 var recycleIcon = "<a href='link/to/recycle/script/when/we/have/js/off' title='Recycle this image' class='ui-icon ui-icon-refresh'>Recycle image</a>";


                 var jsName = removeSpaces(name);
                  if(   $('#container').find('#' + jsName).length==0)
                  {
                      $('#container').append('<div id="' + jsName + '" class="resizable ui-state-active position">' +
                          '<h3 class="ui-widget-header">' + name + '</h3>' + '<div class="collapsible"><a href="#">Media</a></h3>' +
                          '<div >' +

                          '</div>' +
                          '</div> </div>');
                  }




             var playerScreenPos = $("#container").position();
                  
//                      $('#' + jsName).offset({ top: y, left: x });
                            
                               $('#' + jsName).css ("position",   "absolute");
                           $('#' + jsName).css  ("left", (playerScreenPos.left + (x)) + "px");
                             $('#' + jsName).css("top",  (playerScreenPos.top+(y)) + "px");
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

					    alert(

						'pAction: ' + action + '\n\n' +

						'Element ID: ' + $(el).attr('id') + '\n\n' +

						'X: ' + pos.x + '  Y: ' + pos.y + ' (relative to element)\n\n' +

						'X: ' + pos.docX + '  Y: ' + pos.docY + ' (relative to document)'

						);

					});

                 $('#' + jsName).append(recycleIcon);
                 if (nwidth != 0)
                 {
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
                     $('#' + jsName) .fadeIn(function() {
                         $('#' + jsName)
                             .animate({ width: "100px" })
                             .animate({ height: "100px" });
                     }); ;
                 }
                      
                             for(var i in media)
{
                                 
                                   addImage( $("#gi" + media[i]), $('#' + jsName));
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
	    