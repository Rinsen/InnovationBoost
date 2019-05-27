(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerApiResourceService', IdentityServerApiResourceService);

    IdentityServerApiResourceService.$inject = ['$http'];

    function IdentityServerApiResourceService($http) {
        var service = {
            getApiResources: getApiResources,
            saveApiResource: saveApiResource,
            getApiResource: getApiResource
        };

        return service;

        function getApiResources() {
            return $http.get('api/ApiResource');
        }

        function saveApiResource(client) {
            return $http.post('api/ApiResource/Update', client);
        }

        function getApiResource(clientId) {
            return $http.get('api/ApiResource/' + clientId);
        }
    }
})();