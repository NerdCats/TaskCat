(function() {
  'use strict';

  angular
    .module('app')
    .directive('ncPlacesAutocomplete', NCPlacesAutocomplete);

  /* @ngInject */
  function NCPlacesAutocomplete() {
    var directive = {
      restrict: 'A',
      require: '^mdAutocomplete',
      link: linkFunc,
      controller: Controller,
      controllerAs: 'autovm',
      bindToController: true
    };

    return directive;

    function linkFunc(scope, element, attrs, ctrl) {


      ctrl.scope.$watch('searchText', function(query) {

      });

    }

    /* @ngInject */
    function Controller($scope, $element, $attrs) {
      var vm = this;
      vm.results = [];

      $scope.$watch('searchText', function(query) {
        console.log(query);
        if(query && query.length>3)
          fetch(query);
      });
      var googleMapsService = new google.maps.places.AutocompleteService();

      var fetch = function(query) {
        console.log(query);
        googleMapsService.getPlacePredictions({
          input: query
        }, fetchCallback);
      };

      var fetchCallback = function(predictions, status) {
        console.log(status);
        if (status !== google.maps.places.PlacesServiceStatus.OK) {
          vm.results = [];
        } else {
          console.log(predictions);
          vm.results = predictions;
        }
      };
    }

  }
})();
