let validExtensions = ["png", "jpeg", "jpg", "gif"]
let profilePicturePreviewSource = null

$(document).ready(function () {
    getManageComicsList()
    getEvents()
    setFormLockState($("#manage-comic-form"), true)
})

/**
 * On updating the image for a new comic, checks to ensure the uploaded file is of a valid format before updating the preview.
 * Otherwise, a placeholder is displayed.
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
 * On updating the image for an existing comic, checks to ensure the uploaded file is of a valid format before updating the preview.
 * Otherwise, a placeholder is displayed.
 */
$("#manage-comic-image-data").on("change", function () {
    let imageFile = document.getElementById("manage-comic-image-data").files[0]

    if (getFileExtensionValid(imageFile)) {
        $("#manage-comic-preview").removeClass("animate__fadeIn")
        $("#manage-comic-preview").attr("src", URL.createObjectURL(imageFile))
        $("#manage-comic-preview").addClass("animate__fadeIn")
    }
    else {
        $("#manage-comic-preview").attr("src", $("#manage-comic-preview").data("placeholder"))
    }
})

/**
 * On updating the image for a profile picture, checks to ensure the uploaded file is of a valid format before updating the preview.
 * Otherwise, a placeholder is displayed.
 */
$("#manage-me-image-data").on("change", function () {
    let imageFile = document.getElementById("manage-me-image-data").files[0]

    if (getFileExtensionValid(imageFile)) {
        $("#manage-me-preview").removeClass("animate__fadeIn")
        profilePicturePreviewSource = URL.createObjectURL(imageFile)
        $("#manage-me-preview").attr("src", profilePicturePreviewSource)
        $("#manage-me-preview").addClass("animate__fadeIn")
    }
    else {
        $("#manage-me-preview").attr("src", $("#manage-me-preview").data("placeholder"))
    }
})

/**
 * Handles tab switches, such that clicking a tab option displays the relevant tab content.
 * @param {*} tabId - The Id of the tab being selected, for which the matching content should be displayed.
 */
function switchTab(tabId) {
    $(".tab-header").removeClass("selected")
    $(".tab-body").removeClass("selected")
    $(`[data-tab-id="${tabId}"]`).addClass("selected")
}

/**
 * Returns whether the given file has a valid image file extension of the types defined.
 * @param {*} file - The file to check
 * @returns - True if the image extension is one of the types defined, otherwise false.
 */
function getFileExtensionValid(file) {
    let fileSplit = file.name.split(".")
    return validExtensions.includes(fileSplit[fileSplit.length - 1])
}

/**
 * Attempts to post a new comic.
 * If successful, clears the new comic input and resets the form.
 * Otherwise, notifies of any errors.
 * @param {*} element - The calling control to be disabled/enabled.
 */
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
    formData.append("isScheduled", $("#new-comic-is-scheduled").is(":checked"))
    formData.append("scheduleFor", $("#new-comic-scheduled-for").val())

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
                    displayAlert(result.message)
                }
            },
            failure: function () {
                displayAlert("Failed to post comics")
            },
            complete: function () {
                hideLoading(loaderId)
                $(element).removeClass("disabled")
                setFormLockState(newComicForm, false)
            }
        })
    }, 500)
}

/**
 * Attempts to fetch the list of events and display them.
 * @param {any} element - The calling control to be disabled/enabled.
 */
function getEvents(element) {
    if (element != undefined) {
        if ($(element).hasClass("disabled")) {
            return
        }
        $(element).addClass("disabled")
    }
    $("#manage-events-list").css("filter", "brightness(80%)")

    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "?handler=Events",
            dataType: "json",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            },
            success: function (result) {
                if (result.success) {
                    let eventsList = []
                    result.eventData.forEach(function (event) {
                        eventsList.push(`
                            <div class="row manage-events-item w-100 mx-auto">
                                <div class="col">${event.displayCreatedDate}</div>
                                <div class="col">${event.displayEventType}</div>
                                <div class="col">${event.details}</div>
                            </div>
                        `)
                    })
                    $("#manage-events-list").html(eventsList.join(""))
                }
                else {
                    displayAlert("Failed to load events")
                }
            },
            failure: function () {
                hideLoading(loaderId)
                displayAlert("Failed to load events")
            },
            complete: function () {
                $(element).removeClass("disabled")
                $("#manage-events-list").css("filter", "none")
            }
        })
    }, 500)
}

/**
* Attempts to fetch the list of existing comics and display them.
* @param {any} element - The calling control to be disabled/enabled.
*/
function getManageComicsList(element) {
    if (element != undefined) {
        if ($(element).hasClass("disabled")) {
            return
        }
        $(element).addClass("disabled")
    }
    $("#manage-comics-list").css("filter", "brightness(80%)")

    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "?handler=ManageComicsList",
            dataType: "json",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            },
            success: function (result) {
                if (result.success) {
                    let manageComicsList = []
                    result.manageComicListData.forEach(function(comic) {
                        manageComicsList.push(`
                            <div class="row w-100 mx-auto manage-comic-item" onclick="loadManageComic(this, '${comic.guid}')">${comic.displayName}</div>
                        `)
                    })
                    $("#manage-comics-list").html(manageComicsList.join(""))
                }
                else {
                    displayAlert("Failed to load comics")
                }
            },
            failure: function () {
                hideLoading(loaderId)
                displayAlert("Failed to load comics")
            },
            complete: function () {
                $(element).removeClass("disabled")
                $("#manage-comics-list").css("filter", "none")
            }
        })
    }, 500)
}

