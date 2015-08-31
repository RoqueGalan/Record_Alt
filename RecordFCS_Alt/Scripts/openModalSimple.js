$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal].openModal").on("click", function (e) {
        $('#miModalContenido').load(this.href, function () {
            $('#miModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
        });
        return false;
    });
});