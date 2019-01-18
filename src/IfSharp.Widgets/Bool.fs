namespace IfSharp.Widgets

open Newtonsoft.Json

/// Displays a boolean `value` in the form of a checkbox.
//    Parameters
//    ----------
//    value : {True,False}
//        value of the checkbox: True-checked, False-unchecked
//    description : str
//	    description displayed next to the checkbox
//    indent : {True,False}
//        indent the control to align with other controls with a description. The style.description_width attribute controls this width for consistence with other controls.
type Checkbox() =
    inherit ValueWidget<bool>(modelName = "CheckboxModel", viewName = "CheckboxView")
    member val disabled = false with get,set // Bool(False, help="Enable or disable user changes.").tag(sync=True)
    member val indent   = false with get,set // Bool(True, help="Indent the control to align with other controls with a description.").tag(sync=True)

/// Displays a boolean `value` in the form of a toggle button.
///     Parameters
///     ----------
///     value : {True,False}
///         value of the toggle button: True-pressed, False-unpressed
///     description : str
///       description displayed next to the button
///     tooltip: str
///         tooltip caption of the toggle button
///     icon: str
///         font-awesome icon name
type ToggleButton() =
    inherit ValueWidget<bool>(modelName = "ToggleButtonModel", viewName = "ToggleButtonView")
    
    member val value        = false  with get,set
    member val tooltip      = ""     with get,set
    member val icon         = ""     with get,set
    [<JsonConverter(typeof<ButtonStyleSerializer>)>]
    member val button_style = ButtonStyle.NotSet with get,set

/// Displays a boolean `value` in the form of a green check (True / valid)
//    or a red cross (False / invalid).
//    Parameters
//    ----------
//    value: {True,False}
//        value of the Valid widget
type Valid() =
    inherit ValueWidget<bool>(modelName = "ValidModel", viewName = "ValidView")
    member val disabled = false with get,set // Bool(False, help="Enable or disable user changes.").tag(sync=True)
    member val readout  = ""    with get,set // Unicode('Invalid', help="Message displayed when the value is False").tag(sync=True)
