(function () {
    var mapEl = document.getElementById('checkout-map');
    if (!mapEl) return;

    if (typeof L === 'undefined') {
        mapEl.innerHTML =
            '<div class="alert alert-warning h-100 d-flex align-items-center justify-content-center mb-0">Map unavailable. Please enter the address manually.</div>';
        return;
    }

    var initialLat = parseFloat(mapEl.dataset.lat || '55.7558');
    var initialLng = parseFloat(mapEl.dataset.lng || '37.6173');

    L.Icon.Default.mergeOptions({
        iconUrl: '/lib/leaflet/images/marker-icon.png',
        iconRetinaUrl: '/lib/leaflet/images/marker-icon-2x.png',
        shadowUrl: '/lib/leaflet/images/marker-shadow.png'
    });

    var map = L.map('checkout-map').setView([initialLat, initialLng], 10);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    var marker = L.marker([initialLat, initialLng], { draggable: true }).addTo(map);
    marker.on('dragend', function () {
        var p = marker.getLatLng();
        setCoords(p.lat, p.lng);
    });

    function setCoords(lat, lng) {
        var latEl = document.getElementById('Latitude');
        var lngEl = document.getElementById('Longitude');
        if (latEl) latEl.value = lat.toFixed(6);
        if (lngEl) lngEl.value = lng.toFixed(6);
    }

    function setMarker(lat, lng) {
        marker.setLatLng([lat, lng]);
        map.panTo([lat, lng]);
        setCoords(lat, lng);
    }

    var geocodeInFlight = false;
    var lastGeocode = 0;

    function nominatim(url, callback) {
        var now = Date.now();
        if (geocodeInFlight || now - lastGeocode < 1100) return;
        geocodeInFlight = true;
        lastGeocode = now;
        fetch(url)
            .then(function (r) { return r.json(); })
            .then(function (data) { callback(data); })
            .catch(function () { })
            .finally(function () { geocodeInFlight = false; });
    }

    function setField(id, value) {
        var el = document.getElementById(id);
        if (el) el.value = value || '';
    }

    function fillAddress(addr) {
        if (!addr) return;
        setField('Order_Country', addr.country || '');
        setField('Order_City', addr.city || addr.town || addr.village || addr.municipality || '');
        setField('Order_Street', addr.road || addr.pedestrian || addr.cycleway || '');
        setField('Order_Building', addr.house_number || '');
        setField('Order_Apartment', addr.apartment || addr.unit || '');
        setField('Order_PostalCode', addr.postcode || '');
    }

    function reverseGeocode(lat, lng) {
        nominatim('https://nominatim.openstreetmap.org/reverse?format=jsonv2&accept-language=en&lat='
            + lat + '&lon=' + lng, function (data) {
                if (data && data.address) fillAddress(data.address);
            });
    }

    map.on('click', function (e) {
        setMarker(e.latlng.lat, e.latlng.lng);
        reverseGeocode(e.latlng.lat, e.latlng.lng);
    });

    var searchInput = document.getElementById('address-search');
    var results = document.getElementById('address-results');
    var debounceTimer = null;

    searchInput.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        var q = searchInput.value.trim();
        if (q.length < 3) { results.style.display = 'none'; results.innerHTML = ''; return; }
        debounceTimer = setTimeout(function () {
            nominatim('https://nominatim.openstreetmap.org/search?format=jsonv2&limit=5&accept-language=en&q='
                + encodeURIComponent(q), function (data) {
                    results.innerHTML = '';
                    if (!data || !data.length) {
                        results.style.display = 'none';
                        return;
                    }
                    data.forEach(function (item) {
                        var li = document.createElement('li');
                        li.className = 'list-group-item list-group-item-action';
                        li.textContent = item.display_name;
                        li.addEventListener('click', function () {
                            setMarker(parseFloat(item.lat), parseFloat(item.lon));
                            if (item.address) fillAddress(item.address);
                            results.style.display = 'none';
                            searchInput.value = item.display_name;
                        });
                        results.appendChild(li);
                    });
                    results.style.display = 'block';
                });
        }, 600);
    });

    document.addEventListener('click', function (e) {
        if (!(e.target && e.target.closest) || !e.target.closest('#address-results')) {
            results.style.display = 'none';
        }
    });
})();
