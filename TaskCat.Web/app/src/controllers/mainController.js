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

    activate();

    function activate() {

    }

    function toggleSideNav() {
      $mdSidenav('left').toggle();
    }
  }
})();
