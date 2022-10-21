import module from "./module";

function createTemplatesService(q, http, requestHelper) {
  return {
    getTemplates: function () {
      var url = requestHelper.getApiUrl("Our.Umbraco.ContentList.Controllers.ContentListApi", "GetTemplates"),
        def = q.defer();

      http.get(url)
        .then(function (response) {
          def.resolve(response.data);
        });

      return def.promise;
    }
  };
}

module.factory("our.umbraco.contentlist.templates.service", [
  "$q",
  "$http",
  "umbRequestHelper",
  createTemplatesService
]);

