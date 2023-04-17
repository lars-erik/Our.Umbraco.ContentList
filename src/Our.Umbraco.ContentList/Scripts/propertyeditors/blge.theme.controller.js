import module from "./../common/module";

module.controller("our.umbraco.contentlist.blge.theme.controller", [
  "$scope",
  "our.umbraco.contentlist.templates.service",
  "our.umbraco.contentlist.currentDatasource",
  "our.umbraco.contentlist.currentTemplate",
  function (scope, templateService, dsState, templateState) {
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

    scope.themeProperty = {
        "label": "Theme",
        "alias": "theme",
        "key": "theme",
        "description": "How the list should look.",
        "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/dropdown/dropdown.html",
        "config": {
            "items": scope.model.config.items
        },
        "value": scope.model.value
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
        scope.parametersProperty.config.items = templateState.currentTemplate.parameters;
      });

    templateService.getTemplates().then(templates => {
      scope.model.config.all = templates;
      scope.model.config.items = templates.map(
          function (t) {
              return {
                  id: t.name,
                  value: t.displayName || t.name
              };
          }
      );
    });

  }
]);
