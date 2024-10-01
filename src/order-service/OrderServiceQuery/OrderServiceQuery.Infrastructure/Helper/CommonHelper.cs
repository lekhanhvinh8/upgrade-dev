using Newtonsoft.Json.Linq;
using OfficeOpenXml;

namespace OrderServiceQuery.Infrastructure.CommonHelper
{
    public static class CommonHelper
    {
        // Method to read data from a JSON file based on a key (e.g., ExtensionTelephone)
        public static T ReadDataByKey<T>(string filePath, string key)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            if (!jObject.ContainsKey(key))
                throw new ArgumentException($"Key '{key}' not found in the JSON file");

            var token = jObject[key];
            return token.ToObject<T>();
        }

        // Method to read data from an Excel file into a list of DTOs
        public static List<T> ReadExcelData<T>(string filePath) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found", filePath);

            var resultList = new List<T>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                for (int row = 2; row <= rowCount; row++) // Assuming first row is the header
                {
                    var instance = new T();
                    var properties = typeof(T).GetProperties();

                    for (int col = 1; col <= colCount; col++)
                    {
                        var prop = properties[col - 1];
                        var cellValue = worksheet.Cells[row, col].Text;
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            prop.SetValue(instance, Convert.ChangeType(cellValue, prop.PropertyType));
                        }
                    }

                    resultList.Add(instance);
                }
            }

            return resultList;
        }

        // Method to write data to an Excel file from a list of DTOs
        public static void WriteExcelData<T>(string filePath, List<T> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var properties = typeof(T).GetProperties();

                // Write header
                for (int col = 1; col <= properties.Length; col++)
                {
                    worksheet.Cells[1, col].Value = properties[col - 1].Name;
                }

                // Write data
                for (int row = 0; row < data.Count; row++)
                {
                    var item = data[row];
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(item)?.ToString();
                    }
                }

                // Save the file
                var file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }

        // Common method to format DateTime based on the provided format
        public static string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        // Method to format DateTime to a specific common format (e.g., ISO 8601)
        public static string ToIso8601(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        // Method to format DateTime in a short date format
        public static string ToShortDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        // Method to format DateTime with a time component (HH:mm:ss)
        public static string ToTimeOnly(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        // Method to format DateTime in a full date and time format
        public static string ToFullDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }
    }
}