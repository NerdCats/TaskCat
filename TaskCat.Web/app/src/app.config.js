(function() {
  'use strict';

  angular
  .module('app')
  .config(themeConfig);

  themeConfig.$inject = ['$mdThemingProvider', '$mdIconProvider'];

  function themeConfig($mdThemingProvider, $mdIconProvider) {
    $mdIconProvider.icon("menu", "./contents/svg/menu.svg");
  }

  logConfig.$inject = ['$logProvider', 'appSettings'];
  function logConfig($logProvider, appSettings)
  {
    $logProvider.debugEnabled(appSettings.debugEnabled);
  }
})();
