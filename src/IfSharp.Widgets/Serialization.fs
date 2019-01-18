namespace IfSharp.Widgets

open System
open Newtonsoft.Json
open IfSharp.Kernel

/// A serializer that only supports writing instances of IWidget such that the notebook
/// can link the values together in the UI
type WidgetSerializer() =
    inherit JsonConverter()

    override __.WriteJson(writer, value, _serializer) = 
        let isIWidget = typeof<IWidget>.IsAssignableFrom(value.GetType())
        let isIWidgetArray = typeof<IWidget[]>.IsAssignableFrom(value.GetType())
        if isIWidget then
            let w = value :?> IWidget
            writer.WriteValue(w.Key |> string |> sprintf "IPY_MODEL_%s")
        elif isIWidgetArray then
            let w = value :?> IWidget[]
            writer.WriteStartArray()
            w |> Array.iter (fun w -> writer.WriteValue(w.Key |> string |> sprintf "IPY_MODEL_%s"))
            writer.WriteEndArray()
        else
            value.GetType().FullName |> failwithf "Unsupported type: %s. Expected: IWidget"

    override __.ReadJson(_reader, _objectType, _existingValue, _serializer) = raise(NotSupportedException())
    override __.CanConvert(_objectType) = false

type ButtonStyleSerializer() = 
    inherit JsonConverter()

    override __.WriteJson(writer, value, _serializer) = 
        match value with
        | :? ButtonStyle as w -> 
            let styleString = 
                match w with
                | ButtonStyle.Primary    -> "primary"
                | ButtonStyle.Success    -> "success"
                | ButtonStyle.Info       -> "info"
                | ButtonStyle.Warning    -> "warning"
                | ButtonStyle.Danger     -> "danger"
                | ButtonStyle.NotSet     -> ""
                | ButtonStyle.Custom str -> str
            writer.WriteValue(styleString)
        | _ -> 
            value.GetType().FullName |> failwithf "Unsupported type: %s. Expected: ButtonStyle"

    override __.ReadJson(_reader, _objectType, existingValue, _serializer) = 
        match (existingValue |> string).ToLowerInvariant() with
        | "primary" -> ButtonStyle.Primary
        | "success" -> ButtonStyle.Success
        | "info"    -> ButtonStyle.Info
        | "warning" -> ButtonStyle.Warning
        | "danger"  -> ButtonStyle.Danger
        | ""        -> ButtonStyle.NotSet
        | other     -> ButtonStyle.Custom other
        |> box

    override __.CanConvert(_objectType) = false

type BarStyleSerializer() = 
    inherit JsonConverter()

    override __.WriteJson(writer, value, _serializer) = 
        match value with
        | :? BarStyle as w -> 
            let styleString = 
                match w with
                | BarStyle.Success    -> "success"
                | BarStyle.Info       -> "info"
                | BarStyle.Warning    -> "warning"
                | BarStyle.Danger     -> "danger"
                | BarStyle.NotSet     -> ""
            writer.WriteValue(styleString)
        | _ -> 
            value.GetType().FullName |> failwithf "Unsupported type: %s. Expected: BarStyle"

    override __.ReadJson(_reader, _objectType, existingValue, _serializer) = 
        match (existingValue |> string).ToLowerInvariant() with
        | "success"  -> BarStyle.Success
        | "info"     -> BarStyle.Info
        | "warning"  -> BarStyle.Warning
        | "danger"   -> BarStyle.Danger
        | _otherwise -> BarStyle.NotSet
        |> box

    override __.CanConvert(_objectType) = false

type OrientationSerializer() = 
    inherit JsonConverter()

    override __.WriteJson(writer, value, _serializer) = 
        match value with
        | :? Orientation as w -> 
            let styleString = 
                match w with
                | Orientation.Horizontal -> "horizontal"
                | Orientation.Vertical   -> "vertical"
                | Orientation.NotSet     -> ""
            writer.WriteValue(styleString)
        | _ -> 
            value.GetType().FullName |> failwithf "Unsupported type: %s. Expected: Orientation"

    override __.ReadJson(_reader, _objectType, existingValue, _serializer) = 
        match (existingValue |> string).ToLowerInvariant() with
        | "horizontal" -> Orientation.Horizontal
        | "vertical"   -> Orientation.Vertical
        | _otherwise   -> Orientation.NotSet
        |> box

    override __.CanConvert(_objectType) = false
