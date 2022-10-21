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

}());