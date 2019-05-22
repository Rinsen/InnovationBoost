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
            example: ''
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
                .then(identityServerApiResourceService.getApiResource(vm.selectedApiResource.name).
                    then(function (response) {
                        for (var i = 0; i < vm.apiResources.length; i++) {
                            if (vm.apiResources[i].name === vm.selectedApiResource.name) {
                                vm.apiResources[i] = response.data;
                            }
                        }

                        vm.selectApiResource(response.data);
                    }));
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
