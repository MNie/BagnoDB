namespace BagnoDB.Serializator

open System
open MongoDB.Bson.Serialization
open BagnoDB.Serializator
open Microsoft.FSharp.Reflection

type BagnoSerializationProvider() =
    let createInstance = Activator.CreateInstance
    let asBson (value: obj) =
        value :?> IBsonSerializer
    let toBson = createInstance >> asBson

    interface IBsonSerializationProvider with
        member this.GetSerializer(objType) =
            if isOption objType then
                typedefof<OptionSerializer<_>>.MakeGenericType (objType.GetGenericArguments())
                |> toBson
            elif FSharpType.IsUnion objType then
                typedefof<DiscriminatedUnionSerializer<_>>.MakeGenericType(objType)
                |> toBson
            elif FSharpType.IsRecord objType then
                typedefof<RecordSerializer<_>>.MakeGenericType(objType)
                |> toBson
            else
                null