let menuTransitioning = false;
let autoHideMenu = false

$("html").on("click", function (event) {
    if (autoHideMenu && $("#site-menu").is(":visible") && $(event.target).attr("id") != "site-menu" && $(event.target).closest(".container").attr("id") != "site-menu") {
        toggleMenu($("#navbar-menu"))
    }
})

/**
 * Toggles the site navigation menu visibility.
 * @param {*} element - Button summoning the navigation menu
 */
function toggleMenu(element) {
    if (menuTransitioning) {
        return
    }

    $("body").css("overflow", "hidden")
    $(element).toggleClass("toggle-active")
    $(element).toggleClass("animate__animated animate__pulse animate__faster")
    let siteMenu = $("#site-menu")
    menuTransitioning = true

    if ($(element).hasClass("toggle-active")) {
        $(siteMenu).fadeIn(200)
        $(siteMenu).removeClass("animate__animated animate__fadeOutRight animate__faster")
        $(siteMenu).addClass("animate__animated animate__fadeInRight animate__faster")
    }
    else {
        $(siteMenu).fadeOut(200)
        $(siteMenu).removeClass("animate__animated animate__fadeInRight animate__faster")
        $(siteMenu).addClass("animate__animated animate__fadeOutRight animate__faster")
    }
    
    setTimeout(() => {
        menuTransitioning = false
        $("body").css("overflow", "auto")
    }, 500);
}

/**
 * After a brief delay, redirect to the site home page
 * @param {*} delay - The time (in milliseconds) to wait before redirecting, if any.
 */
function resetRedirect(delay = 2000) {
    setTimeout(() => {
        location.href = '/Index'
    }, delay);
}

/**
 * Returns a GUID.
 * @returns string GUID
 */
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

/**
 * Copies the given content to the clipboard.
 * @param {*} content - The content to copy to the clipboard.
 */
function copyToClipboard(content) {
    navigator.clipboard.writeText(content);
    displayAlert("Copied!")
}

/**
 * 
 * @param {*} text 
 * @param {*} delay 
 */
function displayAlert(text, delay = 3000) {
    let alertId = uuidv4()
    let alert = $(`
        <div class="alert-container" id="${alertId}">
            <div class="alert-content animate__animated animate__bounceInDown">${text}</div>
        </div>
    `)
    $("header").append(alert)
    setTimeout(function () {
        $(`#${alertId}`).remove()
    }, delay)
}

/**
 * 
 * @returns 
 */
function displayLoading() {
    let loaderId = uuidv4()
    let loader = $(`
        <div class="loading-container" id="${loaderId}">
            <div class="loader">
        </div>
    `)
    $("header").append(loader)
    return loaderId
}

/**
 * 
 * @param {*} loaderId 
 */
function hideLoading(loaderId) {
    $(`#${loaderId}`).remove()
}

/**
* Returns true when an element is scrolled to the bottom.
* @param {*} element - The element to test
* @returns - true if the element is scrolled to the bottom
*/
function elementIsScrolledToBottom(element) {
    console.log(element.scrollTop)
    return element.scrollTop > 0 && Math.abs(element.scrollHeight - element.clientHeight - element.scrollTop) <= 1
}

/**
 * Sets the lock state of a form, determining if the controls on the form (select, input and textarea) elements may be interacted with.
 * @param {*} form - The form to evaluate for controls to lock/unlock
 * @param {*} state - The locked/unlocked state to set (true to lock)
 */
function setFormLockState(form, state) {
    $(form).find("input").prop("readonly", state)
    $(form).find("select").prop("readonly", state)
    $(form).find("textarea").prop("readonly", state)
    $(form).find(":checkbox").prop("disabled", state)

    if (state) {
        $(form).find('select option:not(:selected)').prop('disabled', true)
        $(form).addClass("locked")
    }
    else {
        $(form).find('select option').prop('disabled', false)
        $(form).find('select option[default]').prop('disabled', true)
        $(form).removeClass("locked")
    }
}

/**
 * Sets the read only state of a single element, based on its existing state.
 * @param {*} element - The element to evaluate
 */
function toggleElementReadOnlyState(element) {
    $(element).prop("readonly", !$(element).prop("readonly"))
}

/**
 * Clears all input-type nodes for a given form. Select fields are set to their first value
 * @param {*} form 
 */
function clearForm(form) {
    $(form).find("input").val("")
    $(form).find("textarea").val("")
    $(form).find("select").prop("selectedIndex", 0)
    $(form).find(":checkbox").prop("checked", false)
}

/**
 * Shows the given modal, based on the ID.
 * @param {any} modalId - The ID of the modal to show
 */
function showModal(modalId) {
    $(`#${modalId}`).fadeIn(200)
}

/**
 * Hides the given modal, based on the ID.
 * @param {any} modalId - The ID of the modal to hide
 */
function hideModal(modalId) {
    $(`#${modalId}`).fadeOut(200)
}