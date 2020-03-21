module Tests

    open System.Threading.Tasks
    open System.Threading
    open MongoDB.Driver
    open MongoDB.Bson
    open MongoDB.Bson.Serialization.Attributes
    open Xunit
    open Swensen.Unquote
    open BagnoDB.Configuring
    open BagnoDB.Connecting
    open BagnoDB.Filtering
    open BagnoDB.Querying

    [<BsonIgnoreExtraElements>]
    type BagnoTest = {
        identifier: ObjectId
        data: string
        value: int
    }

    type BaseRepositoryTests () =
        //"mongodb://admin:123@0.0.0.0:27017"
        let collection = "bagno"
        let database = "bagnoDBTests"

        let config = {
            Host = "0.0.0.0"
            Port = 27017
            User = Some "admin"
            Password = Some "123"
        }
        let bagno = { identifier = ObjectId.GenerateNewId (); data = "Bagno"; value = 2 }
        let mango = { identifier = ObjectId.GenerateNewId (); data = "mango"; value = 2137 }
        let tango = { identifier = ObjectId.GenerateNewId (); data = "tango"; value = 1488 }
        let input = [ bagno; mango; tango ]

        [<Fact>]
        let ``filter with single parameter, return bagno`` () =
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) 2
            let filterOpt = FindOptions<BagnoTest>()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.filter CancellationToken.None filterOpt filter

                result.ToArray () =! [| bagno |]
            } |> Async.StartAsTask

        [<Fact>]
        let ``insert and getAll, return bagno, mango, tango, idaho`` () =
            let newElement = { identifier = ObjectId.GenerateNewId (); data = "idaho"; value = 666 }
            let insertOpt = InsertOneOptions()
            let getOpt = FindOptions<BagnoTest>()
            async {
                let col =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection

                do!
                    col
                    |> Query.insertOne CancellationToken.None insertOpt newElement

                let! result =
                    col
                    |> Query.getAll CancellationToken.None getOpt

                result.ToArray () =! [| bagno; mango; tango; newElement |]
            } |> Async.StartAsTask

        [<Fact>]
        let ``upsert, update mango to the idaho`` () =
            let newElement = { mango with data = "idaho" }
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) mango.value
            let filterOpt = FindOneAndReplaceOptions<BagnoTest>()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.upsert CancellationToken.None filterOpt newElement filter

                result =! newElement
            } |> Async.StartAsTask

        [<Fact>]
        let ``delete, delete mathching record (mango)`` () =
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) mango.value
            let filterOpt = FindOneAndDeleteOptions<BagnoTest>()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.delete CancellationToken.None filterOpt filter

                result =! mango
            } |> Async.StartAsTask

        [<Fact>]
        let ``delete many, delete bagno i tango and keep mango in db`` () =
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.data)) "Bagno"
                |> (|||) (Filter.lt (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) 2137)
            let deleteOpt = DeleteOptions()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.deleteMany CancellationToken.None deleteOpt filter

                result.DeletedCount =! 2L
            } |> Async.StartAsTask

        [<Fact>]
        let ``filter with two parameters, return mango`` () =
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.data)) "mango"
                |> (&&&) (Filter.gte (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) 2137)
            let filterOpt = FindOptions<BagnoTest>()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.filter CancellationToken.None filterOpt filter

                result.ToArray () =! [| mango |]
            } |> Async.StartAsTask

        [<Fact>]
        let ``filter with two parameters that should return 0 results`` () =
            let filter =
                Filter.eq (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.value)) 2
                |> Filter.``and`` (Filter.not (ExpressionHelper.AsExpression (fun (o: BagnoTest) -> o.data)) "Bagno")
            let filterOpt = FindOptions<BagnoTest>()
            async {
                let! result =
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.filter CancellationToken.None filterOpt filter

                result.ToArray () =! [|  |]
            } |> Async.StartAsTask

        let clear () =
            async {
                let opt = DeleteOptions()
                let filter = Filter.empty
                return!
                    Connection.host config
                    |> Connection.database database
                    |> Connection.collection collection
                    |> Query.deleteMany CancellationToken.None opt filter
            }

        interface IAsyncLifetime with
            member _.InitializeAsync () =
                async {
                    let! _ = clear ()
                    let opt = InsertManyOptions()
                    do!
                        Connection.host config
                        |> Connection.database database
                        |> Connection.collection collection
                        |> Query.insertMany CancellationToken.None opt input
                } |> Async.StartAsTask :> Task

            member _.DisposeAsync () =
                async {
                    return! clear ()
                } |> Async.StartAsTask :> Task
