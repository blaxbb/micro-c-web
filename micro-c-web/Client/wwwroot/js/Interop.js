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

window.DestroySortable = function (selector) {
    if (window.sortable !== undefined) {
        window.sortable.destroy();
    }
    $(selector).empty();
}

window.CreateSortable = function (selector, categories, settingsPage) {
    if (window.sortable !== undefined) {
        window.sortable.destroy();
    }

    $(selector).replaceWith("<ul class='list-group' id='settings-category-items'></ul>");
    categories.forEach(cat => $(selector).append("<li class='list-group-item'>" + cat + "</li>"));

    var obj = $(selector).get(0);
    window.sortable = Sortable.create(obj, {
        animation: 100,
        onEnd: function (evt) {
            if (evt.oldIndex != evt.newIndex) {
                settingsPage.invokeMethodAsync("CategoryIndexChanged", evt.oldIndex, evt.newIndex);
            }
            return false;
        }
    });
}

window.SetSelectValue = function(id, value) {
    $(id).val(value);
}

window.Uncheck = function (selector) {
    $(selector).prop("checked", false);
}