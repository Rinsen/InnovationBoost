(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerIdentityResourceController', IdentityServerIdentityResourceController);

    IdentityServerIdentityResourceController.$inject = ['IdentityServerIdentityResourceService'];

    function IdentityServerIdentityResourceController(identityServerIdentityResourceService) {
        /* jshint validthis:true */
        var vm = this;
        vm.identityResources = [];
        vm.selectedIdentityResource = null;
        vm.selectedIdentityResourceIndex = null;
        vm.selectedTab = 'General';
        vm.saving = false;
        vm.create = {
            claimType: '',
            claimValue: '',
            propertyKey: '',
            propertyValue: '',
            identityResourceName: '',
            identityResourceDisplayName: '',
            identityResourceDescription: ''
        };

        vm.selectIdentityResource = function (identityResource) {
            for (var i = 0; i < vm.identityResources.length; i++) {
                if (identityResource.id === vm.identityResources[i].id) {
                    vm.selectedIdentityResourceIndex = i;
                }
            }

            vm.selectedIdentityResource = JSON.parse(JSON.stringify(identityResource));
        };

        vm.undoChanges = function () {
            vm.selectedIdentityResource = vm.identityResources[vm.selectedIdentityResourceIndex];

            toastr.success("Undo comleted");
        };

        vm.selectPreviousIdentityResource = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedIdentityResourceIndex > 0) {
                vm.selectedIdentityResourceIndex--;
                var identityResource = vm.identityResources[vm.selectedIdentityResourceIndex];
                vm.selectIdentityResource(identityResource);
            }
        };

        vm.selectNextIdentityResource = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            if (vm.selectedIdentityResourceIndex < vm.identityResources.length - 1) {
                vm.selectedIdentityResourceIndex++;
                var identityResource = vm.identityResources[vm.selectedIdentityResourceIndex];
                vm.selectIdentityResource(identityResource);
            }
        };

        vm.closeEdit = function () {
            if (stopActionBecauseOfUnsavedChanges()) {
                return;
            }

            vm.selectedIdentityResource = null;
        };

        vm.saveIdentityResource = function () {
            vm.saving = true;

            identityServerIdentityResourceService.saveIdentityResource(vm.selectedIdentityResource)
                .then(function (saveResponse) {
                    if (saveResponse.status === 200) {
                        identityServerIdentityResourceService.getIdentityResource(vm.selectedIdentityResource.name).
                            then(function (response) {
                                for (var i = 0; i < vm.identityResources.length; i++) {
                                    if (vm.identityResources[i].name === vm.selectedIdentityResource.name) {
                                        vm.identityResources[i] = response.data;
                                    }
                                }

                                vm.selectIdentityResource(response.data);

                                toastr.success("Saved");
                                vm.saving = false;
                            });
                    }
                    else {
                        vm.selectedIdentityResource = null;

                        toastr.error("Failed");
                        vm.saving = false;
                    }
                    
                });
        };

        vm.createNewIdentityResource = function () {
            vm.saving = true;

            identityServerIdentityResourceService.createIdentityResource(vm.create.identityResourceName, vm.create.identityResourceDisplayName, vm.create.identityResourceDescription)
                .then(function (saveResponse) {
                    if (saveResponse.status === 201) {
                        vm.identityResources.push(saveResponse.data);

                        toastr.success("Saved");
                        vm.saving = false;

                        vm.create.identityResourceName = '';
                        vm.create.identityResourceDisplayName = '';
                        vm.create.identityResourceDescription = '';
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

        vm.deleteIdentityResource = function (identityResource) {
            if (window.confirm("Delete " + identityResource.displayName)) {
                identityServerIdentityResourceService.deleteIdentityResource(identityResource.name)
                    .then(function (response) {
                        if (response.status === 200) {
                            for (var i = 0; i < vm.identityResources.length; i++) {
                                if (vm.identityResources[i].name === identityResource.name) {
                                    vm.identityResources.splice(i, 1);
                                }
                            }

                            toastr.info('Deleted');
                        }
                    });
            }
        };


        
        vm.createNewClaim = function () {
            vm.selectedIdentityResource.claims.push({ type: vm.create.claimType, value: vm.create.claimValue, state: 1 });

            vm.create.claimType = '';
            vm.create.claimValue = '';
        };

        vm.createNewProperty = function () {
            vm.selectedIdentityResource.properties.push({ key: vm.create.propertyKey, value: vm.create.propertyValue, state: 1 });

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
            if (!angular.equals(vm.selectedIdentityResource, vm.identityResources[vm.selectedIdentityResourceIndex])) {
                if (!window.confirm("Unsaved changes to " + vm.identityResources[vm.selectedIdentityResourceIndex].displayName + " will be lost")) {
                    return true;
                }
            }
            return false;
        }

        activate();

        function activate() {
            identityServerIdentityResourceService.getIdentityResources().then(function (response) {
                vm.identityResources.length = 0;

                response.data.forEach(function (identityResource) {
                    vm.identityResources.push(identityResource);
                });

            });
        }
    }
})();
