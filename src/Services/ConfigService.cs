namespace Services;

using Microsoft.Extensions.Options;
using System.Globalization;
using System.Data.SQLite;

using CsvHelper;
using Dapper;

using Interfaces;
using Models;
using Helpers;

public class ConfigService : IConfigService
{

    private readonly ILogger<ConfigService> _logger;
    private readonly SQLiteConnection _sqliteConn;
    private readonly DatabaseConfiguration _config;

    public ConfigService(IOptions<DatabaseConfiguration> config, ILogger<ConfigService> logger)
    {
        _logger = logger;
        _config = config.Value;

        _sqliteConn = createConnection();

        // createTable();

        // insertData();

        _logger.LogInformation("ConfigService constructor complete");

    }
    public async Task<IEnumerable<Config>> GetConfigAsync()
    {
        var sql = "SELECT id, source, category, name, value FROM config";

        var config = await _sqliteConn.QueryAsync<Config>(sql);

        return config.ToList();

        // foreach (var item in config)
        // {
        //     yield return item;
        // }

    }

    private SQLiteConnection createConnection()
    {

        SQLiteConnection sqlite_conn;

        sqlite_conn = new SQLiteConnection("Data Source = ..data\\caelum.db; Version = 3; New = True; Compress = True; ");
        try
        {
            sqlite_conn.Open();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating connection : {ex.Message}");

        }

        _logger.LogInformation("Database Open");
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

        _logger.LogInformation("Table Created");

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

    private IEnumerable<Config> readFromCSV()
    {
        using var reader = new StreamReader(_config.DataLocation);
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