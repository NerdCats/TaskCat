(function() {
    'use strict';

    angular
        .module('app')
        .controller('deliveryOrderController', deliveryOrderController);

    deliveryOrderController.$inject = ['$rootScope', '$log'];

    /* @ngInject */
    function deliveryOrderController($rootScope, $log) {
      /* jshint validthis:true */
        var vm = this;

        activate();

        function activate() {
          $log.debug('Broadcasting MENU_TITLE, change');
          $rootScope.$broadcast('MENU_TITLE', 'New Delivery');
        }
    }
})();
