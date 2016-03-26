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
      state: 'test',
      config: {
        url: '/test',
        template: 'src/layouts/partial.orders.html'
      }
    }];
  }
})();
