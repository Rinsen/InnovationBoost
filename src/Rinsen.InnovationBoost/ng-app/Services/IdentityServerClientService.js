(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerClientService', IdentityServerClientService);

    IdentityServerClientService.$inject = ['$http'];

    function IdentityServerClientService($http) {
        var service = {
            getClients: getClients,
            saveClient: saveClient
        };

        return service;

        function getClients() {
            return $http.get('api/IdentityServerClient');
        }

        function saveClient(client) {
            return $http.post('api/IdentityServerClient/Update', client);
        }
    }
})();