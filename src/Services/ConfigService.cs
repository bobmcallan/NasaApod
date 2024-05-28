namespace Services;

using System.Globalization;
using CsvHelper;
using System.Data.SQLite;

using Interfaces;
using Models;

public class ConfigService : IConfigService
{

    private readonly ILogger<ConfigService> _logger;
    private readonly SQLiteConnection _sqliteConn;

    private const string _filename = "..data\\config.csv";


    public ConfigService(ILogger<ConfigService> logger)
    {
        _logger = logger;

        _sqliteConn = createConnection();

        createTable();

        insertData();

    }
    public async IEnumerable<Config> GetConfig()
    {
        SQLiteDataReader sqlite_datareader;

        SQLiteCommand sqlite_cmd = _sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = "SELECT id, source, category, name, value FROM config"; ;

        sqlite_datareader = sqlite_cmd.ExecuteReader();
        while (sqlite_datareader.Read())
        {
            string myreader = sqlite_datareader.GetFieldValue<string>(0);
            Console.WriteLine(myreader);
            yield return new Config
            {
                Source = source,
                Category = category,
                Name = name,
                Value = value
            };
        }

    }

    private SQLiteConnection createConnection()
    {

        SQLiteConnection sqlite_conn;

        sqlite_conn = new SQLiteConnection("Data Source = caelum.db; Version = 3; New = True; Compress = True; ");
        try
        {
            sqlite_conn.Open();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating connection : {ex.Message}");

        }
        return sqlite_conn;
    }

    private void createTable()
    {


        string createSQL = @"CREATE TABLE Config (
            id          INTEGER     PRIMARY KEY AUTOINCREMENT,
            source      VARCHAR(55), 
            category    VARCHAR(55), 
            name        TEXT, 
            value       VARCHAR(255)
        )";

        SQLiteCommand sqlite_cmd = _sqliteConn.CreateCommand();
        sqlite_cmd.CommandText = createSQL;
        sqlite_cmd.ExecuteNonQuery();

    }

    private void insertData()
    {

        var sql = "INSERT INTO config(source, category, name, value) VALUES (@source, @category, @name, @value)";

        foreach (var config in readFromCSV())
        {
            using var command = new SQLiteCommand(sql, _sqliteConn);
            command.Parameters.AddWithValue("@source", config.Source);
            command.Parameters.AddWithValue("@category", config.Category);
            command.Parameters.AddWithValue("@name", config.Name);
            command.Parameters.AddWithValue("@value", config.Value);

            command.ExecuteNonQuery();
        }

    }

    private static IEnumerable<Config> readFromCSV()
    {
        using var reader = new StreamReader(_filename);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Skip header of the csv file
        csv.Read();

        // Read the header of the csv file to map to fields
        csv.ReadHeader();

        while (csv.Read())
        {
            var source = csv.GetField<string>("SOURCE");
            var category = csv.GetField<string>("CATEGORY");
            var name = csv.GetField<string>("NAME");
            var value = csv.GetField<string>("VALUE");

            yield return new Config
            {
                Source = source,
                Category = category,
                Name = name,
                Value = value
            };
        }
    }
}