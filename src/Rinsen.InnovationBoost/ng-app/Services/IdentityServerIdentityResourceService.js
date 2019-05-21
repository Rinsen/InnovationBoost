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
            return $http.get('api/IdentityServerIdentityResource');
        }

        function saveIdentityResource(client) {
            return $http.post('api/IdentityServerIdentityResource/Update', client);
        }

        function getIdentityResource(clientId) {
            return $http.get('api/IdentityServerIdentityResource/' + clientId);
        }
    }
})();