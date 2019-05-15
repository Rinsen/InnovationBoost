(function () {
    'use strict';

    angular
        .module('app')
        .directive('objectStateDirective', objectStateDirective);

    objectStateDirective.$inject = ['$window'];

    function objectStateDirective($window) {
        // Usage:
        //     <ObjectStateDirective></ObjectStateDirective>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'E',
            scope: {
                object: '=',
                icon: 'ok'
            },
            template: '<td><span style="width: 1px;" class="glyphicon glyphicon-' + scope.icon + '" aria-hidden="true"></span></td>'
        };
        return directive;

        function link(scope, element, attrs) {

        }
    }

})();