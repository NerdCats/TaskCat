(function() {
  'use strict';

  angular
    .module('app')
    .directive('ncXhrLoader', xhrLoader);

  /* @ngInject */
  function xhrLoader() {
    var directive = {
      restrict: 'EA',
      link: linkFunc
    };

    return directive;

    function linkFunc(scope, element, attr, ctrl) {
      var shownType = element.css('display');

      var hideElement = function() {
        element.css('display', 'none');
      };

      scope.$on('xhrReqStart', function() {
        element.css('display', shownType);
      });

      scope.$on('xhrReqEnd', hideElement);

      //Initally Hide Element
      hideElement();
    }
  }

})();
