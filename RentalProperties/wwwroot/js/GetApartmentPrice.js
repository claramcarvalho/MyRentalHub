/*
$(function () {
    $("#apartmentId").change(function () {
        console.log("entrei no change")
        var ap = document.getElementById("apartmentId")
        console.log(ap.value)
        var apartmentId = $(this).val();
        var url = "@Url.Action('GetApartmentPrice','ApartmentsController')"
        $.ajax({
            url: url, // Substitua pela URL correta
            type: 'GET',
            data: { apartmentId: ap.value },
            success: function (response) {
                $("#priceRent").val(response);
            },
            error: function () {
                console.log("Erro ao obter o preço do apartamento.");
            }
        });
    });
}); */