(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerApiResourceService', IdentityServerApiResourceService);

    IdentityServerApiResourceService.$inject = ['$http'];

    function IdentityServerApiResourceService($http) {
        var service = {
            getApiResources: getApiResources,
            saveApiResources: saveApiResources,
            getApiResource: getApiResource
        };

        return service;

        function getApiResources() {
            return $http.get('api/IdentityServerApiResource');
        }

        function saveApiResources(client) {
            return $http.post('api/IdentityServerApiResource/Update', client);
        }

        function getApiResource(clientId) {
            return $http.get('api/IdentityServerApiResource/' + clientId);
        }
    }
})();