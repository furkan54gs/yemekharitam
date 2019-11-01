
var contentString = '<div id="content">' +
    '<div id="siteNotice" align="center">' +
    '<img width="100" src="/Content/images/#IMG_SRC#"/>' +
    '</div>' +
    '<h4 id="firstHeading" class="firstHeading">#FIRST_HEADING#</h4>' +
    '<div id="bodyContent">' +
    '#DETAIL# <br/>' +
    '</p>' +
    '<a type="button" href="/Home/SupplierDetail/#BUTTON#" class="btn btn-primary btn-sm">Detaylar</a>' +
    '</div>' +
    '</div>';

var suppString =
    '<div class="col-lg-4">' +
    '<img width="75%" height="75%" src="/Content/images/#IMG#" alt="">' +
    '<h4>#NAME#</h4>' +
    '<p><a class="btn btn-default" href="/Home/SupplierDetail/#BUTTON#" role="button">Ayrýntýlar</a></p>' +
    '</div>';


// haritayý initialize edeceðimiz metod, bu metodu document ready'de çaðýrýyoruz.
function initMap() {

    var myCenter = new google.maps.LatLng();
    var mapProp = {
        center: myCenter,
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDoubleClickZoom: true
    };
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

    ///////////////click event
    var clickmarker = new google.maps.Marker({
        draggable: false
    });

    google.maps.event.addListener(map, 'dblclick', function (event) {

        clickmarker.setPosition(event.latLng);
        clickmarker.setMap(map);
        clickmarker.setAnimation(google.maps.Animation.DROP);
        var lat = clickmarker.getPosition().lat();
        var lati = clickmarker.getPosition().lat();
        var lng = clickmarker.getPosition().lng();
        var lngi = clickmarker.getPosition().lng();

        jQuery("#lat").val(lat);
        jQuery("#lng").val(lng);

        $("#demo").empty();

        $.ajax({
            url: '@Url.Action("GetLocation", "Home")',
            type: 'POST',
            dataType: 'json',
            data: { lati, lngi },
            success: function (data) {
                var tson = JSON.parse(data);



                for (var i = 0; i <= tson.length; i += 3) {
                    if (i < 1) {
                        var ts = '<div class="item active">' +
                            '<div class="row">' + suppString;
                    }
                    else {
                        var ts = '<div class="item">' +
                            '<div class="row">' + suppString;
                    }
                    var ks;
                    ts = ts.replace("#NAME#", tson[i].name);
                    ts = ts.replace("#IMG#", tson[i].img);
                    ts = ts.replace("#BUTTON#", tson[i].id);
                    ks = ts;
                    var ts = suppString;
                    ts = ts.replace("#NAME#", tson[i + 1].name);
                    ts = ts.replace("#IMG#", tson[i + 1].img);
                    ts = ts.replace("#BUTTON#", tson[i + 1].id);
                    ks += ts;
                    var ts = suppString + '</div>' + '</div>';
                    ts = ts.replace("#NAME#", tson[i + 2].name);
                    ts = ts.replace("#IMG#", tson[i + 2].img);
                    ts = ts.replace("#BUTTON#", tson[i + 2].id);
                    ks += ts;

                    $('#demo').append(ks);

                }
            }
        });

    });
    //////////

    // /maps/documentation/javascript/examples/map-geolocation
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            var lati = position.coords.latitude;
            var lngi = position.coords.longitude;
            var here = new google.maps.Marker({
                position: pos,
                map: map,
                title: 'Konumunuz'
            });

            //infoWindow.setPosition(pos);
            //infoWindow.setContent(here);
            //infoWindow.open(map);
            map.setCenter(pos);

            $.ajax({
                url: '@Url.Action("GetLocation", "Home")',
                type: 'POST',
                dataType: 'json',
                data: { lati, lngi },
                success: function (data) {
                    var tson = JSON.parse(data);
                    for (var i = 0; i <= tson.length; i += 3) {
                        if (i < 1) {
                            var ts = '<div class="item active">' +
                                '<div class="row">' + suppString;
                        }
                        else {
                            var ts = '<div class="item">' +
                                '<div class="row">' + suppString;
                        }
                        var ks;
                        ts = ts.replace("#NAME#", tson[i].name);
                        ts = ts.replace("#IMG#", tson[i].img);
                        ts = ts.replace("#BUTTON#", tson[i].id);
                        ks = ts;
                        var ts = suppString;
                        ts = ts.replace("#NAME#", tson[i + 1].name);
                        ts = ts.replace("#IMG#", tson[i + 1].img);
                        ts = ts.replace("#BUTTON#", tson[i + 1].id);
                        ks += ts;
                        var ts = suppString + '</div>' + '</div>';
                        ts = ts.replace("#NAME#", tson[i + 2].name);
                        ts = ts.replace("#IMG#", tson[i + 2].img);
                        ts = ts.replace("#BUTTON#", tson[i + 2].id);
                        ks += ts;

                        $('#demo').append(ks);

                    }
                    @* window.location.href =  '@Url.Action("Pencere", "Home")'
                    document.getElementById("p1").innerHTML = "New text!";
                            *@
                }
            });


        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());
    }

    function handleLocationError(browserHasGeolocation, infoWindow, pos) {
        infoWindow.setPosition(pos);
        infoWindow.setContent(browserHasGeolocation ?
            'Error: Lütfen konum ayarlarýnýzý açýn veya izin verin.' :
            'Error: Tarayýcýnýz bu hizmeti desteklemiyor.');
        infoWindow.open(map);
    }
    // Haritayý oluþturuyoruz.

    // Marker'a týklandýðýnda açýlacak infoWindow'u tanýmlýyoruz.
    var infoWindow = new google.maps.InfoWindow({
        maxWidth: 300
    });

    // Koordinat bilgilerini Controllerdan okuyoruz.
    $.get("/Home/GetSuppliers", function (data) {
        // Okunan json verisini parse et

        var json = JSON.parse(data);
        // Markerlarý oluþtur
        for (var i = 0; i < json.length; i++) {
            var position = {
                lat: parseFloat(json[i].lat.replace(',', '.')),
                lng: parseFloat(json[i].lng.replace(',', '.'))
            };
            var marker = new google.maps.Marker({
                position: position,
                icon: '/Content/themes/assets/images/restaurantx.png',
                animation: google.maps.Animation.DROP,
                map: map,
                title: json[i].name,
            });
            // infoWindow içeriðini replace et
            var cs = contentString;
            cs = cs.replace("#FIRST_HEADING#", json[i].name);
            cs = cs.replace("#IMG_SRC#", json[i].img);
            cs = cs.replace("#DETAIL#", json[i].det);
            cs = cs.replace("#BUTTON#", json[i].id);

            // Marker'ýn click event'ini set et
            google.maps.event.addListener(marker, 'click', (function (marker, cs, infoWindow) {
                return function () {
                    infoWindow.setContent(cs);
                    infoWindow.open(map, this);
                };
            })(marker, cs, infoWindow));
        }
    });
}
