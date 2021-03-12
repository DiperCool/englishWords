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
        | LvlUp of ListWordsDB.ListWords
        | Learn of ListWordsDB.ListWords

    let init = 
        {
            ListsWords= List.empty;
            TextNewListWords= "";
            Page = 1
        }, Cmd.ofMsg LoadFromDb

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | LoadFromDb ->
            let res =ListWordsDB.getListWords state.Page |> List.ofSeq
            {state with ListsWords= res}, Cmd.none
        | CreateListWords ->
            ListWordsDB.createListWords {
                id=0;
                Name= state.TextNewListWords;
                TimeToRepiting= System.DateTime.Now;
                Created= System.DateTime.Now;
                Level = "one"
                AmountRepetition = 0
                IsNotificated = "false"
            } |> ignore
            {state with TextNewListWords= ""}, Cmd.ofMsg LoadFromDb
        | UdateTextNewListWords (str) -> {state with TextNewListWords = str}, Cmd.none
        | ViewWord listWords -> state, Cmd.none
        | NextPage -> {state with Page = state.Page+1}, Cmd.ofMsg LoadFromDb
        | PrevPage -> {state with Page = state.Page-1}, Cmd.ofMsg LoadFromDb
        | LvlUp item->
            ListWordsDB.updateLevel item.id "two" |> ignore
            state, Cmd.ofMsg LoadFromDb
        | Learn listWords -> state, Cmd.none
    let viewDots (item: ListWordsDB.ListWords)=
        StackPanel.create[
                StackPanel.orientation Orientation.Horizontal
                StackPanel.children[
                    TextBlock.create[
                        TextBlock.foreground (if item.AmountRepetition>=1 then "green" else "white")
                        TextBlock.fontSize 30.
                        TextBlock.text "•"
                    ]
                    TextBlock.create[
                        TextBlock.foreground (if item.AmountRepetition>=2 then "green" else "white")
                        TextBlock.fontSize 30.
                        TextBlock.text "•"
                    ]
                    TextBlock.create[
                        TextBlock.foreground (if item.AmountRepetition>=3 then "green" else "white")
                        TextBlock.fontSize 30.
                        TextBlock.text "•"
                    ]
                    
                ]
            ]
    let viewListWords (notes) dispatch =
        StackPanel.create[
            StackPanel.children[for item in notes do
                                yield (
                                   Grid.create[
                                       Grid.columnDefinitions "*,*,*"
                                       Grid.children[
                                            TextBlock.create[
                                                TextBlock.fontSize(18.)
                                                Grid.column 0
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
                                            if item.TimeToRepiting<System.DateTime.Now then
                                                StackPanel.create[
                                                    StackPanel.orientation Orientation.Horizontal
                                                    StackPanel.horizontalAlignment HorizontalAlignment.Right
                                                    StackPanel.spacing 30.
                                                    Grid.column 3
                                                    StackPanel.children[
                                                            yield viewDots item
                                                            yield Button.create[
                                                                Button.content "Learn" 
                                                                Button.onClick ((fun _ -> 
                                                                                  dispatch (Learn item)
                                                                                ), Always)
                                                            ]
                                                            
                                                    ]
                                                ]
                                                                             
                                        ]
                                   ]
                                          
                                )
                        ]
        ]
           
    let view (state: State) (dispatch) =
        StackPanel.create[
            StackPanel.children[
                yield TextBox.create[
                    TextBox.dock Dock.Top
                    TextBox.text state.TextNewListWords
                    TextBox.onTextChanged(fun str -> if str=state.TextNewListWords then () else  (dispatch (UdateTextNewListWords(str))) )
                ]
                yield Button.create[
                    Button.content "Create"
                    Button.isEnabled ((not (isNull state.TextNewListWords)) && state.TextNewListWords.Length <> 0)
                    Button.onClick(fun _ -> dispatch(CreateListWords))
                ]
                yield viewListWords state.ListsWords dispatch
                yield 
                    StackPanel.create[
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                        StackPanel.children[
                            Button.create[
                               
                                Button.content "<"
                                Button.onClick(fun _ -> dispatch(PrevPage))
                            ]
                            Button.create[
                                
                                Button.content ">"
                                Button.onClick(fun _ -> dispatch(NextPage))
                            ]
                        ]
                            
                        
                        
                    
                ]
            ]
        ]