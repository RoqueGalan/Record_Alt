﻿$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal].openModal_LetraFolio").on("click", function (e) {

        $('#miModalContenido').load(this.href, function () {
            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            bindForm_LetraFolio(this);
        });
        return false;
    });
});

function bindForm_LetraFolio(dialog) {
    $('form', dialog).submit(function (e) {
        e.preventDefault();

        var validarOK = false;


        validarOK = $(this).validate().valid();

        if (validarOK) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.success) {
                        $('#miModal').modal('hide');

                        if ($('#renderListaLetraFolio').length)
                            $('#renderListaLetraFolio').load(result.url); //  Campo que actualizara
                        else
                            window.location.reload();

                        $('#alertasDiv').load('/Base/_Alertas');

                    } else {
                        $('#miModalContenido').html(result);
                        bindForm_LetraFolio(dialog);
                    }
                }
            });
        }
        return false;
    });
}



//Paginador cargar vista parcial en el div
$(function () {
    $.ajaxSetup({ cache: false });

    $("#contentPager a").on("click", function (e) {
        if ($(this).attr("href")) {
            $.ajax({
                url: $(this).attr("href"),
                type: 'GET',
                success: function (result) {
                    $('#renderListaLetraFolio').html(result);
                }
            });
        }
        return false;
    });
});


//Buscador carga vista parcial en el div
$(function () {
    $.ajaxSetup({ cache: false });

    $('#FormBusqueda').submit(function (e) {

        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                $('#renderListaLetraFolio').html(result);
            }
        });
        return false;
    });
});