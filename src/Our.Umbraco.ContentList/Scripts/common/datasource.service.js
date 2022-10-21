import module from "./module";

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

