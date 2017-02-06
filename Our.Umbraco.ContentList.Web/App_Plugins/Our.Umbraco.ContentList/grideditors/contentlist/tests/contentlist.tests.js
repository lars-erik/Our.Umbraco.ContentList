/// <reference path="../../../../../umbraco/lib/jquery/jquery.min.js" />
/// <reference path="../../../../../umbraco/lib/angular/1.1.5/angular.js" />
/// <reference path="../../../../../umbraco/lib/angular/1.1.5/angular-cookies.js" />
/// <reference path="../../../../../umbraco/lib/angular/1.1.5/angular-mocks.js" />
/// <reference path="../contentlist.js" />

(function() {
    "use strict";

    describe("contentlist editor controller", function () {

        var rootScope,
            scope,
            http,
            httpBackend,
            controllerFactory,
            datasourceService,
            templateService,
            previewUrl = "/umbraco/ourcontentlist/contentlist/preview",
            datasources = [
                {
                    name: "A datasouce",
                    key: "a.datasource",
                    parameters: [
                        { name: "a", key: "a", type: "text" }
                    ]
                },
                {
                    name: "Another datasource",
                    key: "another.datasource"
                }
            ],
            templates = ["Default", "With date"],
            requestHelper = {
                getApiUrl: function() {
                    return previewUrl;
                }
            };

        function createDatasourceService(q) {
            return {
                getDataSources: function() {
                    var def = q.defer();
                    def.resolve(datasources);
                    return def.promise;
                }
            }
        }

        function createTemplatesService(q) {
            return {
                getTemplates: function() {
                    var def = q.defer();
                    def.resolve(templates);
                    return def.promise;
                }
            }
        }

        function createController() {
            scope = rootScope.$new();
            return controllerFactory("our.umbraco.contentlist.editor.controller", {
                "$scope": scope,
                "$http": http,
                "our.umbraco.contentlist.datasource.service": datasourceService,
                "our.umbraco.contentlist.templates.service": templateService,
                "umbRequestHelper": requestHelper,
            });
        }

        beforeEach(function() {
            module("our.umbraco.contentlist");

            inject(function ($rootScope, $controller, $q, $httpBackend, $http) {
                rootScope = $rootScope;
                controllerFactory = $controller;
                datasourceService = createDatasourceService($q);
                templateService = createTemplatesService($q);
                http = $http;
                httpBackend = $httpBackend;
            });

            rootScope.control = { editor: { config: {} }, value: null };
            rootScope.content = { id: 1 };

            createController();
        });

        it("assigns model to inherited control reference", function () {
            // TODO: Flytt til egen mapping controller
            expect(scope.model).toBe(rootScope.control);
        });

        it("assigns an empty config object to model.value if empty", function() {
            expect(rootScope.control.config).toEqual({
                datasource: "",
                view: "default",
                pagesize: 10,
                columns: {small: 1, medium: 2, large: 2},
                parameters: [],
                showPaging: false
            });
        });

        it("gets datasources from service", function () {
            scope.$digest();
            expect(scope.datasources).toBe(datasources);
        });

        it("gets templates from template service", function() {
            scope.$digest();
            expect(scope.templates).toBe(templates);
        });

        it("sets scope parameters when value datasource changes", function() {
            httpBackend.whenPOST(previewUrl).respond("<div/>");
            scope.$digest();
            scope.control.config.datasource = "a.datasource";
            scope.$digest();
            expect(scope.parameters).toBe(datasources[0].parameters);
        });

        it("starts with 'please select data source' as message", function () {
            scope.$digest();
            expect(scope.emptyMessage).toBe("Please select data source");
        });

        xit("calls the preview service with the given parameters", function () {
            httpBackend.whenPOST(previewUrl, { "datasource": "a.datasource", "pagesize": 10, "showPaging": false, "parameters": [], "contentId": 1 }).respond("<div/>");
            httpBackend.whenPOST(previewUrl, { "datasource": "a.datasource", "pagesize": 10, "showPaging": false, "parameters": [{ "key": "a" }], "contentId": 1 }).respond("<h1>It's a win</h1>");
            scope.control.config.datasource = "a.datasource";
            scope.$digest();
            httpBackend.flush();
            expect(scope.preview).toBe("<h1>It's a win</h1>");
        });
    });

}());