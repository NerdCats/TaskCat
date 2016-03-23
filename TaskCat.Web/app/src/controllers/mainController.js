(function() {
  'use strict';

  angular
  .module('app')
  .controller('mainController', mainController);

  mainController.$inject = ['$mdSidenav', 'orderService', '$log'];

  /* @ngInject */
  function mainController($mdSidenav, orderService, $log) {
    var vm = this;
    vm.toggleSideNav = toggleSideNav;
    vm.orders = orderService.getSupportedOrders();

    activate();

    function activate() {
      $log.info('Fetching Supported Orders');
      return orderService.getSupportedOrders().then(function(response) {
        $log.info('Fetched '+ response.data.length +' supported orders');
        vm.orders = data;
        return vm.orders;
      }, function(error){
        $log.error('Unable to load customer data: '+ error.message);
        vm.status='Unable to load customer data: '+ error.message;
      });
    }

    function toggleSideNav() {
      $mdSidenav('left').toggle();
    }
  }
})();
