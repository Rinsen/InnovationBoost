(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerController', IdentityServerController);

    IdentityServerController.$inject = ['$location', 'IdentityServerClientService'];

    function IdentityServerController($location, identityServerClientService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'IdentityServerController';
        vm.clients = [];
        vm.selectedClient = null;
        vm.selectedTab = 'General';

        vm.selectClient = function (client) {
            angular.copy(client, vm.selectedClient);
        };

        vm.saveClient = function () {
            identityServerClientService.saveClient(vm.selectedClient);

            vm.selectedClient = null;
        };

        activate();

        function activate() {
            identityServerClientService.getClients().then(function (response) {
                vm.clients.length = 0;

                response.data.forEach(function (client) {
                    vm.clients.push(client);
                });

            });

        }
    }
})();
