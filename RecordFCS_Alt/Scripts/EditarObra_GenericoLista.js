$('#btnAccion_@AtributoID').on("click", function (e) {


    var attID = '@AtributoID';
    var tipoAttID = '@TipoAtributoID';
    var $inputSelect = $('#id_' + attID);
    var $itemSelected = $inputSelect.find('option:selected');
    var $btnCheck = $('#checkAdd_' + attID);
    var $inputBuscar = $('#buscar_' + attID);

    var $btn = $('#btnAccion_@AtributoID');

    /*
    Elemento Seleccionado:
        Texto = $itemSelected.text()
        Valor o id = $itemSelected.val()

    Input Buscador
        Valor = $inputBuscar.val()

    Boton Checked
        Valor = $btnCheck.is(':checked')

    Atributos
        Mostrar: $btn.attr("name")
        Cambiar Valor :  $btn.attr("name","prueba")
    */

    if ($btn.attr("name") == "Agregar") {
        //alert("entro a agregar");

        var myUrl = '@Url.Action("Crear", "ListaValor", new { id = @TipoAtributoID, EsRegistroObra = true })';

        $.ajaxSetup({ cache: false });

        $('#miModalContenido').load(myUrl, function () {
            $('#Valor').val($inputBuscar.val());

            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');

            bindForm_ListaGenerica(this);
        });

        function bindForm_ListaGenerica(dialog) {

            alert("listaGen_@AtributoID");

            var attID = '@AtributoID';

            $('form', dialog).submit(function (e) {

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

                                $('#buscar_' + attID).val(result.valor);

                                $('#checkAdd_' + attID).trigger("click");
                                $('#btnAccion_' + attID).trigger("click");

                                $('#alertasDiv').load('/Base/_Alertas');

                            } else {
                                $('#miModalContenido').html(result);
                                bindForm_ListaGenerica(dialog);
                            }
                        }
                    });
                }
                return false;
            });

        }

        //mostrar modal para confirmar el campo que se va agregar

    }
    else {

        $.ajaxSetup({ cache: false });

        var myUrl = '@Url.Action("GenerarLista","ListaValor",null)';

        $inputSelect.html('<option> Cargando... </option>');

        $.ajax({
            url: myUrl,
            type: 'POST',
            data: { id: tipoAttID, Filtro: $inputBuscar.val() },
            dataType: 'json',
            success: function (result) {

                if (result.length) {
                    $inputSelect.html('<option> - Elije un resultado - </option>');

                    for (var i = 0; i < result.length; i++) {
                        $inputSelect.append('<option value = "' + result[i].ListaValorID + '">' + result[i].Valor + '</option>');
                    }
                }
                else {
                    $inputSelect.html('<option> - Sin resultados - </option>');
                }


            }
        });

        $inputSelect.focus();

    }

});

$('#checkAdd_@AtributoID').on("click", function (e) {

    var attID = '@AtributoID';
    var $btnCheck = $('#checkAdd_' + attID);
    var $btn = $('#btnAccion_@AtributoID');
    var $inputBuscar = $('#buscar_' + attID);


    if ($btnCheck.is(':checked')) {
        $btn.attr("name", "Agregar");
        $btn.html('<span class="fa fa-floppy-o"></span>');
        $btn.removeClass("btn-default");
        $btn.addClass("btn-success");

        $inputBuscar.attr("placeholder", "Guardar valor");

    }
    else {
        $btn.attr("name", "Buscar");
        $btn.html('<span class="fa fa-search"></span>');
        $btn.removeClass("btn-success");
        $btn.addClass("btn-default");

        $inputBuscar.attr("placeholder", "Buscar valor");

    }
});