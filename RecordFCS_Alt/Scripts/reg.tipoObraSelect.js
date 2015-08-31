$('#LetraFolioID').focus();


$(function () {
	$.ajaxSetup({ cache: false });
	$('#LetraFolioID').focus();


	$('#TipoObraID').change(function () {
		var strSelecto = "";

		$('#TipoObraID option:selected').each(function () {
			strSelecto += $(this)[0].value;
		});

		$('#renderAtributosRequeridos').html('');



		if (strSelecto != "" || strSelecto != 0) {
			var myUrl = '/TipoPieza/ListaSelect';

			$.ajax({
				url: myUrl,
				type: "POST",
				data: { id: strSelecto, esRoot: true },
				success: function (result) {

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