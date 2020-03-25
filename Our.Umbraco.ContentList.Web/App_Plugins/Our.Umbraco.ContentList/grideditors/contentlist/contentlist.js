(function () {
    "use strict";

    var parameterViews = {
        "dropdownlist": "our.umbraco.contentlist.dropdownlist.html",
        "textbox": "our.umbraco.contentlist.textbox.html"
    };

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

    function ContentListEditorController(scope, http, q, datasourceService, templatesService, defaultSettings, editorState) {
        var model,
            content,
            settings,
            parameterLoader = {},
            tempConfig;

        function initialize() {
            scope.preview = null;
            scope.error = false;
            scope.datasources = [];
            model = scope.model = scope.control;
            model.config = $.extend(createConfig(), model.value, model.config);

            if (typeof (model.config.columns) !== "object") {
                model.config.columns = {};
            }

            content = editorState.getCurrent();

            settings = $.extend({}, defaultSettings);
            settings.parameters = $.extend(settings.parameters, {
                "config": {
                    "parameterLoader": parameterLoader,
                    "parentConfig": model.config
                },
                value: []
            });

            settings.parameters.getTempConfig = function () {
                tempConfig = $.extend({}, model.config);
                return tempConfig;
            };

            settings.datasource.update = function(x) {
                initializeParametersWhenDataSourceChange(x.value, scope.model.config.datasource);
            };

            scope.control.editor.config.settings = [
                settings.datasource,
                settings.parameters,
                settings.view,
                settings.pagesize,
                settings.columns,
                settings.showPaging,
                settings.skip
            ];
        }

        function previewLoaded(response) {
            scope.preview = response.data;
            scope.error = false;
        }

        function previewFailed(response) {
            scope.preview = null;
            scope.error = response.data;
        }

        function getParametersForDataSource(newSourceKey) {
            var newSource = $.grep(scope.datasources, function (ds) { return ds.key === newSourceKey; })[0];
            if (newSource) {
                return newSource.parameters || [];
            }
            return [];
        }

        function initializeParametersWhenDataSourceChange(newSourceKey, oldSourceKey) {
            var params = getParametersForDataSource(newSourceKey);
            if (tempConfig) {
                tempConfig.datasource = newSourceKey;
            }
            return params;
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
            settings.view.config.items = $.map(scope.templates, function (t) { return { id:t, value:t }; });
        }

        function updateValueParameters(newParameters) {
            function createFilter(paramKey) {
                return function(param) {
                    return param.key === paramKey;
                };
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

        function isValid() {
            var i;
            for (i = 0; i < model.config.parameters.length; i++) {
                if (!model.config.parameters[i]) {
                    return false;
                }
            }
            if (!model.config.datasource) {
                return false;
            }
            if (!model.config.view) {
                return false;
            }

            console.log("seems valid, gonna preview", model.config);

            return true;
        }

        function loadPreviewIfValid() {
            if (isValid()) {
                var params = $.extend({}, model.config, { contentId: content.id });
                http.post("/umbraco/ourcontentlist/contentlist/preview", params).then(previewLoaded, previewFailed);
            }
        }

        function parameterView(parameter) {
            return parameterViews[parameter.type];
        }

        function main() {
            var p1 = datasourceService.getDataSources().then(datasourcesLoaded),
                p2 = templatesService.getTemplates().then(templatesLoaded);
            if (model.config.datasource) {
                initializeParametersWhenDataSourceChange(model.config.datasource, null);
            }
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

        var unwatchers = [];

        scope.tempConfig = scope.model.getTempConfig();

        function resetProperties() {
            var i;

            for (i = unwatchers.length - 1; i >= 0; i--) {
                unwatchers[i]();
                unwatchers.splice(i, 1);
            }

            scope.properties = $.map(scope.model.config.parameters,
                function (param) {
                    return $.extend({},
                        param,
                        $.grep(scope.model.value, function (val) { return val.key === param.key; })[0],
                        {
                            config: {
                                items: param.options ? $.map(param.options, keyNameToIdValue) : null
                            }
                        }
                    );
                });

            $.each(scope.properties,
                function (i, p) {
                    unwatchers.push(scope.$watch(function () { return p.value; },
                        function () {
                            var modelValue = $.grep(scope.model.value, function (v) { return v.key === p.key; })[0];
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

        function dataSourceUpdated(newValue, oldValue) {
            if (newValue !== oldValue) {
                scope.model.config.parameters = scope.model.config.parameterLoader.update(newValue, oldValue);
                resetProperties();
            }
        }

        // Copy all values to avoid updating while editing
        scope.model.value = JSON.parse(JSON.stringify(scope.model.value));

        scope.$watch("tempConfig.datasource", dataSourceUpdated);
        if (scope.tempConfig.datasource) {
            dataSourceUpdated(scope.tempConfig.datasource, null);
        }
    }

    var module = angular.module("our.umbraco.contentlist", []);

    module.controller("ContentListSettingController", [
        "$scope",
        function (scope) {
            scope.$watch("model.value",
                function(newVal, old) {
                    if (newVal !== old) {
                        scope.model.update(scope.model);
                    }
                });
        }
    ]);

    module.controller("our.umbraco.contentlist.editor.controller", [
        "$scope",
        "$http",
        "$q",
        "our.umbraco.contentlist.datasource.service",
        "our.umbraco.contentlist.templates.service",
        "our.umbraco.contentlist.defaultSettings",
        "editorState",
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
            "description": "How to get the content. Remember to set query parameters below.",
            "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/dropdown/dropdown.html",
            "mandatory": true,
            "config": {
                "items": [
                ]
            }
        },
        parameters: {
            "label": "Parameters",
            "key": "parameters",
            "description": "Data source parameters",
            "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/parameters/parameters.html",
            "config": {
                "parameterLoader": null, // set in controller
                "datasourceConfig": null // set in controller
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