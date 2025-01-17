using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

string connectionString = "mongodb://localhost:27017"; 
var client = new MongoClient(connectionString);

string selectedDatabaseName = string.Empty;
string selectedCollectionName = string.Empty;
List<string> localDatabases = new List<string>();

while(true)
{
    //Console.Clear();
    Console.WriteLine("\nCRUD MongoDB:");
    Console.WriteLine("1. Create Database");
    Console.WriteLine("2. List Databases");
    Console.WriteLine("3. Select Database");
    Console.WriteLine("4. Delete Database");
    Console.WriteLine("5. Exit\n");
    Console.Write("Choose an option: ");

    var mainMenuOption = Console.ReadLine();

    switch(mainMenuOption)
    {
        case "1":
            CreateDatabase(client);
            break;
        case "2":
            ListDatabases(client);
            break;
        case "3":
            var dbName = SelectDatabase(client);
            if (dbName != null)
            {
                selectedDatabaseName = dbName;
                CollectionsMenu(client, selectedDatabaseName);
            }
            break;
        case "4":
            DeleteDatabase(client);
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Invalid option");
            Console.ReadLine();
            break;
    }
}


// Database methods
void CreateDatabase(MongoClient client)
{
    Console.Write("Enter the name of the new database: ");
    var dbName = Console.ReadLine();

    if(!string.IsNullOrWhiteSpace(dbName))
    {   
        localDatabases.Add(dbName);
        var database = client.GetDatabase(dbName);
        Console.WriteLine($"Database '{dbName}' created.");
    }
    else
    {
        Console.WriteLine("Invalid name.");
    }
    Console.ReadLine();
}

void ListDatabases(MongoClient client)
{
    var databaseNames = client.ListDatabaseNames().ToList();

    if (databaseNames.Count == 0)
    {
        Console.WriteLine("No databases found.");
    }

    else
    {
        Console.WriteLine("Databases on the server: ");

        foreach (var db in databaseNames)
        {
            Console.WriteLine(db);
        }

        foreach (var name in localDatabases)
        {
            Console.WriteLine(name);
        }
    }
    Console.ReadLine();
}

string? SelectDatabase(MongoClient client)
{
    var databaseNames = client.ListDatabaseNames().ToList();

    Console.Write("\nEnter the name of the database to select: ");
    var dbName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(dbName))
    {
        if (databaseNames.Contains(dbName) || localDatabases.Contains(dbName))
        {
            Console.WriteLine($"Database '{dbName}' selected.");
            Console.ReadLine();
            return dbName;
        }

        else
        {
            Console.WriteLine("Database not found.");
            Console.ReadLine();
            return null;
        }
    }
    Console.WriteLine("Invalid name.");
    Console.ReadLine();
    return null;
}

void DeleteDatabase(MongoClient client)
{
    Console.Write("Enter the name of the database to delete: ");
    var dbName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(dbName) )
    {
        client.DropDatabase(dbName);

        if(localDatabases.Contains(dbName))
        {
            localDatabases.Remove(dbName);
        }
        Console.WriteLine($"Database '{dbName}' deleted.");
    }

    else
    {
        Console.WriteLine("Invalid name.");
    }
    Console.ReadLine();
}


// Collections methods
void CollectionsMenu(MongoClient client, string selectedDatabaseName)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"Database selected: {selectedDatabaseName}");
        Console.WriteLine("1. Create Collection");
        Console.WriteLine("2. List Collections");
        Console.WriteLine("3. Select Collection");
        Console.WriteLine("4. Edit Collection");
        Console.WriteLine("5. Delete Collection");
        Console.WriteLine("6. Back to Main Menu");
        Console.WriteLine("7. Exit\n");
        Console.Write("Choose an option: ");

        var collectionOption = Console.ReadLine();

        switch (collectionOption)
        {
            case "1":
                CreateCollection(client, selectedDatabaseName);
                break;
            case "2":
                ListCollections(client, selectedDatabaseName);
                break;
            case "3":
                var collectionName = SelectCollection(client, selectedDatabaseName);
                if (collectionName != null)
                {
                    selectedCollectionName = collectionName;
                    DocumentsMenu(client, selectedDatabaseName, selectedCollectionName);
                }
                break;
            case "4":
                EditCollection(client, selectedDatabaseName);
                break;
            case "5":
                DeleteCollection(client, selectedDatabaseName);
                break;
            case "6":
                selectedDatabaseName = string.Empty;
                return;
            case "7":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option");
                Console.ReadLine();
                break;
        }
    }
}


