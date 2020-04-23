module FiltersTests

    open MongoDB.Driver
    open Xunit
    open Swensen.Unquote
    open BagnoDB
    open MongoDB.Bson.Serialization

    type FakeModel =
        {
            data: string
            value: int
        }

    type FilterTypeTests () =
        let serializerRegistry = BsonSerializer.SerializerRegistry;
        let documentSerializer = serializerRegistry.GetSerializer<FakeModel>();

        let toBsonDocument (filter: FilterDefinition<FakeModel>) = filter.Render(documentSerializer, serializerRegistry);

        [<Fact>]
        let ``&&& operator on filter should be coherent with &&& from mongo package`` () =
            let fbuilder = FilterDefinitionBuilder<FakeModel> ()
            let firstFilter = fbuilder.Eq((fun (x: FakeModel) -> x.value), 0)
            let secondFilter = fbuilder.Eq((fun (x: FakeModel) -> x.data), "data")
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let result = first &&& second
            let expected = firstFilter &&& secondFilter

            toBsonDocument result.definition =! toBsonDocument expected

        [<Fact>]
        let ``||| operator on filter should be coherent with ||| from mongo package`` () =
            let fbuilder = FilterDefinitionBuilder<FakeModel> ()
            let firstFilter = fbuilder.Eq((fun (x: FakeModel) -> x.value), 0)
            let secondFilter = fbuilder.Eq((fun (x: FakeModel) -> x.data), "data")
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let result = first ||| second
            let expected = firstFilter ||| secondFilter

            toBsonDocument result.definition =! toBsonDocument expected

        [<Fact>]
        let ``||| operator on filter should be coherent with or function from Filter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let result = first ||| second
            let expected = Filter.``or`` second first
            let d = toBsonDocument result.definition
            let d1 = toBsonDocument expected.definition
            toBsonDocument result.definition =! toBsonDocument expected.definition

        [<Fact>]
        let ``&&& operator on filter should be coherent with or function from Filter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let result = first &&& second
            let expected = Filter.``and`` second first

            toBsonDocument result.definition =! toBsonDocument expected.definition

        [<Fact>]
        let ``||| order of parameters shouldn't matter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let firstResult = first ||| second
            let secondResult = second ||| first

            let firstBsonDoc = toBsonDocument firstResult.definition
            let secondBsonDoc = toBsonDocument secondResult.definition

            firstBsonDoc.ToString () =! "{ \"$or\" : [{ \"value\" : 0 }, { \"data\" : \"data\" }] }"
            secondBsonDoc.ToString () =! "{ \"$or\" : [{ \"data\" : \"data\" }, { \"value\" : 0 }] }"

        [<Fact>]
        let ``&&& order of parameters shouldn't matter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let firstResult = first &&& second
            let secondResult = second &&& first
            let firstBsonDoc = toBsonDocument firstResult.definition
            let secondBsonDoc = toBsonDocument secondResult.definition

            firstBsonDoc.ToString () =! "{ \"value\" : 0, \"data\" : \"data\" }"
            secondBsonDoc.ToString () =! "{ \"data\" : \"data\", \"value\" : 0 }"

        [<Fact>]
        let ``Filter.or order of parameters shouldn't matter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let firstResult = Filter.``or`` first second
            let secondResult = Filter.``or`` second first

            let firstBsonDoc = toBsonDocument firstResult.definition
            let secondBsonDoc = toBsonDocument secondResult.definition

            firstBsonDoc.ToString () =! "{ \"$or\" : [{ \"data\" : \"data\" }, { \"value\" : 0 }] }"
            secondBsonDoc.ToString () =! "{ \"$or\" : [{ \"value\" : 0 }, { \"data\" : \"data\" }] }"

        [<Fact>]
        let ``Filter.and order of parameters shouldn't matter`` () =
            let first = Filter.eq (fun (x: FakeModel) -> x.value) 0
            let second = Filter.eq (fun (x: FakeModel) -> x.data) "data"

            let firstResult = Filter.``and`` first second
            let secondResult = Filter.``and`` second first
            let firstBsonDoc = toBsonDocument firstResult.definition
            let secondBsonDoc = toBsonDocument secondResult.definition

            firstBsonDoc.ToString () =! "{ \"data\" : \"data\", \"value\" : 0 }"
            secondBsonDoc.ToString () =! "{ \"value\" : 0, \"data\" : \"data\" }"