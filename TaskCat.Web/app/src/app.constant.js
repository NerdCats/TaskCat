(function() {
  'use strict';

  angular
    .module('app')
    .constant('appSettings', {
      apiServiceBaseUri: "http://gofetch.cloudapp.net/api/",
      debugEnabled: true
    });
})();
