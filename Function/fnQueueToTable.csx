using System;

public static void Run(Feedback myQueueItem, ICollector<Feedback> tableBinding, ILogger log)
{

    myQueueItem.PartitionKey = myQueueItem.Session;
    myQueueItem.RowKey = myQueueItem.Id;

    tableBinding.Add(myQueueItem);
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem.Issue} in Session {myQueueItem.Session} at {myQueueItem.CreatedAt.ToShortTimeString()}");
}

public class Feedback
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Id { get; set; }
    public string Issue { get; set; }
    public string Session { get; set; }
    public bool Complete { get; set; }
    public DateTime CreatedAt { get; set; }
}