let archiveLoadInProgress = false

$(document).ready(function () {
    getComics()
})

$(window).on("scroll", function () {
    if (!archiveLoadInProgress && getIsWindowScrolledToBottom()) {
        getComics()
    }
})

function getIsWindowScrolledToBottom() {
    return (window.scrollY + window.innerHeight) >= (document.body.scrollHeight - 50)
}

/**
 * 
 * @returns
 */
function getComics() {
    if (archiveLoadInProgress) {
        return
    }

    archiveLoadInProgress = true
    let displayedComics = $(".archive-comic")
    $("#archive-items").append(`<div id="loader" class="row"><div class="loader mt-2 mb-2"></div></row>`)

    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "?handler=Comics",
            dataType: "json",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                startAtComic: $(displayedComics).length > 0 ? $($(displayedComics)[displayedComics.length - 1]).data("guid") : "",
                query: $("#archive-query").val()
            },
            success: function (result) {
                if ($(displayedComics).length === 0) {
                    $("#archive-items").empty()
                }
                else {
                    $("#loader").remove()
                }

                result.archiveComicData.forEach(function (comic) {
                    $("#archive-items").append(`
                    <div class="col-auto animate__animated animate__fadeIn animate__slow">
                        <img data-guid="${comic.guid}" class="archive-comic" src="data:image/jpg;base64,${comic.imageData}" title="View ${comic.displayName}..." onclick="alert("TODO: full screen display")"/>
                    </div>
                `)
                })
            },
            failure: function () {
                displayAlert("Failed to load archive: please try again later")
                if ($(".archive-comic").length === 0) {
                    $("#archive-items").append(`Failed to load archive: please try again later`)
                }
            },
            complete: function () {
                archiveLoadInProgress = false
            }
        })
    }, 500)
}