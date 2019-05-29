(function () {
    'use strict';

    angular
        .module('app')
        .factory('IdentityServerClientService', IdentityServerClientService);

    IdentityServerClientService.$inject = ['$http'];

    function IdentityServerClientService($http) {
        var service = {
            createClient: createClient,
            deleteClient: deleteClient,
            getClients: getClients,
            saveClient: saveClient,
            getClient: getClient
        };

        return service;

        function createClient(clientId, clientName, description) {
            var create = { clientId: clientId, clientName: clientName, description: description };
            return $http.post('api/Client/Create', create);
        }

        function getClients() {
            return $http.get('api/Client');
        }

        function saveClient(client) {
            return $http.post('api/Client/Update', client);
        }

        function getClient(clientId) {
            return $http.get('api/Client/' + clientId);
        }

        function deleteClient(clientId) {
            return $http.delete('api/Client/' + clientId);
        }
    }
})();