(function () {
    "use strict";

    var parameterViews = {
        "dropdownlist": "our.umbraco.contentlist.dropdownlist.html",
        "textbox": "our.umbraco.contentlist.textbox.html"
    };

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

    function ContentListEditorController(scope, http, q, datasourceService, templatesService, defaultSettings, requestHelper) {
        var model,
            content,
            settings,
            parameterLoader = {};

        function initialize() {
            scope.preview = null;
            scope.datasources = [];
            model = scope.model = scope.control;
            model.config = $.extend(createConfig(), model.value, model.config);

            if (typeof (model.config.columns) !== "object")
                model.config.columns = {};

            content = findScopeValue(scope, "content");

            settings = $.extend({}, defaultSettings);
            settings.parameters = $.extend(settings.parameters, {
                "config": {
                    "parameterLoader": parameterLoader,
                    "parentConfig": model.config
                },
                value: []
            });

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
                settings.parameters.value = [];
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
                return function (param) {
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
            return true;
        }

        function loadPreviewIfValid() {
            if (isValid()) {
                var params = $.extend({}, model.config, { contentId: content.id });
                http.post("/umbraco/ourcontentlist/contentlist/preview", params).then(previewLoaded);
            }
        }

        function parameterView(parameter) {
            return parameterViews[parameter.type];
        }

        function main() {
            var p1 = datasourceService.getDataSources().then(datasourcesLoaded),
                p2 = templatesService.getTemplates().then(templatesLoaded);
            scope.$watch("model.config.datasource", initializeParametersWhenDataSourceChange);
            scope.$watch("model.config", loadPreviewIfValid, true);
            scope.$watch("parameters", updateValueParameters, true);
            scope.parameterView = parameterView;

            parameterLoader.update = getParametersForDataSource;

            q.all(p1, p2).then(function () {
                if (!model.config.datasource) {
                    scope.nextSetting();
                }
            });
        }

        var wizardSettings = [
            { message: "Select a data source", settingName: "datasource", predicate: function (val) { return val !== ""; } },
            {
                message: "Select parameters", settingName: "parameters", predicate: function(val) {
                    for (var i = 0; i < val.length; i++) {
                        if (!val[i].value) {
                            return false;
                        }
                    }
                    return true;
                }
                
            },
            { message: "Select theme", settingName: "view", predicate: function (val) { return val !== "" } }
        ];
        scope.wizardStep = -1;

        function setSetting() {
            var wizardStep = scope.wizardStep;
            var stepSetting = wizardSettings[wizardStep];
            scope.wizardMessage = stepSetting.message;
            scope.wizardSettingName = stepSetting.settingName;
            scope.wizardSetting = settings[scope.wizardSettingName];
            scope.wizardPredicate = stepSetting.predicate;
            scope.showPrevious = scope.wizardStep > 0;
        }

        scope.nextSetting = function () {
            scope.wizardStep++;
            setSetting();
        }

        scope.previousSetting = function () {
            scope.wizardStep--;
            setSetting();
        }

        scope.$on("contentlist.nextSetting", function (evt, evtArg) {
            scope.control.config[scope.wizardSettingName] = evtArg.value;
            scope.nextSetting();
        });

        scope.$on("contentlist.previousSetting", function (evt, evtArg) {
            scope.previousSetting();
        });

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

        var unwatchers = [],
            parentConfig = scope.model.config.parentConfig;

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

        scope.parentConfig = parentConfig;

        scope.canQuery = function () {
            for (var i = 0; i<scope.model.value.length; i++) {
                if (!scope.model.value[i].value) {
                    return false;
                }
            }
            return true;
        }

        function count() {
            if (scope.canQuery()) {
                var params = $.extend({}, parentConfig, {parameters:scope.model.value});
                return http.post("/umbraco/ourcontentlist/contentlist/count", params);
            }
            return null;
        }

        scope.$watch("model.value", function(newVal, oldVal) {
            if (oldVal !== newVal) {
                var promise = count(),
                    start = new Date();
                if (promise) {
                    promise.then(function (response) {
                        var end = new Date();
                        scope.queryCount = response.data;
                        scope.queryTime = end - start;
                    });
                }
            }
        }, true);

        scope.$watch("parentConfig.datasource",
            function (newValue, oldValue) {
                scope.model.config.parameters = scope.model.config.parameterLoader.update(newValue, oldValue);
                resetProperties();
            });
    }

    function createWizardDirective() {
        return {
            restrict: "E",
            scope: {
                message: "=",
                model: "=",
                showprev: "=",
                predicate: "="
            },
            template: '<div class="umb-cell-placeholder" ng-if="!preview" style="text-align:center; cursor: default; position: relative;">' +
                      '<div class="cell-tools-add -center" style="position:relative;top:0;left:0;width:50%;display:inline-block;margin:20px;transform:none;">' +
                      '<div>{{message}}</div>' +
                      '<div ng-include="model.view"></div>' +
                      '<button type=button style="' +
                      'border: 0px none; background-color: #2e8aea; color: white; border-radius:22px; ' +
                      'width:82px; height:44px; padding: 7px 14px; margin-top:5px; white-space:nowrap;"' +
                      ' ng-show="showprev" ng-click="back()">' +
                      '<span class="icon icon-arrow-left" style="float:left; font-size: 20px; line-height: 30px;"></span>' +
                      '<span style="float:left; line-height: 30px;">Back</span>' +
                      '</button>' +
                      '<button type=button style="' +
                      'border: 0px none; background-color: #2e8aea; color: white; border-radius:22px; ' +
                      'width:82px; height:44px; padding: 7px 14px; margin-top:5px; white-space:nowrap;"' +
                      ' ng-show="predicate(model.value)" ng-click="nextSetting()">' +
                      '<span class="icon icon-arrow-right" style="float:left; font-size: 20px; line-height: 30px;"></span>' +
                      '<span style="float:left; line-height: 30px;">Next</span>' +
                      '</button>' +
                      '</div>' +
                      '</div>',
            link: function (scope, element, attrs) {
                scope.$watch("model", function () {
                    var mdl = scope.model;
                });

                scope.nextSetting = function () {
                    scope.$emit("contentlist.nextSetting", { value: scope.model.value });
                }

                scope.back = function () {
                    scope.$emit("contentlist.previousSetting", { value: scope.model.value });
                }
            },
            controller: function($scope) {
                
            }
        }
    }

    var module = angular.module("our.umbraco.contentlist", []);

    module.controller("our.umbraco.contentlist.editor.controller", [
        "$scope",
        "$http",
        "$q",
        "our.umbraco.contentlist.datasource.service",
        "our.umbraco.contentlist.templates.service",
        "our.umbraco.contentlist.defaultSettings",
        "umbRequestHelper",
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

    module.directive("contentListWizard", createWizardDirective);

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
        }
    });

}());