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
    type State =
        { 
            ListWordsState: ListWords.State
            AppView : AppView
            WordState : Word.State
        }

    type Msg =
        | ListWordsMsg of ListWords.Msg
        | WordMsg of Word.Msg
        | GoBack
    let init: State * Cmd<_>=
        /// If your children controls don't emit any commands
        /// in the init function, you can just return Cmd.none
        /// otherwise, you can use a batch operation on all of them
        /// you can add more init commands as you need
        let listWords,cmdListWords=ListWords.init;
        let word, cmdWord = Word.init;
        {ListWordsState=listWords; AppView= ListsWords; WordState= word},Cmd.batch [ 
                                                                                    Cmd.map ListWordsMsg cmdListWords
                                                                                    Cmd.map WordMsg cmdWord
                                                                                   ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | ListWordsMsg msg ->
            match msg with
                | ListWords.Msg.ViewWord listWords ->
                    let s, cmd = Word.update (Word.Msg.SetListWord(listWords)) state.WordState
                    {state with AppView = Words; WordState=s }, Cmd.map WordMsg cmd
                | _ ->
                    let s, cmd = ListWords.update msg state.ListWordsState
                    {state with ListWordsState = s}, Cmd.map ListWordsMsg cmd
        | WordMsg msg ->
            let s, cmd = Word.update msg state.WordState
            {state with WordState = s}, Cmd.batch[Cmd.map WordMsg cmd]
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
                    | Words -> Word.view state.WordState (WordMsg >> dispatch))
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
