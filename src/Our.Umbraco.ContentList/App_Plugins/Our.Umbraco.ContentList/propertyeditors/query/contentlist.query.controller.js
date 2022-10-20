(function () {
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

  function keyNameToIdValue(obj) {
    return { id: obj.key, value: obj.name };
  }

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

  module.controller("our.umbraco.contentlist.query.editor.controller",
    [
      "$scope",
      "our.umbraco.contentlist.defaultSettings",
      "editorState",
      function (scope, defaultSettings, editorState) {

        console.log('current', editorState.current);

        scope.enableGui = editorState.current.udi.indexOf("umb://document/") === 0;
        if (!scope.enableGui) {
          return;
        }

        scope.model.value = scope.model.value || createConfig();
        scope.properties = Object.assign({}, defaultSettings);

        for (var key in scope.properties) {
          if (typeof (scope.model.value[key]) === 'boolean') {
            scope.properties[key].value = scope.model.value[key] ? "1" : "0";
          } else {
            scope.properties[key].value = scope.model.value[key];
          }
          scope.$watch("properties." + key + ".value",
            ((subKey) => (val) => {
              scope.model.value[subKey] = val;
            })(key));
        }

        scope.$watch("model.value.showPaging",
          function(val) {
            if (!(val instanceof Boolean)) {
              scope.model.value.showPaging = new Boolean(val === "1");
            }
          });
      }
    ]);

}());