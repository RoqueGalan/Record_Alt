$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal].openModal_TipoMostrar").on("click", function (e) {

        $('#miModalContenido').load(this.href, function () {
            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            bindForm_TipoMostrar(this);
        });
        return false;
    });
});

function bindForm_TipoMostrar(dialog) {


    $('form', dialog).submit(function (e) {
        e.preventDefault();

        if ($(this).validate().valid()) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.success) {
                        $('#miModal').modal('hide');

                        $('#alertasDiv').load('/Base/_Alertas');


                        if ($('#renderListaTipoMostrar').length)
                            $('#renderListaTipoMostrar').load(result.url); //  Campo que actualizara
                        else
                            window.location.reload();


                    } else {
                        $('#miModalContenido').html(result);
                        bindForm_TipoMostrar(dialog);
                    }
                }
            });
        }
        return false;
    });
}