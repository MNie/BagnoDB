namespace BagnoDB.Serializator

open BagnoDB.Serializator
open Microsoft.FSharp.Reflection
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Serializers
open MongoDB.Bson.IO

type DiscriminatedUnionSerializer<'TDiscriminatedUnion>() =
    inherit SerializerBase<'TDiscriminatedUnion>()
    let caseFieldName = "case"
    let valueFieldName = "fields"
    let cases = groupDUByName typeof<'TDiscriminatedUnion>

    let deserialize context args ``type`` =
        BsonSerializer.LookupSerializer(``type``).Deserialize(context, args)

    let serialize context args ``type`` value =
        BsonSerializer.LookupSerializer(``type``).Serialize(context, args, value)

    let deserializeItems context args types =
        types
        |> Seq.fold(fun state t -> (deserialize context args t) :: state) []
        |> Seq.toArray
        |> Array.rev

    override this.Deserialize (context, args): 'TDiscriminatedUnion =
        context.Reader.ReadStartDocument()

        let name = context.Reader.ReadString(caseFieldName)
        let union = cases.[name]
        let unionTypes = (union.GetFields() |> Seq.map(fun f -> f.PropertyType))

        context.Reader.ReadName(valueFieldName)
        context.Reader.ReadStartArray()

        let items = deserializeItems context args unionTypes

        context.Reader.ReadEndArray()
        context.Reader.ReadEndDocument()

        FSharpValue.MakeUnion(union, items) :?> 'TDiscriminatedUnion

    override this.Serialize (context, args, value) =
        let case, fields = FSharpValue.GetUnionFields(value, typeof<'TDiscriminatedUnion>)

        context.Writer.WriteStartDocument()
        context.Writer.WriteString(caseFieldName, case.Name)
        context.Writer.WriteStartArray(valueFieldName)

        fields
        |> Seq.zip(case.GetFields())
        |> Seq.iter(fun (field, value) -> serialize context args field.PropertyType value)

        context.Writer.WriteEndArray()
        context.Writer.WriteEndDocument()