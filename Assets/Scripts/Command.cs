using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public interface ICommand
{
    void Execute();
    string Undo();
}

public class ReadDBCommand : ICommand
{
    IDbConnection dbConnection = new SqliteConnection("URI=file:" + Application.dataPath + "/StreamingAssets/scores.db");
    public void Execute()
    {
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM Scores;";
        dbCommand.CommandText = sqlQuery;
        IDataReader reader = dbCommand.ExecuteReader();
        MetaSceneInformation.names = new List<string>();
        MetaSceneInformation.levels = new List<int>();
        while (reader.Read())
        {
            string name = reader.GetString(1);
            MetaSceneInformation.names.Add(name);
            int level = reader.GetInt32(2);
            MetaSceneInformation.levels.Add(level);
        }
        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    public string Undo()
    {
        return "DB reading has been undone.";
    }
}

public class WriteRowCommand : ICommand
{
    DateTime currentTime = DateTime.Now;
    IDbConnection dbConnection = new SqliteConnection("URI=file:" + Application.dataPath + "/StreamingAssets/scores.db");
    public void Execute()
    {
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = string.Format("INSERT INTO Scores (NAME, LEVEL) VALUES ('{0}', {1});",
            MetaSceneInformation.PlayerName,
            MetaSceneInformation.Level);
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();
        dbCommand.Dispose();
        dbConnection.Close();
    }

    public string Undo()
    {
        return "Writing has been undone.";
    }
}

public class CommandInvoker
{
    private Queue<ICommand> commands_ = new Queue<ICommand>();

    public void SetCommand(ICommand command)
    {
        commands_.Enqueue(command);
    }

    public void Run()
    {
        commands_.Dequeue().Execute();
    }

    public void RunAll()
    {
        while (commands_.Count > 0)
        {
            commands_.Dequeue().Execute();
        }
    }

    public void Cancel()
    {
        commands_.Dequeue().Undo();
    }
    public void CancelAll()
    {
        while (commands_.Count > 0)
        {
            commands_.Dequeue().Undo();
        }
    }
}