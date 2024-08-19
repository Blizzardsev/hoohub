let validExtensions = ["png", "jpeg", "jpg"]

/**
 * 
 */
$("#new-comic-image-data").on("change", function () {
    let imageFile = document.getElementById("new-comic-image-data").files[0]

    if (getFileExtensionValid(imageFile)) {
        $("#new-comic-preview").removeClass("animate__fadeIn")
        $("#new-comic-preview").attr("src", URL.createObjectURL(imageFile))
        $("#new-comic-preview").addClass("animate__fadeIn")
    }
    else {
        $("#new-comic-preview").attr("src", $("#new-comic-preview").data("placeholder"))
    }
})

/**
 * 
 * @param {*} tabId 
 */
function switchTab(tabId) {
    $(".tab-header").removeClass("selected")
    $(".tab-body").removeClass("selected")
    $(`[data-tab-id="${tabId}"]`).addClass("selected")
}

/**
 * 
 * @param {*} file 
 * @returns 
 */
function getFileExtensionValid(file) {
    let fileSplit = file.name.split(".")
    return validExtensions.includes(fileSplit[fileSplit.length - 1])
}

function postNewComic(element) {
    let newComicForm = $("#new-comic-form")

    $(newComicForm).validate()
    if ($(element).hasClass("disabled") || !$(newComicForm).valid()) {
        return
    }
    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("comicNumber", $("#new-comic-comic-number").val())
    formData.append("comicTitle", $("#new-comic-comic-title").val())
    formData.append("comicDescription", $("#new-comic-comic-description").val())
    formData.append("imageData", $("#new-comic-image-data").prop("files")[0])
    formData.append("tags", $("#new-comic-comic-tags").val())
    formData.append("isHidden", $("#new-comic-is-hidden").is(":checked"))

    setFormLockState(newComicForm, true)
    let loaderId = displayLoading()
    setTimeout(function () {
        $.ajax({
            type: "POST",
            url: "?handler=Comic",
            data: formData,
            processData: false, 
            contentType: false,
            success: function (result) {
                if (result.success) {
                    hideLoading(loaderId)
                    displayAlert(`Comic ${$("#new-comic-comic-number").val()} posted!`)
                    clearForm(newComicForm)
                    $("#new-comic-preview").attr("src", $("#new-comic-preview").data("placeholder"))
                }
                else {
                    hideLoading(loaderId)
                    displayAlert("Failed to post comic")
                }
            },
            failure: function () {
                hideLoading(loaderId)
                displayAlert("Failed to post comics")
            },
            complete: function () {
                $(element).removeClass("disabled")
                setFormLockState(newComicForm, false)
            }
        })
    }, 500)
}