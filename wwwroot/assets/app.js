"use strict"; (function () { }).call(void 0);

$(".load-more").click(function (e) {
    e.preventDefault();

    $(this).find("span").text("");
    $(this).addClass("loading");
    $(this).find(".spinner").show();

    $.get("/getmoretweets", function (data) {
        $(".tweetgrid-panel").append(data);
        $(".load-more").removeClass("loading");
        $(".load-more").find(".spinner").hide();
        $(".load-more").find("span").text("Load More");
        $("time.timeago").timeago();
    });

});