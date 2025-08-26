let archiveLoadInProgress = false
let lastScrollPosition = 0

let archiveSearchTimeout = undefined;
let archiveSearchPending = false;

$(document).ready(function () {
    getComics(false, false)
})

/**
 * On scrolling the window, attempt to load more comics.
 */
$(window).on("scroll", function () {
    if (!archiveLoadInProgress && getIsWindowScrolledToBottom()) {
        getComics()
    }
    lastScrollPosition = window.scrollY
})

/**
 * When the user finishes typing their search query, initiate the search after a brief grace period.
 */
$("#archive-query").on("keyup", function () {
    if (archiveSearchPending || archiveLoadInProgress) {
        clearTimeout(archiveSearchTimeout)
    }
    archiveSearchPending = true
    archiveSearchTimeout = setTimeout(() => {
        archiveSearchPending = false
        getComics(newSearch=true)
    }, 1250);
})

/**
 * Returns whether the window is currently scrolled to the bottom.
 * @returns True if the window is currently scrolled to the bottom, otherwise false.
 */
function getIsWindowScrolledToBottom() {
    return ((window.scrollY + window.innerHeight) >= (document.body.scrollHeight + 20) && window.scrollY > lastScrollPosition)
}


/**
 * Attempts to fetch the next batch of comics, based on the last comic currently displayed, and renders them.
 * @param {*} newSearch Whether this is a new comic query, or trailing from what is existing.
 * @param {*} displayEndOfResultsPrompt Whether to display the end of results prompt or not.
 */
function getComics(newSearch=false, displayEndOfResultsPrompt=true) {
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
                startAtComic: $(displayedComics).length === 0 || newSearch
                    ? ""
                    : $($(displayedComics)[displayedComics.length - 1]).data("guid"),
                query: $("#archive-query").val()
            },
            success: function (result) {
                if (result.success) {
                    if ($(displayedComics).length === 0 || newSearch) {
                        $("#archive-items").empty()
                    }
                    else {
                        $("#loader").remove()
                    }
                    
                    if (result.endOfResults && displayEndOfResultsPrompt) {
                        displayAlert("End of results")
                    }
                    if (result.archiveComicData.length > 0) {
                        let newComics = []
                        result.archiveComicData.forEach(function (comic) {
                            newComics.push(`
                                <div class="col-auto d-flex align-middle animate__animated animate__fadeIn">
                                    <img 
                                        role="button" 
                                        tabindex="0"
                                        data-guid="${comic.guid}" 
                                        data-display-name="${comic.displayName}"
                                        data-display-publish-date="${comic.displayPublishDate != undefined ? UtcDateTimeToLocalDateTimeString(comic.displayPublishDate) : "(Not yet published)"}"
                                        data-description="${comic.description}"
                                        data-alt-description="${comic.altDescription}"
                                        data-display-tags="${comic.displayTags}"
                                        class="archive-comic" src="data:image/jpg;base64,${comic.imageData}" 
                                        title="View ${comic.displayName}..." 
                                        onclick="showComicFullView(this)"
                                        onkeydown="onEnterKeyPress(showComicFullView, this)"/>
                                </div>
                            `)
                        })
                        $("#archive-items").append(newComics.join(""))
                    }
                }
                else {
                    displayAlert("Failed to load comics")
                    $("#loader").remove()
                }
            },
            failure: function () {
                displayAlert("Failed to load comics")
                $("#loader").remove()
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

/**
 * Loads the selected comic into the full view, then displays it.
 * @param {any} comic The comic to render in the full view.
 */
function showComicFullView(comic) {
    $("header").append(`
        <div 
            role="button" 
            tabindex="0" 
            class="archive-comic-view-container animate__animated animate__fadeIn animate__faster" 
            onclick="hideComicFullView(this)"
            onkeydown="onEnterKeyPress(hideComicFullView, this)">
            <div class="archive-comic-view-content">
                <h2>${$(comic).data("display-name")}</h2>
                <h5 class="fst-italic mb-4">${$(comic).data("display-publish-date")}</h5>
                <img class="mb-4" src="${$(comic).attr("src")}" alt="${$(comic).attr("alt-description")}">
                <h5 class="fst-italic">${$(comic).data("description")}</h5>
                <h5 class="fst-italic">Tags | ${$(comic).data("display-tags")}</h5>
                <h5 class="fst-italic mt-1 opacity-50">Click anywhere to dismiss</h5>
            </div>
        </div>
    `)
    $("body").addClass("no-scroll")
}

/**
 * Hides the comic full view.
 * @param {any} element
 */
function hideComicFullView(element) {
    $(element).remove()
    $("body").removeClass("no-scroll")
}