//logica para renderizar los campos requeridos

$(function () {
    $.ajaxSetup({ cache: false });
    $('#TipoPiezaID').change(function () {
        var strSelecto = "";

        $('#TipoPiezaID option:selected').each(function () {
            strSelecto += $(this)[0].value;
        });

        if (strSelecto != "" || strSelecto != 0) {
            var myUrl = '/Atributo/RegistroFormulario';

            $('#renderIconoLoading').html('' +
                '<div class="progress">' +
                    '<div class="progress-bar progress-bar-striped active bar-loading" role="progressbar"' +
                    'aria-valuenow="100" aria-valuemin="0" aria-valuemax="100">' +
                        'Espere por favor, procesando la solicitud...' +
                    '</div>' +
                '</div>');

            var bar = $('.bar-loading');
            $(function () {
                $(bar).each(function () {
                    bar_width = $(this).attr('aria-valuenow');
                    $(this).width(bar_width + '%');
                });
            }).delay(800);


           

            $.ajax({
                url: myUrl,
                type: "POST",
                data: { id: strSelecto },
            
                
                success: function (result) {

                    //$('#renderIcono').html('');

                    $('#renderAtributosRequeridos').html(result);
                }
            });

        } else {

            $('#renderAtributosRequeridos').html('');

            //$('#renderAtributosRequeridos').html('' +
            //    '<div class="text-center">' +
            //        '<span class="fa fa-10x fa-list-alt text-muted"></span>' +
            //    '</div>');

            //$('#renderIcono').html('' +
            //            '<div class="text-center">' +
            //                '<span class="fa fa-4x fa-arrow-up text-primary"></span>' +
            //                '<p>' +
            //                    '<i class="text-muted">' +
            //                        'Selecciona el TIPO DE PIEZA.' +
            //                    '</i>' +
            //                '</p>' +
            //            '</div>');


        }
    });
    return false;
});