import module from "./../common/module";

function keyNameToIdValue(obj) {
  return { id: obj.key, value: obj.name };
}

module.controller("our.umbraco.contentlist.blge.datasource.controller",
  [
    "$scope",
    "our.umbraco.contentlist.datasource.service",
    "our.umbraco.contentlist.currentDatasource",
    function (scope, dataSourceService, dataSourceState) {
      function dataSourcesLoaded(sources) {
        scope.datasources = sources;
        scope.datasourceOptions = scope.datasources.map(keyNameToIdValue);
        scope.datasourceProperty.config.items = scope.datasourceOptions;
        if (scope.model.value.type) {
          updateParameters();
        }
      }

      function getParameters(sourceKey) {
        var source = $.grep(scope.datasources || [], function (ds) { return ds.key === sourceKey; })[0];
        if (source) {
          return JSON.parse(JSON.stringify(source.parameters || []));
        }
        return [];
      };

      function updateParameters() {
        if (!scope.datasources) { return; }

        var parameters = getParameters(scope.model.value.type);

        for (var i = 0; i < scope.model.value.parameters.length; i++) {
          var prm = $.grep(parameters, function (x) { return x.key === scope.model.value.parameters[i].key; })[0];
          if (prm) {
            prm.value = scope.model.value.parameters[i].value;
          }
        }

        scope.parametersProperty.config.items = parameters;
      }

      var first = true;

      scope.model.value = scope.model.value ||
      {
        type: "",
        parameters: []
      };

      scope.datasourceProperty = {
        "label": "Source Type",
        "alias": "datasourceType",
        "key": "datasourceType",
        "description": "Type of source.",
        "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/dropdown/dropdown.html",
        "config": {
          "items": scope.values
        },
        "value": scope.model.value.type
      };

      scope.parametersProperty = {
        "label": "Parameters",
        "alias": "parameters",
        "key": "parameters",
        "description": "Parameters for the data source.",
        "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/parameters/parameters.html",
        "hideLabel": true,
        "config": {
          "items": scope.model.value.parameters || []
        }
      };

      dataSourceService.getDataSources().then(dataSourcesLoaded);

      scope.$watch("datasourceProperty.value", function (newVal, oldVal) {
        if (newVal !== oldVal && scope.datasourceProperty.value) {
          scope.model.value.type = scope.datasourceProperty.value;
          dataSourceState.ds = scope.model.value;
        }
        if (newVal !== oldVal || first) {
          first = false;
          updateParameters();
        }
      });

      scope.$watch("parametersProperty.config.items",
        function () {
          scope.model.value.parameters = $.map(scope.parametersProperty.config.items, function (x) {
            return { key: x.key, value: x.value };
          });
        },
        true);
    }
  ]);
