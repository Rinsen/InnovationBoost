(function () {
    'use strict';

    angular
        .module('app')
        .controller('ExternalApplicationsController', ExternalApplicationsController);

    ExternalApplicationsController.$inject = ['$location', 'ExternalApplicationsService'];

    function ExternalApplicationsController($location, externalApplicationsService) {
        /* jshint validthis:true */
        var vm = this;
        vm.loading = true;
        vm.stateChange = false;
        vm.externalApplications = [];
        vm.name = '';
        vm.active = false;
        vm.activeUntil = new Date();
        vm.createFailed = false;
        vm.predicate = 'name';
        vm.reverse = false;

        vm.mode = {
            create: false,
            read: true,
            activateCreate: function () {
                vm.mode.create = true;
                vm.mode.read = false;
            },
            activateRead: function () {
                vm.mode.create = false;
                vm.mode.read = true;
            }
        };

        function activate() {

            externalApplicationsService.getAll()
                .then(function (response) {
                    response.data.externalApplications.forEach(function (externalApplication) {
                        vm.externalApplications.push(externalApplication);
                    });

                    vm.loading = false;
                });
        }

        vm.startCreateNew = function () {
            if (vm.activeUntil === '') {
                vm.activeUntil = new Date();
            }

            vm.mode.activateCreate();
        };

        vm.createNew = function () {
            vm.createFailed = false;
            vm.loading = true;
            var externalApplicationToCreate = {
                name: vm.name,
                active: vm.active,
                activeUntil: vm.activeUntil
            };

            externalApplicationsService.create(externalApplicationToCreate)
                .then(function (response) {
                    response.data.externalApplications.forEach(function (externalApplication) {
                        vm.externalApplications.unshift(externalApplication);
                    });

                    vm.mode.activateRead();
                    vm.createFailed = false;
                    vm.loading = false;
                    vm.name = '';
                    vm.active = false;
                    vm.activeUntil = new Date();
                },
                    function () {
                        vm.loading = false;
                        vm.createFailed = true;
                    });
        };

        vm.cancelNew = function () {
            vm.mode.activateRead();
            vm.createFailed = false;
        };

        vm.order = function (predicate) {
            vm.reverse = vm.predicate === predicate ? !vm.reverse : false;
            vm.predicate = predicate;
        };

        vm.activate = function (id) {
            vm.externalApplications.forEach(function (externalApplication) {
                if (externalApplication.externalApplicationId === id) {
                    if (externalApplication.active === true) {
                        return;
                    }

                    vm.stateChange = true;
                    externalApplication.active = true;
                    externalApplicationsService.update(externalApplication)
                        .then(function () {
                            vm.stateChange = false;
                        },
                            function () {
                                vm.stateChange = false;
                                externalApplication.activate = false;
                                alert('Failed to update active state');
                            });
                }

            });
        };

        vm.deactivate = function (id) {
            vm.externalApplications.forEach(function (externalApplication) {
                if (externalApplication.externalApplicationId === id) {
                    if (externalApplication.active === false) {
                        return;
                    }
                    vm.stateChange = true;
                    externalApplication.active = false;
                    externalApplicationsService.update(externalApplication)
                        .then(function () {
                            vm.stateChange = false;
                        },
                            function () {
                                vm.stateChange = false;
                                externalApplication.activate = true;
                                alert('Failed to update active state');
                            });
                }

            });
        };

        activate();
    }
})();
