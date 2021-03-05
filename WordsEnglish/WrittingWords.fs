module WrittingWords
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

    type Msg = 
    | SetWords of ListWordsDB.ListWords
    | Text of string
    | NextWord
    | Check
    | GoBack
    | IsEnd
    | SaveNewLevel
    type State = {
        CurrentWord : WordDB.Word;
        IndexCurrentWord: int
        ListWords : ListWordsDB.ListWords;
        Words : WordDB.Word list;
        TextValue: string
    }
    let init = 
        {
            CurrentWord= WordsHelper.emptyWord;
            IndexCurrentWord=0;
            ListWords = WordsHelper.emptyListWord;
            Words = List.empty;
            TextValue = "";
        }, Cmd.none
    let update (msg : Msg) (state: State) : State*Cmd<_>=
        match msg with
        | SetWords listWords ->
            let words = WordDB.getWords listWords.id;
            {state with ListWords = listWords; Words= words; CurrentWord= words.[0]}, Cmd.none
        | Text text -> {state with TextValue = text}, Cmd.none
        | Check ->
            if state.CurrentWord.Value=state.TextValue then
                state, Cmd.ofMsg IsEnd
            else
                state, Cmd.ofMsg GoBack
        | IsEnd ->
            if state.IndexCurrentWord+1> state.Words.Length-1 then
                state, Cmd.ofMsg SaveNewLevel
            else
                state, Cmd.ofMsg NextWord
        | NextWord -> {state with CurrentWord= state.Words.[state.IndexCurrentWord+1]; IndexCurrentWord= state.IndexCurrentWord+1}, Cmd.none
        | SaveNewLevel ->
            let newLevel = LevelHelper.nextLevel state.ListWords.Level
            let res = ListWordsDB.updateLevel state.ListWords.id newLevel ( LevelHelper.getTimeToRepeat  newLevel)
            state, Cmd.ofMsg GoBack
        | GoBack -> init
    let view (state: State) (dispatch) =
        StackPanel.create[
            StackPanel.children[
                TextBlock.create[
                    TextBlock.text $"{state.ListWords.Name} learning"
                ]
                TextBlock.create[
                    TextBlock.text state.CurrentWord.Translate
                ]
                TextBox.create[
                    TextBox.watermark "enter the value"
                    TextBox.onTextChanged ( fun text -> dispatch(Text text))
                ]
                Button.create[
                    Button.content "check"
                    Button.onClick (fun _ -> dispatch(Check))
                ]
            ]
        ]