/**
 * Attempts to fetch the details and populate the comic management form for the selected comic.
 * @param {any} element - The calling control to be disabled/enabled.
 * @param {*} comicGuid - The GUID of the comic to load.
 */
function loadManageComic(element, comicGuid) {
    $(".manage-comic-item").removeClass("selected")
    setFormLockState($("#manage-comic-form"), true)
    
    $.ajax({
        type: "GET",
        url: "?handler=ManageComicDetails",
        dataType: "json",
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            comicGuid: comicGuid
        },
        success: function (result) {
            if (result.success) {
                setTimeout(function () {
                    $(element).addClass("selected")
                    setFormLockState($("#manage-comic-form"), false)
                
                    $("#manage-comic-image-data").val(null)
                    $("#manage-comic-preview").attr("src", `data:image/jpg;base64,${result.imageData}`)
                    $("#manage-comic-comic-guid").val(result.guid)
                    $("#manage-comic-comic-number").val(result.comicNumber)
                    $("#manage-comic-comic-title").val(result.comicTitle)
                    $("#manage-comic-comic-description").val(result.comicDescription)
                    $("#manage-comic-comic-tags").val(result.tags)

                    if (result.scheduledFor != null && result.publishDate == null) {
                        $("#manage-comic-schedule-for").val(result.scheduledFor)
                        $("#manage-comic-schedule-for").show()
                        $("#manage-comic-schedule-for").prop("readonly", false)
                    }
                    else {
                        $("#manage-comic-schedule-for").hide()
                        $("#manage-comic-schedule-for").prop("readonly", true)
                    }
                    
                    $("#manage-comic-is-hidden").prop("checked", result.isHidden)
                    $("#manage-comic-update").removeClass("disabled")
                })

                $("#manage-comic-form").validate()
            }
            else {
                displayAlert(result.message)
            }
        },
        failure: function () {
            displayAlert("Failed to load comic")
        }
    })
}

/**
 * On clicking the preview of the manage comic form, prompts a file upload.
 */
function manageComicPreviewClick() {
    if (!$("#manage-comic-form").hasClass("locked")) {
        $('#manage-comic-image-data').click()
    }
}

/**
* Attempts to update an existing comic.
* If successful, informs the user.
* Otherwise, notifies of any errors.
* @param {*} element - The calling control to be disabled/enabled.
*/
function patchComic(element) {
    let manageComicForm = $("#manage-comic-form")

    $(manageComicForm).validate()
    if ($(element).hasClass("disabled") || !$(manageComicForm).valid()) {
        return
    }
    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("comicGuid", $("#manage-comic-comic-guid").val())
    formData.append("comicNumber", $("#manage-comic-comic-number").val())
    formData.append("comicTitle", $("#manage-comic-comic-title").val())
    formData.append("comicDescription", $("#manage-comic-comic-description").val())
    formData.append("imageData", $("#manage-comic-image-data").prop("files")[0])
    formData.append("tags", $("#manage-comic-comic-tags").val())
    formData.append("isHidden", $("#manage-comic-is-hidden").is(":checked"))

    setFormLockState(manageComicForm, true)
    let loaderId = displayLoading()
    setTimeout(function () {
        $.ajax({
            type: "PATCH",
            url: "?handler=Comic",
            data: formData,
            processData: false, 
            contentType: false,
            success: function (result) {
                if (result.success) {
                    displayAlert("Comic updated!")
                }
                else {
                    displayAlert(result.message)
                }

                getManageComicsList($("#manage-comics-refresh"))
            },
            failure: function () {
                displayAlert("Failed to update comic")
            },
            complete: function () {
                hideLoading(loaderId)
                $(element).removeClass("disabled")
                setFormLockState(manageComicForm, false)
            }
        })
    }, 500)
}

/**
* Attempts to update the user's profile.
* If successful, informs the user.
* Otherwise, notifies of any errors.
* @param {*} element - The calling control to be disabled/enabled.
*/
function patchProfile(element) {
    let manageMeForm = $("#manage-me-form")

    $(manageMeForm).validate()
    if ($(element).hasClass("disabled") || !$(manageMeForm).valid()) {
        return
    }
    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("handle", $("#manage-me-handle").val())
    formData.append("imageData", $("#manage-me-image-data").prop("files")[0])

    setFormLockState(manageMeForm, true)
    let loaderId = displayLoading()
    setTimeout(function () {
        $.ajax({
            type: "PATCH",
            url: "?handler=Me",
            data: formData,
            processData: false, 
            contentType: false,
            success: function (result) {
                if (result.success) {
                    displayAlert("Profile updated!")
                    $(".pfp-roundel").attr("title", `Signed in as ${$("#manage-me-handle").val()}`)
                    $(".pfp-roundel").attr("src", profilePicturePreviewSource)
                }
                else {
                    displayAlert(result.message)
                }

                getManageComicsList($("#manage-comics-refresh"))
            },
            failure: function () {
                displayAlert("Failed to update profile")
            },
            complete: function () {
                hideLoading(loaderId)
                $(element).removeClass("disabled")
                setFormLockState(manageMeForm, false)
            }
        })
    }, 500)
}