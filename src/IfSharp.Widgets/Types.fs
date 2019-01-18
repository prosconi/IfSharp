namespace IfSharp.Widgets

type ButtonStyle =
    | Primary
    | Success
    | Info
    | Warning
    | Danger
    | Custom of string
    | NotSet
    static member All() =
        [|
            ButtonStyle.Primary
            ButtonStyle.Success
            ButtonStyle.Info
            ButtonStyle.Warning
            ButtonStyle.Danger
            ButtonStyle.NotSet
        |]

type BarStyle =
    | Success
    | Info
    | Warning
    | Danger
    | NotSet
    static member All() =
        [|
            BarStyle.Success
            BarStyle.Info
            BarStyle.Warning
            BarStyle.Danger
            BarStyle.NotSet
        |]

type Orientation =
    | Horizontal
    | Vertical
    | NotSet
    static member All() =
        [|
            Orientation.Horizontal
            Orientation.Vertical
        |]
