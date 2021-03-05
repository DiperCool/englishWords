module PanelSelectLearning
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



    type Learning=
    | WrittingWords
    
    type State={
        ListWords: ListWordsDB.ListWords
    }
    type Msg=
    | SetListWords of ListWordsDB.ListWords
    | GoToWrittingWords of ListWordsDB.ListWords
    let init=
        {
            ListWords = WordsHelper.emptyListWord
        }, Cmd.none
    let update (msg: Msg) (state:State) =
        match msg with
        | SetListWords listWords -> {state with ListWords=listWords}, Cmd.none
        | GoToWrittingWords listWords-> state, Cmd.none
    let view state dispatch=
        StackPanel.create[
            StackPanel.children[
                Button.create[
                    Button.content "Writting Words"
                    Button.onClick (fun _ -> dispatch (GoToWrittingWords state.ListWords))
                ]
            ]
        ]
