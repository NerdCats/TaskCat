(function() {
  'use strict';

  angular
    .module('app')
    .factory('xhrReqInterceptor', xhrReqInterceptor);

  xhrReqInterceptor.$inject = ['$rootScope'];

  /* @ngInject */
  function xhrReqInterceptor($rootScope) {
    var requestCount = 0;

    var interceptor = {
      'request': startRequest,
      'requestError': endRequest,
      'response': endRequest,
      'responseError': endRequest
    };

    return interceptor;

    function startRequest(config) {
      if (!requestCount) {
        $rootScope.$broadcast('xhrReqStart');
      }

      requestCount++;
      return config;
    }

    function endRequest(arg) {
      // No request ongoing, so make sure we don’t go to negative count
      if (!requestCount)
        return;

      requestCount--;
      // If it was last ongoing request, broadcast event
      if (!requestCount) {
        $rootScope.$broadcast('xhrReqEnd');
      }

      return arg;
    }
  }

})();
