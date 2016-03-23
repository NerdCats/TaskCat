(function() {
  'use strict';

  angular
    .module('app')
    .constant('appSettings', {
      apiServiceBaseUri: "http://taskcatdev.azurewebsites.net/api/",
      debugEnabled: false
    });
})();
