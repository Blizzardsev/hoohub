let archiveLoadInProgress = false

$(document).ready(function () {
    getComics()
})

function getComics() {
    if (archiveLoadInProgress) {
        return
    }

    archiveLoadInProgress = true
    let displayedComics = $(".archive-comic")

    $.ajax({
        type: "GET",
        url: "?handler=Comics",
        dataType: "json",
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            startAtComic: $(displayedComics).length > 0 ? $(displayedComics)[displayedComics.length - 1].data("guid") : ""
        },
        success: function (result) {
            $("#hootbye").addClass("hidden")
            let archiveRowComics = []
            result.archiveComicData.forEach(function (comic) {
                archiveRowComics.push(`
                    <div class="col-auto animate__animated animate__fadeIn animate__slow">
                        <img data-guid="${comic.guid}" class="archive-comic" src="data:image/jpg;base64,${comic.imageData}" onclick="alert("TODO: full screen display")"/>
                    </div>
                `)

                if (archiveRowComics.length === 4) {
                    $("#archive-items").append(`
                        <div class="row animate__animated animate__fadeIn">
                            ${archiveRowComics.join("")}
                        </div>
                    `)
                    archiveRowComics = []
                }
            })
        },
        failure: function () {
            displayAlert("Failed to load archive: please try again later")
            if ($(".archive-comic").length > 0) {
                $("#hootbye").removeClass("hidden")
            }
        },
        complete: function () {
            archiveLoadInProgress = false
        }
    })
}