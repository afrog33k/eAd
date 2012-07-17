$(document).ready(function () {


    var x = 0, y = 0, w = 0, h = 0;
         // Create variables (in this scope) to hold the API and image size
      var jcrop_api, boundx, boundy;
        var imageHandler = "ImageHandler.ashx?id=";

        $(function () {

                                    $('#imgUploadedImage').Jcrop({
                                        onChange: setCoords,
                                        onSelect: setCoords,
                                        aspectRatio: @(ViewBag.Width  / (float)ViewBag.Height),
                                        maxSize: [ @ViewBag.Width, @ViewBag.Height]
                                      },function(){
        // Use the API to get the real image size
        var bounds = this.getBounds();
        boundx = bounds[0];
        boundy = bounds[1];
        // Store the API in the jcrop_api variable
        jcrop_api = this;
      });
//            $("#preview").resizable();

                                    });
          
     

        function setCoords(c) {
            window.x = c.x;
             window.y = c.y;
             window.w = c.w;
            window.h = c.h;
            if (parseInt(c.w) > 0)
        {
          var rx = @ViewBag.Width / c.w;
          var ry = @ViewBag.Height / c.h;

           var top=     $('#imgUploadedImage').offset().top;
          
          $('#preview').css({
            width: Math.round(rx * boundx) + 'px',
            height: Math.round(ry * boundy) + 'px',
            marginLeft: '-' + Math.round(rx * c.x) + 'px',
            marginTop: '-' + Math.round(ry * (c.y+top)) + 'px'
          });
        }

        };

        function cropImage() {
           var size= $('#imgUploadedImage');
            var x2 = (x * @ViewBag.Width ) / size.width();
            var y2 = ((y  * @ViewBag.Height))/ size.height();
            
              var wx = (w / size.width()) * @ViewBag.Width;
          var hy = (h / size.height()) * @ViewBag.Height;

           var top=     $('#imgUploadedImage').offset().top;

            $.ajax({
                url: "@Url.Action("CropImage", "Media" )",
                type: "POST",
                data: { x: Math.round(x), y: Math.round(y2), w: Math.round(w), h: Math.round(h), url: $('#imgUploadedImage').attr('src')  },
                success: function (data) {
                  //  $('#lblMethodError').hide();
                  //  $("#pnlNewImage").show();
                   // $("#imgNewImage").attr("src", data);
               //  window.parent.parent.LoadComplete(data);
                 $('#ResizeImage').dialog('destroy');
            $('#ResizeImage').remove();
            window.plist[0].img = data;
            window.parent.CloseMediaPicker( window.plist, componentId.val());
                },
                error: function (xhr, status, error) {
                    // Show the error
                    $('#lblMethodError').text(xhr.responseText);
                    $('#lblMethodError').show();
                }
            });

        }

        function resizeImage() {
            // Call the server side function to resize the image
            $.ajax({
                url: "@Url.Action("ResizeImage", "Media")",
                type: "POST",
                success: function (data) {
                    $('#lblMethodError').hide();
                    $("#pnlNewImage").show();
                    $("#imgNewImage").attr("src",  data);
                },
                error: function (xhr, status, error) {
                    // Show the error
                    $('#lblMethodError').text(xhr.responseText);
                    $('#lblMethodError').show();
                }
            });
        }
});