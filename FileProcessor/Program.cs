using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileProcessor
{
    class Program
    {

        static void Main(string[] args)
        {
            var startTime = DateTime.Now;

            var allCsvRecords = File.ReadAllLines("../../../../../Storage/in.csv")
                .Skip(1)
                .Select(line => Record.ConvertCSV(line))
                .ToList();



            var stores = ReadStorewise(allCsvRecords);

            foreach (Store store in stores)
            {
                WriteStoreToFile(store);
            }

            Console.WriteLine("Total Items Processed - " + allCsvRecords.Count());
            Console.WriteLine("Processing Time (in milliSeconds)- " + DateTime.Now.Subtract(startTime).TotalMilliseconds);
        }

        /// <summary>
        /// This function writes to csv files.
        /// </summary>
        /// <param name="store"></param>
        private static void WriteStoreToFile(Store store)
        {
            StreamWriter file = new 
                StreamWriter("../../../../../Storage/Store" + DateTime.Now.Ticks.ToString() + store.Id + ".csv", false);
            foreach(Item item in store.Items)
            {
                file.WriteLine(item.ConvertToCSVString());

            }
            file.Close();
        }

        /// <summary>
        /// This function reads the records from csv and stores items against each store.
        /// </summary>
        /// <param name="allCsvRecords"></param>
        /// <returns></returns>
        private static List<Store> ReadStorewise(List<Record> allCsvRecords)
        {
            var stores = new List<Store>();
            foreach (Record record in allCsvRecords)
            {
                Store store;
                //check if store already exists
                //yes, then add the item to store
                //no, create new store and add item
                if (stores.Where(s => s.Id == record.StoreId).Count() <= 0)
                {
                    store = new Store();
                    stores.Add(store);
                    store.Id = record.StoreId;
                }
                else
                {
                    store = stores.Where(s => s.Id == record.StoreId).FirstOrDefault();
                }

                Item item = new Item()
                {
                    Price = record.Price,
                    SKU = record.SKU
                };

                store.AddItem(item);

            }
            return stores;
        }
    }

    class Store
    {
        public string Id { get; set; }
        public List<Item> Items { get; set; }

        public Store()
        {
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            item.DiscountedPrice = item.Price * 0.95;
            Items.Add(item);
        }
    }


    class Item
    {
        public string SKU { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }

        public string ConvertToCSVString()
        {
            return SKU + "," + DiscountedPrice;
        }
    }
    class Record
    {

        public string StoreId { get; set; }
        public string SKU { get; set; }
        public double Price { get; set; }
        public string InternalId { get; set; }
        public string ExternalId { get; set; }

        public double CostPrice { get; set; }
        int GST;
        public static Record ConvertCSV(string line)
        {
            string[] fields = line.Split(",");
            Record record = new Record
            {
                StoreId = fields[0],
                SKU = fields[1],
                Price = Convert.ToDouble(fields[2]),
                InternalId = fields[3],
                ExternalId = fields[4],
                CostPrice = Convert.ToDouble(fields[5]),
                GST = Convert.ToInt32(fields[6])
            };

            return record;
        }
}
}
