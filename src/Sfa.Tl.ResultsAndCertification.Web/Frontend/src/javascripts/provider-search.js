﻿$(document).ready(function () {

    var element = document.querySelector('#search');

    var providerNames = new Array();
    var providerIds = new Object();

    accessibleAutocomplete.enhanceSelectElement({
        defaultValue: '',
        autoSelect: true,
        selectElement: element,
        minLength: 3,
        name: "Search",
        source:
            function (query, process) {
                providerNames = [];
                providerIds = new Object();
                $.ajax({
                    url: "search-provider/" + query,
                    type: "get",
                    contentType: "json",
                    success: function (data) {
                        $.each(data, function (idx, provider) {
                            providerNames.push(provider.displayName);
                            providerIds[provider.displayName] = provider.id;
                        });

                        process(providerNames);
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            },
        onConfirm: (val) => {
            if (val != null) {
                var id = providerIds[val];
                $('#SelectedProviderId').val(id);
                console.log('you choose: ' + val + ' id is: ' + id);
            }
        }
    });
});