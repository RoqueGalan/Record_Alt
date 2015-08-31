$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal].openModal_TipoObra").on("click", function (e) {

        $('#miModalContenido').load(this.href, function () {
            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            bindForm_TipoObra(this);
        });
        return false;
    });
});

function bindForm_TipoObra(dialog) {
   

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

                        
                        if ($('#renderListaTipoObra').length)
                            $('#renderListaTipoObra').load(result.url); //  Campo que actualizara
                        else
                            window.location.reload();


                    } else {
                        $('#miModalContenido').html(result);
                        bindForm_TipoObra(dialog);
                    }
                }
            });
        }
        return false;
    });
}