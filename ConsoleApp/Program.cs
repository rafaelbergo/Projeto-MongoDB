using MongoDB.Driver;

string connectionString = "mongodb://localhost:27017"; 

var client = new MongoClient(connectionString);

while(true)
{
    Console.Clear();
    Console.WriteLine("\nCRUD MongoDB:");
    Console.WriteLine("1. Create Collection");
    Console.WriteLine("2. List Collections");
    Console.WriteLine("3. Delete Collection");
    Console.WriteLine("4. Insert Document");
    Console.WriteLine("5. List Documents");
    Console.WriteLine("6. Edit Document");
    Console.WriteLine("7. Delete Document");
    Console.WriteLine("8. Exit");
    Console.Write("Choose an option: ");
    
    var opcao = Console.ReadLine();

    switch(opcao)
    {
        case "1":
            CreateCollection(client);
            break;
        case "2":
            ListCollections(client);
            break;
        case "3":
            DeleteCollection(client);
            break;
        case "4":
            InsertDocument(client);
            break;
        case "5":
            ListDocuments(client);
            break;
        case "6":
            EditDocument(client);
            break;
        case "7":
            DeleteDocument(client);
            break;
        case "8":
            return;
        default:
            Console.WriteLine("Invalid option");
            break;
    }
}

static void CreateCollection(MongoClient client)
{
    throw new NotImplementedException();
}

static void ListCollections(MongoClient client)
{
    throw new NotImplementedException();
}

static void DeleteCollection(MongoClient client)
{
    throw new NotImplementedException();
}

static void InsertDocument(MongoClient client)
{
    throw new NotImplementedException();
}

static void ListDocuments(MongoClient client)
{
    throw new NotImplementedException();
}

static void EditDocument(MongoClient client)
{
    throw new NotImplementedException();
}

static void DeleteDocument(MongoClient client)
{
    throw new NotImplementedException();
}