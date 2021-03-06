﻿window.setupTooltips = function () {
    $(function () {
        HideTooltips();
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

let focusHandlers = {};

window.ShowModalFocus = function (modal, focus) {
    if (focusHandlers[modal] === undefined) {
        focusHandlers[modal] = true;
        $(modal).on('shown.bs.modal', function () {
            console.log(focus);
            $(focus).trigger('focus');
        });
    }
    $(modal).modal();
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

window.Uncheck = function (id) {
    $('#' + CSS.escape(id)).prop("checked", false);
}

window.Collapse = function (selector) {
    $(selector).collapse('hide');
}

window.setupSearchKeybindings = function (search) {
    document.addEventListener('keydown', (event) => {
        switch (event.key) {
            case "ArrowUp":
                break;
            case "ArrowDown":
                break;
        }
    });
}

window.ScrollToElement = function (ele) {
    if (ele.scrollIntoView) {
        ele.scrollIntoView();
    }
}

window.FocusElement = function (ele) {
    if (ele.scrollIntoView) {
        ele.focus();
    }
}
window.InitBarcode = function (modal) {
    window.scannerModal = modal;
    console.log("START");
    Quagga.init({
        inputStream: {
            name: "Live",
            type: "LiveStream",
            target: document.querySelector('#scannerView'),
            constraints: {
                width: { min: 640, ideal: 1920, max: 4096 },
                height: { min: 480, ideal: 1080, max: 2400 },
                aspectRatio: { min: 1, max: 2 },
                facingMode: 'environment', // or user
            },
            area: { // defines rectangle of the detection/localization area
                top: "25%",    // top offset
                right: "25%",  // right offset
                left: "25%",   // left offset
                bottom: "25%"  // bottom offset
            },
        },
        decoder: {
            readers: [
                "upc_reader",
                "code_128_reader"
            ],
            debug: {
                drawBoundingBox: false,
                showFrequency: false,
                drawScanline: false,
                showPattern: false
            }
        },
        numOfWorkers: navigator.hardwareConcurrency,
        frequency: 30,
        //locate: true,
        //locator: {
        //    halfSample: true,
        //    patchSize: "medium", // x-small, small, medium, large, x-large
        //    debug: {
        //        showCanvas: false,
        //        showPatches: false,
        //        showFoundPatches: false,
        //        showSkeleton: false,
        //        showLabels: false,
        //        showPatchLabels: false,
        //        showRemainingPatchLabels: false,
        //        boxFromPatches: {
        //            showTransformed: false,
        //            showTransformedBox: false,
        //            showBB: false
        //        }
        //    }
        //}
    }, function (err) {
        console.log("init");
        Quagga.start();
        if (err) {
            console.log(err);
            return
        }
    });
    $("#scannerModal").on('hide.bs.modal', function (e) {
        Quagga.stop();
    });
    Quagga.onDetected(function (result) {
        Quagga.stop();
        if (window.scannerModal) {
            window.scannerModal.invokeMethodAsync("ScanResult", result.codeResult.code);
        }
    });
}

window.StartScanner = function () {
    Quagga.start();
}

window.StopScanner = function () {
    Quagga.stop();
}

window.PrintElement = function (id) {
    var divToPrint = document.getElementById(id);

    var stylesheets = $("[rel='stylesheet']").toArray();
    
    newWin = window.open("");
    window.newWin = newWin;
    newWin.document.write('<html><head>');
    stylesheets.forEach(stylesheet => newWin.document.write(stylesheet.outerHTML));
    newWin.document.write('<link rel="stylesheet" href="main.css" type="text/css" />');
    newWin.document.write('</head><body ><div>');
    newWin.document.write(divToPrint.innerHTML);
    newWin.document.write('</div></body></html>');
    newWin.document.close(); // necessary for IE >= 10
    console.log($(newWin).find("#id").innerHTML);

    newWin.print();
    //newWin.close();
}

window.unfocus = function () {
    $(':focus').blur();
}