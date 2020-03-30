module FiltersTests

    open MongoDB.Driver
    open Xunit
    open Swensen.Unquote
    open BagnoDB

    type FakeModel =
        {
            data: string
            value: int
        }

    type FilterTypeTests () =
        [<Fact>]
        let ``&&& operator on filter should be coherent with &&& from mongo package`` () =
            let fbuilder = FilterDefinitionBuilder<FakeModel> ()
            let firstFilter = fbuilder.Eq((fun (x: FakeModel) -> x.value), 0)
            let secondFilter = fbuilder.Eq((fun (x: FakeModel) -> x.data), "data")
            let first = { definition = firstFilter }
            let second = { definition = secondFilter }

            let result = first &&& second
            let expected = firstFilter &&& secondFilter
            result.definition =! expected

        [<Fact>]
        let ``||| operator on filter should be coherent with ||| from mongo package`` () =
            let fbuilder = FilterDefinitionBuilder<FakeModel> ()
            let firstFilter = fbuilder.Eq((fun (x: FakeModel) -> x.value), 0)
            let secondFilter = fbuilder.Eq((fun (x: FakeModel) -> x.data), "data")
            let first = { definition = firstFilter }
            let second = { definition = secondFilter }

            let result = first ||| second
            let expected = firstFilter ||| secondFilter
            result.definition =! expected