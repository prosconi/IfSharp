namespace IfSharp.Widgets

open System
open System.ComponentModel
open IfSharp.Kernel
open Newtonsoft.Json

type Widget(modelName: string, viewName: string, ?modelModule, ?modelModuleVersion, ?viewModule, ?viewModuleVersion) as this =

    let domClasses = ResizeArray<_>()
    let ev = new Event<_,_>()
    let key = Guid.NewGuid()

    /// Sends an of the specified value to the UI
    let sendUpdate (propertyName, value) =
        App.Kernel |> Option.iter (fun k -> k.UpdateWidgetState(this, propertyName, value))

    member val comm_id = key
    member val _dom_classes          = ResizeArray<_>()                                   with get, set
    member val _model_module         = defaultArg modelModule "@jupyter-widgets/controls" with get, set
    member val _model_module_version = defaultArg modelModuleVersion "1.4.0"              with get, set
    member val _model_name           = modelName                                          with get, set
    member val _view_count           = null                                               with get, set
    member val _view_module          = defaultArg viewModule "@jupyter-widgets/base"      with get, set
    member val _view_module_version  = defaultArg viewModuleVersion "1.1.0"               with get, set
    member val _view_name            = viewName                                           with get, set

    /// Adds a CSS class
    member __.AddClass(className) = 
        match domClasses.Contains className with
        | true -> ()
        | false -> domClasses.Add className

    /// Removes a CSS class
    member __.RemoveClass(className) = 
        domClasses.Remove className

    /// When any property changes, send the update (see: PropertyChanged.Fody)
    member __.OnPropertyChanged(propertyName: string, before: obj, after: obj) =
        sendUpdate (propertyName, after)

    [<CLIEvent>]
    [<JsonIgnore>]
    member __.PropertyChanged = ev.Publish

    interface IWidget with

        /// The key for this widget
        member __.Key = key

        /// Dynamically gets the "parents" of this widget using reflection
        member this.GetParents() =
            this.GetType()
            |> lookupSerializationProperties<WidgetSerializer>
            |> Seq.map (fun prop -> prop.GetValue this)
            |> Seq.cast<IWidget>
            |> Seq.toArray

        /// Called by the kernel to tell this widget to open comms
        member this.OpenWidgetComm() =
            let name = string this.comm_id
            App.Kernel |> Option.iter (fun kernel -> 
                kernel.RegisterComm
                    (
                        name,
                        (fun msg data -> ()),
                        (fun msg data -> 
                            kernel.SendStateBusy()
                            match data.data.TryGetValue("state") with
                            | true, state ->
                                let json = JsonConvert.SerializeObject(state)
                                JsonConvert.PopulateObject(json, this)
                            | _ -> ()
                            kernel.SendStateIdle()
                        ),
                        (fun data -> ())
                    )
            )

    interface INotifyPropertyChanged with
        
        [<CLIEvent>]
        [<JsonIgnore>]
        member __.PropertyChanged = ev.Publish

