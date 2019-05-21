(function () {
    'use strict';

    angular
        .module('app')
        .controller('IdentityServerIdentityResourceController', IdentityServerIdentityResourceController);

    IdentityServerIdentityResourceController.$inject = ['$location'];

    function IdentityServerIdentityResourceController($location) {
        /* jshint validthis:true */
        var vm = this;

        activate();

        function activate() { }
    }
})();
