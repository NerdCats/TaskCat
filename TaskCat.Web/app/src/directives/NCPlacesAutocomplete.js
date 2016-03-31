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
      controller: Controller,
      controllerAs: 'placesvm',
      bindToController: true
    };

    return directive;

    /* @ngInject */
    function Controller($scope, $element, $attrs) {
      var vm = this;
      vm.results = [];

       $element.on('input', function(query) {
           query =  $scope.$eval($attrs.mdSearchText);
           if(query)
             fetch(query);
       });

      var googleMapsService = new google.maps.places.AutocompleteService();

      var fetch = function(query) {
        googleMapsService.getPlacePredictions({
          input: query
        }, fetchCallback);
      };

      var fetchCallback = function(predictions, status) {
        if (status !== google.maps.places.PlacesServiceStatus.OK) {
          vm.results = [];
        } else {
          vm.results = predictions;
        }
      };
    }

  }
})();