void CreateCollection(MongoClient client, string selectedDatabaseName)
{
    Console.Write("Enter the name of the new collection: ");
    var collectionName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(collectionName))
    {
        var database = client.GetDatabase(selectedDatabaseName);

        if(database.ListCollectionNames().ToList().Contains(collectionName))
        {
            Console.WriteLine($"Collection '{collectionName}' already exists.");
        }
        else
        {
            database.CreateCollection(collectionName);

            if(localDatabases.Contains(selectedDatabaseName))
            {
                localDatabases.Remove(selectedDatabaseName);
            }
            Console.WriteLine($"Collection '{collectionName}' created successfully.");
        }
    }

    else
    {
        Console.WriteLine("Invalid collection name.");
    }
    Console.ReadLine();
}

void ListCollections(MongoClient client, string selectedDatabaseName)
{
    var database = client.GetDatabase(selectedDatabaseName); // Obtém o banco de dados
    var collectionNames = database.ListCollectionNames().ToList();

    if (collectionNames.Count == 0)
    {
        Console.WriteLine("No collections found.");
        Console.ReadLine();
    }

    else
    {
        Console.WriteLine("Collections on the database: ");

        foreach (var name in collectionNames)
        {
            Console.WriteLine(name);
        }

        Console.ReadLine();
    }
}

string? SelectCollection(MongoClient client, string selectedDatabaseName)
{
    var database = client.GetDatabase(selectedDatabaseName);
    var collectionNamesList = database.ListCollectionNames().ToList();

    Console.Write("\nEnter the name of the collection to select: ");
    var collectionName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(collectionName))
    {
        if (collectionNamesList.Contains(collectionName))
        {
            Console.WriteLine($"Collection '{collectionName}' selected.");
            Console.ReadLine();
            return collectionName;
        }

        else
        {
            Console.WriteLine("Collection not found.");
            Console.ReadLine();
            return null;
        }
    }
    else
    {
        Console.WriteLine("Invalid name.");
        Console.ReadLine();
        return null;
    }	
}

void EditCollection(MongoClient client, string selectedDatabaseName)
{
    var database = client.GetDatabase(selectedDatabaseName);
    var collectionNames = database.ListCollectionNames().ToList();

    Console.Write("\nEnter the name of the collection: ");
    var oldName = Console.ReadLine();

    Console.Write("Enter the new name of the collection: ");
    var newName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(oldName) && !string.IsNullOrWhiteSpace(newName))
    {

        if (collectionNames.Contains(oldName) && !collectionNames.Contains(newName))
        {
            database.RenameCollection(oldName, newName);
            Console.WriteLine($"Collection '{oldName}' renamed to '{newName}'.");
        }

        else
        {
            Console.WriteLine("Collection not found or new name already exists.");
        }
    }

    else
    {
        Console.WriteLine("Invalid name.");
    }
    Console.ReadLine();
}

void DeleteCollection(MongoClient client, string selectedDatabaseName)
{
    Console.Write("Enter the name of the collection to delete: ");
    var collectionName = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(collectionName))
    {
        var database = client.GetDatabase(selectedDatabaseName);

        if (database.ListCollectionNames().ToList().Contains(collectionName))
        {
            database.DropCollection(collectionName);
            Console.WriteLine($"Collection '{collectionName}' deleted.");
        }

        else
        {
            Console.WriteLine("Collection not found.");
        }
    }

    else
    {
        Console.WriteLine("Invalid name.");
    }
    Console.ReadLine();
}


// Document methods
void DocumentsMenu(MongoClient client, string selectedDatabaseName, string selectedCollectionName)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"Database selected: {selectedDatabaseName}");
        Console.WriteLine($"Collection selected: {selectedCollectionName}");
        Console.WriteLine("1. Create Document");
        Console.WriteLine("2. List Documents");
        Console.WriteLine("3. Edit Document");
        Console.WriteLine("4. Delete Collection");
        Console.WriteLine("5. Return to previous menu");
        Console.WriteLine("6. Exit\n");
        Console.Write("Choose an option: ");

        var collectionOption = Console.ReadLine();

        switch (collectionOption)
        {
            case "1":
                CreateDocument(client, selectedDatabaseName, selectedCollectionName);
                break;
            case "2":
                ListDocuments(client, selectedDatabaseName, selectedCollectionName);
                break;
            case "3":
                EditDocument(client, selectedDatabaseName, selectedCollectionName);
                break;
            case "4":
                DeleteDocument(client, selectedDatabaseName, selectedCollectionName);
                break;
            case "5":
                selectedCollectionName = string.Empty;
                return;
            case "6":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option");
                Console.ReadLine();
                break;
        }
    }
}

