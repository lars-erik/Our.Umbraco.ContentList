
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

export default module;