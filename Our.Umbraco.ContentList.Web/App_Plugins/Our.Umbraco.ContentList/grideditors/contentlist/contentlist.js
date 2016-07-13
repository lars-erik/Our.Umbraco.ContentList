(function () {
    "use strict";

    var parameterViews = {
        "dropdownlist": "our.umbraco.contentlist.dropdownlist.html",
        "textbox": "our.umbraco.contentlist.textbox.html"
    }

    function findScopeValue(scope, key) {
        if (!scope) {
            return null;
        }
        if (scope[key]) {
            return scope[key];
        }
        if (scope.$parent) {
            return findScopeValue(scope.$parent, key);
        }
        return null;
    }

    function createConfig() {
        return {
            datasource: "",
            view: "default",
            pagesize: 10,
            showPaging: false,
            columns: {
                small: 1,
                medium: 2,
                large: 2
            },
            parameters: []
        };
    }

    function keyNameToIdValue(obj) {
        return { id: obj.key, value: obj.name };
    }

    function ContentListEditorController(scope, http, datasourceService, templatesService, requestHelper) {
        var model,
            content,
            settings,
            parameterLoader = {};

        function initialize() {
            scope.preview = "Select data source to enable preview";
            scope.datasources = [];
            model = scope.model = scope.control;
            model.config = $.extend(createConfig(), model.value, model.config);

            if (typeof (model.config.columns) !== "object")
                model.config.columns = {};

            content = findScopeValue(scope, "content");

            // new shit

            settings = {
                datasource: {
                    "label": "Data source",
                    "key": "datasource",
                    "description": "The source of items for the list",
                    "view": "/umbraco/views/propertyeditors/dropdown/dropdown.html",
                    "config": {
                        "items": [
                        ]
                    }
                },
                view: {
                    "label": "Theme",
                    "key": "view",
                    "description": "How the list should look",
                    "view": "/umbraco/views/propertyeditors/dropdown/dropdown.html",
                    "config": {
                        "items": [
                        ]
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
                showPaging: {
                    "label": "Show Paging",
                    "key": "showPaging",
                    "description": "",
                    "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/boolean/boolean.html"
                },
                skip: {
                    "label": "Skip items",
                    "key": "skip",
                    "description": "Skip the first items in the list",
                    "view": "/umbraco/views/propertyeditors/integer/integer.html",
                },
                parameters: {
                    "label": "Parameters",
                    "key": "parameters",
                    "description": "Data source parameters",
                    "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/parameters/parameters.html",
                    "config": {
                        "parameterLoader": parameterLoader
                    }
                }
            };
            scope.control.editor.config.settings = [
                settings.datasource,
                settings.view,
                settings.pagesize,
                settings.columns,
                settings.showPaging,
                settings.skip,
                settings.parameters
            ];

        }

        function previewLoaded(response) {
            scope.preview = response.data;
        }

        function getParametersForDataSource(newSourceKey) {
            var newSource = $.grep(scope.datasources, function (ds) { return ds.key === newSourceKey; })[0];
            if (newSource) {
                return newSource.parameters || [];
            }
            return [];
        }

        function initializeParametersWhenDataSourceChange(newSourceKey, oldSourceKey) {
            if (newSourceKey !== oldSourceKey) {
                var params = getParametersForDataSource(newSourceKey);
                scope.parameters = params;
                scope.hasParams = scope.parameters.length;
                settings.parameters.config.parameters = scope.parameters;
            }
            return scope.parameters;
        }

        function datasourcesLoaded(sources) {
            scope.datasources = sources;
            settings.datasource.config.items = $.map(scope.datasources, keyNameToIdValue);
            if (model.config.datasource) {
                initializeParametersWhenDataSourceChange(model.config.datasource, "");
            }
        }

        function templatesLoaded(templates) {
            scope.templates = templates;
            settings.view.config.items = scope.templates;
        }

        function updateValueParameters(newParameters) {
            function createFilter(paramKey) {
                return function(param) {
                    return param.key === paramKey;
                }
            }

            var i, extParam;

            if (!newParameters) {
                return;
            }

            for (i = 0; i < newParameters.length; i++) {
                extParam = $.grep(model.config.parameters, createFilter(newParameters[i].key))[0];
                if (extParam) {
                    if (newParameters[i].value) {
                        extParam.value = newParameters[i].value;
                    } else {
                        newParameters[i].value = extParam.value;
                    }
                } else {
                    model.config.parameters.push({ key: newParameters[i].key, value: newParameters[i].value });
                }
            }
            for (i = model.config.parameters.length - 1; i >= 0; i--) {
                if ($.grep(newParameters, createFilter(model.config.parameters[i].key)).length === 0) {
                    model.config.parameters.splice(i, 1);
                }
            }
        }

        function loadPreviewIfValid() {
            if (model.config.datasource) {
                // TODO: Validate parameters
                var params = angular.extend({}, model.config, { contentId: content.id });
                http.post("/umbraco/ourcontentlist/contentlist/preview", params).then(previewLoaded);
            }
        }

        function parameterView(parameter) {
            return parameterViews[parameter.type];
        }

        function main() {
            datasourceService.getDataSources().then(datasourcesLoaded);
            templatesService.getTemplates().then(templatesLoaded);
            scope.$watch("model.config.datasource", initializeParametersWhenDataSourceChange);
            scope.$watch("model.config", loadPreviewIfValid, true);
            scope.$watch("parameters", updateValueParameters, true);
            scope.parameterView = parameterView;

            parameterLoader.update = getParametersForDataSource;
        }

        initialize();
        main();
    }

    function createDataSourceService(q, http, requestHelper) {
        return {
            getDataSources: function() {
                var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Web.DataSources.ListableDataSource", "GetDataSources"),
                    def = q.defer();

                http.get(url)
                    .then(function(response) {
                        def.resolve(response.data);
                    });

                return def.promise;
            }
        };
    }

    function createTemplatesService(q, http, requestHelper) {
        return {
            getTemplates: function() {
                var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Web.ContentListApi", "ListTemplates"),
                    def = q.defer();

                http.get(url)
                    .then(function(response) {
                        def.resolve(response.data);
                    });

                return def.promise;
            }
        };
    }

    function ParametersController(scope) {

        var unwatchers = []
            , parentModel = scope.$parent.$parent.$parent.model
            , datasourceModel = $.grep(parentModel.config, function(setting) {
                  return setting.key === "datasource";
              })[0]
            ;

        function resetProperties() {
            var i;

            for (i = unwatchers.length - 1; i >= 0; i--) {
                unwatchers[i]();
                unwatchers.splice(i, 1);
            }

            scope.properties = $.map(scope.model.config.parameters,
                function(param) {
                    return $.extend({},
                        param,
                        $.grep(scope.model.value, function(val) { return val.key === param.key; })[0],
                        {
                            config: {
                                items: param.options ? $.map(param.options, keyNameToIdValue) : null
                            }
                        }
                    );
                });

            $.each(scope.properties,
                function(i, p) {
                    unwatchers.push(scope.$watch(function() { return p.value; },
                        function() {
                            var modelValue = $.grep(scope.model.value, function(v) { return v.key === p.key; })[0];
                            if (!modelValue) {
                                scope.model.value.push({
                                    key: p.key,
                                    value: p.value
                                });
                            } else {
                                modelValue.value = p.value;
                            }
                        }));
                });
        }

        scope.datasourceModel = datasourceModel;

        scope.$watch("datasourceModel.value",
            function(newValue, oldValue) {
                scope.model.config.parameters = scope.model.config.parameterLoader.update(newValue, oldValue);
                resetProperties();
            });
    }

    var module = angular.module("our.umbraco.contentlist", []);

    module.controller("our.umbraco.contentlist.editor.controller", [
        "$scope",
        "$http",
        "our.umbraco.contentlist.datasource.service",
        "our.umbraco.contentlist.templates.service",
        "umbRequestHelper",
        ContentListEditorController
    ]);

    module.controller("our.umbraco.contentlist.parameters.controller", [
        "$scope",
        ParametersController
    ]);

    module.factory("our.umbraco.contentlist.datasource.service", [
        "$q",
        "$http",
        "umbRequestHelper",
        createDataSourceService
    ]);

    module.factory("our.umbraco.contentlist.templates.service", [
        "$q",
        "$http",
        "umbRequestHelper",
        createTemplatesService
    ]);

    module.run([
        "$templateCache",
        function(cache) {

            cache.put("our.umbraco.contentlist.dropdownlist.html", "<select ng-model=\"parameter.value\" ng-options=\"opt.key as opt.name for opt in parameter.options\"></select>");
            cache.put("our.umbraco.contentlist.textbox.html", "<input type=\"text\" ng-model=\"parameter.value\" />");

        }]);

    try {
        angular.module("umbraco").requires.push("our.umbraco.contentlist");
    } catch (e) {
        // swallow no umbraco
    }

}());