namespace BagnoDB.Serializator

open Microsoft.FSharp.Reflection
open MongoDB.Bson
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Serializers
open BagnoDB.Serializator

type OptionSerializer<'TOption when 'TOption: equality>() =
    inherit SerializerBase<'TOption option>()

    let cases = groupDUByName typeof<'TOption option>

    override this.Serialize (context, _, value) =
        match value with
        | None -> BsonSerializer.Serialize(context.Writer, null)
        | Some x -> BsonSerializer.Serialize(context.Writer, x)

    override this.Deserialize (context, _) =
        let typeOfArg = typeof<'TOption>

        let (case, args) =
            let value =
                let isDecimalValueNull = (typeOfArg = typeof<decimal> && context.Reader.CurrentBsonType = BsonType.Null)
                if typeOfArg.IsPrimitive || isDecimalValueNull then
                    BsonSerializer.Deserialize(context.Reader, typeof<obj>)
                else
                    BsonSerializer.Deserialize(context.Reader, typeOfArg)
            match value with
            | null -> (cases.["None"], [||])
            | _ -> (cases.["Some"], [| value |])
        FSharpValue.MakeUnion(case, args) :?> 'TOption option
