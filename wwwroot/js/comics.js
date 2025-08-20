let mouseX = 0
let mouseY = 0

$("html").click(function (event) { 
    mouseX = event.pageX
    mouseY = event.pageY
})

/**
 * 
 * @param {any} comicGuid
 * @returns
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

const defaults = {
    spread: 15,
    ticks: 10,
    gravity: 2,
    decay: 0.95,
    startVelocity: 15,
    shapes: ["heart"],
    colors: ["#FF0000"],
};