[<AutoOpen>]
module IfSharp.Widgets.Internals

open System
open System.Collections.Generic
open Newtonsoft.Json
open IfSharp.Kernel

/// Creates a function that caches the results when called
let internal memoize f =
    let cache = Dictionary<_, _>()
    fun x ->
        if cache.ContainsKey(x) then
            cache.[x]
        else
            let res = f x
            cache.[x] <- res
            res

/// A memoized function that takes in a Type and looks for all properties that 
/// return an IWidget that is decorated by an JsonConverterAttribute that uses
/// the WidgetSerializer
let internal lookupSerializationProperties<'Serializer> = memoize (fun (t: Type) -> 
    t.GetProperties()
    |> Array.choose (fun prop -> 
        let isIWidget = typeof<IWidget>.IsAssignableFrom(prop.PropertyType)
        match isIWidget with
        | true ->
            match prop.GetCustomAttributes(typeof<JsonConverterAttribute>, false) with
            | [| jc |] -> 
                let jc = jc :?> JsonConverterAttribute
                match jc.ConverterType = typeof<'Serializer> with
                | true -> Some prop
                | _ -> None
            | _ -> None
        | _ -> None
    )
)
