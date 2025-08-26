let validExtensions = ["png", "jpeg", "jpg", "gif"]
let profilePicturePreviewSource = null
let loadedComicToManageWasScheduled = false

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
    formData.append("comicAltDescription", $("#new-comic-comic-alt-description").val())
    formData.append("imageData", $("#new-comic-image-data").prop("files")[0])
    formData.append("tags", $("#new-comic-comic-tags").val())
    formData.append("isHidden", $("#new-comic-is-hidden").is(":checked"))
    formData.append("isScheduled", $("#new-comic-is-scheduled").is(":checked"))
    formData.append("scheduleFor", $("#new-comic-schedule-for").val())

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
    $("#manage-events-count").text("Fetching...")

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
                    $("#manage-events-count").text(`Displaying ${result.eventData.length} ${result.eventData.length === 1 ? "event" : "events"}`)
                    let eventsList = []
                    result.eventData.forEach(function (event) {
                        eventsList.push(`
                            <div class="row manage-events-item w-100 mx-auto">
                                <div class="col">${UtcDateTimeToLocalDateTimeString(event.displayCreatedDate)}</div>
                                <div class="col">${event.displayEventType}</div>
                                <div class="col" style="white-space: break-spaces;">${event.details}</div>
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
                $("#manage-events-count").text("Failed to load events")
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
    $("#manage-comics-count").text("Fetching...")

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
                    $("#manage-comics-count").text(`Displaying ${result.manageComicsListData.length} ${result.manageComicsListData.length === 1 ? "comic" : "comics"}`)
                    let manageComicsList = []
                    result.manageComicsListData.forEach(function(comic) {
                        manageComicsList.push(`
                            <div class="row w-100 mx-auto manage-comic-item" onclick="loadManageComic(this, '${comic.guid}')">
                                <div class="col-5 fw-bold">${comic.displayName}</div>
                                <div class="col fst-italic">${comic.guid}</div>
                            </div>
                        `)
                    })
                    $("#manage-comics-list").html(manageComicsList.join(""))
                }
                else {
                    displayAlert("Failed to load comics")
                }
            },
            failure: function () {
                $("#manage-comics-count").text("Failed to load comics")
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
                    $("#manage-comic-comic-alt-description").val(result.comicAltDescription)
                    $("#manage-comic-comic-tags").val(result.tags)

                    $("#manage-comic-published-info").text(result.displayPublished === null ? "N/A" : UtcDateTimeToLocalDateTimeString(result.displayPublished))
                    $("#manage-comic-modified-info").text(`${UtcDateTimeToLocalDateTimeString(result.displayLastModified)} by ${result.lastModifiedHandle}`)
                    $("#manage-comic-heart-info").text(result.likeCount)

                    if (result.scheduledDate != null && result.publishDate == null) {
                        loadedComicToManageWasScheduled = true
                        $("#manage-comic-schedule-details").show()
                        $("#manage-comic-schedule-for").val(result.scheduledDate.split("T")[0])

                        $("#manage-comic-schedule-for").prop("readonly", false)
                        $("#manage-comic-is-scheduled").prop("checked", true)
                    }
                    else {
                        loadedComicToManageWasScheduled = false
                        $("#manage-comic-schedule-details").hide()
                        $("#manage-comic-schedule-for").val(new Date().getDate() + 1)
                        $("#manage-comic-schedule-for").prop("readonly", true)
                        $("#manage-comic-is-scheduled").prop("checked", false)
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

    // Unscheduling a scheduled comic will make it available immediately
    if (loadedComicToManageWasScheduled
        && !$("#manage-comic-is-scheduled").is(":checked")
        && !confirm("This comic is already scheduled for release!\nAre you sure you want to deschedule it?\nThe comic will be posted immediately.")) {
        return;
    }

    // Scheduling a released comic will hide it immediately
    if (!loadedComicToManageWasScheduled
        && $("#manage-comic-is-scheduled").is(":checked")
        && !$("#manage-comic-is-hidden").is(":checked")
        && !confirm("This comic is already released!\nAre you sure you want to schedule it?\nThe comic will no longer be available.")) {
        return;
    }

    // Make sure the post is hidden if scheduled


    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("comicGuid", $("#manage-comic-comic-guid").val())
    formData.append("comicNumber", $("#manage-comic-comic-number").val())
    formData.append("comicTitle", $("#manage-comic-comic-title").val())
    formData.append("comicDescription", $("#manage-comic-comic-description").val())
    formData.append("comicAltDescription", $("#manage-comic-comic-alt-description").val())
    formData.append("imageData", $("#manage-comic-image-data").prop("files")[0])
    formData.append("tags", $("#manage-comic-comic-tags").val())
    formData.append("isHidden", $("#manage-comic-is-hidden").is(":checked"))
    formData.append("isScheduled", $("#manage-comic-is-scheduled").is(":checked"))
    formData.append("scheduleFor", $("#manage-comic-schedule-for").val())

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
    formData.append("socialLink", $("#manage-me-social").val())
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

/**
 * On toggling whether the comic should be scheduled, determines visibility of corresponding controls and checked state of hidden option.
 * @param {any} element - Calling element
 * @param {any} comicType - Whether this is for the new comic control set, or manage comic control set
 */
function toggleComicIsScheduled(element, comicType) {
    toggleElementReadOnlyState($(`#${comicType}-comic-schedule-for`))
    if ($(element).is(":checked")) {
        $(`#${comicType}-comic-schedule-details`).show()
        $(`#${comicType}-comic-schedule-for`).prop("readonly", false)
    }
    else {
        $(`#${comicType}-comic-schedule-details`).hide()
        $(`#${comicType}-comic-schedule-for`).prop("readonly", true)
    }

    $(`#${comicType}-comic-is-hidden`).prop("checked", $(element).is(":checked"))
}

/**
 * On toggling whether the comic should be hidden, determines visibility of corresponding controls and checked state of scheduled option.
 * @param {any} element - Calling element
 * @param {any} comicType - Whether this is for the new comic control set, or manage comic control set
 */
function toggleComicIsHidden(element, comicType) {
    if (!$(element).is(":checked")) {
        $(`#${comicType}-comic-schedule-details`).hide()
        $(`#${comicType}-comic-is-scheduled`).prop("checked", false)
    }
}

/**
* Attempts to update the app settings.
* If successful, informs the user.
* Otherwise, notifies of any errors.
* @param {*} element - The calling control to be disabled/enabled.
*/function patchAppSettings(element) {
    let manageAppForm = $("#manage-app-form")

    $(manageAppForm).validate()
    if ($(element).hasClass("disabled") || !$(manageAppForm).valid()) {
        return
    }
    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("publicAccessEnabled", $("#manage-app-public-access").val())
    formData.append("archiveAccess", $("#manage-app-archive-access").val())
    formData.append("archiveMaximumComicsPerFetch", $("#manage-app-max-comics-per-fetch").val())
    formData.append("manageEventsMaximumHistory", $("#manage-app-max-event-history").val())
    formData.append("scheduledComicReleaseTime", $("#manage-app-comic-release-time").val())

    setFormLockState(manageAppForm, true)
    let loaderId = displayLoading()
    setTimeout(function () {
        $.ajax({
            type: "PATCH",
            url: "?handler=AppSettings",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.success) {
                    displayAlert("App settings updated!")
                }
                else {
                    displayAlert(result.message)
                }
            },
            failure: function () {
                displayAlert("Failed to update app settings")
            },
            complete: function () {
                hideLoading(loaderId)
                $(element).removeClass("disabled")
                setFormLockState(manageAppForm, false)
            }
        })
    }, 500)
}

