namespace IfSharp.Widgets

/// Renders the string `value` as HTML.
type Html(?value) =
    inherit ValueWidget<string>(modelName = "HTMLModel", viewName = "HTMLView", defaultValue = defaultArg value "")

/// Renders the string `value` as HTML, and render mathematics.
type HtmlMath(?value) =
    inherit ValueWidget<string>(modelName = "HTMLMathModel", viewName = "HTMLMathView", defaultValue = defaultArg value "")

/// Label widget.
/// It also renders math inside the string `value` as Latex (requires $ $ or
/// $$ $$ and similar latex tags).
type Label(?value) =
    inherit ValueWidget<string>(modelName = "LabelModel", viewName = "LabelView", defaultValue = defaultArg value "")

/// Multiline text area widget.
type Textarea(?value) =
    inherit ValueWidget<string>(modelName = "TextareaModel", viewName = "TextareaView", defaultValue = defaultArg value "")
    member val rows = 10 with get,set
    member val disabled = false with get,set
    member val continuous_update = true with get,set

/// Single line textbox widget.
type Text(?value) =
    inherit ValueWidget<string>(modelName = "TextModel", viewName = "TextView", defaultValue = defaultArg value "")
    member val disabled = false with get,set
    member val continuous_update = true with get,set

/// Single line textbox widget.
type Password(?value) =
    inherit ValueWidget<string>(modelName = "PasswordModel", viewName = "PasswordView", defaultValue = defaultArg value "")
    member val disabled = false with get,set