void CreateDocument(MongoClient client, string selectedDatabaseName, string selectedCollectionName)
{
    var database = client.GetDatabase(selectedDatabaseName);

    Console.WriteLine("Enter the document data in JSON format (e.g., {\"key1\": \"value1\", \"key2\": \"value2\", ...}):");
    var jsonData = Console.ReadLine();
    
    if (!string.IsNullOrWhiteSpace(jsonData))
    {
        var collection = database.GetCollection<BsonDocument>(selectedCollectionName);
        var document = BsonDocument.Parse(jsonData);
        collection.InsertOne(document);
        Console.WriteLine("Document created successfully.");
    }
    else
    {
        Console.WriteLine("Invalid data.");
    }
    Console.ReadLine();
}

void ListDocuments(MongoClient client, string selectedDatabaseName, string selectedCollectionName)
{
    var database = client.GetDatabase(selectedDatabaseName);
    var collection = database.GetCollection<BsonDocument>(selectedCollectionName);
    var documents = collection.Find(FilterDefinition<BsonDocument>.Empty).ToList();

    if (documents.Count != 0)
    {
        Console.WriteLine($"Documents in the collection '{selectedCollectionName}':");
        foreach (var document in documents)
        {   
            Console.WriteLine(document.ToJson(new JsonWriterSettings { Indent = true }));
        }
    }

    else
    {
        Console.WriteLine("No documents found.");
        Console.ReadLine();
    }   
    Console.ReadLine();
}

void EditDocument(MongoClient client, string selectedDatabaseName, string selectedCollectionName)
{
    var database = client.GetDatabase(selectedDatabaseName);
    var collection = database.GetCollection<BsonDocument>(selectedCollectionName);
    
    Console.WriteLine("\nChoose how to find the document to edit:");
    Console.WriteLine("1. Find by ID");
    Console.WriteLine("2. Find by Key:Value");
    Console.WriteLine("3. Return to previous menu\n");

    Console.Write("Choose an option: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            EditDocumentById(collection);
            break;

        case "2":
            EditDocumentByKeyValue(collection);
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Invalid option");
            Console.ReadLine();
            break;
    }
    Console.ReadLine();
}

void EditDocumentById(IMongoCollection<BsonDocument> collection)
{
    Console.Write("Enter the ID of the document to edit (_id): ");
    var idInput = Console.ReadLine();

    if(!string.IsNullOrWhiteSpace(idInput))
    {
        var objectId = new ObjectId(idInput);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
        var existingDocument = collection.Find(filter).FirstOrDefault();

        if (existingDocument != null)
        {
            Console.WriteLine("Enter the new data in JSON format {\"key1\": \"value1\", \"key2\": \"value2\", ...}:");
            var jsonData = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                var update = new BsonDocument("$set", BsonDocument.Parse(jsonData));
                collection.UpdateOne(filter, update);
                Console.WriteLine("Document updated.");
            }
            else
            {
                Console.WriteLine("Invalid data.");
            }
        }
        else
        {
            Console.WriteLine("Document not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
    Console.ReadLine();
}

void EditDocumentByKeyValue(IMongoCollection<BsonDocument> collection)
{
    throw new NotImplementedException();
}

void DeleteDocument(MongoClient client, string selectedDatabaseName, string selectedCollectionName)
{
    var database = client.GetDatabase(selectedDatabaseName);
    var collection = database.GetCollection<BsonDocument>(selectedCollectionName);
    
    Console.WriteLine("\nChoose how to find the document to delete:");
    Console.WriteLine("1. Find by ID");
    Console.WriteLine("2. Find by Key:Value");
    Console.WriteLine("3. Return to previous menu\n");

    Console.Write("Choose an option: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            DeleteDocumentById(collection);
            break;

        case "2":
            DeleteDocumentByKeyValue(collection);
            break;

        case "3":
            return;

        default:
            Console.WriteLine("Invalid option");
            Console.ReadLine();
            break;
    }
    Console.ReadLine();
}

void DeleteDocumentById(IMongoCollection<BsonDocument> collection)
{
    Console.Write("Enter the ID of the document to delete (_id): ");
    var idInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(idInput))
    {
        var objectId = new ObjectId(idInput);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
        var existingDocument = collection.Find(filter).FirstOrDefault();
        if (existingDocument != null)
        {
            collection.DeleteOne(filter);
            Console.WriteLine("Document deleted.");
        }
        else
        {
            Console.WriteLine("Document not found.");
        }    
    }
    else
    {
        Console.WriteLine("Invalid ID.");
    }
    Console.ReadLine();
}

void DeleteDocumentByKeyValue(IMongoCollection<BsonDocument> collection)
{
    Console.WriteLine("Enter the filter in JSON format {\"nome\": \"valor\"}:");
    var jsonValue = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(jsonValue))
    {
        var filter = BsonDocument.Parse(jsonValue);
        var existingDocument = collection.Find(filter).FirstOrDefault();
        
        if(existingDocument != null)
        {
            collection.DeleteOne(filter);
            Console.WriteLine("Document deleted.");
        }
        else
        {
            Console.WriteLine("Document not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid key or value.");
    }
}