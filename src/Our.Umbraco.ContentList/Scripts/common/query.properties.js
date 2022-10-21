import module from "./module";

module.value("our.umbraco.contentlist.defaultSettings", {
  datasource: {
    "label": "Data source",
    "key": "datasource",
    "description": "How to get the content.",
    "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/datasource/blge-datasource.html",
    "hideLabel": true,
    "config": {
      "items": [
      ]
    }
  },
  view: {
    "label": "Theme",
    "key": "view",
    "description": "How the list should look",
    "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/theme/blge-theme.html",
    "config": {
      "items": [
      ],
      "all": []
    }
  },
  columns: {
    "label": "Columns",
    "key": "columns",
    "description": "Columns per screen size (advanced)",
    "view": "/app_plugins/our.umbraco.contentlist/propertyeditors/columns/columns.html",
    "config": {
      "min": 1,
      "max": 100
    }
  },
  pagesize: {
    "label": "Page Size",
    "key": "pagesize",
    "description": "Count of items per page",
    "view": "/umbraco/views/propertyeditors/integer/integer.html",
    "config": {
      "min": 1,
      "max": 100
    }
  },
  skip: {
    "label": "Skip items",
    "key": "skip",
    "description": "Enter a number of items to skip.",
    "view": "/umbraco/views/propertyeditors/integer/integer.html",
  },
  showPaging: {
    "label": "Show paging",
    "key": "showPaging",
    "description": "",
    "view": "/umbraco/views/propertyeditors/boolean/boolean.html"
  }
});
