(function() {
  'use strict';

  angular
  .module('app')
  .service('orderService', orderService);

  orderService.$inject = ['appSettings'];

  /* @ngInject */
  function orderService(appSettings) {
    this.getSupportedOrders = getSupportedOrders;

    function getSupportedOrders() {
      //This is mock data of course
      var data=[
        {
          "_id":"56e90879180373262cd72d1b",
          "ActionName":"Get",
          "OrderName":"Food",
          "ImageUrl":"string",
          "OrderCode":"GETFOOD"
        },
        {
          "_id":"56e90879180373262cd72d1c",
          "ActionName":"Grab",
          "OrderName":"CNG",
          "ImageUrl":"string",
          "OrderCode":"GRABCNG"
        },
      ];

      return data;
    }
  }
})();
