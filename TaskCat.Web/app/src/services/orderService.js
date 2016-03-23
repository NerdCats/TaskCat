(function() {
  'use strict';

  angular
    .module('app')
    .service('orderService', orderService);

  orderService.$inject = ['appSettings', '$http', '$q'];

  /* @ngInject */
  function orderService(appSettings, $http, $q) {
    this.getSupportedOrders = getSupportedOrders;
    var serviceUrlExtension = 'order/';

    function getSupportedOrders() {
      var deferred = $q.defer();
      return $http.get(appSettings.apiServiceBaseUri + serviceUrlExtension + 'supportedOrder');
    }
  }
})();
