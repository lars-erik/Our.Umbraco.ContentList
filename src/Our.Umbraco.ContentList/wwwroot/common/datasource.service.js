(function() {

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

  function createDataSourceService(q, http, requestHelper) {
    var data = null;

    return {
      getDataSources: function () {
        var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Controllers.ContentListApi", "GetDataSources"),
          def = q.defer();
        if (data) {
          def.resolve(data);
        } else {
          http.get(url)
            .then(function (response) {
              data = response.data;
              def.resolve(response.data);
            });
        }
        return def.promise;
      }
    };
  }

  module.factory("our.umbraco.contentlist.datasource.service", [
    "$q",
    "$http",
    "umbRequestHelper",
    createDataSourceService
  ]);

}());