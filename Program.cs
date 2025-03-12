using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

 

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var SalesSummaryReportPath = Path.Combine(currentDirectory, "salesSummary.txt");

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateSalesSummary(salesFiles, SalesSummaryReportPath, salesTotal);
Console.WriteLine("Sales summary report generated successfully");

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}


void GenerateSalesSummary(IEnumerable<string> salesFile, string reportFilePath, double salesTotal)
{
    List<string> reportLines = new List<string>(); 

    reportLines.Add("Sales Summary");
    reportLines.Add("......................");

    foreach(var file in salesFile)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double fileSales = data?.Total ?? 0;

        reportLines.Add($" {Path.GetFileName(file)}: ${fileSales:F2}");
    }

    reportLines.Add($"\n Total Sales: ${salesTotal:F2}");

    File.WriteAllLines(reportFilePath, reportLines);
}

record SalesData (double Total);