const twoFactor = $(".two-factor")

$(document).ready(function () {
    $(twoFactor).val("")
})

/**
 * Allows pasting/typing to submit the 2FA form so long as all six characters are present
 */
$(twoFactor).on("keyup", function() {
    if ($(this).val().length == 6) {
        identitySubmit($(".site-option"))
    }
})

$(twoFactor).on("paste", function() {
    if ($(this).val().length == 6) {
        identitySubmit($(".site-option"))
    }
})

/**
 * Allows the Enter key to submit the Identity forms
 */
$("input").keydown(function (event) {
    if (event.keyCode == 13) {
        identitySubmit(this)
    }
})

/**
 * Generic submit for an identity form; common to all Areas/Identity pages.
 * @param {*} element - The form to submit
 */
function identitySubmit(element) {
    const form = $(element).closest("form")
    const button = $(form).find(".option-button")

    if (form.valid() && !$(button).hasClass("disabled")) {
        $(form).find("input").prop("readonly", true)

        $(button).addClass("disabled")
        $(button).addClass("animate__animated animate__pulse animate__fast animate__infinite infinite")

        setTimeout(() => {
            form.submit()
        }, 500);
    }
}

/**
 * After a brief delay, redirect to the site home page
 */
function resetRedirect() {
    setTimeout(() => {
        location.href = '/Index'
    }, 2000);
}