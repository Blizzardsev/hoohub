let mouseX = 0
let mouseY = 0

// These are the default values for confetti anim when liking comic
const defaults = {
    spread: 15,
    ticks: 10,
    gravity: 2,
    decay: 0.95,
    startVelocity: 15,
    shapes: ["heart"],
    colors: ["#FF0000"],
};

$(document).ready(function () {
    // Ensure the publish date for the comic is handled according to locale
    let publishDateValue = $("#about-comic-publish-date-info").val()
    $("#about-comic-publish-date-display").text(`First published  ${publishDateValue.length > 0 ? UtcDateTimeToLocalDateTimeString(publishDateValue) : "(Unknown)"}`)
})

$("html").click(function (event) { 
    mouseX = event.pageX
    mouseY = event.pageY
})

/**
 * On swiping right, load the previous comic, if one exists.
 */
document.getElementById("view-comic-full").addEventListener("swiped-right", function (event) {
    if (document.getElementById("comic-swipe-previous-link") === null) {
        displayAlert("End of comics")
        return
    }
    displayLoading()
    document.getElementById("comic-swipe-previous-link").click()
})

/**
 * On swiping left, load the next comic, if one exists.
 */
document.getElementById("view-comic-full").addEventListener("swiped-left", function (event) {
    if (document.getElementById("comic-swipe-next-link") === null) {
        displayAlert("End of comics")
        return
    }
    displayLoading()
    document.getElementById("comic-swipe-next-link").click()
})

/**
 * On swiping up, dimiss the full comic view
 */
document.getElementById("view-comic-full").addEventListener("swiped-up", function (event) {
    hideModal('view-comic-full')
})

/**
 * On swiping up, dimiss the about comic view
 */
document.getElementById("about-comic").addEventListener("swiped-up", function (event) {
    hideModal('about-comic')
})

/**
 * Attempts to toggle the heart/liked status for the currently viewed comic.
 * @param {any} element - Calling element to disable
 * @param {any} comicGuid - The GUID of the comic to toggle the heart/liked status for
 */
function heartComic(element, comicGuid) {
    if ($(element).hasClass("disabled")) {
        return
    }
    $(element).addClass("disabled")
    
    $.ajax({
        type: "PATCH",
        url: "?handler=ComicLiked",
        dataType: "json",
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            comicGuid: comicGuid
        },
        success: function (result) {
            if (result.success) {
                $("#comic-like-count").text(result.likeCount)

                if (result.wasLiked) {

                    const position = {
                        x: mouseX / window.innerWidth,
                        y: mouseY / window.innerHeight
                    };

                    confetti({
                        ...defaults,
                        particleCount: 30,
                        origin: position,
                        scalar: 2
                    });

                    confetti({
                        ...defaults,
                        particleCount: 15,
                        origin: position,
                        scalar: 3,
                    });

                    confetti({
                        ...defaults,
                        particleCount: 5,
                        origin: position,
                        scalar: 4,
                    });

                    displayAlert("Thank you! ♥")
                    $(element).addClass("heart")
                    $(element).attr("title", "Unlike this comic...")
                }
                else {
                    $(element).removeClass("heart")
                    $(element).attr("title", "Like this comic!")
                }
            }
            else {
                displayAlert("Please try again later")
            }
        },
        failure: function () {
            displayAlert("Please try again later")
        },
        complete: function () {
            $(element).removeClass("disabled")
        }
    })
}