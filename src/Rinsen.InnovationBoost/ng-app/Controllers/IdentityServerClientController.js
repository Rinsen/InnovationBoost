(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerClientController', IdentityServerClientController);

    IdentityServerClientController.$inject = ['IdentityServerClientService', 'IdentityServerApiResourceService', 'IdentityServerIdentityResourceService'];

    function IdentityServerClientController(identityServerClientService, identityServerApiResourceService, identityServerIdentityResourceService) {
        /* jshint validthis:true */
        var vm = this;
        vm.clients = [];
        vm.types = [];
        vm.apiResources = [];
        vm.identityResources = [];
        vm.selectedClient = null;
        vm.selectedClientIndex = null;
        vm.selectedTab = 'General';
        vm.saving = false;
        vm.create = {
            active: false,
            allowedCorsOrigin: '',
            allowedGrantType: '',
            claimType: '',
            claimValue: '',
            clientSecretType: 'SharedSecret',
            clientSecretDescription: '',
            clientSecretExpiration: '',
            identityProviderRestriction: '',
            postLogoutRedirectUri: '',
            redirectUri: '',
            clientId: '',
            clientName: '',
            clientDescription: ''
        };

        vm.selectClient = function (client) {
            for (var i = 0; i < vm.clients.length; i++) {
                if (client.id === vm.clients[i].id) {
                    vm.selectedClientIndex = i;
                }
            }

            vm.selectedClient = JSON.parse(JSON.stringify(client));

            vm.apiResources.forEach(function (apiResource) {
                apiResource.scopes.forEach(function (scope) {
                    scope.selected = false;
                    scope.previousState = 'notSelected';

                    vm.selectedClient.allowedScopes.forEach(function (allowedScope) {
                        if (scope.name === allowedScope.scope) {
                            scope.selected = true;
                            scope.previousState = allowedScope.state;
                        }
                    });
                });
            });

            vm.identityResources.forEach(function (identityResource) {
                identityResource.selected = false;
                identityResource.previousState = 'notSelected';

                vm.selectedClient.allowedScopes.forEach(function (allowedScope) {
                    if (identityResource.name === allowedScope.scope) {
                        identityResource.selected = true;
                        identityResource.previousState = allowedScope.state;
                    }
                });
            });
        };

        vm.selectPreviousClient = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedClientIndex > 0) {
                vm.selectedClientIndex--;
                var client = vm.clients[vm.selectedClientIndex];
                vm.selectClient(client);
            }
        };

        vm.selectNextClient = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedClientIndex < vm.clients.length - 1) {
                vm.selectedClientIndex++;
                var client = vm.clients[vm.selectedClientIndex];
                vm.selectClient(client);
            }
        };

        vm.closeEdit = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            vm.selectedClient = null;
        };

        vm.undoChanges = function () {
            vm.selectedClient = vm.clients[vm.selectedClientIndex];

            toastr.success("Undo comleted");
        };

        vm.saveClient = function () {
            vm.saving = true;

            vm.apiResources.forEach(function (apiResource) {
                apiResource.scopes.forEach(function (scope) {
                    if (scope.selected && scope.previousState === 'notSelected') { // new selection

                        vm.selectedClient.allowedScopes.push({ scope: scope.name, state: 1 });
                    }
                    else if (!scope.selected && scope.previousState === 0) { // removed selection
                        vm.selectedClient.allowedScopes.forEach(function (allowedScope) {
                            if (scope.name === allowedScope.scope) {
                                allowedScope.state = 3;
                            }
                        });
                    }
                });
            });

            vm.identityResources.forEach(function (identityResource) {
                if (identityResource.selected && identityResource.previousState === 'notSelected') { // new selection

                    vm.selectedClient.allowedScopes.push({ scope: identityResource.name, state: 1 });
                }
                else if (!identityResource.selected && identityResource.previousState === 0) { // removed selection
                    vm.selectedClient.allowedScopes.forEach(function (allowedScope) {
                        if (identityResource.name === allowedScope.scope) {
                            allowedScope.state = 3;
                        }
                    });
                }
            });

            identityServerClientService.saveClient(vm.selectedClient)
                .then(function (saveResponse) {
                    if (saveResponse.status === 200) {
                        identityServerClientService.getClient(vm.selectedClient.clientId).
                            then(function (response) {
                                var client = response.data;

                                addClientToClientsList(client, true);

                                vm.selectClient(response.data);

                                toastr.success("Saved");
                                vm.saving = false;
                            });
                    } else {
                        vm.selectedClient = null;

                        toastr.error("Failed");
                        vm.saving = false;
                    }
                });
        };

        vm.createNewNodeClient = function () {
            vm.saving = true;

            identityServerClientService.createNodeClient(vm.create.clientName, vm.create.clientDescription)
                .then(clientCreated,
                    function (response) {
                        toastr.error("Failed");
                        vm.saving = false;
                    });
        };

        vm.createNewWebsiteClient = function () {
            vm.saving = true;

            identityServerClientService.createWebsiteClient(vm.create.clientName, vm.create.clientDescription)
                .then(clientCreated,
                    function (response) {
                        toastr.error("Failed");
                        vm.saving = false;
                    });
        };

        function clientCreated(saveResponse) {
            if (saveResponse.status === 201) {
                var client = saveResponse.data;
                
                addClientToClientsList(client);

                toastr.success("Saved");
                vm.saving = false;

                vm.create.clientId = '';
                vm.create.clientName = '';
                vm.create.clientDescription = '';
                vm.create.active = false;
                vm.selectClient(client);
            }
            else {
                toastr.error("Failed");
                vm.saving = false;
            }
        }

        vm.deleteClient = function (client) {
            if (window.confirm("Delete " + client.clientName)) {
                identityServerClientService.deleteClient(client.clientId)
                    .then(function (response) {
                        if (response.status === 200) {
                            for (var i = 0; i < vm.clients.length; i++) {
                                if (vm.clients[i].clientId === client.clientId) {
                                    vm.clients.splice(i, 1);
                                }
                            }

                            toastr.info('Deleted');
                        }
                    });
            }
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
            identityServerClientService.getRandomString(40)
                .then(function (response) {
                    vm.selectedClient.clientSecrets.push({ type: vm.create.clientSecretType, value: response.data, description: vm.create.clientSecretDescription, expiration: vm.create.clientSecretExpiration, state: 1 });

                    vm.create.clientSecretType = 'SharedSecret';
                    vm.create.clientSecretDescription = '';
                    vm.create.clientSecretExpiration = '';
                });


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

        function stopActionBecauseOfUnsavedChanges() {
            if (!angular.equals(vm.selectedClient, vm.clients[vm.selectedClientIndex])) {
                if (!window.confirm("Unsaved changes to " + vm.clients[vm.selectedClientIndex].clientName + " will be lost")) {
                    return true;
                }
            }
            return false;
        }

        function addClientToClientsList(client, updated) {
            client.clientTypeName = '';

            if (client.clientTypeId !== undefined) {
                vm.types.forEach(function (type) {
                    if (type.id === client.clientTypeId) {
                        client.clientTypeName = type.name;
                    }
                });
            }

            if (updated !== undefined && updated !== false) {
                for (var i = 0; i < vm.clients.length; i++) {
                    if (vm.clients[i].clientId === vm.selectedClient.clientId) {
                        vm.clients[i] = response.data;
                    }
                }
            }
            else {
                vm.clients.push(client);
            }
        }

        activate();

        function activate() {
            // All of these could be fetched in paralell as long as the last one is defered until the first three is done
            identityServerClientService.getClientTypes().then(function (typeResponse) {
                typeResponse.data.forEach(function (type) {
                    vm.types.push(type);
                });

                identityServerApiResourceService.getApiResources().then(function (apiResourceResponse) {
                    apiResourceResponse.data.forEach(function (apiResource) {
                        vm.apiResources.push(apiResource);
                    });

                    identityServerIdentityResourceService.getIdentityResources().then(function (identityResourceResponse) {
                        identityResourceResponse.data.forEach(function (identityResource) {
                            vm.identityResources.push(identityResource);
                        });

                        identityServerClientService.getClients().then(function (clientResponse) {
                            vm.clients.length = 0;

                            clientResponse.data.forEach(function (client) {
                                addClientToClientsList(client);
                            });
                        });
                    });
                });
            });
        }
    }
})();


