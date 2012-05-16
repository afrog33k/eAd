function ApproveMedia(id, count, fn)
 {

    var Media =
    {
        MediaID: id
       
    };

    $.post("/Media/Approve", Media, function (data) {
        //            alert('Successfully Changed Cart');
        // get the result and do some magic with it
        //var message = data.Message;
        if(fn!=null)
        fn(data);

    });

}

function UnApproveMedia(id, count, fn) {

    var Media =
    {
        MediaID: id
       
    };

    $.post("/Media/UnApprove", Media, function (data) {
        //            alert('Successfully Changed Cart');
        // get the result and do some magic with it
        //var message = data.Message;
         if(fn!=null)
        fn(data);
      

    });

}



//Shopping Cart Stuff

function addToCart(cartUrl,id, count) 
{

    var cartItem =
    {
        MediaID: id,
        Count: count
    };

    $.post("/ShoppingCart/Add", cartItem, function (data) {
        //            alert('Successfully Changed Cart');
        // get the result and do some magic with it
        //var message = data.Message;
        $("#ajaxCart").each(function () {
                 $(this).html(data);
         });
      
    });

}

function addToCartNonAsync(cartUrl,id, count, callback) 
{

    var cartItem =
    {
        MediaID: id,
        Count: count
    };



$.ajax({
  type: 'POST',
  url: "/ShoppingCart/Add",
  data: cartItem,
  async: false,
  cache: false,
  timeout: 30000
});


  

}

function removeFromCart(cartUrl, id, count) {

    var cartItem =
    {
        MediaID: id,
        Count: count
    };

    $.post("/ShoppingCart/Remove", cartItem, function (data) {
       
        $("#ajaxCart").each(function () {
            $(this).html(data);
        });
    });

}

function ClearCart(cartUrl, id, count) {

    var cartItem =
    {
        MediaID: id,
        Count: count
    };

    $.post("/ShoppingCart/Clear", cartItem, function (data) {
       
        $("#ajaxCart").each(function () {
            $(this).html(data);
        });
    });

}


function removeBuyOneItem(itemId) {
    var json = itemId;
    removeFromCart("", json, 1);
}

function addBuyOneItem(itemId, MediaCount) {
    var json = itemId;
    if (!MediaCount)
        MediaCount = 1;
    addToCart("", json, MediaCount);

}

function addBuyOneItemNonAsync(itemId, MediaCount,callback) {
    var json = itemId;
    if (!MediaCount)
        MediaCount = 1;
    addToCartNonAsync("", json, MediaCount,callback);

}
//Login Stuff

function IsUserLoggedIn() {
    var loggedIn = false;



    $.ajax({
        type: "GET",
        cache: false,
        async: false,
        url: "/Account/UserLoggedIn",
        success: function (data) {
            loggedIn = data.loggedIn;
        }


    });

    $.getJSON("/Account/UserLoggedIn",

              function (data) {
                  loggedIn = data.loggedIn;
              }

       );

    return loggedIn;
}

//Modal Dialog Stuff

//Popup dialog
function showDialog(title, message, name, closeEvent)
 {

    if (name == null) 
    {
        $('#dialog').html(message);
        $('#dialog').dialog({ title: title, modal: true });
    }
    else {

       
        $('#' + name + 'dialog').dialog({ title: title, modal: true });
        if (name == "login") {
            $('#' + name + 'dialog').dialog({draggable: false, resizable: false, title: title, modal: true, show: 'fade', hide: 'fade', close: closeEvent });
            $('#' + name + 'dialog').find("#loginMessage").load("/Account/AjaxLogon");
            return;
        }

        if (name == "Payment") {
            $('#' + name + 'dialog').dialog({ draggable: false, resizable: true, title: title, modal: true, show: 'fade', hide: 'fade', close: closeEvent, minHeight: '300px', width: '500px' });
           
            return;
        }

        if(message!=null)
        $('#' + name + 'dialog').html(message);
    }
}

//Hide dialog
function hideDialog(name, closeEvent) {
    if (name == null) 
    {
        $('#dialog').dialog('close', { close: closeEvent });
    }
    else {
        $('#' + name + 'dialog').dialog('close', { close: closeEvent });
    }
   
}

/////////////////
//Collapsible Panel Stuff
<!--
function MM_preloadImages() { //v3.0
  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
    var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
}

function MM_findObj(n, d) { //v4.01
  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
  if(!x && d.getElementById) x=d.getElementById(n); return x;
}

function MM_nbGroup(event, grpName) { //v6.0
  var i,img,nbArr,args=MM_nbGroup.arguments;
  if (event == "init" && args.length > 2) {
    if ((img = MM_findObj(args[2])) != null && !img.MM_init) {
      img.MM_init = true; img.MM_up = args[3]; img.MM_dn = img.src;
      if ((nbArr = document[grpName]) == null) nbArr = document[grpName] = new Array();
      nbArr[nbArr.length] = img;
      for (i=4; i < args.length-1; i+=2) if ((img = MM_findObj(args[i])) != null) {
        if (!img.MM_up) img.MM_up = img.src;
        img.src = img.MM_dn = args[i+1];
        nbArr[nbArr.length] = img;
    } }
  } else if (event == "over") {
    document.MM_nbOver = nbArr = new Array();
    for (i=1; i < args.length-1; i+=3) if ((img = MM_findObj(args[i])) != null) {
      if (!img.MM_up) img.MM_up = img.src;
      img.src = (img.MM_dn && args[i+2]) ? args[i+2] : ((args[i+1])? args[i+1] : img.MM_up);
      nbArr[nbArr.length] = img;
    }
  } else if (event == "out" ) {
    for (i=0; i < document.MM_nbOver.length; i++) {
      img = document.MM_nbOver[i]; img.src = (img.MM_dn) ? img.MM_dn : img.MM_up; }
  } else if (event == "down") {
    nbArr = document[grpName];
    if (nbArr)
      for (i=0; i < nbArr.length; i++) { img=nbArr[i]; img.src = img.MM_up; img.MM_dn = 0; }
    document[grpName] = nbArr = new Array();
    for (i=2; i < args.length-1; i+=2) if ((img = MM_findObj(args[i])) != null) {
      if (!img.MM_up) img.MM_up = img.src;
      img.src = img.MM_dn = (args[i+1])? args[i+1] : img.MM_up;
      nbArr[nbArr.length] = img;
  } }
}

// Facebook Stuff

function FacebookLogin(alocation)
{


FB.login(function(response) 
{
  if (response.session) 
  {
    if (response.perms)
    {
      // user is logged in and granted some permissions.
      // perms is a comma separated list of granted permissions
     window.location = alocation;
    } else 
    {
        FacebookRequestPermission();
    }
  } else
  {alert ("Facebook Login Failed");
    // user is not logged in
  }
}, {perms:'email'});

}

function FacebookRequestPermission()
{

  var myPermissions = "email,user_mobile_phone"; // permissions your app needs

FB.Connect.showPermissionDialog(myPermissions , function(perms) {
  if (!perms)
  {
    // handles if the user rejects the request for permissions. This is a good place to log off from Facebook connect
  }
  else
  {
      // finish up here if the user has accepted permission request
  };
  });

}



//  Dialog  startup Stuff





   