/**
* Attempts to fetch the list of existing users and display them.
* @param {any} element - The calling control to be disabled/enabled.
*/
function getManageUsersList(element) {
    if (element != undefined) {
        if ($(element).hasClass("disabled")) {
            return
        }
        $(element).addClass("disabled")
    }
    $("#manage-users-list").css("filter", "brightness(80%)")
    $("#manage-users-count").text("Fetching...")

    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "?handler=ManageUsersList",
            dataType: "json",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            },
            success: function (result) {
                if (result.success) {
                    $("#manage-users-count").text(`Displaying ${result.manageUsersListData.length} ${result.manageUsersListData.length === 1 ? "user" : "users"}`)
                    let manageUsersList = []
                    result.manageUsersListData.forEach(function (user) {
                        manageUsersList.push(`
                            <div class="row w-100 mx-auto manage-user-item" onclick="loadManageUser(this, '${user.guid}')">
                                <div class="col-5 fw-bold">${user.displayEmail}</div>
                                <div class="col fst-italic">${user.guid}</div>
                            </div>
                        `)
                    })
                    $("#manage-users-list").html(manageUsersList.join(""))
                }
                else {
                    displayAlert("Failed to load users")
                }
            },
            failure: function () {
                $("#manage-users-count").text("Failed to load users")
                hideLoading(loaderId)
                displayAlert("Failed to load users")
            },
            complete: function () {
                $(element).removeClass("disabled")
                $("#manage-users-list").css("filter", "none")
            }
        })
    }, 500)
}

