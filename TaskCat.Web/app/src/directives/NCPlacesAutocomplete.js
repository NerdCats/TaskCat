(function() {
  'use strict';

  angular
    .module('app')
    .directive('NCPlacesAutocomplete', NCPlacesAutocomplete);

  /* @ngInject */
  function NCPlacesAutocomplete() {
    var directive = {
      restrict: 'A',
      link: linkFunc
    };

    return directive;

    function linkFunc(scope, element, attrs, ctrl) {
      var googleMapsService = new google.maps.places.AutocompleteService();

      element.bind('md-search-text-change', function() {
        var query = element.val();
        if (query && query.length >= 3) {
          fetch(query);
        } else {
          scope.$apply(function() {
            attrs["md-items"] = [];
          });
        }
      });

      var fetch = function(query) {
        googleMapsService.getPlacePredictions({
          input: query
        }, fetchCallback);
      };

      var fetchCallback = function(predictions, status) {
        if (status !== google.maps.places.PlacesServiceStatus.OK) {
          scope.$apply(function() {
            attrs["md-items"] = [];
          });
          return;
        } else {
          scope.$apply(function() {
            attrs["md-items"] = predictions;
            attrs["md-item-text"] = 
          });
        }
      };


    }
  }
})();
