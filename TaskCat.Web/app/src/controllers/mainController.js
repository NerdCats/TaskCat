(function() {
  'use strict';

  angular
  .module('app')
  .controller('mainController', mainController);

  mainController.$inject = ['$mdSidenav', 'orderService'];

  /* @ngInject */
  function mainController($mdSidenav, orderService) {
    var vm = this;
    vm.toggleSideNav = toggleSideNav;
    vm.orders = orderService.getSupportedOrders();

    activate();

    function activate() {
      return orderService.getSupportedOrders().then(function(data) {
        vm.orders = data;
        return vm.orders;
      }, function(error){
        vm.status='Unable to load customer data: '+ error.message;
      });
    }

    function toggleSideNav() {
      $mdSidenav('left').toggle();
    }
  }
})();
