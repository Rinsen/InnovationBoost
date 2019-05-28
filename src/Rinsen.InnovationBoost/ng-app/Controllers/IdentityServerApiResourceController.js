(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerApiResourceController', IdentityServerApiResourceController);

    IdentityServerApiResourceController.$inject = ['IdentityServerApiResourceService'];

    function IdentityServerApiResourceController(identityServerApiResourceService) {
        /* jshint validthis:true */
        var vm = this;
        vm.apiResources = [];
        vm.selectedApiResource = null;
        vm.selectedTab = 'General';
        vm.create = {
            apiSecretDescription: '',
            apiSecretExpiration: '',
            apiSecretType: '',
            apiSecretValue: '',
            claimType: '',
            claimValue: '',
            propertyKey: '',
            propertyValue: '',
            scopeName: '',
            scopeDisplayName: '',
            scopeDescription: '',
            scopeRequired: false,
            scopeEmphasize: false,
            scopeShowInDiscoveryDocument: false
        };

        vm.selectApiResource = function (apiResource) {
            vm.selectedApiResource = JSON.parse(JSON.stringify(apiResource));
        };

        vm.undoChanges = function () {
            for (var i = 0; i < vm.apiResources.length; i++) {
                if (vm.apiResources[i].name === vm.selectedApiResource.name) {
                    vm.selectedApiResource = vm.apiResources[i];
                }
            }
        };

        vm.saveApiResource = function () {
            identityServerApiResourceService.saveApiResource(vm.selectedApiResource)
                .then(function (saveResponse) {
                    if (saveResponse.status === 200) {
                        identityServerApiResourceService.getApiResource(vm.selectedApiResource.name).
                            then(function (response) {
                                for (var i = 0; i < vm.apiResources.length; i++) {
                                    if (vm.apiResources[i].name === vm.selectedApiResource.name) {
                                        vm.apiResources[i] = response.data;
                                    }
                                }

                                vm.selectApiResource(response.data);
                            });
                    }
                    else {
                        vm.selectedApiResource = null;
                    }
                });
        };

        vm.createNewApiSecret = function () {
            vm.selectedApiResource.apiSecrets.push({ description: vm.create.apiSecretDescription, expiration: vm.create.apiSecretExpiration, type: vm.create.apiSecretType, value: vm.create.apiSecretValue, state: 1 });

            vm.create.apiSecretDescription = '';
            vm.create.apiSecretExpiration = '';
            vm.create.apiSecretType = '';
            vm.create.apiSecretValue = '';
        };

        vm.createNewClaim = function () {
            vm.selectedApiResource.claims.push({ type: vm.create.claimType, value: vm.create.claimValue, state: 1 });

            vm.create.claimType = '';
            vm.create.claimValue = '';
        };

        vm.createNewScope = function () {
            vm.selectedApiResource.scopes.push({ name: vm.create.scopeName, displayName: vm.create.scopeDisplayName, description: vm.create.scopeDescription, emphasize: vm.create.scopeEmphasize, required: vm.create.scopeRequired, showInDiscoveryDocument: vm.create.scopeShowInDiscoveryDocument, state: 1 });

            vm.create.scopeName = '';
            vm.create.scopeDisplayName = '';
            vm.create.scopeDescription = '';
            vm.create.scopeRequired = false;
            vm.create.scopeEmphasize = false;
            vm.create.scopeShowInDiscoveryDocument = false;
        };

        vm.createNewProperty = function () {
            vm.selectedApiResource.properties.push({ key: vm.create.propertyKey, value: vm.create.propertyValue, state: 1 });

            vm.create.propertyKey = '';
            vm.create.propertyValue = '';
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
            identityServerApiResourceService.getApiResources().then(function (response) {
                vm.apiResources.length = 0;

                response.data.forEach(function (apiResource) {
                    vm.apiResources.push(apiResource);
                });

            });
        }
    }
})();
