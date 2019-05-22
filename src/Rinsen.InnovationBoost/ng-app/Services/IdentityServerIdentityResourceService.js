(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerIdentityResourceService', IdentityServerIdentityResourceService);

    IdentityServerIdentityResourceService.$inject = ['$http'];

    function IdentityServerIdentityResourceService($http) {
        var service = {
            getIdentityResources: getIdentityResources,
            saveIdentityResource: saveIdentityResource,
            getIdentityResource: getIdentityResource
        };

        return service;

        function getIdentityResources() {
            return $http.get('api/IdentityResource');
        }

        function saveIdentityResource(client) {
            return $http.post('api/IdentityResource/Update', client);
        }

        function getIdentityResource(clientId) {
            return $http.get('api/IdentityResource/' + clientId);
        }
    }
})();