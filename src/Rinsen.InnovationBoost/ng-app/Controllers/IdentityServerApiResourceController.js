﻿(function () {
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
        vm.selectedApiResourceIndex = null;
        vm.selectedTab = 'General';
        vm.saving = false;
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
            scopeShowInDiscoveryDocument: false,
            apiResourceName: '',
            apiResourceDisplayName: '',
            apiResourceDescription: ''
        };

        vm.selectApiResource = function (apiResource) {
            for (var i = 0; i < vm.apiResources.length; i++) {
                if (apiResource.id === vm.apiResources[i].id) {
                    vm.selectedApiResourceIndex = i;
                }
            }

            vm.selectedApiResource = JSON.parse(JSON.stringify(apiResource));
        };

        vm.undoChanges = function () {
            vm.selectedApiResource = vm.apiResources[vm.selectedApiResourceIndex];

            toastr.success("Undo comleted");
        };

        vm.selectPreviousApiResource = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedApiResourceIndex > 0) {
                vm.selectedApiResourceIndex--;
                var apiResource = vm.apiResources[vm.selectedApiResourceIndex];
                vm.selectApiResource(apiResource);
            }
        }; 

        vm.selectNextApiResource = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedApiResourceIndex < vm.apiResources.length - 1) {
                vm.selectedApiResourceIndex++;
                var apiResource = vm.apiResources[vm.selectedApiResourceIndex];
                vm.selectApiResource(apiResource);
            }
        };

        vm.closeEdit = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            vm.selectedApiResource = null;
        };

        vm.saveApiResource = function () {
            vm.saving = true;

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

                                toastr.success("Saved");
                                vm.saving = false;
                            });
                    }
                    else {
                        vm.selectedApiResource = null;

                        toastr.error("Failed");
                        vm.saving = false;
                    }
                });
        };

        vm.createNewApiResource = function () {
            vm.saving = true;

            identityServerApiResourceService.createApiResource(vm.create.apiResourceName, vm.create.apiResourceDisplayName, vm.create.apiResourceDescription)
                .then(function (saveResponse) {
                    if (saveResponse.status === 201) {
                        vm.apiResources.push(saveResponse.data);

                        toastr.success("Saved");
                        vm.saving = false;

                        vm.create.apiResourceName = '';
                        vm.create.apiResourceDisplayName = '';
                        vm.create.apiResourceDescription = '';
                    }
                    else {
                        toastr.error("Failed");
                        vm.saving = false;
                    }
                },
                    function (response) {
                        toastr.error("Failed");
                        vm.saving = false;
                    });
        };

        vm.deleteApiResource = function (apiResource) {
            if (window.confirm("Delete " + apiResource.displayName)) {
                identityServerApiResourceService.deleteApiResource(apiResource.name)
                    .then(function (response) {
                        if (response.status === 200) {
                            for (var i = 0; i < vm.apiResources.length; i++) {
                                if (vm.apiResources[i].name === apiResource.name) {
                                    vm.apiResources.splice(i, 1);
                                }
                            }

                            toastr.info('Deleted');
                        }
                    });
            }
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

        function stopActionBecauseOfUnsavedChanges() {
            if (!angular.equals(vm.selectedApiResource, vm.apiResources[vm.selectedApiResourceIndex])) {
                if (!window.confirm("Unsaved changes to " + vm.apiResources[vm.selectedApiResourceIndex].displayName + " will be lost")) {
                    return true;
                }
            }
            return false;
        }

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