/// Layout specification
/// Defines a layout that can be expressed using CSS.  Supports a subset of
/// https://developer.mozilla.org/en-US/docs/Web/CSS/Reference
/// When a property is also accessible via a shorthand property, we only
/// expose the shorthand.
/// For example:
/// - ``flex-grow``, ``flex-shrink`` and ``flex-basis`` are bound to ``flex``.
/// - ``flex-wrap`` and ``flex-direction`` are bound to ``flex-flow``.
/// - ``margin-[top/bottom/left/right]`` values are bound to ``margin``, etc.
type Layout() =
    inherit Widget
        (
            modelName = "LayoutModel",
            viewName = "LayoutView",
            modelModule = "@jupyter-widgets/base",
            modelModuleVersion = "1.1.0",
            viewModule = "@jupyter-widgets/base",
            viewModuleVersion = "1.1.0"
        )

    /// The align-content CSS attribute.
    member val align_content         : string = null with get, set// CaselessStrEnum(['flex-start', 'flex-end', 'center', 'space-between', 'space-around', 'space-evenly', 'stretch'] + CSS_PROPERTIES, allow_none=True, help="The align-content CSS attribute.").tag(sync=True)

    /// The align-items CSS attribute.
    member val align_items           : string = null with get, set// CaselessStrEnum(['flex-start', 'flex-end', 'center','baseline', 'stretch'] + CSS_PROPERTIES, allow_none=True, help="The align-items CSS attribute.").tag(sync=True)

    /// The align-self CSS attribute.
    member val align_self            : string = null with get, set// CaselessStrEnum(['auto', 'flex-start', 'flex-end','center', 'baseline', 'stretch'] + CSS_PROPERTIES, allow_none=True, help="The align-self CSS attribute.").tag(sync=True)
    member val bottom                : string = null with get, set// Unicode(None, allow_none=True, help="The bottom CSS attribute.").tag(sync=True)
    member val border                : string = null with get, set// Unicode(None, allow_none=True, help="The border CSS attribute.").tag(sync=True)
    member val display               : string = null with get, set// Unicode(None, allow_none=True, help="The display CSS attribute.").tag(sync=True)
    member val flex                  : string = null with get, set// Unicode(None, allow_none=True, help="The flex CSS attribute.").tag(sync=True)
    member val flex_flow             : string = null with get, set// Unicode(None, allow_none=True, help="The flex-flow CSS attribute.").tag(sync=True)
    member val height                : string = null with get, set// Unicode(None, allow_none=True, help="The height CSS attribute.").tag(sync=True)
    member val justify_content       : string = null with get, set// CaselessStrEnum(['flex-start', 'flex-end', 'center','space-between', 'space-around'] + CSS_PROPERTIES, allow_none=True, help="The justify-content CSS attribute.").tag(sync=True)
    member val left                  : string = null with get, set// Unicode(None, allow_none=True, help="The left CSS attribute.").tag(sync=True)
    member val margin                : string = null with get, set// Unicode(None, allow_none=True, help="The margin CSS attribute.").tag(sync=True)
    member val max_height            : string = null with get, set// Unicode(None, allow_none=True, help="The max-height CSS attribute.").tag(sync=True)
    member val max_width             : string = null with get, set// Unicode(None, allow_none=True, help="The max-width CSS attribute.").tag(sync=True)
    member val min_height            : string = null with get, set// Unicode(None, allow_none=True, help="The min-height CSS attribute.").tag(sync=True)
    member val min_width             : string = null with get, set// Unicode(None, allow_none=True, help="The min-width CSS attribute.").tag(sync=True)
    member val overflow              : string = null with get, set// CaselessStrEnum(['visible', 'hidden', 'scroll', 'auto'] + CSS_PROPERTIES, allow_none=True, help="The overflow CSS attribute.").tag(sync=True)
    member val overflow_x            : string = null with get, set// CaselessStrEnum(['visible', 'hidden', 'scroll', 'auto'] + CSS_PROPERTIES, allow_none=True, help="The overflow-x CSS attribute.").tag(sync=True)
    member val overflow_y            : string = null with get, set// CaselessStrEnum(['visible', 'hidden', 'scroll', 'auto'] + CSS_PROPERTIES, allow_none=True, help="The overflow-y CSS attribute.").tag(sync=True)
    member val order                 : string = null with get, set// Unicode(None, allow_none=True, help="The order CSS attribute.").tag(sync=True)
    member val padding               : string = null with get, set// Unicode(None, allow_none=True, help="The padding CSS attribute.").tag(sync=True)
    member val right                 : string = null with get, set// Unicode(None, allow_none=True, help="The right CSS attribute.").tag(sync=True)
    member val top                   : string = null with get, set// Unicode(None, allow_none=True, help="The top CSS attribute.").tag(sync=True)
    member val visibility            : string = null with get, set// CaselessStrEnum(['visible', 'hidden']+CSS_PROPERTIES, allow_none=True, help="The visibility CSS attribute.").tag(sync=True)
    member val width                 : string = null with get, set// Unicode(None, allow_none=True, help="The width CSS attribute.").tag(sync=True)
                                                     
    member val grid_auto_columns     : string = null with get, set// Unicode(None, allow_none=True, help="The grid-auto-columns CSS attribute.").tag(sync=True)
    member val grid_auto_flow        : string = null with get, set// CaselessStrEnum(['column','row','row dense','column dense']+ CSS_PROPERTIES, allow_none=True, help="The grid-auto-flow CSS attribute.").tag(sync=True)
    member val grid_auto_rows        : string = null with get, set// Unicode(None, allow_none=True, help="The grid-auto-rows CSS attribute.").tag(sync=True)
    member val grid_gap              : string = null with get, set// Unicode(None, allow_none=True, help="The grid-gap CSS attribute.").tag(sync=True)
    member val grid_template_rows    : string = null with get, set// Unicode(None, allow_none=True, help="The grid-template-rows CSS attribute.").tag(sync=True)
    member val grid_template_columns : string = null with get, set// Unicode(None, allow_none=True, help="The grid-template-columns CSS attribute.").tag(sync=True)
    member val grid_template_areas   : string = null with get, set// Unicode(None, allow_none=True, help="The grid-template-areas CSS attribute.").tag(sync=True)
    member val grid_row              : string = null with get, set// Unicode(None, allow_none=True, help="The grid-row CSS attribute.").tag(sync=True)
    member val grid_column           : string = null with get, set// Unicode(None, allow_none=True, help="The grid-column CSS attribute.").tag(sync=True)
    member val grid_area             : string = null with get, set// Unicode(None, allow_none=True, help="The grid-area CSS attribute.").tag(sync=True)

/// Description style widget.
type DescriptionStyle(?modelName) =
    inherit Widget
        (
            modelName = (defaultArg modelName "DescriptionStyleModel"),
            viewName = "StyleView",
            modelModule = "@jupyter-widgets/controls",
            modelModuleVersion = "1.4.0"
        )

    member val description_width: string = null with get, set // Unicode(help="Width of the description to the side of the control.").tag(sync=True)

/// Widget that can be inserted into the DOM
type DOMWidget(modelName, viewName, ?modelModule, ?modelModuleVersion, ?viewModule, ?viewModuleVersion) =
    inherit Widget
        (
            modelName = modelName,
            viewName = viewName,
            modelModule = defaultArg modelModule "@jupyter-widgets/controls",
            modelModuleVersion = defaultArg modelModuleVersion "1.4.0",
            viewModule = defaultArg viewModule "@jupyter-widgets/controls",
            viewModuleVersion = defaultArg viewModuleVersion "1.4.0"
        )

    member val description         = "" with get, set
    member val description_tooltip = "" with get, set

    [<JsonConverter(typeof<WidgetSerializer>)>]
    member val layout              = Layout() with get, set
    [<JsonConverter(typeof<WidgetSerializer>)>]
    member val style               = DescriptionStyle() with get, set

type ValueWidget<'t>(modelName, viewName, ?defaultValue) =
    inherit DOMWidget(modelName, viewName)
    member val value : 't = defaultArg defaultValue Unchecked.defaultof<'t> with get,set
