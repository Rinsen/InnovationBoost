﻿(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoggerController', LoggerController);

    LoggerController.$inject = ['$location', 'LogSelectionOptionsService'];

    function LoggerController($location, logSelectionOptionsService) {
        /* jshint validthis:true */
        var vm = this;
        vm.show = show;
        vm.dataLoading = false;
        vm.logs = [];
        vm.itemsByPage = 100;
        vm.pages = 0;

        function show() {
            vm.dataLoading = true;
            vm.logs.length = 0;

            var searchModel = {
                from: vm.options.from,
                to: vm.options.to,
                logLevels: [],
                logEnvironments: [],
                logApplications: [],
                logSources: []
            };

            vm.options.logLevels.forEach(function (logLevel) {
                if (logLevel.selected) {
                    searchModel.logLevels.push(logLevel.level);
                }
            });

            vm.options.logEnvironments.forEach(function (logEnvironment) {
                if (logEnvironment.selected) {
                    searchModel.logEnvironments.push(logEnvironment.id);
                }
            });

            vm.options.logApplications.forEach(function (logApplication) {
                if (logApplication.selected) {
                    searchModel.logApplications.push(logApplication.id);
                }
            });

            vm.options.logSources.forEach(function (logSource) {
                if (logSource.selected) {
                    searchModel.logSources.push(logSource.id);
                }
            });

            logSelectionOptionsService.getLogs(searchModel).then(function (response) {
                response.data.forEach(function (log) {
                    log.expanded = false;
                    vm.logs.push(log);
                });

                vm.pages = Math.ceil(vm.logs.length / vm.itemsByPage);

                vm.dataLoading = false;
            });
        }
        

        activate();

        function activate() {
            vm.options = logSelectionOptionsService.getOptions();
        }
    }
})();
