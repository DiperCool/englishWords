module ListWords
    open WordsEnglish.Core
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Elmish
    open Avalonia.Layout
    open Avalonia.Media

    type State = {
        ListsWords : ListWordsDB.ListWords list;
        TextNewListWords : string;
        Page: int
    }
    type Msg=
        | LoadFromDb
        | CreateListWords
        | UdateTextNewListWords of string
        | ViewWord of ListWordsDB.ListWords
        | NextPage
        | PrevPage

    let init = 
        {
            ListsWords= List.empty;
            TextNewListWords= "";
            Page = 1
        }, Cmd.ofMsg LoadFromDb

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | LoadFromDb ->
            let res =ListWordsDB.getListWords state.Page
            {state with ListsWords= res}, Cmd.none
        | CreateListWords ->
            ListWordsDB.createListWords {
                id=0;
                Name= state.TextNewListWords;
                TimeToRepiting= System.DateTime.Now;
                Created= System.DateTime.Now;
            } |> ignore
            state, Cmd.ofMsg LoadFromDb
        | UdateTextNewListWords (str) -> {state with TextNewListWords = str}, Cmd.none
        | ViewWord listWords -> state, Cmd.none
        | NextPage -> {state with Page = state.Page+1}, Cmd.ofMsg LoadFromDb
        | PrevPage -> {state with Page = state.Page-1}, Cmd.ofMsg LoadFromDb
    let viewListWords (notes) dispatch : Types.IView list=
        [for item in notes |> List.ofSeq  do
            yield (
                    TextBlock.create[
                        TextBlock.fontSize(18.)
                        TextBlock.margin(5.0, 5.0)
                        TextBlock.onTapped ((fun _ -> 
                                          dispatch (ViewWord item)
                                        //гандонище
                                        //   |
                                        //   |
                                        //   |
                                        //  \ /
                                        ), Always)
                        TextBlock.text(item.Name)
                    ]
            )]

           
    let view (state: State) (dispatch) =
        StackPanel.create[
            StackPanel.children[
                TextBox.create[
                    TextBox.dock Dock.Top
                    TextBox.onTextChanged(fun str -> (dispatch (UdateTextNewListWords(str))) )
                ]
                Button.create[
                    Button.content "Create"
                    Button.isEnabled ((not (isNull state.TextNewListWords)) && state.TextNewListWords.Length <> 0)
                    Button.onClick(fun _ -> dispatch(CreateListWords))
                ]
                ListBox.create[
                    ListBox.viewItems(viewListWords state.ListsWords dispatch)
                ]
                Button.create[
                    Button.content "Prev"
                    Button.onClick(fun _ -> dispatch(PrevPage))
                ]
                Button.create[
                    Button.content "Next"
                    Button.onClick(fun _ -> dispatch(NextPage))
                ]
            ]
        ]