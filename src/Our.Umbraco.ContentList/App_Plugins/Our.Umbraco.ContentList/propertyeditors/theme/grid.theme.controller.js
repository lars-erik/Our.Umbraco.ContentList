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

  module.controller("our.umbraco.contentlist.theme.controller", [
    "$scope",
    "our.umbraco.contentlist.currentDatasource",
    "our.umbraco.contentlist.currentTemplate",
    function (scope, dsState, templateState) {
      var nameEx = /\.([^\.]+),/i,
        getName = function (str) {
          return ((str || "").match(nameEx) || [])[1];
        };

      function findTemplate(name) {
        return $.grep(scope.model.config.all, function (x) { return x.name === name; })[0];
      }

      scope.dsState = dsState;
      scope.type = dsState.ds.type;
      scope.shortType = getName(scope.type);

      scope.$watch("dsState.ds.type",
        function () {
          scope.type = dsState.ds.type;
          scope.shortType = getName(scope.type);
        });

      scope.shouldShow = function (item, i, a) {
        var template = findTemplate(item.value);
        if (template) {
          if ((template.compatibleSources || []).length === 0) {
            return true;
          } else {
            return $.grep(template.compatibleSources, function (x) { return x === scope.shortType; }).length > 0;
          }
        }
        return true;
      };

      scope.$watch("model.value",
        function () {
          templateState.currentTemplate = findTemplate(scope.model.value);
        });
    }
  ]);

}());