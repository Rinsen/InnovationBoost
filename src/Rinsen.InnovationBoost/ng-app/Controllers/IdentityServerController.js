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
        vm.create = {
            allowedScope: ''
        };

        vm.selectClient = function (client) {
            vm.selectedClient = JSON.parse(JSON.stringify(client));
        };

        vm.saveClient = function () {
            identityServerClientService.saveClient(vm.selectedClient);

            vm.selectedClient = null;
        };

        vm.createNewAllowedScope = function () {
            vm.selectedClient.allowedScopes.push({ scope: vm.create.allowedScope, state: 1 });

            vm.create.allowedScope = '';
        };

        vm.setModified = function (object) {
            if (object.state === 0) {
                object.state = 1;
            }
        };

        vm.toggleDelete = function (object) {
            if (object.state === 3) {
                object.state = 2;

                return;
            }

            if (object.state === 1) {
                object.state = 4;

                return;
            }

            if (object.state === 4) {
                object.state = 1;

                return;
            }

            object.state = 3;
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
