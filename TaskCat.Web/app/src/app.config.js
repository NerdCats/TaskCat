(function() {
  'use strict';

  angular
    .module('app')
    .config(themeConfig)
    .config(logConfig)
    .config(httpProvidercConfig);

  themeConfig.$inject = ['$mdThemingProvider', '$mdIconProvider'];

  function themeConfig($mdThemingProvider, $mdIconProvider) {
    $mdIconProvider.icon("menu", "./contents/svg/menu.svg");
    $mdThemingProvider.theme('default')
    .primaryPalette('deep-purple');
  }

  logConfig.$inject = ['$logProvider', 'appSettings'];

  function logConfig($logProvider, appSettings) {
    $logProvider.debugEnabled(appSettings.debugEnabled);
  }

  httpProvidercConfig.$inject = ['$httpProvider'];

  function httpProvidercConfig($httpProvider) {
    $httpProvider.interceptors.push('xhrReqInterceptor');
  }

})();
