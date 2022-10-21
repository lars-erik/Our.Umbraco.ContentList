import module from "./module";

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