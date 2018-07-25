(function () {
    'use strict';

    angular
        .module('app')
        .factory('ExternalApplicationsService', ExternalApplicationsService);

    ExternalApplicationsService.$inject = ['$http'];

    function ExternalApplicationsService($http) {
        var service = {
            getAll: getAll,
            create: create,
            update: update
        };

        return service;

        function getAll() {
            return $http.get('/webapi/ExternalApplications/GetAll');
        }

        function create(data) {
            return $http.post('/webapi/ExternalApplications/Create', data);
        }

        function update(data) {
            return $http.post('/webapi/ExternalApplications/Update', data);
        }


    }
})();