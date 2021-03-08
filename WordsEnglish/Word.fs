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
    open System.Threading
    type State={
        ListWord: ListWordsDB.ListWords
        Words: WordDB.Word list
        Value: string
        Translate: string
        CancelToken : CancellationTokenSource
        AutoComplete: string
    }

    let init = 
        {
            ListWord = WordsHelper.emptyListWord;
            Words = List.empty
            Value =null
            Translate = null
            CancelToken =  null
            AutoComplete=""
        }

    type Msg =
    | LoadFromDB
    | SetListWord of ListWordsDB.ListWords
    | Create
    | NewValue of string * (Msg-> unit)
    | NewTranslate of string
    | SetAutoComplete of string
    | AutoCompleteClick
    | ClearAutoComplete
    let setAutoComplete str (dispatch: (Msg -> unit)) = 
        async{
            do! Async.Sleep 2000
            let! result = TranslateHelper.getTranslate(str)
            dispatch(SetAutoComplete(result))
        }
    let update (msg : Msg) (state: State) : State*Cmd<_>=
        match msg with
        | LoadFromDB ->
            {state with Words=WordDB.getWords state.ListWord.id},Cmd.none
        | SetListWord listWord ->
            {state with ListWord = listWord}, Cmd.ofMsg LoadFromDB
        | Create ->
            WordDB.createWord {id=0; Value=state.Value; Translate = state.Translate; Created= System.DateTime.Now; idListWords= state.ListWord.id} |> ignore
            {state with Value=""; Translate=""}, Cmd.ofMsg LoadFromDB
        | NewValue (str,dispatch)->
            if state.CancelToken<>null then
                state.CancelToken.Cancel()
            if not(System.String.IsNullOrEmpty str) && (System.String.IsNullOrEmpty(state.Translate)) then
                let cancellationSource = new CancellationTokenSource()
                Async.Start (setAutoComplete str (dispatch), cancellationSource.Token)
                {state with Value = str; CancelToken=cancellationSource; AutoComplete="" }, Cmd.none
            else
                {state with Value = str;}, Cmd.none
        | NewTranslate str -> {state with Translate = str; AutoComplete=""},Cmd.ofMsg ClearAutoComplete
        | SetAutoComplete str -> {state with AutoComplete = str}, Cmd.none
        | AutoCompleteClick -> state, Cmd.ofMsg (NewTranslate(state.AutoComplete))
        | ClearAutoComplete -> {state with AutoComplete= ""}, Cmd.none 
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
                    TextBox.onTextChanged(fun str -> if str=state.Translate then () else (dispatch (NewValue(str, dispatch))) )
                    TextBox.text state.Value
                    TextBox.watermark "Enter a word"
                ]
                TextBox.create[
                    TextBox.onTextChanged(fun str -> if str=state.Translate then () else (dispatch (NewTranslate(str))) )
                    TextBox.text state.Translate
                    TextBox.watermark "Enter a translate"
                ]
                if state.AutoComplete<>"" then
                    Button.create[
                        Button.content state.AutoComplete
                        Button.onClick ( fun _ -> dispatch(AutoCompleteClick))
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