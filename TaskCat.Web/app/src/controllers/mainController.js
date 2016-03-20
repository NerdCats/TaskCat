(function() {
  'use strict';

  angular
    .module('app')
    .controller('mainController', mainController);

  mainController.$inject = ['$mdSidenav'];

  /* @ngInject */
  function mainController($mdSidenav) {
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
