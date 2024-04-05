﻿using LagerhotellAPI.Models.FrontendModels.Custom;

namespace Lagerhotell.Services;

public class LagerhotellXService
{
    private readonly HttpClient client = new();
    private readonly string _baseUrl = "https://localhost:7272/locations";

    public (string, string) CalculateTime(string openingTime, string closingTime, WarehouseHotel relevantWarehouseHotel)
    {
        string newOpeningTime = openingTime;
        string newClosingTime = closingTime;

        if (newOpeningTime.Length == 1)
        {
            newOpeningTime = "0" + newOpeningTime;
        }
        if (newClosingTime.Length == 1)
        {
            newClosingTime = newClosingTime.Insert(0, "0");
            newClosingTime = "0" + newClosingTime;
        }
        if (newOpeningTime.Length == 2 && !(relevantWarehouseHotel.OpeningHours.Opens % 1 != 0))
        {
            newOpeningTime += ":00";
        }
        if (newOpeningTime.Length == 2 && relevantWarehouseHotel.OpeningHours.Opens % 1 != 0)
        {
            newOpeningTime += ":30";
        }
        if (newOpeningTime.Length == 3 && relevantWarehouseHotel.OpeningHours.Opens % 1 != 0)
        {
            newOpeningTime = newOpeningTime.Insert(0, "0");
            newOpeningTime = newOpeningTime.Insert(2, ":");
            newOpeningTime = newOpeningTime.Remove(newOpeningTime.Length - 2);
            newOpeningTime += "30";
        }
        if (newClosingTime.Length == 2 && !(relevantWarehouseHotel.OpeningHours.Closes % 1 != 0))
        {
            newClosingTime += ":00";
        }
        if (newClosingTime.Length == 2 && relevantWarehouseHotel.OpeningHours.Closes % 1 != 0)
        {
            newClosingTime += ":30";
        }
        if (newClosingTime.Length == 4 && relevantWarehouseHotel.OpeningHours.Closes % 1 != 0)
        {
            newClosingTime = newClosingTime.Insert(2, ":");
            newClosingTime = newClosingTime.Remove(newClosingTime.Length - 2);
            newClosingTime += "30";
        }

        return (newOpeningTime, newClosingTime);
    }


    /// <summary>
    /// Returns a list of the unique area sizes of the provided list of storage units
    /// </summary>
    /// <param name="storageUnits"></param>
    /// <returns></returns>
    public async Task<List<StorageUnitSize>> GetUniqueStorageUnitAreas(List<StorageUnit> storageUnits)
    {
        List<StorageUnitSize> storageUnitSizes = new();
        foreach (var storageUnit in storageUnits)
        {
            if (!storageUnitSizes.Any(storageUnitSize => storageUnitSize.Area == storageUnit.Dimensions.Area))
            {
                storageUnitSizes.Add(new StorageUnitSize
                {
                    Area = storageUnit.Dimensions.Area,
                    Volume = storageUnit.Dimensions.Volume,
                    Price = storageUnit.PricePerMonth,
                    Temperated = storageUnit.Temperated
                });
            }
            else if (storageUnitSizes.Any(storageUnitSize => storageUnitSize.Area == storageUnit.Dimensions.Area && storageUnitSize.Temperated != storageUnit.Temperated))
            {
                storageUnitSizes.Add(new StorageUnitSize
                {
                    Area = storageUnit.Dimensions.Area,
                    Volume = storageUnit.Dimensions.Volume,
                    Price = storageUnit.PricePerMonth,
                    Temperated = storageUnit.Temperated
                });
            }
        }

        storageUnitSizes.Sort((x, y) => x.Area.Value.CompareTo(y.Area.Value));
        return storageUnitSizes;
    }
}
