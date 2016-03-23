(function() {
  'use strict';

  angular
    .module('app')
    .directive('ncBackImg', ncBackImg);

  /* @ngInject */
  function ncBackImg() {
    var directive = {
      restrict: 'EA',
      link: linkFunc
    };

    return directive;

    function linkFunc(scope, element, attrs, ctrl) {
      var url = attrs.ncBackImg;
      element.css({
        'background-image': 'url(' + url + ')',
        'background-size': 'cover'
      });
    }
  }
})();
