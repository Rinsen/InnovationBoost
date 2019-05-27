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
        vm.selectedTab = 'General';
        vm.create = {
            claimType: '',
            claimValue: '',
            propertyKey: '',
            propertyValue: ''
        };

        vm.selectIdentityResource = function (identityResource) {
            vm.selectedIdentityResource = JSON.parse(JSON.stringify(identityResource));
        };

        vm.undoChanges = function () {
            for (var i = 0; i < vm.identityResources.length; i++) {
                if (vm.identityResources[i].name === vm.selectedIdentityResource.name) {
                    vm.selectedIdentityResource = vm.identityResources[i];
                }
            }
        };

        vm.saveIdentityResource = function () {
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
                            });
                    }
                    else {
                        vm.selectedIdentityResource = null;
                    }
                    
                });
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
