namespace BagnoDB.Serializator

open Microsoft.FSharp.Reflection
open System

[<AutoOpen>]
module internal Helpers =
    let isOption objType = FSharpType.IsUnion objType &&
                           objType.IsGenericType &&
                           objType.GetGenericTypeDefinition() = typedefof<_ option>

    let groupDUByName objType =
        FSharpType.GetUnionCases(objType)
        |> Seq.map(fun x -> (x.Name, x))
        |> dict
