$(function () {
    $("#ajaxUploadForm").ajaxForm(
         {
             iframe: true,
             dataType: "json",
             beforeSubmit: function () {

                 $.growlUI(null, '<h1><img src="/Content/busy.gif" /> Uploading file...</h1>');
//                 $("#ajaxUploadForm").block(
//                 {
//                     message: null,
//                     //                    message: '<h1><img src="/Content/busy.gif" /> Uploading file...</h1>' 
//                 });
             },
             success: function (result) {
                 $("#ajaxUploadForm").unblock();
                 $("#ajaxUploadForm").resetForm();
                 $.growlUI(null, 'Image Successfully Uploaded');
                 var uploadedImage = document.getElementById("uploadedImage");
                 uploadedImage.src = result.message;
                 uploadedImage.alt = result.message;
             },
             error: function (xhr, textStatus, errorThrown) {
                 $("#ajaxUploadForm").unblock();
                 $("#ajaxUploadForm").resetForm();
                 $.growlUI(null, 'Error uploading file');
             }
         });
});