(function(factory) {
  typeof define === "function" && define.amd ? define(factory) : factory();
})(function() {
  "use strict";
  var module = null;
  try {
    module = angular.module("our.umbraco.contentlist");
  } catch (e) {
    module = angular.module("our.umbraco.contentlist", ["ngSanitize"]);
    try {
      angular.module("umbraco").requires.push("our.umbraco.contentlist");
    } catch (e2) {
    }
  }
  const module$1 = module;
  function createDataSourceService(q, http, requestHelper) {
    var data = null;
    return {
      getDataSources: function() {
        var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Controllers.ContentListApi", "GetDataSources"), def = q.defer();
        if (data) {
          def.resolve(data);
        } else {
          http.get(url).then(function(response) {
            data = response.data;
            def.resolve(response.data);
          });
        }
        return def.promise;
      }
    };
  }
  module$1.factory("our.umbraco.contentlist.datasource.service", [
    "$q",
    "$http",
    "umbRequestHelper",
    createDataSourceService
  ]);
  module$1.value("our.umbraco.contentlist.defaultSettings", {
    datasource: {
      "label": "Data source",
      "key": "datasource",
      "description": "How to get the content.",
      "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/datasource/blge-datasource.html",
      "hideLabel": true,
      "config": {
        "items": []
      }
    },
    view: {
      "label": "Theme",
      "key": "view",
      "description": "How the list should look",
      "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/theme/blge-theme.html",
      "config": {
        "items": [],
        "all": []
      }
    },
    columns: {
      "label": "Columns",
      "key": "columns",
      "description": "Columns per screen size (advanced)",
      "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/columns/columns.html",
      "config": {
        "min": 1,
        "max": 100
      }
    },
    pagesize: {
      "label": "Page Size",
      "key": "pagesize",
      "description": "Count of items per page",
      "view": "/umbraco/views/propertyeditors/integer/integer.html",
      "config": {
        "min": 1,
        "max": 100
      }
    },
    skip: {
      "label": "Skip items",
      "key": "skip",
      "description": "Enter a number of items to skip.",
      "view": "/umbraco/views/propertyeditors/integer/integer.html"
    },
    showPaging: {
      "label": "Show paging",
      "key": "showPaging",
      "description": "",
      "view": "/umbraco/views/propertyeditors/boolean/boolean.html"
    }
  });
  var currentDataSource = new function() {
    this.ds = {};
  }();
  var currentTemplate = new function() {
    this.template = {};
  }();
  module$1.factory("our.umbraco.contentlist.currentDatasource", [
    function() {
      return currentDataSource;
    }
  ]);
  module$1.factory("our.umbraco.contentlist.currentTemplate", [
    function() {
      return currentTemplate;
    }
  ]);
  function createTemplatesService(q, http, requestHelper) {
    return {
      getTemplates: function() {
        var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Controllers.ContentListApi", "GetTemplates"), def = q.defer();
        http.get(url).then(function(response) {
          def.resolve(response.data);
        });
        return def.promise;
      }
    };
  }
  module$1.factory("our.umbraco.contentlist.templates.service", [
    "$q",
    "$http",
    "umbRequestHelper",
    createTemplatesService
  ]);
  function createConfig$1() {
    return {
      datasource: {
        type: "",
        parameters: []
      },
      view: "",
      pagesize: 10,
      showPaging: false,
      columns: {
        small: 1,
        medium: 2,
        large: 2
      }
    };
  }
  function keyNameToIdValue$1(obj) {
    return { id: obj.key, value: obj.name };
  }
  function ContentListEditorController(scope, http, q, datasourceService, templatesService, defaultSettings, editorState, sce, dsState) {
    function initialize() {
      if (!scope.control.editor.config.settings.datasource) {
        scope.control.editor.config.settings = defaultSettings;
      }
      scope.preview = null;
      scope.error = false;
      scope.datasources = [];
      scope.model = scope.control;
      scope.model.config = $.extend(
        createConfig$1(),
        scope.model.config,
        { scopeId: scope.$id }
      );
      scope.content = editorState.getCurrent();
    }
    function datasourcesLoaded(sources) {
      scope.datasources = sources;
      scope.control.editor.config.settings.datasource.config.items = $.map(scope.datasources, keyNameToIdValue$1);
      scope.control.editor.config.settings.datasource.config.datasources = scope.datasources;
      dsState.all = scope.datasources;
    }
    function templatesLoaded(templates) {
      scope.templates = templates;
      scope.control.editor.config.settings.view.config.items = $.map(scope.templates, function(t) {
        return { id: t.name, value: t.displayName || t.name };
      });
      scope.control.editor.config.settings.view.config.all = scope.templates;
    }
    function previewLoaded(response) {
      scope.preview = response.data;
      scope.error = false;
    }
    function previewFailed(response) {
      scope.preview = null;
      scope.error = response.data;
    }
    function isValid() {
      if (!scope.model.config.datasource.type) {
        return false;
      }
      if (!scope.model.config.view) {
        return false;
      }
      return scope.model.config.showPaging instanceof Boolean;
    }
    function loadPreviewIfValid() {
      if (isValid()) {
        var params = $.extend({}, scope.model.config, { contentId: scope.content.id });
        http.post("/umbraco/ourcontentlist/contentlist/preview", params).then(previewLoaded, previewFailed);
      }
    }
    scope.trustedHtml = function(plainText) {
      return sce.trustAsHtml(plainText);
    };
    function main() {
      if (!scope.control.editor.config.loadStarted) {
        scope.control.editor.config.loadStarted = true;
        var p1 = datasourceService.getDataSources().then(datasourcesLoaded), p2 = templatesService.getTemplates().then(templatesLoaded);
        q.when(p1, p2).then(function() {
          scope.$watch("model.config", loadPreviewIfValid, true);
        });
      } else {
        scope.$watch("model.config", loadPreviewIfValid, true);
      }
      scope.model.config.showPaging = scope.model.config.showPaging ? "1" : "0";
      scope.$watch(
        "model.config.showPaging",
        function(val) {
          if (!(val instanceof Boolean)) {
            scope.model.config.showPaging = new Boolean(val === "1");
          }
        }
      );
    }
    initialize();
    main();
  }
  module$1.controller("our.umbraco.contentlist.editor.controller", [
    "$scope",
    "$http",
    "$q",
    "our.umbraco.contentlist.datasource.service",
    "our.umbraco.contentlist.templates.service",
    "our.umbraco.contentlist.defaultSettings",
    "editorState",
    "$sce",
    "our.umbraco.contentlist.currentDatasource",
    ContentListEditorController
  ]);
  module$1.run([
    "$templateCache",
    function(cache) {
      cache.put("our.umbraco.contentlist.dropdownlist.html", '<select ng-model="parameter.value" ng-options="opt.key as opt.name for opt in parameter.options"></select>');
      cache.put("our.umbraco.contentlist.textbox.html", '<input type="text" ng-model="parameter.value" />');
    }
  ]);
  function keyNameToIdValue(obj) {
    return { id: obj.key, value: obj.name };
  }
  module$1.controller(
    "our.umbraco.contentlist.blge.datasource.controller",
    [
      "$scope",
      "our.umbraco.contentlist.datasource.service",
      "our.umbraco.contentlist.currentDatasource",
      function(scope, dataSourceService, dataSourceState) {
        function dataSourcesLoaded(sources) {
          scope.datasources = sources;
          scope.datasourceOptions = scope.datasources.map(keyNameToIdValue);
          scope.datasourceProperty.config.items = scope.datasourceOptions;
          if (scope.model.value.type) {
            updateParameters();
          }
        }
        function getParameters(sourceKey) {
          var source = $.grep(scope.datasources || [], function(ds) {
            return ds.key === sourceKey;
          })[0];
          if (source) {
            return JSON.parse(JSON.stringify(source.parameters || []));
          }
          return [];
        }
        function updateParameters() {
          if (!scope.datasources) {
            return;
          }
          var parameters = getParameters(scope.model.value.type);
          for (var i = 0; i < scope.model.value.parameters.length; i++) {
            var prm = $.grep(parameters, function(x) {
              return x.key === scope.model.value.parameters[i].key;
            })[0];
            if (prm) {
              prm.value = scope.model.value.parameters[i].value;
            }
          }
          scope.parametersProperty.config.items = parameters;
        }
        var first = true;
        scope.model.value = scope.model.value || {
          type: "",
          parameters: []
        };
        dataSourceService.getDataSources().then(dataSourcesLoaded);
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
        scope.$watch("datasourceProperty.value", function(newVal, oldVal) {
          if (newVal !== oldVal && scope.datasourceProperty.value) {
            scope.model.value.type = scope.datasourceProperty.value;
            dataSourceState.ds = scope.model.value;
          }
          if (newVal !== oldVal || first) {
            first = false;
            updateParameters();
          }
        });
        scope.$watch(
          "parametersProperty.config.items",
          function() {
            scope.model.value.parameters = $.map(scope.parametersProperty.config.items, function(x) {
              return { key: x.key, value: x.value };
            });
          },
          true
        );
      }
    ]
  );
  module$1.controller("our.umbraco.contentlist.blge.theme.controller", [
    "$scope",
    "our.umbraco.contentlist.templates.service",
    "our.umbraco.contentlist.currentDatasource",
    "our.umbraco.contentlist.currentTemplate",
    function(scope, templateService, dsState, templateState) {
      var nameEx = /\.([^\.]+),/i, getName = function(str) {
        return ((str || "").match(nameEx) || [])[1];
      };
      function findTemplate(name) {
        return $.grep(scope.model.config.all, function(x) {
          return x.name === name;
        })[0];
      }
      scope.dsState = dsState;
      scope.type = dsState.ds.type;
      scope.shortType = getName(scope.type);
      scope.$watch(
        "dsState.ds.type",
        function() {
          scope.type = dsState.ds.type;
          scope.shortType = getName(scope.type);
        }
      );
      scope.shouldShow = function(item, i, a) {
        var template = findTemplate(item.value);
        if (template) {
          if ((template.compatibleSources || []).length === 0) {
            return true;
          } else {
            return $.grep(template.compatibleSources, function(x) {
              return x === scope.shortType;
            }).length > 0;
          }
        }
        return true;
      };
      scope.$watch(
        "model.value",
        function() {
          templateState.currentTemplate = findTemplate(scope.model.value);
        }
      );
      templateService.getTemplates().then((templates) => {
        scope.model.config.all = templates;
        scope.model.config.items = templates.map(
          function(t) {
            return { id: t.name, value: t.displayName || t.name };
          }
        );
      });
    }
  ]);
  module$1.controller("our.umbraco.contentlist.columnssettingcontroller", [
    "$scope",
    "our.umbraco.contentlist.currentTemplate",
    function(scope, templateState) {
      scope.templateState = templateState;
      scope.supportColumns = true;
      scope.$watch(
        "templateState.currentTemplate",
        function() {
          scope.supportColumns = !(templateState.currentTemplate || {}).disableColumnsSetting;
        }
      );
      scope.$watch(
        "model.value.medium",
        function() {
          scope.model.value.large = Math.max(scope.model.value.medium, scope.model.value.large);
        }
      );
    }
  ]);
  function createConfig() {
    return {
      datasource: {
        type: "",
        parameters: []
      },
      view: "",
      pagesize: 10,
      showPaging: false,
      columns: {
        small: 1,
        medium: 2,
        large: 2
      }
    };
  }
  module$1.controller(
    "our.umbraco.contentlist.query.editor.controller",
    [
      "$scope",
      "our.umbraco.contentlist.defaultSettings",
      "editorState",
      function(scope, defaultSettings, editorState) {
        console.log("current", editorState.current);
        scope.enableGui = editorState.current.udi.indexOf("umb://document/") === 0;
        if (!scope.enableGui) {
          return;
        }
        scope.model.value = scope.model.value || createConfig();
        scope.properties = Object.assign({}, defaultSettings);
        for (var key in scope.properties) {
          if (typeof scope.model.value[key] === "boolean") {
            scope.properties[key].value = scope.model.value[key] ? "1" : "0";
          } else {
            scope.properties[key].value = scope.model.value[key];
          }
          scope.$watch(
            "properties." + key + ".value",
            ((subKey) => (val) => {
              scope.model.value[subKey] = val;
            })(key)
          );
        }
        scope.$watch(
          "model.value.showPaging",
          function(val) {
            if (!(val instanceof Boolean)) {
              scope.model.value.showPaging = new Boolean(val === "1");
            }
          }
        );
      }
    ]
  );
  function DataSourceController(scope, dsState) {
    var first = true;
    scope.model.value = JSON.parse(JSON.stringify(scope.model.value || {}));
    dsState.ds = scope.model.value;
    function getParameters(sourceKey) {
      var source = $.grep(scope.model.config.datasources || [], function(ds) {
        return ds.key === sourceKey;
      })[0];
      if (source) {
        return JSON.parse(JSON.stringify(source.parameters || []));
      }
      return [];
    }
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
    scope.$watch(
      "datasourceProperty.value",
      function(newVal, oldVal) {
        if (newVal !== oldVal && scope.datasourceProperty.value) {
          scope.model.value.type = scope.datasourceProperty.value;
          dsState.ds = scope.model.value;
        }
        if (newVal !== oldVal || first) {
          first = false;
          var parameters = getParameters(scope.model.value.type);
          for (var i = 0; i < scope.model.value.parameters.length; i++) {
            var prm = $.grep(parameters, function(x) {
              return x.key === scope.model.value.parameters[i].key;
            })[0];
            if (prm) {
              prm.value = scope.model.value.parameters[i].value;
            }
          }
          scope.parametersProperty.config.items = parameters;
        }
      }
    );
    scope.$watch(
      "parametersProperty.config.items",
      function() {
        scope.model.value.parameters = $.map(
          scope.parametersProperty.config.items,
          function(x) {
            return { key: x.key, value: x.value };
          }
        );
      },
      true
    );
  }
  module$1.controller("our.umbraco.contentlist.datasource.controller", [
    "$scope",
    "our.umbraco.contentlist.currentDatasource",
    DataSourceController
  ]);
  module$1.controller("our.umbraco.contentlist.theme.controller", [
    "$scope",
    "our.umbraco.contentlist.currentDatasource",
    "our.umbraco.contentlist.currentTemplate",
    function(scope, dsState, templateState) {
      var nameEx = /\.([^\.]+),/i, getName = function(str) {
        return ((str || "").match(nameEx) || [])[1];
      };
      function findTemplate(name) {
        return $.grep(scope.model.config.all, function(x) {
          return x.name === name;
        })[0];
      }
      scope.dsState = dsState;
      scope.type = dsState.ds.type;
      scope.shortType = getName(scope.type);
      scope.$watch(
        "dsState.ds.type",
        function() {
          scope.type = dsState.ds.type;
          scope.shortType = getName(scope.type);
        }
      );
      scope.shouldShow = function(item, i, a) {
        var template = findTemplate(item.value);
        if (template) {
          if ((template.compatibleSources || []).length === 0) {
            return true;
          } else {
            return $.grep(template.compatibleSources, function(x) {
              return x === scope.shortType;
            }).length > 0;
          }
        }
        return true;
      };
      scope.$watch(
        "model.value",
        function() {
          templateState.currentTemplate = findTemplate(scope.model.value);
        }
      );
    }
  ]);
});
