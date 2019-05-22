(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerClientService', IdentityServerClientService);

    IdentityServerClientService.$inject = ['$http'];

    function IdentityServerClientService($http) {
        var service = {
            getClients: getClients,
            saveClient: saveClient,
            getClient: getClient
        };

        return service;

        function getClients() {
            return $http.get('api/Client');
        }

        function saveClient(client) {
            return $http.post('api/Client/Update', client);
        }

        function getClient(clientId) {
            return $http.get('api/Client/' + clientId);
        }
    }
})();