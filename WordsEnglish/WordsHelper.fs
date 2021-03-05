module WordsHelper
    open WordsEnglish.Core
    let emptyListWord: ListWordsDB.ListWords= {
        id=0;
        Name="";
        TimeToRepiting= System.DateTime.Now;
        Created = System.DateTime.Now;
        Level= ""        
    }

    let emptyWord: WordDB.Word={
        id= 0
        Value=""
        Translate=""
        idListWords = 0
        Created= System.DateTime.Now
    }