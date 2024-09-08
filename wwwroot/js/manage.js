let validExtensions = ["png", "jpeg", "jpg"]
let profilePicturePreviewSource = null

$(document).ready(function() {
    getManageComicsList()
    setFormLockState($("#manage-comic-form"), true)
})

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
 * 
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

/**
 * 
 * @param {*} element 
 * @returns 
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
 * 
 * @param {*} element 
 */
function getManageComicsList(element) {
    if ($(element).hasClass("disabled")) {
        return
    }
    
    $(element).addClass("disabled")
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
 * 
 * @param {*} comicGuid 
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
 * 
 */
function manageComicPreviewClick() {
    if (!$("#manage-comic-form").hasClass("locked")) {
        $('#manage-comic-image-data').click()
    }
}

/**
 * 
 * @param {*} element 
 * @returns 
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
 * 
 * @param {*} element 
 * @returns 
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