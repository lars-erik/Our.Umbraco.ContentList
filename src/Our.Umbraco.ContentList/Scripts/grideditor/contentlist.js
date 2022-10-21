import module from "./../common/module";

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
    dsState.all = scope.datasources;
  }

  function templatesLoaded(templates) {
    scope.templates = templates;
    scope.control.editor.config.settings.view.config.items = $.map(scope.templates, function (t) { return { id: t.name, value: t.displayName || t.name }; });
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
    // TODO: Figure way to validate parameters

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

  scope.trustedHtml = function (plainText) {
    return sce.trustAsHtml(plainText);
  };

  function main() {
    if (!scope.control.editor.config.loadStarted) {
      scope.control.editor.config.loadStarted = true;
      var p1 = datasourceService.getDataSources().then(datasourcesLoaded),
        p2 = templatesService.getTemplates().then(templatesLoaded);
      q.when(p1, p2).then(function () {
        scope.$watch("model.config", loadPreviewIfValid, true);
      });
    } else {
      scope.$watch("model.config", loadPreviewIfValid, true);
    }

    scope.model.config.showPaging = scope.model.config.showPaging ? "1" : "0";
    scope.$watch("model.config.showPaging",
      function (val) {
        if (!(val instanceof Boolean)) {
          scope.model.config.showPaging = new Boolean(val === "1");
        }
      });

  }

  initialize();
  main();
}

module.controller("our.umbraco.contentlist.editor.controller", [
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

module.run([
  "$templateCache",
  function (cache) {

    cache.put("our.umbraco.contentlist.dropdownlist.html", "<select ng-model=\"parameter.value\" ng-options=\"opt.key as opt.name for opt in parameter.options\"></select>");
    cache.put("our.umbraco.contentlist.textbox.html", "<input type=\"text\" ng-model=\"parameter.value\" />");

  }]);

