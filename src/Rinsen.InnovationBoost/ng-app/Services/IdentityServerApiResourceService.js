(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerApiResourceService', IdentityServerApiResourceService);

    IdentityServerApiResourceService.$inject = ['$http'];

    function IdentityServerApiResourceService($http) {
        var service = {
            createApiResource: createApiResource,
            deleteApiResource: deleteApiResource,
            getApiResources: getApiResources,
            saveApiResource: saveApiResource,
            getApiResource: getApiResource
        };

        return service;

        function createApiResource(name, displayName, description) {
            var create = { name: name, displayName: displayName, description: description };
            return $http.post('api/ApiResource/Create', create);
        }

        function getApiResources() {
            return $http.get('api/ApiResource');
        }

        function saveApiResource(client) {
            return $http.post('api/ApiResource/Update', client);
        }

        function getApiResource(clientId) {
            return $http.get('api/ApiResource/' + clientId);
        }

        function deleteApiResource(clientId) {
            return $http.delete('api/ApiResource/' + clientId);
        }
    }
})();