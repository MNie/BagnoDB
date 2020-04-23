namespace BagnoDB.Serializator

open BagnoDB.Serializator
open Microsoft.FSharp.Reflection
open MongoDB.Bson.Serialization.Conventions

type OptionConvention() =
    inherit ConventionBase("F# Option Type")

    interface IMemberMapConvention with
        member this.Apply (memberMap) =
            let objType = memberMap.MemberType
            if isOption objType then
                memberMap.SetDefaultValue None |> ignore
                memberMap.SetIgnoreIfNull true |> ignore

type RecordConvention() =
    inherit ConventionBase("F# Record Type")

    interface IClassMapConvention with
        member this.Apply(classMap) =
            let objType = classMap.ClassType

            if FSharpType.IsRecord objType then
                classMap.SetIgnoreExtraElements(true)
                let fields = FSharpType.GetRecordFields objType
                let names = fields |> Array.map (fun x -> x.Name)
                let types = fields |> Array.map (fun x -> x.PropertyType)

                let ctor = objType.GetConstructor(types)

                classMap.MapConstructor(ctor, names) |> ignore
                fields |> Array.iter (fun x -> classMap.MapMember(x) |> ignore)