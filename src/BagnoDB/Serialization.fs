namespace BagnoDB

    open MongoDB.Bson.Serialization
    open MongoDB.Bson.Serialization.Serializers

    module Serialization =
        let bson bsonSerializer =
            BsonSerializer.RegisterSerializationProvider bsonSerializer

        let registerDecimalSerializer () =
            BsonSerializer.RegisterSerializer(typeof<decimal>, DecimalSerializer MongoDB.Bson.BsonType.Decimal128)