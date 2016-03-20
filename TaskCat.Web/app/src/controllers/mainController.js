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

    }

    function toggleSideNav() {
      $mdSidenav('left').toggle();
    }
  }
})();