/**
* Attempts to fetch the details and populate the user management form for the selected user.
* @param {any} element - The calling control to be disabled/enabled.
* @param {*} userGuid - The GUID of the user to load.
*/
function loadManageUser(element, userGuid) {
    $(".manage-user-item").removeClass("selected")
    setFormLockState($("#manage-user-form"), true)

    $.ajax({
        type: "GET",
        url: "?handler=ManageUserDetails",
        dataType: "json",
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            userGuid: userGuid
        },
        success: function (result) {
            if (result.success) {
                setTimeout(function () {
                    $(element).addClass("selected")
                    setFormLockState($("#manage-user-form"), false)

                    $("#manage-user-preview").attr("src", `data:image/jpg;base64,${result.displayPictureData}`)
                    $("#manage-user-user-guid").val(result.guid)
                    $("#manage-user-handle-info").text(result.handle)
                    $("#manage-user-social-info").text(result.socialLink)
                    $("#manage-user-social-link").attr("href", result.socialLink)

                    $("#manage-user-last-login-info").text(result.lastLoginDate === null
                        ? "User has never logged in"
                        : `${UtcDateTimeToLocalDateTimeString(result.lastLoginDate)} from ${result.lastLoginIpAddress}`)
                    $("#manage-user-is-locked").prop("checked", result.accountIsLocked)
                    $("#manage-user-is-disabled").prop("checked", result.accountIsDisabled)
                })

                $("#manage-user-form").validate()
                setFormLockState($("#manage-user-form"), false)
                $("#manage-user-update").removeClass("disabled")
            }
            else {
                displayAlert(result.message)
            }
        },
        failure: function () {
            displayAlert("Failed to load user")
        }
    })
}

/**
* Attempts to update an existing user.
* If successful, informs the user.
* Otherwise, notifies of any errors.
* @param {*} element - The calling control to be disabled/enabled.
*/
function patchUser(element) {
    let manageUserForm = $("#manage-user-form")

    $(manageUserForm).validate()
    if ($(element).hasClass("disabled") || !$(manageUserForm).valid()) {
        return
    }

    $(element).addClass("disabled")

    let formData = new FormData()
    formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val())
    formData.append("userGuid", $("#manage-user-user-guid").val())
    formData.append("accountIsLocked", $("#manage-user-is-locked").is(":checked"))
    formData.append("accountIsDisabled", $("#manage-user-is-disabled").is(":checked"))

    setFormLockState(manageUserForm, true)
    let loaderId = displayLoading()
    setTimeout(function () {
        $.ajax({
            type: "PATCH",
            url: "?handler=User",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.success) {
                    displayAlert("User updated!")
                }
                else {
                    displayAlert(result.message)
                }

                getManageUsersList($("#manage-users-refresh"))
            },
            failure: function () {
                displayAlert("Failed to update user")
            },
            complete: function () {
                hideLoading(loaderId)
                $(element).removeClass("disabled")
                setFormLockState(manageUserForm, false)
            }
        })
    }, 500)
}