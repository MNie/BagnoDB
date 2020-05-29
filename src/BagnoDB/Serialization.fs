namespace BagnoDB

    open MongoDB.Bson.Serialization
    open MongoDB.Bson.Serialization.Serializers

    module Serialization =
        let bson bsonSerializer =
            BsonSerializer.RegisterSerializationProvider bsonSerializer

        let typedBson<'TType> bsonSerializer =
            BsonSerializer.RegisterSerializer<'TType> bsonSerializer
            
        let decimal () =
            BsonSerializer.RegisterSerializer(typeof<decimal>, DecimalSerializer MongoDB.Bson.BsonType.Decimal128)
