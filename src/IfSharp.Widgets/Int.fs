namespace IfSharp.Widgets

open Newtonsoft.Json

/// Textbox widget that represents an integer.
type IntText() =
    inherit ValueWidget<int>(modelName = "IntTextModel", viewName = "IntTextView")
    member val disabled          = false with get,set
    member val continuous_update = true  with get,set
    member val step              = 1     with get,set

/// Textbox widget that represents an integer bounded from above and below.
type BoundedIntText() =
    inherit ValueWidget<int>(modelName = "BoundedIntTextModel", viewName = "IntTextView")
    member val step              = 1     with get,set
    member val min               = 0     with get,set
    member val max               = 100   with get,set
    member val disabled          = false with get,set
    member val continuous_update = true  with get,set

/// Button style widget.
type SliderStyle() =
    inherit DescriptionStyle("SliderStyleModel")
    member val handle_color = "" with get,set

/// Slider widget that represents an integer bounded from above and below.
type IntSlider() =
    inherit ValueWidget<int>(modelName = "IntSliderModel", viewName = "IntSliderView")
    member val step              = 1             with get,set
    member val orientation       = "horizontal"  with get,set
    member val readout           = true          with get,set
    member val readout_format    = "d"           with get,set
    member val min               = 0             with get,set
    member val max               = 100           with get,set
    member val disabled          = false         with get,set
    member val continuous_update = true          with get,set
    [<JsonConverter(typeof<WidgetSerializer>)>]
    member val style             = SliderStyle() with get,set

/// Button style widget.
type ProgressStyle() =
    inherit DescriptionStyle("ProgressStyleModel")
    member val bar_color = "" with get,set

/// Progress bar that represents an integer bounded from above and below.
type IntProgress() =
    inherit ValueWidget<int>(modelName = "IntProgressModel", viewName = "ProgressView")
    [<JsonConverter(typeof<OrientationSerializer>)>]
    member val orientation       = Orientation.Horizontal with get,set
    [<JsonConverter(typeof<BarStyleSerializer>)>]
    member val bar_style         = BarStyle.NotSet        with get,set
    [<JsonConverter(typeof<WidgetSerializer>)>]
    member val style             = ProgressStyle()        with get,set

/// Play/repeat buttons to step through values automatically, and optionally loop.
type Play() =
    inherit ValueWidget<int>(modelName = "PlayModel", viewName = "PlayView")
    member val interval    = 100   with get,set
    member val step        = 1     with get,set
    member val disabled    = false with get,set
    member val _playing    = false with get,set
    member val _repeat     = false with get,set
    member val show_repeat = true  with get,set
    
/// Slider/trackbar that represents a pair of ints bounded by minimum and maximum value.
/// Parameters
/// ----------
/// value : int tuple
///     The pair (`lower`, `upper`) of integers
/// min : int
///     The lowest allowed value for `lower`
/// max : int
///     The highest allowed value for `upper`
type IntRangeSlider() =
    inherit ValueWidget<int[]>(modelName = "IntRangeSliderModel", viewName = "IntRangeSliderView", defaultValue = [| 10; 20 |])
    member val step               = 1                      with get,set
    member val min                = 0                      with get,set
    member val max                = 100                    with get,set
    [<JsonConverter(typeof<OrientationSerializer>)>]
    member val orientation        = Orientation.Horizontal with get,set
    member val readout            = true                   with get,set
    member val readout_format     = "d"                    with get,set
    member val continuous_update  = true                   with get,set
    [<JsonConverter(typeof<WidgetSerializer>)>]
    member val style              = SliderStyle()          with get, set
    member val disabled           = false                  with get,set
