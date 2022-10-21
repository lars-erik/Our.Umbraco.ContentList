(function () {

  var module = null;
  try {
    module = angular.module("our.umbraco.contentlist");
  } catch (e) {
    module = angular.module("our.umbraco.contentlist", ["ngSanitize"]);

    try {
      angular.module("umbraco").requires.push("our.umbraco.contentlist");
    } catch (e) {
      // swallow no umbraco
    }
  }

  module.controller("our.umbraco.contentlist.columnssettingcontroller", [
    "$scope",
    "our.umbraco.contentlist.currentTemplate",
    function (scope, templateState) {
      scope.templateState = templateState;
      scope.supportColumns = true;
      scope.$watch("templateState.currentTemplate",
        function () {
          scope.supportColumns = !(templateState.currentTemplate || {}).disableColumnsSetting;
        });

      scope.$watch("model.value.medium",
        function () {
          scope.model.value.large = Math.max(scope.model.value.medium, scope.model.value.large);
        });
    }
  ]);

}());