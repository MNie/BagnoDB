namespace BagnoDB.Serializator.Tests

open System.Threading
open Xunit
open Swensen.Unquote
open BagnoDB
open BagnoDB.Serializator
open MongoDB.Bson
open MongoDB.Driver

type SimpleRecord =
    {
        Id: string
        Foo: int
        Bar: string
    }

type Union =
    | TestCase1 of int
    | TestCase2 of string
    | TestCase3 of float
    | TestCase4 of decimal
    | TestCase5 of bool
    | TestCase6 of SimpleRecord
    | TestCase7 of SimpleRecord option
    | TestCase8

type RecordWithUnions =
    {
        Id: ObjectId
        Foo: int
        Bar1: Union
        Bar2: Union
        Bar3: Union
        Bar4: Union
        Bar5: Union
        Bar6: Union
        Bar7: Union
        Bar8: Union
        Bar9: int list
    }

type SmallestPossibleRecord =
    {
        Foo: Union
    }

type SerializatorTests() =
    let database = "BagnoDB_Serializator_IntegrationTests"

    let config = {
        host = "0.0.0.0"
        port = 27017
        user = Some "admin"
        password = Some "123"
    }

    let connection (collection) =
        let connection =
            Connection.host config
            |> Connection.database database
            |> Connection.collection collection

        Conventions.create
        |> Conventions.add (OptionConvention ())
        |> Conventions.add (RecordConvention ())
        |> Conventions.build "F# Type Conventions"

        Serialization.bson (BagnoSerializationProvider ())

        connection

    [<Fact>]
    member _.``simple record with primitive fields should be serialized/deserialized correctly`` () =
        let wildcard = Filter.empty
        let testCase =
            {
                SimpleRecord.Id = "Id"
                SimpleRecord.Foo = 5
                SimpleRecord.Bar = "Bar"
            }

        let con = connection "SimpleRecord_Tests"
        let delOpt = DeleteOptions()
        let insOpt = InsertOneOptions()
        let filterOpt = FindOptions<SimpleRecord>()

        async {
            let! _ = con |> Query.deleteMany CancellationToken.None delOpt wildcard
            do! con |> Query.insertOne CancellationToken.None insOpt testCase
            let! saved = con |> Query.filter CancellationToken.None filterOpt wildcard
            testCase =! (saved |> Seq.head)
        } |> Async.RunSynchronously

    [<Fact>]
    member _.``records with nested unions which contains records should be serialized/deserialized correctly`` () =
        let wildcard = Filter.empty
        let testCase =
            {
                RecordWithUnions.Id = ObjectId.GenerateNewId()
                RecordWithUnions.Foo = 0
                RecordWithUnions.Bar1 = Union.TestCase1(2)
                RecordWithUnions.Bar2 = Union.TestCase2("BarBar")
                RecordWithUnions.Bar3 = Union.TestCase3(1.4)
                RecordWithUnions.Bar4 = Union.TestCase4(3.14M)
                RecordWithUnions.Bar5 = Union.TestCase5(true)
                RecordWithUnions.Bar6 = Union.TestCase6({ SimpleRecord.Id = "NestedId"; SimpleRecord.Foo = 5; SimpleRecord.Bar = "NestedBar"})
                RecordWithUnions.Bar7 = Union.TestCase7(None)
                RecordWithUnions.Bar8 = Union.TestCase8
                RecordWithUnions.Bar9 = [1; 2; 3; 4; 5]
            }

        let con = connection "RecordWithUnions_Tests"
        let delOpt = DeleteOptions()
        let insOpt = InsertOneOptions()
        let filterOpt = FindOptions<RecordWithUnions>()
        async {
            let! _ = con |> Query.deleteMany CancellationToken.None delOpt wildcard
            do! con |> Query.insertOne CancellationToken.None insOpt testCase
            let! saved = con |> Query.filter CancellationToken.None filterOpt wildcard
            testCase =! (saved |> Seq.head)
        } |> Async.RunSynchronously

    [<Fact>]
    member _.``record with single primitive field should be serialized/deserialized correclty`` () =
        let wildcard = Filter.empty
        let testCase =
            {
                SmallestPossibleRecord.Foo = Union.TestCase7 None
            }

        let con = connection "SmallestPossibleRecord_Tests"
        let delOpt = DeleteOptions()
        let insOpt = InsertOneOptions()
        let filterOpt = FindOptions<SmallestPossibleRecord>()
        async {
            let! _ = con |> Query.deleteMany CancellationToken.None delOpt wildcard
            do! con |> Query.insertOne CancellationToken.None insOpt testCase
            let! saved = con |> Query.filter CancellationToken.None filterOpt wildcard
            testCase =! (saved |> Seq.head)
        } |> Async.RunSynchronously
