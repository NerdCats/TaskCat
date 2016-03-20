(function() {
  'use strict';

  angular
    .module('app')
    .config(themeConfig);

  themeConfig.$inject = ['$mdThemingProvider', '$mdIconProvider'];

  function themeConfig($mdThemingProvider, $mdIconProvider) {
    $mdIconProvider.icon("menu", "./contents/svg/menu.svg");
  }
})();
