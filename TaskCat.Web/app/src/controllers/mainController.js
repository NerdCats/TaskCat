(function() {
  'use strict';

  angular
    .module('app')
    .controller('mainController', mainController);

  mainController.$inject = ['$mdSidenav', 'orderService', '$log', '$scope'];

  /* @ngInject */
  function mainController($mdSidenav, orderService, $log, $scope) {
    /* jshint validthis:true */
    var vm = this;
    vm.toggleSideNav = toggleSideNav;
    vm.isLeftSideNavOpen = true;

    activate();

    function activate() {
      vm.MainMenuTitle = "GO! Fetch";
      $scope.$on('MENU_TITLE', function(event, data) {
        $log.debug("MENU_TITLE broadcast change came as " + data);
        vm.MainMenuTitle = data;
      });
    }

    function toggleSideNav() {
      vm.isLeftSideNavOpen = !vm.isLeftSideNavOpen;
    }
  }
})();
