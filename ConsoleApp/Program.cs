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
        Console.WriteLine("4. Delete Collection");
        Console.WriteLine("5. Back to Main Menu");
        Console.WriteLine("6. Exit\n");
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
                DeleteCollection(client, selectedDatabaseName);
                break;
            case "5":
                selectedDatabaseName = string.Empty;
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
    throw new NotImplementedException();
}

void InsertDocument(MongoClient client)
{
    throw new NotImplementedException();
}

void ListDocuments(MongoClient client)
{
    throw new NotImplementedException();
}

void EditDocument(MongoClient client)
{
    throw new NotImplementedException();
}

void DeleteDocument(MongoClient client)
{
    throw new NotImplementedException();
}