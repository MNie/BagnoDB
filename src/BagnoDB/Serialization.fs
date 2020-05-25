namespace BagnoDB

    open MongoDB.Bson.Serialization

    module Serialization =
        let bson bsonSerializer =
            BsonSerializer.RegisterSerializationProvider bsonSerializer

        let typedBson<'TType> bsonSerializer =
            BsonSerializer.RegisterSerializer<'TType> bsonSerializer
