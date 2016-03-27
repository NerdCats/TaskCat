(function() {
  angular
    .module('app')
    .run(appRun);

  appRun.$inject = ['routerHelper'];
  /* @ngInject */
  function appRun(routerHelper) {
    routerHelper.configureStates(getStates());
  }

  /* @ngInject */
  function getStates() {
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
        controller: function($stateParams) {
          return $stateParams.orderCode + 'OrderController';
        },
        controllerAs: 'vm'
      }
    }];
  }
})();
