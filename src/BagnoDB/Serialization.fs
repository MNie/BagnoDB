namespace BagnoDB

    open MongoDB.Bson.Serialization
    open MongoDB.Bson.Serialization.Serializers

    [<RequireQualifiedAccess>]
    module Serialization =
        let bson bsonSerializer =
            BsonSerializer.RegisterSerializationProvider bsonSerializer

        let typedBson<'T> bsonSerializer =
            BsonSerializer.RegisterSerializer<'T> bsonSerializer

        let registerDecimal () =
            BsonSerializer.RegisterSerializer(typeof<decimal>, DecimalSerializer MongoDB.Bson.BsonType.Decimal128)