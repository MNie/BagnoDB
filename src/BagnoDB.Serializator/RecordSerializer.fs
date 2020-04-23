namespace BagnoDB.Serializator

open System.Reflection
open MongoDB.Bson.Serialization
open MongoDB.Bson.Serialization.Serializers
open Microsoft.FSharp.Reflection

type RecordSerializer<'TRecord>() =
    inherit SerializerBase<'TRecord>()
    let classMap = BsonClassMap.LookupClassMap(typeof<'TRecord>)
    let serializer = BsonClassMapSerializer(classMap)
    let fields = FSharpType.GetRecordFields typeof<'TRecord>

    override this.Serialize(context, args, value) =
        let mutable nargs = args
        nargs.NominalType <- typeof<'TRecord>
        serializer.Serialize(context, nargs, value)

    override this.Deserialize(context, args) =
        let mutable nargs = args
        nargs.NominalType <- typeof<'TRecord>
        serializer.Deserialize(context, nargs)

    interface IBsonDocumentSerializer with
        member x.TryGetMemberSerializationInfo(memberName, serializationInfo) =
            if Array.exists (fun (el: PropertyInfo) -> el.Name = memberName) fields then
                let mm = classMap.GetMemberMap(memberName)
                serializationInfo <- new BsonSerializationInfo(mm.ElementName, mm.GetSerializer(), mm.MemberType)
                true
            else
                false
