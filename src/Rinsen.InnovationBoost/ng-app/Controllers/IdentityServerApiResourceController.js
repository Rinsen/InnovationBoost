(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerApiResourceController', IdentityServerApiResourceController);

    IdentityServerApiResourceController.$inject = ['$location'];

    function IdentityServerApiResourceController($location) {
        /* jshint validthis:true */
        var vm = this;

        activate();

        function activate() { }
    }
})();
