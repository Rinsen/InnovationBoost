(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerClientController', IdentityServerClientController);

    IdentityServerClientController.$inject = ['IdentityServerClientService'];

    function IdentityServerClientController(identityServerClientService) {
        /* jshint validthis:true */
        var vm = this;
        vm.clients = [];
        vm.selectedClient = null;
        vm.selectedTab = 'General';
        vm.create = {
            allowedScope: '',
            allowedCorsOrigin: '',
            allowedGrantType: '',
            claimType: '',
            claimValue: '',
            clientSecretType: '',
            clientSecretValue: '',
            clientSecretDescription: '',
            clientSecretExpiration: '',
            identityProviderRestriction: '',
            postLogoutRedirectUri: '',
            redirectUri: ''
        };

        vm.selectClient = function (client) {
            vm.selectedClient = JSON.parse(JSON.stringify(client));
        };

        vm.undoChanges = function () {
            for (var i = 0; i < vm.clients.length; i++) {
                if (vm.clients[i].clientId === vm.selectedClient.clientId) {
                    vm.selectedClient = vm.clients[i];
                }
            }
        };

        vm.saveClient = function () {
            identityServerClientService.saveClient(vm.selectedClient)
                .then(function (saveResponse) {
                    if (saveResponse.status === 200) {
                        identityServerClientService.getClient(vm.selectedClient.clientId).
                            then(function (response) {
                                for (var i = 0; i < vm.clients.length; i++) {
                                    if (vm.clients[i].clientId === vm.selectedClient.clientId) {
                                        vm.clients[i] = response.data;
                                    }
                                }

                                vm.selectClient(response.data);
                            });
                    } else {
                        vm.selectedClient = null;
                    }
                });
        };

        vm.createNewAllowedCorsOrigin = function () {
            vm.selectedClient.allowedCorsOrigins.push({ origin: vm.create.allowedCorsOrigin, state: 1 });

            vm.create.allowedCorsOrigin = '';
        };

        vm.createNewAllowedScope = function () {
            vm.selectedClient.allowedScopes.push({ scope: vm.create.allowedScope, state: 1 });

            vm.create.allowedScope = '';
        };

        vm.createNewAllowedGrantType = function () {
            vm.selectedClient.allowedGrantTypes.push({ grantType: vm.create.allowedGrantType, state: 1 });

            vm.create.allowedGrantType = '';
        };

        vm.createNewClaim = function () {
            vm.selectedClient.claims.push({ type: vm.create.claimType, value: vm.create.claimValue, state: 1 });

            vm.create.claimType = '';
            vm.create.claimValue = '';
        };

        vm.createNewClientSecret = function () {
            vm.selectedClient.clientSecrets.push({ type: vm.create.clientSecretType, value: vm.create.clientSecretValue, description: vm.create.clientSecretDescription, expiration: vm.create.clientSecretExpiration, state: 1 });

            vm.create.clientSecretType = '';
            vm.create.clientSecretValue = '';
            vm.create.clientSecretDescription = '';
            vm.create.clientSecretExpiration = '';
        };

        vm.createNewIdentityProviderRestriction = function () {
            vm.selectedClient.identityProviderRestrictions.push({ provider: vm.create.identityProviderRestriction, state: 1 });

            vm.create.identityProviderRestriction = '';
        };

        vm.createNewPostLogoutRedirectUri = function () {
            vm.selectedClient.postLogoutRedirectUris.push({ postLogoutRedirectUri: vm.create.postLogoutRedirectUri, state: 1 });

            vm.create.postLogoutRedirectUri = '';
        };

        vm.createNewRedirectUri = function () {
            vm.selectedClient.redirectUris.push({ redirectUri: vm.create.redirectUri, state: 1 });

            vm.create.redirectUri = '';
        };

        vm.setModified = function (object) {
            if (object.state === 0) {
                object.state = 2;
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
