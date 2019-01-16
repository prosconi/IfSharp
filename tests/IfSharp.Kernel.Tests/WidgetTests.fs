module WidgetTests

open Xunit
open IfSharp.Widgets
open IfSharp.Kernel
open Newtonsoft.Json
open System.IO

let getParents (w: #IWidget) = w.GetParents()
let getKey (w: #IWidget) = w.Key

[<Fact>]
let propertyChangeEventsShouldWork() =
    let propertiesChanged = ResizeArray()

    let w = Html()
    w.PropertyChanged.Add(fun x -> propertiesChanged.Add x.PropertyName)
    w.description <- "description"

    Assert.Equal(1, propertiesChanged.Count)
    Assert.Equal("description", propertiesChanged |> Seq.head)

[<Fact>]
let selectionContainerFormalConstructorShouldWork() =
    let children =
        [|
            "Html1", Html() :> IWidget
            "Html2", Html() :> IWidget
        |]

    let container = SelectionContainer("modelName", "viewName", children)
    Assert.Equal(2, container.children.Length)
    Assert.Equal(2, container._titles.Count)
    Assert.Equal<_[]>([| children.[0] |> snd; children.[1] |> snd |], container.children)
    Assert.Equal<_[]>([| children.[0] |> fst; children.[1] |> fst |], container._titles.Values |> Seq.toArray)
   
[<Fact>]
let getParentsShouldWork() =
    let dw = DOMWidget("modelName", "viewName")
    let parents = getParents dw
    Assert.Equal(2, parents.Length)
    Assert.Equal(dw.layout, parents.[0] :?> Layout)
    Assert.Equal(dw.style, parents.[1] :?> DescriptionStyle)

[<Fact>]
let widgetSerializerShoulWriteWidget() =
    use sw = new StringWriter()
    use jw = new JsonTextWriter(sw)
    let serializer = JsonSerializer()
    let ws = WidgetSerializer()
    let widget = Html()
    ws.WriteJson(jw, widget, serializer)
    
    let actual = sw.ToString()
    Assert.Contains("IPY_MODEL", actual)
    Assert.Contains(widget |> getKey |> string, actual)

[<Fact>]
let widgetSerializerShouldWriteWidgetArray() =
    use sw = new StringWriter()
    use jw = new JsonTextWriter(sw)
    let serializer = JsonSerializer()
    let ws = WidgetSerializer()
    let widgets = 
        [|
            Html() :> IWidget
            VBox() :> IWidget
            HBox() :> IWidget
        |]

    ws.WriteJson(jw, widgets, serializer)

    let actual = JsonConvert.DeserializeObject<string[]>(sw.ToString())
    Assert.Equal(3, actual.Length)
    Assert.Contains("IPY_MODEL", actual.[0])
    Assert.Contains("IPY_MODEL", actual.[1])
    Assert.Contains("IPY_MODEL", actual.[2])
    Assert.Contains(widgets.[0] |> getKey |> string, actual.[0])
    Assert.Contains(widgets.[1] |> getKey |> string, actual.[1])
    Assert.Contains(widgets.[2] |> getKey |> string, actual.[2])
