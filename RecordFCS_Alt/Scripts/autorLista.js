$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal].openModal_Autor").on("click", function (e) {

        $('#miModalContenido').load(this.href, function () {
            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            bindForm_Autor(this);
        });
        return false;
    });
});

function bindForm_Autor(dialog) {
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

                        if ($('#renderListaAutor').length)
                            $('#renderListaAutor').load(result.url); //  Campo que actualizara
                        else
                            window.location.reload();

                        $('#alertasDiv').load('/Base/_Alertas');

                    } else {
                        $('#miModalContenido').html(result);
                        bindForm_Autor(dialog);
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
                    $('#renderListaAutor').html(result);
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
                $('#renderListaAutor').html(result);
            }
        });
        return false;
    });
});