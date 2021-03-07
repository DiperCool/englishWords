namespace WordsEnglish
open WordsEnglish.Core

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
    open Elmish
    open Avalonia
    open Avalonia.Layout
    open Avalonia.Media
    open Avalonia.Controls
    open Avalonia.Input
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Elmish
    type AppView = 
        | ListsWords
        | Words
        | WrittingWords
        | PanelSelectLearning
    type State =
        { 
            ListWordsState: ListWords.State
            AppView : AppView
            WordState : Word.State
            WrittingWords: WrittingWords.State
            PanelSelectLearning: PanelSelectLearning.State
        }

    type Msg =
        | ListWordsMsg of ListWords.Msg
        | WordMsg of Word.Msg
        | WrittingWordsMsg of WrittingWords.Msg
        | PanelSelectLearningMsg of PanelSelectLearning.Msg
        | GoBack
    let init: State * Cmd<_>=
        /// If your children controls don't emit any commands
        /// in the init function, you can just return Cmd.none
        /// otherwise, you can use a batch operation on all of them
        /// you can add more init commands as you need
        let listWords,cmdListWords=ListWords.init;
        let word = Word.init;
        let writtingWords, cmdWrittingWords = WrittingWords.init;
        let panelSelectLearning, cmdPanelSelectLearning = PanelSelectLearning.init;
        {
            ListWordsState=listWords; 
            AppView= ListsWords; 
            WordState= word; 
            WrittingWords=writtingWords
            PanelSelectLearning= panelSelectLearning
        },
        Cmd.batch [
            Cmd.map PanelSelectLearningMsg cmdPanelSelectLearning
            Cmd.map ListWordsMsg cmdListWords
            Cmd.map WrittingWordsMsg cmdWrittingWords
           ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | ListWordsMsg msg ->
            match msg with
                | ListWords.Msg.ViewWord listWords ->
                    let s, cmd = Word.update (Word.Msg.SetListWord(listWords)) state.WordState
                    {state with AppView = Words; WordState=s }, Cmd.map WordMsg cmd
                | ListWords.Msg.Learn listWords ->
                    let s, cmd = PanelSelectLearning.update (PanelSelectLearning.Msg.SetListWords(listWords)) state.PanelSelectLearning
                    {state with AppView = PanelSelectLearning; PanelSelectLearning=s }, Cmd.map PanelSelectLearningMsg cmd
                | _ ->
                    let s, cmd = ListWords.update msg state.ListWordsState
                    {state with ListWordsState = s}, Cmd.map ListWordsMsg cmd
        | WordMsg msg ->
            let s, cmd = Word.update msg state.WordState
            {state with WordState = s}, Cmd.batch[Cmd.map WordMsg cmd]
        | WrittingWordsMsg msg->
            match msg with
            | WrittingWords.Msg.GoBack ->
                let s, cmd = WrittingWords.update msg state.WrittingWords
                let sListWords, cmdListWords= ListWords.init
                {state with WrittingWords=s;ListWordsState=sListWords}, Cmd.batch[
                    Cmd.map WrittingWordsMsg cmd;
                    Cmd.ofMsg GoBack;
                    Cmd.map ListWordsMsg cmdListWords;
                ]
            | _ ->
                let s, cmd = WrittingWords.update msg state.WrittingWords
                {state with WrittingWords=s }, Cmd.map WrittingWordsMsg cmd
        | PanelSelectLearningMsg msg ->
            match msg with
            | PanelSelectLearning.Msg.GoToWrittingWords listWords ->
                let s, cmd = WrittingWords.update (WrittingWords.Msg.SetWords(listWords)) state.WrittingWords
                {state with AppView = WrittingWords; WrittingWords=s }, Cmd.map WrittingWordsMsg cmd
            | _ ->
                let s, cmd = PanelSelectLearning.update msg state.PanelSelectLearning
                {state with PanelSelectLearning=s }, Cmd.map PanelSelectLearningMsg cmd

        | GoBack -> {state with AppView = ListsWords}, Cmd.none
    let view (state: State) (dispatch) =
        StackPanel.create[
            StackPanel.children[
                Button.create[
                    Button.content "go back"
                    Button.onClick (fun _ -> dispatch(GoBack))
                    Button.isEnabled (state.AppView<>ListsWords)
                ]
                (match state.AppView with
                    | ListsWords -> ListWords.view state.ListWordsState (ListWordsMsg>> dispatch)
                    | Words -> Word.view state.WordState (WordMsg >> dispatch)
                    | WrittingWords -> WrittingWords.view state.WrittingWords (WrittingWordsMsg >> dispatch)
                    | PanelSelectLearning -> PanelSelectLearning.view state.PanelSelectLearning (PanelSelectLearningMsg >> dispatch)
                )
            ]
        ]

    /// This is the main window of your application
    /// you can do all sort of useful things here like setting heights and widths
    /// as well as attaching your dev tools that can be super useful when developing with
    /// Avalonia
    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "xuy"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.run
