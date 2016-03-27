(function() {
  angular
    .module('app')
    .run(appRun);

  appRun.$inject = ['routerHelper', '$log'];
  /* @ngInject */
  function appRun(routerHelper, $log) {
    routerHelper.configureStates(getStates($log));
  }

  /* @ngInject */
  function getStates($log) {
    return [{
      state: 'index',
      config: {
        url: '/',
        templateUrl: 'src/layouts/partial.orders.html',
        controller: 'orderController',
        controllerAs: 'vm'
      }
    }, {
      state: 'order',
      config: {
        url: '/order/:orderCode',
        templateUrl: function($stateParams) {
          return 'src/layouts/partial.orders.' + $stateParams.orderCode + '.html';
        },
        controllerProvider: function($stateParams) {
          $log.debug("Selected Controller is " + $stateParams.orderCode.toLowerCase() + 'OrderController');
          return $stateParams.orderCode.toLowerCase() + 'OrderController';
        },
        controllerAs: 'vm'
      }
    }];
  }
})();
