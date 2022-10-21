
(function() {

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


  function DataSourceController(scope, dsState) {
    var first = true;

    scope.model.value = JSON.parse(JSON.stringify(scope.model.value || {}));
    dsState.ds = scope.model.value;

    function getParameters(sourceKey) {
      var source = $.grep(scope.model.config.datasources || [], function(ds) { return ds.key === sourceKey; })[0];
      if (source) {
        return JSON.parse(JSON.stringify(source.parameters || []));
      }
      return [];
    };

    scope.datasourceProperty = {
      "label": "Source Type",
      "hideLabel": false,
      "alias": "datasourceType",
      "key": "datasourceType",
      "description": "Type of source.",
      "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/dropdown/dropdown.html",
      "config": {
        "items": scope.model.config.items
      },
      "value": scope.model.value.type
    };

    scope.parametersProperty = {
      "label": "Parameters",
      "alias": "parameters",
      "key": "parameters",
      "description": "Parameters for the data source.",
      "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/parameters/parameters.html",
      "config": {
        "items": []
      }
    };

    scope.$watch("datasourceProperty.value",
      function(newVal, oldVal) {
        if (newVal !== oldVal && scope.datasourceProperty.value) {
          scope.model.value.type = scope.datasourceProperty.value;
          dsState.ds = scope.model.value;
        }
        if (newVal !== oldVal || first) {
          first = false;
          var parameters = getParameters(scope.model.value.type);

          for (var i = 0; i < scope.model.value.parameters.length; i++) {
            var prm = $.grep(parameters, function(x) { return x.key === scope.model.value.parameters[i].key; })[0];
            if (prm) {
              prm.value = scope.model.value.parameters[i].value;
            }
          }

          scope.parametersProperty.config.items = parameters;
        }
      });

    scope.$watch("parametersProperty.config.items",
      function() {
        scope.model.value.parameters = $.map(scope.parametersProperty.config.items,
          function(x) {
            return { key: x.key, value: x.value };
          });
      },
      true);
  }

  module.controller("our.umbraco.contentlist.datasource.controller", [
    "$scope",
    "our.umbraco.contentlist.currentDatasource",
    DataSourceController
  ]);

}());