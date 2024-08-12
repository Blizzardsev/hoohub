let menuTransitioning = false;

$("html").on("click", function (event) {
    if ($("#site-menu").is(":visible") && $(event.target).attr("id") != "site-menu" && $(event.target).closest(".container").attr("id") != "site-menu") {
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
 * 
 * @param {*} content 
 */
function copyToClipboard(content) {
    navigator.clipboard.writeText(content);
    alert("")
}