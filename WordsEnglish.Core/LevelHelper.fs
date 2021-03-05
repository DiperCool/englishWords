namespace WordsEnglish.Core
    module LevelHelper=
        let getTimeToRepeat level=
            match level with
            | "one" -> System.DateTime.Now
            | "two" -> System.DateTime.Now.AddDays(1.)
            | "three" -> System.DateTime.Now.AddDays(3.)
            | "four" -> System.DateTime.Now.AddDays(14.)
            | "five" -> System.DateTime.Now.AddMonths(2)
            | "six" -> System.DateTime.Now.AddMonths(2)
            | _ -> System.DateTime.MinValue
        let nextLevel level=
            match level with
            | "one" -> "two"
            | "two" -> "three"
            | "three" -> "four"
            | "four"->"five"
            | "five" -> "six"
            | _ -> ""