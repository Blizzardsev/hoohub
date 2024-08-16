let archiveLoadInProgress = false
let lastScrollPosition = 0

$(document).ready(function () {
    getComics()
})

/**
 * 
 */
$(window).on("scroll", function () {
    if (!archiveLoadInProgress && getIsWindowScrolledToBottom()) {
        getComics()
    }
    lastScrollPosition = window.scrollY
})

$("#archive-query").on("keyup", function () {
    
})

/**
 * 
 * @returns 
 */
function getIsWindowScrolledToBottom() {
    return ((window.scrollY + window.innerHeight) >= (document.body.scrollHeight + 20) && window.scrollY > lastScrollPosition)
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
    $("body").addClass("no-scroll")
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
                let newComics = []
                result.archiveComicData.forEach(function (comic) {
                    newComics.push(`
                        <div class="col-auto animate__animated animate__fadeIn">
                            <img 
                                data-guid="${comic.guid}" 
                                data-display-name="${comic.displayName}"
                                data-display-description="${comic.description}"
                                data-tags="${comic.tags.replace(",", ", ")}"
                                class="archive-comic" src="data:image/jpg;base64,${comic.imageData}" title="View ${comic.displayName}..." onclick="comicFullView(this)"/>
                        </div>
                    `)
                })
                $("#archive-items").append(newComics.join(""))
            },
            failure: function () {
                displayAlert("Failed to load archive: please try again later")
                if ($(".archive-comic").length === 0) {
                    $("#archive-items").append(`Failed to load archive: please try again later`)
                }
            },
            complete: function () {
                setTimeout(function () {
                    $("body").removeClass("no-scroll")
                    archiveLoadInProgress = false
                }, 1025)
            }
        })
    }, 500)
}

function comicFullView() {

}