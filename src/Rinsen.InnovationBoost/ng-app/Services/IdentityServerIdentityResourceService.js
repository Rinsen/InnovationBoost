(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerIdentityResourceService', IdentityServerIdentityResourceService);

    IdentityServerIdentityResourceService.$inject = ['$http'];

    function IdentityServerIdentityResourceService($http) {
        var service = {
            createIdentityResource: createIdentityResource,
            deleteIdentityResource: deleteIdentityResource,
            getIdentityResources: getIdentityResources,
            saveIdentityResource: saveIdentityResource,
            getIdentityResource: getIdentityResource
        };

        return service;

        function createIdentityResource(name, displayName, description) {
            var create = { name: name, displayName: displayName, description: description };
            return $http.post('api/IdentityResource/Create', create);
        }

        function getIdentityResources() {
            return $http.get('api/IdentityResource');
        }

        function saveIdentityResource(identityResource) {
            return $http.post('api/IdentityResource/Update', identityResource);
        }

        function getIdentityResource(id) {
            return $http.get('api/IdentityResource/' + id);
        }

        function deleteIdentityResource(id) {
            return $http.delete('api/IdentityResource/' + id);
        }
    }
})();