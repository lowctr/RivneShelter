using System.Xml.Linq;
using RivneSheltersProject.Data;
using RivneSheltersProject.Models;
using Microsoft.EntityFrameworkCore;

namespace RivneSheltersProject.Services;

public static class KmlDataSeeder
{
    public static async Task SeedFromKmlAsync(ApplicationDbContext context, string kmlFilePath)
    {
        // Only seed if database is empty
        if (await context.Locations.AnyAsync())
        {
            return;
        }

        if (!File.Exists(kmlFilePath))
        {
            Console.WriteLine($"KML file not found: {kmlFilePath}");
            return;
        }

        var locations = ParseKmlFile(kmlFilePath);

        if (locations.Count > 0)
        {
            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {locations.Count} locations from KML file.");
        }
    }

    private static List<Location> ParseKmlFile(string kmlFilePath)
    {
        var locations = new List<Location>();

        XNamespace kmlNs = "http://www.opengis.net/kml/2.2";
        var doc = XDocument.Load(kmlFilePath);

        var placemarks = doc.Descendants(kmlNs + "Placemark");

        foreach (var placemark in placemarks)
        {
            var extendedData = placemark.Element(kmlNs + "ExtendedData");
            var coordinates = placemark.Descendants(kmlNs + "coordinates").FirstOrDefault()?.Value.Trim();

            if (extendedData == null || string.IsNullOrEmpty(coordinates))
                continue;

            var location = new Location();

            // Parse ExtendedData fields
            foreach (var data in extendedData.Elements(kmlNs + "Data"))
            {
                var name = data.Attribute("name")?.Value ?? "";
                var value = data.Element(kmlNs + "value")?.Value ?? "";

                switch (name)
                {
                    case "Розташування":
                        location.Address = value;
                        break;
                    case "Форма власності":
                        location.Ownership = value;
                        break;
                    case "Балансоутримувач":
                        location.Holder = value;
                        break;
                    case "Наявність пристосування для доступу осіб з інвалідністю та інших маломобільних груп населення (пандусу)":
                        location.Accessibility = value;
                        break;
                    case "Контакт відповідального за ключі ":
                        location.Contact = value;
                        break;
                    case "Номер":
                        location.Number = value;
                        break;
                }
            }

            // Parse coordinates (format: longitude,latitude,altitude)
            var coordParts = coordinates.Split(',');
            if (coordParts.Length >= 2)
            {
                if (double.TryParse(coordParts[0].Trim(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var longitude))
                {
                    location.Longitude = longitude;
                }
                if (double.TryParse(coordParts[1].Trim(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var latitude))
                {
                    location.Latitude = latitude;
                }
            }

            locations.Add(location);
        }

        return locations;
    }
}
