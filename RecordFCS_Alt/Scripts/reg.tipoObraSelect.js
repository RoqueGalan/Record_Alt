$('#LetraFolioID').focus();


$(function () {
	$.ajaxSetup({ cache: false });
	$('#LetraFolioID').focus();



	$('#LetraFolioID').change(function (e) {
	    $('#TipoObraID').focus();

	});





	$('#TipoObraID').change(function () {
		var strSelecto = "";

		$('#TipoObraID option:selected').each(function () {
			strSelecto += $(this)[0].value;
		});

		$('#renderAtributosRequeridos').html('');



		if (strSelecto != "" || strSelecto != 0) {
			var myUrl = '/TipoPieza/ListaSelect';
			//alert("alerta 1");
			$.ajax({
				url: myUrl,
				type: "POST",
				data: { id: strSelecto, esRoot: true },
				success: function (result) {
				    //alert("alerta 2");

					$('#renderSelectTipoPieza').html(result);

					$('#TipoPiezaID').focus();
				}
			});

		}
		else {
			$('#renderSelectTipoPieza').html('' +
				'<select class="form-control">' +
					'<option>- Selecciona un tipo de obra -</option>' +
				'</select>');

		}

	});

	return false;
});