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

  var currentDataSource = new (function () {
    this.ds = {};
  })();

  var currentTemplate = new (function () {
    this.template = {};
  })();

  module.factory("our.umbraco.contentlist.currentDatasource", [
    function () {
      return currentDataSource;
    }
  ]);

  module.factory("our.umbraco.contentlist.currentTemplate", [
    function () {
      return currentTemplate;
    }
  ]);

}());