module Word
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
    type State={
        ListWord: ListWordsDB.ListWords
        Words: WordDB.Word list
        Value: string
        Translate: string
    }
    let emptyListWord: ListWordsDB.ListWords= {
        id=0;
        Name="";
        TimeToRepiting= System.DateTime.Now;
        Created = System.DateTime.Now;
    }

    let init = 
        {
            ListWord = emptyListWord;
            Words = List.empty
            Value =""
            Translate = ""
        }, Cmd.none
    type Msg =
    | LoadFromDB
    | SetListWord of ListWordsDB.ListWords
    | Create
    | NewValue of string
    | NewTranslate of string

    let update (msg : Msg) (state: State) : State*Cmd<_>=
        match msg with
        | LoadFromDB ->
            {state with Words=WordDB.getWords state.ListWord.id},Cmd.none
        | SetListWord listWord ->
            {state with ListWord = listWord}, Cmd.ofMsg LoadFromDB
        | Create ->
            WordDB.createWord {id=0; Value=state.Value; Translate = state.Translate; Created= System.DateTime.Now; idListWords= state.ListWord.id} |> ignore
            state, Cmd.ofMsg LoadFromDB
        | NewValue str -> {state with Value = str }, Cmd.none
        | NewTranslate str -> {state with Translate = str}, Cmd.none
    let viewWords (state: State) (dispatch) : Types.IView list=
        List.ofSeq (seq {
            for i = 0 to state.Words.Length-1 do
                yield (
                        TextBlock.create[
                            TextBlock.fontSize(18.)
                            TextBlock.margin(5.0, 5.0)
                            Grid.column(0)
                            Grid.row(i)
                            TextBlock.text(state.Words.[i].Value)
                        ]
                )
                yield(
                        TextBlock.create[
                            TextBlock.fontSize(18.)
                            TextBlock.margin(5.0, 5.0)
                            Grid.column(2)
                            Grid.row(i)
                            TextBlock.text(state.Words.[i].Translate)
                            
                        ]
                )
                yield (
                    Separator.create[
                        Separator.height 1.
                        Separator.background "silver"
                    ]
                )
        })
    let view (state: State) (dispatch) =
        StackPanel.create[
            StackPanel.children[
                TextBlock.create[
                    TextBlock.text state.ListWord.Name
                ]
                TextBox.create[
                    TextBox.onTextChanged(fun str -> (dispatch (NewValue(str))) )
                    TextBox.watermark "Enter a word"
                ]
                TextBox.create[
                    TextBox.onTextChanged(fun str -> (dispatch (NewTranslate(str))) )
                    TextBox.watermark "Enter a translate"
                ]
                Button.create[
                    Button.isEnabled (((not (isNull state.Value)) && state.Value.Length <> 0)&& ((not (isNull state.Translate)) && state.Translate.Length <> 0))
                    Button.onClick (fun _ -> (dispatch Create))
                    Button.content "Add"
                ]
                ListBox.create[
                    ListBox.viewItems(viewWords state dispatch)
                ]
            ]
        ]