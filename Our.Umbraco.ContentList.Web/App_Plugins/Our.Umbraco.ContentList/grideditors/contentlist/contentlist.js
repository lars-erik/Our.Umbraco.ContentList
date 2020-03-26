(function () {
    "use strict";

    var parameterViews = {
        "dropdownlist": "our.umbraco.contentlist.dropdownlist.html",
        "textbox": "our.umbraco.contentlist.textbox.html"
    };

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

    function keyNameToIdValue(obj) {
        return { id: obj.key, value: obj.name };
    }

    function ContentListEditorController(scope, http, q, datasourceService, templatesService, defaultSettings, editorState, sce) {
        
        function initialize() {
            if (!scope.control.editor.config.settings.datasource) {
                scope.control.editor.config.settings = defaultSettings;
            }

            scope.preview = null;
            scope.error = false;
            scope.datasources = [];
            scope.model = scope.control;
            scope.model.config = $.extend(
                createConfig(),
                scope.model.config,
                { scopeId: scope.$id }
            );
            
            scope.content = editorState.getCurrent();
        }

        function datasourcesLoaded(sources) {
            scope.datasources = sources;
            scope.control.editor.config.settings.datasource.config.items = $.map(scope.datasources, keyNameToIdValue);
            scope.control.editor.config.settings.datasource.config.datasources = scope.datasources;
        }

        function templatesLoaded(templates) {
            scope.templates = templates;
            scope.control.editor.config.settings.view.config.items = $.map(scope.templates, function (t) { return { id:t, value:t }; });
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
            var i;
            for (i = 0; i < scope.model.config.datasource.parameters.length; i++) {
                if (!scope.model.config.datasource.parameters[i].value) {
                    return false;
                }
            }
            if (!scope.model.config.datasource.type) {
                return false;
            }
            if (!scope.model.config.view) {
                return false;
            }

            return true;
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
                var p1 = datasourceService.getDataSources().then(datasourcesLoaded),
                    p2 = templatesService.getTemplates().then(templatesLoaded);
                q.when(p1, p2).then(function() {
                    scope.$watch("model.config", loadPreviewIfValid, true);
                });
            } else {
                scope.$watch("model.config", loadPreviewIfValid, true);
            }
        }

        initialize();
        main();
    }

    function createDataSourceService(q, http, requestHelper) {
        return {
            getDataSources: function () {
                var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Web.DataSources.ListableDataSource", "GetDataSources"),
                    def = q.defer();

                http.get(url)
                    .then(function (response) {
                        def.resolve(response.data);
                    });

                return def.promise;
            }
        };
    }

    function createTemplatesService(q, http, requestHelper) {
        return {
            getTemplates: function () {
                var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Web.ContentListApi", "ListTemplates"),
                    def = q.defer();

                http.get(url)
                    .then(function (response) {
                        def.resolve(response.data);
                    });

                return def.promise;
            }
        };
    }

    function ParametersController(scope, http) {
    }

    function DataSourceController(scope) {
        var first = true;

        scope.model.value = JSON.parse(JSON.stringify(scope.model.value));

        function getParameters(sourceKey) {
            var source = $.grep(scope.model.config.datasources, function (ds) { return ds.key === sourceKey; })[0];
            if (source) {
                return JSON.parse(JSON.stringify(source.parameters || []));
            }
            return [];
        };

        scope.datasourceProperty = {
            "label": "Source Type",
            "hideLabel": true,
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

        scope.$watch("datasourceProperty.value", function (newVal, oldVal) {
            if (newVal !== oldVal && scope.datasourceProperty.value) {
                scope.model.value.type = scope.datasourceProperty.value;
            }
            if (newVal !== oldVal || first) {
                first = false;
                var parameters = getParameters(scope.model.value.type);

                for (var i = 0; i < scope.model.value.parameters.length; i++) {
                    var prm = $.grep(parameters, function (x) { return x.key === scope.model.value.parameters[i].key; })[0];
                    if (prm) {
                        prm.value = scope.model.value.parameters[i].value;
                    }
                }

                scope.parametersProperty.config.items = parameters;
            }
        });

        scope.$watch("parametersProperty.config.items",
            function () {
                scope.model.value.parameters = $.map(scope.parametersProperty.config.items, function(x) {
                    return { key: x.key, value: x.value };
                });
            },
            true);
    }

    var module = angular.module("our.umbraco.contentlist", ["ngSanitize"]);

    module.controller("our.umbraco.contentlist.datasource.controller", [
        "$scope",
        DataSourceController
    ]);

    module.controller("our.umbraco.contentlist.editor.controller", [
        "$scope",
        "$http",
        "$q",
        "our.umbraco.contentlist.datasource.service",
        "our.umbraco.contentlist.templates.service",
        "our.umbraco.contentlist.defaultSettings",
        "editorState",
        "$sce",
        ContentListEditorController
    ]);

    module.controller("our.umbraco.contentlist.parameters.controller", [
        "$scope",
        "$http",
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

    //module.directive("contentListWizard", createWizardDirective);

    module.run([
        "$templateCache",
        function (cache) {

            cache.put("our.umbraco.contentlist.dropdownlist.html", "<select ng-model=\"parameter.value\" ng-options=\"opt.key as opt.name for opt in parameter.options\"></select>");
            cache.put("our.umbraco.contentlist.textbox.html", "<input type=\"text\" ng-model=\"parameter.value\" />");

        }]);

    try {
        angular.module("umbraco").requires.push("our.umbraco.contentlist");
    } catch (e) {
        // swallow no umbraco
    }

    module.value("our.umbraco.contentlist.defaultSettings", {
        datasource: {
            "label": "Data source",
            "key": "datasource",
            "description": "How to get the content.",
            "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/datasource/datasource.html",
            "config": {
                "items": [
                ]
            }
        },
        view: {
            "label": "Theme",
            "key": "view",
            "description": "How the list should look",
            "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/dropdown/dropdown.html",
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
            "label": "Show paging",
            "key": "showPaging",
            "description": "",
            "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/boolean/boolean.html"
        },
        skip: {
            "label": "Skip items",
            "key": "skip",
            "description": "Enter a number of items to skip.",
            "view": "/umbraco/views/propertyeditors/integer/integer.html",
        }
    });

}());