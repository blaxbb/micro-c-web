window.setupTooltips = function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
}

window.stopPropagation = function (selector) {
    $(document).on('click', selector, function (e) {
        e.stopPropagation();
    });
}

window.ShowModal = function (id) {
    $(id).modal();
}

window.HideModal = function (id) {
    $(id).modal('hide');
}

window.HideTooltips = function () {
    $('.tooltip').hide();
}

window.SetLocalStorage = function (key, value) {
    localStorage.setItem(key, value);
}

window.GetLocalStorage = function (key) {
    return localStorage.getItem(key);
}