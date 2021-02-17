[
    {
        "name": "Rich text editor",
        "alias": "rte",
        "view": "rte",
        "icon": "icon-article"
    },
    {
        "name": "Image",
        "nameTemplate": "{{ value && value.udi ? (value.udi | ncNodeName) : '' }}",
        "alias": "media",
        "view": "media",
        "icon": "icon-picture"
    },
    {
        "name": "Macro",
        "nameTemplate": "{{ value && value.macroAlias ? value.macroAlias : '' }}",
        "alias": "macro",
        "view": "macro",
        "icon": "icon-settings-alt"
    },
    {
        "name": "Embed",
        "alias": "embed",
        "view": "embed",
        "icon": "icon-movie-alt"
    },
    {
        "name": "Headline",
        "nameTemplate": "{{ value }}",
        "alias": "headline",
        "view": "textstring",
        "icon": "icon-coin",
        "config": {
            "style": "font-size: 36px; line-height: 45px; font-weight: bold",
            "markup": "<h1>#value#</h1>",
            "settings": [
                {
                    "key": "color",
                    "label": "Color",
                    "view": "textstring"
                }
            ]
        }
    },
    {
        "name": "Quote",
        "nameTemplate": "{{ value ? value.substring(0,32) + (value.length > 32 ? '...' : '') : '' }}",
        "alias": "quote",
        "view": "textstring",
        "icon": "icon-quote",
        "config": {
            "style": "border-left: 3px solid #ccc; padding: 10px; color: #ccc; font-family: serif; font-style: italic; font-size: 18px",
            "markup": "<blockquote>#value#</blockquote>"
        }
    },
    {
        "name": "Searchbox",
        "alias": "searchbox",
        "view": "textstring",
        "icon": "icon-search",
        "config": {
            "style": "border-left: 3px solid #ccc; padding: 10px; color: #ccc; font-family: serif; font-style: italic; font-size: 18px",
            "markup": "<form method=\"get\"><input name=\"#config.name#\" placeholder=\"#value#\"><button>S�k</button></form>",
            "settings": {
                "name": {
                    "key": "name",
                    "label": "Parameter name",
                    "view": "textstring"
                }
            }
        }
    }
]
