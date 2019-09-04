$(document).ready(function () {
    $("#btnDetails").on('click', function () {
        $(".sidebarInfo").toggleClass("active");
        //add ScrollBar
        const ps = new PerfectScrollbar('#sidebarInfo');
       
    });
 
    $(".js__input__ui").on("blur", function () {
       
        if ($(this).val() <= 0) {
            $(this).addClass("error__ui");
            $(this).siblings("svg").addClass("error__ui");
        }
        else {
            $(this).removeClass("error__ui");
            $(this).siblings("svg").removeClass("error__ui");
        }

    });
   
});